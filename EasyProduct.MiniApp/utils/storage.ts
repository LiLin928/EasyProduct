// utils/storage.ts
const MEMBER_TOKEN = 'member_token'
const LOCALE_CACHE = 'i18n_cache'

export const getMemberToken = (): string => wx.getStorageSync(MEMBER_TOKEN) || ''
export const setMemberToken = (token: string): void => wx.setStorageSync(MEMBER_TOKEN, token)
export const clearMemberToken = (): void => wx.removeStorageSync(MEMBER_TOKEN)

export const getLocaleCache = <T>(key: string): T | null => {
  const all = wx.getStorageSync(LOCALE_CACHE) as Record<string, T> | ''
  return all && all[key] ? all[key] : null
}
export const setLocaleCache = (key: string, value: unknown): void => {
  const all = (wx.getStorageSync(LOCALE_CACHE) as Record<string, unknown> | '') || {}
  const next = typeof all === 'string' ? {} : all
  next[key] = value
  wx.setStorageSync(LOCALE_CACHE, next)
}
