# EasyProduct Site 11 门户页实现计划（F1 阶段）

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成 EasyProduct.Site 官网 11 个门户页（首页/产品列表/产品详情/新闻列表/新闻详情/视频/下载/关于/联系/询价/404），实现完整询价闭环，所有 API 走 mock-server。

**Architecture:** 在 F0 Site 骨架基础上，新增通用组件库、产品/新闻/视频/下载/关于/联系/询价页面，mock-server 补全 site 分区全量路由，询价篮使用本地 Pinia store，提交时 POST 到 mock。

**Tech Stack:** Vue 3.4+ / TypeScript 5.3+ / Vite 5.4+ / Pinia 2.1+ / vue-i18n 9.9+ / axios 1.6+ / dayjs 1.11+ / SCSS（CSS Variables） / 响应式断点（768px/1200px）

---

## Global Constraints

1. 统一响应信封 `{code, message, data, timestamp}`，HTTP 一律 200，`code===200` 成功
2. 主键/外键一律 GUID 字符串；JSON 字段 camelCase；分页 `pageIndex/pageSize` + `list/total`
3. 样式仅 SCSS、组件必须 scoped、禁止硬编码色值/间距（走 variables/CSS Variables）
4. 所有可见文案走 i18n key，禁止硬编码中文；`npm run check:i18n` 门禁
5. API 文件 `api/site/<模块>.ts`；类型 `types/site.ts`，接口不加 I 前缀
6. 产品与新闻内容的中英字段由后端存储（title/titleEn），不进语言包
7. 响应式断点：mobile (<768px)、tablet (768-1199px)、desktop (≥1200px)
8. 每个提交满足 `docs/frontend-guidelines.md` 第 5 节 Definition of Done

---

## F1 文件结构增量

```text
EasyProduct.Site/src/
├── api/site/product.ts  category.ts  news.ts  video.ts  download.ts  about.ts  contact.ts  inquiry.ts
├── components/
│   ├── common/AppCarousel.vue  AppPagination.vue  AppEmpty.vue  AppLoading.vue  AppSkeleton.vue
│   └── product/ProductCard.vue  ProductGrid.vue  ProductFilter.vue
├── composables/useLoading.ts  useInquiry.ts  useResponsive.ts
├── stores/inquiry.ts            # 询价篮（本地 store，提交落 site_inquiry）
├── types/site.ts                # 追加 Product/ProductCategory/Video/Download/About/Contact/Inquiry 类型
├── i18n/*/site.json             # 追加 products/news/videos/downloads/about/contact/inquiry 键组
└── views/
    ├── products/index.vue  products/detail.vue
    ├── news/index.vue  news/detail.vue
    ├── videos/index.vue  downloads/index.vue
    ├── about/index.vue  contact/index.vue  inquiry/index.vue
    └── home/index.vue（F0 占位升级为完整首页）
mock-server/src/
├── data/product.ts  site-full.ts
├── store/inquiry.ts
└── routes/site/product.ts  category.ts  news.ts  video.ts  download.ts  about.ts  contact.ts  inquiry.ts
```

---

### Task F1-0: mock-server site 分区全量路由与数据

**Files:**
- Create: `mock-server/src/data/product.ts`
- Create: `mock-server/src/data/site-full.ts`
- Create: `mock-server/src/store/inquiry.ts`
- Create: `mock-server/src/routes/site/product.ts`
- Create: `mock-server/src/routes/site/category.ts`
- Create: `mock-server/src/routes/site/news.ts`
- Create: `mock-server/src/routes/site/video.ts`
- Create: `mock-server/src/routes/site/download.ts`
- Create: `mock-server/src/routes/site/about.ts`
- Create: `mock-server/src/routes/site/contact.ts`
- Create: `mock-server/src/routes/site/inquiry.ts`
- Modify: `mock-server/src/server.ts`（挂载路由）
- Modify: `mock-server/src/registry.ts`（注册 inquiry store reset）

**Interfaces:**
- Produces:
  - `GET /api/site/product/list?pageIndex&pageSize&categoryId&keyword` → 分页产品列表
  - `GET /api/site/product/:id` → 产品详情
  - `GET /api/site/category/list` → 分类树
  - `GET /api/site/news/list` → 新闻分页
  - `GET /api/site/news/:id` → 新闻详情
  - `GET /api/site/video/list` → 视频列表
  - `GET /api/site/download/list` → 下载列表
  - `GET /api/site/about/detail` → 关于单页
  - `POST /api/site/contact` → 联系表单提交
  - `POST /api/site/inquiry` → 询价提交（含明细）
  - `GET /api/site/inquiry/:id` → 查询询价单

- [ ] **Step F1-0.1: 创建产品数据 `mock-server/src/data/product.ts`**

```typescript
// mock-server/src/data/product.ts
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

// 产品分类
export const CATEGORIES = [
  { id: guid(), name: '工业设备', nameEn: 'Industrial Equipment', parentId: '0', sort: 1 },
  { id: guid(), name: '电子产品', nameEn: 'Electronics', parentId: '0', sort: 2 },
  { id: guid(), name: '办公用品', nameEn: 'Office Supplies', parentId: '0', sort: 3 },
]

// 产品列表（mockjs 生成 50 条）
export const PRODUCTS = Mock.mock({
  'list|50': [{
    id: '@guid',
    categoryId: () => CATEGORIES[Math.floor(Math.random() * CATEGORIES.length)].id,
    code: '@word(8)',
    name: '@ctitle(10,20)',
    nameEn: '@title(5,10)',
    summary: '@cparagraph(1)',
    summaryEn: '@sentence(5,10)',
    coverImage: '@image(800x600)',
    'images|3-6': ['@image(800x600)'],
    'price|100-10000.2': 1,
    unit: '台',
    specs: JSON.stringify({ weight: '@float(1,100,2,2)kg', size: 'LxWxH mm' }),
    status: 'active',
    createdAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((p: Record<string, unknown>) => ({ ...p, id: guid(), createdAt: isoTime() }))
```

