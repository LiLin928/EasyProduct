# F2-0 Admin 封装层实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 建立 Admin 管理后台通用封装层（composables + 组件 + mock 补齐），为后续八大模块开发提供统一基础设施。

**Architecture:** 基于 Vue 3 Composition API 创建可复用的 composables（useTable/useForm/useDialog/useDict/useLocale/usePermission），实现通用组件（BaseTable），补充 mock-server admin 分区骨架路由。所有 composables 签名逐字对齐前端规范 2.4。

**Tech Stack:** Vue 3.4+ / TypeScript 5.3+ / Element Plus 2.6+ / Pinia 2.1+ / vue-i18n 9.9+ / mockjs

**上游依据：**
- `docs/frontend-guidelines.md` 第 2.4 节（composables 签名）
- `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md` 第 9 节（菜单树）
- `docs/superpowers/plans/2026-08-08-frontend-first-development.md` F2-0 任务表

---

## Global Constraints

1. 所有 composables 函数签名必须逐字对齐 `docs/frontend-guidelines.md` 第 2.4 节
2. 禁用 TS enum，使用字符串联合类型 + `as const`
3. `no-explicit-any: error`、`no-console: error`
4. 样式仅 SCSS、组件必须 scoped、禁止硬编码色值（走 CSS Variables）
5. 所有可见文案走 i18n key，禁止硬编码中文
6. mock 路由与后端规范第 5 节逐字一致
7. 参考源项目重写，禁止直接复制源项目代码

---

## 文件结构

```text
EasyProduct.Admin/src/
├── composables/
│   ├── useTable.ts
│   ├── useForm.ts
│   ├── useDialog.ts
│   ├── useDict.ts
│   ├── useLocale.ts
│   └── usePermission.ts
├── components/
│   └── common/
│       └── BaseTable.vue
└── directives/
    └── permission.ts

mock-server/src/
├── data/admin/
│   ├── user.ts
│   ├── dept.ts
│   ├── role.ts
│   ├── menu.ts
│   └── dict.ts
└── routes/admin/
    ├── user.ts
    ├── dept.ts
    ├── role.ts
    └── dict.ts
```

---

### Task 1: useTable composable（列表页核心）

**Files:**
- Create: `EasyProduct.Admin/src/composables/useTable.ts`

**Interfaces:**
- Consumes: `ref` / `reactive` / `computed` from Vue 3
- Produces: `useTable<T>(fetchFn, options?)` 返回 `{ loading, list, total, query, handleSearch, handleReset, handlePageChange, reload }`

- [ ] **Step 1.1: 创建 useTable.ts 文件**

```typescript
// src/composables/useTable.ts
import { ref, reactive } from 'vue'
import type { Ref } from 'vue'
import type { PageQuery, PageResult } from '@/types/api'

export interface UseTableOptions {
  defaultPageSize?: number
  immediate?: boolean
}

export interface UseTableReturn<T> {
  loading: Ref<boolean>
  list: Ref<T[]>
  total: Ref<number>
  query: {
    pageIndex: number
    pageSize: number
    [key: string]: unknown
  }
  handleSearch: () => Promise<void>
  handleReset: () => void
  handlePageChange: (page: number) => Promise<void>
  reload: () => Promise<void>
}

/**
 * 列表页通用 composable（签名对齐前端规范 2.4）
 * @param fetchFn 分页查询函数，接收 PageQuery 返回 Promise<PageResult<T>>
 * @param options 配置项
 */
export function useTable<T>(
  fetchFn: (params: PageQuery & Record<string, unknown>) => Promise<PageResult<T>>,
  options: UseTableOptions = {},
): UseTableReturn<T> {
  const { defaultPageSize = 10, immediate = true } = options

  const loading = ref(false)
  const list = ref<T[]>([]) as Ref<T[]>
  const total = ref(0)
  const query = reactive<PageQuery & Record<string, unknown>>({
    pageIndex: 1,
    pageSize: defaultPageSize,
  })

  const fetchData = async (): Promise<void> => {
    loading.value = true
    try {
      const result = await fetchFn(query)
      list.value = result.list
      total.value = result.total
    } finally {
      loading.value = false
    }
  }

  const handleSearch = async (): Promise<void> => {
    query.pageIndex = 1
    await fetchData()
  }

  const handleReset = (): void => {
    query.pageIndex = 1
    query.pageSize = defaultPageSize
    Object.keys(query).forEach((key) => {
      if (key !== 'pageIndex' && key !== 'pageSize') {
        delete query[key]
      }
    })
  }

  const handlePageChange = async (page: number): Promise<void> => {
    query.pageIndex = page
    await fetchData()
  }

  const reload = async (): Promise<void> => {
    await fetchData()
  }

  if (immediate) {
    fetchData()
  }

  return {
    loading,
    list,
    total,
    query,
    handleSearch,
    handleReset,
    handlePageChange,
    reload,
  }
}
```

