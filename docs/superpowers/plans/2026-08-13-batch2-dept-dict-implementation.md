# 批次2：部门管理与字典管理实现计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成部门管理页面、字典管理页面和部门选择组件的开发

**Architecture:** 采用最小可行路径，先完成部门管理，再完成字典管理。使用 F2-0 封装层（useDialog/useForm/useLocale），复用 Element Plus 组件。

**Tech Stack:** Vue 3 + TypeScript + Element Plus + Pinia + Vue Router

**设计文档：** `docs/superpowers/specs/2026-08-13-batch2-dept-dict-design.md`

**参考项目：**
- 部门管理：`D:\4-MyProject\EasyProject\PCWeb\src\views\basic\department`
- 字典管理：`D:\4-MyProject\EasyProject\PCWeb\src\views\basic\dict`

---

## 模块1：类型定义和 API 层

### Task 1: 扩展类型定义

**Files:**
- Modify: `EasyProduct.Admin/src/types/basic.ts`

- [ ] **Step 1: 在 basic.ts 中添加部门类型定义**

```typescript
/** 部门（完整模型） */
export interface Dept {
  id: string
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  leaderName?: string      // 部门负责人
  phone?: string           // 联系电话
  email?: string           // 邮箱
  fullPath?: string        // 部门路径（如：总公司/技术部/前端组）
  level?: number           // 层级
  memberCount?: number     // 成员数量
  description?: string     // 描述
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
```

- [ ] **Step 2: 在 basic.ts 中添加字典类型定义**

```typescript
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
```

- [ ] **Step 3: 提交类型定义**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/types/basic.ts && git commit -m "feat(admin): 添加部门和字典管理类型定义"
```

---

### Task 2: 创建部门 API

**Files:**
- Create: `EasyProduct.Admin/src/api/basic/dept.ts`

- [ ] **Step 1: 创建 dept.ts 文件并实现 API 函数**

```typescript
// src/api/basic/dept.ts
import { get, post, put, del } from '@/utils/request'
import type { Dept, DeptCreateParams, DeptUpdateParams } from '@/types/basic'
import type { UserInfo } from '@/types'

/** 部门树 */
export const getDeptTree = () =>
  get<Dept[]>('/api/admin/basic/dept/tree')

/** 部门详情 */
export const getDeptDetail = (id: string) =>
  get<Dept>(`/api/admin/basic/dept/${id}`)

/** 部门成员列表 */
export const getDeptUsers = (deptId: string) =>
  get<UserInfo[]>(`/api/admin/basic/dept/${deptId}/users`)

/** 新增部门 */
export const createDept = (data: DeptCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dept', data)

/** 编辑部门 */
export const updateDept = (id: string, data: DeptUpdateParams) =>
  put<{ id: string }>(`/api/admin/basic/dept/${id}`, data)

/** 删除部门 */
export const deleteDept = (id: string) =>
  del<null>(`/api/admin/basic/dept/${id}`)
```

- [ ] **Step 2: 提交部门 API**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/api/basic/dept.ts && git commit -m "feat(admin): 创建部门管理 API"
```

---

### Task 3: 扩展字典 API

**Files:**
- Modify: `EasyProduct.Admin/src/api/basic/dict.ts`

- [ ] **Step 1: 读取现有 dict.ts 文件**

运行：检查现有内容

- [ ] **Step 2: 在 dict.ts 中添加字典类型管理 API**

在文件末尾添加：

```typescript
// 字典类型管理
/** 字典类型列表（分页） */
export const getDictTypeList = (params: DictTypeQuery) =>
  get<PageResult<DictType>>('/api/admin/basic/dict-type/list', params)

/** 新增字典类型 */
export const createDictType = (data: DictTypeCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dict-type', data)

/** 编辑字典类型 */
export const updateDictType = (id: string, data: Partial<DictTypeCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/dict-type/${id}`, data)

/** 删除字典类型 */
export const deleteDictType = (id: string) =>
  del<null>(`/api/admin/basic/dict-type/${id}`)

// 字典数据管理
/** 字典数据列表（分页） */
export const getDictDataList = (params: DictDataQuery) =>
  get<PageResult<DictData>>('/api/admin/basic/dict-data/list', params)

