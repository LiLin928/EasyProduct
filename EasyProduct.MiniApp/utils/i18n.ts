// utils/i18n.ts
import zhCommon from '../i18n/zh-CN/common.json'
import enCommon from '../i18n/en-US/common.json'
import { getLocaleCache, setLocaleCache } from './storage'

export type MiniLocale = 'zh-CN' | 'en-US'

const BASELINE: Record<MiniLocale, Record<string, unknown>> = {
  'zh-CN': { common: zhCommon },
  'en-US': { common: enCommon },
}

let currentLocale: MiniLocale = 'zh-CN'
let merged: Record<string, unknown> = {}

function deepMerge(target: Record<string, unknown>, source: Record<string, unknown>): Record<string, unknown> {
  for (const key of Object.keys(source)) {
    const sv = source[key]
    const tv = target[key]
    if (sv && typeof sv === 'object' && !Array.isArray(sv) && tv && typeof tv === 'object') {
      target[key] = deepMerge({ ...(tv as Record<string, unknown>) }, sv as Record<string, unknown>)
    } else {
      target[key] = sv
    }
  }
  return target
}

function lookup(obj: Record<string, unknown>, keyPath: string): string {
  const parts = keyPath.split('.')
  let cur: unknown = obj
  for (const p of parts) {
    if (!cur || typeof cur !== 'object') return keyPath
    cur = (cur as Record<string, unknown>)[p]
  }
  return typeof cur === 'string' ? cur : keyPath
}

export function getLocale(): MiniLocale {
  return currentLocale
}

/** 取文案：key 形如 common.button.confirm */
export function t(key: string): string {
  return lookup(merged, key)
}

/** 启动时调用：系统语言 → 缓存远程包 → 内置基线合并远程覆盖 */
export async function initI18n(): Promise<void> {
  const sysInfo = wx.getSystemInfoSync()
  const sys = sysInfo.language || 'zh_CN'
  currentLocale = sys.toLowerCase().includes('en') ? 'en-US' : 'zh-CN'
  merged = deepMerge({}, BASELINE[currentLocale])

  const cacheKey = `remote:${currentLocale}`
  const cached = getLocaleCache<Record<string, unknown>>(cacheKey)
  if (cached) merged = deepMerge(merged, cached)

  try {
    const remote = await new Promise<Record<string, unknown>>((resolve, reject) => {
      wx.request({
        // 语言包托管在 /api/i18n（非 /api/app 分区），mock/生产同源
        url: `http://localhost:7700/api/i18n/${currentLocale}/common.json`,
        success: (res) => resolve(res.data as Record<string, unknown>),
        fail: reject,
      })
    })
    merged = deepMerge(merged, { common: remote })
    setLocaleCache(cacheKey, { common: remote })
  } catch {
    // 离线：内置基线 + 上次缓存
  }
}
