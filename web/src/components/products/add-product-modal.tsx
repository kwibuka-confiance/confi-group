'use client';

import { motion } from 'framer-motion';
import { AlertCircle, Hash, Package, Wallet, X } from 'lucide-react';
import { useActionState, useEffect, useRef } from 'react';

import {
  createProductAction,
  type CreateProductState,
} from '@/app/(dashboard)/products/actions';
import type { Dictionary } from '@/lib/i18n/dictionaries';

const initialState: CreateProductState = {};

interface AddProductModalProps {
  opened: boolean;
  onClose: () => void;
  dict: Dictionary;
}

/**
 * A self-contained dialog. Deliberately not a component-library modal: this
 * renders inline, so there is no portal to go wrong and the overlay is styled
 * with the same tokens as the rest of the panel.
 */
export function AddProductModal({ opened, onClose, dict }: AddProductModalProps) {
  const [state, formAction, pending] = useActionState(createProductAction, initialState);
  const formRef = useRef<HTMLFormElement>(null);

  // The server revalidated the catalog, so closing is all that is left to do.
  useEffect(() => {
    if (state.ok) {
      formRef.current?.reset();
      onClose();
    }
  }, [state.ok, onClose]);

  // Escape closes, matching what people expect of a dialog.
  useEffect(() => {
    if (!opened) return;
    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose();
    };
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [opened, onClose]);

  if (!opened) return null;

  return (
    <div className="fixed inset-0 z-50 grid place-items-center p-4">
      <button
        type="button"
        aria-hidden
        tabIndex={-1}
        onClick={onClose}
        className="absolute inset-0 cursor-default bg-black/50"
      />

      <motion.div
        role="dialog"
        aria-modal="true"
        aria-labelledby="add-product-title"
        initial={{ opacity: 0, scale: 0.97, y: 8 }}
        animate={{ opacity: 1, scale: 1, y: 0 }}
        transition={{ duration: 0.16, ease: 'easeOut' }}
        className="relative w-full max-w-md rounded-2xl border border-line bg-panel p-5 shadow-xl"
      >
        <div className="mb-4 flex items-center justify-between">
          <h2 id="add-product-title" className="font-display text-lg font-bold text-ink">
            {dict.products.add}
          </h2>
          <button
            type="button"
            onClick={onClose}
            aria-label={dict.products.add}
            className="grid size-8 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink"
          >
            <X size={17} aria-hidden />
          </button>
        </div>

        <form ref={formRef} action={formAction} className="space-y-3">
          {state.error && (
            <p
              role="alert"
              className="flex items-start gap-2 rounded-xl bg-red-50 p-3 text-sm text-red-800 dark:bg-red-950/40 dark:text-red-200"
            >
              <AlertCircle size={17} aria-hidden className="mt-0.5 shrink-0" />
              {state.error}
            </p>
          )}

          <Field name="name" label={dict.products.columnProduct} icon={Package} required autoFocus />
          <Field name="sku" label={dict.products.columnSku} icon={Hash} required />

          <div className="grid grid-cols-[2fr_1fr] gap-3">
            <Field
              name="priceAmount"
              label={dict.products.columnPrice}
              icon={Wallet}
              type="number"
              step="0.01"
              min="0"
              required
            />
            <Field name="currencyCode" label="ISO" defaultValue="RWF" maxLength={3} required />
          </div>

          <button
            type="submit"
            disabled={pending}
            className="mt-2 w-full rounded-xl bg-brand px-4 py-3 text-sm font-semibold text-white transition-colors hover:bg-brand-strong disabled:opacity-60"
          >
            {pending ? dict.products.adding : dict.products.add}
          </button>
        </form>
      </motion.div>
    </div>
  );
}

interface FieldProps {
  name: string;
  label: string;
  icon?: typeof Package;
  type?: string;
  defaultValue?: string;
  required?: boolean;
  step?: string;
  min?: string;
  maxLength?: number;
  autoFocus?: boolean;
}

function Field({ name, label, icon: Icon, type = 'text', ...rest }: FieldProps) {
  return (
    <div>
      <label htmlFor={name} className="mb-1.5 block text-sm font-medium text-ink">
        {label}
      </label>
      <div className="relative">
        {Icon && (
          <Icon
            size={16}
            aria-hidden
            className="pointer-events-none absolute top-1/2 left-3 -translate-y-1/2 text-ink-muted"
          />
        )}
        <input
          id={name}
          name={name}
          type={type}
          {...rest}
          className={`w-full rounded-xl border border-line bg-panel-muted py-2.5 pr-3 text-sm text-ink transition-colors hover:border-brand/40 ${
            Icon ? 'pl-9' : 'pl-3'
          }`}
        />
      </div>
    </div>
  );
}
