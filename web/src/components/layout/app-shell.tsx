'use client';

import { Drawer } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Menu } from 'lucide-react';
import { usePathname } from 'next/navigation';
import type { ReactNode } from 'react';

import { buildNav } from './nav-config';
import { Sidebar } from './sidebar';
import { ThemeToggle } from './theme-toggle';
import { initialsOf } from '@/lib/format';
import type { Dictionary } from '@/lib/i18n/dictionaries';

interface AppShellProps {
  dict: Dictionary;
  user: { fullName: string; email: string };
  children: ReactNode;
}

/**
 * The signed-in frame: a dark rail beside a light content panel. The rail is
 * inline from `lg` up and moves into a drawer below it.
 */
export function AppShell({ dict, user, children }: AppShellProps) {
  const [opened, { open, close }] = useDisclosure(false);
  const pathname = usePathname();
  const groups = buildNav(dict);

  // The header names the current section, so it stays in step with the rail.
  const title = pathname.startsWith('/products')
    ? dict.products.title
    : dict.nav.dashboard;

  return (
    <div className="flex min-h-dvh gap-3 bg-page p-0 lg:p-3">
      <aside className="hidden w-[264px] shrink-0 lg:block">
        <Sidebar groups={groups} dict={dict} user={user} />
      </aside>

      <Drawer
        opened={opened}
        onClose={close}
        size={264}
        padding={0}
        withCloseButton={false}
        classNames={{ content: 'bg-rail', body: 'h-full p-0' }}
        aria-label={dict.nav.openMenu}
      >
        <Sidebar groups={groups} dict={dict} user={user} onNavigate={close} />
      </Drawer>

      <main className="flex min-w-0 flex-1 flex-col overflow-hidden border-line bg-panel lg:rounded-2xl lg:border">
        <header className="flex items-center gap-2 border-b border-line px-3 py-2.5 sm:px-4">
          <button
            type="button"
            onClick={open}
            aria-label={dict.nav.openMenu}
            className="grid size-9 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink lg:hidden"
          >
            <Menu size={20} aria-hidden />
          </button>

          <h1 className="font-display text-lg font-bold text-ink sm:text-xl">{title}</h1>

          <div className="ml-auto flex items-center gap-2">
            <ThemeToggle label={dict.nav.toggleTheme} />
            <span
              aria-hidden
              className="grid size-9 place-items-center rounded-full bg-brand-soft text-[13px] font-bold text-brand-strong"
            >
              {initialsOf(user.fullName)}
            </span>
          </div>
        </header>

        <div className="flex-1 overflow-y-auto">{children}</div>
      </main>
    </div>
  );
}
