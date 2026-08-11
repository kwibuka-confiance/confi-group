'use client';

import { motion } from 'framer-motion';
import { AlertCircle, Hash, Package, Plus, Trash2, Wallet, X } from 'lucide-react';
import { useActionState, useEffect, useId, useRef, useState } from 'react';

import {
  createProductAction,
  type CreateProductState,
} from '@/app/(dashboard)/products/actions';
import { format, type Dictionary } from '@/lib/i18n/dictionaries';

const initialState: CreateProductState = {};

interface AddProductModalProps {
  onClose: () => void;
  dict: Dictionary;
}

/**
 * A self-contained dialog. Deliberately not a component-library modal: this
 * renders inline, so there is no portal to go wrong and the overlay is styled
 * with the same tokens as the rest of the panel.
 *
 * The caller mounts this only while the dialog is open, so a half-filled form is
 * discarded on close without any state to reset by hand.
 */
export function AddProductModal({ onClose, dict }: AddProductModalProps) {
  const [state, formAction, pending] = useActionState(createProductAction, initialState);

  // Rows are identified by key so removing one does not renumber the others.
  const [packRows, setPackRows] = useState<number[]>([]);
  const nextRowKey = useRef(0);

  // What the base unit is called drives the deposit label: "Deposit per BOTTLE".
  const [baseUnit, setBaseUnit] = useState('');

  // Unmounting on success discards the form; there is no local state to clear.
  useEffect(() => {
    if (state.ok) onClose();
  }, [state.ok, onClose]);

  useEffect(() => {
    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose();
    };
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [onClose]);

  const unitLabel = baseUnit.trim().toUpperCase() || 'EA';

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
        aria-labelledby="add-product-title"
        initial={{ opacity: 0, scale: 0.97, y: 8 }}
        animate={{ opacity: 1, scale: 1, y: 0 }}
        transition={{ duration: 0.16, ease: 'easeOut' }}
        className="relative my-4 w-full max-w-xl rounded-2xl border border-line bg-panel p-5 shadow-xl"
      >
        <div className="mb-4 flex items-center justify-between">
          <h2 id="add-product-title" className="font-display text-lg font-bold text-ink">
            {dict.products.add}
          </h2>
          <button
            type="button"
            onClick={onClose}
            aria-label={dict.products.close}
            className="grid size-8 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink"
          >
            <X size={17} aria-hidden />
          </button>
        </div>

        <form action={formAction} className="space-y-5">
          {state.error && (
            <p
              role="alert"
              className="flex items-start gap-2 rounded-xl bg-red-50 p-3 text-sm text-red-800 dark:bg-red-950/40 dark:text-red-200"
            >
              <AlertCircle size={17} aria-hidden className="mt-0.5 shrink-0" />
              {state.error}
            </p>
          )}

          <Section title={dict.products.basics}>
            <Field name="name" label={dict.products.columnProduct} icon={Package} required autoFocus />
            <div className="grid grid-cols-2 gap-3">
              <Field name="sku" label={dict.products.columnSku} icon={Hash} required />
              <Field
                name="baseUnitCode"
                label={dict.products.baseUnit}
                hint={dict.products.baseUnitHint}
                placeholder="EA"
                maxLength={16}
                value={baseUnit}
                onChange={setBaseUnit}
              />
            </div>
            <Field name="description" label={dict.products.descriptionLabel} />
          </Section>

          <Section title={dict.products.pricing}>
            <div className="grid grid-cols-[2fr_2fr_1fr] gap-3">
              <Field
                name="priceAmount"
                label={dict.products.sellingPrice}
                icon={Wallet}
                type="number"
                step="0.01"
                min="0"
                required
              />
              <Field
                name="costAmount"
                label={dict.products.costPrice}
                type="number"
                step="0.01"
                min="0"
              />
              <Field name="currencyCode" label="ISO" defaultValue="RWF" maxLength={3} required />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <label className="block">
                <span className="mb-1.5 block text-sm font-medium text-ink">
                  {dict.products.taxClassLabel}
                </span>
                <select
                  name="taxClass"
                  defaultValue="Standard"
                  className="w-full rounded-xl border border-line bg-panel-muted px-3 py-2.5 text-sm text-ink"
                >
                  <option value="Standard">{dict.products.taxStandard}</option>
                  <option value="Zero">{dict.products.taxZero}</option>
                  <option value="Exempt">{dict.products.taxExempt}</option>
                </select>
              </label>
              <Field
                name="depositAmount"
                label={format(dict.products.returnableLabel, { unit: unitLabel })}
                hint={dict.products.returnableHint}
                type="number"
                step="0.01"
                min="0"
              />
            </div>
          </Section>

          <Section title={dict.products.packagingSection} hint={dict.products.packHint}>
            {packRows.map((key) => (
              <div key={key} className="grid grid-cols-[1fr_1fr_1.4fr_auto] items-end gap-2">
                <Field name="packagingUnit" label={dict.products.packUnit} placeholder="CRATE" maxLength={16} />
                <Field
                  name="packagingQuantity"
                  label={dict.products.packQuantity}
                  type="number"
                  min="1"
                  step="1"
                  placeholder="24"
                />
                <Field
                  name="packagingPrice"
                  label={dict.products.packPrice}
                  type="number"
                  min="0"
                  step="0.01"
                />
                <button
                  type="button"
                  onClick={() => setPackRows((rows) => rows.filter((row) => row !== key))}
                  aria-label={dict.products.removePack}
                  title={dict.products.removePack}
                  className="mb-0.5 grid size-9 shrink-0 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink"
                >
                  <Trash2 size={16} aria-hidden />
                </button>
              </div>
            ))}

            <button
              type="button"
              onClick={() => setPackRows((rows) => [...rows, nextRowKey.current++])}
              className="inline-flex items-center gap-1.5 rounded-lg px-2 py-1.5 text-sm font-semibold text-brand transition-colors hover:bg-brand-soft"
            >
              <Plus size={15} aria-hidden />
              {dict.products.addPack}
            </button>
          </Section>

          <button
            type="submit"
            disabled={pending}
            className="w-full rounded-xl bg-brand px-4 py-3 text-sm font-semibold text-white transition-colors hover:bg-brand-strong disabled:opacity-60"
          >
            {pending ? dict.products.adding : dict.products.add}
          </button>
        </form>
      </motion.div>
    </div>
  );
}

