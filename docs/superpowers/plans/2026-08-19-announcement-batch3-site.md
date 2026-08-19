# 公告管理 - 批次 3：官网前端 + Mock API 实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现公告管理的官网前端展示功能和 Mock API

**Architecture:** Vue3 + TypeScript，官网门户公开访问（无需登录）

**Tech Stack:** Vue 3.x, TypeScript 5.x, Element Plus, vue-i18n, Axios

---

## 文件结构

**创建文件：**
- `EasyProduct.Site/src/types/announcement.ts` - 公告类型定义（复用 Admin）
- `EasyProduct.Site/src/api/announcement.ts` - 公告 API（官网）
- `EasyProduct.Site/src/views/announcement/index.vue` - 公告列表页
- `EasyProduct.Site/src/views/announcement/detail.vue` - 公告详情页
- `mock-server/routes/site/announcement.js` - 官网 Mock API

**修改文件：**
- `EasyProduct.Site/src/router/index.ts` - 添加公告路由
- `EasyProduct.Site/src/i18n/zh-CN/site.json` - 添加公告国际化
- `EasyProduct.Site/src/i18n/en-US/site.json` - 添加公告国际化

---

## Task 1: 创建官网公告类型定义

**Files:**
- Create: `EasyProduct.Site/src/types/announcement.ts`

- [ ] **Step 1: 创建公告类型定义文件**

```typescript
// src/types/announcement.ts

/**
 * 公告级别枚举
 */
export type AnnouncementLevel = 'normal' | 'important' | 'urgent'

/**
 * 附件信息
 */
export interface Attachment {
  name: string
  url: string
  size: number
}

/**
 * 公开公告实体（官网展示）
 */
export interface PublicAnnouncement {
  id: string
  title: string
  content: string
  level: AnnouncementLevel
  isTop: boolean
  publishTime: string
  attachments?: Attachment[]
}

/**
 * 公告查询参数
 */
export interface AnnouncementQuery {
  pageIndex: number
  pageSize: number
}
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Site && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 提交类型定义**

```bash
git add EasyProduct.Site/src/types/announcement.ts
git commit -m "feat(site): 添加官网公告类型定义"
```

---

## Task 2: 创建官网公告 API

**Files:**
- Create: `EasyProduct.Site/src/api/announcement.ts`

- [ ] **Step 1: 创建公告 API 文件**

```typescript
// src/api/announcement.ts
import { get } from '@/utils/request'
import type { PublicAnnouncement, AnnouncementQuery } from '@/types/announcement'

/**
 * 获取公告列表（官网公开）
 */
export const getPublicAnnouncementList = (params: AnnouncementQuery) =>
  get<{ list: PublicAnnouncement[]; total: number }>('/api/site/announcement', params)

/**
 * 获取公告详情（官网公开）
 */
export const getPublicAnnouncementById = (id: string) =>
  get<PublicAnnouncement>(`/api/site/announcement/${id}`)
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Site && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 提交 API 层**

```bash
git add EasyProduct.Site/src/api/announcement.ts
git commit -m "feat(site): 添加官网公告 API"
```

---

## Task 3: 添加官网国际化文本

**Files:**
- Modify: `EasyProduct.Site/src/i18n/zh-CN/site.json`
- Modify: `EasyProduct.Site/src/i18n/en-US/site.json`

- [ ] **Step 1: 在 zh-CN/site.json 中添加公告国际化**

添加：

```json
"announcement": {
  "title": "公告中心",
  "list": "公告列表",
  "detail": "公告详情",
  "level": "级别",
  "levelNormal": "普通",
  "levelImportant": "重要",
  "levelUrgent": "紧急",
  "publishTime": "发布时间",
  "attachments": "附件",
  "downloadAttachment": "下载附件",
  "backToList": "返回列表",
  "noData": "暂无公告"
}
```

- [ ] **Step 2: 在 en-US/site.json 中添加公告国际化**

添加：

```json
"announcement": {
  "title": "Announcement Center",
  "list": "Announcement List",
  "detail": "Announcement Detail",
  "level": "Level",
  "levelNormal": "Normal",
  "levelImportant": "Important",
  "levelUrgent": "Urgent",
  "publishTime": "Publish Time",
  "attachments": "Attachments",
  "downloadAttachment": "Download",
  "backToList": "Back to List",
  "noData": "No announcements"
}
```