/** 新增字典数据 */
export const createDictData = (data: DictDataCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dict-data', data)

/** 编辑字典数据 */
export const updateDictData = (id: string, data: Partial<DictDataCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/dict-data/${id}`, data)

/** 删除字典数据 */
export const deleteDictData = (id: string) =>
  del<null>(`/api/admin/basic/dict-data/${id}`)

/** 批量删除字典数据 */
export const deleteDictDataBatch = (ids: string[]) =>
  post<null>('/api/admin/basic/dict-data/batch-delete', { ids })
```

- [ ] **Step 3: 在文件顶部添加类型导入**

```typescript
import type { DictType, DictTypeQuery, DictTypeCreateParams, DictData, DictDataQuery, DictDataCreateParams } from '@/types/basic'
```

- [ ] **Step 4: 提交字典 API 扩展**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/api/basic/dict.ts && git commit -m "feat(admin): 扩展字典管理 API（类型和数据管理）"
```

---

**模块1完成检查：**
- [ ] 类型定义已添加（Dept、DictType、DictData）
- [ ] 部门 API 已创建
- [ ] 字典 API 已扩展
- [ ] 所有更改已提交

---

## 模块2：Mock 数据和路由

### Task 4: 扩展部门 Mock 数据

**Files:**
- Modify: `mock-server/src/data/admin/dept.ts`

- [ ] **Step 1: 读取现有 dept.ts 文件**

- [ ] **Step 2: 扩展部门数据结构（添加 children 和详情字段）**

替换整个文件内容为：

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
  leaderName?: string
  phone?: string
  email?: string
  fullPath?: string
  level?: number
  memberCount?: number
  description?: string
  children?: Dept[]
}

export const DEPTS: Dept[] = [
  {
    id: 'dept-root-001',
    parentId: '0',
    name: '总公司',
    code: 'ROOT',
    sort: 1,
    status: 'enabled',
    leaderName: '张总',
    phone: '13800138000',
    email: 'zhang@company.com',
    fullPath: '总公司',
    level: 1,
    memberCount: 5,
    description: '公司总部',
    children: [
      {
        id: 'dept-tech-001',
        parentId: 'dept-root-001',
        name: '技术部',
        code: 'TECH',
        sort: 1,
        status: 'enabled',
        leaderName: '李经理',
        phone: '13800138001',
        email: 'li@company.com',
        fullPath: '总公司/技术部',
        level: 2,
        memberCount: 15,
        description: '技术研发部门',
        children: [
          {
            id: 'dept-tech-fe-001',
            parentId: 'dept-tech-001',
            name: '前端组',
            code: 'TECH_FE',
            sort: 1,
            status: 'enabled',
            fullPath: '总公司/技术部/前端组',
            level: 3,
            memberCount: 5,
            description: '前端开发团队',
          },
          {
            id: 'dept-tech-be-001',
            parentId: 'dept-tech-001',
            name: '后端组',
            code: 'TECH_BE',
            sort: 2,
            status: 'enabled',
            fullPath: '总公司/技术部/后端组',
            level: 3,
            memberCount: 8,
            description: '后端开发团队',
          },
        ],
      },
      {
        id: 'dept-sales-001',
        parentId: 'dept-root-001',
        name: '销售部',
        code: 'SALES',
        sort: 2,
        status: 'enabled',
        leaderName: '王经理',
        phone: '13800138002',
        email: 'wang@company.com',
        fullPath: '总公司/销售部',
        level: 2,
        memberCount: 10,
        description: '市场销售部门',
      },
    ],
  },
]
```

- [ ] **Step 3: 提交部门 Mock 数据**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/src/data/admin/dept.ts && git commit -m "feat(mock): 扩展部门 Mock 数据结构"
```

---

### Task 5: 扩展部门 Mock 路由

**Files:**
- Modify: `mock-server/src/routes/admin/dept.ts`

- [ ] **Step 1: 读取现有 dept.ts 路由文件**

- [ ] **Step 2: 添加部门树和部门成员接口**

替换整个文件内容为：

```typescript
// src/routes/admin/dept.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { DEPTS } from '../../data/admin/dept.js'
import { guid } from '../../helpers/id.js'

export const adminDeptRouter = Router()

/** 部门树 */
adminDeptRouter.get('/basic/dept/tree', (_req, res) => {
  res.json(ok(DEPTS))
})

/** 部门详情 */
adminDeptRouter.get('/basic/dept/:id', (req, res) => {
  const findDept = (list: any[], id: string): any => {
    for (const item of list) {
      if (item.id === id) return item
      if (item.children) {
        const found = findDept(item.children, id)
        if (found) return found
      }
    }
    return null
  }

  const dept = findDept(DEPTS, req.params.id)
  if (!dept) {
    res.json(fail('部门不存在', 404))
    return
  }
  res.json(ok(dept))
})

/** 部门成员列表 */
adminDeptRouter.get('/basic/dept/:id/users', (req, res) => {
  // 返回模拟成员数据
  res.json(ok([
    {
      id: guid(),
      userName: 'user001',
      realName: '张三',
      phone: '13800138000',
      email: 'zhangsan@company.com',
      status: 'enabled',
      roles: ['技术员'],
    },
    {
      id: guid(),
      userName: 'user002',
      realName: '李四',
      phone: '13800138001',
      email: 'lisi@company.com',
      status: 'enabled',
      roles: ['开发工程师'],
    },
  ]))
})

/** 新增部门 */
adminDeptRouter.post('/basic/dept', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

/** 编辑部门 */
adminDeptRouter.put('/basic/dept/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

/** 删除部门 */
adminDeptRouter.delete('/basic/dept/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})
```

- [ ] **Step 3: 提交部门 Mock 路由**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/src/routes/admin/dept.ts && git commit -m "feat(mock): 扩展部门路由（部门树和成员列表）"
```

---

### Task 6: 创建字典 Mock 数据

**Files:**
- Create: `mock-server/src/data/admin/dict.ts`

- [ ] **Step 1: 创建 dict.ts 文件**

```typescript
// src/data/admin/dict.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface DictType {
  id: string
  name: string
  code: string
  status: 'enabled' | 'disabled'
  remark: string
}

export interface DictData {
  id: string
  typeCode: string
  value: string
  labelKey: string
  sort: number
  status: 'enabled' | 'disabled'
}

// 字典类型种子数据
export const DICT_TYPES: DictType[] = [
  {
    id: 'dict-type-001',
    name: '通用状态',
    code: 'common_status',
    status: 'enabled',
    remark: '通用状态字典',
  },
  {
    id: 'dict-type-002',
    name: '用户性别',
    code: 'user_gender',
    status: 'enabled',
    remark: '用户性别字典',
  },
  {
    id: 'dict-type-003',
    name: '订单状态',
    code: 'order_status',
    status: 'enabled',
    remark: '订单状态字典',
  },
  {
    id: 'dict-type-004',
    name: '支付方式',
    code: 'payment_method',
    status: 'enabled',
    remark: '支付方式字典',
  },
  {
    id: 'dict-type-005',
    name: '物流状态',
    code: 'logistics_status',
    status: 'disabled',
    remark: '物流状态字典',
  },
]

// 字典数据种子数据
export const DICT_DATA: DictData[] = [
  // 通用状态
  {
    id: 'dict-data-001',
    typeCode: 'common_status',
    value: 'enabled',
    labelKey: 'common.status.enabled',
    sort: 1,
    status: 'enabled',
  },
  {
    id: 'dict-data-002',
    typeCode: 'common_status',
    value: 'disabled',
    labelKey: 'common.status.disabled',
    sort: 2,
    status: 'enabled',
  },
  // 用户性别
  {
    id: 'dict-data-003',
    typeCode: 'user_gender',
    value: 'male',
    labelKey: 'common.gender.male',
    sort: 1,
    status: 'enabled',
  },
  {
    id: 'dict-data-004',
    typeCode: 'user_gender',
    value: 'female',
    labelKey: 'common.gender.female',
    sort: 2,
    status: 'enabled',
  },
  // 订单状态
  {
    id: 'dict-data-005',
    typeCode: 'order_status',
    value: 'pending',
    labelKey: 'order.status.pending',
    sort: 1,
    status: 'enabled',
  },
  {
    id: 'dict-data-006',
    typeCode: 'order_status',
    value: 'paid',
    labelKey: 'order.status.paid',
    sort: 2,
    status: 'enabled',
  },
  {
    id: 'dict-data-007',
    typeCode: 'order_status',
    value: 'shipped',
    labelKey: 'order.status.shipped',
    sort: 3,
    status: 'enabled',
  },
  {
    id: 'dict-data-008',
    typeCode: 'order_status',
    value: 'completed',
    labelKey: 'order.status.completed',
    sort: 4,
    status: 'enabled',
  },
]
```

- [ ] **Step 2: 提交字典 Mock 数据**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/src/data/admin/dict.ts && git commit -m "feat(mock): 创建字典 Mock 数据"
```

---

### Task 7: 创建字典 Mock 路由

**Files:**
- Create: `mock-server/src/routes/admin/dict.ts`

- [ ] **Step 1: 创建 dict.ts 路由文件**

```typescript
// src/routes/admin/dict.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { DICT_TYPES, DICT_DATA } from '../../data/admin/dict.js'
import { guid } from '../../helpers/id.js'

export const adminDictRouter = Router()

// ========== 字典类型管理 ==========

/** 字典类型列表（分页） */
adminDictRouter.get('/basic/dict-type/list', (req, res) => {
  const { pageIndex = 1, pageSize = 10, name, code } = req.query

  let filtered = [...DICT_TYPES]
  if (name) {
    filtered = filtered.filter(item => item.name.includes(name as string))
  }
  if (code) {
    filtered = filtered.filter(item => item.code.includes(code as string))
  }

  const start = (Number(pageIndex) - 1) * Number(pageSize)
  const end = start + Number(pageSize)
  const list = filtered.slice(start, end)

  res.json(ok({ list, total: filtered.length }))
})

/** 新增字典类型 */
adminDictRouter.post('/basic/dict-type', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

/** 编辑字典类型 */
adminDictRouter.put('/basic/dict-type/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

/** 删除字典类型 */
adminDictRouter.delete('/basic/dict-type/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})

// ========== 字典数据管理 ==========

/** 字典数据列表（分页） */
adminDictRouter.get('/basic/dict-data/list', (req, res) => {
  const { typeCode, pageIndex = 1, pageSize = 10 } = req.query

  let filtered = DICT_DATA.filter(item => item.typeCode === typeCode)
  const start = (Number(pageIndex) - 1) * Number(pageSize)
  const end = start + Number(pageSize)
  const list = filtered.slice(start, end)

  res.json(ok({ list, total: filtered.length }))
})

/** 新增字典数据 */
adminDictRouter.post('/basic/dict-data', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

/** 编辑字典数据 */
adminDictRouter.put('/basic/dict-data/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

/** 删除字典数据 */
adminDictRouter.delete('/basic/dict-data/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})

/** 批量删除字典数据 */
adminDictRouter.post('/basic/dict-data/batch-delete', (req, res) => {
  res.json(ok(null, '删除成功'))
})
```

- [ ] **Step 2: 在 mock-server 主路由中注册字典路由**

在 `mock-server/src/routes/index.ts` 中添加：

```typescript
import { adminDictRouter } from './admin/dict.js'

// 在 admin 路由组中添加
adminRouter.use('/admin', adminDictRouter)
```

- [ ] **Step 3: 提交字典 Mock 路由**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/src/routes/admin/dict.ts mock-server/src/routes/index.ts && git commit -m "feat(mock): 创建字典路由并注册到主路由"
```

---

**模块2完成检查：**
- [ ] 部门 Mock 数据已扩展
- [ ] 部门 Mock 路由已扩展
- [ ] 字典 Mock 数据已创建
- [ ] 字典 Mock 路由已创建并注册
- [ ] 所有更改已提交

---

## 模块3：部门管理页面

### Task 8: 创建部门表单弹窗组件

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/dept/components/DeptFormDialog.vue`

- [ ] **Step 1: 创建目录结构**

```bash
mkdir -p EasyProduct.Admin/src/views/basic/dept/components
```

- [ ] **Step 2: 创建 DeptFormDialog.vue 文件**

```vue
<!-- src/views/basic/dept/components/DeptFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? t('basic.dept.tree.edit') : (parentId ? t('basic.dept.tree.addChild') : t('basic.dept.tree.addRoot'))"
    width="600px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="120px"
    >
      <el-form-item :label="t('basic.dept.form.parentId')" prop="parentId">
        <el-tree-select
          v-model="formData.parentId"
          :data="treeData"
          :props="{ children: 'children', label: 'name', value: 'id' }"
          :placeholder="t('common.selectPlaceholder')"
          clearable
          check-strictly
          :render-after-expand="false"
          style="width: 100%"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('basic.dept.form.namePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.code')" prop="code">
        <el-input
          v-model="formData.code"
          :placeholder="t('basic.dept.form.codePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.sort')" prop="sort">
        <el-input-number
          v-model="formData.sort"
          :min="1"
          :max="999"
          style="width: 100%"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.status')" prop="status">
        <el-radio-group v-model="formData.status">
          <el-radio value="enabled">{{ t('common.status.enabled') }}</el-radio>
          <el-radio value="disabled">{{ t('common.status.disabled') }}</el-radio>
        </el-radio-group>
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.leaderName')" prop="leaderName">
        <el-input
          v-model="formData.leaderName"
          :placeholder="t('basic.dept.form.leaderNamePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.phone')" prop="phone">
        <el-input
          v-model="formData.phone"
          :placeholder="t('basic.dept.form.phonePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.email')" prop="email">
        <el-input
          v-model="formData.email"
          :placeholder="t('basic.dept.form.emailPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dept.form.description')" prop="description">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="3"
          :placeholder="t('basic.dept.form.descriptionPlaceholder')"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">{{ t('common.cancel') }}</el-button>
      <el-button type="primary" :loading="loading" @click="handleSubmit">
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { createDept, updateDept } from '@/api/basic/dept'
import type { Dept, DeptCreateParams } from '@/types/basic'

interface Props {
  modelValue: boolean
  deptId?: string
  parentId?: string
  treeData: Dept[]
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const formRef = ref<FormInstance>()
const loading = ref(false)

const isEdit = computed(() => !!props.deptId)

const formData = ref<DeptCreateParams>({
  parentId: '0',
  name: '',
  code: '',
  sort: 1,
  status: 'enabled',
  leaderName: '',
  phone: '',
  email: '',
  description: '',
})

const formRules: FormRules = {
  name: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  code: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  sort: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  status: [{ required: true, message: t('common.required'), trigger: 'change' }],
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val) {
    if (isEdit.value) {
      // 编辑模式：加载部门数据（这里简化为从树数据中查找）
      const dept = findDeptById(props.treeData, props.deptId!)
      if (dept) {
        formData.value = {
          parentId: dept.parentId,
          name: dept.name,
          code: dept.code,
          sort: dept.sort,
          status: dept.status,
          leaderName: dept.leaderName || '',
          phone: dept.phone || '',
          email: dept.email || '',
          description: dept.description || '',
        }
      }
    } else {
      // 新增模式
      formData.value = {
        parentId: props.parentId || '0',
        name: '',
        code: '',
        sort: 1,
        status: 'enabled',
        leaderName: '',
        phone: '',
        email: '',
        description: '',
      }
    }
  }
})

// 在树中查找部门
const findDeptById = (list: Dept[], id: string): Dept | null => {
  for (const item of list) {
    if (item.id === id) return item
    if (item.children) {
      const found = findDeptById(item.children, id)
      if (found) return found
    }
  }
  return null
}

const handleClose = () => {
  formRef.value?.resetFields()
  visible.value = false
}

const handleSubmit = async () => {
  await formRef.value?.validate()

  loading.value = true
  try {
    if (isEdit.value) {
      await updateDept(props.deptId!, formData.value)
      ElMessage.success(t('basic.dept.message.updateSuccess'))
    } else {
      await createDept(formData.value)
      ElMessage.success(t('basic.dept.message.createSuccess'))
    }
    emit('success')
    handleClose()
  } catch (error) {
    // 错误已由拦截器处理
  } finally {
    loading.value = false
  }
}
</script>
```

- [ ] **Step 3: 提交部门表单弹窗组件**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/dept/components/DeptFormDialog.vue && git commit -m "feat(admin): 创建部门表单弹窗组件"
```

---

---

### Task 9: 创建部门管理主页面

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/dept/index.vue`

- [ ] **Step 1: 创建部门管理主页面（模板部分）**

```vue
<!-- src/views/basic/dept/index.vue -->
<template>
  <div class="dept-page">
    <el-row :gutter="20">
      <!-- 左侧：部门树 -->
      <el-col :span="5">
        <el-card shadow="never" class="tree-card">
          <template #header>
            <div class="tree-header">
              <span>{{ t('basic.dept.tree.title') }}</span>
              <el-button
                v-permission="['basic:dept:edit']"
                type="primary"
                size="small"
                @click="handleAddRoot"
              >
                {{ t('basic.dept.tree.addRoot') }}
              </el-button>
            </div>
          </template>

          <el-input
            v-model="filterText"
            :placeholder="t('basic.dept.tree.searchPlaceholder')"
            clearable
            class="tree-search"
          />

          <el-tree
            ref="treeRef"
            :data="treeData"
            :props="{ children: 'children', label: 'name' }"
            node-key="id"
            highlight-current
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            default-expand-all
            @node-click="handleNodeClick"
          >
            <template #default="{ data }">
              <span class="tree-node">
                <span class="node-label">{{ data.name }}</span>
                <span v-if="data.memberCount > 0" class="node-count">
                  ({{ data.memberCount }})
                </span>
                <span class="tree-actions">
                  <el-button
                    v-permission="['basic:dept:edit']"
                    link
                    size="small"
                    type="primary"
                    @click.stop="handleAdd(data)"
                  >
                    {{ t('basic.dept.tree.addChild') }}
                  </el-button>
                  <el-button
                    v-permission="['basic:dept:edit']"
                    link
                    size="small"
                    type="primary"
                    @click.stop="handleEdit(data)"
                  >
                    {{ t('basic.dept.tree.edit') }}
                  </el-button>
                  <el-button
                    v-permission="['basic:dept:delete']"
                    link
                    size="small"
                    type="danger"
                    :disabled="data.children && data.children.length > 0"
                    @click.stop="handleDelete(data)"
                  >
                    {{ t('basic.dept.tree.delete') }}
                  </el-button>
                </span>
              </span>
            </template>
          </el-tree>
        </el-card>
      </el-col>

      <!-- 右侧：部门详情/成员列表 -->
      <el-col :span="19">
        <el-card shadow="never" class="detail-card">
          <template #header>
            <div class="detail-header">
              <span>{{ currentDept?.name || t('basic.dept.detail.selectDept') }}</span>
              <el-button
                v-if="currentDept"
                v-permission="['basic:dept:edit']"
                type="primary"
                size="small"
                @click="handleEditDetail"
              >
                {{ t('basic.dept.tree.edit') }}
              </el-button>
            </div>
          </template>

          <!-- 部门详情 -->
          <div v-if="currentDept" class="dept-info">
            <el-descriptions :column="2" border>
              <el-descriptions-item :label="t('basic.dept.detail.name')">
                {{ currentDept.name }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.code')">
                {{ currentDept.code || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.path')">
                {{ currentDept.fullPath || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.level')">
                {{ currentDept.level }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.leader')">
                {{ currentDept.leaderName || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.phone')">
                {{ currentDept.phone || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.email')">
                {{ currentDept.email || '-' }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.memberCount')">
                {{ currentDept.memberCount }}
              </el-descriptions-item>
              <el-descriptions-item :label="t('basic.dept.detail.description')" :span="2">
                {{ currentDept.description || '-' }}
              </el-descriptions-item>
            </el-descriptions>
          </div>

          <!-- 成员列表 -->
          <div v-if="currentDept" class="member-section">
            <div class="section-header">
              <span>{{ t('basic.dept.member.title') }}</span>
            </div>

            <el-table :data="memberList" v-loading="memberLoading" border stripe>
              <el-table-column prop="userName" :label="t('basic.dept.member.userName')" width="120" />
              <el-table-column prop="realName" :label="t('basic.dept.member.realName')" width="120" />
              <el-table-column prop="phone" :label="t('basic.dept.member.phone')" width="150" />
              <el-table-column prop="email" :label="t('basic.dept.member.email')" width="200" />
              <el-table-column :label="t('basic.dept.member.status')" width="100" align="center">
                <template #default="{ row }">
                  <el-tag :type="row.status === 'enabled' ? 'success' : 'danger'" size="small">
                    {{ row.status === 'enabled' ? t('common.status.enabled') : t('common.status.disabled') }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column :label="t('basic.dept.member.roles')" min-width="150">
                <template #default="{ row }">
                  <el-tag
                    v-for="role in row.roles"
                    :key="role"
                    type="primary"
                    effect="plain"
                    size="small"
                    style="margin-right: 4px"
                  >
                    {{ role }}
                  </el-tag>
                  <span v-if="!row.roles?.length">-</span>
                </template>
              </el-table-column>
            </el-table>
          </div>

          <!-- 无数据提示 -->
          <el-empty v-if="!currentDept" :description="t('basic.dept.detail.selectDeptTip')" />
        </el-card>
      </el-col>
    </el-row>

    <!-- 部门表单弹窗 -->
    <DeptFormDialog
      v-model="dialogVisible"
      :dept-id="currentDeptId"
      :parent-id="currentParentId"
      :tree-data="treeData"
      @success="handleDialogSuccess"
    />
  </div>
</template>
```

- [ ] **Step 2: 创建部门管理主页面（脚本部分）**

在同一个文件中添加 `<script setup>` 部分：

```vue
<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ElTree } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getDeptTree, getDeptUsers, deleteDept } from '@/api/basic/dept'
import type { Dept } from '@/types/basic'
import type { UserInfo } from '@/types'
import DeptFormDialog from './components/DeptFormDialog.vue'

const { t } = useLocale()

// 树相关
const treeRef = ref<InstanceType<typeof ElTree>>()
const treeData = ref<Dept[]>([])
const filterText = ref('')
const loading = ref(false)

// 当前选中的部门
const currentDept = ref<Dept | null>(null)
const memberList = ref<UserInfo[]>([])
const memberLoading = ref(false)

// 弹窗相关
const dialogVisible = ref(false)
const currentDeptId = ref<string | undefined>(undefined)
const currentParentId = ref<string | null>(null)

// 保存后需要重新选中的部门ID
const refreshSelectDeptId = ref<string | null>(null)

// 过滤树节点
const filterNode = (value: string, data: any) => {
  if (!value) return true
  return data.name?.includes(value)
}

watch(filterText, (val) => {
  treeRef.value?.filter(val)
})

onMounted(() => {
  loadTree()
})

// 加载部门树
const loadTree = async () => {
  loading.value = true
  try {
    const data = await getDeptTree()
    treeData.value = data

    // 如果有需要重新选中的部门ID，刷新后重新选中
    if (refreshSelectDeptId.value) {
      setTimeout(() => {
        treeRef.value?.setCurrentKey(refreshSelectDeptId.value)
        const node = findDeptById(treeData.value, refreshSelectDeptId.value!)
        if (node) {
          currentDept.value = node
          loadMembers(node.id)
        }
        refreshSelectDeptId.value = null
      }, 100)
    }
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    loading.value = false
  }
}

// 根据ID在树中查找部门
const findDeptById = (list: Dept[], id: string): Dept | null => {
  for (const item of list) {
    if (item.id === id) return item
    if (item.children) {
      const found = findDeptById(item.children, id)
      if (found) return found
    }
  }
  return null
}

// 加载部门成员
const loadMembers = async (deptId: string) => {
  memberLoading.value = true
  try {
    const users = await getDeptUsers(deptId)
    memberList.value = users
  } catch (error) {
    memberList.value = []
  } finally {
    memberLoading.value = false
  }
}

// 点击树节点
const handleNodeClick = async (data: Dept) => {
  currentDept.value = data
  loadMembers(data.id)
}

// 新增根部门
const handleAddRoot = () => {
  currentDeptId.value = undefined
  currentParentId.value = null
  dialogVisible.value = true
}

// 新增子部门
const handleAdd = (data: Dept) => {
  currentDeptId.value = undefined
  currentParentId.value = data.id
  dialogVisible.value = true
}

// 编辑部门
const handleEdit = (data: Dept) => {
  currentDeptId.value = data.id
  currentParentId.value = null
  refreshSelectDeptId.value = data.id
  dialogVisible.value = true
}

// 从右侧详情区编辑当前部门
const handleEditDetail = () => {
  if (currentDept.value) {
    currentDeptId.value = currentDept.value.id
    currentParentId.value = null
    refreshSelectDeptId.value = currentDept.value.id
    dialogVisible.value = true
  }
}

// 弹窗保存成功后的处理
const handleDialogSuccess = () => {
  loadTree()
}

// 删除部门
const handleDelete = async (data: Dept) => {
  if (data.children && data.children.length > 0) {
    ElMessage.warning(t('basic.dept.message.hasChildren'))
    return
  }

  try {
    await ElMessageBox.confirm(
      t('basic.dept.message.deleteConfirm', { name: data.name }),
      t('common.message.warning'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDept(data.id)
    ElMessage.success(t('basic.dept.message.deleteSuccess'))
    if (currentDept.value?.id === data.id) {
      currentDept.value = null
      memberList.value = []
    }
    loadTree()
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>
```

- [ ] **Step 3: 创建部门管理主页面（样式部分）**

在同一个文件中添加 `<style>` 部分：

```vue
<style scoped lang="scss">
.dept-page {
  padding: 20px;
  height: calc(100vh - 100px);

  .el-row {
    height: 100%;
  }

  .tree-card {
    height: 100%;

    .tree-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .tree-search {
      margin-bottom: 12px;
    }

    .tree-node {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: space-between;
      font-size: 14px;
      padding-right: 8px;

      .node-label {
        flex: 1;
      }

      .node-count {
        color: #909399;
        font-size: 12px;
        margin-left: 4px;
      }

      .tree-actions {
        display: none;
      }
    }

    :deep(.el-tree-node__content:hover) .tree-actions {
      display: inline-flex;
    }
  }

  .detail-card {
    height: 100%;

    .detail-header {
      font-size: 16px;
      font-weight: 500;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .dept-info {
      margin-bottom: 20px;
    }

    .member-section {
      .section-header {
        font-size: 14px;
        font-weight: 500;
        margin-bottom: 12px;
        padding-bottom: 8px;
        border-bottom: 1px solid #ebeef5;
      }
    }
  }
}
</style>
```

- [ ] **Step 4: 提交部门管理主页面**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/dept/index.vue && git commit -m "feat(admin): 创建部门管理主页面"
```

---

**模块3完成检查：**
- [ ] 部门表单弹窗组件已创建
- [ ] 部门管理主页面已创建
- [ ] 所有更改已提交

---

## 模块4：字典管理页面

### Task 10: 创建字典类型表单弹窗组件

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/dict/components/DictTypeFormDialog.vue`

- [ ] **Step 1: 创建目录结构**

```bash
mkdir -p EasyProduct.Admin/src/views/basic/dict/components
```

- [ ] **Step 2: 创建 DictTypeFormDialog.vue 文件**

```vue
<!-- src/views/basic/dict/components/DictTypeFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? t('basic.dict.editType') : t('basic.dict.addType')"
    width="500px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item :label="t('basic.dict.form.typeName')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('basic.dict.form.typeNamePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dict.form.typeCode')" prop="code">
        <el-input
          v-model="formData.code"
          :placeholder="t('basic.dict.form.typeCodePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('common.status')" prop="status">
        <el-radio-group v-model="formData.status">
          <el-radio value="enabled">{{ t('common.status.enabled') }}</el-radio>
          <el-radio value="disabled">{{ t('common.status.disabled') }}</el-radio>
        </el-radio-group>
      </el-form-item>

      <el-form-item :label="t('basic.dict.form.remark')" prop="remark">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="3"
          :placeholder="t('basic.dict.form.remarkPlaceholder')"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">{{ t('common.cancel') }}</el-button>
      <el-button type="primary" :loading="loading" @click="handleSubmit">
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { createDictType, updateDictType, getDictTypeList } from '@/api/basic/dict'
import type { DictTypeCreateParams } from '@/types/basic'

interface Props {
  modelValue: boolean
  typeId?: string
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const formRef = ref<FormInstance>()
const loading = ref(false)

const isEdit = computed(() => !!props.typeId)

const formData = ref<DictTypeCreateParams>({
  name: '',
  code: '',
  status: 'enabled',
  remark: '',
})

const formRules: FormRules = {
  name: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  code: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  status: [{ required: true, message: t('common.required'), trigger: 'change' }],
}

// 监听弹窗打开
watch(visible, async (val) => {
  if (val && isEdit.value) {
    // 编辑模式：加载字典类型数据
    try {
      const data = await getDictTypeList({ pageIndex: 1, pageSize: 100 })
      const type = data.list.find(item => item.id === props.typeId)
      if (type) {
        formData.value = {
          name: type.name,
          code: type.code,
          status: type.status,
          remark: type.remark,
        }
      }
    } catch (error) {
      // 错误已由拦截器处理
    }
  } else if (val) {
    // 新增模式
    formData.value = {
      name: '',
      code: '',
      status: 'enabled',
      remark: '',
    }
  }
})

const handleClose = () => {
  formRef.value?.resetFields()
  visible.value = false
}

const handleSubmit = async () => {
  await formRef.value?.validate()

  loading.value = true
  try {
    if (isEdit.value) {
      await updateDictType(props.typeId!, formData.value)
      ElMessage.success(t('common.updateSuccess'))
    } else {
      await createDictType(formData.value)
      ElMessage.success(t('common.createSuccess'))
    }
    emit('success')
    handleClose()
  } catch (error) {
    // 错误已由拦截器处理
  } finally {
    loading.value = false
  }
}
</script>
```

- [ ] **Step 3: 提交字典类型表单弹窗组件**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/dict/components/DictTypeFormDialog.vue && git commit -m "feat(admin): 创建字典类型表单弹窗组件"
```

---

### Task 11: 创建字典数据表单弹窗组件

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/dict/components/DictDataFormDialog.vue`

- [ ] **Step 1: 创建 DictDataFormDialog.vue 文件**

```vue
<!-- src/views/basic/dict/components/DictDataFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? t('basic.dict.editData') : t('basic.dict.addData')"
    width="500px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item :label="t('basic.dict.form.dataLabel')" prop="labelKey">
        <el-input
          v-model="formData.labelKey"
          :placeholder="t('basic.dict.form.dataLabelPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dict.form.dataValue')" prop="value">
        <el-input
          v-model="formData.value"
          :placeholder="t('basic.dict.form.dataValuePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('basic.dict.form.dataSort')" prop="sort">
        <el-input-number
          v-model="formData.sort"
          :min="1"
          :max="999"
          style="width: 100%"
        />
      </el-form-item>

      <el-form-item :label="t('common.status')" prop="status">
        <el-radio-group v-model="formData.status">
          <el-radio value="enabled">{{ t('common.status.enabled') }}</el-radio>
          <el-radio value="disabled">{{ t('common.status.disabled') }}</el-radio>
        </el-radio-group>
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">{{ t('common.cancel') }}</el-button>
      <el-button type="primary" :loading="loading" @click="handleSubmit">
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { createDictData, updateDictData } from '@/api/basic/dict'
import type { DictDataCreateParams, DictData } from '@/types/basic'

interface Props {
  modelValue: boolean
  dataId?: string
  typeCode: string
  currentData?: DictData
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const formRef = ref<FormInstance>()
const loading = ref(false)

const isEdit = computed(() => !!props.dataId)

const formData = ref<DictDataCreateParams>({
  typeCode: props.typeCode,
  value: '',
  labelKey: '',
  sort: 1,
  status: 'enabled',
})

const formRules: FormRules = {
  labelKey: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  value: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  sort: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  status: [{ required: true, message: t('common.required'), trigger: 'change' }],
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val) {
    if (isEdit.value && props.currentData) {
      // 编辑模式
      formData.value = {
        typeCode: props.currentData.typeCode,
        value: props.currentData.value,
        labelKey: props.currentData.labelKey,
        sort: props.currentData.sort,
        status: props.currentData.status,
      }
    } else {
      // 新增模式
      formData.value = {
        typeCode: props.typeCode,
        value: '',
        labelKey: '',
        sort: 1,
        status: 'enabled',
      }
    }
  }
})

const handleClose = () => {
  formRef.value?.resetFields()
  visible.value = false
}

const handleSubmit = async () => {
  await formRef.value?.validate()

  loading.value = true
  try {
    if (isEdit.value) {
      await updateDictData(props.dataId!, formData.value)
      ElMessage.success(t('common.updateSuccess'))
    } else {
      await createDictData(formData.value)
      ElMessage.success(t('common.createSuccess'))
    }
    emit('success')
    handleClose()
  } catch (error) {
    // 错误已由拦截器处理
  } finally {
    loading.value = false
  }
}
</script>
```

- [ ] **Step 2: 提交字典数据表单弹窗组件**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/dict/components/DictDataFormDialog.vue && git commit -m "feat(admin): 创建字典数据表单弹窗组件"
```

---

---

### Task 12: 创建字典管理主页面

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/dict/index.vue`

由于字典管理主页面代码较长，我将分成4个步骤编写。

- [ ] **Step 1: 创建字典管理主页面（模板部分）**

```vue
<!-- src/views/basic/dict/index.vue -->
<template>
  <div class="dict-page">
    <el-card shadow="never">
      <template #header>
        <span>{{ t('basic.dict.title') }}</span>
      </template>

      <div class="dict-layout">
        <!-- 左侧：字典类型 -->
        <div class="dict-type-panel">
          <div class="panel-header">
            <span>{{ t('basic.dict.dictType') }}</span>
            <el-button
              v-permission="['basic:dict:edit']"
              type="primary"
              size="small"
              @click="handleAddType"
            >
              {{ t('basic.dict.addType') }}
            </el-button>
          </div>

          <!-- 搜索 -->
          <el-input
            v-model="typeSearchKey"
            :placeholder="t('basic.dict.searchTypePlaceholder')"
            clearable
            style="margin-bottom: 12px"
          />

          <!-- 类型列表 -->
          <el-table
            v-loading="typeLoading"
            :data="filteredTypeList"
            highlight-current-row
            @current-change="handleTypeSelect"
          >
            <el-table-column prop="name" :label="t('basic.dict.typeName')">
              <template #default="{ row }">
                <span class="type-name" :class="{ disabled: row.status === 'disabled' }">
                  {{ row.name }}
                </span>
              </template>
            </el-table-column>
            <el-table-column :label="t('common.status')" width="80" align="center">
              <template #default="{ row }">
                <el-switch
                  v-model="row.status"
                  active-value="enabled"
                  inactive-value="disabled"
                  size="small"
                  @change="handleTypeStatusChange(row)"
                />
              </template>
            </el-table-column>
            <el-table-column :label="t('common.operation')" width="100" align="center">
              <template #default="{ row }">
                <el-button
                  v-permission="['basic:dict:edit']"
                  link
                  type="primary"
                  size="small"
                  @click="handleEditType(row)"
                >
                  {{ t('common.edit') }}
                </el-button>
                <el-button
                  v-permission="['basic:dict:delete']"
                  link
                  type="danger"
                  size="small"
                  @click="handleDeleteType(row)"
                >
                  {{ t('common.delete') }}
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <!-- 右侧：字典数据 -->
        <div class="dict-data-panel">
          <div class="panel-header">
            <span>
              {{ t('basic.dict.dictData') }}
              <template v-if="selectedType">
                - {{ selectedType.name }}
              </template>
            </span>
            <div>
              <el-button
                v-permission="['basic:dict:delete']"
                type="danger"
                size="small"
                :disabled="selectedDataRows.length === 0"
                @click="handleBatchDeleteData"
              >
                {{ t('basic.dict.batchDelete') }}
              </el-button>
              <el-button
                v-permission="['basic:dict:edit']"
                type="primary"
                size="small"
                :disabled="!selectedType"
                @click="handleAddData"
              >
                {{ t('basic.dict.addData') }}
              </el-button>
            </div>
          </div>

          <!-- 提示或数据表格 -->
          <template v-if="!selectedType">
            <el-empty :description="t('basic.dict.selectDictType')" />
          </template>
          <template v-else>
            <el-table
              v-loading="dataLoading"
              :data="dataTableList"
              @selection-change="handleDataSelectionChange"
            >
              <el-table-column type="selection" width="50" align="center" />
              <el-table-column prop="labelKey" :label="t('basic.dict.dataLabel')" min-width="120" />
              <el-table-column prop="value" :label="t('basic.dict.dataValue')" min-width="120" />
              <el-table-column prop="sort" :label="t('basic.dict.dataSort')" width="80" align="center" />
              <el-table-column :label="t('common.status')" width="80" align="center">
                <template #default="{ row }">
                  <el-switch
                    v-model="row.status"
                    active-value="enabled"
                    inactive-value="disabled"
                    size="small"
                    @change="handleDataStatusChange(row)"
                  />
                </template>
              </el-table-column>
              <el-table-column :label="t('common.operation')" width="100" align="center">
                <template #default="{ row }">
                  <el-button
                    v-permission="['basic:dict:edit']"
                    link
                    type="primary"
                    size="small"
                    @click="handleEditData(row)"
                  >
                    {{ t('common.edit') }}
                  </el-button>
                  <el-button
                    v-permission="['basic:dict:delete']"
                    link
                    type="danger"
                    size="small"
                    @click="handleDeleteData(row)"
                  >
                    {{ t('common.delete') }}
                  </el-button>
                </template>
              </el-table-column>
            </el-table>

            <el-pagination
              v-model:current-page="dataQueryParams.pageIndex"
              v-model:page-size="dataQueryParams.pageSize"
              :total="dataTotal"
              :page-sizes="[10, 20, 50]"
              layout="total, sizes, prev, pager, next"
              style="margin-top: 12px; justify-content: flex-end"
              @size-change="loadDataList"
              @current-change="loadDataList"
            />
          </template>
        </div>
      </div>
    </el-card>

    <!-- 字典类型弹窗 -->
    <DictTypeFormDialog
      v-model="typeDialogVisible"
      :type-id="currentTypeId"
      @success="handleTypeSuccess"
    />

    <!-- 字典数据弹窗 -->
    <DictDataFormDialog
      v-model="dataDialogVisible"
      :data-id="currentDataId"
      :type-code="selectedType?.code || ''"
      :current-data="currentData"
      @success="loadDataList"
    />
  </div>
</template>
```

**说明：模板部分参考了 EasyProject PCWeb 的实现，适配了 EasyProduct 的规范（权限、i18n、状态字段）。**

---

---

- [ ] **Step 2: 创建字典管理主页面（脚本部分 - 状态和方法）**

在同一个文件中添加 `<script setup>` 部分的状态定义和基础方法：

```vue
<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import {
  getDictTypeList,
  getDictDataList,
  updateDictType,
  updateDictData,
  deleteDictType,
  deleteDictData,
  deleteDictDataBatch,
} from '@/api/basic/dict'
import type { DictType, DictData } from '@/types/basic'
import DictTypeFormDialog from './components/DictTypeFormDialog.vue'
import DictDataFormDialog from './components/DictDataFormDialog.vue'

const { t } = useLocale()

// 字典类型状态
const typeLoading = ref(false)
const typeList = ref<DictType[]>([])
const typeSearchKey = ref('')
const selectedType = ref<DictType | null>(null)

// 字典数据状态
const dataLoading = ref(false)
const dataTableList = ref<DictData[]>([])
const dataTotal = ref(0)
const selectedDataRows = ref<DictData[]>([])

const dataQueryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  typeCode: '',
})

// 弹窗状态
const typeDialogVisible = ref(false)
const currentTypeId = ref<string | undefined>(undefined)
const dataDialogVisible = ref(false)
const currentDataId = ref<string | undefined>(undefined)
const currentData = ref<DictData | undefined>(undefined)

// 过滤后的类型列表
const filteredTypeList = computed(() => {
  if (!typeSearchKey.value) return typeList.value
  return typeList.value.filter((item) =>
    item.name.toLowerCase().includes(typeSearchKey.value.toLowerCase())
  )
})

onMounted(() => {
  loadTypeList()
})

// 加载字典类型列表
const loadTypeList = async () => {
  typeLoading.value = true
  try {
    const data = await getDictTypeList({
      pageIndex: 1,
      pageSize: 100,
    })
    typeList.value = data.list
    // 默认选中第一个
    if (typeList.value.length > 0 && !selectedType.value) {
      handleTypeSelect(typeList.value[0])
    }
  } catch (error) {
    // Error handled by interceptor
  } finally {
    typeLoading.value = false
  }
}

// 加载字典数据列表
const loadDataList = async () => {
  if (!selectedType.value) return

  dataLoading.value = true
  try {
    const data = await getDictDataList({
      ...dataQueryParams,
      typeCode: selectedType.value.code,
    })
    dataTableList.value = data.list
    dataTotal.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    dataLoading.value = false
  }
}

// 选择字典类型
const handleTypeSelect = (row: DictType | null) => {
  selectedType.value = row
  if (row) {
    dataQueryParams.typeCode = row.code
    dataQueryParams.pageIndex = 1
    loadDataList()
  }
}
```

---

- [ ] **Step 3: 创建字典管理主页面（脚本部分 - 操作方法）**

继续在同一个 `<script setup>` 中添加操作方法：

```vue
// 字典类型操作
const handleAddType = () => {
  currentTypeId.value = undefined
  typeDialogVisible.value = true
}

const handleEditType = (row: DictType) => {
  currentTypeId.value = row.id
  typeDialogVisible.value = true
}

const handleTypeSuccess = () => {
  loadTypeList()
}

const handleTypeStatusChange = async (row: DictType) => {
  try {
    await updateDictType(row.id, {
      status: row.status,
    })
    ElMessage.success(row.status === 'enabled' ? t('basic.dict.enabled') : t('basic.dict.disabled'))
  } catch (error) {
    row.status = row.status === 'enabled' ? 'disabled' : 'enabled'
  }
}

const handleDeleteType = async (row: DictType) => {
  try {
    await ElMessageBox.confirm(
      t('basic.dict.deleteTypeConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDictType(row.id)
    ElMessage.success(t('basic.dict.deleteTypeSuccess'))
    // 清空选中状态
    if (selectedType.value?.id === row.id) {
      selectedType.value = null
      dataTableList.value = []
      dataTotal.value = 0
    }
    loadTypeList()
  } catch (error) {
    // User cancelled or request failed
  }
}

// 字典数据操作
const handleAddData = () => {
  currentDataId.value = undefined
  currentData.value = undefined
  dataDialogVisible.value = true
}

const handleEditData = (row: DictData) => {
  currentDataId.value = row.id
  currentData.value = row
  dataDialogVisible.value = true
}

const handleDeleteData = async (row: DictData) => {
  try {
    await ElMessageBox.confirm(
      t('basic.dict.deleteDataConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDictData(row.id)
    ElMessage.success(t('basic.dict.deleteDataSuccess'))
    loadDataList()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleDataStatusChange = async (row: DictData) => {
  try {
    await updateDictData(row.id, {
      status: row.status,
    })
    ElMessage.success(row.status === 'enabled' ? t('basic.dict.enabled') : t('basic.dict.disabled'))
  } catch (error) {
    row.status = row.status === 'enabled' ? 'disabled' : 'enabled'
  }
}

const handleDataSelectionChange = (rows: DictData[]) => {
  selectedDataRows.value = rows
}

const handleBatchDeleteData = async () => {
  if (selectedDataRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('basic.dict.deleteDatasConfirm', { count: selectedDataRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedDataRows.value.map((row) => row.id)
    await deleteDictDataBatch(ids)
    ElMessage.success(t('basic.dict.deleteDataSuccess'))
    loadDataList()
  } catch (error) {
    // User cancelled or request failed
  }
}
```

---

- [ ] **Step 4: 创建字典管理主页面（样式部分）**

在同一个文件中添加 `<style>` 部分：

```vue
<style scoped lang="scss">
.dict-page {
  padding: 20px;

  .dict-layout {
    display: flex;
    gap: 20px;
    min-height: 500px;
  }

  .dict-type-panel {
    width: 320px;
    flex-shrink: 0;
    border-right: 1px solid var(--el-border-color-lighter);
    padding-right: 20px;
  }

  .dict-data-panel {
    flex: 1;
    min-width: 0;
  }

  .panel-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    font-weight: 500;
  }

  .type-name {
    cursor: pointer;

    &.disabled {
      color: var(--el-text-color-placeholder);
    }
  }
}
</style>
```

- [ ] **Step 5: 提交字典管理主页面**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/dict/index.vue && git commit -m "feat(admin): 创建字典管理主页面（主从结构）"
```

---

**模块4完成检查：**
- [ ] 字典类型表单弹窗组件已创建
- [ ] 字典数据表单弹窗组件已创建
- [ ] 字典管理主页面已创建
- [ ] 所有更改已提交

---

## 模块5：部门选择组件和 i18n 翻译

### Task 13: 创建部门选择组件

**Files:**
- Create: `EasyProduct.Admin/src/components/common/DeptSelect.vue`

- [ ] **Step 1: 创建 DeptSelect.vue 组件**

```vue
<!-- src/components/common/DeptSelect.vue -->
<template>
  <el-tree-select
    v-model="selectedValue"
    :data="treeData"
    :props="{ children: 'children', label: 'name', value: 'id' }"
    :placeholder="placeholder"
    :disabled="disabled"
    clearable
    filterable
    check-strictly
    :render-after-expand="false"
    :loading="loading"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { getDeptTree } from '@/api/basic/dept'
import type { Dept } from '@/types/basic'

interface Props {
  modelValue?: string
  placeholder?: string
  disabled?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '请选择部门',
  disabled: false,
})

const emit = defineEmits<Emits>()

const selectedValue = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val as string),
})

const treeData = ref<Dept[]>([])
const loading = ref(false)

const loadTree = async () => {
  loading.value = true
  try {
    const data = await getDeptTree()
    treeData.value = data
  } catch (error) {
    // 错误已由拦截器处理
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadTree()
})

// 暴露刷新方法
defineExpose({
  refresh: loadTree,
})
</script>
```

- [ ] **Step 2: 提交部门选择组件**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/components/common/DeptSelect.vue && git commit -m "feat(admin): 创建部门选择组件"
```

---

### Task 14: 添加 i18n 翻译（中文）

**Files:**
- Modify: `EasyProduct.Admin/src/i18n/locales/zh-CN/basic.json`

- [ ] **Step 1: 读取现有 basic.json 文件**

- [ ] **Step 2: 在 basic.json 中添加部门管理翻译**

```json
{
  "dept": {
    "title": "部门管理",
    "tree": {
      "title": "部门树",
      "addRoot": "新增根部门",
      "addChild": "新增子部门",
      "edit": "编辑",
      "delete": "删除",
      "searchPlaceholder": "搜索部门名称"
    },
    "detail": {
      "selectDept": "请选择部门",
      "selectDeptTip": "请在左侧选择部门查看详情",
      "name": "部门名称",
      "code": "部门编码",
      "path": "部门路径",
      "level": "层级",
      "leader": "部门负责人",
      "phone": "联系电话",
      "email": "邮箱",
      "memberCount": "成员数量",
      "description": "描述"
    },
    "member": {
      "title": "部门成员",
      "userName": "用户名",
      "realName": "真实姓名",
      "phone": "手机号",
      "email": "邮箱",
      "status": "状态",
      "roles": "角色"
    },
    "form": {
      "parentId": "上级部门",
      "name": "部门名称",
      "code": "部门编码",
      "sort": "排序",
      "status": "状态",
      "leaderName": "部门负责人",
      "phone": "联系电话",
      "email": "邮箱",
      "description": "描述",
      "namePlaceholder": "请输入部门名称",
      "codePlaceholder": "请输入部门编码",
      "sortPlaceholder": "请输入排序",
      "leaderNamePlaceholder": "请输入负责人姓名",
      "phonePlaceholder": "请输入联系电话",
      "emailPlaceholder": "请输入邮箱",
      "descriptionPlaceholder": "请输入描述"
    },
    "message": {
      "hasChildren": "存在子部门，无法删除",
      "deleteConfirm": "确定要删除部门【{name}】吗？",
      "deleteSuccess": "删除成功",
      "createSuccess": "创建成功",
      "updateSuccess": "更新成功"
    }
  }
}
```

- [ ] **Step 3: 在 basic.json 中添加字典管理翻译**

```json
{
  "dict": {
    "title": "字典管理",
    "dictType": "字典类型",
    "dictData": "字典数据",
    "addType": "新增类型",
    "editType": "编辑类型",
    "deleteType": "删除类型",
    "addData": "新增数据",
    "editData": "编辑数据",
    "deleteData": "删除数据",
    "batchDelete": "批量删除",
    "searchTypePlaceholder": "搜索类型名称",
    "selectDictType": "请在左侧选择字典类型",
    "typeName": "类型名称",
    "typeCode": "类型编码",
    "dataLabel": "字典标签",
    "dataValue": "字典值",
    "dataSort": "排序",
    "status": "状态",
    "operation": "操作",
    "enabled": "已启用",
    "disabled": "已禁用",
    "deleteTypeConfirm": "确定要删除该字典类型吗？",
    "deleteDataConfirm": "确定要删除该字典数据吗？",
    "deleteDatasConfirm": "确定要删除选中的 {count} 条字典数据吗？",
    "deleteTypeSuccess": "删除字典类型成功",
    "deleteDataSuccess": "删除字典数据成功",
    "form": {
      "typeName": "类型名称",
      "typeCode": "类型编码",
      "remark": "备注",
      "dataLabel": "字典标签",
      "dataValue": "字典值",
      "dataSort": "排序",
      "typeNamePlaceholder": "请输入类型名称",
      "typeCodePlaceholder": "请输入类型编码",
      "remarkPlaceholder": "请输入备注",
      "dataLabelPlaceholder": "请输入字典标签",
      "dataValuePlaceholder": "请输入字典值",
      "dataSortPlaceholder": "请输入排序"
    }
  }
}
```

- [ ] **Step 4: 提交中文 i18n 翻译**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/i18n/locales/zh-CN/basic.json && git commit -m "feat(i18n): 添加部门和字典管理中文翻译"
```

---

### Task 15: 添加 i18n 翻译（英文）

**Files:**
- Modify: `EasyProduct.Admin/src/i18n/locales/en-US/basic.json`

- [ ] **Step 1: 读取现有 basic.json 文件**

- [ ] **Step 2: 在 basic.json 中添加部门管理英文翻译**

```json
{
  "dept": {
    "title": "Department Management",
    "tree": {
      "title": "Department Tree",
      "addRoot": "Add Root",
      "addChild": "Add Child",
      "edit": "Edit",
      "delete": "Delete",
      "searchPlaceholder": "Search department name"
    },
    "detail": {
      "selectDept": "Select Department",
      "selectDeptTip": "Please select a department on the left to view details",
      "name": "Department Name",
      "code": "Department Code",
      "path": "Department Path",
      "level": "Level",
      "leader": "Leader",
      "phone": "Phone",
      "email": "Email",
      "memberCount": "Member Count",
      "description": "Description"
    },
    "member": {
      "title": "Department Members",
      "userName": "Username",
      "realName": "Real Name",
      "phone": "Phone",
      "email": "Email",
      "status": "Status",
      "roles": "Roles"
    },
    "form": {
      "parentId": "Parent Department",
      "name": "Department Name",
      "code": "Department Code",
      "sort": "Sort",
      "status": "Status",
      "leaderName": "Leader Name",
      "phone": "Phone",
      "email": "Email",
      "description": "Description",
      "namePlaceholder": "Please enter department name",
      "codePlaceholder": "Please enter department code",
      "sortPlaceholder": "Please enter sort",
      "leaderNamePlaceholder": "Please enter leader name",
      "phonePlaceholder": "Please enter phone",
      "emailPlaceholder": "Please enter email",
      "descriptionPlaceholder": "Please enter description"
    },
    "message": {
      "hasChildren": "Cannot delete department with children",
      "deleteConfirm": "Are you sure to delete department [{name}]?",
      "deleteSuccess": "Deleted successfully",
      "createSuccess": "Created successfully",
      "updateSuccess": "Updated successfully"
    }
  }
}
```

- [ ] **Step 3: 在 basic.json 中添加字典管理英文翻译**

```json
{
  "dict": {
    "title": "Dictionary Management",
    "dictType": "Dictionary Type",
    "dictData": "Dictionary Data",
    "addType": "Add Type",
    "editType": "Edit Type",
    "deleteType": "Delete Type",
    "addData": "Add Data",
    "editData": "Edit Data",
    "deleteData": "Delete Data",
    "batchDelete": "Batch Delete",
    "searchTypePlaceholder": "Search type name",
    "selectDictType": "Please select a dictionary type on the left",
    "typeName": "Type Name",
    "typeCode": "Type Code",
    "dataLabel": "Dictionary Label",
    "dataValue": "Dictionary Value",
    "dataSort": "Sort",
    "status": "Status",
    "operation": "Operation",
    "enabled": "Enabled",
    "disabled": "Disabled",
    "deleteTypeConfirm": "Are you sure to delete this dictionary type?",
    "deleteDataConfirm": "Are you sure to delete this dictionary data?",
    "deleteDatasConfirm": "Are you sure to delete {count} selected dictionary data?",
    "deleteTypeSuccess": "Dictionary type deleted successfully",
    "deleteDataSuccess": "Dictionary data deleted successfully",
    "form": {
      "typeName": "Type Name",
      "typeCode": "Type Code",
      "remark": "Remark",
      "dataLabel": "Dictionary Label",
      "dataValue": "Dictionary Value",
      "dataSort": "Sort",
      "typeNamePlaceholder": "Please enter type name",
      "typeCodePlaceholder": "Please enter type code",
      "remarkPlaceholder": "Please enter remark",
      "dataLabelPlaceholder": "Please enter dictionary label",
      "dataValuePlaceholder": "Please enter dictionary value",
      "dataSortPlaceholder": "Please enter sort"
    }
  }
}
```

- [ ] **Step 4: 提交英文 i18n 翻译**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/i18n/locales/en-US/basic.json && git commit -m "feat(i18n): 添加部门和字典管理英文翻译"
```

---

### Task 16: 添加路由配置

**Files:**
- Modify: `EasyProduct.Admin/src/router/modules/basic.ts`

- [ ] **Step 1: 读取现有 basic.ts 路由文件**

- [ ] **Step 2: 在路由配置中添加部门和字典路由**

在 `children` 数组中添加：

```typescript
{
  path: 'dept',
  name: 'basic-dept',
  component: () => import('@/views/basic/dept/index.vue'),
  meta: { title: 'menu.basic.dept', icon: 'OfficeBuilding' }
},
{
  path: 'dict',
  name: 'basic-dict',
  component: () => import('@/views/basic/dict/index.vue'),
  meta: { title: 'menu.basic.dict', icon: 'Collection' }
},
```

- [ ] **Step 3: 在 i18n 的 menu.json 中添加菜单翻译**

在 `zh-CN/menu.json` 中添加：

```json
{
  "basic.dept": "部门管理",
  "basic.dict": "字典管理"
}
```

在 `en-US/menu.json` 中添加：

```json
{
  "basic.dept": "Department",
  "basic.dict": "Dictionary"
}
```

- [ ] **Step 4: 提交路由和菜单配置**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/router/modules/basic.ts EasyProduct.Admin/src/i18n/locales/zh-CN/menu.json EasyProduct.Admin/src/i18n/locales/en-US/menu.json && git commit -m "feat(admin): 添加部门和字典管理路由配置"
```

---

**模块5完成检查：**
- [ ] 部门选择组件已创建
- [ ] 中文 i18n 翻译已添加
- [ ] 英文 i18n 翻译已添加
- [ ] 路由配置已添加
- [ ] 菜单翻译已添加
- [ ] 所有更改已提交

---

## 验收清单

- [ ] **功能验收**
  - [ ] 部门树可正常展示、搜索
  - [ ] 可新增根部门和子部门
  - [ ] 可编辑、删除部门
  - [ ] 右侧详情和成员列表正常显示
  - [ ] 字典类型列表可正常展示、搜索
  - [ ] 可新增、编辑、删除字典类型
  - [ ] 字典数据表格可正常展示、分页
  - [ ] 可新增、编辑、删除字典数据
  - [ ] 可批量删除字典数据

- [ ] **代码验收**
  - [ ] `pnpm type-check` 零错误
  - [ ] `pnpm lint` 通过
  - [ ] `pnpm check:i18n` 无硬编码中文

- [ ] **集成验收**
  - [ ] 部门选择组件可在其他页面使用
  - [ ] 路由和菜单正常显示

---

**实现计划完成！**

**执行方式选择：**

**1. Subagent-Driven（推荐）** - 我将逐任务派发子代理执行，每个任务完成后进行审查

**2. Inline Execution** - 我将在当前会话中使用 executing-plans skill 批量执行任务

请选择执行方式。