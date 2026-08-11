'use client';

import { motion } from 'framer-motion';
import {
  AlertCircle,
  Building2,
  Check,
  Clock,
  Eye,
  EyeOff,
  Link2,
  Lock,
  Mail,
  Store,
  User,
} from 'lucide-react';
import { useActionState, useMemo, useState } from 'react';

import { signUpAction, type SignUpState } from './actions';
import { MIN_PASSWORD_LENGTH, SLUG_PATTERN, slugify } from '@/lib/api/tenants';
import { format, type Dictionary } from '@/lib/i18n/dictionaries';

const initialState: SignUpState = {};

interface Values {
  name: string;
  slug: string;
  firstBranchName: string;
  countryCode: string;
  currencyCode: string;
  defaultLanguage: string;
  timeZoneId: string;
  ownerFullName: string;
  ownerEmail: string;
  ownerPassword: string;
}

const defaults: Values = {
  name: '',
  slug: '',
  firstBranchName: 'Main Branch',
  countryCode: 'RW',
  currencyCode: 'RWF',
  defaultLanguage: 'en',
  timeZoneId: 'Africa/Kigali',
  ownerFullName: '',
  ownerEmail: '',
  ownerPassword: '',
};

const TOTAL_STEPS = 4;

/**
 * Guided sign-up. The person is walked through four short steps rather than one
 * long form, and everything is posted at once from the review step.
 */
