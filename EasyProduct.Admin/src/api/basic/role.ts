import { get, post, put, del } from '@/utils/request'
import type { Role, RoleQuery, RoleCreateParams } from '@/types/basic'
import type { PageResult } from '@/types/api'

/** 角色列表（分页） */
export const getRoleList = (params: RoleQuery) =>
  get<PageResult<Role>>('/api/admin/basic/role/list', params)

/** 角色详情 */
export const getRoleDetail = (id: string) => get<Role>(`/api/admin/basic/role/${id}`)

/** 新增角色 */
export const createRole = (data: RoleCreateParams) =>
  post<{ id: string }>('/api/admin/basic/role', data)

/** 编辑角色 */
export const updateRole = (id: string, data: RoleCreateParams) =>
  put<{ id: string }>(`/api/admin/basic/role/${id}`, data)

/** 删除角色 */
export const deleteRole = (id: string) => del<null>(`/api/admin/basic/role/${id}`)

/** 分配菜单 */
export const assignMenus = (id: string, menuIds: string[]) =>
  post<null>(`/api/admin/basic/role/${id}/menus`, { menuIds })

/** 获取角色的菜单ID列表 */
export const getRoleMenus = (id: string) =>
  get<string[]>(`/api/admin/basic/role/${id}/menus`)