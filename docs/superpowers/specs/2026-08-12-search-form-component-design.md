# 搜索表单组件设计方案

## 元数据

- **创建日期**: 2026-08-12
- **作者**: Claude
- **状态**: 设计阶段
- **优先级**: P1（高优先级，提高开发效率）

## 背景

### 问题分析

当前项目中三个页面（菜单管理、用户管理、角色管理）都存在重复的搜索表单代码：

```vue
<!-- 每个页面都有类似的代码 -->
<el-card class="xxx-page__search">
  <el-form :inline="true" :model="query">
    <el-form-item label="名称">
      <el-input v-model="query.name" />
    </el-form-item>
    <el-form-item label="状态">
      <el-select v-model="query.status">
        <el-option label="启用" value="enabled" />
        <el-option label="禁用" value="disabled" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button @click="handleSearch">搜索</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</el-card>
```

**存在的问题：**

1. 代码重复：每个页面都要写 20-30 行相似的代码
2. 维护困难：修改样式或行为需要改多个地方
3. 不一致：各页面的搜索行为略有差异（有的支持回车，有的不支持）
4. 缺少类型安全：没有统一的字段配置类型定义

### 解决方案

创建可复用的搜索表单组件 + Composable，类似项目现有的 BaseTable + useTable 模式。

## 设计方案

### 架构设计

采用**组件 + Composable 分离**模式：

```
src/
├── types/
│   └── search.ts           # 类型定义
├── composables/
│   └── useSearch.ts        # 搜索逻辑 composable
└── components/
    └── common/
        └── BaseSearchForm.vue  # UI 组件
```

**优势：**

1. 职责清晰：UI 和逻辑分离
2. 灵活性高：可单独使用 composable 或组件
3. 符合现有模式：与 BaseTable + useTable 一致
4. 类型安全：完整的 TypeScript 类型定义

### 核心组件

#### 1. 类型定义（SearchField）

支持三种基础字段类型：

```typescript
// src/types/search.ts

/** 基础字段配置 */
export interface BaseSearchField {
  prop: string           // 字段名
  label: string          // 标签（i18n key）
  type: 'input' | 'select' | 'dateRange'
  placeholder?: string   // 占位符（可选）
  clearable?: boolean    // 是否可清空（默认 true）
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

**字段配置示例：**

```typescript
const searchFields: SearchField[] = [
  {
    prop: 'name',
    label: 'basic.menu.searchName',
    type: 'input',
    placeholder: 'common.inputPlaceholder'
  },
  {
    prop: 'status',
    label: 'basic.menu.searchStatus',
    type: 'select',
    options: [
      { label: 'basic.menu.enabled', value: 'enabled' },
      { label: 'basic.menu.disabled', value: 'disabled' }
    ]
  },
  {
    prop: 'createdAt',
    label: '创建时间',
    type: 'dateRange',
    startPlaceholder: '开始时间',
    endPlaceholder: '结束时间'
  }
]
```

#### 2. useSearch Composable

**API 设计：**

```typescript
// src/composables/useSearch.ts

export interface UseSearchOptions {
  defaultModel?: Record<string, unknown>
  onSearch?: (model: Record<string, unknown>) => void
  onReset?: () => void
}

export interface UseSearchReturn {
  searchModel: Record<string, unknown>
  handleSearch: () => void
  handleReset: () => void
  getSearchParams: () => Record<string, unknown>
}

