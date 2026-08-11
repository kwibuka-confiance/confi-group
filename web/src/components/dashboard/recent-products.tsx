import { ArrowRight, Package } from 'lucide-react';
import Link from 'next/link';

import { formatMoney } from '@/lib/format';
import type { Product } from '@/lib/api/types';
import type { Dictionary, Locale } from '@/lib/i18n/dictionaries';

interface RecentProductsProps {
  products: Product[];
  dict: Dictionary;
  locale: Locale;
}

export function RecentProducts({ products, dict, locale }: RecentProductsProps) {
  return (
    <section aria-labelledby="recent-heading">
      <div className="mb-2 flex items-center justify-between">
        <h2 id="recent-heading" className="font-display text-base font-bold text-ink">
          {dict.dashboard.recentlyAdded}
        </h2>
        {products.length > 0 && (
          <Link
            href="/products"
            className="inline-flex items-center gap-1.5 rounded-lg px-2 py-1 text-sm font-medium text-brand transition-colors hover:bg-brand-soft"
          >
            {dict.dashboard.viewAll}
            <ArrowRight size={15} aria-hidden />
          </Link>
        )}
      </div>

      <div className="overflow-hidden rounded-2xl border border-line bg-panel-muted">
        {products.length === 0 ? (
          <p className="p-5 text-sm text-ink-muted">{dict.dashboard.emptyHint}</p>
        ) : (
          <ul className="divide-y divide-[var(--border)]">
            {products.map((product) => (
              <li key={product.id} className="flex items-center gap-3 px-4 py-3">
                <span className="grid size-9 shrink-0 place-items-center rounded-lg bg-panel text-ink-muted">
                  <Package size={17} aria-hidden />
                </span>
                <span className="min-w-0 flex-1">
                  <span className="block truncate text-sm font-semibold text-ink">
                    {product.name}
                  </span>
                  <span className="block truncate text-xs text-ink-muted">{product.sku}</span>
                </span>
                <span className="shrink-0 text-sm font-bold text-ink tabular-nums">
                  {formatMoney(product.priceAmount, product.currencyCode, locale)}
                </span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </section>
  );
}
