# F2-1 Basic 模块设计方案

> **日期：** 2026-08-12
> **状态：** 待评审
> **阶段：** F2-1（Admin Basic 模块）
> **上游依据：** `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`、`docs/frontend-guidelines.md`、`docs/superpowers/plans/2026-08-08-frontend-first-development.md`

---

## 1. 背景与目标

### 1.1 背景

F2-0 已完成 Admin 封装层（useTable/useForm/useDialog/useDict/useLocale/usePermission + BaseTable + mock-server admin 分区骨架路由）。F2-1 需要在封装层基础上开发 Basic 模块，建立完整的 RBAC 权限体系。

### 1.2 目标

- 完成 Basic 模块 9 个页面：用户/部门/角色/菜单/字典/公告/工作台布局/系统参数/个人中心
- 实现完整的 RBAC 权限体系
- 完成动态路由切换（从静态 placeholder 切换为菜单驱动）
- 提供权限标识种子数据供后续模块参考

### 1.3 范围

**包含**：
- 9 个页面的完整实现（列表/新增/编辑/删除）
- 完整的 API 层和 Mock 数据
- 动态路由切换机制
- 权限控制（按钮级）

**不包含**：
- 后端实现（属于 B 系列 P1 阶段）
- 数据迁移（全新数据库）
- 审批流（属于 Workflow 模块）

---

## 2. 总体设计

### 2.1 批次交付策略

采用 4 批次交付，逐批验收：

| 批次 | 页面 | 核心功能 | 验收标准 |
|------|------|---------|---------|
| **批次 1（核心 RBAC）** | 用户/角色/菜单 | RBAC 核心 + 动态路由切换 | 权限体系可用、动态路由生效 |
| **批次 2（基础数据）** | 部门/字典 | 树形结构 + 字典管理 | 用户可选择部门、字典可渲染 |
| **批次 3（系统功能）** | 公告/系统参数 | 富文本编辑 + 配置管理 | 公告可发布、参数可修改 |
| **批次 4（个人中心）** | 个人中心/工作台布局 | 个人信息管理 + 布局保存 | 可改密、可保存布局 |

### 2.2 开发顺序

**批次 1**：用户管理 → 角色管理 → 菜单管理 → 动态路由切换
**批次 2**：部门管理 → 字典管理
**批次 3**：公告管理 → 系统参数
**批次 4**：个人中心 → 工作台布局

### 2.3 权限标识设计

采用三层结构：`模块:页面:操作`

**操作类型**：
- `list`：查看权限
- `edit`：编辑权限（包含新增和修改）
- `delete`：删除权限

**批次 1 权限标识**：
- 用户管理：`basic:user:list` / `basic:user:edit` / `basic:user:delete`
- 角色管理：`basic:role:list` / `basic:role:edit` / `basic:role:delete`
- 菜单管理：`basic:menu:list` / `basic:menu:edit` / `basic:menu:delete`

**批次 2-4 权限标识**：
- 部门管理：`basic:dept:list` / `basic:dept:edit` / `basic:dept:delete`
- 字典管理：`basic:dict:list` / `basic:dict:edit` / `basic:dict:delete`
- 公告管理：`basic:announcement:list` / `basic:announcement:edit` / `basic:announcement:delete`
- 系统参数：`basic:setting:list` / `basic:setting:edit`
- 个人中心：`basic:profile:edit`

---

## 3. 技术架构

### 3.1 文件结构

```
EasyProduct.Admin/src/
├── views/basic/
│   ├── user/           # 批次 1
│   │   ├── index.vue
│   │   └── components/
│   │       ├── UserForm.vue
│   │       └── RoleSelect.vue
│   ├── role/           # 批次 1
│   │   ├── index.vue
│   │   └── components/
│   │       └── MenuAssign.vue
│   ├── menu/           # 批次 1
│   │   ├── index.vue
│   │   └── components/
│   │       └── MenuForm.vue
│   ├── dept/           # 批次 2
│   ├── dict/           # 批次 2
│   ├── announcement/    # 批次 3
│   ├── setting/        # 批次 3
│   └── profile/        # 批次 4
├── api/basic/
│   ├── user.ts
│   ├── role.ts
│   ├── menu.ts
│   ├── dept.ts
│   ├── dict.ts
│   ├── announcement.ts
│   └── setting.ts
├── types/basic.ts      # 扩展现有文件
└── router/modules/
    └── basic.ts        # 替换 placeholder

mock-server/src/
├── data/admin/
│   ├── user.ts
│   ├── role.ts
│   ├── menu.ts
│   ├── dept.ts
│   └── dict.ts
└── routes/admin/
    ├── user.ts
    ├── role.ts
    ├── menu.ts
    ├── dept.ts
    └── dict.ts
```

