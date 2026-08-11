import { BarChart3, LayoutDashboard, Package, ShoppingCart, Warehouse } from 'lucide-react';
import type { LucideIcon } from 'lucide-react';

import type { Dictionary } from '@/lib/i18n/dictionaries';

export interface NavItem {
  label: string;
  icon: LucideIcon;
  /** Undefined for sections that are not built yet; those render disabled. */
  href?: string;
}

export interface NavGroup {
  title: string;
  items: NavItem[];
}

export function buildNav(dict: Dictionary): NavGroup[] {
  return [
    {
      title: dict.nav.groupMain,
      items: [
        { label: dict.nav.dashboard, icon: LayoutDashboard, href: '/' },
        { label: dict.nav.products, icon: Package, href: '/products' },
      ],
    },
    {
      title: dict.nav.groupOperations,
      items: [
        { label: dict.nav.inventory, icon: Warehouse },
        { label: dict.nav.sales, icon: ShoppingCart },
        { label: dict.nav.reports, icon: BarChart3 },
      ],
    },
  ];
}
