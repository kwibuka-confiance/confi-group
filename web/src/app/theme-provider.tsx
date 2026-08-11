'use client';

import { useEffect, useSyncExternalStore, type ReactNode } from 'react';

export type Theme = 'light' | 'dark';

const STORAGE_KEY = 'confios.theme';

/**
 * The colour scheme lives outside React: it is a property of the document, read
 * from storage and written to `data-theme`. Components subscribe to it rather
 * than owning it, which keeps the server render deterministic — the server always
 * says "light" and the stored preference is applied after mount, so there is
 * nothing for hydration to disagree about.
 */
let current: Theme = 'light';
let initialised = false;
const listeners = new Set<() => void>();

function subscribe(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

function getSnapshot(): Theme {
  return current;
}

function getServerSnapshot(): Theme {
  return 'light';
}

function apply(theme: Theme, persist: boolean): void {
  current = theme;
  document.documentElement.dataset.theme = theme;
  if (persist) {
    window.localStorage.setItem(STORAGE_KEY, theme);
  }
  listeners.forEach((listener) => listener());
}

function preferredTheme(): Theme {
  const stored = window.localStorage.getItem(STORAGE_KEY);
  if (stored === 'light' || stored === 'dark') {
    return stored;
  }
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

/** Applies the stored preference once, after the first paint. */
export function ThemeProvider({ children }: { children: ReactNode }) {
  useEffect(() => {
    if (initialised) return;
    initialised = true;
    apply(preferredTheme(), false);
  }, []);

  return <>{children}</>;
}

export function useTheme(): { theme: Theme; toggle: () => void } {
  const theme = useSyncExternalStore(subscribe, getSnapshot, getServerSnapshot);
  return {
    theme,
    toggle: () => apply(theme === 'dark' ? 'light' : 'dark', true),
  };
}