- [ ] **Step 3: 运行 i18n 检查验证**

Run: `cd EasyProduct.Site && pnpm run check:i18n`
Expected: PASS

- [ ] **Step 4: 提交国际化文件**

```bash
git add EasyProduct.Site/src/i18n/zh-CN/site.json EasyProduct.Site/src/i18n/en-US/site.json
git commit -m "feat(site): 添加官网公告国际化文本"
```

---

## Task 4: 创建官网公告列表页

**Files:**
- Create: `EasyProduct.Site/src/views/announcement/index.vue`

- [ ] **Step 1: 创建公告列表页组件**

```vue
<!-- src/views/announcement/index.vue -->
<template>
  <div class="announcement-page">
    <div class="announcement-page__header">
      <h1>{{ t('site.announcement.title') }}</h1>
    </div>

    <div v-loading="loading" class="announcement-page__list">
      <div v-if="!loading && announcements.length === 0" class="announcement-page__empty">
        {{ t('site.announcement.noData') }}
      </div>

      <div
        v-for="item in announcements"
        :key="item.id"
        class="announcement-item"
        @click="handleView(item)"
      >
        <div class="announcement-item__header">
          <span class="announcement-item__title">
            <el-tag
              v-if="item.level !== 'normal'"
              :type="getLevelType(item.level)"
              size="small"
            >
              {{ t(`site.announcement.level${capitalize(item.level)}`) }}
            </el-tag>
            {{ item.title }}
          </span>
          <span class="announcement-item__time">
            {{ formatDate(item.publishTime) }}
          </span>
        </div>
      </div>

      <el-pagination
        v-if="total > pageSize"
        :current-page="pageIndex"
        :page-size="pageSize"
        :total="total"
        layout="prev, pager, next"
        @current-change="handlePageChange"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useLocale } from '@/composables/useLocale'
import { getPublicAnnouncementList } from '@/api/announcement'
import type { PublicAnnouncement } from '@/types/announcement'

const { t } = useLocale()
const router = useRouter()

const loading = ref(false)
const announcements = ref<PublicAnnouncement[]>([])
const total = ref(0)
const pageIndex = ref(1)
const pageSize = ref(10)

const loadData = async () => {
  loading.value = true
  try {
    const { list, total: totalCount } = await getPublicAnnouncementList({
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
    })
    announcements.value = list
    total.value = totalCount
  } finally {
    loading.value = false
  }
}

const handlePageChange = (page: number) => {
  pageIndex.value = page
  loadData()
}

const handleView = (item: PublicAnnouncement) => {
  router.push(`/announcement/${item.id}`)
}

const getLevelType = (level: string) => {
  const types: Record<string, string> = {
    normal: 'info',
    important: 'warning',
    urgent: 'danger',
  }
  return types[level] || 'info'
}

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString()
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
  max-width: 800px;
  margin: 0 auto;
  padding: $spacing-lg;

  &__header {
    margin-bottom: $spacing-lg;
    text-align: center;

    h1 {
      margin: 0;
      font-size: $font-size-xl;
      color: $text-primary;
    }
  }

  &__list {
    min-height: 400px;
  }

  &__empty {
    text-align: center;
    color: $text-secondary;
    padding: $spacing-xl;
  }
}

.announcement-item {
  padding: $spacing-md;
  border-bottom: 1px solid $border-color;
  cursor: pointer;
  transition: background-color 0.3s;

  &:hover {
    background-color: $bg-hover;
  }

  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  &__title {
    font-size: $font-size-md;
    color: $text-primary;
    flex: 1;

    .el-tag {
      margin-right: $spacing-xs;
    }
  }

  &__time {
    font-size: $font-size-sm;
    color: $text-secondary;
  }
}
</style>
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Site && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 运行 ESLint 检查**

Run: `cd EasyProduct.Site && pnpm lint`
Expected: PASS

- [ ] **Step 4: 提交公告列表页**

```bash
git add EasyProduct.Site/src/views/announcement/index.vue
git commit -m "feat(site): 创建官网公告列表页"
```

---

## Task 5: 创建官网公告详情页

**Files:**
- Create: `EasyProduct.Site/src/views/announcement/detail.vue`

- [ ] **Step 1: 创建公告详情页组件**

```vue
<!-- src/views/announcement/detail.vue -->
<template>
  <div v-loading="loading" class="announcement-detail">
    <div v-if="announcement" class="announcement-detail__content">
      <div class="announcement-detail__header">
        <el-button @click="handleBack">
          {{ t('site.announcement.backToList') }}
        </el-button>
      </div>

      <h1 class="announcement-detail__title">{{ announcement.title }}</h1>

      <div class="announcement-detail__meta">
        <el-tag
          v-if="announcement.level !== 'normal'"
          :type="getLevelType(announcement.level)"
          size="small"
        >
          {{ t(`site.announcement.level${capitalize(announcement.level)}`) }}
        </el-tag>
        <span class="announcement-detail__time">
          {{ t('site.announcement.publishTime') }}: {{ formatDate(announcement.publishTime) }}
        </span>
      </div>

      <div class="announcement-detail__body" v-html="announcement.content"></div>

      <div v-if="announcement.attachments && announcement.attachments.length > 0" class="announcement-detail__attachments">
        <h3>{{ t('site.announcement.attachments') }}</h3>
        <div class="attachment-list">
          <div
            v-for="(file, index) in announcement.attachments"
            :key="index"
            class="attachment-item"
          >
            <span class="attachment-item__name">{{ file.name }}</span>
            <el-button link type="primary" @click="handleDownload(file)">
              {{ t('site.announcement.downloadAttachment') }}
            </el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useLocale } from '@/composables/useLocale'
