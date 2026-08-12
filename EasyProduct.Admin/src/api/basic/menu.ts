import { get, post, put, del } from '@/utils/request'
import type { MenuItem, Menu, MenuCreateParams } from '@/types/basic'

/** 当前用户菜单树（侧边栏使用） */
export const getUserMenuTree = () => get<MenuItem[]>('/api/admin/basic/menu/tree')

/** 菜单树（菜单管理使用，包含完整字段） */
export const getMenuTree = () => get<Menu[]>('/api/admin/basic/menu/tree')

/** 菜单详情 */
export const getMenuDetail = (id: string) => get<Menu>(`/api/admin/basic/menu/${id}`)

/** 新增菜单 */
export const createMenu = (data: MenuCreateParams) =>
  post<{ id: string }>('/api/admin/basic/menu', data)

/** 编辑菜单 */
export const updateMenu = (id: string, data: MenuCreateParams) =>
  put<{ id: string }>(`/api/admin/basic/menu/${id}`, data)

/** 删除菜单 */
export const deleteMenu = (id: string) => del<null>(`/api/admin/basic/menu/${id}`)

/** 更新菜单排序 */
export const updateMenuSort = (dragId: string, dropId: string, type: 'prev' | 'next') =>
  post<null>('/api/admin/basic/menu/sort', { dragId, dropId, type })

/** 更新菜单状态 */
export const updateMenuStatus = (id: string, status: 'enabled' | 'disabled') =>
  put<null>(`/api/admin/basic/menu/${id}/status`, { status })

/** 更新菜单可见性 */
export const updateMenuVisible = (id: string, visible: boolean) =>
  put<null>(`/api/admin/basic/menu/${id}/visible`, { visible })
