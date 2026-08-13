# 搜索表单组件优化实施计划

> **Goal:** 优化 BaseSearchForm 布局，修复重置功能，改善用户体验

**问题：**
1. 下拉框太窄
2. 新增按钮位置不合理（应在搜索表单上方/合并）
3. 搜索/重置按钮应在右侧
4. 重置功能不可用（未清空 searchModel）

---

## Task 1: 优化 useSearch Composable

**Files:**
- Modify: `src/composables/useSearch.ts`

**Changes:**
1. 添加 `resetModel` 方法到接口和实现
2. 提供清空 searchModel 的能力

**Expected Code:**
```typescript
export interface UseSearchReturn<T = Record<string, unknown>> {
  searchModel: UnwrapNestedRefs<T>
  resetModel: () => void  // 新增
  handleSearch: () => void
  handleReset: () => void
  getSearchParams: () => Partial<T>
}

// 实现中添加
const resetModel = (): void => {
  Object.keys(searchModel).forEach(key => {
    searchModel[key] = defaultModel[key] ?? ''
  })
}

return {
  searchModel,
  resetModel,  // 新增
  handleSearch,
  handleReset,
  getSearchParams
}
```

---

## Task 2: 优化 BaseSearchForm 组件布局

**Files:**
- Modify: `src/components/common/BaseSearchForm.vue`

**Changes:**
1. 添加 toolbar 插槽（用于放置新增等按钮）
2. 优化布局：toolbar 在上方，搜索字段和按钮在下方
3. 添加样式：下拉框宽度、按钮右对齐
4. 支持隐藏按钮（showButtons）

**Expected Layout:**
```
┌──────────────────────────────────────────┐
│ [新增按钮] （toolbar 插槽）              │
├──────────────────────────────────────────┤
│ 字段1 字段2 字段3    [搜索] [重置]      │ ← 按钮右对齐
└──────────────────────────────────────────┘
```

**Expected Template:**
```vue
<el-card class="base-search-form">
  <!-- 工具栏插槽 -->
  <div v-if="$slots.toolbar" class="base-search-form__toolbar">
    <slot name="toolbar" />
  </div>

  <el-form :model="localModel" :inline="true" :label-width="labelWidth">
    <!-- 搜索字段 -->
    <el-form-item v-for="field in fields">
      <!-- Input/Select/DateRange -->
    </el-form-item>

    <!-- 操作按钮（右对齐） -->
    <el-form-item v-if="showButtons" class="base-search-form__actions">
      <el-button type="primary" @click="handleSearch">
        {{ t(searchButtonText) }}
      </el-button>
      <el-button @click="handleReset">
        {{ t(resetButtonText) }}
      </el-button>
    </el-form-item>
  </el-form>
</el-card>
```

**Expected Styles:**
```scss
.base-search-form {
  margin-bottom: $spacing-md;

  &__toolbar {
    margin-bottom: $spacing-md;
    padding-bottom: $spacing-md;
    border-bottom: 1px solid #ebeef5;
  }

  &__actions {
    margin-left: auto; // 推到右侧
  }

  // 下拉框最小宽度
  .el-select {
    min-width: 200px;
  }

  // 日期选择器最小宽度
  .el-date-editor--daterange {
    min-width: 240px;
  }
}
```

---

## Task 3: 重构用户管理页面

**Files:**
- Modify: `src/views/basic/user/index.vue`

**Changes:**
1. 使用 toolbar 插槽放置新增按钮
2. 删除独立的工具栏卡片
3. 使用 resetModel 方法修复重置功能

**Expected Template:**
```vue
<BaseSearchForm
  :fields="searchFields"
  :model="searchModel"
  @search="handleSearch"
  @reset="handleReset"
>
  <template #toolbar>
    <el-button
      v-permission="['basic:user:edit']"
      type="primary"
      @click="handleAdd"
    >
      {{ t('common.add') }}
    </el-button>
  </template>
</BaseSearchForm>

<!-- 删除工具栏卡片 -->
<!-- <el-card class="user-page__toolbar">...</el-card> -->
```

**Expected Script:**
```typescript
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: {
    userName: '',
    realName: '',
    status: ''
  }
})

// 重置方法
const handleReset = (): void => {
  resetModel()  // 清空 UI
  delete query.userName
  delete query.realName
  delete query.status
  tableReset()
}
```

---

## Task 4: 重构菜单管理页面

**Files:**
- Modify: `src/views/basic/menu/index.vue`

**Changes:**
1. 使用 toolbar 插槽放置新增、展开、折叠按钮
2. 删除独立的工具栏卡片
3. 使用 resetModel 方法修复重置功能（如果有重置逻辑）

---

## Task 5: 重构角色管理页面

**Files:**
- Modify: `src/views/basic/role/index.vue`

**Changes:**
1. 使用 toolbar 插槽放置新增按钮
2. 删除独立的工具栏卡片
3. 使用 resetModel 方法修复重置功能

---

## Task 6: 最终验证

**Steps:**
1. 运行 `pnpm type-check` 验证类型
2. 运行 `pnpm lint` 验证代码风格
3. 提交所有更改
4. 推送到远程仓库

---

## 预期效果

**布局改进：**
- ✅ 新增按钮在搜索表单上方（通过 toolbar 插槽）
- ✅ 搜索/重置按钮在右侧（通过 `margin-left: auto`）
- ✅ 下拉框宽度充足（min-width: 200px）
- ✅ 页面更紧凑（减少一个工具栏卡片）

**功能改进：**
- ✅ 重置按钮正常工作（清空 UI 和数据）
- ✅ 提供独立的 resetModel 方法
- ✅ 布局符合常见后台管理模式