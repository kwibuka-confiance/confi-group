import '@mantine/core/styles.css';
import './globals.css';

import { mantineHtmlProps } from '@mantine/core';
import type { Metadata } from 'next';
import { Inter, Sora } from 'next/font/google';
import type { ReactNode } from 'react';

import { Providers } from './providers';

const inter = Inter({ subsets: ['latin'], variable: '--font-inter', display: 'swap' });
const sora = Sora({ subsets: ['latin'], variable: '--font-sora', display: 'swap' });

export const metadata: Metadata = {
  title: 'ConfiOS',
  description: 'Run your whole business in one place.',
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en" {...mantineHtmlProps}>
      {/*
        Mantine's ColorSchemeScript is deliberately not used: it renders a <script>
        from inside a component, which Next 16 rejects and which broke hydration.
        The scheme is applied by MantineProvider after mount instead, so the server
        always renders the same markup.
      */}
      <body className={`${inter.variable} ${sora.variable} font-sans`}>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
