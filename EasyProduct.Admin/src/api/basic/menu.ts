import { get } from '@/utils/request'
import type { MenuItem } from '@/types/basic'

/** 当前用户菜单树 */
export const getMenuList = () => get<MenuItem[]>('/api/admin/basic/menu/list')
