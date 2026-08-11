'use client';

import { motion } from 'framer-motion';
import { AlertCircle, Eye, EyeOff, Link2, Lock, Mail } from 'lucide-react';
import { useActionState, useState } from 'react';

import { signInAction, type SignInState } from './actions';
import type { Dictionary } from '@/lib/i18n/dictionaries';

const initialState: SignInState = {};

export function SignInForm({ dict }: { dict: Dictionary }) {
  const [state, formAction, pending] = useActionState(signInAction, initialState);
  const [showPassword, setShowPassword] = useState(false);

  return (
    <motion.form
      action={formAction}
      initial={{ opacity: 0, y: 12 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.32, ease: 'easeOut' }}
      className="w-full max-w-sm"
    >
      <h1 className="font-display text-3xl font-bold text-ink">{dict.auth.signInTitle}</h1>
      <p className="mt-1 text-sm text-ink-muted">{dict.auth.signInSubtitle}</p>

      {state.error && (
        <p
          role="alert"
          className="mt-5 flex items-start gap-2 rounded-xl bg-red-50 p-3 text-sm text-red-800 dark:bg-red-950/40 dark:text-red-200"
        >
          <AlertCircle size={17} aria-hidden className="mt-0.5 shrink-0" />
          {state.error}
        </p>
      )}

      <div className="mt-6 space-y-3">
        <Field
          name="businessHandle"
          label={dict.auth.businessHandle}
          hint={dict.auth.businessHandleHint}
          icon={Link2}
          autoComplete="organization"
        />
        <Field
          name="email"
          type="email"
          label={dict.auth.email}
          icon={Mail}
          autoComplete="email"
        />
        <Field
          name="password"
          type={showPassword ? 'text' : 'password'}
          label={dict.auth.password}
          icon={Lock}
          autoComplete="current-password"
          trailing={
            <button
              type="button"
              onClick={() => setShowPassword((value) => !value)}
              aria-label={dict.auth.password}
              className="grid size-8 place-items-center rounded-lg text-ink-muted transition-colors hover:text-ink"
            >
              {showPassword ? <EyeOff size={17} aria-hidden /> : <Eye size={17} aria-hidden />}
            </button>
          }
        />
      </div>

      <button
        type="submit"
        disabled={pending}
        className="mt-6 w-full rounded-xl bg-brand px-4 py-3 text-sm font-semibold text-white transition-colors hover:bg-brand-strong disabled:opacity-60"
      >
        {pending ? dict.auth.signingIn : dict.auth.signIn}
      </button>
    </motion.form>
  );
}

interface FieldProps {
  name: string;
  label: string;
  icon: typeof Mail;
  type?: string;
  hint?: string;
  autoComplete?: string;
  trailing?: React.ReactNode;
}

function Field({ name, label, icon: Icon, type = 'text', hint, autoComplete, trailing }: FieldProps) {
  return (
    <div>
      <label htmlFor={name} className="mb-1.5 block text-sm font-medium text-ink">
        {label}
      </label>
      <div className="relative">
        <Icon
          size={17}
          aria-hidden
          className="pointer-events-none absolute top-1/2 left-3 -translate-y-1/2 text-ink-muted"
        />
        <input
          id={name}
          name={name}
          type={type}
          autoComplete={autoComplete}
          aria-describedby={hint ? `${name}-hint` : undefined}
          className="w-full rounded-xl border border-line bg-panel-muted py-3 pr-10 pl-9 text-sm text-ink transition-colors hover:border-brand/40"
        />
        {trailing && <span className="absolute top-1/2 right-1.5 -translate-y-1/2">{trailing}</span>}
      </div>
      {hint && (
        <p id={`${name}-hint`} className="mt-1 text-xs text-ink-muted">
          {hint}
        </p>
      )}
    </div>
  );
}
