import 'server-only';

import { cookies, headers } from 'next/headers';

import { defaultLocale, isLocale, locales, type Locale } from './dictionaries';

export const LOCALE_COOKIE = 'confios_locale';

/**
 * The language to render in.
 *
 * A chosen language wins, because it is an explicit decision. Otherwise the
 * browser's own preference is honoured, so a Kinyarwanda speaker is not shown
 * English merely for never having opened the switcher. English is the last
 * resort.
 */
export async function readLocale(): Promise<Locale> {
  const store = await cookies();
  const chosen = store.get(LOCALE_COOKIE)?.value;
  if (isLocale(chosen)) {
    return chosen;
  }

  const accepted = (await headers()).get('accept-language');
  return matchAcceptLanguage(accepted);
}

/**
 * Picks the best supported language from an Accept-Language header.
 *
 * Only the language subtag is compared, so `fr-CA` matches `fr`. Quality values
 * are respected, so a browser asking for `rw;q=0.9, en;q=0.8` gets Kinyarwanda.
 */
export function matchAcceptLanguage(header: string | null): Locale {
  if (!header) return defaultLocale;

  const ranked = header
    .split(',')
    .map((part) => {
      const [tag, ...params] = part.trim().split(';');
      const quality = params
        .map((param) => param.trim())
        .find((param) => param.startsWith('q='));
      return {
        language: tag.trim().toLowerCase().split('-')[0],
        quality: quality ? Number(quality.slice(2)) : 1,
      };
    })
    .filter((entry) => Number.isFinite(entry.quality))
    .sort((a, b) => b.quality - a.quality);

  const match = ranked.find((entry) => (locales as readonly string[]).includes(entry.language));
  return match ? (match.language as Locale) : defaultLocale;
}
