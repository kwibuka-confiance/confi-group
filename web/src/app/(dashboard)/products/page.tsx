import { ProductsTable } from '@/components/products/products-table';
import { getProducts } from '@/lib/api/catalog';
import { ApiError } from '@/lib/api/types';
import { readSession } from '@/lib/auth/session';
import { getDictionary } from '@/lib/i18n/dictionaries';
import { readLocale } from '@/lib/i18n/locale';
import type { Product } from '@/lib/api/types';

export const dynamic = 'force-dynamic';

export default async function ProductsPage() {
  const session = await readSession();
  if (!session) return null; // The layout has already redirected.

  const locale = await readLocale();
  const dict = getDictionary(locale);

  let products: Product[] = [];
  let loadFailed = false;

  try {
    products = await getProducts(session.accessToken, locale);
  } catch (error) {
    loadFailed = true;
    if (!(error instanceof ApiError)) throw error;
  }

  if (loadFailed) {
    return (
      <div className="p-6">
        <p role="alert" className="rounded-2xl border border-line bg-panel-muted p-5 text-sm text-ink-muted">
          {dict.errors.loadFailed}
        </p>
      </div>
    );
  }

  return <ProductsTable products={products} dict={dict} locale={locale} />;
}
