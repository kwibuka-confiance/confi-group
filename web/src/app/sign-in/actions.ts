'use server';

import { redirect } from 'next/navigation';

import { ApiError } from '@/lib/api/types';
import { login, writeSession } from '@/lib/auth/session';
import { defaultLocale, getDictionary } from '@/lib/i18n/dictionaries';

export interface SignInState {
  error?: string;
  fieldErrors?: Record<string, string[]>;
}

/**
 * Signs in against the ConfiOS API and stores the session in an httpOnly cookie.
 * Credentials never reach client-side JavaScript.
 */
export async function signInAction(
  _previous: SignInState,
  formData: FormData,
): Promise<SignInState> {
  const dict = getDictionary();

  const businessHandle = String(formData.get('businessHandle') ?? '').trim();
  const email = String(formData.get('email') ?? '').trim();
  const password = String(formData.get('password') ?? '');

  if (!businessHandle || !email || !password) {
    return { error: dict.errors.required };
  }

  try {
    const session = await login({ businessHandle, email, password }, defaultLocale);
    await writeSession(session);
  } catch (error) {
    if (error instanceof ApiError) {
      // The API already returns a localised, non-enumerating message.
      return { error: error.message, fieldErrors: error.fieldErrors };
    }
    return { error: dict.errors.unexpected };
  }

  redirect('/');
}
