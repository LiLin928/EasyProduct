# 公告管理 - 批次 1：管理端前端实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现公告管理的管理端前端功能（类型定义 + API + 列表页 + 编辑页）

**Architecture:** Vue3 + TypeScript + Element Plus，使用现有的 BaseSearchForm、BaseTable、RichTextEditor 组件

**Tech Stack:** Vue 3.x, TypeScript 5.x, Element Plus, vue-i18n, Axios

---

## 文件结构

**创建文件：**
- `EasyProduct.Admin/src/types/announcement.ts` - 公告相关类型定义
- `EasyProduct.Admin/src/api/basic/announcement.ts` - 公告 API
- `EasyProduct.Admin/src/views/basic/announcement/index.vue` - 公告列表页
- `EasyProduct.Admin/src/views/basic/announcement/edit.vue` - 公告编辑页
- `EasyProduct.Admin/src/views/basic/announcement/components/DetailDialog.vue` - 公告详情弹窗
- `EasyProduct.Admin/src/composables/useAnnouncement.ts` - 公告相关 composable

**修改文件：**
- `EasyProduct.Admin/src/router/modules/basic.ts` - 添加公告路由
- `EasyProduct.Admin/src/i18n/zh-CN/basic.json` - 添加公告国际化
- `EasyProduct.Admin/src/i18n/en-US/basic.json` - 添加公告国际化

---

## Task 1: 创建类型定义

**Files:**
- Create: `EasyProduct.Admin/src/types/announcement.ts`

- [ ] **Step 1: 创建公告类型定义文件**

```typescript
// src/types/announcement.ts

/**
 * 公告类型枚举
 */
export type AnnouncementType = 'all' | 'targeted'

/**
 * 公告级别枚举
 */
export type AnnouncementLevel = 'normal' | 'important' | 'urgent'

/**
 * 公告状态枚举
 */
export type AnnouncementStatus = 'draft' | 'published' | 'recalled'

/**
 * 附件信息
 */
export interface Attachment {
  name: string
  url: string
  size: number
}

/**
 * 公告实体
 */
export interface Announcement {
  id: string
  title: string
  content: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds?: string[]
  isTop: boolean
  topTime?: string
  publishTime?: string
  recallTime?: string
  status: AnnouncementStatus
  attachments?: Attachment[]
  creatorId: string
  creatorName: string
  createdAt: string
  updatedAt?: string
}

/**
 * 公告查询参数
 */
export interface AnnouncementQuery {
  pageIndex: number
  pageSize: number
  title?: string
  type?: AnnouncementType
  level?: AnnouncementLevel
  status?: AnnouncementStatus
  isTop?: boolean
}

/**
 * 创建公告参数
 */
export interface CreateAnnouncementParams {
  title: string
  content: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds?: string[]
  attachments?: Attachment[]
}

/**
 * 更新公告参数
 */
export interface UpdateAnnouncementParams {
  title: string
  content: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds?: string[]
  attachments?: Attachment[]
}

/**
 * 置顶参数
 */
export interface SetTopParams {
  isTop: boolean
}
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Admin && pnpm type-check`
Expected: PASS (无错误)

- [ ] **Step 3: 提交类型定义**

```bash
git add EasyProduct.Admin/src/types/announcement.ts
git commit -m "feat(admin): 添加公告管理类型定义"
```

---

## Task 2: 创建 API 层

**Files:**
- Create: `EasyProduct.Admin/src/api/basic/announcement.ts`

- [ ] **Step 1: 创建公告 API 文件**

