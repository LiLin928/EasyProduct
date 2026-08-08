// src/data/basic.ts
// [backend: Basic 模块 | status: pending]
import { guid } from '../helpers/id.js'

export const ADMIN_USERS = [
  { id: guid(), userName: 'admin', password: 'admin123', realName: '系统管理员' },
  { id: guid(), userName: 'sales', password: 'sales123', realName: '销售专员' },
  { id: guid(), userName: 'ops', password: 'ops123', realName: '运维专员' },
]

/** 权限标识：super 用通配 '*'；其余账号 F2 Basic 模块时细化 */
export const USER_PERMISSIONS: Record<string, string[]> = {
  admin: ['*'],
  sales: ['crm:customer:list', 'crm:customer:add', 'mall:order:list'],
  ops: ['ops:log:list'],
}

/** 菜单树骨架：工作台 + 八大模块根（children 由 F2 Basic 模块补全） */
export const MENU_TREE = [
  { id: guid(), parentId: '0', name: 'desktop', path: '/desktop', titleKey: 'menu.desktop', icon: 'Monitor', sort: 1, children: [] },
  { id: guid(), parentId: '0', name: 'basic', path: '/basic', titleKey: 'menu.basic', icon: 'Setting', sort: 2, children: [] },
  { id: guid(), parentId: '0', name: 'product', path: '/product', titleKey: 'menu.product', icon: 'Goods', sort: 3, children: [] },
  { id: guid(), parentId: '0', name: 'site', path: '/site', titleKey: 'menu.site', icon: 'Monitor', sort: 4, children: [] },
  { id: guid(), parentId: '0', name: 'mall', path: '/mall', titleKey: 'menu.mall', icon: 'ShoppingCart', sort: 5, children: [] },
  { id: guid(), parentId: '0', name: 'crm', path: '/crm', titleKey: 'menu.crm', icon: 'User', sort: 6, children: [] },
  { id: guid(), parentId: '0', name: 'workflow', path: '/workflow', titleKey: 'menu.workflow', icon: 'Connection', sort: 7, children: [] },
  { id: guid(), parentId: '0', name: 'report', path: '/report', titleKey: 'menu.report', icon: 'DataLine', sort: 8, children: [] },
  { id: guid(), parentId: '0', name: 'ops', path: '/ops', titleKey: 'menu.ops', icon: 'Document', sort: 9, children: [] },
]

/** 字典种子：labelKey 走 i18n，前端 useDict 用 t(labelKey) 渲染 */
export const DICT_DATA: Record<string, Array<{ value: string; labelKey: string; sort: number }>> = {
  common_status: [
    { value: 'enabled', labelKey: 'common.status.enabled', sort: 1 },
    { value: 'disabled', labelKey: 'common.status.disabled', sort: 2 },
  ],
  customer_type: [
    { value: 'b2b', labelKey: 'common.dict.customerType.b2b', sort: 1 },
    { value: 'retail', labelKey: 'common.dict.customerType.retail', sort: 2 },
  ],
}
