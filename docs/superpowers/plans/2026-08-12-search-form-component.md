# 搜索表单组件实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 创建可复用的搜索表单组件和 Composable，重构三个管理页面的搜索功能。

**Architecture:** 组件 + Composable 分离模式，类似 BaseTable + useTable。支持 Input、Select、DateRange 三种字段类型，自动渲染表单项，自动触发搜索。

**Tech Stack:** Vue 3 + TypeScript + Element Plus + vue-i18n

---

## 文件结构

**新建文件：**
- `src/types/search.ts` - 类型定义
- `src/composables/useSearch.ts` - 搜索逻辑 composable
- `src/components/common/BaseSearchForm.vue` - 搜索表单组件

**修改文件：**
- `src/views/basic/menu/index.vue` - 重构菜单管理搜索
- `src/views/basic/user/index.vue` - 重构用户管理搜索
- `src/views/basic/role/index.vue` - 重构角色管理搜索

---

## Task 1: 创建类型定义

**Files:**
- Create: `src/types/search.ts`

- [ ] **Step 1: 创建类型定义文件**

```typescript
// src/types/search.ts

/** 基础字段配置 */
export interface BaseSearchField {
  prop: string // 字段名
  label: string // 标签（i18n key）
  type: 'input' | 'select' | 'dateRange'
  placeholder?: string // 占位符（可选，默认使用通用占位符）
  clearable?: boolean // 是否可清空（默认 true）
}

/** Input 类型字段 */
export interface InputSearchField extends BaseSearchField {
  type: 'input'
}

/** Select 类型字段 */
export interface SelectSearchField extends BaseSearchField {
  type: 'select'
  options: Array<{ label: string; value: string }>
}

/** DateRange 类型字段 */
export interface DateRangeSearchField extends BaseSearchField {
  type: 'dateRange'
  startPlaceholder?: string
  endPlaceholder?: string
}

/** 联合类型 */
export type SearchField = InputSearchField | SelectSearchField | DateRangeSearchField
```

- [ ] **Step 2: 验证类型定义**

Run: `pnpm type-check`

Expected: PASS（无错误）

- [ ] **Step 3: 提交类型定义**

```bash
git add src/types/search.ts
git commit -m "feat(admin): 添加搜索字段类型定义

- 支持 Input、Select、DateRange 三种字段类型
- 完整的 TypeScript 类型定义
- 支持国际化 key
"
```

---

## Task 2: 实现 useSearch Composable

**Files:**
- Create: `src/composables/useSearch.ts`

- [ ] **Step 1: 创建 useSearch composable**

```typescript
// src/composables/useSearch.ts
import { reactive } from 'vue'
import type { UnwrapNestedRefs } from 'vue'

export interface UseSearchOptions {
  defaultModel?: Record<string, unknown>
  onSearch?: (model: Record<string, unknown>) => void
  onReset?: () => void
}

export interface UseSearchReturn {
  searchModel: UnwrapNestedRefs<Record<string, unknown>>
  handleSearch: () => void
  handleReset: () => void
  getSearchParams: () => Record<string, unknown>
}

/**
 * 搜索表单 composable
 * @param options 配置项
 */
export function useSearch(options: UseSearchOptions = {}): UseSearchReturn {
  const { defaultModel = {}, onSearch, onReset } = options

  // 响应式搜索模型
  const searchModel = reactive<Record<string, unknown>>({ ...defaultModel })

  /**
   * 获取搜索参数（过滤空值）
   */
  const getSearchParams = (): Record<string, unknown> => {
    const params: Record<string, unknown> = {}

    Object.entries(searchModel).forEach(([key, value]) => {
      // 过滤空值
      if (value !== '' && value !== null && value !== undefined) {
        // 日期范围处理：拆解为 startTime/endTime
        if (Array.isArray(value) && value.length === 2) {
          params['startTime'] = value[0]
          params['endTime'] = value[1]
        } else {
          params[key] = value
        }
      }
    })

    return params
  }

  /**
   * 搜索方法
   */
  const handleSearch = (): void => {
    onSearch?.(getSearchParams())
  }

  /**
   * 重置方法
   */
  const handleReset = (): void => {
    Object.assign(searchModel, defaultModel)
    onReset?.()
  }

  return {
    searchModel,
    handleSearch,
    handleReset,
    getSearchParams
  }
}
```