- [ ] **Step 1.2: 验证 useTable 类型检查**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

Expected: 零错误

- [ ] **Step 1.3: Commit**

```powershell
git add EasyProduct.Admin/src/composables/useTable.ts
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): useTable composable（列表页通用逻辑，签名对齐前端规范 2.4）"
```

---

### Task 2: useForm composable（表单核心）

**Files:**
- Create: `EasyProduct.Admin/src/composables/useForm.ts`

**Interfaces:**
- Consumes: `ref` / `reactive` from Vue 3
- Produces: `useForm<T>(options)` 返回 `{ formRef, model, rules, validate, resetFields, submitLoading, handleSubmit }`

- [ ] **Step 2.1: 创建 useForm.ts 文件**

```typescript
// src/composables/useForm.ts
import { ref, reactive } from 'vue'
import type { Ref, UnwrapNestedRefs } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'

export interface UseFormOptions<T> {
  defaultModel?: Partial<T>
  rules?: FormRules
  onSubmit?: (model: T) => Promise<void>
}

export interface UseFormReturn<T> {
  formRef: Ref<FormInstance | undefined>
  model: UnwrapNestedRefs<T>
  rules: FormRules
  validate: () => Promise<boolean>
  resetFields: () => void
  submitLoading: Ref<boolean>
  handleSubmit: () => Promise<void>
}

/**
 * 表单通用 composable（签名对齐前端规范 2.4）
 * @param options 配置项
 */
export function useForm<T extends Record<string, unknown>>(
  options: UseFormOptions<T> = {},
): UseFormReturn<T> {
  const { defaultModel = {}, rules = {}, onSubmit } = options

  const formRef = ref<FormInstance>()
  const model = reactive<T>((defaultModel || {}) as T) as UnwrapNestedRefs<T>
  const submitLoading = ref(false)

  const validate = async (): Promise<boolean> => {
    if (!formRef.value) return false
    try {
      await formRef.value.validate()
      return true
    } catch {
      return false
    }
  }

  const resetFields = (): void => {
    formRef.value?.resetFields()
  }

  const handleSubmit = async (): Promise<void> => {
    const valid = await validate()
    if (!valid) return

    if (!onSubmit) return

    submitLoading.value = true
    try {
      await onSubmit(model as T)
    } finally {
      submitLoading.value = false
    }
  }

  return {
    formRef,
    model,
    rules,
    validate,
    resetFields,
    submitLoading,
    handleSubmit,
  }
}
```

- [ ] **Step 2.2: 验证 useForm 类型检查**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

Expected: 零错误

- [ ] **Step 2.3: Commit**

```powershell
git add EasyProduct.Admin/src/composables/useForm.ts
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): useForm composable（表单通用逻辑，签名对齐前端规范 2.4）"
```

---

### Task 3: useDialog composable（弹窗管理）

**Files:**
- Create: `EasyProduct.Admin/src/composables/useDialog.ts`

**Interfaces:**
- Consumes: `ref` from Vue 3
- Produces: `useDialog<T>()` 返回 `{ visible, payload, isEdit, open, close }`

- [ ] **Step 3.1: 创建 useDialog.ts 文件**

```typescript
// src/composables/useDialog.ts
import { ref } from 'vue'
import type { Ref } from 'vue'

export interface UseDialogReturn<T> {
  visible: Ref<boolean>
  payload: Ref<T | undefined>
  isEdit: Ref<boolean>
  open: (data?: T) => void
  close: () => void
}

/**
 * 弹窗通用 composable（签名对齐前端规范 2.4）
 */
export function useDialog<T = unknown>(): UseDialogReturn<T> {
  const visible = ref(false)
  const payload = ref<T>() as Ref<T | undefined>
  const isEdit = ref(false)

  const open = (data?: T): void => {
    if (data) {
      payload.value = data
      isEdit.value = true
    } else {
      payload.value = undefined
      isEdit.value = false
    }
    visible.value = true
  }

  const close = (): void => {
    visible.value = false
    payload.value = undefined
    isEdit.value = false
  }

  return {
    visible,
    payload,
    isEdit,
    open,
    close,
  }
}
```

