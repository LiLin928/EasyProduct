# 视频分类筛选 - 简化实施方案

> 基于 EasyProduct 项目现状，采用最小改动实现视频分类筛选功能

---

## 📋 现状分析

### 当前问题
1. **Mock 视频数据** (`mock-server/src/data/site-full.ts`) 没有 `categoryId` 字段
2. **Site Videos 页面** (`EasyProduct.Site/src/views/videos/index.vue`) 无分类筛选 UI
3. **Admin 视频管理** 无分类选择字段

### 简化思路
- 复用现有的 **新闻分类** 数据作为视频分类（或创建简单的视频分类数据）
- 仅在前端实现分类筛选（不修改后端 API 结构）
- 保持现有数据结构和 API 不变

---

## 🎯 实施步骤

### 步骤 1: Mock 数据层修改

#### 1.1 修改视频数据结构
**文件**: `mock-server/src/data/site-full.ts`

```typescript
// 在 SiteVideo 接口中添加 categoryId
export interface SiteVideo {
  id: string;
  categoryId: string;  // ← 新增字段
  title: string;
  titleEn: string;
  // ... 其他字段
}

// 视频数据添加分类（复用现有分类或创建简单分类映射）
const VIDEO_CATEGORIES = ['产品教程', '企业宣传', '客户案例', '技术培训'];

export const VIDEOS: SiteVideo[] = Array.from({ length: 20 }, (_, i) => ({
  id: guid(),
  categoryId: `cat-${(i % 4) + 1}`,  // ← 关联到4个分类
  title: Mock.mock("@ctitle(10,20)"),
  titleEn: Mock.mock("@title(5,10)"),
  // ... 其他字段
}));
```

#### 1.2 新增视频分类数据（可选）
**文件**: `mock-server/src/data/site-full.ts`（追加）

```typescript
export interface VideoCategory {
  id: string;
  name: string;
  nameEn: string;
  sort: number;
}

export const VIDEO_CATEGORIES: VideoCategory[] = [
  { id: 'cat-1', name: '产品教程', nameEn: 'Product Tutorials', sort: 1 },
  { id: 'cat-2', name: '企业宣传', nameEn: 'Corporate', sort: 2 },
  { id: 'cat-3', name: '客户案例', nameEn: 'Case Studies', sort: 3 },
  { id: 'cat-4', name: '技术培训', nameEn: 'Technical Training', sort: 4 },
];
```

#### 1.3 新增视频分类 API
**文件**: `mock-server/src/routes/site/video.ts`

```typescript
import { VIDEO_CATEGORIES } from '../../data/site-full.js';

// 新增：获取视频分类列表
siteVideoRouter.get('/video-category/list', (_req, res) => {
  res.json(ok(VIDEO_CATEGORIES));
});

// 修改：支持按分类筛选（可选，前端筛选也可）
siteVideoRouter.get('/video/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1);
  const pageSize = Number(req.query.pageSize ?? 9);
  const categoryId = req.query.categoryId as string | undefined;
  
  let list = VIDEOS.filter(v => v.status === 'published');
  
  // 按分类筛选
  if (categoryId) {
    list = list.filter(v => v.categoryId === categoryId);
  }
  
  res.json(ok(paginate(list, pageIndex, pageSize)));
});
```

---

### 步骤 2: Site 前端修改

#### 2.1 新增类型定义
**文件**: `EasyProduct.Site/src/types/site.ts`

```typescript
// 在 Video 接口中添加 categoryId
export interface Video {
  id: string;
  categoryId: string;  // ← 新增
  title: string;
  titleEn: string;
  // ... 其他字段
}

// 新增 VideoCategory 类型
export interface VideoCategory {
  id: string;
  name: string;
  nameEn: string;
  sort: number;
}
```

#### 2.2 新增 API 函数
**文件**: `EasyProduct.Site/src/api/site/video.ts`

```typescript
import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Video, VideoCategory } from '@/types/site'

// 获取视频分类列表
export const getVideoCategoryList = () =>
  get<VideoCategory[]>('/api/site/video-category/list')

// 修改：支持分类筛选参数
export const getVideoList = (params: { 
  pageIndex: number
  pageSize: number
  categoryId?: string  // ← 新增
  keyword?: string 
}) => get<PageResult<Video>>('/api/site/video/list', params)
```

#### 2.3 修改 Videos 页面
**文件**: `EasyProduct.Site/src/views/videos/index.vue`

参考 `products/index.vue` 的结构，添加以下功能：

