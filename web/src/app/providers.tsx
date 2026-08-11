'use client';

import { MantineProvider } from '@mantine/core';
import type { ReactNode } from 'react';

import { theme } from '@/theme/mantine-theme';

/**
 * Client boundary for everything that needs React context. Kept as thin as
 * possible so pages stay Server Components.
 */
export function Providers({ children }: { children: ReactNode }) {
  return (
    <MantineProvider theme={theme} defaultColorScheme="auto">
      {children}
    </MantineProvider>
  );
}