- [ ] **Step F1-0.2: 创建完整数据 `mock-server/src/data/site-full.ts`**

```typescript
// mock-server/src/data/site-full.ts
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

// 新闻列表（30 条）
export const NEWS_FULL = Mock.mock({
  'list|30': [{
    id: '@guid',
    categoryId: '@guid',
    title: '@ctitle(15,30)',
    titleEn: '@title(8,15)',
    summary: '@cparagraph(1,2)',
    summaryEn: '@sentence(10,20)',
    content: '@cparagraph(5,10)',
    contentEn: '@paragraph(10,20)',
    coverImage: '@image(640x360)',
    isTop: '@boolean',
    'viewCount|100-9999': 1,
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((n: Record<string, unknown>) => ({ ...n, id: guid(), publishTime: isoTime() }))

// 视频列表（10 条）
export const VIDEOS = Mock.mock({
  'list|10': [{
    id: '@guid',
    title: '@ctitle(10,20)',
    titleEn: '@title(5,10)',
    coverImage: '@image(1280x720)',
    videoUrl: 'https://example.com/video.mp4',
    duration: '@integer(60,600)',
    'viewCount|100-5000': 1,
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((v: Record<string, unknown>) => ({ ...v, id: guid(), publishTime: isoTime() }))

// 下载列表（15 条）
export const DOWNLOADS = Mock.mock({
  'list|15': [{
    id: '@guid',
    title: '@ctitle(8,15)',
    titleEn: '@title(4,8)',
    fileUrl: 'https://example.com/file.pdf',
    'fileSize|1024-10485760': 1,
    'downloadCount|0-500': 1,
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((d: Record<string, unknown>) => ({ ...d, id: guid(), publishTime: isoTime() }))

// 关于单页
export const ABOUT = {
  id: guid(),
  title: '关于我们',
  titleEn: 'About Us',
  content: 'EasyProduct 是一家专注于工业设备与电子产品研发的高科技企业...',
  contentEn: 'EasyProduct is a high-tech enterprise focused on industrial equipment and electronics...',
  updatedAt: isoTime(),
}

// 联系信息
export const CONTACT_INFO = {
  address: '北京市朝阳区建国路88号',
  addressEn: '88 Jianguo Road, Chaoyang District, Beijing',
  phone: '010-12345678',
  email: 'contact@easyproduct.com',
  workingHours: '周一至周五 9:00-18:00',
  workingHoursEn: 'Mon-Fri 9:00-18:00',
}
```

- [ ] **Step F1-0.3: 创建询价 store `mock-server/src/store/inquiry.ts`**

```typescript
// mock-server/src/store/inquiry.ts
import { guid, isoTime } from '../helpers/id.js'

interface InquiryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
  remark?: string
}

interface Inquiry {
  id: string
  companyName: string
  contactName: string
  phone: string
  email: string
  items: InquiryItem[]
  status: 'pending' | 'processing' | 'completed'
  createdAt: string
}

const inquiries: Inquiry[] = []

export function resetInquiryStore(): void {
  inquiries.length = 0
}

export function createInquiry(data: Omit<Inquiry, 'id' | 'status' | 'createdAt'>): Inquiry {
  const inquiry: Inquiry = {
    ...data,
    id: guid(),
    status: 'pending',
    createdAt: isoTime(),
  }
  inquiries.push(inquiry)
  return inquiry
}

export function getInquiry(id: string): Inquiry | undefined {
  return inquiries.find(i => i.id === id)
}

export function listInquiries(): Inquiry[] {
  return [...inquiries]
}
```

- [ ] **Step F1-0.4: 创建产品路由 `mock-server/src/routes/site/product.ts`**

```typescript
// mock-server/src/routes/site/product.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { PRODUCTS, CATEGORIES } from '../../data/product.js'

export const siteProductRouter = Router()

// 产品列表（支持分类筛选、关键词搜索）
siteProductRouter.get('/product/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const categoryId = req.query.categoryId as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = PRODUCTS
  if (categoryId) {
    filtered = filtered.filter(p => p.categoryId === categoryId)
  }
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(p =>
      p.name.includes(keyword) ||
      p.nameEn.toLowerCase().includes(kw) ||
      p.code.toLowerCase().includes(kw)
    )
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 产品详情
siteProductRouter.get('/product/:id', (req, res) => {
  const product = PRODUCTS.find(p => p.id === req.params.id)
  if (!product) {
    res.json(fail('产品不存在', 404))
    return
  }
  res.json(ok(product))
})
```

- [ ] **Step F1-0.5: 创建分类路由 `mock-server/src/routes/site/category.ts`**

```typescript
// mock-server/src/routes/site/category.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { CATEGORIES } from '../../data/product.js'

export const siteCategoryRouter = Router()

// 分类树（一级分类）
siteCategoryRouter.get('/category/list', (_req, res) => {
  res.json(ok(CATEGORIES))
})
```

- [ ] **Step F1-0.6: 创建新闻路由 `mock-server/src/routes/site/news.ts`**

```typescript
// mock-server/src/routes/site/news.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { NEWS_FULL } from '../../data/site-full.js'

export const siteNewsRouter = Router()

// 新闻列表
siteNewsRouter.get('/news/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined

  let filtered = NEWS_FULL
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(n =>
      n.title.includes(keyword) ||
      n.titleEn.toLowerCase().includes(kw)
    )
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 新闻详情
siteNewsRouter.get('/news/:id', (req, res) => {
  const news = NEWS_FULL.find(n => n.id === req.params.id)
  if (!news) {
    res.json(fail('新闻不存在', 404))
    return
  }
  res.json(ok(news))
})
```

- [ ] **Step F1-0.7: 创建视频路由 `mock-server/src/routes/site/video.ts`**

```typescript
// mock-server/src/routes/site/video.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { VIDEOS } from '../../data/site-full.js'

export const siteVideoRouter = Router()

siteVideoRouter.get('/video/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  res.json(ok(paginate(VIDEOS, pageIndex, pageSize)))
})
```