**Template 修改：**
```vue
<template>
  <div class="videos-page">
    <div class="videos-container">
      <!-- Header -->
      <div class="videos-header">
        <h1>{{ t('site.videos.title') }}</h1>
        <p>{{ t('site.videos.subtitle') }}</p>
      </div>

      <div class="videos-content">
        <!-- 左侧分类筛选（桌面端） -->
        <aside class="videos-sidebar">
          <div class="filter-section">
            <h3 class="filter-title">{{ t('site.videos.filter') }}</h3>
            <ul class="category-list">
              <li>
                <button
                  class="category-btn"
                  :class="{ active: !selectedCategory }"
                  @click="selectCategory(undefined)"
                >
                  {{ t('site.videos.all') }}
                </button>
              </li>
              <li v-for="cat in categories" :key="cat.id">
                <button
                  class="category-btn"
                  :class="{ active: selectedCategory === cat.id }"
                  @click="selectCategory(cat.id)"
                >
                  {{ cat.name }}
                </button>
              </li>
            </ul>
          </div>
        </aside>

        <!-- 主内容区 -->
        <main class="videos-main">
          <!-- 移动端分类选择 -->
          <div class="category-tabs-mobile">
            <button
              class="tab-btn"
              :class="{ active: !selectedCategory }"
              @click="selectCategory(undefined)"
            >
              {{ t('site.videos.all') }}
            </button>
            <button
              v-for="cat in categories"
              :key="cat.id"
              class="tab-btn"
              :class="{ active: selectedCategory === cat.id }"
              @click="selectCategory(cat.id)"
            >
              {{ cat.name }}
            </button>
          </div>

          <!-- 搜索框 -->
          <div class="videos-toolbar">
            <input
              v-model="keyword"
              type="search"
              class="search-input"
              :placeholder="t('site.videos.searchPlaceholder')"
            />
          </div>

          <!-- 视频网格 -->
          <!-- ... 保持不变 ... -->
        </main>
      </div>
    </div>
  </div>
</template>
```

**Script 修改：**
```typescript
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { getVideoList, getVideoCategoryList } from '@/api/site/video'
import type { Video, VideoCategory } from '@/types/site'

const { t, locale } = useI18n()

// 新增：分类相关状态
const categories = ref<VideoCategory[]>([])
const selectedCategory = ref<string | undefined>(undefined)

// 选择分类
const selectCategory = (categoryId?: string) => {
  selectedCategory.value = categoryId
  query.value.pageIndex = 1
  loadVideos()
}

// 修改：加载视频时传入分类参数
const loadVideos = async () => {
  loading.value = true
  try {
    const res = await getVideoList({
      pageIndex: query.value.pageIndex,
      pageSize: query.value.pageSize,
      categoryId: selectedCategory.value,
      keyword: keyword.value?.trim() || undefined
    })
    videos.value = res.list || []
    total.value = res.total || 0
  } catch {
    videos.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}

// 加载分类数据
const loadCategories = async () => {
  try {
    const res = await getVideoCategoryList()
    categories.value = res || []
  } catch {
    categories.value = []
  }
}

onMounted(() => {
  loadCategories()
  loadVideos()
})
```

**Style 修改（追加）：**
```scss
.videos-content {
  display: grid;
  grid-template-columns: 200px 1fr;
  gap: $spacing-xl;

  @include compact {
    grid-template-columns: 1fr;
  }
}

.videos-sidebar {
  @include compact {
    display: none;  // 移动端隐藏侧边栏
  }
}

.category-tabs-mobile {
  display: none;
  
  @include compact {
    display: flex;
    gap: $spacing-sm;
    overflow-x: auto;
    margin-bottom: $spacing-md;
    
    .tab-btn {
      white-space: nowrap;
      padding: 8px 16px;
      border-radius: 999px;
      border: 1px solid $color-border;
      background: white;
      
      &.active {
        background: $color-primary;
        color: white;
        border-color: $color-primary;
      }
    }
  }
}

// 复用 products 页面的分类样式
.filter-section {
  background: white;
  padding: $spacing-md;
  border-radius: $radius-lg;
  box-shadow: $shadow-sm;
}

.category-list {
  list-style: none;
  padding: 0;
  margin: 0;

  li {
    margin-bottom: $spacing-xs;
  }
}

.category-btn {
  width: 100%;
  padding: $spacing-sm $spacing-md;
  background: transparent;
  border: none;
  border-radius: $radius-sm;
  text-align: left;
  cursor: pointer;
  transition: all $transition-fast;
  color: $color-text-secondary;

  &:hover {
    background: $color-bg-secondary;
    color: $color-primary;
  }

  &.active {
    background: $color-primary;
    color: white;
    font-weight: 600;
  }
}
```