### 3.2 技术选型

| 功能 | 技术方案 | 说明 |
|------|---------|------|
| 列表页 | useTable + BaseTable | F2-0 已完成 |
| 表单弹窗 | useForm + useDialog | F2-0 已完成 |
| 权限控制 | usePermission + v-permission | F2-0 已完成 |
| 字典渲染 | useDict | F2-0 已完成 |
| 菜单树 | Element Plus el-tree | 支持拖拽排序 |
| 部门树 | Element Plus el-tree-select | 支持搜索选择 |
| 富文本编辑 | WangEditor | 批次 3 引入（首次使用需说明依赖理由） |

---

## 4. 数据模型

### 4.1 用户模型

```typescript
// types/basic.ts

/** 用户 */
export interface User {
  id: string                      // GUID
  userName: string                // 登录账号
  password?: string               // 密码（仅新增/编辑时传入）
  realName: string                // 真实姓名
  email: string                   // 邮箱
  phone: string                   // 手机号
  status: 'enabled' | 'disabled'  // 状态
  deptId: string                  // 部门ID（GUID）
  deptName?: string               // 部门名称（列表查询返回）
  roleIds: string[]               // 角色ID列表（GUID数组）
  roleNames?: string[]            // 角色名称列表（列表查询返回）
  createdAt: string               // 创建时间（ISO 8601）
  updatedAt: string               // 更新时间（ISO 8601）
}

/** 用户查询参数 */
export interface UserQuery extends PageQuery {
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
```

### 4.2 角色模型

```typescript
/** 角色 */
export interface Role {
  id: string                      // GUID
  name: string                    // 角色名称
  code: string                    // 角色编码
  status: 'enabled' | 'disabled'  // 状态
  sort: number                    // 排序
  remark: string                  // 备注
  menuIds: string[]               // 分配的菜单ID列表（GUID数组）
  createdAt: string               // 创建时间（ISO 8601）
  updatedAt: string               // 更新时间（ISO 8601）
}

/** 角色查询参数 */
export interface RoleQuery extends PageQuery {
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
```

### 4.3 菜单模型

```typescript
/** 菜单 */
export interface Menu {
  id: string                      // GUID
  parentId: string                // 父菜单ID（GUID，根节点为 '0'）
  name: string                    // 路由 name
  path: string                    // 路由 path
  titleKey: string                // i18n key（如 'menu.basic.user'）
  icon: string                    // 图标名称
  sort: number                    // 排序
  permission?: string             // 权限标识（按钮级，如 'basic:user:edit'）
  component?: string              // 组件路径（如 'basic/user/index'）
  visible: boolean                // 是否显示在菜单
  status: 'enabled' | 'disabled'  // 状态
  children?: Menu[]               // 子菜单
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
```

### 4.4 部门模型

```typescript
/** 部门 */
export interface Dept {
  id: string                      // GUID
  parentId: string                // 父部门ID（GUID，根节点为 '0'）
  name: string                    // 部门名称
  code: string                    // 部门编码
  sort: number                    // 排序
  status: 'enabled' | 'disabled'  // 状态
  createdAt: string               // 创建时间（ISO 8601）
  updatedAt: string               // 更新时间（ISO 8601）
  children?: Dept[]               // 子部门
}
```

### 4.5 字典模型

```typescript
/** 字典类型 */
export interface DictType {
  id: string                      // GUID
  name: string                    // 字典类型名称
  code: string                    // 字典类型编码
  status: 'enabled' | 'disabled'  // 状态
  remark: string                  // 备注
}

/** 字典数据 */
export interface DictData {
  id: string                      // GUID
  typeCode: string                // 字典类型编码
  value: string                   // 字典值
  labelKey: string                // i18n key（如 'common.status.enabled'）
  sort: number                    // 排序
  status: 'enabled' | 'disabled'  // 状态
}
```

---

## 5. API 契约

### 5.1 用户管理 API

