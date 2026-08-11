import { apiRequest } from './client';

export interface ProvisionTenantInput {
  name: string;
  slug: string;
  countryCode: string;
  currencyCode: string;
  defaultLanguage: string;
  timeZoneId: string;
  ownerEmail: string;
  ownerFullName: string;
  ownerPassword: string;
  firstBranchName: string;
}

export interface ProvisionTenantResult {
  tenantId: string;
  branchId: string;
  ownerUserId: string;
}

/** Self-service sign-up: creates a business, its first branch and its owner. */
export function provisionTenant(
  input: ProvisionTenantInput,
  locale?: string,
): Promise<ProvisionTenantResult> {
  return apiRequest<ProvisionTenantResult>('/api/v1/tenants', {
    method: 'POST',
    body: input,
    locale,
  });
}

/** Minimum the backend accepts. Mirrored here so the form can say so up front. */
export const MIN_PASSWORD_LENGTH = 12;

/** Turns a business name into a suggested handle. */
export function slugify(value: string): string {
  return value
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '');
}

export const SLUG_PATTERN = /^[a-z0-9]+(-[a-z0-9]+)*$/;
