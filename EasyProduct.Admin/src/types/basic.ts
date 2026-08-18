import type { PageQuery } from './api'

/** 登录入参 */
export interface LoginParams {
  userName: string
  password: string
}

/** 登录结果 */
export interface LoginResult {
  accessToken: string
  refreshToken: string
  user: { id: string; userName: string; realName: string }
  permissions: string[]
}

/** 菜单项（树） */
export interface MenuItem {
  id: string
  parentId: string
  name: string
  path: string
  titleKey: string
  icon: string
  sort: number
  children: MenuItem[]
}

/** 字典项（labelKey 走 i18n 渲染） */
export interface DictItem {
  value: string
  labelKey: string
  sort: number
}

// ==================== 用户管理 ====================

/** 用户实体 */
export interface User {
  id: string // GUID
  userName: string // 登录账号
  password?: string // 密码（仅新增/编辑时传入）
  realName: string // 真实姓名
  email: string // 邮箱
  phone: string // 手机号
  status: 'enabled' | 'disabled' // 状态
  deptId: string // 部门ID（GUID）
  deptName?: string // 部门名称（列表查询返回）
  roleIds: string[] // 角色ID列表（GUID数组）
  roleNames?: string[] // 角色名称列表（列表查询返回）
  createdAt: string // 创建时间（ISO 8601）
  updatedAt: string // 更新时间（ISO 8601）
}

/** 用户查询参数 */
export interface UserQuery {
  pageIndex: number
  pageSize: number
  userName?: string
  realName?: string
  status?: string
  deptId?: string
}

/** 用户创建参数 */
export interface UserCreateParams {
  userName: string
  password: string
  realName: string
  email: string
  phone: string
  status: 'enabled' | 'disabled'
  deptId: string
  roleIds: string[]
}

/** 用户更新参数 */
export interface UserUpdateParams extends Partial<UserCreateParams> {
  // password 可选（修改密码时传入）
}

// ==================== 角色管理 ====================

/** 角色实体 */
export interface Role {
  id: string // GUID
  name: string // 角色名称
  code: string // 角色编码
  status: 'enabled' | 'disabled' // 状态
  sort: number // 排序
  remark: string // 备注
  menuIds: string[] // 分配的菜单ID列表（GUID数组）
  createdAt: string // 创建时间（ISO 8601）
  updatedAt: string // 更新时间（ISO 8601）
}

/** 角色查询参数 */
export interface RoleQuery {
  pageIndex: number
  pageSize: number
  name?: string
  code?: string
  status?: string
}

/** 角色创建参数 */
export interface RoleCreateParams {
  name: string
  code: string
  status: 'enabled' | 'disabled'
  sort: number
  remark: string
  menuIds: string[]
}

// ==================== 菜单管理 ====================

/** 菜单实体 */
export interface Menu {
  id: string // GUID
  parentId: string // 父菜单ID（GUID，根节点为 '0'）
  name: string // 路由 name
  path: string // 路由 path
  titleKey: string // i18n key（如 'menu.basic.user'）
  icon: string // 图标名称
  sort: number // 排序
  permission?: string // 权限标识（按钮级，如 'basic:user:edit'）
  component?: string // 组件路径（如 'basic/user/index'）
  visible: boolean // 是否显示在菜单
  status: 'enabled' | 'disabled' // 状态
  children?: Menu[] // 子菜单
}

/** 菜单创建参数 */
export interface MenuCreateParams {
  parentId: string
  name: string
  path: string
  titleKey: string
  icon: string
  sort: number
  permission?: string
  component?: string
  visible: boolean
  status: 'enabled' | 'disabled'
}

// ==================== 部门管理 ====================

/** 部门（完整模型） */
export interface Dept {
  id: string
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  leaderName?: string // 部门负责人
  phone?: string // 联系电话
  email?: string // 邮箱
  fullPath?: string // 部门路径（如：总公司/技术部/前端组）
  level?: number // 层级
  memberCount?: number // 成员数量
  description?: string // 描述
  children?: Dept[]
}

/** 部门创建参数 */
export interface DeptCreateParams {
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  leaderName?: string
  phone?: string
  email?: string
  description?: string
}

/** 部门更新参数 */
export type DeptUpdateParams = Partial<DeptCreateParams>

// ==================== 字典管理 ====================

/** 字典类型 */
export interface DictType {
  id: string
  name: string
  code: string
  status: 'enabled' | 'disabled'
  remark: string
}

/** 字典类型查询参数 */
export interface DictTypeQuery extends PageQuery {
  name?: string
  code?: string
}

/** 字典类型创建参数 */
export interface DictTypeCreateParams {
  name: string
  code: string
  status: 'enabled' | 'disabled'
  remark: string
}

/** 字典数据（管理页面用） */
export interface DictData {
  id: string
  typeCode: string
  value: string
  labelKey: string
  sort: number
  status: 'enabled' | 'disabled'
}

/** 字典数据查询参数 */
export interface DictDataQuery extends PageQuery {
  typeCode: string
}

/** 字典数据创建参数 */
export interface DictDataCreateParams {
  typeCode: string
  value: string
  labelKey: string
  sort: number
  status: 'enabled' | 'disabled'
}

// ==================== 系统参数 ====================

/** 系统参数值类型（驱动编辑控件） */
export type SystemConfigType = 'string' | 'number' | 'boolean'

/** 系统参数实体 */
export interface SystemConfig {
  id: string // GUID
  key: string // 参数键（唯一）
  label: string // 参数名称
  value: string // 参数值
  type: SystemConfigType // 值类型
  remark: string // 备注
  createdAt: string // 创建时间（ISO 8601）
  updatedAt: string // 更新时间（ISO 8601）
}

/** 系统参数查询参数 */
export interface SystemConfigQuery extends PageQuery {
  key?: string
  label?: string
}

/** 系统参数创建参数 */
export interface SystemConfigCreateParams {
  key: string
  label: string
  value: string
  type: SystemConfigType
  remark: string
}