function Section({
  title,
  hint,
  children,
}: {
  title: string;
  hint?: string;
  children: React.ReactNode;
}) {
  return (
    <fieldset className="space-y-3">
      <legend className="text-xs font-bold tracking-wide text-ink-muted uppercase">
        {title}
      </legend>
      {hint && <p className="text-xs text-ink-muted">{hint}</p>}
      {children}
    </fieldset>
  );
}

interface FieldProps {
  name: string;
  label: string;
  icon?: typeof Package;
  type?: string;
  hint?: string;
  placeholder?: string;
  defaultValue?: string;
  required?: boolean;
  step?: string;
  min?: string;
  maxLength?: number;
  autoFocus?: boolean;
  /** Supplying value and onChange makes the field controlled. */
  value?: string;
  onChange?: (value: string) => void;
}

function Field({ name, label, icon: Icon, type = 'text', hint, value, onChange, ...rest }: FieldProps) {
  const controlled = value !== undefined && onChange !== undefined;
  // Pack rows repeat the same field names, so the id cannot be derived from the
  // name: duplicates would point every row's label at the first row's input.
  const id = useId();

  return (
    <div>
      <label htmlFor={id} className="mb-1.5 block text-sm font-medium text-ink">
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
          id={id}
          name={name}
          type={type}
          {...(controlled ? { value, onChange: (e) => onChange(e.target.value) } : {})}
          {...rest}
          className={`w-full rounded-xl border border-line bg-panel-muted py-2.5 pr-3 text-sm text-ink transition-colors hover:border-brand/40 ${
            Icon ? 'pl-9' : 'pl-3'
          }`}
        />
      </div>
      {hint && <p className="mt-1 text-xs text-ink-muted">{hint}</p>}
    </div>
  );
}