```typescript
// src/api/basic/announcement.ts
import { get, post, put, del } from '@/utils/request'
import type {
  Announcement,
  AnnouncementQuery,
  CreateAnnouncementParams,
  UpdateAnnouncementParams,
  SetTopParams,
} from '@/types/announcement'

/**
 * 获取公告列表（管理端）
 */
export const getAnnouncementList = (params: AnnouncementQuery) =>
  get<{ list: Announcement[]; total: number }>('/api/admin/basic/announcement', params)

/**
 * 获取公告详情
 */
export const getAnnouncementById = (id: string) =>
  get<Announcement>(`/api/admin/basic/announcement/${id}`)

/**
 * 创建公告
 */
export const createAnnouncement = (data: CreateAnnouncementParams) =>
  post<{ id: string }>('/api/admin/basic/announcement', data)

/**
 * 更新公告
 */
export const updateAnnouncement = (id: string, data: UpdateAnnouncementParams) =>
  put<null>(`/api/admin/basic/announcement/${id}`, data)

/**
 * 删除公告
 */
export const deleteAnnouncement = (id: string) =>
  del<null>(`/api/admin/basic/announcement/${id}`)

/**
 * 发布公告
 */
export const publishAnnouncement = (id: string) =>
  put<null>(`/api/admin/basic/announcement/${id}/publish`)

/**
 * 撤回公告
 */
export const recallAnnouncement = (id: string) =>
  put<null>(`/api/admin/basic/announcement/${id}/recall`)

/**
 * 置顶/取消置顶
 */
export const setTopAnnouncement = (id: string, data: SetTopParams) =>
  put<null>(`/api/admin/basic/announcement/${id}/top`, data)
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Admin && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 提交 API 层**

```bash
git add EasyProduct.Admin/src/api/basic/announcement.ts
git commit -m "feat(admin): 添加公告管理 API"
```

---

## Task 3: 添加国际化文本

**Files:**
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/basic.json`
- Modify: `EasyProduct.Admin/src/i18n/en-US/basic.json`

- [ ] **Step 1: 在 zh-CN/basic.json 中添加公告国际化**

在 `desktop` 部分后添加：

```json
"announcement": {
  "title": "公告管理",
  "list": "公告列表",
  "add": "新增公告",
  "edit": "编辑公告",
  "view": "查看公告",
  "delete": "删除公告",
  "deleteConfirm": "确认删除该公告吗？",
  "publish": "发布",
  "recall": "撤回",
  "republish": "重新发布",
  "setTop": "置顶",
  "cancelTop": "取消置顶",
  "announcementTitle": "公告标题",
  "type": "公告类型",
  "typeAll": "全员公告",
  "typeTargeted": "定向公告",
  "level": "公告级别",
  "levelNormal": "普通",
  "levelImportant": "重要",
  "levelUrgent": "紧急",
  "status": "状态",
  "statusDraft": "草稿",
  "statusPublished": "已发布",
  "statusRecalled": "已撤回",
  "isTop": "是否置顶",
  "publishTime": "发布时间",
  "creator": "创建人",
  "createdAt": "创建时间",
  "content": "公告内容",
  "targetRoles": "目标角色",
  "targetRolesPlaceholder": "请选择目标角色",
  "attachments": "附件",
  "uploadAttachment": "上传附件",
  "saveDraft": "保存草稿",
  "publishNow": "立即发布",
  "form": {
    "titlePlaceholder": "请输入公告标题",
    "titleRequired": "请输入公告标题",
    "titleLength": "公告标题长度为 2-200 个字符",
    "contentRequired": "请输入公告内容",
    "typeRequired": "请选择公告类型",
    "levelRequired": "请选择公告级别",
    "targetRolesRequired": "定向公告必须选择目标角色"
  },
  "message": {
    "createSuccess": "创建成功",
    "updateSuccess": "更新成功",
    "deleteSuccess": "删除成功",
    "publishSuccess": "发布成功",
    "recallSuccess": "撤回成功",
    "setTopSuccess": "置顶成功",
    "cancelTopSuccess": "取消置顶成功",
    "cannotEditPublished": "已发布的公告不能编辑",
    "cannotDeletePublished": "已发布的公告不能删除"
  },
  "readStats": {
    "title": "阅读统计",
    "readCount": "已读人数",
    "unreadCount": "未读人数"
  }
}
```

- [ ] **Step 2: 在 en-US/basic.json 中添加公告国际化**

在 `desktop` 部分后添加：