- [ ] **Step F1-0.8: 创建下载路由 `mock-server/src/routes/site/download.ts`**

```typescript
// mock-server/src/routes/site/download.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { DOWNLOADS } from '../../data/site-full.js'

export const siteDownloadRouter = Router()

siteDownloadRouter.get('/download/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  res.json(ok(paginate(DOWNLOADS, pageIndex, pageSize)))
})
```

- [ ] **Step F1-0.9: 创建关于路由 `mock-server/src/routes/site/about.ts`**

```typescript
// mock-server/src/routes/site/about.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { ABOUT, CONTACT_INFO } from '../../data/site-full.js'

export const siteAboutRouter = Router()

// 关于单页
siteAboutRouter.get('/about/detail', (_req, res) => {
  res.json(ok(ABOUT))
})

// 联系信息
siteAboutRouter.get('/contact/info', (_req, res) => {
  res.json(ok(CONTACT_INFO))
})
```

- [ ] **Step F1-0.10: 创建联系表单路由 `mock-server/src/routes/site/contact.ts`**

```typescript
// mock-server/src/routes/site/contact.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'

export const siteContactRouter = Router()

// 简单内存限流（同一 IP 60秒内最多提交3次）
const rateLimit = new Map<string, number[]>()
const LIMIT = 3
const WINDOW = 60000

siteContactRouter.post('/contact', (req, res) => {
  const ip = req.ip || 'unknown'
  const now = Date.now()

  const times = rateLimit.get(ip) || []
  const recent = times.filter(t => now - t < WINDOW)

  if (recent.length >= LIMIT) {
    res.json(fail('提交过于频繁，请稍后再试', 429))
    return
  }

  recent.push(now)
  rateLimit.set(ip, recent)

  // 模拟成功
  res.json(ok(null, '提交成功'))
})
```

- [ ] **Step F1-0.11: 创建询价路由 `mock-server/src/routes/site/inquiry.ts`**

```typescript
// mock-server/src/routes/site/inquiry.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { createInquiry, getInquiry } from '../../store/inquiry.js'

export const siteInquiryRouter = Router()

// 提交询价
siteInquiryRouter.post('/inquiry', (req, res) => {
  const { companyName, contactName, phone, email, items } = req.body

  if (!companyName || !contactName || !phone || !email || !items || !items.length) {
    res.json(fail('请填写完整信息'))
    return
  }

  const inquiry = createInquiry({ companyName, contactName, phone, email, items })
  res.json(ok(inquiry, '询价提交成功'))
})

// 查询询价单
siteInquiryRouter.get('/inquiry/:id', (req, res) => {
  const inquiry = getInquiry(req.params.id)
  if (!inquiry) {
    res.json(fail('询价单不存在', 404))
    return
  }
  res.json(ok(inquiry))
})
```

- [ ] **Step F1-0.12: 注册询价 store reset**

修改 `mock-server/src/helpers/registry.ts`：

```typescript
// 添加到已有的 resetters 注册
import { resetInquiryStore } from '../store/inquiry.js'

// 在已有 registerReset 调用后添加
registerReset(resetInquiryStore)
```

- [ ] **Step F1-0.13: 挂载路由到 server.ts**

修改 `mock-server/src/server.ts`：

```typescript
// 在已有的 siteHomeRouter 导入后添加
import { siteProductRouter } from './routes/site/product.js'
import { siteCategoryRouter } from './routes/site/category.js'
import { siteNewsRouter } from './routes/site/news.js'
import { siteVideoRouter } from './routes/site/video.js'
import { siteDownloadRouter } from './routes/site/download.js'
import { siteAboutRouter } from './routes/site/about.js'
import { siteContactRouter } from './routes/site/contact.js'
import { siteInquiryRouter } from './routes/site/inquiry.js'

// 在已有的 app.use('/api/site', siteHomeRouter) 后添加
app.use('/api/site', siteProductRouter)
app.use('/api/site', siteCategoryRouter)
app.use('/api/site', siteNewsRouter)
app.use('/api/site', siteVideoRouter)
app.use('/api/site', siteDownloadRouter)
app.use('/api/site', siteAboutRouter)
app.use('/api/site', siteContactRouter)
app.use('/api/site', siteInquiryRouter)
```

- [ ] **Step F1-0.14: 验证 mock 路由**

Run:

```bash
cd mock-server && pnpm dev
# 另开终端验证
curl http://localhost:7700/api/site/product/list?pageIndex=1&pageSize=5
curl http://localhost:7700/api/site/category/list
curl http://localhost:7700/api/site/news/list?pageIndex=1&pageSize=5
curl http://localhost:7700/api/site/video/list
curl http://localhost:7700/api/site/download/list
curl http://localhost:7700/api/site/about/detail
curl -X POST http://localhost:7700/api/site/contact -H "Content-Type: application/json" -d '{"companyName":"Test","contactName":"Test","phone":"13800138000","email":"test@example.com","content":"Test"}'
```

Expected: 各接口返回 `code=200`，数据结构正确。

- [ ] **Step F1-0.15: Commit**

```bash
git add mock-server
git commit -m "feat(mock): site 分区全量路由（产品/分类/新闻/视频/下载/关于/联系/询价）"
```

---

### Task F1-1: Site 通用组件库

**Files:**
- Create: `EasyProduct.Site/src/components/common/AppCarousel.vue`
- Create: `EasyProduct.Site/src/components/common/AppPagination.vue`
- Create: `EasyProduct.Site/src/components/common/AppEmpty.vue`
- Create: `EasyProduct.Site/src/components/common/AppLoading.vue`
- Create: `EasyProduct.Site/src/components/common/AppSkeleton.vue`

**Interfaces:**
- Produces: 5 个通用组件，所有列表页可复用

- [ ] **Step F1-1.1: 创建 AppCarousel.vue**

