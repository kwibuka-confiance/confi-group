'use server';

import { revalidatePath } from 'next/cache';
import { cookies } from 'next/headers';

import { isLocale } from '@/lib/i18n/dictionaries';
import { LOCALE_COOKIE } from '@/lib/i18n/locale';

const ONE_YEAR_IN_SECONDS = 60 * 60 * 24 * 365;

/**
 * Remembers the language to render in.
 *
 * Not httpOnly: this is a display preference, not a credential, and keeping it
 * readable lets the choice survive without a round trip if it is ever needed
 * client-side. An unrecognised value is ignored rather than stored, so the
 * cookie can never put the app into a language it cannot render.
 */
export async function setLocaleAction(formData: FormData): Promise<void> {
  const requested = String(formData.get('locale') ?? '');
  if (!isLocale(requested)) {
    return;
  }

  const store = await cookies();
  store.set(LOCALE_COOKIE, requested, {
    sameSite: 'lax',
    secure: process.env.NODE_ENV === 'production',
    path: '/',
    maxAge: ONE_YEAR_IN_SECONDS,
  });

  // Every rendered string depends on this, including the shell.
  revalidatePath('/', 'layout');
}
