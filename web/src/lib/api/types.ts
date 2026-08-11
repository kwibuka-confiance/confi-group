/**
 * Wire contracts shared with the ConfiOS backend.
 *
 * Success responses are wrapped in an envelope; failures carry a stable
 * machine-readable code plus per-field validation codes.
 */
export interface ApiResponse<T> {
  data: T;
  meta: unknown | null;
  traceId: string;
}

export interface ApiErrorBody {
  code: string;
  message: string;
  details: { failures?: Record<string, string[]> } | null;
  traceId: string;
}

/** Codes the client reasons about directly; everything else uses the server message. */
export const ApiErrorCodes = {
  validationFailed: 'VALIDATION_FAILED',
  invalidCredentials: 'INVALID_CREDENTIALS',
  tenantSlugTaken: 'TENANT_SLUG_TAKEN',
  network: 'NETWORK',
  unauthorized: 'UNAUTHORIZED',
} as const;

export class ApiError extends Error {
  constructor(
    readonly code: string,
    message: string,
    readonly status: number,
    readonly fieldErrors: Record<string, string[]> = {},
    readonly traceId?: string,
  ) {
    super(message);
    this.name = 'ApiError';
  }

  get isUnauthorized(): boolean {
    return this.status === 401;
  }
}

/** A way of counting a product: a crate of 24, a box of 100. */
export interface Packaging {
  unitCode: string;
  quantityInBaseUnit: number;
  sellingPriceAmount: number;
  costAmount: number | null;
  barcode: string | null;
  /**
   * False when a quarter would not come to a whole base unit, so the client hides
   * the quarter option rather than offering a sale the API will reject.
   */
  allowsQuarters: boolean;
}

export type TaxClass = 'Standard' | 'Zero' | 'Exempt';

export interface Product {
  id: string;
  name: string;
  sku: string;
  description: string | null;
  priceAmount: number;
  costAmount: number | null;
  currencyCode: string;
  /** Unit stock is counted in, for example BOTTLE. */
  baseUnitCode: string;
  taxClass: TaxClass;
  /** Whether the base unit is exchanged with the customer rather than sold outright. */
  isReturnable: boolean;
  depositAmount: number | null;
  packagings: Packaging[];
  isActive: boolean;
}

/** A business offered when credentials unlock more than one. */
export interface BusinessSummary {
  tenantId: string;
  name: string;
  slug: string;
}

/**
 * Sign-in returns one of two shapes, told apart by `status`: a session, or the
 * businesses to choose between.
 */
export interface SignInResponse {
  status: 'authenticated' | 'select_business';
  accessToken: string | null;
  expiresAt: string | null;
  userId: string | null;
  tenantId: string | null;
  businessName: string | null;
  fullName: string | null;
  email: string | null;
  permissions: string[] | null;
  selectionToken: string | null;
  businesses: BusinessSummary[] | null;
}

export interface Session {
  accessToken: string;
  expiresAt: string;
  userId: string;
  tenantId: string;
  businessName: string;
  fullName: string;
  email: string;
  permissions: string[];
}