```vue
<!-- src/components/common/AppCarousel.vue -->
<template>
  <div class="app-carousel">
    <div v-for="(item, index) in items" :key="index" v-show="index === currentIndex" class="app-carousel__item">
      <img :src="item.imageUrl" :alt="item.title" class="app-carousel__image" />
    </div>
    <div v-if="items.length > 1" class="app-carousel__dots">
      <button
        v-for="(_, index) in items"
        :key="index"
        class="app-carousel__dot"
        :class="{ 'app-carousel__dot--active': index === currentIndex }"
        @click="currentIndex = index"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

interface CarouselItem {
  imageUrl: string
  title: string
}

defineProps<{ items: CarouselItem[] }>()

const currentIndex = ref(0)
let timer: NodeJS.Timeout | null = null

onMounted(() => {
  timer = setInterval(() => {
    currentIndex.value = (currentIndex.value + 1) % props.items.length
  }, 5000)
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
})

const props = defineProps<{ items: CarouselItem[] }>()
</script>

<style scoped lang="scss">
.app-carousel {
  position: relative;
  width: 100%;
  border-radius: $radius-md;
  overflow: hidden;

  &__item {
    width: 100%;
  }

  &__image {
    width: 100%;
    display: block;
  }

  &__dots {
    position: absolute;
    bottom: $spacing-md;
    left: 50%;
    transform: translateX(-50%);
    display: flex;
    gap: $spacing-xs;
  }

  &__dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    border: none;
    background: rgba(255, 255, 255, 0.5);
    cursor: pointer;

    &--active {
      background: $color-primary;
    }
  }
}
</style>
```

- [ ] **Step F1-1.2: 创建 AppPagination.vue**

```vue
<!-- src/components/common/AppPagination.vue -->
<template>
  <div v-if="total > 0" class="app-pagination">
    <button class="app-pagination__btn" :disabled="currentPage === 1" @click="$emit('change', currentPage - 1)">
      {{ t('common.pagination.prev') }}
    </button>
    <span class="app-pagination__info">
      {{ t('common.pagination.page', { current: currentPage, total: totalPages }) }}
    </span>
    <button class="app-pagination__btn" :disabled="currentPage === totalPages" @click="$emit('change', currentPage + 1)">
      {{ t('common.pagination.next') }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps<{
  total: number
  pageSize: number
  currentPage: number
}>()

defineEmits<{ change: [page: number] }>()

const totalPages = computed(() => Math.ceil(props.total / props.pageSize))
</script>

<style scoped lang="scss">
.app-pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-md;
  margin-top: $spacing-lg;

  &__btn {
    padding: $spacing-xs $spacing-md;
    border: 1px solid $color-border;
    border-radius: $radius-sm;
    background: $color-bg;
    cursor: pointer;

    &:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  }

  &__info {
    color: $color-text-secondary;
  }
}
</style>
```

- [ ] **Step F1-1.3: 创建 AppEmpty.vue**

```vue
<!-- src/components/common/AppEmpty.vue -->
<template>
  <div class="app-empty">
    <div class="app-empty__icon">📭</div>
    <p class="app-empty__text">{{ text || t('common.empty') }}</p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

defineProps<{ text?: string }>()
</script>

<style scoped lang="scss">
.app-empty {
  padding: $spacing-xl * 2;
  text-align: center;

  &__icon {
    font-size: 48px;
    margin-bottom: $spacing-md;
  }

  &__text {
    color: $color-text-secondary;
  }
}
</style>
```

- [ ] **Step F1-1.4: 创建 AppLoading.vue**

```vue
<!-- src/components/common/AppLoading.vue -->
<template>
  <div v-if="loading" class="app-loading">
    <div class="app-loading__spinner" />
    <p class="app-loading__text">{{ t('common.loading') }}</p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

defineProps<{ loading: boolean }>()
</script>

<style scoped lang="scss">
.app-loading {
  padding: $spacing-xl;
  text-align: center;

  &__spinner {
    width: 32px;
    height: 32px;
    margin: 0 auto $spacing-md;
    border: 3px solid $color-border;
    border-top-color: $color-primary;
    border-radius: 50%;
    animation: spin 1s linear infinite;
  }

  &__text {
    color: $color-text-secondary;
  }
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
```

- [ ] **Step F1-1.5: 创建 AppSkeleton.vue**

```vue
<!-- src/components/common/AppSkeleton.vue -->
<template>
  <div class="app-skeleton">
    <div v-for="i in count" :key="i" class="app-skeleton__item">
      <div class="app-skeleton__image" />
      <div class="app-skeleton__text" />
      <div class="app-skeleton__text app-skeleton__text--short" />
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{ count: number }>()
</script>

<style scoped lang="scss">
.app-skeleton {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: $spacing-md;

  &__item {
    padding: $spacing-md;
    background: $color-bg-card;
    border-radius: $radius-md;
  }

  &__image {
    width: 100%;
    height: 150px;
    margin-bottom: $spacing-sm;
    background: linear-gradient(90deg, $color-bg-soft 25%, lighten($color-bg-soft, 5%) 50%, $color-bg-soft 75%);
    background-size: 200% 100%;
    animation: skeleton-loading 1.5s infinite;
    border-radius: $radius-sm;
  }

  &__text {
    height: 16px;
    margin-bottom: $spacing-xs;
    background: $color-bg-soft;
    border-radius: $radius-sm;

    &--short {
      width: 60%;
    }
  }
}

@keyframes skeleton-loading {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
</style>
```

- [ ] **Step F1-1.6: 补充 i18n 键**

在 `src/i18n/zh-CN/common.json` 添加：

```json
{
  "pagination": {
    "prev": "上一页",
    "next": "下一页",
    "page": "第 {current} 页 / 共 {total} 页"
  },
  "empty": "暂无数据",
  "loading": "加载中..."
}
```

在 `src/i18n/en-US/common.json` 添加：

```json
{
  "pagination": {
    "prev": "Previous",
    "next": "Next",
    "page": "Page {current} of {total}"
  },
  "empty": "No data",
  "loading": "Loading..."
}
```

- [ ] **Step F1-1.7: 验证组件**

```bash
cd EasyProduct.Site && pnpm type-check && pnpm lint && pnpm check:i18n
```

