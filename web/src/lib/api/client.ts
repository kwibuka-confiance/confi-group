import { ApiError, ApiErrorCodes, type ApiErrorBody, type ApiResponse } from './types';

const BASE_URL = process.env.API_BASE_URL ?? 'http://localhost:5080';

interface RequestOptions {
  method?: 'GET' | 'POST';
  body?: unknown;
  /** Bearer token for authenticated calls. */
  token?: string;
  /** Forwarded as Accept-Language so the backend localises its errors. */
  locale?: string;
  /** Server components opt out of caching for tenant data by default. */
  cache?: RequestCache;
}

/**
 * Calls the ConfiOS API and unwraps its envelope.
 *
 * Transport and envelope handling live here so callers only ever see typed data
 * or an {@link ApiError}.
 */
export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = 'GET', body, token, locale, cache = 'no-store' } = options;

  let response: Response;
  try {
    response = await fetch(`${BASE_URL}${path}`, {
      method,
      cache,
      headers: {
        Accept: 'application/json',
        ...(body ? { 'Content-Type': 'application/json' } : {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...(locale ? { 'Accept-Language': locale } : {}),
      },
      body: body ? JSON.stringify(body) : undefined,
    });
  } catch (cause) {
    throw new ApiError(
      ApiErrorCodes.network,
      cause instanceof Error ? cause.message : 'Network error',
      0,
    );
  }

  const payload: unknown = await response.json().catch(() => null);

  if (!response.ok) {
    const error = (payload ?? {}) as Partial<ApiErrorBody>;
    throw new ApiError(
      error.code ?? 'UNKNOWN_ERROR',
      error.message ?? response.statusText,
      response.status,
      error.details?.failures ?? {},
      error.traceId,
    );
  }

  return (payload as ApiResponse<T>).data;
}
