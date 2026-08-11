import { Package } from 'lucide-react';
import Link from 'next/link';

import { RecentProducts } from '@/components/dashboard/recent-products';
import { StatCard } from '@/components/dashboard/stat-card';
import { getProducts, summariseCatalog } from '@/lib/api/catalog';
import { ApiError } from '@/lib/api/types';
import { readSession } from '@/lib/auth/session';
import { formatCount, formatMoney, initialsOf } from '@/lib/format';
import { defaultLocale, format, getDictionary } from '@/lib/i18n/dictionaries';

/** Tenant data is per-request; nothing here is cached across users. */
export const dynamic = 'force-dynamic';

export default async function DashboardPage() {
  const session = await readSession();
  if (!session) return null; // The layout has already redirected.

  const dict = getDictionary();
  const locale = defaultLocale;

  let summary: ReturnType<typeof summariseCatalog> | null = null;
  let loadFailed = false;

  try {
    summary = summariseCatalog(await getProducts(session.accessToken, locale));
  } catch (error) {
    // A catalog that will not load must not take the whole dashboard down.
    loadFailed = true;
    if (!(error instanceof ApiError)) throw error;
  }

  /**
   * Amounts in different currencies are never summed. One currency shows its
   * total; several report how many currencies are in play.
   */
  const catalogValue = (() => {
    if (!summary) return '—';
    const entries = Object.entries(summary.valueByCurrency);
    if (entries.length === 0) return '—';
    if (entries.length > 1) {
      return format(dict.dashboard.currencyCount, { count: entries.length });
    }
    const [currency, total] = entries[0]!;
    return formatMoney(total, currency, locale);
  })();

  return (
    <div className="mx-auto w-full max-w-6xl px-4 py-6 sm:px-6 lg:px-7">
      <header className="flex items-center gap-4">
        <span className="grid size-12 shrink-0 place-items-center rounded-2xl bg-brand-soft font-display text-base font-bold text-brand-strong">
          {initialsOf(session.fullName)}
        </span>
        <div className="min-w-0">
          <h2 className="truncate font-display text-2xl font-bold text-ink">
            {format(dict.dashboard.welcome, { name: session.fullName })}
          </h2>
          <p className="truncate text-sm text-ink-muted">
            {format(dict.dashboard.signedInTo, { business: session.businessName })}
          </p>
        </div>
      </header>

      {loadFailed ? (
        <p
          role="alert"
          className="mt-6 rounded-2xl border border-line bg-panel-muted p-5 text-sm text-ink-muted"
        >
          {dict.errors.loadFailed}
        </p>
      ) : (
        <>
          <section
            aria-label={dict.nav.dashboard}
            className="mt-6 grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-4"
          >
            <StatCard
              index={0}
              label={dict.dashboard.statProducts}
              value={formatCount(summary?.total ?? 0, locale)}
              icon="package"
            />
            <StatCard
              index={1}
              label={dict.dashboard.statActive}
              value={formatCount(summary?.active ?? 0, locale)}
              icon="check"
            />
            <StatCard
              index={2}
              label={dict.dashboard.statCatalogValue}
              value={catalogValue}
              icon="wallet"
            />
            <StatCard
              index={3}
              label={dict.dashboard.statPermissions}
              value={formatCount(session.permissions.length, locale)}
              icon="shield"
            />
          </section>

          <div className="mt-7">
            <RecentProducts products={summary?.recent ?? []} dict={dict} locale={locale} />
          </div>
        </>
      )}

      <section aria-labelledby="quick-actions" className="mt-7">
        <h2 id="quick-actions" className="font-display text-base font-bold text-ink">
          {dict.dashboard.quickActions}
        </h2>
        <div className="mt-3 flex flex-wrap gap-3">
          <Link
            href="/products"
            className="inline-flex items-center gap-2 rounded-xl bg-brand-soft px-4 py-2.5 text-sm font-semibold text-brand-strong transition-colors hover:bg-brand hover:text-white"
          >
            <Package size={17} aria-hidden />
            {dict.nav.products}
          </Link>
        </div>
      </section>
    </div>
  );
}