- [ ] **Step 3.2: 验证 useDialog 类型检查**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

Expected: 零错误

- [ ] **Step 3.3: Commit**

```powershell
git add EasyProduct.Admin/src/composables/useDialog.ts
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): useDialog composable（弹窗通用逻辑，签名对齐前端规范 2.4）"
```

---

### Task 4: useDict composable（字典数据）

**Files:**
- Create: `EasyProduct.Admin/src/composables/useDict.ts`

**Interfaces:**
- Consumes: `getDictData` from `@/api/basic/dict`、`useI18n` from `vue-i18n`
- Produces: `useDict(typeCode)` 返回 `{ options, getLabel }`

- [ ] **Step 4.1: 创建 useDict.ts 文件**

```typescript
// src/composables/useDict.ts
import { ref, computed } from 'vue'
import type { Ref, ComputedRef } from 'vue'
import { useI18n } from 'vue-i18n'
import { getDictData } from '@/api/basic/dict'
import type { DictItem } from '@/types/basic'

export interface DictOption {
  value: string
  label: string
}

export interface UseDictReturn {
  options: Ref<DictOption[]>
  getLabel: (value: string) => string
  loading: Ref<boolean>
}

/**
 * 字典数据 composable（签名对齐前端规范 2.4）
 * @param typeCode 字典类型编码
 */
export function useDict(typeCode: string): UseDictReturn {
  const { t } = useI18n()
  const options = ref<DictOption[]>([])
  const loading = ref(false)
  const dictMap = ref<Map<string, DictItem>>(new Map())

  const fetchDict = async (): Promise<void> => {
    loading.value = true
    try {
      const items = await getDictData(typeCode)
      dictMap.value = new Map(items.map((item) => [item.value, item]))
      options.value = items.map((item) => ({
        value: item.value,
        label: t(item.labelKey),
      }))
    } finally {
      loading.value = false
    }
  }

  const getLabel = (value: string): string => {
    const item = dictMap.value.get(value)
    return item ? t(item.labelKey) : value
  }

  // 立即加载字典数据
  fetchDict()

  return {
    options,
    getLabel,
    loading,
  }
}
```

- [ ] **Step 4.2: 验证 useDict 类型检查**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

Expected: 零错误

- [ ] **Step 4.3: Commit**

```powershell
git add EasyProduct.Admin/src/composables/useDict.ts
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): useDict composable（字典数据加载与渲染，签名对齐前端规范 2.4）"
```

---

### Task 5: useLocale composable（语言切换）

**Files:**
- Create: `EasyProduct.Admin/src/composables/useLocale.ts`

**Interfaces:**
- Consumes: `useI18n` from `vue-i18n`、`useAppStore` from `@/stores/app`
- Produces: `useLocale()` 返回 `{ locale, locales, setLocale, t }`

- [ ] **Step 5.1: 创建 useLocale.ts 文件**

```typescript
// src/composables/useLocale.ts
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/app'
import { SUPPORT_LOCALES } from '@/i18n'
import type { Locale } from '@/i18n'

export interface LocaleOption {
  value: Locale
  label: string
}

export interface UseLocaleReturn {
  locale: Locale
  locales: LocaleOption[]
  setLocale: (locale: Locale) => void
  t: (key: string, ...args: unknown[]) => string
}

/**
 * 语言切换 composable（签名对齐前端规范 2.4）
 */
export function useLocale(): UseLocaleReturn {
  const { t, locale } = useI18n()
  const appStore = useAppStore()

  const locales: LocaleOption[] = [
    { value: 'zh-CN', label: '中文' },
    { value: 'en-US', label: 'English' },
  ]

  const setLocale = (next: Locale): void => {
    appStore.switchLocale(next)
  }

  return {
    locale: locale.value as Locale,
    locales,
    setLocale,
    t: t as (key: string, ...args: unknown[]) => string,
  }
}
```

