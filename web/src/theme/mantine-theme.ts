'use client';

import { createTheme, type MantineColorsTuple } from '@mantine/core';

/** Brand ramp generated around the ConfiOS teal seed (#12715E). */
const brand: MantineColorsTuple = [
  '#eaf6f2',
  '#d3ece4',
  '#a6d8c9',
  '#76c3ad',
  '#4fb195',
  '#36a686',
  '#28a07e',
  '#178c6b',
  '#057c5e',
  '#006b4f',
];

export const theme = createTheme({
  primaryColor: 'brand',
  primaryShade: { light: 7, dark: 5 },
  colors: { brand },
  fontFamily: 'var(--font-inter), system-ui, sans-serif',
  headings: { fontFamily: 'var(--font-sora), system-ui, sans-serif', fontWeight: '700' },
  defaultRadius: 'md',
  radius: { md: '0.75rem', lg: '1rem' },
  cursorType: 'pointer',
  focusRing: 'auto',
});
