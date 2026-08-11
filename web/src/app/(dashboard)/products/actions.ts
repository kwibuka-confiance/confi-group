'use server';

import { revalidatePath } from 'next/cache';

import { createProduct } from '@/lib/api/catalog';
import { ApiError } from '@/lib/api/types';
import { readSession } from '@/lib/auth/session';
import { defaultLocale, getDictionary } from '@/lib/i18n/dictionaries';

export interface CreateProductState {
  ok?: boolean;
  error?: string;
  /** Per-field codes from the backend, keyed by its PascalCase field names. */
  fieldErrors?: Record<string, string[]>;
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

  const name = String(formData.get('name') ?? '').trim();
  const sku = String(formData.get('sku') ?? '').trim();
  const currencyCode = String(formData.get('currencyCode') ?? '').trim().toUpperCase();
  const priceAmount = Number(String(formData.get('priceAmount') ?? '').trim());

  if (!name || !sku || currencyCode.length !== 3 || !Number.isFinite(priceAmount) || priceAmount < 0) {
    return { error: dict.errors.required };
  }

  try {
    await createProduct(
      session.accessToken,
      { name, sku, priceAmount, currencyCode },
      defaultLocale,
    );
  } catch (error) {
    if (error instanceof ApiError) {
      // Covers PRODUCT_SKU_TAKEN and per-field validation alike.
      return { error: error.message, fieldErrors: error.fieldErrors };
    }
    return { error: dict.errors.unexpected };
  }

  revalidatePath('/products');
  revalidatePath('/');

  return { ok: true };
}