- [ ] **Step 5.2: 验证 useLocale 类型检查**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

Expected: 零错误

- [ ] **Step 5.3: Commit**

```powershell
git add EasyProduct.Admin/src/composables/useLocale.ts
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): useLocale composable（语言切换，签名对齐前端规范 2.4）"
```

---

### Task 6: usePermission composable + v-permission 指令（权限控制）

**Files:**
- Create: `EasyProduct.Admin/src/composables/usePermission.ts`
- Create: `EasyProduct.Admin/src/directives/permission.ts`

**Interfaces:**
- Consumes: `useUserStore` from `@/stores/user`
- Produces: `usePermission()` 返回 `{ has }`；`v-permission` 指令

- [ ] **Step 6.1: 创建 usePermission.ts 文件**

```typescript
// src/composables/usePermission.ts
import { useUserStore } from '@/stores/user'

export interface UsePermissionReturn {
  has: (code: string) => boolean
}

/**
 * 权限判断 composable（签名对齐前端规范 2.4）
 */
export function usePermission(): UsePermissionReturn {
  const userStore = useUserStore()

  const has = (code: string): boolean => {
    return userStore.has(code)
  }

  return {
    has,
  }
}
```

- [ ] **Step 6.2: 创建 v-permission 指令**

```typescript
// src/directives/permission.ts
import type { Directive, DirectiveBinding } from 'vue'
import { useUserStore } from '@/stores/user'

/**
 * 权限指令：无权限时移除元素
 * 用法：v-permission="'user:create'" 或 v-permission="['user:create', 'user:edit']"
 */
export const permission: Directive<HTMLElement, string | string[]> = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    const userStore = useUserStore()
    const { value } = binding

    if (!value) return

    const codes = Array.isArray(value) ? value : [value]
    const hasPermission = codes.some((code) => userStore.has(code))

    if (!hasPermission) {
      el.parentNode?.removeChild(el)
    }
  },
}

/**
 * 注册权限指令
 */
export function setupPermissionDirective(app: import('vue').App): void {
  app.directive('permission', permission)
}
```

- [ ] **Step 6.3: 在 main.ts 中注册指令**

修改 `EasyProduct.Admin/src/main.ts`：

```typescript
import { createPinia } from 'pinia'
import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'
import { i18n } from './i18n'
import { setupRouter } from './router'
import { setupPermissionDirective } from './directives/permission'
import './styles/index.scss'

const app = createApp(App)
app.use(createPinia())
app.use(i18n)
app.use(ElementPlus)
setupPermissionDirective(app)
setupRouter(app)
app.mount('#app')
```

- [ ] **Step 6.4: 验证类型检查**

Run:

```powershell
cd D:\4-MyProduct\EasyProduct\EasyProduct.Admin
pnpm type-check
```

Expected: 零错误

- [ ] **Step 6.5: Commit**

```powershell
git add EasyProduct.Admin/src/composables/usePermission.ts EasyProduct.Admin/src/directives/permission.ts EasyProduct.Admin/src/main.ts
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): usePermission composable + v-permission 指令（按钮级权限控制）"
```

---

### Task 7: BaseTable 组件（通用表格）

**Files:**
- Create: `EasyProduct.Admin/src/components/common/BaseTable.vue`

**Interfaces:**
- Consumes: `useTable` composable、Element Plus `el-table` / `el-pagination`
- Produces: 可复用的表格组件，支持列配置 + 分页插槽

- [ ] **Step 7.1: 创建 BaseTable.vue 组件**

```vue
<!-- src/components/common/BaseTable.vue -->
<template>
  <div class="base-table">
    <el-table
      v-loading="loading"
      :data="data"
      :border="border"
      :stripe="stripe"
      class="base-table__body"
      @selection-change="handleSelectionChange"
    >
      <slot />
    </el-table>
    <div v-if="showPagination" class="base-table__footer">
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="pageSize"
        :total="total"
        :page-sizes="pageSizes"
        layout="total, sizes, prev, pager, next, jumper"
        @current-change="handlePageChange"
        @size-change="handleSizeChange"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

interface Props {
  loading?: boolean
  data: unknown[]
  total: number
  pageSize?: number
  currentPage?: number
  pageSizes?: number[]
  showPagination?: boolean
  border?: boolean
  stripe?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  pageSize: 10,
  currentPage: 1,
  pageSizes: () => [10, 20, 50, 100],
  showPagination: true,
  border: true,
  stripe: false,
})

const emit = defineEmits<{
  'page-change': [page: number]
  'size-change': [size: number]
  'selection-change': [selection: unknown[]]
}>()

const { t } = useI18n()

const handlePageChange = (page: number): void => {
  emit('page-change', page)
}

const handleSizeChange = (size: number): void => {
  emit('size-change', size)
}

const handleSelectionChange = (selection: unknown[]): void => {
  emit('selection-change', selection)
}
</script>

<style scoped lang="scss">
.base-table {
  &__body {
    width: 100%;
  }

  &__footer {
    display: flex;
    justify-content: flex-end;
    padding: $spacing-md 0;
  }
}
</style>
```

