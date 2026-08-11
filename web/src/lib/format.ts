/**
 * Locale-aware money formatting. The ISO code is shown alongside the amount
 * rather than a symbol, so currencies are never ambiguous.
 */
export function formatMoney(amount: number, currencyCode: string, locale: string): string {
  const formatted = new Intl.NumberFormat(locale, {
    maximumFractionDigits: 2,
  }).format(amount);
  return `${formatted} ${currencyCode}`;
}

export function formatCount(value: number, locale: string): string {
  return new Intl.NumberFormat(locale).format(value);
}

/**
 * Two-letter initials for an avatar, or "?" when the name is unusable.
 *
 * Lives in a shared module rather than a client component so Server Components
 * can call it too.
 */
export function initialsOf(fullName: string): string {
  const parts = fullName.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  if (parts.length === 1) return parts[0]!.charAt(0).toUpperCase();
  return (parts[0]!.charAt(0) + parts[parts.length - 1]!.charAt(0)).toUpperCase();
}