export function useSearch(options: UseSearchOptions = {}): UseSearchReturn
```

**核心功能：**

1. **响应式搜索模型**：自动管理搜索字段状态
2. **智能重置**：重置为默认值或清空所有字段
3. **参数过滤**：自动过滤空值（空字符串、null、undefined）
4. **日期范围处理**：自动拆解为 startTime/endTime

**实现要点：**

```typescript
export function useSearch(options: UseSearchOptions = {}): UseSearchReturn {
  const { defaultModel = {}, onSearch, onReset } = options

  // 响应式搜索模型
  const searchModel = reactive<Record<string, unknown>>({ ...defaultModel })

  // 搜索方法
  const handleSearch = (): void => {
    onSearch?.(getSearchParams())
  }

  // 重置方法
  const handleReset = (): void => {
    Object.assign(searchModel, defaultModel)
    onReset?.()
  }

  // 获取搜索参数（过滤空值）
  const getSearchParams = (): Record<string, unknown> => {
    const params: Record<string, unknown> = {}

    Object.entries(searchModel).forEach(([key, value]) => {
      // 过滤空值
      if (value !== '' && value !== null && value !== undefined) {
        // 日期范围处理
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

  return {
    searchModel,
    handleSearch,
    handleReset,
    getSearchParams
  }
}
```

#### 3. BaseSearchForm 组件

**Props 设计：**

```typescript
interface Props {
  fields: SearchField[]              // 字段配置数组
  model: Record<string, unknown>     // 搜索模型（v-model）
  loading?: boolean                   // 加载状态（可选）
  labelWidth?: string | number       // 标签宽度（默认 auto）
  showButtons?: boolean              // 是否显示按钮（默认 true）
  searchButtonText?: string          // 搜索按钮文字
  resetButtonText?: string           // 重置按钮文字
}

interface Emits {
  'update:model': [value: Record<string, unknown>]
  'search': []
  'reset': []
}
```

**组件结构：**

```vue
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
          {{ t(searchButtonText || 'common.search') }}
        </el-button>
        <el-button @click="handleReset">
          {{ t(resetButtonText || 'common.reset') }}
        </el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>
```

**组件特性：**

1. **自动渲染字段**：根据 fields 配置自动生成表单项
2. **自动触发搜索**：
   - Input: 回车、清空时触发
   - Select: 切换时触发
   - DateRange: 选择时触发
3. **国际化支持**：所有文本都支持 i18n key
4. **灵活布局**：inline 布局，自适应宽度
5. **可选按钮**：可通过 showButtons 控制是否显示按钮

## 使用示例

### 示例 1：菜单管理页面

```vue
<template>
  <div class="menu-page">
    <!-- 搜索表单 -->
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <!-- 工具栏 -->
    <el-card class="menu-page__toolbar">
      <el-button type="primary" @click="handleAdd">
        {{ t('common.add') }}
      </el-button>
    </el-card>

    <!-- 表格 -->
    <el-card class="menu-page__table">
      <el-table :data="filteredTableData" row-key="id">
        <!-- 列定义 -->
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import { useSearch } from '@/composables/useSearch'
import type { SearchField } from '@/types/search'

const { t } = useI18n()

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

const { searchModel, handleSearch, handleReset } = useSearch({
  defaultModel: { name: '', status: '' }
})

const filteredTableData = computed(() => {
  return filterMenuTree(menuTree.value, searchModel)
})
</script>
```

### 示例 2：用户管理页面（与 useTable 集成）

```vue
<template>
  <div class="user-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      :loading="loading"
    />

    <BaseTable
      :data="list"
      :total="total"
      :loading="loading"
      @page-change="handlePageChange"
    >
      <!-- 列定义 -->
    </BaseTable>
  </div>
</template>

<script setup lang="ts">
const searchFields: SearchField[] = [
  { prop: 'userName', label: 'basic.user.userName', type: 'input' },
  { prop: 'realName', label: 'basic.user.realName', type: 'input' },
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

const { searchModel } = useSearch({
  onSearch: () => handleSearch()
})

const { query, list, total, loading, handleSearch, handlePageChange } = useTable(
  getUserList,
  { immediate: true }
)
</script>
```

### 示例 3：角色管理页面

```vue
<script setup lang="ts">
const searchFields: SearchField[] = [
  { prop: 'name', label: 'basic.role.name', type: 'input' },
  { prop: 'code', label: 'basic.role.code', type: 'input' },
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

const { searchModel, handleSearch, handleReset } = useSearch()
</script>
```

## 技术细节

### 日期范围处理

当使用 `dateRange` 类型时，自动将数组拆解为两个字段：

```typescript
// 输入
searchModel.createdAt = ['2026-01-01', '2026-12-31']

// 输出（getSearchParams）
{
  startTime: '2026-01-01',
  endTime: '2026-12-31'
}
```

如果需要其他命名，可以在字段配置中指定：

```typescript
{
  prop: 'createdAt',
  type: 'dateRange',
  // 未来可扩展：
  // startField: 'startTime',
  // endField: 'endTime'
}
```

### 自动触发搜索策略

| 字段类型 | 触发时机 | 说明 |
|---------|---------|------|
| Input | 回车、清空 | 减少不必要的搜索请求 |
| Select | 切换选项 | 即时反馈 |
| DateRange | 选择完成 | 即时反馈 |

如果需要手动控制，可以设置 `showButtons: true` 并禁用自动触发：

```vue
<BaseSearchForm
  :fields="searchFields"
  :model="searchModel"
  :show-buttons="true"
  @search="handleSearch"
/>
```

### 国际化处理

所有文本字段都支持 i18n key：

```typescript
{
  prop: 'name',
  label: 'basic.menu.searchName',  // i18n key
  placeholder: 'common.inputPlaceholder'  // i18n key
}
```

组件内部使用 `useI18n` 的 `t()` 函数进行翻译。

### 类型安全

完整的 TypeScript 类型定义，编译时检查：

```typescript
// ✅ 正确
const fields: SearchField[] = [
  { prop: 'name', label: '名称', type: 'input' }
]

// ❌ 错误：缺少必填字段
const fields: SearchField[] = [
  { type: 'input' }  // Error: 缺少 prop 和 label
]

// ❌ 错误：Select 类型缺少 options
const fields: SearchField[] = [
  { prop: 'status', label: '状态', type: 'select' }  // Error: 缺少 options
]
```

## 实施计划

### 文件结构

```
src/
├── types/
│   └── search.ts                    # 新增：类型定义
├── composables/
│   └── useSearch.ts                 # 新增：搜索逻辑 composable
└── components/
    └── common/
        └── BaseSearchForm.vue       # 新增：搜索表单组件
```

### 实施步骤

1. **创建类型定义**：`src/types/search.ts`
2. **实现 useSearch**：`src/composables/useSearch.ts`
3. **实现 BaseSearchForm**：`src/components/common/BaseSearchForm.vue`
4. **重构菜单管理页面**：应用新组件
5. **重构用户管理页面**：应用新组件
6. **重构角色管理页面**：应用新组件
7. **验证和测试**：确保功能正常

### 预期收益

**代码量减少：**

- 每个页面减少 20-30 行代码
- 三个页面共减少 60-90 行代码

**维护性提升：**

- 修改样式只需改一个文件
- 新增字段类型只需扩展组件
- 统一的搜索行为

**开发效率：**

- 新页面只需配置字段，无需编写重复代码
- 类型安全，减少错误

## 后续扩展

### 可选扩展（不在本次范围内）

1. **更多字段类型**：
   - `treeSelect`: 树形选择
   - `cascader`: 级联选择
   - `numberRange`: 数字范围

2. **高级功能**：
   - 展开/收起（字段过多时）
   - 保存搜索条件
   - 搜索历史记录

3. **自定义插槽**：
   ```vue
   <BaseSearchForm :fields="searchFields">
     <template #custom-field="{ prop }">
       <!-- 自定义字段 -->
     </template>
   </BaseSearchForm>
   ```

## 验收标准

### 功能验收

- ✅ 支持 Input、Select、DateRange 三种字段类型
- ✅ 自动渲染表单项
- ✅ 自动触发搜索
- ✅ 支持国际化
- ✅ 支持手动搜索和重置

### 质量验收

- ✅ TypeScript 类型完整
- ✅ ESLint 检查通过
- ✅ 代码符合项目规范
- ✅ 三个页面重构完成

### 测试验收

- ✅ 菜单管理页面搜索功能正常
- ✅ 用户管理页面搜索功能正常
- ✅ 角色管理页面搜索功能正常

## 参考资料

- 项目现有封装：BaseTable + useTable
- 项目现有封装：useForm
- Element Plus 文档：Form、Input、Select、DatePicker