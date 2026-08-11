'use client';

import { Check, Languages } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';

import { setLocaleAction } from '@/app/locale-actions';
import { locales, type Dictionary, type Locale } from '@/lib/i18n/dictionaries';

/**
 * Languages are named in their own language, so someone who cannot read the
 * current one can still find theirs.
 */
const NATIVE_NAMES: Record<Locale, string> = {
  en: 'English',
  rw: 'Ikinyarwanda',
  fr: 'Français',
};

interface LocaleSwitcherProps {
  dict: Dictionary;
  current: Locale;
}

export function LocaleSwitcher({ dict, current }: LocaleSwitcherProps) {
  const [open, setOpen] = useState(false);
  const container = useRef<HTMLDivElement>(null);

  // The menu closes once the new language has actually been applied, never on
  // click: closing on click unmounts the form mid-submit and the action is lost.
  const [applied, setApplied] = useState(current);
  if (applied !== current) {
    setApplied(current);
    setOpen(false);
  }

  // Clicking away or pressing Escape closes the menu, as with any popover.
  useEffect(() => {
    if (!open) return;

    const onPointerDown = (event: PointerEvent) => {
      if (!container.current?.contains(event.target as Node)) setOpen(false);
    };
    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') setOpen(false);
    };

    document.addEventListener('pointerdown', onPointerDown);
    document.addEventListener('keydown', onKey);
    return () => {
      document.removeEventListener('pointerdown', onPointerDown);
      document.removeEventListener('keydown', onKey);
    };
  }, [open]);

  return (
    <div ref={container} className="relative">
      <button
        type="button"
        onClick={() => setOpen((value) => !value)}
        aria-label={dict.nav.language}
        title={dict.nav.language}
        aria-haspopup="menu"
        aria-expanded={open}
        className="grid size-9 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink"
      >
        <Languages size={18} aria-hidden />
      </button>

      {open && (
        <div
          role="menu"
          aria-label={dict.nav.language}
          className="absolute top-full right-0 z-50 mt-1 w-44 rounded-xl border border-line bg-panel p-1 shadow-xl"
        >
          {locales.map((locale) => (
            <form key={locale} action={setLocaleAction}>
              <input type="hidden" name="locale" value={locale} />
              <button
                type="submit"
                role="menuitemradio"
                aria-checked={locale === current}
                className="flex w-full items-center gap-2 rounded-lg px-2.5 py-2 text-left text-sm text-ink transition-colors hover:bg-panel-muted"
              >
                <span className="flex-1 truncate">{NATIVE_NAMES[locale]}</span>
                {locale === current && <Check size={15} aria-hidden className="text-brand" />}
              </button>
            </form>
          ))}
        </div>
      )}
    </div>
  );
}