#### 2.4 新增 i18n 翻译
**文件**: `EasyProduct.Site/src/i18n/locales/zh-CN/site.json`

```json
{
  "videos": {
    "title": "视频中心",
    "subtitle": "观看产品教程、企业宣传片和客户案例",
    "filter": "分类筛选",
    "all": "全部视频",
    "searchPlaceholder": "搜索视频标题"
  }
}
```

**文件**: `EasyProduct.Site/src/i18n/locales/en-US/site.json`

```json
{
  "videos": {
    "title": "Video Center",
    "subtitle": "Watch product tutorials, corporate videos and case studies",
    "filter": "Filter by Category",
    "all": "All Videos",
    "searchPlaceholder": "Search video title"
  }
}
```

---

### 步骤 3: Admin 后台修改（可选）

如果需要在 Admin 后台显示/编辑视频分类，可进行以下修改：

#### 3.1 修改视频管理表单
**文件**: `EasyProduct.Admin/src/views/site/video/index.vue`

在视频表单中添加分类选择：

```vue
<el-form-item :label="t('site.video.category')">
  <el-select v-model="form.categoryId" :placeholder="t('site.video.categoryPlaceholder')">
    <el-option
      v-for="cat in categories"
      :key="cat.id"
      :label="cat.name"
      :value="cat.id"
    />
  </el-select>
</el-form-item>
```

#### 3.2 修改视频表格
在表格中新增分类列：

```vue
<el-table-column :label="t('site.video.category')" width="120">
  <template #default="{ row }">
    {{ getCategoryName(row.categoryId) }}
  </template>
</el-table-column>
```

---

## 📁 修改文件清单

### Mock 服务器 (mock-server)
| 文件 | 操作 | 说明 |
|------|------|------|
| `src/data/site-full.ts` | 修改 | Video 接口添加 categoryId，新增 VIDEO_CATEGORIES 数据 |
| `src/routes/site/video.ts` | 修改 | 新增 video-category API，video list 支持 categoryId 参数 |

### Site 前端 (EasyProduct.Site)
| 文件 | 操作 | 说明 |
|------|------|------|
| `src/types/site.ts` | 修改 | Video 添加 categoryId，新增 VideoCategory 接口 |
| `src/api/site/video.ts` | 修改 | 新增 getVideoCategoryList，修改 getVideoList 参数 |
| `src/views/videos/index.vue` | 修改 | 添加分类筛选 UI 和逻辑 |
| `src/i18n/locales/zh-CN/site.json` | 修改 | 新增 videos.filter, videos.all 等翻译 |
| `src/i18n/locales/en-US/site.json` | 修改 | 同上，英文翻译 |

### Admin 后台 (EasyProduct.Admin) - 可选
| 文件 | 操作 | 说明 |
|------|------|------|
| `src/views/site/video/index.vue` | 修改 | 表单和表格添加分类字段 |
| `src/types/site.ts` | 修改 | Video 添加 categoryId |

---

## ✅ 验收标准

1. **Mock 数据**: Videos 数据包含 `categoryId` 字段，有4个视频分类
2. **Site 页面**: 
   - 左侧显示分类筛选（桌面端）
   - 顶部显示分类标签（移动端）
   - 点击分类可筛选视频
   - 搜索和分类筛选可同时生效
3. **Admin 后台**: 视频管理可查看/编辑分类（可选）

---

## 💡 注意事项

1. **数据兼容性**: 修改 Mock 数据后需重启 Mock 服务器
2. **类型检查**: 修改接口后运行 `pnpm type-check` 确保无类型错误
3. **构建测试**: 修改后运行 `pnpm run build` 确保构建成功
4. **i18n 检查**: 运行 `pnpm check:i18n` 确保无硬编码中文

---

## 🚀 快速开始

执行以下命令快速启动项目验证修改：

```bash
# 1. 启动 Mock 服务器
cd D:\4-MyProject\EasyProduct\mock-server
pnpm dev

# 2. 启动 Site 前端（新终端）
cd D:\4-MyProject\EasyProduct\EasyProduct.Site
pnpm dev

# 3. 访问页面
# http://localhost:5174/videos
```

---

*文档生成时间: 2026-09-03*
*适用项目: EasyProduct*
*方案类型: 简化方案（最小改动实现视频分类筛选）*