Expected: 三门禁全绿。

- [ ] **Step F1-1.8: Commit**

```bash
git add EasyProduct.Site/src/components/common EasyProduct.Site/src/i18n
git commit -m "feat(site): 通用组件库（carousel/pagination/empty/loading/skeleton）"
```

---

### Task F1-2: 产品域（API + 组件 + 列表页 + 详情页）

**Files:**
- Create: `EasyProduct.Site/src/api/site/product.ts`
- Create: `EasyProduct.Site/src/api/site/category.ts`
- Modify: `EasyProduct.Site/src/types/site.ts`（追加 Product/ProductCategory 类型）
- Create: `EasyProduct.Site/src/components/product/ProductCard.vue`
- Create: `EasyProduct.Site/src/components/product/ProductGrid.vue`
- Create: `EasyProduct.Site/src/components/product/ProductFilter.vue`
- Create: `EasyProduct.Site/src/views/products/index.vue`
- Create: `EasyProduct.Site/src/views/products/detail.vue`
- Modify: `EasyProduct.Site/src/router/index.ts`（添加产品路由）
- Modify: `EasyProduct.Site/src/i18n/zh-CN/site.json`（追加产品页文案）
- Modify: `EasyProduct.Site/src/i18n/en-US/site.json`

**Interfaces:**
- Produces: 产品列表（分类筛选+网格布局+分页）+ 产品详情（图集+参数+询价篮）

- [ ] **Step F1-2.1: 追加产品类型到 types/site.ts**

在已有的 `types/site.ts` 末尾添加：

```typescript
/** 产品分类 */
export interface ProductCategory {
  id: string
  name: string
  nameEn: string
  parentId: string
  sort: number
}

/** 产品 */
export interface Product {
  id: string
  categoryId: string
  code: string
  name: string
  nameEn: string
  summary: string
  summaryEn: string
  coverImage: string
  images: string[]
  price: number
  unit: string
  specs: string
  status: string
  createdAt: string
}

/** 产品查询参数 */
export interface ProductQuery {
  pageIndex: number
  pageSize: number
  categoryId?: string
  keyword?: string
}
```

- [ ] **Step F1-2.2: 创建 API 文件**

`src/api/site/product.ts`:

```typescript
import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Product, ProductQuery } from '@/types/site'

/** 产品列表 */
export const getProductList = (params: ProductQuery) =>
  get<PageResult<Product>>('/api/site/product/list', params)

/** 产品详情 */
export const getProductDetail = (id: string) =>
  get<Product>(`/api/site/product/${id}`)
```

`src/api/site/category.ts`:

```typescript
import { get } from '@/utils/request'
import type { ProductCategory } from '@/types/site'

/** 分类列表 */
export const getCategoryList = () =>
  get<ProductCategory[]>('/api/site/category/list')
```

- [ ] **Step F1-2.3: 创建 ProductCard 组件**

`src/components/product/ProductCard.vue`:

```vue
<template>
  <router-link :to="`/products/${product.id}`" class="product-card">
    <img :src="product.coverImage" :alt="productName" class="product-card__image" />
    <div class="product-card__content">
      <h3 class="product-card__title">{{ productName }}</h3>
      <p class="product-card__summary">{{ product.summary }}</p>
      <div class="product-card__footer">
        <span class="product-card__price">¥{{ product.price.toFixed(2) }}</span>
        <span class="product-card__unit">/{{ product.unit }}</span>
      </div>
    </div>
  </router-link>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { Product } from '@/types/site'

const { locale } = useI18n()

const props = defineProps<{ product: Product }>()

const productName = computed(() =>
  locale.value === 'zh-CN' ? props.product.name : props.product.nameEn
)
</script>

<style scoped lang="scss">
.product-card {
  display: block;
  border-radius: $radius-md;
  background: $color-bg-card;
  overflow: hidden;
  transition: box-shadow 0.3s;

  &:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }

  &__image {
    width: 100%;
    height: 200px;
    object-fit: cover;
  }

  &__content {
    padding: $spacing-md;
  }

  &__title {
    margin: 0 0 $spacing-xs;
    font-size: $font-size-lg;
    color: $color-text;
    @include ellipsis(2);
  }

  &__summary {
    margin: 0 0 $spacing-sm;
    color: $color-text-secondary;
    font-size: $font-size-sm;
    @include ellipsis(2);
  }

  &__footer {
    display: flex;
    align-items: baseline;
    gap: $spacing-xs;
  }

  &__price {
    font-size: 18px;
    font-weight: 600;
    color: $color-primary;
  }

  &__unit {
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }
}
</style>
```

- [ ] **Step F1-2.4: 创建 ProductGrid 组件**

`src/components/product/ProductGrid.vue`:

```vue
<template>
  <div class="product-grid">
    <ProductCard v-for="product in products" :key="product.id" :product="product" />
  </div>
</template>

<script setup lang="ts">
import ProductCard from './ProductCard.vue'
import type { Product } from '@/types/site'

defineProps<{ products: Product[] }>()
</script>

<style scoped lang="scss">
.product-grid {
  display: grid;
  gap: $spacing-lg;

  @include mobile {
    grid-template-columns: 1fr;
  }

  @include tablet {
    grid-template-columns: repeat(2, 1fr);
  }

  @include desktop {
    grid-template-columns: repeat(4, 1fr);
  }
}
</style>
```

- [ ] **Step F1-2.5: 创建 ProductFilter 组件**

`src/components/product/ProductFilter.vue`:

