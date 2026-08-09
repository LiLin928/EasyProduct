import { createI18n } from 'vue-i18n'
import enCommon from './en-US/common.json'
import enMenu from './en-US/menu.json'
import zhCommon from './zh-CN/common.json'
import zhMenu from './zh-CN/menu.json'

export const SUPPORT_LOCALES = ['zh-CN', 'en-US'] as const
export type Locale = (typeof SUPPORT_LOCALES)[number]

const LOCALE_KEY = 'locale'

const messages = {
  'zh-CN': { common: zhCommon, menu: zhMenu },
  'en-US': { common: enCommon, menu: enMenu },
}

export const i18n = createI18n({
  legacy: false,
  locale: (localStorage.getItem(LOCALE_KEY) as Locale) || 'zh-CN',
  fallbackLocale: 'zh-CN',
  messages,
})

const loaded = new Set<string>()

/** 加载业务模块语言包：远程覆盖优先，失败用包内基线（免发布维护机制） */
export async function loadModuleLocale(module: string): Promise<void> {
  const locale = i18n.global.locale.value as Locale
  const key = `${locale}:${module}`
  if (loaded.has(key)) return
  try {
    const res = await fetch(`/api/i18n/${locale}/${module}.json`)
    if (res.ok) {
      const data = (await res.json()) as Record<string, unknown>
      i18n.global.mergeLocaleMessage(locale, { [module]: data })
    }
  } catch {
    // 远程失败：使用包内基线，不阻塞
  }
  loaded.add(key)
}

export function setLocale(locale: Locale): void {
  i18n.global.locale.value = locale
  localStorage.setItem(LOCALE_KEY, locale)
  document.documentElement.lang = locale
}

/** 供非组件上下文（如 utils/request）使用的翻译函数 */
export const tStandalone = (key: string): string => i18n.global.t(key)
