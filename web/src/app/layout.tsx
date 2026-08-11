import './globals.css';

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
    <html lang="en" suppressHydrationWarning>
      <body className={`${inter.variable} ${sora.variable} font-sans`}>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
