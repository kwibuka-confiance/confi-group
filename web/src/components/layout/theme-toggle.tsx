'use client';

import { useMantineColorScheme } from '@mantine/core';
import { Moon, Sun } from 'lucide-react';

interface ThemeToggleProps {
  label: string;
}

export function ThemeToggle({ label }: ThemeToggleProps) {
  const { colorScheme, setColorScheme } = useMantineColorScheme();
  const isDark = colorScheme === 'dark';

  return (
    <button
      type="button"
      aria-label={label}
      title={label}
      onClick={() => setColorScheme(isDark ? 'light' : 'dark')}
      className="grid size-9 place-items-center rounded-lg text-ink-muted transition-colors hover:bg-panel-muted hover:text-ink"
    >
      {isDark ? <Sun size={18} aria-hidden /> : <Moon size={18} aria-hidden />}
    </button>
  );
}