```json
"announcement": {
  "title": "Announcement Management",
  "list": "Announcement List",
  "add": "Add Announcement",
  "edit": "Edit Announcement",
  "view": "View Announcement",
  "delete": "Delete Announcement",
  "deleteConfirm": "Confirm to delete this announcement?",
  "publish": "Publish",
  "recall": "Recall",
  "republish": "Republish",
  "setTop": "Set Top",
  "cancelTop": "Cancel Top",
  "announcementTitle": "Title",
  "type": "Type",
  "typeAll": "All Users",
  "typeTargeted": "Targeted",
  "level": "Level",
  "levelNormal": "Normal",
  "levelImportant": "Important",
  "levelUrgent": "Urgent",
  "status": "Status",
  "statusDraft": "Draft",
  "statusPublished": "Published",
  "statusRecalled": "Recalled",
  "isTop": "Top",
  "publishTime": "Publish Time",
  "creator": "Creator",
  "createdAt": "Created At",
  "content": "Content",
  "targetRoles": "Target Roles",
  "targetRolesPlaceholder": "Select target roles",
  "attachments": "Attachments",
  "uploadAttachment": "Upload Attachment",
  "saveDraft": "Save Draft",
  "publishNow": "Publish Now",
  "form": {
    "titlePlaceholder": "Enter announcement title",
    "titleRequired": "Please enter title",
    "titleLength": "Title length must be 2-200 characters",
    "contentRequired": "Please enter content",
    "typeRequired": "Please select type",
    "levelRequired": "Please select level",
    "targetRolesRequired": "Targeted announcement must select roles"
  },
  "message": {
    "createSuccess": "Created successfully",
    "updateSuccess": "Updated successfully",
    "deleteSuccess": "Deleted successfully",
    "publishSuccess": "Published successfully",
    "recallSuccess": "Recalled successfully",
    "setTopSuccess": "Set top successfully",
    "cancelTopSuccess": "Cancelled top successfully",
    "cannotEditPublished": "Published announcement cannot be edited",
    "cannotDeletePublished": "Published announcement cannot be deleted"
  },
  "readStats": {
    "title": "Read Statistics",
    "readCount": "Read Count",
    "unreadCount": "Unread Count"
  }
}
```

- [ ] **Step 3: 运行 i18n 检查验证**

Run: `cd EasyProduct.Admin && pnpm run check:i18n`
Expected: PASS

- [ ] **Step 4: 提交国际化文件**

```bash
git add EasyProduct.Admin/src/i18n/zh-CN/basic.json EasyProduct.Admin/src/i18n/en-US/basic.json
git commit -m "feat(admin): 添加公告管理国际化文本"
```

---