export function SignUpForm({ dict }: { dict: Dictionary }) {
  const [state, formAction, pending] = useActionState(signUpAction, initialState);
  const [step, setStep] = useState(0);
  const [values, setValues] = useState<Values>(defaults);
  const [handleEdited, setHandleEdited] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [touched, setTouched] = useState(false);

  const set = <K extends keyof Values>(key: K, value: Values[K]) =>
    setValues((current) => ({ ...current, [key]: value }));

  const errors = useMemo(() => validate(values, dict), [values, dict]);

  const stepFields: Record<number, (keyof Values)[]> = {
    0: ['name', 'slug', 'firstBranchName'],
    1: ['countryCode', 'currencyCode', 'timeZoneId'],
    2: ['ownerFullName', 'ownerEmail', 'ownerPassword'],
    3: [],
  };

  const stepIsValid = stepFields[step]!.every((field) => !errors[field]);

  const next = () => {
    setTouched(true);
    if (stepIsValid) {
      setTouched(false);
      setStep((current) => Math.min(current + 1, TOTAL_STEPS - 1));
    }
  };

  const titles = [
    { title: dict.signUp.businessTitle, subtitle: dict.signUp.businessSubtitle },
    { title: dict.signUp.regionTitle, subtitle: dict.signUp.regionSubtitle },
    { title: dict.signUp.accountTitle, subtitle: dict.signUp.accountSubtitle },
    { title: dict.signUp.reviewTitle, subtitle: dict.signUp.reviewSubtitle },
  ];

  const labels = [
    dict.signUp.stepBusiness,
    dict.signUp.stepRegion,
    dict.signUp.stepAccount,
    dict.signUp.stepReview,
  ];

  const error = (field: keyof Values) => (touched ? errors[field] : undefined);

  return (
    <form action={formAction} className="w-full max-w-md">
      {/* Everything collected so far travels with the final submit. */}
      {Object.entries(values).map(([key, value]) => (
        <input key={key} type="hidden" name={key} value={value} />
      ))}

      <Progress labels={labels} step={step} />

      <p className="mt-6 text-xs font-semibold text-brand">
        {format(dict.signUp.stepOf, { current: step + 1, total: TOTAL_STEPS })}
      </p>
      <h1 className="mt-1 font-display text-2xl font-bold text-ink">{titles[step]!.title}</h1>
      <p className="mt-1 text-sm text-ink-muted">{titles[step]!.subtitle}</p>

      {state.error && (
        <p
          role="alert"
          className="mt-5 flex items-start gap-2 rounded-xl bg-red-50 p-3 text-sm text-red-800 dark:bg-red-950/40 dark:text-red-200"
        >
          <AlertCircle size={17} aria-hidden className="mt-0.5 shrink-0" />
          {state.error}
        </p>
      )}

      {/*
        Entrance only, keyed on the step. An exit animation here would need
        AnimatePresence, which keeps the previous step mounted while it plays —
        leaving two sets of inputs in the form and the wrong fields on screen.
      */}
      <motion.div
          key={step}
          initial={{ opacity: 0, x: 12 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.2, ease: 'easeOut' }}
          className="mt-5 space-y-3"
        >
          {step === 0 && (
            <>
              <Field
                label={dict.signUp.name}
                icon={Store}
                value={values.name}
                placeholder={dict.signUp.nameHint}
                error={error('name')}
                onChange={(value) => {
                  set('name', value);
                  if (!handleEdited) set('slug', slugify(value));
                }}
              />
              <Field
                label={dict.signUp.handle}
                icon={Link2}
                value={values.slug}
                hint={dict.signUp.handleHint}
                error={error('slug')}
                onChange={(value) => {
                  setHandleEdited(true);
                  set('slug', value);
                }}
              />
              <Field
                label={dict.signUp.branch}
                icon={Building2}
                value={values.firstBranchName}
                hint={dict.signUp.branchHint}
                error={error('firstBranchName')}
                onChange={(value) => set('firstBranchName', value)}
              />
            </>
          )}

          {step === 1 && (
            <>
              <div className="grid grid-cols-2 gap-3">
                <Field
                  label={dict.signUp.country}
                  value={values.countryCode}
                  hint={dict.signUp.countryHint}
                  error={error('countryCode')}
                  onChange={(value) => set('countryCode', value.toUpperCase())}
                />
                <Field
                  label={dict.signUp.currency}
                  value={values.currencyCode}
                  hint={dict.signUp.currencyHint}
                  error={error('currencyCode')}
                  onChange={(value) => set('currencyCode', value.toUpperCase())}
                />
              </div>
              <label className="block">
                <span className="mb-1.5 block text-sm font-medium text-ink">
                  {dict.signUp.language}
                </span>
                <select
                  value={values.defaultLanguage}
                  onChange={(event) => set('defaultLanguage', event.target.value)}
                  className="w-full rounded-xl border border-line bg-panel-muted px-3 py-3 text-sm text-ink"
                >
                  <option value="en">English</option>
                  <option value="rw">Kinyarwanda</option>
                  <option value="fr">Français</option>
                </select>
              </label>
              <Field
                label={dict.signUp.timeZone}
                icon={Clock}
                value={values.timeZoneId}
                error={error('timeZoneId')}
                onChange={(value) => set('timeZoneId', value)}
              />
            </>
          )}

          {step === 2 && (
            <>
              <Field
                label={dict.signUp.fullName}
                icon={User}
                value={values.ownerFullName}
                error={error('ownerFullName')}
                onChange={(value) => set('ownerFullName', value)}
              />
              <Field
                label={dict.signUp.email}
                icon={Mail}
                type="email"
                value={values.ownerEmail}
                error={error('ownerEmail')}
                onChange={(value) => set('ownerEmail', value)}
              />
              <Field
                label={dict.signUp.password}
                icon={Lock}
                type={showPassword ? 'text' : 'password'}
                value={values.ownerPassword}
                hint={format(dict.signUp.passwordHint, { min: MIN_PASSWORD_LENGTH })}
                error={error('ownerPassword')}
                onChange={(value) => set('ownerPassword', value)}
                trailing={
                  <button
                    type="button"
                    onClick={() => setShowPassword((current) => !current)}
                    aria-label={dict.signUp.password}
                    className="grid size-8 place-items-center rounded-lg text-ink-muted hover:text-ink"
                  >
                    {showPassword ? <EyeOff size={17} aria-hidden /> : <Eye size={17} aria-hidden />}
                  </button>
                }
              />
            </>
          )}

          {step === 3 && (
            <Review dict={dict} values={values} onEdit={setStep} fieldErrors={state.fieldErrors} />
          )}
      </motion.div>

      <div className="mt-6 flex gap-3">
        {step > 0 && (
          <button
            type="button"
            onClick={() => setStep((current) => current - 1)}
            disabled={pending}
            className="flex-1 rounded-xl border border-line px-4 py-3 text-sm font-semibold text-ink transition-colors hover:bg-panel-muted disabled:opacity-60"
          >
            {dict.signUp.back}
          </button>
        )}
        {step < TOTAL_STEPS - 1 ? (
          <button
            type="button"
            onClick={next}
            className="flex-[2] rounded-xl bg-brand px-4 py-3 text-sm font-semibold text-white transition-colors hover:bg-brand-strong"
          >
            {dict.signUp.continue}
          </button>
        ) : (
          <button
            type="submit"
            disabled={pending}
            className="flex-[2] rounded-xl bg-brand px-4 py-3 text-sm font-semibold text-white transition-colors hover:bg-brand-strong disabled:opacity-60"
          >
            {pending ? dict.signUp.creating : dict.signUp.create}
          </button>
        )}
      </div>

      <p className="mt-5 text-center text-sm text-ink-muted">
        {dict.signUp.haveBusiness}{' '}
        <a href="/sign-in" className="font-semibold text-brand hover:underline">
          {dict.signUp.signInLink}
        </a>
      </p>
    </form>
  );
}

function validate(values: Values, dict: Dictionary): Partial<Record<keyof Values, string>> {
  const errors: Partial<Record<keyof Values, string>> = {};
  const required = dict.errors.required;

  if (!values.name.trim()) errors.name = required;
  if (!values.slug.trim()) errors.slug = required;
  else if (!SLUG_PATTERN.test(values.slug.trim())) errors.slug = dict.signUp.invalidHandle;
  if (!values.firstBranchName.trim()) errors.firstBranchName = required;
  if (values.countryCode.trim().length !== 2) errors.countryCode = dict.signUp.invalidCode;
  if (values.currencyCode.trim().length !== 3) errors.currencyCode = dict.signUp.invalidCode;
  if (!values.timeZoneId.trim()) errors.timeZoneId = required;
  if (!values.ownerFullName.trim()) errors.ownerFullName = required;
  if (!/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(values.ownerEmail.trim())) {
    errors.ownerEmail = values.ownerEmail.trim() ? dict.signUp.invalidEmail : required;
  }
  if (values.ownerPassword.length < MIN_PASSWORD_LENGTH) {
    errors.ownerPassword = format(dict.signUp.passwordTooShort, { min: MIN_PASSWORD_LENGTH });
  }

  return errors;
}

