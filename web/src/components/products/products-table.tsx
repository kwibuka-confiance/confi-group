'use client';

import { motion } from 'framer-motion';
import { CheckCircle2, Package, Plus, Search, SearchX } from 'lucide-react';
import { useMemo, useState } from 'react';

import { formatMoney } from '@/lib/format';
import type { Product } from '@/lib/api/types';
import { format, type Dictionary, type Locale } from '@/lib/i18n/dictionaries';

interface ProductsTableProps {
  products: Product[];
  dict: Dictionary;
  locale: Locale;
}

/**
 * The catalog table. Search filters the rows already loaded, so typing never
 * re-queries the API.
 */
export function ProductsTable({ products, dict, locale }: ProductsTableProps) {
  const [query, setQuery] = useState('');

  const visible = useMemo(() => {
    const needle = query.trim().toLowerCase();
    if (!needle) return products;
    return products.filter(
      (product) =>
        product.name.toLowerCase().includes(needle) ||
        product.sku.toLowerCase().includes(needle),
    );
  }, [products, query]);

  const countLabel =
    visible.length === 1
      ? dict.products.itemCountOne
      : format(dict.products.itemCount, { count: visible.length });

  return (
    <div className="flex h-full flex-col">
      <div className="flex flex-wrap items-center gap-3 px-4 py-4 sm:px-6">
        <span className="rounded-lg bg-brand-soft px-2.5 py-1 text-xs font-bold text-brand-strong">
          {countLabel}
        </span>

        <div className="ml-auto flex w-full items-center gap-3 sm:w-auto">
          <label className="relative flex-1 sm:w-72 sm:flex-none">
            <span className="sr-only">{dict.products.searchPlaceholder}</span>
            <Search
              size={17}
              aria-hidden
              className="pointer-events-none absolute top-1/2 left-3 -translate-y-1/2 text-ink-muted"
            />
            <input
              type="search"
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder={dict.products.searchPlaceholder}
              className="w-full rounded-xl border border-line bg-panel-muted py-2.5 pr-3 pl-9 text-sm text-ink transition-colors placeholder:text-ink-muted hover:border-brand/40"
            />
          </label>

          <button
            type="button"
            className="inline-flex shrink-0 items-center gap-2 rounded-xl bg-brand px-4 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-brand-strong"
          >
            <Plus size={17} aria-hidden />
            <span className="hidden sm:inline">{dict.products.add}</span>
          </button>
        </div>
      </div>

      {products.length === 0 ? (
        <EmptyState dict={dict} />
      ) : visible.length === 0 ? (
        <NoMatches dict={dict} />
      ) : (
        <div className="min-h-0 flex-1 overflow-auto">
          <table className="w-full border-collapse text-left">
            <thead className="sticky top-0 z-10 bg-panel-muted">
              <tr className="text-[12px] font-bold text-ink-muted">
                <th scope="col" className="py-3 pr-3 pl-4 font-bold sm:pl-6">
                  {dict.products.columnProduct}
                </th>
                <th scope="col" className="hidden px-3 py-3 font-bold md:table-cell">
                  {dict.products.columnSku}
                </th>
                <th scope="col" className="px-3 py-3 text-right font-bold">
                  {dict.products.columnPrice}
                </th>
                <th scope="col" className="py-3 pr-4 pl-3 text-right font-bold sm:pr-6">
                  {dict.products.columnStatus}
                </th>
              </tr>
            </thead>
            <tbody>
              {/*
                Rows fade in on mount only. Exit animations are deliberately not
                used here: a <tr> leaving an AnimatePresence keeps the filtered-out
                row mounted, so the table would disagree with the count.
              */}
              {visible.map((product, index) => (
                  <motion.tr
                    key={product.id}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.18, delay: Math.min(index * 0.02, 0.16) }}
                    className="border-t border-line transition-colors hover:bg-panel-muted"
                  >
                    <td className="py-3 pr-3 pl-4 sm:pl-6">
                      <div className="flex items-center gap-3">
                        <span className="grid size-9 shrink-0 place-items-center rounded-lg bg-brand-soft text-brand-strong">
                          <Package size={17} aria-hidden />
                        </span>
                        <div className="min-w-0">
                          <p className="truncate text-sm font-semibold text-ink">
                            {product.name}
                          </p>
                          <p className="truncate text-xs text-ink-muted md:hidden">
                            {product.sku}
                          </p>
                        </div>
                      </div>
                    </td>
                    <td className="hidden px-3 py-3 text-sm text-ink-muted md:table-cell">
                      {product.sku}
                    </td>
                    <td className="px-3 py-3 text-right text-sm font-bold text-ink tabular-nums">
                      {formatMoney(product.priceAmount, product.currencyCode, locale)}
                    </td>
                    <td className="py-3 pr-4 pl-3 text-right sm:pr-6">
                      <StatusBadge isActive={product.isActive} dict={dict} />
                    </td>
                  </motion.tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

/** State is carried by an icon and a label, never by colour alone. */
function StatusBadge({ isActive, dict }: { isActive: boolean; dict: Dictionary }) {
  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-[11px] font-semibold ${
        isActive ? 'bg-brand-soft text-brand-strong' : 'bg-panel-muted text-ink-muted'
      }`}
    >
      {isActive ? <CheckCircle2 size={13} aria-hidden /> : <Package size={13} aria-hidden />}
      {isActive ? dict.products.statusActive : dict.products.statusArchived}
    </span>
  );
}

function EmptyState({ dict }: { dict: Dictionary }) {
  return (
    <div className="grid flex-1 place-items-center px-6 py-16 text-center">
      <div>
        <span className="mx-auto grid size-16 place-items-center rounded-full bg-panel-muted text-ink-muted">
          <Package size={28} aria-hidden />
        </span>
        <h2 className="mt-4 font-display text-lg font-bold text-ink">{dict.products.empty}</h2>
        <p className="mt-1 text-sm text-ink-muted">{dict.products.emptyHint}</p>
      </div>
    </div>
  );
}

function NoMatches({ dict }: { dict: Dictionary }) {
  return (
    <div className="grid flex-1 place-items-center px-6 py-16 text-center">
      <div>
        <SearchX size={34} aria-hidden className="mx-auto text-ink-muted" />
        <p className="mt-3 text-sm text-ink-muted">{dict.products.noMatches}</p>
      </div>
    </div>
  );
}
