import { post, get } from '@/utils/request'
import type { LoginParams, LoginResult } from '@/types/basic'
import type { MenuItem } from '@/types/basic'

/** 管理端登录 */
export const login = (data: LoginParams) => post<LoginResult>('/api/admin/auth/login', data)

/** 刷新 accessToken */
export const refresh = (data: { refreshToken: string }) =>
  post<{ accessToken: string }>('/api/admin/auth/refresh', data)

/** 获取当前用户信息（用于刷新页面后恢复登录状态） */
export const getUserInfo = () =>
  get<{ user: LoginResult['user']; permissions: string[] }>('/api/admin/auth/user-info')

/** 获取当前用户菜单树 */
export const getMenuList = () => get<MenuItem[]>('/api/admin/auth/menu-list')