'use client';

import { motion } from 'framer-motion';
import { AlertCircle, ChevronRight, Check, Lock, Store, X } from 'lucide-react';
import { useActionState, useEffect, useId, useState } from 'react';

import {
  confirmSwitchAction,
  startSwitchAction,
  type SwitchBusinessState,
} from '@/app/(dashboard)/actions';
import { format, type Dictionary } from '@/lib/i18n/dictionaries';

const initialState: SwitchBusinessState = {};

interface BusinessSwitcherProps {
  dict: Dictionary;
  businessName: string;
  tenantId: string;
}

/**
 * Names the business currently open and lets the person move to another one.
 *
 * Switching asks for the password again because accounts are per business, each
 * with its own. See the server action for why that is not merely a formality.
 */
export function BusinessSwitcher({ dict, businessName, tenantId }: BusinessSwitcherProps) {
  const [open, setOpen] = useState(false);

  return (
    <>
      <button
        type="button"
        onClick={() => setOpen(true)}
        className="group flex w-full items-center gap-3 rounded-xl bg-white/6 px-3 py-2.5 text-left transition-colors hover:bg-white/10"
      >
        <span className="grid size-9 shrink-0 place-items-center rounded-lg bg-brand text-white">
          <Store size={18} aria-hidden />
        </span>
        <span className="min-w-0 flex-1">
          <span className="block truncate text-sm font-semibold text-white">{businessName}</span>
          <span className="block truncate text-[11px] text-white/55">
            {dict.business.switchAction}
          </span>
        </span>
        <ChevronRight
          size={16}
          aria-hidden
          className="shrink-0 text-white/50 transition-transform group-hover:translate-x-0.5"
        />
      </button>

      {open && (
        <SwitchDialog
          dict={dict}
          currentTenantId={tenantId}
          onClose={() => setOpen(false)}
        />
      )}
    </>
  );
}

interface SwitchDialogProps {
  dict: Dictionary;
  currentTenantId: string;
  onClose: () => void;
}

/**
 * Mounted only while open, so an abandoned password never lingers in a detached
 * component's state.
 */
function SwitchDialog({ dict, currentTenantId, onClose }: SwitchDialogProps) {
  const [state, formAction, pending] = useActionState(startSwitchAction, initialState);
  const passwordId = useId();

  useEffect(() => {
    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose();
    };
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [onClose]);

  return (
    <div className="fixed inset-0 z-50 grid place-items-center overflow-y-auto p-4">
      <button
        type="button"
        aria-hidden
        tabIndex={-1}
        onClick={onClose}
        className="fixed inset-0 cursor-default bg-black/50"
      />

      <motion.div
        role="dialog"
        aria-modal="true"
        aria-labelledby="switch-business-title"
        initial={{ opacity: 0, scale: 0.97, y: 8 }}
        animate={{ opacity: 1, scale: 1, y: 0 }}
        transition={{ duration: 0.16, ease: 'easeOut' }}
        className="relative w-full max-w-sm rounded-2xl border border-line bg-panel p-5 shadow-xl"
      >
        <div className="mb-1 flex items-start justify-between gap-3">
          <h2 id="switch-business-title" className="font-display text-lg font-bold text-ink">
            {dict.business.switchTitle}
          </h2>
          <button
            type="button"
            onClick={onClose}
            aria-label={dict.products.close}
            className="-mt-1 grid size-8 shrink-0 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink"
          >
            <X size={17} aria-hidden />
          </button>
        </div>

        {state.error && <Notice message={state.error} tone="error" />}

        {state.choice ? (
          <BusinessList
            dict={dict}
            choice={state.choice}
            currentTenantId={currentTenantId}
          />
        ) : (
          <form action={formAction}>
            <p className="text-sm text-ink-muted">{dict.business.switchPrompt}</p>

            {state.onlyOne && <Notice message={dict.business.onlyOne} tone="info" />}

            <label htmlFor={passwordId} className="mt-4 mb-1.5 block text-sm font-medium text-ink">
              {dict.auth.password}
            </label>
            <div className="relative">
              <Lock
                size={17}
                aria-hidden
                className="pointer-events-none absolute top-1/2 left-3 -translate-y-1/2 text-ink-muted"
              />
              <input
                id={passwordId}
                name="password"
                type="password"
                autoComplete="current-password"
                autoFocus
                className="w-full rounded-xl border border-line bg-panel-muted py-3 pr-3 pl-9 text-sm text-ink transition-colors hover:border-brand/40"
              />
            </div>

            <button
              type="submit"
              disabled={pending}
              className="mt-4 w-full rounded-xl bg-brand px-4 py-3 text-sm font-semibold text-white transition-colors hover:bg-brand-strong disabled:opacity-60"
            >
              {pending ? dict.business.checking : dict.business.continue}
            </button>
          </form>
        )}
      </motion.div>
    </div>
  );
}

interface BusinessListProps {
  dict: Dictionary;
  choice: NonNullable<SwitchBusinessState['choice']>;
  currentTenantId: string;
}

function BusinessList({ dict, choice, currentTenantId }: BusinessListProps) {
  const [state, formAction, pending] = useActionState(confirmSwitchAction, initialState);

  return (
    <div>
      <p className="text-sm text-ink-muted">{dict.business.chooseHint}</p>

      {state.error && <Notice message={state.error} tone="error" />}

      <ul className="mt-4 space-y-2">
        {choice.businesses.map((business) => {
          const current = business.tenantId === currentTenantId;

          return (
            <li key={business.tenantId}>
              <form action={formAction}>
                <input type="hidden" name="selectionToken" value={choice.selectionToken} />
                <input type="hidden" name="tenantId" value={business.tenantId} />
                <button
                  type="submit"
                  disabled={pending || current}
                  className="group flex w-full items-center gap-3 rounded-xl border border-line bg-panel-muted p-3 text-left transition-colors enabled:hover:border-brand/50 enabled:hover:bg-brand-soft disabled:cursor-default"
                >
                  <span className="grid size-9 shrink-0 place-items-center rounded-lg bg-brand text-white">
                    <Store size={17} aria-hidden />
                  </span>
                  <span className="min-w-0 flex-1">
                    <span className="block truncate text-sm font-semibold text-ink">
                      {business.name}
                    </span>
                    <span className="block truncate text-xs text-ink-muted">
                      {current ? dict.business.currentBadge : business.slug}
                    </span>
                  </span>
                  {current ? (
                    <Check size={17} aria-hidden className="shrink-0 text-brand" />
                  ) : (
                    <ChevronRight
                      size={17}
                      aria-hidden
                      className="shrink-0 text-ink-muted transition-transform group-hover:translate-x-0.5"
                    />
                  )}
                </button>
              </form>
            </li>
          );
        })}
      </ul>

      <p className="mt-4 text-xs text-ink-muted">
        {format(dict.business.countHint, { count: choice.businesses.length })}
      </p>
    </div>
  );
}

function Notice({ message, tone }: { message: string; tone: 'error' | 'info' }) {
  return (
    <p
      role={tone === 'error' ? 'alert' : 'status'}
      className={`mt-4 flex items-start gap-2 rounded-xl p-3 text-sm ${
        tone === 'error'
          ? 'bg-red-50 text-red-800 dark:bg-red-950/40 dark:text-red-200'
          : 'bg-brand-soft text-brand-strong'
      }`}
    >
      <AlertCircle size={17} aria-hidden className="mt-0.5 shrink-0" />
      {message}
    </p>
  );
}
