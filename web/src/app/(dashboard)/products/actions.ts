'use server';

import { revalidatePath } from 'next/cache';

import {
  createProduct,
  setProductStatus,
  updateProduct,
  type CreatePackagingInput,
  type UpdateProductInput,
} from '@/lib/api/catalog';
import { ApiError } from '@/lib/api/types';
import { readSession } from '@/lib/auth/session';
import { getDictionary } from '@/lib/i18n/dictionaries';
import { readLocale } from '@/lib/i18n/locale';

export interface CreateProductState {
  ok?: boolean;
  error?: string;
  /** Per-field codes from the backend, keyed by its PascalCase field names. */
  fieldErrors?: Record<string, string[]>;
}

/** Reads a form value, returning null when it is blank. */
function text(formData: FormData, key: string): string | null {
  const value = String(formData.get(key) ?? '').trim();
  return value.length > 0 ? value : null;
}

/** Reads a numeric form value, returning null when blank and NaN when unparseable. */
function number(formData: FormData, key: string): number | null {
  const raw = text(formData, key);
  return raw === null ? null : Number(raw);
}

/**
 * Packaging rows arrive as parallel arrays, one entry per row the user added.
 * Rows with no unit code are ignored, so an empty row left behind is harmless.
 */
function readPackagings(formData: FormData): CreatePackagingInput[] {
  const units = formData.getAll('packagingUnit').map((value) => String(value).trim());
  const quantities = formData.getAll('packagingQuantity').map((value) => String(value).trim());
  const prices = formData.getAll('packagingPrice').map((value) => String(value).trim());
  const costs = formData.getAll('packagingCost').map((value) => String(value).trim());
  const barcodes = formData.getAll('packagingBarcode').map((value) => String(value).trim());

  return units
    .map((unitCode, index) => ({
      unitCode: unitCode.toUpperCase(),
      quantityInBaseUnit: Number(quantities[index] ?? ''),
      sellingPriceAmount: Number(prices[index] ?? ''),
      costAmount: costs[index] ? Number(costs[index]) : null,
      barcode: barcodes[index] || null,
    }))
    .filter((packaging) => packaging.unitCode.length > 0);
}

/**
 * Reads the product fields common to adding and correcting, or returns the message
 * to show when the form does not carry a usable product.
 */
function readProduct(
  formData: FormData,
  dict: ReturnType<typeof getDictionary>,
): { input: UpdateProductInput } | { error: string } {
  const name = text(formData, 'name');
  const sku = text(formData, 'sku');
  const currencyCode = text(formData, 'currencyCode')?.toUpperCase() ?? '';
  const priceAmount = number(formData, 'priceAmount');
  const packagings = readPackagings(formData);

  if (
    !name ||
    !sku ||
    currencyCode.length !== 3 ||
    priceAmount === null ||
    !Number.isFinite(priceAmount) ||
    priceAmount < 0
  ) {
    return { error: dict.errors.required };
  }

  // A packaging with a missing or unusable quantity or price would be rejected by
  // the API anyway; catching it here keeps the message about the row, not the form.
  const brokenRow = packagings.find(
    (packaging) =>
      !Number.isFinite(packaging.quantityInBaseUnit) ||
      packaging.quantityInBaseUnit <= 0 ||
      !Number.isFinite(packaging.sellingPriceAmount) ||
      packaging.sellingPriceAmount < 0,
  );
  if (brokenRow) {
    return { error: dict.products.packagingInvalid };
  }

  return {
    input: {
      name,
      sku,
      priceAmount,
      currencyCode,
      description: text(formData, 'description'),
      costAmount: number(formData, 'costAmount'),
      taxClass: text(formData, 'taxClass'),
      depositAmount: number(formData, 'depositAmount'),
      packagings,
    },
  };
}

/** The catalog feeds the table and the dashboard figures, so both are refreshed. */
function revalidateCatalog(): void {
  revalidatePath('/products');
  revalidatePath('/');
}

/**
 * Adds a product to the signed-in tenant's catalog.
 *
 * The catalog feeds both the products table and the dashboard figures, so both
 * are revalidated on success rather than leaving the dashboard stale.
 */
export async function createProductAction(
  _previous: CreateProductState,
  formData: FormData,
): Promise<CreateProductState> {
  const locale = await readLocale();
  const dict = getDictionary(locale);

  const session = await readSession();
  if (!session) {
    return { error: dict.errors.unexpected };
  }

  const parsed = readProduct(formData, dict);
  if ('error' in parsed) {
    return parsed;
  }

  try {
    await createProduct(
      session.accessToken,
      { ...parsed.input, baseUnitCode: text(formData, 'baseUnitCode') },
      locale,
    );
  } catch (error) {
    if (error instanceof ApiError) {
      // Covers PRODUCT_SKU_TAKEN, PACKAGING_UNIT_TAKEN and validation alike.
      return { error: error.message, fieldErrors: error.fieldErrors };
    }
    return { error: dict.errors.unexpected };
  }

  revalidateCatalog();

  return { ok: true };
}

/**
 * Corrects a product.
 *
 * The whole record is sent, so a field the person cleared is cleared here too and
 * the packagings left in the form become the complete set. The base unit is not
 * sent: the API refuses to change it, because stock is counted in it.
 */
export async function updateProductAction(
  _previous: CreateProductState,
  formData: FormData,
): Promise<CreateProductState> {
  const locale = await readLocale();
  const dict = getDictionary(locale);

  const session = await readSession();
  if (!session) {
    return { error: dict.errors.unexpected };
  }

  const productId = text(formData, 'productId');
  if (!productId) {
    return { error: dict.errors.unexpected };
  }

  const parsed = readProduct(formData, dict);
  if ('error' in parsed) {
    return parsed;
  }

  try {
    await updateProduct(session.accessToken, productId, parsed.input, locale);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message, fieldErrors: error.fieldErrors };
    }
    return { error: dict.errors.unexpected };
  }

  revalidateCatalog();

  return { ok: true };
}

/**
 * Withdraws a product from sale, or puts it back.
 *
 * Archiving never deletes: the product stays on past orders and can be restored.
 */
export async function setProductStatusAction(formData: FormData): Promise<void> {
  const locale = await readLocale();

  const session = await readSession();
  if (!session) {
    return;
  }

  const productId = String(formData.get('productId') ?? '');
  if (!productId) {
    return;
  }

  const isActive = String(formData.get('isActive')) === 'true';

  try {
    await setProductStatus(session.accessToken, productId, isActive, locale);
  } catch {
    // The table re-renders from the server either way, so a failure shows as the
    // row simply not having changed rather than as a stale optimistic update.
    return;
  }

  revalidateCatalog();
}