```typescript
// api/basic/user.ts

/** 用户列表（分页） */
export const getUserList = (params: UserQuery) =>
  get<PageResult<User>>('/api/admin/basic/user/list', params)

/** 用户详情 */
export const getUserDetail = (id: string) =>
  get<User>(`/api/admin/basic/user/${id}`)

/** 新增用户 */
export const createUser = (data: UserCreateParams) =>
  post<{ id: string }>('/api/admin/basic/user', data)

/** 编辑用户 */
export const updateUser = (id: string, data: UserUpdateParams) =>
  put<{ id: string }>(`/api/admin/basic/user/${id}`, data)

/** 删除用户 */
export const deleteUser = (id: string) =>
  del<null>(`/api/admin/basic/user/${id}`)

/** 重置密码 */
export const resetPassword = (id: string) =>
  post<null>(`/api/admin/basic/user/${id}/reset-password`)
```

### 5.2 角色管理 API

```typescript
// api/basic/role.ts

/** 角色列表（分页） */
export const getRoleList = (params: RoleQuery) =>
  get<PageResult<Role>>('/api/admin/basic/role/list', params)

/** 角色详情 */
export const getRoleDetail = (id: string) =>
  get<Role>(`/api/admin/basic/role/${id}`)

/** 新增角色 */
export const createRole = (data: RoleCreateParams) =>
  post<{ id: string }>('/api/admin/basic/role', data)

/** 编辑角色 */
export const updateRole = (id: string, data: RoleCreateParams) =>
  put<{ id: string }>(`/api/admin/basic/role/${id}`, data)

/** 删除角色 */
export const deleteRole = (id: string) =>
  del<null>(`/api/admin/basic/role/${id}`)

/** 分配菜单 */
export const assignMenus = (id: string, menuIds: string[]) =>
  post<null>(`/api/admin/basic/role/${id}/menus`, { menuIds })

/** 获取角色的菜单ID列表 */
export const getRoleMenus = (id: string) =>
  get<string[]>(`/api/admin/basic/role/${id}/menus`)
```

### 5.3 菜单管理 API

```typescript
// api/basic/menu.ts

/** 菜单树 */
export const getMenuTree = () =>
  get<Menu[]>('/api/admin/basic/menu/tree')

/** 菜单详情 */
export const getMenuDetail = (id: string) =>
  get<Menu>(`/api/admin/basic/menu/${id}`)

/** 新增菜单 */
export const createMenu = (data: MenuCreateParams) =>
  post<{ id: string }>('/api/admin/basic/menu', data)

/** 编辑菜单 */
export const updateMenu = (id: string, data: MenuCreateParams) =>
  put<{ id: string }>(`/api/admin/basic/menu/${id}`, data)

/** 删除菜单 */
export const deleteMenu = (id: string) =>
  del<null>(`/api/admin/basic/menu/${id}`)

/** 更新菜单排序 */
export const updateMenuSort = (dragId: string, dropId: string, type: 'prev' | 'next') =>
  post<null>('/api/admin/basic/menu/sort', { dragId, dropId, type })
```

### 5.4 部门管理 API

```typescript
// api/basic/dept.ts

/** 部门树 */
export const getDeptTree = () =>
  get<Dept[]>('/api/admin/basic/dept/tree')

/** 部门详情 */
export const getDeptDetail = (id: string) =>
  get<Dept>(`/api/admin/basic/dept/${id}`)

/** 新增部门 */
export const createDept = (data: DeptCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dept', data)

/** 编辑部门 */
export const updateDept = (id: string, data: DeptCreateParams) =>
  put<{ id: string }>(`/api/admin/basic/dept/${id}`, data)

/** 删除部门 */
export const deleteDept = (id: string) =>
  del<null>(`/api/admin/basic/dept/${id}`)
```

### 5.5 字典管理 API

```typescript
// api/basic/dict.ts

/** 字典类型列表 */
export const getDictTypeList = (params: DictTypeQuery) =>
  get<PageResult<DictType>>('/api/admin/basic/dict-type/list', params)

/** 字典数据列表（按类型） */
export const getDictDataList = (typeCode: string) =>
  get<DictData[]>('/api/admin/basic/dict-data', { typeCode })

/** 新增字典数据 */
export const createDictData = (data: DictDataCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dict-data', data)

/** 编辑字典数据 */
export const updateDictData = (id: string, data: DictDataCreateParams) =>
  put<{ id: string }>(`/api/admin/basic/dict-data/${id}`, data)

/** 删除字典数据 */
export const deleteDictData = (id: string) =>
  del<null>(`/api/admin/basic/dict-data/${id}`)
```

---

## 6. Mock 数据设计

