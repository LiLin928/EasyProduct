// src/composables/useLocale.ts
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/app'
import { SUPPORT_LOCALES } from '@/i18n'
import type { Locale } from '@/i18n'

export interface LocaleOption {
  value: Locale
  label: string
}

export interface UseLocaleReturn {
  locale: Locale
  locales: LocaleOption[]
  setLocale: (locale: Locale) => void
  t: (key: string, ...args: unknown[]) => string
}

/**
 * 语言切换 composable（签名对齐前端规范 2.4）
 */
export function useLocale(): UseLocaleReturn {
  const { t, locale } = useI18n()
  const appStore = useAppStore()

  const locales: LocaleOption[] = [
    { value: 'zh-CN', label: '中文' },
    { value: 'en-US', label: 'English' },
  ]

  const setLocale = (next: Locale): void => {
    appStore.switchLocale(next)
  }

  return {
    locale: locale.value as Locale,
    locales,
    setLocale,
    t: t as (key: string, ...args: unknown[]) => string,
  }
}
