import { get, post, put, del } from '@/utils/request'
import type { User, UserQuery, UserCreateParams, UserUpdateParams } from '@/types/basic'
import type { PageResult } from '@/types/api'

/** 用户列表（分页） */
export const getUserList = (params: UserQuery) =>
  get<PageResult<User>>('/api/admin/basic/user/list', params)

/** 用户详情 */
export const getUserDetail = (id: string) => get<User>(`/api/admin/basic/user/${id}`)

/** 新增用户 */
export const createUser = (data: UserCreateParams) =>
  post<{ id: string }>('/api/admin/basic/user', data)

/** 编辑用户 */
export const updateUser = (id: string, data: UserUpdateParams) =>
  put<{ id: string }>(`/api/admin/basic/user/${id}`, data)

/** 删除用户 */
export const deleteUser = (id: string) => del<null>(`/api/admin/basic/user/${id}`)

/** 重置密码 */
export const resetPassword = (id: string) =>
  post<null>(`/api/admin/basic/user/${id}/reset-password`)