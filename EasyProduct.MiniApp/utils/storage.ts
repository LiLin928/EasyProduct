// utils/storage.ts

const MEMBER_TOKEN = 'member_token'
const MEMBER_INFO = 'member_info'
const LOCALE_CACHE = 'i18n_cache'

/** 获取会员 Token */
export const getMemberToken = (): string => wx.getStorageSync(MEMBER_TOKEN) || ''

/** 设置会员 Token */
export const setMemberToken = (token: string): void => wx.setStorageSync(MEMBER_TOKEN, token)

/** 清除会员 Token */
export const clearMemberToken = (): void => wx.removeStorageSync(MEMBER_TOKEN)

/** 移除会员 Token（alias） */
export const removeMemberToken = (): void => wx.removeStorageSync(MEMBER_TOKEN)

/** 获取会员信息 */
export const getMemberInfo = (): { id: string; nickName: string; avatar: string; level: string; points: number } | null => {
  return wx.getStorageSync(MEMBER_INFO) || null
}

/** 设置会员信息 */
export const setMemberInfo = (info: { id: string; nickName: string; avatar: string; level: string; points: number }): void => {
  wx.setStorageSync(MEMBER_INFO, info)
}

/** 清除会员信息 */
export const clearMemberInfo = (): void => {
  wx.removeStorageSync(MEMBER_INFO)
}

/** 获取本地化缓存 */
export const getLocaleCache = <T>(key: string): T | null => {
  const all = wx.getStorageSync(LOCALE_CACHE) as Record<string, T> | ''
  return all && all[key] ? all[key] : null
}

/** 设置本地化缓存 */
export const setLocaleCache = (key: string, value: unknown): void => {
  const all = (wx.getStorageSync(LOCALE_CACHE) as Record<string, unknown> | '') || {}
  const next = typeof all === 'string' ? {} : all
  next[key] = value
  wx.setStorageSync(LOCALE_CACHE, next)
}
