import { BarChart3, Package, ShoppingCart, Store } from 'lucide-react';
import { redirect } from 'next/navigation';

import { SignUpForm } from './sign-up-form';
import { readSession } from '@/lib/auth/session';
import { getDictionary } from '@/lib/i18n/dictionaries';

export const dynamic = 'force-dynamic';

export default async function SignUpPage() {
  if (await readSession()) {
    redirect('/');
  }

  const dict = getDictionary();
  const points = [
    { icon: Package, label: dict.auth.pointCatalog },
    { icon: ShoppingCart, label: dict.auth.pointSales },
    { icon: BarChart3, label: dict.auth.pointInsights },
  ];

  return (
    <div className="grid min-h-dvh lg:grid-cols-[5fr_6fr]">
      {/* Brand panel: fixed dark teal in both schemes, so it always reads as brand. */}
      <aside className="relative hidden flex-col justify-center overflow-hidden bg-rail px-12 lg:flex">
        <div
          aria-hidden
          className="absolute inset-0 bg-gradient-to-br from-brand/45 via-transparent to-transparent"
        />
        <div className="relative">
          <div className="flex items-center gap-3">
            <span className="grid size-10 place-items-center rounded-xl bg-brand text-white">
              <Store size={21} aria-hidden />
            </span>
            <span className="font-display text-xl font-bold text-white">{dict.appName}</span>
          </div>

          <h2 className="mt-12 max-w-md font-display text-4xl leading-tight font-bold text-white">
            {dict.auth.tagline}
          </h2>
          <p className="mt-4 max-w-md text-base text-white/75">{dict.auth.blurb}</p>

          <ul className="mt-10 space-y-4">
            {points.map(({ icon: Icon, label }) => (
              <li key={label} className="flex items-center gap-3.5 text-white/90">
                <Icon size={20} aria-hidden />
                {label}
              </li>
            ))}
          </ul>
        </div>
      </aside>

      <main className="grid place-items-center bg-panel px-6 py-10">
        <SignUpForm dict={dict} />
      </main>
    </div>
  );
}