- [ ] **Step 7.2: 验证 BaseTable 类型检查**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
pnpm lint
pnpm check:i18n
```

Expected: 三门禁全绿

- [ ] **Step 7.3: Commit**

```powershell
git add EasyProduct.Admin/src/components/common/BaseTable.vue
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): BaseTable 组件（通用表格 + 分页，支持列配置插槽）"
```

---

### Task 8: mock-server admin 分区骨架路由补齐

**Files:**
- Create: `mock-server/src/data/admin/user.ts`
- Create: `mock-server/src/data/admin/dept.ts`
- Create: `mock-server/src/data/admin/role.ts`
- Create: `mock-server/src/routes/admin/user.ts`
- Create: `mock-server/src/routes/admin/dept.ts`
- Create: `mock-server/src/routes/admin/role.ts`
- Modify: `mock-server/src/server.ts`（挂载路由）

**Interfaces:**
- Consumes: Task 1 的 `ok/fail/paginate`、`guid/isoTime`、`adminGuard`
- Produces: Basic 模块用户/部门/角色 CRUD 契约（列表/详情/新增/修改/删除）

- [ ] **Step 8.1: 创建 admin 数据文件**

`mock-server/src/data/admin/user.ts`：

```typescript
// src/data/admin/user.ts
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
```

`mock-server/src/data/admin/dept.ts`：

```typescript
// src/data/admin/dept.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface Dept {
  id: string
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  createdAt: string
  updatedAt: string
}

export const DEPTS: Dept[] = [
  {
    id: guid(),
    parentId: '0',
    name: '总公司',
    code: 'ROOT',
    sort: 1,
    status: 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  },
  {
    id: guid(),
    parentId: '0',
    name: '技术部',
    code: 'TECH',
    sort: 2,
    status: 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  },
  {
    id: guid(),
    parentId: '0',
    name: '销售部',
    code: 'SALES',
    sort: 3,
    status: 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  },
]
```

`mock-server/src/data/admin/role.ts`：

```typescript
// src/data/admin/role.ts
import Mock from 'mockjs'
import { guid, isoTime } from '../../helpers/id.js'

export interface Role {
  id: string
  name: string
  code: string
  status: 'enabled' | 'disabled'
  sort: number
  remark: string
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
  createdAt: isoTime(),
  updatedAt: isoTime(),
})
```

- [ ] **Step 8.2: 创建 admin 路由文件**

`mock-server/src/routes/admin/user.ts`：

```typescript
// src/routes/admin/user.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ADMIN_USERS } from '../../data/admin/user.js'

export const adminUserRouter = Router()