import { getPublicAnnouncementById } from '@/api/announcement'
import type { PublicAnnouncement, Attachment } from '@/types/announcement'

const { t } = useLocale()
const route = useRoute()
const router = useRouter()

const loading = ref(false)
const announcement = ref<PublicAnnouncement | null>(null)

const loadData = async () => {
  const id = route.params.id as string
  if (!id) return

  loading.value = true
  try {
    announcement.value = await getPublicAnnouncementById(id)
  } finally {
    loading.value = false
  }
}

const handleBack = () => {
  router.push('/announcement')
}

const handleDownload = (file: Attachment) => {
  window.open(file.url, '_blank')
}

const getLevelType = (level: string) => {
  const types: Record<string, string> = {
    normal: 'info',
    important: 'warning',
    urgent: 'danger',
  }
  return types[level] || 'info'
}

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleDateString()
}

const capitalize = (str: string) => {
  return str.charAt(0).toUpperCase() + str.slice(1)
}

onMounted(() => {
  loadData()
})
</script>

<style scoped lang="scss">
.announcement-detail {
  max-width: 800px;
  margin: 0 auto;
  padding: $spacing-lg;
  min-height: 600px;

  &__header {
    margin-bottom: $spacing-lg;
  }

  &__title {
    margin: 0 0 $spacing-md;
    font-size: $font-size-xl;
    color: $text-primary;
  }

  &__meta {
    display: flex;
    align-items: center;
    gap: $spacing-md;
    margin-bottom: $spacing-lg;
    padding-bottom: $spacing-md;
    border-bottom: 1px solid $border-color;
  }

  &__time {
    font-size: $font-size-sm;
    color: $text-secondary;
  }

  &__body {
    line-height: 1.8;
    color: $text-primary;

    p {
      margin-bottom: $spacing-md;
    }

    ul, ol {
      margin-bottom: $spacing-md;
      padding-left: $spacing-lg;
    }
  }

  &__attachments {
    margin-top: $spacing-xl;
    padding-top: $spacing-lg;
    border-top: 1px solid $border-color;

    h3 {
      margin-bottom: $spacing-md;
      font-size: $font-size-md;
      color: $text-primary;
    }
  }
}

.attachment-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-sm;
}

.attachment-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: $spacing-sm;
  background-color: $bg-secondary;
  border-radius: $border-radius;

  &__name {
    font-size: $font-size-sm;
    color: $text-primary;
  }
}
</style>
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Site && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 运行 ESLint 检查**

Run: `cd EasyProduct.Site && pnpm lint`
Expected: PASS

- [ ] **Step 4: 提交公告详情页**

```bash
git add EasyProduct.Site/src/views/announcement/detail.vue
git commit -m "feat(site): 创建官网公告详情页"
```

---

## Task 6: 创建官网 Mock API

**Files:**
- Create: `mock-server/routes/site/announcement.js`