- [ ] **Step 2: 验证类型检查**

Run: `pnpm type-check`

Expected: PASS（无错误）

- [ ] **Step 3: 提交 useSearch**

```bash
git add src/composables/useSearch.ts
git commit -m "feat(admin): 实现 useSearch composable

- 响应式搜索模型管理
- 自动过滤空值参数
- 日期范围自动拆解
- 支持搜索和重置回调
"
```

---

## Task 3: 实现 BaseSearchForm 组件

**Files:**
- Create: `src/components/common/BaseSearchForm.vue`

- [ ] **Step 1: 创建 BaseSearchForm 组件**

```vue
<!-- src/components/common/BaseSearchForm.vue -->
<template>
  <el-card class="base-search-form">
    <el-form :model="localModel" :inline="true" :label-width="labelWidth">
      <!-- 动态渲染字段 -->
      <el-form-item
        v-for="field in fields"
        :key="field.prop"
        :label="t(field.label)"
      >
        <!-- Input 类型 -->
        <el-input
          v-if="field.type === 'input'"
          v-model="localModel[field.prop]"
          :placeholder="t(field.placeholder || 'common.inputPlaceholder')"
          :clearable="field.clearable !== false"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />

        <!-- Select 类型 -->
        <el-select
          v-else-if="field.type === 'select'"
          v-model="localModel[field.prop]"
          :placeholder="t(field.placeholder || 'common.selectPlaceholder')"
          :clearable="field.clearable !== false"
          @change="handleSearch"
        >
          <el-option
            v-for="option in field.options"
            :key="option.value"
            :label="t(option.label)"
            :value="option.value"
          />
        </el-select>

        <!-- DateRange 类型 -->
        <el-date-picker
          v-else-if="field.type === 'dateRange'"
          v-model="localModel[field.prop]"
          type="daterange"
          :start-placeholder="t(field.startPlaceholder || 'common.startDate')"
          :end-placeholder="t(field.endPlaceholder || 'common.endDate')"
          value-format="YYYY-MM-DD"
          @change="handleSearch"
        />
      </el-form-item>

      <!-- 操作按钮 -->
      <el-form-item v-if="showButtons">
        <el-button type="primary" :loading="loading" @click="handleSearch">
          {{ t(searchButtonText) }}
        </el-button>
        <el-button @click="handleReset">
          {{ t(resetButtonText) }}
        </el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { SearchField } from '@/types/search'

interface Props {
  fields: SearchField[]
  model: Record<string, unknown>
  loading?: boolean
  labelWidth?: string | number
  showButtons?: boolean
  searchButtonText?: string
  resetButtonText?: string
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  labelWidth: 'auto',
  showButtons: true,
  searchButtonText: 'common.search',
  resetButtonText: 'common.reset'
})

const emit = defineEmits<{
  'update:model': [value: Record<string, unknown>]
  search: []
  reset: []
}>()

const { t } = useI18n()

// 本地模型（用于双向绑定）
const localModel = computed({
  get: () => props.model,
  set: (value) => emit('update:model', value)
})

// 搜索方法
const handleSearch = (): void => {
  emit('search')
}

// 重置方法
const handleReset = (): void => {
  emit('reset')
}
</script>

<style scoped lang="scss">
.base-search-form {
  margin-bottom: $spacing-md;
}
</style>
```

- [ ] **Step 2: 验证类型和 ESLint**

Run: `pnpm type-check && pnpm lint`

Expected: PASS（无错误）

- [ ] **Step 3: 提交 BaseSearchForm 组件**

```bash
git add src/components/common/BaseSearchForm.vue
git commit -m "feat(admin): 实现 BaseSearchForm 组件

- 支持动态渲染 Input、Select、DateRange 字段
- 自动触发搜索（回车、清空、切换）
- 支持国际化
- 可选的搜索/重置按钮
"
```

---

## Task 4: 添加国际化翻译

