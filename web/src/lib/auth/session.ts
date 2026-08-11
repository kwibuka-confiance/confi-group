import 'server-only';

import { cookies } from 'next/headers';

import { apiRequest } from '@/lib/api/client';
import type { Session, SignInResponse } from '@/lib/api/types';

const SESSION_COOKIE = 'confios_session';

/**
 * The session is held in an httpOnly cookie so the access token is never exposed
 * to client-side JavaScript.
 */
export async function readSession(): Promise<Session | null> {
  const store = await cookies();
  const raw = store.get(SESSION_COOKIE)?.value;
  if (!raw) return null;

  try {
    const session = JSON.parse(raw) as Session;
    if (!session.accessToken || new Date(session.expiresAt) <= new Date()) {
      return null;
    }
    return session;
  } catch {
    // A corrupt cookie degrades to signed out rather than breaking the render.
    return null;
  }
}

export async function writeSession(session: Session): Promise<void> {
  const store = await cookies();
  store.set(SESSION_COOKIE, JSON.stringify(session), {
    httpOnly: true,
    sameSite: 'lax',
    secure: process.env.NODE_ENV === 'production',
    path: '/',
    expires: new Date(session.expiresAt),
  });
}

export async function clearSession(): Promise<void> {
  const store = await cookies();
  store.delete(SESSION_COOKIE);
}

export interface LoginInput {
  email: string;
  password: string;
  /** Optional: skips the chooser when the business is already known. */
  businessHandle?: string;
}

export function login(input: LoginInput, locale?: string): Promise<SignInResponse> {
  return apiRequest<SignInResponse>('/api/v1/auth/login', {
    method: 'POST',
    body: input,
    locale,
  });
}

export function selectBusiness(
  selectionToken: string,
  tenantId: string,
  locale?: string,
): Promise<SignInResponse> {
  return apiRequest<SignInResponse>('/api/v1/auth/select-business', {
    method: 'POST',
    body: { selectionToken, tenantId },
    locale,
  });
}

/** Narrows an authenticated sign-in response into a stored session. */
export function toSession(response: SignInResponse): Session | null {
  if (response.status !== 'authenticated' || !response.accessToken) {
    return null;
  }

  return {
    accessToken: response.accessToken,
    expiresAt: response.expiresAt ?? new Date().toISOString(),
    userId: response.userId ?? '',
    tenantId: response.tenantId ?? '',
    businessName: response.businessName ?? '',
    fullName: response.fullName ?? '',
    email: response.email ?? '',
    permissions: response.permissions ?? [],
  };
}
