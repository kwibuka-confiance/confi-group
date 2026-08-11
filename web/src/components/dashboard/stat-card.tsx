'use client';

import { motion } from 'framer-motion';
import { CheckCircle2, Package, ShieldCheck, Wallet } from 'lucide-react';

/**
 * Icons are selected by key rather than passed as a component: only plain data
 * crosses the server/client boundary.
 */
const icons = {
  package: Package,
  check: CheckCircle2,
  wallet: Wallet,
  shield: ShieldCheck,
} as const;

export type StatIcon = keyof typeof icons;

export interface StatCardProps {
  label: string;
  value: string;
  icon: StatIcon;
  /** Staggers the entrance so the row resolves left to right. */
  index?: number;
}

/**
 * A single headline figure. The value wears text ink and the icon carries the
 * identity, so meaning never rests on colour alone.
 */
export function StatCard({ label, value, icon, index = 0 }: StatCardProps) {
  const Icon = icons[icon];

  return (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.28, delay: index * 0.05, ease: 'easeOut' }}
      whileHover={{ y: -2 }}
      className="rounded-2xl border border-line bg-panel-muted p-4 transition-shadow hover:shadow-sm"
    >
      <div className="flex items-center gap-2.5">
        <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-brand-soft text-brand-strong">
          <Icon size={17} aria-hidden />
        </span>
        <p className="truncate text-[13px] font-medium text-ink-muted">{label}</p>
      </div>
      <p className="mt-3.5 truncate font-display text-2xl font-bold text-ink">{value}</p>
    </motion.div>
  );
}