**Files:**
- Modify: `src/i18n/zh-CN/common.json`
- Modify: `src/i18n/en-US/common.json`

- [ ] **Step 1: 添加中文翻译**

```json
{
  "search": "搜索",
  "reset": "重置",
  "inputPlaceholder": "请输入",
  "selectPlaceholder": "请选择",
  "startDate": "开始日期",
  "endDate": "结束日期"
}
```

- [ ] **Step 2: 添加英文翻译**

```json
{
  "search": "Search",
  "reset": "Reset",
  "inputPlaceholder": "Please input",
  "selectPlaceholder": "Please select",
  "startDate": "Start Date",
  "endDate": "End Date"
}
```

- [ ] **Step 3: 提交国际化**

```bash
git add src/i18n/zh-CN/common.json src/i18n/en-US/common.json
git commit -m "feat(i18n): 添加搜索表单通用翻译"
```

---

## Task 5: 重构菜单管理页面

**Files:**
- Modify: `src/views/basic/menu/index.vue`

- [ ] **Step 1: 导入新组件和类型**

```typescript
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import { useSearch } from '@/composables/useSearch'
import type { SearchField } from '@/types/search'
```

- [ ] **Step 2: 定义搜索字段配置**

```typescript
// 搜索字段配置
const searchFields: SearchField[] = [
  {
    prop: 'name',
    label: 'basic.menu.searchName',
    type: 'input'
  },
  {
    prop: 'status',
    label: 'basic.menu.searchStatus',
    type: 'select',
    options: [
      { label: 'basic.menu.enabled', value: 'enabled' },
      { label: 'basic.menu.disabled', value: 'disabled' }
    ]
  }
]
```

- [ ] **Step 3: 使用 useSearch**

```typescript
// 搜索逻辑
const { searchModel, handleSearch, handleReset } = useSearch({
  defaultModel: {
    name: '',
    status: ''
  }
})
```

- [ ] **Step 4: 替换搜索表单模板**

```vue
<!-- 搜索表单 -->
<BaseSearchForm
  :fields="searchFields"
  :model="searchModel"
  @search="handleSearch"
  @reset="handleReset"
/>
```

- [ ] **Step 5: 调整过滤逻辑**

```typescript
// 过滤后的表格数据
const filteredTableData = computed(() => {
  return filterMenuTree(menuTree.value, searchModel)
})
```

- [ ] **Step 6: 验证功能**

启动开发服务器，手动测试：
1. 搜索框输入菜单名称
2. 选择状态筛选
3. 点击搜索和重置按钮
4. 验证数据过滤正确

- [ ] **Step 7: 验证类型和 ESLint**

Run: `pnpm type-check && pnpm lint`

Expected: PASS（无错误）

- [ ] **Step 8: 提交菜单管理重构**

```bash
git add src/views/basic/menu/index.vue
git commit -m "refactor(admin): 重构菜单管理页面搜索功能

- 使用 BaseSearchForm 组件替代手动表单
- 使用 useSearch composable 管理搜索状态
- 减少 20+ 行重复代码
"
```

---

## Task 6: 重构用户管理页面

**Files:**
- Modify: `src/views/basic/user/index.vue`

- [ ] **Step 1: 导入新组件和类型**

```typescript
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import { useSearch } from '@/composables/useSearch'
import type { SearchField } from '@/types/search'
```

- [ ] **Step 2: 定义搜索字段配置**

```typescript
const searchFields: SearchField[] = [
  {
    prop: 'userName',
    label: 'basic.user.userName',
    type: 'input'
  },
  {
    prop: 'realName',
    label: 'basic.user.realName',
    type: 'input'
  },
  {
    prop: 'status',
    label: 'basic.user.status',
    type: 'select',
    options: [
      { label: 'basic.menu.enabled', value: 'enabled' },
      { label: 'basic.menu.disabled', value: 'disabled' }
    ]
  }
]
```

- [ ] **Step 3: 使用 useSearch**

```typescript
const { searchModel, handleSearch, handleReset } = useSearch({
  defaultModel: {
    userName: '',
    realName: '',
    status: ''
  }
})
```

