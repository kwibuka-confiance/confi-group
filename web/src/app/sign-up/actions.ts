'use server';

import { redirect } from 'next/navigation';

import { provisionTenant } from '@/lib/api/tenants';
import { ApiError } from '@/lib/api/types';
import { login, toSession, writeSession } from '@/lib/auth/session';
import { getDictionary } from '@/lib/i18n/dictionaries';
import { readLocale } from '@/lib/i18n/locale';

export interface SignUpState {
  error?: string;
  /** Per-field codes from the backend, keyed by its PascalCase field names. */
  fieldErrors?: Record<string, string[]>;
}

/**
 * Creates a business and signs the new owner in.
 *
 * The owner chose their password here, so there is no invitation to accept: once
 * provisioning succeeds we exchange the same credentials for a session rather
 * than sending the person to a sign-in form they just filled in.
 */
export async function signUpAction(
  _previous: SignUpState,
  formData: FormData,
): Promise<SignUpState> {
  const locale = await readLocale();
  const dict = getDictionary(locale);
  const read = (key: string) => String(formData.get(key) ?? '').trim();

  const ownerEmail = read('ownerEmail');
  const ownerPassword = String(formData.get('ownerPassword') ?? '');

  try {
    await provisionTenant(
      {
        name: read('name'),
        slug: read('slug').toLowerCase(),
        countryCode: read('countryCode').toUpperCase(),
        currencyCode: read('currencyCode').toUpperCase(),
        defaultLanguage: read('defaultLanguage') || 'en',
        timeZoneId: read('timeZoneId'),
        ownerEmail,
        ownerFullName: read('ownerFullName'),
        ownerPassword,
        firstBranchName: read('firstBranchName'),
      },
      locale,
    );
  } catch (error) {
    if (error instanceof ApiError) {
      return { error: error.message, fieldErrors: error.fieldErrors };
    }
    return { error: dict.errors.unexpected };
  }

  try {
    const response = await login(
      { email: ownerEmail, password: ownerPassword, businessHandle: read('slug').toLowerCase() },
      locale,
    );
    const session = toSession(response);
    if (session) {
      await writeSession(session);
    }
  } catch {
    // The business exists either way; fall through to sign-in rather than
    // implying the sign-up failed.
    redirect('/sign-in');
  }

  redirect('/');
}
