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