function Progress({ labels, step }: { labels: string[]; step: number }) {
  return (
    <ol className="flex gap-1.5">
      {labels.map((label, index) => {
        const reached = index <= step;
        return (
          <li key={label} className="flex-1">
            <span
              aria-hidden
              className={`block h-1.5 rounded-full transition-colors ${
                reached ? 'bg-brand' : 'bg-line'
              }`}
            />
            <span
              className={`mt-1.5 block truncate text-[11px] font-semibold ${
                reached ? 'text-brand' : 'text-ink-muted'
              }`}
              aria-current={index === step ? 'step' : undefined}
            >
              {label}
            </span>
          </li>
        );
      })}
    </ol>
  );
}

interface ReviewProps {
  dict: Dictionary;
  values: Values;
  onEdit: (step: number) => void;
  fieldErrors?: Record<string, string[]>;
}

function Review({ dict, values, onEdit, fieldErrors }: ReviewProps) {
  const sections = [
    {
      step: 0,
      heading: dict.signUp.stepBusiness,
      rows: [
        [dict.signUp.name, values.name],
        [dict.signUp.handle, values.slug],
        [dict.signUp.branch, values.firstBranchName],
      ],
    },
    {
      step: 1,
      heading: dict.signUp.stepRegion,
      rows: [
        [dict.signUp.country, values.countryCode],
        [dict.signUp.currency, values.currencyCode],
        [dict.signUp.timeZone, values.timeZoneId],
      ],
    },
    {
      step: 2,
      heading: dict.signUp.stepAccount,
      rows: [
        [dict.signUp.fullName, values.ownerFullName],
        [dict.signUp.email, values.ownerEmail],
        [dict.signUp.password, '•'.repeat(values.ownerPassword.length)],
      ],
    },
  ];

  return (
    <div className="space-y-3">
      {fieldErrors && Object.keys(fieldErrors).length > 0 && (
        <p className="text-xs text-red-700 dark:text-red-300">
          {Object.keys(fieldErrors).join(', ')}
        </p>
      )}
      {sections.map((section) => (
        <div key={section.heading} className="rounded-xl border border-line bg-panel-muted p-3">
          <div className="mb-1.5 flex items-center justify-between">
            <h2 className="text-sm font-bold text-ink">{section.heading}</h2>
            <button
              type="button"
              onClick={() => onEdit(section.step)}
              className="rounded-lg px-2 py-1 text-xs font-semibold text-brand hover:bg-brand-soft"
            >
              {dict.signUp.edit}
            </button>
          </div>
          <dl className="space-y-1">
            {section.rows.map(([label, value]) => (
              <div key={label} className="flex gap-3 text-sm">
                <dt className="w-28 shrink-0 text-ink-muted">{label}</dt>
                <dd className="min-w-0 flex-1 truncate font-medium text-ink">{value || '—'}</dd>
              </div>
            ))}
          </dl>
        </div>
      ))}
      <p className="flex items-center gap-2 text-xs text-ink-muted">
        <Check size={14} aria-hidden className="text-brand" />
        {dict.signUp.reviewSubtitle}
      </p>
    </div>
  );
}

interface FieldProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  icon?: typeof Mail;
  type?: string;
  hint?: string;
  placeholder?: string;
  error?: string;
  trailing?: React.ReactNode;
}

function Field({
  label,
  value,
  onChange,
  icon: Icon,
  type = 'text',
  hint,
  placeholder,
  error,
  trailing,
}: FieldProps) {
  return (
    <div>
      <label className="mb-1.5 block text-sm font-medium text-ink">
        {label}
        <div className="relative mt-1.5">
          {Icon && (
            <Icon
              size={17}
              aria-hidden
              className="pointer-events-none absolute top-1/2 left-3 -translate-y-1/2 text-ink-muted"
            />
          )}
          <input
            type={type}
            value={value}
            placeholder={placeholder}
            onChange={(event) => onChange(event.target.value)}
            aria-invalid={error ? true : undefined}
            className={`w-full rounded-xl border bg-panel-muted py-3 text-sm text-ink transition-colors ${
              Icon ? 'pl-9' : 'pl-3'
            } ${trailing ? 'pr-10' : 'pr-3'} ${
              error ? 'border-red-500' : 'border-line hover:border-brand/40'
            }`}
          />
          {trailing && (
            <span className="absolute top-1/2 right-1.5 -translate-y-1/2">{trailing}</span>
          )}
        </div>
      </label>
      {error ? (
        <p className="mt-1 text-xs text-red-700 dark:text-red-300">{error}</p>
      ) : (
        hint && <p className="mt-1 text-xs text-ink-muted">{hint}</p>
      )}
    </div>
  );
}
