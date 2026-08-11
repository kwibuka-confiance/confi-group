import { redirect } from 'next/navigation';
import type { ReactNode } from 'react';

import { AppShell } from '@/components/layout/app-shell';
import { readSession } from '@/lib/auth/session';
import { getDictionary } from '@/lib/i18n/dictionaries';
import { readLocale } from '@/lib/i18n/locale';

/**
 * Everything signed in renders inside the shell. The session is read on the
 * server, so an unauthenticated visitor never receives the dashboard markup.
 */
export default async function DashboardLayout({ children }: { children: ReactNode }) {
  const session = await readSession();
  if (!session) {
    redirect('/sign-in');
  }

  const locale = await readLocale();
  const dict = getDictionary(locale);

  return (
    <AppShell
      dict={dict}
      locale={locale}
      user={{ fullName: session.fullName, email: session.email }}
      business={{ name: session.businessName, tenantId: session.tenantId }}
    >
      {children}
    </AppShell>
  );
}