### 6.1 菜单树种子数据

```typescript
// mock-server/src/data/admin/menu.ts

import { guid, isoTime } from '../../helpers/id.js'

export const MENU_TREE: Menu[] = [
  {
    id: guid(), // e.g., 'a1b2c3d4-e5f6-7890-abcd-ef1234567890'
    parentId: '0',
    name: 'desktop',
    path: '/desktop',
    titleKey: 'menu.desktop',
    icon: 'Monitor',
    sort: 1,
    visible: true,
    status: 'enabled',
    children: []
  },
  {
    id: guid(),
    parentId: '0',
    name: 'basic',
    path: '/basic',
    titleKey: 'menu.basic',
    icon: 'Setting',
    sort: 2,
    visible: true,
    status: 'enabled',
    children: [
      {
        id: guid(),
        parentId: '2', // 指向 basic 的 ID
        name: 'basic-user',
        path: '/basic/user',
        titleKey: 'menu.basic.user',
        icon: 'User',
        sort: 1,
        permission: 'basic:user:list',
        component: 'basic/user/index',
        visible: true,
        status: 'enabled'
      },
      {
        id: guid(),
        parentId: '2',
        name: 'basic-role',
        path: '/basic/role',
        titleKey: 'menu.basic.role',
        icon: 'UserFilled',
        sort: 2,
        permission: 'basic:role:list',
        component: 'basic/role/index',
        visible: true,
        status: 'enabled'
      },
      {
        id: guid(),
        parentId: '2',
        name: 'basic-menu',
        path: '/basic/menu',
        titleKey: 'menu.basic.menu',
        icon: 'Menu',
        sort: 3,
        permission: 'basic:menu:list',
        component: 'basic/menu/index',
        visible: true,
        status: 'enabled'
      }
      // 批次 2~4 的菜单后续追加
    ]
  },
  // 其他模块根节点（product/site/mall/crm/workflow/report/ops）
  // 暂时只显示，不可点击（无 children）
]
```

### 6.2 用户种子数据

```typescript
// mock-server/src/data/admin/user.ts

import Mock from 'mockjs'
import { guid, isoTime } from '../../helpers/id.js'

export interface AdminUser {
  id: string
  userName: string
  password: string
  realName: string
  email: string
  phone: string
  status: 'enabled' | 'disabled'
  deptId: string
  roleIds: string[]
  createdAt: string
  updatedAt: string
}

export const ADMIN_USERS: AdminUser[] = Mock.mock({
  'list|10': [{
    id: '@guid',
    userName: '@word(5,10)',
    password: '123456',
    realName: '@cname',
    email: '@email',
    phone: /^1[3-9]\d{9}$/,
    status: '@pick(["enabled", "disabled"])',
    deptId: '@guid',
    roleIds: () => [guid()],
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }],
}).list

// 添加默认管理员（覆盖第一条）
ADMIN_USERS[0] = {
  id: guid(),
  userName: 'admin',
  password: 'admin123',
  realName: '系统管理员',
  email: 'admin@example.com',
  phone: '13800138000',
  status: 'enabled',
  deptId: guid(),
  roleIds: [guid()],
  createdAt: isoTime(),
  updatedAt: isoTime(),
}

/** 权限标识种子 */
export const USER_PERMISSIONS: Record<string, string[]> = {
  admin: ['*'],  // 超级管理员
  sales: ['basic:user:list', 'basic:role:list'],
  ops: ['basic:menu:list', 'basic:menu:edit']
}
```

### 6.3 角色种子数据

```typescript
// mock-server/src/data/admin/role.ts

import Mock from 'mockjs'
import { guid, isoTime } from '../../helpers/id.js'

export interface Role {
  id: string
  name: string
  code: string
  status: 'enabled' | 'disabled'
  sort: number
  remark: string
  menuIds: string[]
  createdAt: string
  updatedAt: string
}

export const ROLES: Role[] = Mock.mock({
  'list|5': [{
    id: '@guid',
    name: '@ctitle(4,8)',
    code: '@word(4,8)',
    status: '@pick(["enabled", "disabled"])',
    'sort|1-10': 1,
    remark: '@csentence(10,20)',
    menuIds: () => [guid()],
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }],
}).list

// 添加超级管理员角色
ROLES.unshift({
  id: guid(),
  name: '超级管理员',
  code: 'super_admin',
  status: 'enabled',
  sort: 1,
  remark: '拥有所有权限',
  menuIds: [], // 空数组表示拥有所有菜单权限
  createdAt: isoTime(),
  updatedAt: isoTime(),
})
```

