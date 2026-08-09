// utils/request.ts
import { ENV } from '../config/env'
import { getMemberToken } from './storage'
import { silentLogin } from './wx-login'
import type { ApiResponse } from '../types/api.types'

interface RequestOptions {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  data?: unknown
  /** 是否需要会员 Token（默认 true；wx-login 自身传 false） */
  needAuth?: boolean
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
  if (envelope.code === 401) throw Object.assign(new Error(envelope.message), { code: 401 })
  wx.showToast({ title: envelope.message, icon: 'none' })
  throw new Error(envelope.message)
}

export async function request<T>(options: RequestOptions): Promise<T> {
  const needAuth = options.needAuth !== false
  let token = needAuth ? getMemberToken() : ''
  try {
    return await rawRequest<T>(options, token)
  } catch (err) {
    if (needAuth && err instanceof Error && 'code' in err && err.code === 401) {
      token = await silentLogin() // 静默重登后重放一次
      return rawRequest<T>(options, token)
    }
    throw err
  }
}
