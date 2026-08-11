'use server';

import { revalidatePath } from 'next/cache';
import { redirect } from 'next/navigation';

import { ApiError, type BusinessSummary } from '@/lib/api/types';
import { login, readSession, selectBusiness, toSession, writeSession } from '@/lib/auth/session';
import { defaultLocale, getDictionary } from '@/lib/i18n/dictionaries';

export interface SwitchBusinessState {
  error?: string;
  /** The businesses the re-entered password unlocked. */
  choice?: {
    selectionToken: string;
    businesses: BusinessSummary[];
  };
  /** Set when the password only unlocks the business already open. */
  onlyOne?: boolean;
}

/**
 * Starts a switch to another business by re-checking the password.
 *
 * Accounts are per business and each carries its own password, so the session
 * held for one business proves nothing about the others. Asking again reuses the
 * sign-in path, which only ever returns businesses whose own password matched.
 * That is also why there is no endpoint listing a person's businesses: it would
 * tell whoever holds this session that the email is used elsewhere, without any
 * password for those accounts having been given.
 *
 * The email comes from the session rather than the form, so this cannot be used
 * to probe someone else's address.
 */
export async function startSwitchAction(
  _previous: SwitchBusinessState,
  formData: FormData,
): Promise<SwitchBusinessState> {
  const dict = getDictionary();

  const session = await readSession();
  if (!session) {
    redirect('/sign-in');
  }

  const password = String(formData.get('password') ?? '');
  if (!password) {
    return { error: dict.errors.required };
  }

  try {
    const response = await login({ email: session.email, password }, defaultLocale);

    if (response.status === 'select_business') {
      return {
        choice: {
          selectionToken: response.selectionToken ?? '',
          businesses: response.businesses ?? [],
        },
      };
    }

    // Authenticated outright means the password unlocked exactly one business,
    // which is the one already open. Say so rather than silently reloading.
    return { onlyOne: true };
  } catch (error) {
    if (error instanceof ApiError) {
      // Already localised, and worded so it reveals nothing about other accounts.
      return { error: error.message };
    }
    return { error: dict.errors.unexpected };
  }
}

/** Completes the switch, replacing the session with one for the chosen business. */
export async function confirmSwitchAction(
  _previous: SwitchBusinessState,
  formData: FormData,
): Promise<SwitchBusinessState> {
  const dict = getDictionary();

  const selectionToken = String(formData.get('selectionToken') ?? '');
  const tenantId = String(formData.get('tenantId') ?? '');

  if (!selectionToken || !tenantId) {
    return { error: dict.errors.unexpected };
  }

  try {
    const response = await selectBusiness(selectionToken, tenantId, defaultLocale);
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

  // Everything rendered so far belongs to the previous business, so the whole
  // tree is dropped rather than left to serve another tenant's data.
  revalidatePath('/', 'layout');
  redirect('/');
}
