'use server';

import { revalidatePath } from 'next/cache';

import { createProduct, type CreatePackagingInput } from '@/lib/api/catalog';
import { ApiError } from '@/lib/api/types';
import { readSession } from '@/lib/auth/session';
import { defaultLocale, getDictionary } from '@/lib/i18n/dictionaries';

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
 * Adds a product to the signed-in tenant's catalog.
 *
 * The catalog feeds both the products table and the dashboard figures, so both
 * are revalidated on success rather than leaving the dashboard stale.
 */
export async function createProductAction(
  _previous: CreateProductState,
  formData: FormData,
): Promise<CreateProductState> {
  const dict = getDictionary();

  const session = await readSession();
  if (!session) {
    return { error: dict.errors.unexpected };
  }

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

  try {
    await createProduct(
      session.accessToken,
      {
        name,
        sku,
        priceAmount,
        currencyCode,
        baseUnitCode: text(formData, 'baseUnitCode'),
        description: text(formData, 'description'),
        costAmount: number(formData, 'costAmount'),
        taxClass: text(formData, 'taxClass'),
        depositAmount: number(formData, 'depositAmount'),
        packagings,
      },
      defaultLocale,
    );
  } catch (error) {
    if (error instanceof ApiError) {
      // Covers PRODUCT_SKU_TAKEN, PACKAGING_UNIT_TAKEN and validation alike.
      return { error: error.message, fieldErrors: error.fieldErrors };
    }
    return { error: dict.errors.unexpected };
  }

  revalidatePath('/products');
  revalidatePath('/');

  return { ok: true };
}
