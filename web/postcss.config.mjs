/**
 * Tailwind v4 and Mantine both run through PostCSS. Mantine's preset must come
 * first so its custom media queries and `rem()` helpers are resolved before
 * Tailwind processes the sheet.
 */
const config = {
  plugins: {
    'postcss-preset-mantine': {},
    'postcss-simple-vars': {
      variables: {
        'mantine-breakpoint-xs': '36em',
        'mantine-breakpoint-sm': '48em',
        'mantine-breakpoint-md': '62em',
        'mantine-breakpoint-lg': '75em',
        'mantine-breakpoint-xl': '88em',
      },
    },
    '@tailwindcss/postcss': {},
  },
};

export default config;