- [ ] **Step 1: 创建官网 Mock API 路由文件**

```javascript
// mock-server/routes/site/announcement.js
const express = require('express')
const router = express.Router()
const announcements = require('../../data/announcement')

/**
 * 获取公告列表（官网公开）
 */
router.get('/', (req, res) => {
  const { pageIndex = 1, pageSize = 10 } = req.query

  // 仅返回已发布、未撤回的公告
  const published = announcements.filter(
    item => item.status === 'published'
  )

  // 排序：置顶优先，然后按发布时间倒序
  published.sort((a, b) => {
    if (a.isTop !== b.isTop) {
      return b.isTop ? 1 : -1
    }
    return new Date(b.publishTime) - new Date(a.publishTime)
  })

  // 分页
  const start = (pageIndex - 1) * pageSize
  const end = start + parseInt(pageSize)
  const list = published.slice(start, end).map(item => ({
    id: item.id,
    title: item.title,
    content: item.content,
    level: item.level,
    isTop: item.isTop,
    publishTime: item.publishTime,
    attachments: item.attachments
  }))

  res.json({
    code: 200,
    message: 'success',
    data: {
      list,
      total: published.length
    },
    timestamp: Date.now()
  })
})

/**
 * 获取公告详情（官网公开）
 */
router.get('/:id', (req, res) => {
  const { id } = req.params
  const announcement = announcements.find(item => item.id === id)

  // 仅返回已发布的公告
  if (!announcement || announcement.status !== 'published') {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  res.json({
    code: 200,
    message: 'success',
    data: {
      id: announcement.id,
      title: announcement.title,
      content: announcement.content,
      level: announcement.level,
      isTop: announcement.isTop,
      publishTime: announcement.publishTime,
      attachments: announcement.attachments
    },
    timestamp: Date.now()
  })
})

module.exports = router
```

- [ ] **Step 2: 提交官网 Mock API**

```bash
git add mock-server/routes/site/announcement.js
git commit -m "feat(mock): 添加官网公告 Mock API"
```

---

## Task 7: 注册官网 Mock 路由

**Files:**
- Modify: `mock-server/app.js`

- [ ] **Step 1: 在 app.js 中注册官网公告路由**

找到路由注册部分，添加：

```javascript
// 官网公告路由
const siteAnnouncementRouter = require('./routes/site/announcement')
app.use('/api/site/announcement', siteAnnouncementRouter)
```

- [ ] **Step 2: 重启 Mock 服务器验证**

Run: `cd mock-server && pnpm dev`
Expected: 服务正常启动

- [ ] **Step 3: 测试官网 API**

测试：
```bash
curl http://localhost:7700/api/site/announcement?pageIndex=1&pageSize=10
```

Expected: 返回已发布的公告列表

- [ ] **Step 4: 提交路由注册**

```bash
git add mock-server/app.js
git commit -m "feat(mock): 注册官网公告 Mock 路由"
```

---

## Task 8: 添加官网路由配置

**Files:**
- Modify: `EasyProduct.Site/src/router/index.ts`

- [ ] **Step 1: 在路由配置中添加公告路由**

在路由配置中添加：

```typescript
{
  path: '/announcement',
  name: 'announcement',
  component: () => import('@/views/announcement/index.vue'),
  meta: { title: 'site.announcement.title' }
},
{
  path: '/announcement/:id',
  name: 'announcement-detail',
  component: () => import('@/views/announcement/detail.vue'),
  meta: { title: 'site.announcement.detail' }
}
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.Site && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 提交路由配置**

```bash
git add EasyProduct.Site/src/router/index.ts
git commit -m "feat(site): 添加官网公告路由"
```

---

## 验收检查

- [ ] 运行官网完整验证

```bash
cd EasyProduct.Site
pnpm type-check
pnpm lint
pnpm run check:i18n
pnpm build
```

Expected: 全部通过

- [ ] 手动测试官网页面

1. 访问 http://localhost:5174/announcement（或实际端口）
2. 验证公告列表显示
3. 点击公告查看详情
4. 验证附件下载

- [ ] 提交批次 3 所有文件

```bash
git add .
git commit -m "feat: 完成公告管理批次3 - 官网前端 + Mock API"
```

---

## 后续批次

批次 3 完成后，将继续：
- 批次 4：小程序前端 + Mock