```vue
<template>
  <div class="product-filter">
    <div class="product-filter__category">
      <button
        class="product-filter__btn"
        :class="{ 'product-filter__btn--active': !selectedCategoryId }"
        @click="$emit('select', undefined)"
      >
        {{ t('site.products.allCategories') }}
      </button>
      <button
        v-for="cat in categories"
        :key="cat.id"
        class="product-filter__btn"
        :class="{ 'product-filter__btn--active': selectedCategoryId === cat.id }"
        @click="$emit('select', cat.id)"
      >
        {{ locale === 'zh-CN' ? cat.name : cat.nameEn }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { ProductCategory } from '@/types/site'

const { t, locale } = useI18n()

defineProps<{
  categories: ProductCategory[]
  selectedCategoryId?: string
}>()

defineEmits<{ select: [categoryId: string | undefined] }>()
</script>

<style scoped lang="scss">
.product-filter {
  margin-bottom: $spacing-lg;

  &__category {
    display: flex;
    flex-wrap: wrap;
    gap: $spacing-sm;
  }

  &__btn {
    padding: $spacing-xs $spacing-md;
    border: 1px solid $color-border;
    border-radius: $radius-sm;
    background: $color-bg;
    cursor: pointer;
    transition: all 0.3s;

    &--active {
      border-color: $color-primary;
      color: $color-primary;
    }
  }
}
</style>
```

- [ ] **Step F1-2.6: 创建产品列表页**

`src/views/products/index.vue`:

```vue
<template>
  <div class="products-page">
    <h1 class="products-page__title">{{ t('site.products.title') }}</h1>
    <ProductFilter
      :categories="categories"
      :selected-category-id="query.categoryId"
      @select="handleCategorySelect"
    />
    <AppLoading v-if="loading" />
    <AppEmpty v-else-if="!products.length" />
    <ProductGrid v-else :products="products" />
    <AppPagination
      v-if="total > 0"
      :total="total"
      :page-size="query.pageSize"
      :current-page="query.pageIndex"
      @change="handlePageChange"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import ProductFilter from '@/components/product/ProductFilter.vue'
import ProductGrid from '@/components/product/ProductGrid.vue'
import AppLoading from '@/components/common/AppLoading.vue'
import AppEmpty from '@/components/common/AppEmpty.vue'
import AppPagination from '@/components/common/AppPagination.vue'
import { getCategoryList } from '@/api/site/category'
import { getProductList } from '@/api/site/product'
import type { ProductCategory, Product } from '@/types/site'

const { t } = useI18n()

const categories = ref<ProductCategory[]>([])
const products = ref<Product[]>([])
const loading = ref(false)
const total = ref(0)
const query = ref({ pageIndex: 1, pageSize: 10 })

onMounted(async () => {
  categories.value = await getCategoryList()
  await loadProducts()
})

async function loadProducts() {
  loading.value = true
  try {
    const result = await getProductList(query.value)
    products.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
}

function handleCategorySelect(categoryId?: string) {
  query.value.categoryId = categoryId
  query.value.pageIndex = 1
  loadProducts()
}

function handlePageChange(page: number) {
  query.value.pageIndex = page
  loadProducts()
}
</script>

<style scoped lang="scss">
.products-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title {
    margin-bottom: $spacing-lg;
  }
}
</style>
```

- [ ] **Step F1-2.7: 创建产品详情页**

`src/views/products/detail.vue`:

```vue
<template>
  <div class="product-detail">
    <AppLoading v-if="loading" />
    <div v-else-if="product" class="product-detail__content">
      <div class="product-detail__gallery">
        <img :src="currentImage" :alt="productName" class="product-detail__main-image" />
        <div v-if="product.images.length > 1" class="product-detail__thumbnails">
          <button
            v-for="(img, i) in product.images"
            :key="i"
            class="product-detail__thumb"
            :class="{ 'product-detail__thumb--active': currentImage === img }"
            @click="currentImage = img"
          >
            <img :src="img" alt="" />
          </button>
        </div>
      </div>
      <div class="product-detail__info">
        <h1 class="product-detail__title">{{ productName }}</h1>
        <p class="product-detail__summary">{{ product.summary }}</p>
        <div class="product-detail__price-box">
          <span class="product-detail__price">¥{{ product.price.toFixed(2) }}</span>
          <span class="product-detail__unit">/{{ product.unit }}</span>
        </div>
        <div class="product-detail__quantity">
          <label>{{ t('site.products.quantity') }}:</label>
          <input v-model.number="quantity" type="number" min="1" />
        </div>
        <button class="product-detail__inquiry-btn" @click="handleAddToInquiry">
          {{ t('site.products.addToInquiry') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useInquiryStore } from '@/stores/inquiry'
import { getProductDetail } from '@/api/site/product'
import AppLoading from '@/components/common/AppLoading.vue'
import type { Product } from '@/types/site'

const { t, locale } = useI18n()
const route = useRoute()
const router = useRouter()
const inquiryStore = useInquiryStore()

const product = ref<Product | null>(null)
const loading = ref(false)
const currentImage = ref('')
const quantity = ref(1)

const productName = computed(() =>
  product.value ? (locale.value === 'zh-CN' ? product.value.name : product.value.nameEn) : ''
)

onMounted(async () => {
  loading.value = true
  try {
    product.value = await getProductDetail(route.params.id as string)
    currentImage.value = product.value.coverImage
  } catch (error) {
    router.push('/products')
  } finally {
    loading.value = false
  }
})

function handleAddToInquiry() {
  if (!product.value) return
  inquiryStore.addItem({
    productId: product.value.id,
    productName: productName.value,
    quantity: quantity.value,
    unit: product.value.unit,
  })
  router.push('/inquiry')
}
</script>

<style scoped lang="scss">
.product-detail {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__content {
    display: grid;
    gap: $spacing-lg;

    @include desktop {
      grid-template-columns: 1fr 1fr;
    }
  }

  &__gallery {
    display: flex;
    flex-direction: column;
    gap: $spacing-md;
  }

  &__main-image {
    width: 100%;
    border-radius: $radius-md;
  }

  &__thumbnails {
    display: flex;
    gap: $spacing-sm;
    overflow-x: auto;
  }

  &__thumb {
    flex-shrink: 0;
    width: 80px;
    height: 80px;
    border: 2px solid transparent;
    border-radius: $radius-sm;
    overflow: hidden;
    cursor: pointer;

    &--active {
      border-color: $color-primary;
    }

    img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }
  }

  &__info {
    display: flex;
    flex-direction: column;
    gap: $spacing-md;
  }

  &__title {
    margin: 0;
    font-size: 24px;
  }

  &__summary {
    color: $color-text-secondary;
  }

  &__price-box {
    padding: $spacing-md;
    background: $color-bg-soft;
    border-radius: $radius-sm;
  }

  &__price {
    font-size: 24px;
    font-weight: 600;
    color: $color-primary;
  }

  &__unit {
    color: $color-text-secondary;
  }

  &__quantity {
    display: flex;
    align-items: center;
    gap: $spacing-sm;

    input {
      width: 80px;
      padding: $spacing-xs $spacing-sm;
      border: 1px solid $color-border;
      border-radius: $radius-sm;
    }
  }

  &__inquiry-btn {
    padding: $spacing-md $spacing-lg;
    border: none;
    border-radius: $radius-sm;
    background: $color-primary;
    color: #fff;
    font-size: $font-size-lg;
    cursor: pointer;

    &:hover {
      opacity: 0.9;
    }
  }
}
</style>
```