adminUserRouter.get('/basic/user/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined

  let filtered = ADMIN_USERS
  if (keyword) {
    filtered = filtered.filter(
      (u) =>
        u.userName.includes(keyword) ||
        u.realName.includes(keyword) ||
        u.email.includes(keyword),
    )
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminUserRouter.get('/basic/user/:id', (req, res) => {
  const user = ADMIN_USERS.find((u) => u.id === req.params.id)
  if (!user) {
    res.json(fail('用户不存在', 404))
    return
  }
  res.json(ok(user))
})

adminUserRouter.post('/basic/user', (req, res) => {
  const data = req.body as Partial<AdminUser>
  // 简化实现：仅返回成功
  res.json(ok({ id: guid() }, '创建成功'))
})

adminUserRouter.put('/basic/user/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

adminUserRouter.delete('/basic/user/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})
```

`mock-server/src/routes/admin/dept.ts`：

```typescript
// src/routes/admin/dept.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { DEPTS } from '../../data/admin/dept.js'

export const adminDeptRouter = Router()

adminDeptRouter.get('/basic/dept/list', (_req, res) => {
  res.json(ok(DEPTS))
})

adminDeptRouter.get('/basic/dept/:id', (req, res) => {
  const dept = DEPTS.find((d) => d.id === req.params.id)
  if (!dept) {
    res.json(fail('部门不存在', 404))
    return
  }
  res.json(ok(dept))
})

adminDeptRouter.post('/basic/dept', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

adminDeptRouter.put('/basic/dept/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

adminDeptRouter.delete('/basic/dept/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})
```

`mock-server/src/routes/admin/role.ts`：

```typescript
// src/routes/admin/role.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ROLES } from '../../data/admin/role.js'

export const adminRoleRouter = Router()

adminRoleRouter.get('/basic/role/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  res.json(ok(paginate(ROLES, pageIndex, pageSize)))
})

adminRoleRouter.get('/basic/role/:id', (req, res) => {
  const role = ROLES.find((r) => r.id === req.params.id)
  if (!role) {
    res.json(fail('角色不存在', 404))
    return
  }
  res.json(ok(role))
})

adminRoleRouter.post('/basic/role', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

adminRoleRouter.put('/basic/role/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

adminRoleRouter.delete('/basic/role/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})
```

- [ ] **Step 8.3: 修改 server.ts 挂载路由**

在 `mock-server/src/server.ts` 中添加路由导入和挂载：

```typescript
// 在文件顶部的导入区域添加：
import { adminUserRouter } from './routes/admin/user.js'
import { adminDeptRouter } from './routes/admin/dept.js'
import { adminRoleRouter } from './routes/admin/role.js'

// 在 app.use('/api/admin', adminGuard, ...) 区域添加：
app.use('/api/admin', adminGuard, adminAuthRouter, adminMenuRouter, adminDictRouter, adminUserRouter, adminDeptRouter, adminRoleRouter)
```

- [ ] **Step 8.4: 验证 mock-server 启动**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\mock-server
pnpm dev
# 另开终端验证
Invoke-RestMethod http://localhost:7700/__mock/status
```

Expected: 返回状态表

- [ ] **Step 8.5: Commit**

```powershell
git add mock-server
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(mock): admin 分区骨架路由（用户/部门/角色 CRUD）"
```

---

### Task 9: 整体验收（三门禁 + 功能验证）

**Files:**
- Verify: 所有新增文件

- [ ] **Step 9.1: Admin 三门禁验证**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
pnpm lint
pnpm check:i18n
pnpm build
```

Expected: 全部通过

- [ ] **Step 9.2: mock-server 验证**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\mock-server
pnpm dev
# 另开终端测试
Invoke-RestMethod -Uri 'http://localhost:7700/api/admin/basic/user/list?pageIndex=1&pageSize=10' -Headers @{ Authorization = 'Bearer test' }
```

Expected: 返回用户列表分页数据

- [ ] **Step 9.3: Commit（如需）**

```powershell
git status
# 如有未提交的改动：
git add .
git -c user.name='lilin' -c user.email='lilin@local' commit -m "chore: F2-0 整体验收通过"
```

---

## Self-Review

**1. Spec 覆盖检查：**

| 上游要求 | 落点 |
|----------|------|
| 前端规范 §2.4 composables 签名 | Task 1~6 签名逐字对齐 |
| useTable（loading/list/total/query/handleSearch/handleReset/handlePageChange/reload） | Task 1 实现 |
| useForm（formRef/model/rules/validate/resetFields/submitLoading/handleSubmit） | Task 2 实现 |
| useDialog（visible/payload/isEdit/open/close） | Task 3 实现 |
| useDict（按 typeCode 取 options、value→label） | Task 4 实现 |
| usePermission（has + v-permission） | Task 6 实现 |
| BaseTable 组件 | Task 7 实现 |
| mock admin 分区路由 | Task 8 实现 |

**2. Placeholder 扫描：**
- 无 TBD/TODO/FIXME 占位符
- 所有代码块包含完整实现

**3. 类型一致性：**
- 所有 composables 返回类型与签名一致
- mock 路由契约与 Admin API 层一致

---

**计划完成判定：** Task 1~9 全部 checkbox 完成，三门禁全绿，mock 接口可调用。