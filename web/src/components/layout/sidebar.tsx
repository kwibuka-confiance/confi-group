'use client';

import { motion } from 'framer-motion';
import { LogOut } from 'lucide-react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';

import { BusinessSwitcher } from './business-switcher';
import type { NavGroup } from './nav-config';
import { initialsOf } from '@/lib/format';
import type { Dictionary } from '@/lib/i18n/dictionaries';

interface SidebarProps {
  groups: NavGroup[];
  dict: Dictionary;
  user: { fullName: string; email: string };
  business: { name: string; tenantId: string };
  /** Closes the drawer after navigating on small screens. */
  onNavigate?: () => void;
}

export function Sidebar({ groups, dict, user, business, onNavigate }: SidebarProps) {
  const pathname = usePathname();

  return (
    <nav
      aria-label={dict.nav.groupMain}
      className="flex h-full flex-col rounded-2xl bg-rail text-white/80"
    >
      <div className="px-3 pt-4 pb-1">
        <p className="px-2 pb-2 font-display text-base font-bold text-white/90">{dict.appName}</p>
        <BusinessSwitcher
          dict={dict}
          businessName={business.name}
          tenantId={business.tenantId}
        />
      </div>

      <div className="flex-1 overflow-y-auto px-3 py-2">
        {groups.map((group) => (
          <div key={group.title} className="mb-2">
            <p className="px-3 pt-4 pb-2 text-[11px] font-bold tracking-[0.09em] text-white/45 uppercase">
              {group.title}
            </p>
            <ul className="space-y-1">
              {group.items.map((item) => {
                const active = item.href === pathname;
                const Icon = item.icon;

                if (!item.href) {
                  return (
                    <li key={item.label}>
                      <span
                        aria-disabled
                        className="flex cursor-not-allowed items-center gap-3 rounded-xl px-3 py-3 text-sm font-medium text-white/35"
                      >
                        <Icon size={19} aria-hidden />
                        <span className="flex-1 truncate">{item.label}</span>
                        <span className="rounded-md bg-white/8 px-1.5 py-0.5 text-[10px] text-white/50">
                          {dict.nav.comingSoon}
                        </span>
                      </span>
                    </li>
                  );
                }

                return (
                  <li key={item.label} className="relative">
                    {active && (
                      <motion.span
                        layoutId="nav-active"
                        transition={{ type: 'spring', stiffness: 380, damping: 32 }}
                        className="absolute inset-0 rounded-xl bg-brand"
                        aria-hidden
                      />
                    )}
                    <Link
                      href={item.href}
                      onClick={onNavigate}
                      aria-current={active ? 'page' : undefined}
                      className={`relative flex items-center gap-3 rounded-xl px-3 py-3 text-sm transition-colors ${
                        active
                          ? 'font-bold text-white'
                          : 'font-medium text-white/70 hover:bg-white/6 hover:text-white'
                      }`}
                    >
                      <Icon size={19} aria-hidden />
                      <span className="flex-1 truncate">{item.label}</span>
                    </Link>
                  </li>
                );
              })}
            </ul>
          </div>
        ))}
      </div>

      <div className="p-3">
        <div className="flex items-center gap-3 rounded-2xl bg-white/6 px-3 py-2.5">
          <span className="grid size-9 shrink-0 place-items-center rounded-full bg-brand text-[13px] font-bold text-white">
            {initialsOf(user.fullName)}
          </span>
          <span className="min-w-0 flex-1">
            <span className="block truncate text-sm font-semibold text-white">
              {user.fullName}
            </span>
            <span className="block truncate text-[11px] text-white/55">{user.email}</span>
          </span>
          <form action="/api/auth/sign-out" method="post">
            <button
              type="submit"
              aria-label={dict.nav.signOut}
              title={dict.nav.signOut}
              className="grid size-9 place-items-center rounded-lg text-white/70 transition-colors hover:bg-white/10 hover:text-white"
            >
              <LogOut size={17} aria-hidden />
            </button>
          </form>
        </div>
      </div>
    </nav>
  );
}
