import { post } from '@/utils/request'
import type { LoginParams, LoginResult } from '@/types/basic'

/** 管理端登录 */
export const login = (data: LoginParams) => post<LoginResult>('/api/admin/auth/login', data)

/** 刷新 accessToken */
export const refresh = (data: { refreshToken: string }) =>
  post<{ accessToken: string }>('/api/admin/auth/refresh', data)
