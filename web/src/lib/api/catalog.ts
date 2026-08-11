import { apiRequest } from './client';
import type { Product } from './types';

/** Products for the signed-in tenant. */
export function getProducts(token: string, locale?: string): Promise<Product[]> {
  return apiRequest<Product[]>('/api/v1/products', { token, locale });
}

/** A packaging supplied when a product is created. */
export interface CreatePackagingInput {
  unitCode: string;
  quantityInBaseUnit: number;
  sellingPriceAmount: number;
  costAmount?: number | null;
  barcode?: string | null;
}

/**
 * Everything needed to add a product. Only the first four are required; a product
 * sold as individual items needs nothing else. Every amount is in `currencyCode`.
 */
export interface CreateProductInput {
  name: string;
  sku: string;
  priceAmount: number;
  currencyCode: string;
  baseUnitCode?: string | null;
  description?: string | null;
  costAmount?: number | null;
  taxClass?: string | null;
  /** Supplying a deposit marks the base unit returnable. */
  depositAmount?: number | null;
  packagings?: CreatePackagingInput[];
}

export function createProduct(
  token: string,
  input: CreateProductInput,
  locale?: string,
): Promise<string> {
  return apiRequest<string>('/api/v1/products', {
    method: 'POST',
    body: input,
    token,
    locale,
  });
}

/** Figures the dashboard reports, derived only from what the API returned. */
export interface CatalogSummary {
  total: number;
  active: number;
  /** Value per currency. Amounts in different currencies are never added together. */
  valueByCurrency: Record<string, number>;
  /** Newest first: ids are UUIDv7, so descending id order is creation order. */
  recent: Product[];
}

export function summariseCatalog(products: Product[], recentCount = 5): CatalogSummary {
  const valueByCurrency = products.reduce<Record<string, number>>((totals, product) => {
    totals[product.currencyCode] = (totals[product.currencyCode] ?? 0) + product.priceAmount;
    return totals;
  }, {});

  const recent = [...products].sort((a, b) => b.id.localeCompare(a.id)).slice(0, recentCount);

  return {
    total: products.length,
    active: products.filter((product) => product.isActive).length,
    valueByCurrency,
    recent,
  };
}
