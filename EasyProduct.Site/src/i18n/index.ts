import { createI18n } from 'vue-i18n'
import enCommon from './en-US/common.json'
import enSite from './en-US/site.json'
import zhCommon from './zh-CN/common.json'
import zhSite from './zh-CN/site.json'

export const SUPPORT_LOCALES = ['zh-CN', 'en-US'] as const
export type Locale = (typeof SUPPORT_LOCALES)[number]

const LOCALE_KEY = 'locale'
const MODULES = ['common', 'site'] as const

const messages = {
  'zh-CN': { common: zhCommon, site: zhSite },
  'en-US': { common: enCommon, site: enSite },
}

export const i18n = createI18n({
  legacy: false,
  locale: (localStorage.getItem(LOCALE_KEY) as Locale) || 'zh-CN',
  fallbackLocale: 'zh-CN',
  messages,
})

/** 启动时拉取远程语言包覆盖内置基线（免发布维护；失败静默用基线） */
export async function refreshRemoteOverrides(): Promise<void> {
  const locale = i18n.global.locale.value as Locale
  for (const module of MODULES) {
    try {
      const res = await fetch(`/api/i18n/${locale}/${module}.json`)
      if (res.ok) {
        const data = (await res.json()) as Record<string, unknown>
        i18n.global.mergeLocaleMessage(locale, { [module]: data })
      }
    } catch {
      // 离线/失败：用包内基线
    }
  }
}

export function setLocale(locale: Locale): void {
  i18n.global.locale.value = locale
  localStorage.setItem(LOCALE_KEY, locale)
  document.documentElement.lang = locale
}
