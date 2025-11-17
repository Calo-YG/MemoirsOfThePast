import { ref, computed, watchEffect } from 'vue';

export type ThemeMode = 'light' | 'dark' | 'system';

const THEME_KEY = 'APP_THEME_MODE';

const media = typeof window !== 'undefined'
  ? window.matchMedia('(prefers-color-scheme: dark)')
  : null;

function systemTheme(): 'light' | 'dark' {
  return media?.matches ? 'dark' : 'light';
}

const saved = (typeof localStorage !== 'undefined'
  ? (localStorage.getItem(THEME_KEY) as ThemeMode | null)
  : null) ?? 'system';

const mode = ref<ThemeMode>(saved);

const resolved = computed<'light' | 'dark'>(() => {
  return mode.value === 'system' ? systemTheme() : mode.value;
});

const isDark = computed<boolean>(() => resolved.value === 'dark');

function applyTheme(theme: 'light' | 'dark') {
  if (typeof document === 'undefined') return;
  const el = document.documentElement;
  el.setAttribute('data-theme', theme);
  // 让表单控件等遵循主题（Chrome/Chromium）
  (el as HTMLElement).style.colorScheme = theme;
}

let mediaListenerAttached = false;
function ensureSystemListener() {
  if (!media || mediaListenerAttached) return;
  const handler = () => {
    if (mode.value === 'system') {
      applyTheme(systemTheme());
    }
  };
  media.addEventListener?.('change', handler);
  mediaListenerAttached = true;
}

export function setTheme(next: ThemeMode) {
  mode.value = next;
  try {
    localStorage.setItem(THEME_KEY, next);
  } catch {}
  if (next === 'system') {
    ensureSystemListener();
    applyTheme(systemTheme());
  } else {
    applyTheme(next);
  }
}

export function toggleTheme() {
  // 系统模式下以当前解算结果为基准进行明暗切换，并切换为显式模式
  const target: 'light' | 'dark' = resolved.value === 'dark' ? 'light' : 'dark';
  setTheme(target);
}

export function getThemeMode(): ThemeMode {
  return mode.value;
}

export { isDark, mode as themeMode, resolved as resolvedTheme };

// 初始化应用主题
(function init() {
  if (saved === 'system') {
    ensureSystemListener();
  }
  applyTheme(resolved.value);
})();

// 当模式发生变化时，自动应用
watchEffect(() => {
  applyTheme(resolved.value);
});