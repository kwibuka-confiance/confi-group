import { redirect } from 'next/navigation';
import type { ReactNode } from 'react';

import { AppShell } from '@/components/layout/app-shell';
import { readSession } from '@/lib/auth/session';
import { getDictionary } from '@/lib/i18n/dictionaries';

/**
 * Everything signed in renders inside the shell. The session is read on the
 * server, so an unauthenticated visitor never receives the dashboard markup.
 */
export default async function DashboardLayout({ children }: { children: ReactNode }) {
  const session = await readSession();
  if (!session) {
    redirect('/sign-in');
  }

  const dict = getDictionary();

  return (
    <AppShell dict={dict} user={{ fullName: session.fullName, email: session.email }}>
      {children}
    </AppShell>
  );
}