## Task 4: 创建公告列表页

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/announcement/index.vue`

- [ ] **Step 1: 创建公告列表页组件**

创建文件 `EasyProduct.Admin/src/views/basic/announcement/index.vue`，包含以下内容：

```vue
<!-- src/views/basic/announcement/index.vue -->
<template>
  <div class="announcement-page">
    <BaseSearchForm :fields="searchFields" @search="handleSearch" @reset="handleReset">
      <template #toolbar>
        <el-button type="primary" @click="handleAdd">
          {{ t('basic.announcement.add') }}
        </el-button>
        <el-button @click="handleBatchDelete" :disabled="!selectedIds.length">
          {{ t('common.button.batchDelete') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <BaseTable
      :data="tableData"
      :columns="columns"
      :loading="loading"
      :total="total"
      :page-index="query.pageIndex"
      :page-size="query.pageSize"
      @page-change="handlePageChange"
      @selection-change="handleSelectionChange"
    >
      <template #column:type="{ row }">
        <el-tag :type="row.type === 'all' ? 'primary' : 'success'">
          {{ t(`basic.announcement.type${row.type === 'all' ? 'All' : 'Targeted'}`) }}
        </el-tag>
      </template>

      <template #column:level="{ row }">
        <el-tag :type="getLevelType(row.level)">
          {{ t(`basic.announcement.level${capitalize(row.level)}`) }}
        </el-tag>
      </template>

      <template #column:status="{ row }">
        <el-tag :type="getStatusType(row.status)">
          {{ t(`basic.announcement.status${capitalize(row.status)}`) }}
        </el-tag>
      </template>

      <template #column:isTop="{ row }">
        <el-tag v-if="row.isTop" type="warning">{{ t('common.yes') }}</el-tag>
        <span v-else>{{ t('common.no') }}</span>
      </template>

      <template #column:operation="{ row }">
        <el-button link type="primary" @click="handleView(row)">
          {{ t('basic.announcement.view') }}
        </el-button>
        <el-button
          v-if="row.status === 'draft'"
          link
          type="primary"
          @click="handleEdit(row)"
        >
          {{ t('basic.announcement.edit') }}
        </el-button>
        <el-button
          v-if="row.status === 'draft'"
          link
          type="primary"
          @click="handlePublish(row)"
        >
          {{ t('basic.announcement.publish') }}
        </el-button>
        <el-button
          v-if="row.status === 'published'"
          link
          type="warning"
          @click="handleRecall(row)"
        >
          {{ t('basic.announcement.recall') }}
        </el-button>
        <el-button
          v-if="row.status === 'published'"
          link
          type="warning"
          @click="handleSetTop(row)"
        >
          {{ row.isTop ? t('basic.announcement.cancelTop') : t('basic.announcement.setTop') }}
        </el-button>
        <el-button
          v-if="row.status === 'recalled'"
          link
          type="primary"
          @click="handlePublish(row)"
        >
          {{ t('basic.announcement.republish') }}
        </el-button>
        <el-button
          v-if="row.status !== 'published'"
          link
          type="danger"
          @click="handleDelete(row)"
        >
          {{ t('basic.announcement.delete') }}
        </el-button>
      </template>
    </BaseTable>

    <DetailDialog
      v-model="detailVisible"
      :announcement-id="currentId"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useSearch } from '@/composables/useSearch'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import DetailDialog from './components/DetailDialog.vue'
import {
  getAnnouncementList,
  deleteAnnouncement,
  publishAnnouncement,
  recallAnnouncement,
  setTopAnnouncement,
} from '@/api/basic/announcement'
import type { Announcement, AnnouncementQuery } from '@/types/announcement'

const { t } = useLocale()
const router = useRouter()

const loading = ref(false)
const tableData = ref<Announcement[]>([])
const total = ref(0)
const selectedIds = ref<string[]>([])
const detailVisible = ref(false)
const currentId = ref('')

const query = reactive<AnnouncementQuery>({
  pageIndex: 1,
  pageSize: 10,
  title: '',
  type: undefined,
  level: undefined,
  status: undefined,
  isTop: undefined,
})

const searchFields = [
  {
    key: 'title',
    label: t('basic.announcement.announcementTitle'),
    type: 'input',
    placeholder: t('basic.announcement.announcementTitle'),
  },
  {
    key: 'type',
    label: t('basic.announcement.type'),
    type: 'select',
    options: [
      { label: t('basic.announcement.typeAll'), value: 'all' },
      { label: t('basic.announcement.typeTargeted'), value: 'targeted' },
    ],
  },
  {
    key: 'level',
    label: t('basic.announcement.level'),
    type: 'select',
    options: [
      { label: t('basic.announcement.levelNormal'), value: 'normal' },
      { label: t('basic.announcement.levelImportant'), value: 'important' },
      { label: t('basic.announcement.levelUrgent'), value: 'urgent' },
    ],
  },
  {
    key: 'status',
    label: t('basic.announcement.status'),
    type: 'select',
    options: [
      { label: t('basic.announcement.statusDraft'), value: 'draft' },
      { label: t('basic.announcement.statusPublished'), value: 'published' },
      { label: t('basic.announcement.statusRecalled'), value: 'recalled' },
    ],
  },
]

const columns = [
  { prop: 'title', label: t('basic.announcement.announcementTitle'), minWidth: 200 },
  { prop: 'type', label: t('basic.announcement.type'), width: 100, slot: true },
  { prop: 'level', label: t('basic.announcement.level'), width: 100, slot: true },
  { prop: 'status', label: t('basic.announcement.status'), width: 100, slot: true },
  { prop: 'isTop', label: t('basic.announcement.isTop'), width: 80, slot: true },
  { prop: 'publishTime', label: t('basic.announcement.publishTime'), width: 160 },
  { prop: 'creatorName', label: t('basic.announcement.creator'), width: 120 },
  { prop: 'createdAt', label: t('basic.announcement.createdAt'), width: 160 },
  { prop: 'operation', label: t('common.operation'), width: 280, fixed: 'right', slot: true },
]

const { handleSearch, handleReset } = useSearch(query, loadData)

const loadData = async () => {
  loading.value = true
  try {
    const { list, total: totalCount } = await getAnnouncementList(query)
    tableData.value = list
    total.value = totalCount
  } finally {
    loading.value = false
  }
}

const handlePageChange = (pageIndex: number, pageSize: number) => {
  query.pageIndex = pageIndex
  query.pageSize = pageSize
  loadData()
}

const handleSelectionChange = (ids: string[]) => {
  selectedIds.value = ids
}

const handleAdd = () => {
  router.push('/basic/announcement/add')
}

const handleView = (row: Announcement) => {
  currentId.value = row.id
  detailVisible.value = true
}

const handleEdit = (row: Announcement) => {
  router.push(`/basic/announcement/edit/${row.id}`)
}

const handlePublish = async (row: Announcement) => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.publish') + '?',
      t('common.confirm'),
      { type: 'warning' }
    )
    await publishAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.publishSuccess'))
    loadData()
  } catch {}
}