---

## 7. 动态路由切换

### 7.1 切换时机

批次 1 完成后（用户/角色/菜单三个页面），立即切换动态路由。

### 7.2 实现方案

**步骤 1：定义模块路由**

```typescript
// router/modules/basic.ts

import type { RouteRecordRaw } from 'vue-router'

export const basicRoutes: RouteRecordRaw[] = [
  {
    path: '/basic',
    name: 'basic',
    redirect: '/basic/user',
    meta: { title: 'menu.basic', icon: 'Setting' },
    children: [
      {
        path: 'user',
        name: 'basic-user',
        component: () => import('@/views/basic/user/index.vue'),
        meta: { title: 'menu.basic.user', icon: 'User' }
      },
      {
        path: 'role',
        name: 'basic-role',
        component: () => import('@/views/basic/role/index.vue'),
        meta: { title: 'menu.basic.role', icon: 'UserFilled' }
      },
      {
        path: 'menu',
        name: 'basic-menu',
        component: () => import('@/views/basic/menu/index.vue'),
        meta: { title: 'menu.basic.menu', icon: 'Menu' }
      },
      // 批次 2~4 的路由后续追加
    ]
  }
]
```

**步骤 2：修改 stores/user.ts**

```typescript
// stores/user.ts

import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { login as loginApi, getMenuList } from '@/api/basic/auth'
import { clearTokens, getAccessToken, setTokens } from '@/utils/auth'
import { basicRoutes } from '@/router/modules/basic'

const MODULE_ROUTES = {
  basic: basicRoutes,
  // product: productRoutes, // F2-3 时添加
  // site: siteRoutes,        // F2-2 时添加
  // mall: mallRoutes,        // F2-4 时添加
  // crm: crmRoutes,          // F2-5 时添加
  // workflow: workflowRoutes, // F2-6 时添加
  // report: reportRoutes,    // F2-7 时添加
  // ops: opsRoutes,          // F2-8 时添加
}

export const useUserStore = defineStore('user', () => {
  const router = useRouter()
  const token = ref(getAccessToken())
  const realName = ref('')
  const permissions = ref<string[]>([])

  async function login(userName: string, password: string): Promise<void> {
    const data = await loginApi({ userName, password })
    setTokens(data.accessToken, data.refreshToken)
    token.value = data.accessToken
    realName.value = data.user.realName
    permissions.value = data.permissions

    // 加载菜单并动态注册路由
    await loadMenuRoutes()
  }

  /** 加载菜单并动态注册路由 */
  async function loadMenuRoutes(): Promise<void> {
    const menus = await getMenuList()

    // 根据菜单树动态注册路由
    menus.forEach(menu => {
      const routes = MODULE_ROUTES[menu.name as keyof typeof MODULE_ROUTES]
      if (routes) {
        routes.forEach(route => router.addRoute(route))
      }
    })

    // 移除 placeholder 路由
    router.removeRoute('placeholder-basic')
  }

  function logout(): void {
    clearTokens()
    token.value = ''
    realName.value = ''
    permissions.value = []
  }

  /** 按钮级权限判断 */
  function has(code: string): boolean {
    return permissions.value.includes('*') || permissions.value.includes(code)
  }

  return { token, realName, permissions, login, logout, has }
})
```

**步骤 3：更新 router/index.ts**

```typescript
// router/index.ts

import type { App } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { setupGuards } from './guards'

export const routes: RouteRecordRaw[] = [
  { path: '/login', name: 'login', component: () => import('@/views/login/index.vue'), meta: { title: 'common.login.title' } },
  {
    path: '/',
    component: () => import('@/layouts/MainLayout.vue'),
    redirect: '/desktop',
    children: [
      { path: 'desktop', name: 'desktop', component: () => import('@/views/desktop/index.vue'), meta: { title: 'menu.desktop', icon: 'Monitor' } },
      // 动态路由由 useUserStore.loadMenuRoutes() 注册
    ],
  },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/error/404.vue'), meta: { title: 'menu.notFound' } },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

export function setupRouter(app: App): void {
  setupGuards(router)
  app.use(router)
}
```

---

## 8. 页面实现要点

### 8.1 用户管理

**核心功能**：
- 列表展示（分页、搜索、状态筛选）
- 新增/编辑用户（表单弹窗）
- 删除用户（确认对话框）
- 分配角色（多选组件）
- 重置密码（单独操作）