- [ ] **Step F1-2.8: 创建询价 store**

`src/stores/inquiry.ts`:

```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

interface InquiryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
  remark?: string
}

export const useInquiryStore = defineStore('inquiry', () => {
  const items = ref<InquiryItem[]>([])

  const totalItems = computed(() =>
    items.value.reduce((sum, item) => sum + item.quantity, 0)
  )

  function addItem(item: InquiryItem) {
    const existing = items.value.find(i => i.productId === item.productId)
    if (existing) {
      existing.quantity += item.quantity
    } else {
      items.value.push(item)
    }
  }

  function removeItem(productId: string) {
    items.value = items.value.filter(i => i.productId !== productId)
  }

  function updateQuantity(productId: string, quantity: number) {
    const item = items.value.find(i => i.productId === productId)
    if (item) {
      item.quantity = quantity
    }
  }

  function clearItems() {
    items.value = []
  }

  return {
    items,
    totalItems,
    addItem,
    removeItem,
    updateQuantity,
    clearItems,
  }
})
```

- [ ] **Step F1-2.9: 添加产品路由**

修改 `src/router/index.ts`：

```typescript
// 在已有的 home 路由 children 中添加
{ path: 'products', name: 'products', component: () => import('@/views/products/index.vue'), meta: { title: 'site.products.title' } },
{ path: 'products/:id', name: 'product-detail', component: () => import('@/views/products/detail.vue'), meta: { title: 'site.products.detail' } },
```

- [ ] **Step F1-2.10: 补充 i18n 文案**

在 `src/i18n/zh-CN/site.json` 添加：

```json
{
  "products": {
    "title": "产品中心",
    "detail": "产品详情",
    "allCategories": "全部分类",
    "quantity": "数量",
    "addToInquiry": "加入询价篮"
  }
}
```

在 `src/i18n/en-US/site.json` 添加：

```json
{
  "products": {
    "title": "Products",
    "detail": "Product Detail",
    "allCategories": "All Categories",
    "quantity": "Quantity",
    "addToInquiry": "Add to Inquiry"
  }
}
```

- [ ] **Step F1-2.11: 验证产品域**

```bash
cd EasyProduct.Site && pnpm type-check && pnpm lint && pnpm check:i18n
```

Expected: 三门禁全绿。

- [ ] **Step F1-2.12: Commit**

```bash
git add EasyProduct.Site
git commit -m "feat(site): 产品域（API+组件+列表页+详情页+询价篮）"
```

---

### Task F1-3: 新闻域（API + 列表页 + 详情页）

**Files:**
- Create: `EasyProduct.Site/src/api/site/news.ts`
- Create: `EasyProduct.Site/src/views/news/index.vue`
- Create: `EasyProduct.Site/src/views/news/detail.vue`
- Modify: `EasyProduct.Site/src/router/index.ts`（添加新闻路由）
- Modify: `EasyProduct.Site/src/i18n/*/site.json`

- [ ] **Step F1-3.1: 追加新闻类型**

在 `src/types/site.ts` 添加：

```typescript
/** 新闻详情 */
export interface NewsDetail {
  id: string
  categoryId: string
  title: string
  titleEn: string
  summary: string
  summaryEn: string
  content: string
  contentEn: string
  coverImage: string
  isTop: boolean
  viewCount: number
  publishTime: string
}

/** 新闻查询参数 */
export interface NewsQuery {
  pageIndex: number
  pageSize: number
  keyword?: string
}
```

- [ ] **Step F1-3.2: 创建新闻 API**

`src/api/site/news.ts`:

```typescript
import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { NewsDetail, NewsQuery } from '@/types/site'

/** 新闻列表 */
export const getNewsList = (params: NewsQuery) =>
  get<PageResult<NewsDetail>>('/api/site/news/list', params)

/** 新闻详情 */
export const getNewsDetail = (id: string) =>
  get<NewsDetail>(`/api/site/news/${id}`)
```

- [ ] **Step F1-3.3: 创建新闻列表页**

`src/views/news/index.vue`:

