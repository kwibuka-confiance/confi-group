'use client';

import { motion } from 'framer-motion';
import { AlertCircle, ArrowLeft, ChevronRight, Eye, EyeOff, Lock, Mail, Store } from 'lucide-react';
import { useActionState, useState } from 'react';

import { selectBusinessAction, signInAction, type SignInState } from './actions';
import type { Dictionary } from '@/lib/i18n/dictionaries';

const initialState: SignInState = {};

export function SignInForm({ dict }: { dict: Dictionary }) {
  const [state, formAction, pending] = useActionState(signInAction, initialState);

  // The credentials matched several businesses, so the person picks one. The
  // password is not asked for again: the selection token carries that check.
  if (state.choice) {
    return <BusinessChooser dict={dict} choice={state.choice} />;
  }

  return <CredentialsForm dict={dict} state={state} formAction={formAction} pending={pending} />;
}

interface CredentialsFormProps {
  dict: Dictionary;
  state: SignInState;
  formAction: (payload: FormData) => void;
  pending: boolean;
}

function CredentialsForm({ dict, state, formAction, pending }: CredentialsFormProps) {
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

      {state.error && <ErrorNotice message={state.error} />}

      <div className="mt-6 space-y-3">
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

interface BusinessChooserProps {
  dict: Dictionary;
  choice: NonNullable<SignInState['choice']>;
}

function BusinessChooser({ dict, choice }: BusinessChooserProps) {
  const [state, formAction, pending] = useActionState(selectBusinessAction, initialState);

  return (
    <motion.div
      initial={{ opacity: 0, y: 12 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.32, ease: 'easeOut' }}
      className="w-full max-w-sm"
    >
      <h1 className="font-display text-3xl font-bold text-ink">
        {dict.auth.chooseBusinessTitle}
      </h1>
      <p className="mt-1 text-sm text-ink-muted">{dict.auth.chooseBusinessSubtitle}</p>

      {state.error && <ErrorNotice message={state.error} />}

      <ul className="mt-6 space-y-2">
        {choice.businesses.map((business, index) => (
          <motion.li
            key={business.tenantId}
            initial={{ opacity: 0, y: 8 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.22, delay: index * 0.05 }}
          >
            <form action={formAction}>
              <input type="hidden" name="selectionToken" value={choice.selectionToken} />
              <input type="hidden" name="tenantId" value={business.tenantId} />
              <button
                type="submit"
                disabled={pending}
                className="group flex w-full items-center gap-3 rounded-xl border border-line bg-panel-muted p-3 text-left transition-colors hover:border-brand/50 hover:bg-brand-soft disabled:opacity-60"
              >
                <span className="grid size-10 shrink-0 place-items-center rounded-lg bg-brand text-white">
                  <Store size={19} aria-hidden />
                </span>
                <span className="min-w-0 flex-1">
                  <span className="block truncate text-sm font-semibold text-ink">
                    {business.name}
                  </span>
                  <span className="block truncate text-xs text-ink-muted">{business.slug}</span>
                </span>
                <ChevronRight
                  size={18}
                  aria-hidden
                  className="shrink-0 text-ink-muted transition-transform group-hover:translate-x-0.5"
                />
              </button>
            </form>
          </motion.li>
        ))}
      </ul>

      <a
        href="/sign-in"
        className="mt-5 inline-flex items-center gap-1.5 text-sm font-medium text-ink-muted transition-colors hover:text-ink"
      >
        <ArrowLeft size={15} aria-hidden />
        {dict.auth.backToSignIn}
      </a>
    </motion.div>
  );
}

function ErrorNotice({ message }: { message: string }) {
  return (
    <p
      role="alert"
      className="mt-5 flex items-start gap-2 rounded-xl bg-red-50 p-3 text-sm text-red-800 dark:bg-red-950/40 dark:text-red-200"
    >
      <AlertCircle size={17} aria-hidden className="mt-0.5 shrink-0" />
      {message}
    </p>
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
