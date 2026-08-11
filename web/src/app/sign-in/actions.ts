'use server';

import { redirect } from 'next/navigation';

import { ApiError, type BusinessSummary } from '@/lib/api/types';
import { login, selectBusiness, toSession, writeSession } from '@/lib/auth/session';
import { getDictionary } from '@/lib/i18n/dictionaries';
import { readLocale } from '@/lib/i18n/locale';

export interface SignInState {
  error?: string;
  /**
   * Present when the credentials unlocked more than one business. The token
   * carries the completed password check, so choosing does not re-ask for it.
   */
  choice?: {
    selectionToken: string;
    businesses: BusinessSummary[];
  };
}

/**
 * Signs in against the ConfiOS API and stores the session in an httpOnly cookie.
 * Credentials never reach client-side JavaScript.
 *
 * A person may hold accounts at several businesses, so this either signs in
 * directly or hands back the businesses to choose between.
 */
export async function signInAction(
  _previous: SignInState,
  formData: FormData,
): Promise<SignInState> {
  const locale = await readLocale();
  const dict = getDictionary(locale);

  const email = String(formData.get('email') ?? '').trim();
  const password = String(formData.get('password') ?? '');

  if (!email || !password) {
    return { error: dict.errors.required };
  }

  try {
    const response = await login({ email, password }, locale);

    if (response.status === 'select_business') {
      return {
        choice: {
          selectionToken: response.selectionToken ?? '',
          businesses: response.businesses ?? [],
        },
      };
    }

    const session = toSession(response);
    if (!session) {
      return { error: dict.errors.unexpected };
    }
    await writeSession(session);
  } catch (error) {
    if (error instanceof ApiError) {
      // The API already returns a localised, non-enumerating message.
      return { error: error.message };
    }
    return { error: dict.errors.unexpected };
  }

  redirect('/');
}

/** Completes a sign-in once the person has picked which business to open. */
export async function selectBusinessAction(
  _previous: SignInState,
  formData: FormData,
): Promise<SignInState> {
  const locale = await readLocale();
  const dict = getDictionary(locale);

  const selectionToken = String(formData.get('selectionToken') ?? '');
  const tenantId = String(formData.get('tenantId') ?? '');

  if (!selectionToken || !tenantId) {
    return { error: dict.errors.unexpected };
  }

  try {
    const response = await selectBusiness(selectionToken, tenantId, locale);
    const session = toSession(response);
    if (!session) {
      return { error: dict.errors.unexpected };
    }
    await writeSession(session);
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message };
    }
    return { error: dict.errors.unexpected };
  }

  redirect('/');
}
