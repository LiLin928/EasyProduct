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
