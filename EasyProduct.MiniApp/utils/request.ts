// utils/request.ts
import { ENV } from '../config/env'
import { getMemberToken, clearMemberToken, clearMemberInfo } from './storage'
import { silentLogin } from './wx-login'
import type { ApiResponse } from '../types/api.types'

interface RequestOptions {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  data?: unknown
  /** 是否需要会员 Token（默认 true；wx-login 自身传 false） */
  needAuth?: boolean
}

/** 获取当前页面路径 */
function getCurrentPagePath(): string {
  const pages = getCurrentPages()
  if (pages.length === 0) return ''
  const currentPage = pages[pages.length - 1]
  const route = currentPage.route || ''
  const options = currentPage.options || {}
  const queryString = Object.keys(options)
    .map(key => `${key}=${encodeURIComponent(options[key])}`)
    .join('&')
  return queryString ? `/${route}?${queryString}` : `/${route}`
}

/** 跳转到登录页 */
function redirectToLogin(fromPage?: string): void {
  // 清除旧的登录信息
  clearMemberToken()
  clearMemberInfo()
  
  const currentPath = fromPage || getCurrentPagePath()
  const loginUrl = currentPath 
    ? `/pages/login/login?from=redirect&redirect=${encodeURIComponent(currentPath)}`
    : '/pages/login/login'
  
  wx.navigateTo({ url: loginUrl })
}

async function rawRequest<T>(options: RequestOptions, token: string): Promise<T> {
  const res = await new Promise<WechatMiniprogram.RequestSuccessCallbackResult>((resolve, reject) => {
    wx.request({
      url: `${ENV.apiBase}${options.url}`,
      method: options.method ?? 'GET',
      data: options.data as WechatMiniprogram.IAnyObject | undefined,
      header: token ? { Authorization: `Bearer ${token}` } : {},
      success: resolve,
      fail: () => reject(new Error('network error')),
    })
  })
  const envelope = res.data as ApiResponse<T>
  if (envelope.code === 200) return envelope.data
  if (envelope.code === 401) {
    // 清除登录状态并跳转登录页
    clearMemberToken()
    clearMemberInfo()
    throw Object.assign(new Error(envelope.message), { code: 401, needLogin: true })
  }
  wx.showToast({ title: envelope.message, icon: 'none' })
  throw new Error(envelope.message)
}

export async function request<T>(options: RequestOptions): Promise<T> {
  const needAuth = options.needAuth !== false
  let token = needAuth ? getMemberToken() : ''
  
  // 如果没有token且需要认证，直接跳转登录页
  if (needAuth && !token) {
    redirectToLogin()
    throw new Error('请先登录')
  }
  
  try {
    return await rawRequest<T>(options, token)
  } catch (err) {
    if (needAuth && err instanceof Error && 'code' in err && err.code === 401) {
      // 尝试静默登录
      try {
        token = await silentLogin()
        return rawRequest<T>(options, token)
      } catch (loginErr) {
        // 静默登录失败，跳转到登录页
        redirectToLogin()
        throw new Error('登录已过期，请重新登录')
      }
    }
    throw err
  }
}

/** 检查是否已登录 */
export function checkLogin(): boolean {
  return !!getMemberToken()
}

/** 退出登录 */
export function logout(): void {
  clearMemberToken()
  clearMemberInfo()
  wx.showToast({ title: '已退出登录', icon: 'success' })
  setTimeout(() => {
    wx.switchTab({ url: '/pages/index/index' })
  }, 800)
}