- [ ] **Step 4: 替换搜索表单模板**

```vue
<!-- 搜索栏 -->
<BaseSearchForm
  :fields="searchFields"
  :model="searchModel"
  :loading="loading"
  @search="handleSearch"
  @reset="handleReset"
/>
```

- [ ] **Step 5: 调整 useTable 集成**

```typescript
const { query, list, total, loading, handleSearch: handleTableSearch } = useTable(
  getUserList,
  { immediate: true }
)

// 搜索时同步到 query
const handleSearch = (): void => {
  Object.assign(query, searchModel)
  handleTableSearch()
}
```

- [ ] **Step 6: 验证功能**

手动测试用户管理搜索功能。

- [ ] **Step 7: 验证类型和 ESLint**

Run: `pnpm type-check && pnpm lint`

Expected: PASS（无错误）

- [ ] **Step 8: 提交用户管理重构**

```bash
git add src/views/basic/user/index.vue
git commit -m "refactor(admin): 重构用户管理页面搜索功能

- 使用 BaseSearchForm 组件
- 集成 useTable 和 useSearch
- 减少 30+ 行重复代码
"
```

---

## Task 7: 重构角色管理页面

**Files:**
- Modify: `src/views/basic/role/index.vue`

- [ ] **Step 1: 导入新组件和类型**

```typescript
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import { useSearch } from '@/composables/useSearch'
import type { SearchField } from '@/types/search'
```

- [ ] **Step 2: 定义搜索字段配置**

```typescript
const searchFields: SearchField[] = [
  {
    prop: 'name',
    label: 'basic.role.name',
    type: 'input'
  },
  {
    prop: 'code',
    label: 'basic.role.code',
    type: 'input'
  },
  {
    prop: 'status',
    label: 'basic.role.status',
    type: 'select',
    options: [
      { label: 'basic.menu.enabled', value: 'enabled' },
      { label: 'basic.menu.disabled', value: 'disabled' }
    ]
  }
]
```

- [ ] **Step 3: 使用 useSearch**

```typescript
const { searchModel, handleSearch, handleReset } = useSearch({
  defaultModel: {
    name: '',
    code: '',
    status: ''
  }
})
```

- [ ] **Step 4: 替换搜索表单模板**

```vue
<!-- 搜索栏 -->
<BaseSearchForm
  :fields="searchFields"
  :model="searchModel"
  :loading="loading"
/>
```

- [ ] **Step 5: 验证功能**

手动测试角色管理搜索功能。

- [ ] **Step 6: 验证类型和 ESLint**

Run: `pnpm type-check && pnpm lint`

Expected: PASS（无错误）

- [ ] **Step 7: 提交角色管理重构**

```bash
git add src/views/basic/role/index.vue
git commit -m "refactor(admin): 重构角色管理页面搜索功能

- 使用 BaseSearchForm 组件
- 减少 25+ 行重复代码
"
```

---

## Task 8: 最终验证和提交

- [ ] **Step 1: 运行完整验证**

Run: `pnpm type-check && pnpm lint`

Expected: PASS（无错误）

- [ ] **Step 2: 推送到远程仓库**

```bash
git push origin main
```

- [ ] **Step 3: 验收测试**

在浏览器中测试三个页面：
1. 菜单管理：搜索功能正常
2. 用户管理：搜索功能正常
3. 角色管理：搜索功能正常

---

## 验收清单

- [ ] 类型定义完整（Input、Select、DateRange）
- [ ] useSearch composable 实现完整
- [ ] BaseSearchForm 组件实现完整
- [ ] 三个页面重构完成
- [ ] TypeScript 类型检查通过
- [ ] ESLint 检查通过
- [ ] 功能测试通过（三个页面搜索正常）
- [ ] Git 提交清晰（每个功能一个提交）

---

## 预期收益

**代码量：**
- 每个页面减少 20-30 行代码
- 三个页面共减少 60-90 行代码

**维护性：**
- 修改样式只需改一个文件
- 新增字段类型只需扩展组件
- 统一的搜索行为

**开发效率：**
- 新页面只需配置字段，无需编写重复代码
- 类型安全，减少错误