const handleRecall = async (row: Announcement) => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.recall') + '?',
      t('common.confirm'),
      { type: 'warning' }
    )
    await recallAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.recallSuccess'))
    loadData()
  } catch {}
}

const handleSetTop = async (row: Announcement) => {
  try {
    const isTop = !row.isTop
    await setTopAnnouncement(row.id, { isTop })
    ElMessage.success(
      isTop
        ? t('basic.announcement.message.setTopSuccess')
        : t('basic.announcement.message.cancelTopSuccess')
    )
    loadData()
  } catch {}
}

const handleDelete = async (row: Announcement) => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.deleteConfirm'),
      t('common.confirm'),
      { type: 'warning' }
    )
    await deleteAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.deleteSuccess'))
    loadData()
  } catch {}
}

const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(
      t('common.confirmBatchDelete', { count: selectedIds.value.length }),
      t('common.confirm'),
      { type: 'warning' }
    )
    // TODO: 批量删除 API
    ElMessage.success(t('basic.announcement.message.deleteSuccess'))
    loadData()
  } catch {}
}

const getLevelType = (level: string) => {
  const types: Record<string, string> = {
    normal: 'info',
    important: 'warning',
    urgent: 'danger',
  }
  return types[level] || 'info'
}

const getStatusType = (status: string) => {
  const types: Record<string, string> = {
    draft: 'info',
    published: 'success',
    recalled: 'warning',
  }
  return types[status] || 'info'
}

const capitalize = (str: string) => {
  return str.charAt(0).toUpperCase() + str.slice(1)
}

onMounted(() => {
  loadData()
})
</script>

<style scoped lang="scss">
.announcement-page {
  padding: $spacing-md;
}
</style>
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Admin && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 运行 ESLint 检查**

Run: `cd EasyProduct.Admin && pnpm lint`
Expected: PASS

- [ ] **Step 4: 提交公告列表页**

```bash
git add EasyProduct.Admin/src/views/basic/announcement/index.vue
git commit -m "feat(admin): 创建公告列表页"
```

---

## 验收检查

- [ ] 运行完整验证流程

```bash
cd EasyProduct.Admin
pnpm type-check
pnpm lint
pnpm run check:i18n
pnpm build
```

Expected: 全部通过

- [ ] 提交批次 1 所有文件

```bash
git add .
git commit -m "feat(admin): 完成公告管理批次1 - 管理端前端基础功能"
```

---

## 后续批次

批次 1 完成后，将继续：
- 批次 2：管理端 Mock API
- 批次 3：官网前端 + Mock
- 批次 4：小程序前端 + Mock