```vue
<template>
  <div class="news-page">
    <h1 class="news-page__title">{{ t('site.news.title') }}</h1>
    <AppLoading v-if="loading" />
    <AppEmpty v-else-if="!newsList.length" />
    <ul v-else class="news-page__list">
      <li v-for="item in newsList" :key="item.id" class="news-item">
        <router-link :to="`/news/${item.id}`" class="news-item__link">
          <img :src="item.coverImage" :alt="item.title" class="news-item__image" />
          <div class="news-item__content">
            <div class="news-item__header">
              <h2 class="news-item__title">{{ newsTitle(item) }}</h2>
              <span v-if="item.isTop" class="news-item__top">{{ t('site.news.top') }}</span>
            </div>
            <p class="news-item__summary">{{ item.summary }}</p>
            <div class="news-item__meta">
              <span>{{ dayjs(item.publishTime).format('YYYY-MM-DD') }}</span>
              <span>{{ item.viewCount }} {{ t('site.news.views') }}</span>
            </div>
          </div>
        </router-link>
      </li>
    </ul>
    <AppPagination
      v-if="total > 0"
      :total="total"
      :page-size="query.pageSize"
      :current-page="query.pageIndex"
      @change="handlePageChange"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import AppLoading from '@/components/common/AppLoading.vue'
import AppEmpty from '@/components/common/AppEmpty.vue'
import AppPagination from '@/components/common/AppPagination.vue'
import { getNewsList } from '@/api/site/news'
import type { NewsDetail } from '@/types/site'

const { t, locale } = useI18n()

const newsList = ref<NewsDetail[]>([])
const loading = ref(false)
const total = ref(0)
const query = ref({ pageIndex: 1, pageSize: 10 })

const newsTitle = (item: NewsDetail) =>
  locale.value === 'zh-CN' ? item.title : item.titleEn

onMounted(async () => {
  loading.value = true
  try {
    const result = await getNewsList(query.value)
    newsList.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
})

function handlePageChange(page: number) {
  query.value.pageIndex = page
  loadNews()
}

async function loadNews() {
  loading.value = true
  try {
    const result = await getNewsList(query.value)
    newsList.value = result.list
    total.value = result.total
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
.news-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__title {
    margin-bottom: $spacing-lg;
  }

  &__list {
    list-style: none;
    padding: 0;
    margin: 0;
  }
}

.news-item {
  margin-bottom: $spacing-lg;
  border-bottom: 1px solid $color-border;

  &__link {
    display: grid;
    gap: $spacing-md;
    padding-bottom: $spacing-lg;

    @include desktop {
      grid-template-columns: 200px 1fr;
    }
  }

  &__image {
    width: 100%;
    height: 120px;
    object-fit: cover;
    border-radius: $radius-sm;
  }

  &__content {
    display: flex;
    flex-direction: column;
    gap: $spacing-sm;
  }

  &__header {
    display: flex;
    align-items: center;
    gap: $spacing-sm;
  }

  &__title {
    margin: 0;
    font-size: $font-size-lg;
  }

  &__top {
    padding: 2px 8px;
    background: $color-primary;
    color: #fff;
    font-size: 12px;
    border-radius: $radius-sm;
  }

  &__summary {
    margin: 0;
    color: $color-text-secondary;
    @include ellipsis(2);
  }

  &__meta {
    display: flex;
    gap: $spacing-md;
    font-size: $font-size-sm;
    color: $color-text-secondary;
  }
}
</style>
```

- [ ] **Step F1-3.4: 创建新闻详情页**

`src/views/news/detail.vue`（类似产品详情，略）

- [ ] **Step F1-3.5: 添加路由和i18n，验证并提交**

```bash
cd EasyProduct.Site && pnpm type-check && pnpm lint && pnpm check:i18n
git add EasyProduct.Site
git commit -m "feat(site): 新闻域（API+列表页+详情页）"
```

---

### Task F1-4: 视频/下载页面

**Files:**
- Create: `src/api/site/video.ts` `download.ts`
- Create: `src/views/videos/index.vue` `downloads/index.vue`
- 添加路由和 i18n

- [ ] **Step F1-4.1: 创建视频和下载页面**

参考产品列表页的实现，创建视频列表（卡片+弹窗播放）和下载列表（文件信息+下载按钮）。

- [ ] **Step F1-4.2: 验证并提交**

```bash
git add EasyProduct.Site
git commit -m "feat(site): 视频/下载页面"
```

---

### Task F1-5: 关于/联系页面

**Files:**
- Create: `src/api/site/about.ts` `contact.ts`
- Create: `src/views/about/index.vue` `contact/index.vue`

- [ ] **Step F1-5.1: 创建关于页**

从 API 获取单页内容显示。

- [ ] **Step F1-5.2: 创建联系页**

表单提交（公司名、联系人、电话、邮箱、内容），防重复提交。

- [ ] **Step F1-5.3: 验证并提交**

```bash
git commit -m "feat(site): 关于/联系页面"
```

---

### Task F1-6: 询价闭环

**Files:**
- Create: `src/composables/useInquiry.ts`
- Create: `src/api/site/inquiry.ts`
- Create: `src/views/inquiry/index.vue`
- Create: `src/stores/inquiry.ts`（已在 F1-2 完成）

- [ ] **Step F1-6.1: 创建询价页**

显示询价篮明细，填写公司信息，提交 POST `/api/site/inquiry`。

- [ ] **Step F1-6.2: 验证并提交**

```bash
git commit -m "feat(site): 询价闭环（询价篮+提交）"
```

---

### Task F1-7: 收尾

**Files:**
- Modify: `src/components/layout/AppNavbar.vue`（补全导航）
- Modify: `src/views/home/index.vue`（升级首页）
- Modify: `src/i18n/*/site.json`

- [ ] **Step F1-7.1: 补全导航**

在 AppNavbar 添加产品/新闻/视频/下载/关于/联系链接。

- [ ] **Step F1-7.2: 升级首页**

添加产品推荐、新闻动态、视频展示等模块。

- [ ] **Step F1-7.3: 最终验证**

```bash
cd EasyProduct.Site
pnpm type-check
pnpm lint
pnpm check:i18n
pnpm build
```

Expected: 所有门禁通过，build 成功。

- [ ] **Step F1-7.4: 最终提交**

```bash
git add EasyProduct.Site
git commit -m "feat(site): F1 收尾（导航补全+首页升级+SEO）"
```

---

## 验收清单

- [ ] 11 个页面可正常访问
- [ ] 产品列表筛选、分页正常
- [ ] 产品详情图集、询价篮正常
- [ ] 新闻列表、详情正常
- [ ] 视频/下载列表正常
- [ ] 关于/联系页面正常
- [ ] 询价提交成功并在 mock 端可查
- [ ] 中英双语切换正常
- [ ] 响应式布局正常
- [ ] 所有门禁通过
- [ ] `pnpm build` 成功

---

**计划版本**: v1.0（2026-08-09）