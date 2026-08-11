import 'server-only';

import { cookies } from 'next/headers';

import { apiRequest } from '@/lib/api/client';
import type { Session } from '@/lib/api/types';

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
  businessHandle: string;
  email: string;
  password: string;
}

export function login(input: LoginInput, locale?: string): Promise<Session> {
  return apiRequest<Session>('/api/v1/auth/login', {
    method: 'POST',
    body: input,
    locale,
  });
}
