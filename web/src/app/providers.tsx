'use client';

import type { ReactNode } from 'react';

import { ThemeProvider } from './theme-provider';

/**
 * Client boundary for everything that needs React context. Kept as thin as
 * possible so pages stay Server Components.
 */
export function Providers({ children }: { children: ReactNode }) {
  return <ThemeProvider>{children}</ThemeProvider>;
}
