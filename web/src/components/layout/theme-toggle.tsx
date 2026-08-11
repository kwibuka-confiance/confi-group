'use client';

import { Moon, Sun } from 'lucide-react';

import { useTheme } from '@/app/theme-provider';

export function ThemeToggle({ label }: { label: string }) {
  const { theme, toggle } = useTheme();
  const isDark = theme === 'dark';

  return (
    <button
      type="button"
      aria-label={label}
      title={label}
      onClick={toggle}
      className="grid size-9 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink"
    >
      {isDark ? <Sun size={18} aria-hidden /> : <Moon size={18} aria-hidden />}
    </button>
  );
}