**实现要点**：
- 使用 `useTable` 管理列表状态
- 使用 `useDialog` 管理弹窗状态
- 使用 `useForm` 管理表单验证
- 使用 `v-permission` 控制按钮显示

### 8.2 角色管理

**核心功能**：
- 列表展示（分页、搜索）
- 新增/编辑角色（表单弹窗）
- 删除角色（确认对话框）
- 分配菜单（树形选择）

**实现要点**：
- 菜单树使用 `el-tree` 组件，支持勾选
- 保存时提交选中的菜单 ID 数组

### 8.3 菜单管理

**核心功能**：
- 树形展示（展开所有节点）
- 新增菜单（指定父菜单）
- 编辑菜单（修改菜单属性）
- 删除菜单（检查是否有子菜单）
- 拖拽排序（同级节点排序）

**实现要点**：
- 使用 `el-tree` 的 `draggable` 属性
- 拖拽后调用排序接口更新后端数据
- 删除前检查是否有子节点

---

## 9. 验收标准

### 9.1 批次 1 验收

**功能验收**：
- [ ] 用户列表可正常展示、搜索、分页
- [ ] 可新增/编辑/删除用户
- [ ] 可为用户分配角色
- [ ] 角色列表可正常展示、搜索、分页
- [ ] 可新增/编辑/删除角色
- [ ] 可为角色分配菜单
- [ ] 菜单树可正常展示
- [ ] 可新增/编辑/删除菜单
- [ ] 菜单可拖拽排序
- [ ] 动态路由切换生效（登录后侧栏菜单由后端数据驱动）

**权限验收**：
- [ ] 超级管理员（admin）可以看到所有菜单和按钮
- [ ] 普通用户只能看到有权限的菜单和按钮
- [ ] 无权限时按钮不显示（`v-permission` 生效）

**代码验收**：
- [ ] `pnpm type-check` 零错误
- [ ] `pnpm lint` 通过
- [ ] `pnpm check:i18n` 无硬编码中文

### 9.2 批次 2-4 验收

（在批次 1 完成后细化）

---

## 10. 风险与依赖

### 10.1 已知风险

| 风险 | 影响 | 缓解措施 |
|------|------|---------|
| 动态路由切换时机不当 | 用户可能看到空白页面 | 批次 1 完成后立即切换，充分测试 |
| 权限数据不一致 | 用户可能绕过权限检查 | 前后端使用同一份权限种子数据 |
| 菜单拖拽排序复杂 | 实现难度较高 | 简化为只支持同级排序，不支持跨级 |

### 10.2 依赖项

| 依赖 | 状态 | 说明 |
|------|------|------|
| F2-0 封装层 | 已完成 | useTable/useForm/useDialog/useDict/usePermission/BaseTable |
| mock-server 骨架 | 已完成 | admin 分区基础路由 |
| Element Plus | 已安装 | 树形组件、表单组件 |

---

## 11. 后续计划

### 11.1 批次 2-4 开发

完成批次 1 后，依次开发：
- 批次 2：部门管理、字典管理
- 批次 3：公告管理（引入 WangEditor）、系统参数
- 批次 4：个人中心、工作台布局

### 11.2 其他模块

F2-1 完成后，按照前端优先开发计划继续：
- F2-2：Site 管理模块
- F2-3：Product 商品中心
- F2-4：Mall 商城业务
- F2-5：Crm 模块
- F2-6：Workflow 工作流
- F2-7：Report 报表
- F2-8：Ops 运维
- F2-9：工作台整合

---

## 附录 A：i18n 键值补充

```json
// i18n/zh-CN/menu.json 追加
{
  "basic": "基础管理",
  "basic.user": "用户管理",
  "basic.role": "角色管理",
  "basic.menu": "菜单管理",
  "basic.dept": "部门管理",
  "basic.dict": "字典管理",
  "basic.announcement": "公告管理",
  "basic.setting": "系统参数",
  "basic.profile": "个人中心"
}

// i18n/en-US/menu.json 追加
{
  "basic": "Basic",
  "basic.user": "User",
  "basic.role": "Role",
  "basic.menu": "Menu",
  "basic.dept": "Department",
  "basic.dict": "Dictionary",
  "basic.announcement": "Announcement",
  "basic.setting": "Settings",
  "basic.profile": "Profile"
}
```

---

**设计版本：** v1.0
**最后更新：** 2026-08-12