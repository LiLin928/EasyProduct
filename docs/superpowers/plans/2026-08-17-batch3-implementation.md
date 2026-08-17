# 批次3 实现计划：公告 / 系统参数 / 个人中心 / 工作台轻量首页

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成 F2-1 Basic 模块剩余 4 页（公告/系统参数/个人中心/工作台轻量首页）+ 首次引入 ImageUpload/RichTextEditor 通用组件，收口 Basic 模块。

**Architecture:** 分层推进 + 由简到繁：先建通用组件层（ImageUpload/RichTextEditor/file 上传 mock），再按系统参数→工作台→个人中心→公告顺序逐页交付，每页完整链路（mock data + mock route + types + api + 页面 + 路由 + i18n），独立可演示可提交。

**Tech Stack:** Vue 3 + TypeScript + Element Plus + Pinia + Vue Router + vue-i18n + axios；富文本 @wangeditor/editor + @wangeditor/editor-for-vue；mock-server Node20 + Express + multer（文件上传）+ mockjs。

**设计文档：** `docs/superpowers/specs/2026-08-17-batch3-announcement-config-profile-desktop-design.md`

---

## Global Constraints（执行前必读）

1. 前端工程无单测框架，DoD = `vue-tsc --noEmit` 零错误 + ESLint 通过 + `check:i18n` 无硬编码中文 + `pnpm build` 通过。每个涉及前端的 Task 收尾都跑前三项。
2. 所有可见文案走 i18n key（`basic.announcement.*` / `basic.config.*` / `basic.profile.*` / `desktop.*`），禁止硬编码中文。i18n 文件 `EasyProduct.Admin/src/i18n/{zh-CN,en-US}/basic.json` 与 `menu.json`。
3. mock 路由注册在 `mock-server/src/server.ts:44` 的 `app.use('/api/admin', ...)` 行，新增路由在此追加。mock 路由文件 import 用 `.js` 后缀（NodeNext）。
4. 统一信封 `ok/fail/paginate`（`mock-server/src/helpers/envelope.ts`），HTTP 一律 200，`code===200` 成功。分页入参 `pageIndex/pageSize`，出参 `list/total`。
5. 主键 GUID（`guid()`）、时间 ISO 8601（`isoTime()`），均在 `mock-server/src/helpers/id.ts`。
6. 前端 API 文件 `api/basic/*.ts` 不带 Api 后缀；类型在 `types/basic.ts` 追加；接口不加 I 前缀；禁用 TS enum（用字符串联合 + as const）。
7. 列表页统一组装：`BaseSearchForm` + `useSearch` + `BaseTable` + `useTable`，弹窗用 `useDialog`/`useForm` 或直接 `ref`+`reactive`（参考 `views/basic/dict/components/DictTypeFormDialog.vue` 模板）。
8. 权限指令 `v-permission="['basic:xxx:action']"`，已全局注册（`directives/permission.ts`）。
9. 所有后端/mock 方法无需中文注释（仅后端 C# 强制）；前端代码注释中文允许但 i18n 文案禁止硬编码。
10. 提交规范：`feat(admin):` / `feat(mock):` / `refactor(admin):` 等，Conventional Commits。

## 文件结构（本批次全部文件）

**mock-server 新增/修改：**
- Create: `mock-server/src/data/admin/announcement.ts` — 公告种子
- Create: `mock-server/src/data/admin/config.ts` — 系统参数种子
- Create: `mock-server/src/routes/admin/file.ts` — 文件上传路由（multer）
- Create: `mock-server/src/routes/admin/announcement.ts` — 公告路由
- Create: `mock-server/src/routes/admin/config.ts` — 系统参数路由
- Create: `mock-server/src/routes/admin/profile.ts` — 个人中心路由
- Create: `mock-server/src/routes/admin/desktop.ts` — 工作台概览路由
- Modify: `mock-server/src/server.ts` — 注册上述路由 + 引入
- Modify: `mock-server/package.json` — 加 multer 依赖

**EasyProduct.Admin 新增/修改：**
- Modify: `EasyProduct.Admin/src/types/basic.ts` — 追加 Announcement/SystemConfig/ProfileInfo/DesktopOverview 等
- Create: `EasyProduct.Admin/src/api/common/file.ts` — 文件上传 API
- Create: `EasyProduct.Admin/src/api/basic/announcement.ts`
- Create: `EasyProduct.Admin/src/api/basic/config.ts`
- Create: `EasyProduct.Admin/src/api/basic/profile.ts`
- Create: `EasyProduct.Admin/src/api/basic/desktop.ts`
- Create: `EasyProduct.Admin/src/components/common/ImageUpload.vue`
- Create: `EasyProduct.Admin/src/components/common/RichTextEditor.vue`
- Create: `EasyProduct.Admin/src/views/basic/announcement/index.vue`
- Create: `EasyProduct.Admin/src/views/basic/announcement/components/AnnouncementFormDialog.vue`
- Create: `EasyProduct.Admin/src/views/basic/config/index.vue`
- Create: `EasyProduct.Admin/src/views/basic/config/components/ConfigFormDialog.vue`
- Create: `EasyProduct.Admin/src/views/basic/profile/index.vue`
- Modify: `EasyProduct.Admin/src/views/desktop/index.vue` — 重写为轻量首页
- Modify: `EasyProduct.Admin/src/router/modules/basic.ts` — 追加 announcement/config 子路由
- Modify: `EasyProduct.Admin/src/router/index.ts` — 追加 /profile 独立路由
- Modify: `EasyProduct.Admin/src/layouts/components/AppTopbar.vue` — realName 改为跳 /profile 入口
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/basic.json` + `en-US/basic.json` — 追加文案
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/menu.json` + `en-US/menu.json` — 追加菜单 key
- Modify: `EasyProduct.Admin/package.json` — 加 @wangeditor/editor + @wangeditor/editor-for-vue

---

## 模块0：通用组件层

### Task 1: 文件上传 mock 路由 + 前端 file API

**Files:**
- Modify: `mock-server/package.json`（加 multer）
- Create: `mock-server/src/routes/admin/file.ts`
- Modify: `mock-server/src/server.ts`
- Create: `EasyProduct.Admin/src/api/common/file.ts`

- [ ] **Step 1: 在 mock-server 安装 multer**

Run:
```bash
cd D:/4-MyProject/EasyProduct/mock-server && pnpm add multer && pnpm add -D @types/multer
```
Expected: multer 与 @types/multer 写入 dependencies/devDependencies。

- [ ] **Step 2: 创建 mock 文件上传路由**

创建 `mock-server/src/routes/admin/file.ts`：

```typescript
// src/routes/admin/file.ts
import { Router } from 'express'
import multer from 'multer'
import { ok } from '../../helpers/envelope.js'

export const adminFileRouter = Router()
const upload = multer({ storage: multer.memoryStorage() })

/** 文件上传（mock 阶段返回 base64 占位 URL，后端交付换真实 OSS） */
adminFileRouter.post('/basic/file/upload', upload.single('file'), (req, res) => {
  if (!req.file) {
    res.json(ok(null, '未接收到文件'))
    return
  }
  const ext = (req.file.originalname.split('.').pop() || 'png').toLowerCase()
  const base64 = `data:image/${ext};base64,${req.file.buffer.toString('base64')}`
  res.json(ok({ url: base64, fileName: req.file.originalname }, '上传成功'))
})
```

- [ ] **Step 3: 在 server.ts 注册 file 路由**

修改 `mock-server/src/server.ts`：

在 `import { adminRoleRouter } from './routes/admin/role.js'` 之后追加一行：
```typescript
import { adminFileRouter } from './routes/admin/file.js'
```

将 `app.use('/api/admin', adminGuard, adminAuthRouter, adminMenuRouter, adminDictRouter, adminUserRouter, adminDeptRouter, adminRoleRouter)` 末尾追加 `adminFileRouter`，改为：
```typescript
app.use('/api/admin', adminGuard, adminAuthRouter, adminMenuRouter, adminDictRouter, adminUserRouter, adminDeptRouter, adminRoleRouter, adminFileRouter)
```

- [ ] **Step 4: 启动 mock-server 验证文件上传**

Run:
```bash
cd D:/4-MyProject/EasyProduct/mock-server && pnpm dev
```
另开终端验证（先用登录拿 token，再上传）：
```bash
TOKEN=$(curl -s -X POST http://localhost:7700/api/admin/auth/login -H "Content-Type: application/json" -d '{"userName":"admin","password":"admin123"}' | node -pe 'JSON.parse(require("fs").readFileSync(0)).data.accessToken')
echo "test image content" > /tmp/test.txt
curl -s -X POST http://localhost:7700/api/admin/basic/file/upload -H "Authorization: Bearer $TOKEN" -F "file=@/tmp/test.txt" | head -c 200
```
Expected: 返回 `{"code":200,...,"data":{"url":"data:image/txt;base64,...","fileName":"test.txt"},...}`。

- [ ] **Step 5: 创建前端文件上传 API**

创建 `EasyProduct.Admin/src/api/common/file.ts`：

```typescript
// src/api/common/file.ts
import service from '@/utils/request'

/** 上传响应 */
export interface UploadResult {
  url: string
  fileName: string
}

/**
 * 上传文件（multipart form-data）
 * @param file 文件对象
 */
export const uploadFile = (file: File): Promise<UploadResult> => {
  const formData = new FormData()
  formData.append('file', file)
  return service.post('/api/admin/basic/file/upload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  }) as Promise<UploadResult>
}
```

> 注：`service` 是 `utils/request.ts` 导出的 axios 实例。若 `request.ts` 未具名导出 `service`，则改用：在文件顶部 `import { post } from '@/utils/request'` 并实现为 `post<UploadResult>('/api/admin/basic/file/upload', formData)`——执行时先读 `utils/request.ts` 确认导出，二选一。

- [ ] **Step 6: 前端类型检查**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check
```
Expected: 零错误（若报 `service` 未导出，按 Step 5 注释改用 `post`）。

- [ ] **Step 7: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/src/routes/admin/file.ts mock-server/src/server.ts mock-server/package.json EasyProduct.Admin/src/api/common/file.ts && git commit -m "feat(mock): 添加文件上传路由（multer）+ 前端 file API"
```

---

### Task 2: ImageUpload 通用组件

**Files:**
- Create: `EasyProduct.Admin/src/components/common/ImageUpload.vue`

- [ ] **Step 1: 创建 ImageUpload.vue**

创建 `EasyProduct.Admin/src/components/common/ImageUpload.vue`：

```vue
<!-- src/components/common/ImageUpload.vue -->
<template>
  <div class="image-upload">
    <el-upload
      :show-file-list="false"
      :before-upload="handleBeforeUpload"
      :http-request="handleHttpRequest"
      :disabled="disabled"
      :accept="accept"
    >
      <div
        v-if="modelValue"
        class="image-upload__preview"
        :class="{ 'image-upload__preview--circle': circle }"
      >
        <img :src="modelValue" alt="preview" />
        <div class="image-upload__mask">
          <span>{{ t('common.upload.reupload') }}</span>
        </div>
      </div>
      <div
        v-else
        class="image-upload__placeholder"
        :class="{ 'image-upload__placeholder--circle': circle }"
      >
        <el-icon><Plus /></el-icon>
        <span>{{ t('common.upload.clickToUpload') }}</span>
      </div>
    </el-upload>
    <el-button
      v-if="modelValue && !disabled"
      link
      type="danger"
      size="small"
      class="image-upload__remove"
      @click="handleRemove"
    >
      {{ t('common.upload.remove') }}
    </el-button>
  </div>
</template>

<script setup lang="ts">
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { uploadFile } from '@/api/common/file'

interface Props {
  modelValue: string
  maxSize?: number // MB
  accept?: string
  disabled?: boolean
  circle?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  maxSize: 2,
  accept: 'image/*',
  disabled: false,
  circle: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const { t } = useLocale()

const handleBeforeUpload = (file: File): boolean => {
  const sizeMb = file.size / 1024 / 1024
  if (sizeMb > props.maxSize) {
    ElMessage.warning(t('common.upload.oversize', { size: props.maxSize }))
    return false
  }
  return true
}

const handleHttpRequest = async (options: { file: File }): Promise<void> => {
  try {
    const result = await uploadFile(options.file)
    emit('update:modelValue', result.url)
  } catch {
    // 错误已由拦截器处理
  }
}

const handleRemove = (): void => {
  emit('update:modelValue', '')
}
</script>

<style scoped lang="scss">
.image-upload {
  display: inline-block;

  &__preview,
  &__placeholder {
    width: 120px;
    height: 120px;
    border: 1px dashed var(--ep-border);
    border-radius: $radius-md;
    overflow: hidden;
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;

    &--circle {
      border-radius: 50%;
    }
  }

  &__preview {
    img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .image-upload__mask {
      position: absolute;
      inset: 0;
      background: rgba(0, 0, 0, 0.5);
      color: #fff;
      display: flex;
      align-items: center;
      justify-content: center;
      opacity: 0;
      transition: opacity 0.2s;
    }

    &:hover .image-upload__mask {
      opacity: 1;
    }
  }

  &__placeholder {
    color: var(--ep-text-secondary);
    flex-direction: column;
    gap: $spacing-xs;

    &--circle {
      border-radius: 50%;
    }
  }

  &__remove {
    margin-top: $spacing-xs;
  }
}
</style>
```

- [ ] **Step 2: 在 i18n basic.json 追加 common.upload 文案（若 common.json 未含 upload 节点）**

先读 `EasyProduct.Admin/src/i18n/zh-CN/common.json` 确认是否有 `upload` 节点。若无，在 zh-CN/common.json 顶层追加：
```json
"upload": {
  "clickToUpload": "点击上传",
  "reupload": "重新上传",
  "remove": "移除",
  "oversize": "文件大小不能超过 {size}MB"
}
```
en-US/common.json 对应追加：
```json
"upload": {
  "clickToUpload": "Click to upload",
  "reupload": "Re-upload",
  "remove": "Remove",
  "oversize": "File size cannot exceed {size}MB"
}
```
> 注：i18n key 在 common.json 而非 basic.json，因为上传是通用文案。用 `t('common.upload.xxx')`。若 common.json 已有 upload 节点则跳过。

- [ ] **Step 3: 类型检查 + lint + i18n 检查**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check && pnpm lint && pnpm check:i18n
```
Expected: 三项全绿。

- [ ] **Step 4: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/components/common/ImageUpload.vue EasyProduct.Admin/src/i18n/zh-CN/common.json EasyProduct.Admin/src/i18n/en-US/common.json && git commit -m "feat(admin): 添加 ImageUpload 通用图片上传组件"
```

---

### Task 3: RichTextEditor 通用组件（首次引入 WangEditor）

**Files:**
- Modify: `EasyProduct.Admin/package.json`（加依赖）
- Create: `EasyProduct.Admin/src/components/common/RichTextEditor.vue`

- [ ] **Step 1: 安装 WangEditor**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm add @wangeditor/editor @wangeditor/editor-for-vue@next
```
Expected: 两个包写入 dependencies。依赖理由（提交说明里写）：公告正文需图文排版，WangEditor 为 F2-0 蓝图指定的首个富文本实现，Vue3 官方适配。

- [ ] **Step 2: 创建 RichTextEditor.vue**

创建 `EasyProduct.Admin/src/components/common/RichTextEditor.vue`：

```vue
<!-- src/components/common/RichTextEditor.vue -->
<template>
  <div class="rich-text-editor">
    <Toolbar
      :editor="editorRef"
      :default-config="toolbarConfig"
      :mode="mode"
      class="rich-text-editor__toolbar"
    />
    <Editor
      v-model="valueHtml"
      :default-config="editorConfig"
      :mode="mode"
      :style="{ height }"
      @on-created="handleCreated"
      @on-change="handleChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, shallowRef, onBeforeUnmount, watch } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import '@wangeditor/editor/dist/css/style.css'
import { useLocale } from '@/composables/useLocale'
import { uploadFile } from '@/api/common/file'

interface Props {
  modelValue: string
  height?: string
  placeholder?: string
  disabled?: boolean
  mode?: 'default' | 'simple'
}

const props = withDefaults(defineProps<Props>(), {
  height: '300px',
  placeholder: '',
  disabled: false,
  mode: 'default',
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const { t } = useLocale()

const editorRef = shallowRef()
const valueHtml = ref<string>(props.modelValue)

watch(
  () => props.modelValue,
  (val) => {
    if (val !== valueHtml.value) {
      valueHtml.value = val
    }
  },
)

const toolbarConfig = {
  excludeKeys: ['group-video', 'fullScreen'],
}

const editorConfig = {
  placeholder: props.placeholder || t('common.richText.placeholder'),
  readOnly: props.disabled,
  MENU_CONF: {
    uploadImage: {
      async customUpload(file: File, insertFn: (url: string) => void) {
        try {
          const result = await uploadFile(file)
          insertFn(result.url)
        } catch {
          // 错误已由拦截器处理
        }
      },
    },
  },
}

const handleCreated = (editor: unknown) => {
  editorRef.value = editor
}

const handleChange = (editor: { getHtml: () => string }) => {
  emit('update:modelValue', editor.getHtml())
}

onBeforeUnmount(() => {
  const editor = editorRef.value as { destroy: () => void } | undefined
  editor?.destroy()
})
</script>

<style scoped lang="scss">
.rich-text-editor {
  border: 1px solid var(--ep-border);
  border-radius: $radius-md;
  overflow: hidden;

  &__toolbar {
    border-bottom: 1px solid var(--ep-border);
  }
}
</style>
```

- [ ] **Step 3: 在 common.json 追加 richText 占位文案**

zh-CN/common.json 顶层追加：
```json
"richText": { "placeholder": "请输入内容" }
```
en-US/common.json 顶层追加：
```json
"richText": { "placeholder": "Please enter content" }
```

- [ ] **Step 4: 类型检查 + lint + i18n 检查**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check && pnpm lint && pnpm check:i18n
```
Expected: 三项全绿。若 `@wangeditor/editor-for-vue` 无类型声明导致 type-check 报错，在 `EasyProduct.Admin/env.d.ts` 追加 `declare module '@wangeditor/editor-for-vue'`。

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/components/common/RichTextEditor.vue EasyProduct.Admin/src/i18n/zh-CN/common.json EasyProduct.Admin/src/i18n/en-US/common.json EasyProduct.Admin/package.json EasyProduct.Admin/env.d.ts && git commit -m "feat(admin): 添加 RichTextEditor 富文本组件（WangEditor 封装，公告正文用）"
```

---

## 模块1：系统参数（扁平键值列表，内联编辑）

### Task 4: 系统参数类型定义

**Files:**
- Modify: `EasyProduct.Admin/src/types/basic.ts`

- [ ] **Step 1: 在 basic.ts 末尾追加系统参数类型**

在 `EasyProduct.Admin/src/types/basic.ts` 文件末尾追加：

```typescript
// ==================== 系统参数 ====================

/** 系统参数值类型（驱动编辑控件） */
export type SystemConfigType = 'string' | 'number' | 'boolean'

/** 系统参数实体 */
export interface SystemConfig {
  id: string // GUID
  key: string // 参数键（唯一）
  label: string // 参数名称
  value: string // 参数值
  type: SystemConfigType // 值类型
  remark: string // 备注
  createdAt: string // 创建时间（ISO 8601）
  updatedAt: string // 更新时间（ISO 8601）
}

/** 系统参数查询参数 */
export interface SystemConfigQuery extends PageQuery {
  key?: string
  label?: string
}

/** 系统参数创建参数 */
export interface SystemConfigCreateParams {
  key: string
  label: string
  value: string
  type: SystemConfigType
  remark: string
}
```

> 注：`PageQuery` 已在文件顶部 `import type { PageQuery } from './api'`，无需重复导入。

- [ ] **Step 2: 类型检查**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check
```
Expected: 零错误。

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/types/basic.ts && git commit -m "feat(admin): 添加系统参数类型定义"
```

---

### Task 5: 系统参数 mock data + route

**Files:**
- Create: `mock-server/src/data/admin/config.ts`
- Create: `mock-server/src/routes/admin/config.ts`
- Modify: `mock-server/src/server.ts`

- [ ] **Step 1: 创建系统参数种子数据**

创建 `mock-server/src/data/admin/config.ts`：

```typescript
// src/data/admin/config.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface SystemConfig {
  id: string
  key: string
  label: string
  value: string
  type: 'string' | 'number' | 'boolean'
  remark: string
  createdAt: string
  updatedAt: string
}

const now = () => isoTime()

export const SYSTEM_CONFIGS: SystemConfig[] = [
  {
    id: guid(),
    key: 'site_name',
    label: '站点名称',
    value: 'EasyProduct 管理系统',
    type: 'string',
    remark: '浏览器标签与登录页标题',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'site_icp',
    label: '备案号',
    value: '粤ICP备00000000号',
    type: 'string',
    remark: '官网底部备案号',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'upload_max_size',
    label: '上传大小上限(MB)',
    value: '2',
    type: 'number',
    remark: '单文件上传大小上限',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'register_enabled',
    label: '允许会员注册',
    value: 'true',
    type: 'boolean',
    remark: '小程序会员注册开关',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'customer_service_phone',
    label: '客服电话',
    value: '400-000-0000',
    type: 'string',
    remark: '官网客服联系电话',
    createdAt: now(),
    updatedAt: now(),
  },
]
```

- [ ] **Step 2: 创建系统参数路由**

创建 `mock-server/src/routes/admin/config.ts`：

```typescript
// src/routes/admin/config.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { SYSTEM_CONFIGS } from '../../data/admin/config.js'
import { guid, isoTime } from '../../helpers/id.js'

export const adminConfigRouter = Router()

/** 系统参数列表（分页 + key/label 筛选） */
adminConfigRouter.get('/basic/config/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const key = req.query.key as string | undefined
  const label = req.query.label as string | undefined

  let filtered = SYSTEM_CONFIGS
  if (key) filtered = filtered.filter((c) => c.key.includes(key))
  if (label) filtered = filtered.filter((c) => c.label.includes(label))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

/** 新增系统参数（key 唯一校验） */
adminConfigRouter.post('/basic/config', (req, res) => {
  const { key, label, value, type, remark } = req.body || {}
  if (!key || !label || !type) {
    res.json(fail('参数键、名称、值类型不能为空', 400))
    return
  }
  if (SYSTEM_CONFIGS.some((c) => c.key === key)) {
    res.json(fail('参数键已存在', 400))
    return
  }
  const now = isoTime()
  const item = {
    id: guid(),
    key,
    label,
    value: value ?? '',
    type,
    remark: remark ?? '',
    createdAt: now,
    updatedAt: now,
  }
  SYSTEM_CONFIGS.push(item)
  res.json(ok({ id: item.id }, '创建成功'))
})

/** 编辑系统参数 */
adminConfigRouter.put('/basic/config/:id', (req, res) => {
  const idx = SYSTEM_CONFIGS.findIndex((c) => c.id === req.params.id)
  if (idx === -1) {
    res.json(fail('参数不存在', 404))
    return
  }
  const { label, value, type, remark } = req.body || {}
  const now = isoTime()
  SYSTEM_CONFIGS[idx] = {
    ...SYSTEM_CONFIGS[idx],
    label: label ?? SYSTEM_CONFIGS[idx].label,
    value: value ?? SYSTEM_CONFIGS[idx].value,
    type: type ?? SYSTEM_CONFIGS[idx].type,
    remark: remark ?? SYSTEM_CONFIGS[idx].remark,
    updatedAt: now,
  }
  res.json(ok(null, '更新成功'))
})

/** 删除系统参数 */
adminConfigRouter.delete('/basic/config/:id', (req, res) => {
  const idx = SYSTEM_CONFIGS.findIndex((c) => c.id === req.params.id)
  if (idx === -1) {
    res.json(fail('参数不存在', 404))
    return
  }
  SYSTEM_CONFIGS.splice(idx, 1)
  res.json(ok(null, '删除成功'))
})

/** 批量删除系统参数 */
adminConfigRouter.post('/basic/config/batch-delete', (req, res) => {
  const ids: string[] = req.body?.ids || []
  if (!Array.isArray(ids) || ids.length === 0) {
    res.json(fail('ids 不能为空', 400))
    return
  }
  for (const id of ids) {
    const idx = SYSTEM_CONFIGS.findIndex((c) => c.id === id)
    if (idx !== -1) SYSTEM_CONFIGS.splice(idx, 1)
  }
  res.json(ok(null, '删除成功'))
})
```

- [ ] **Step 3: 在 server.ts 注册 config 路由**

修改 `mock-server/src/server.ts`：在 `import { adminFileRouter } from './routes/admin/file.js'` 后追加：
```typescript
import { adminConfigRouter } from './routes/admin/config.js'
```
在 `app.use('/api/admin', ...)` 行的 `adminFileRouter` 后追加 `adminConfigRouter`。

- [ ] **Step 4: 启动 mock 验证系统参数接口**

Run:
```bash
cd D:/4-MyProject/EasyProduct/mock-server && pnpm dev
```
另终端：
```bash
TOKEN=$(curl -s -X POST http://localhost:7700/api/admin/auth/login -H "Content-Type: application/json" -d '{"userName":"admin","password":"admin123"}' | node -pe 'JSON.parse(require("fs").readFileSync(0)).data.accessToken')
curl -s "http://localhost:7700/api/admin/basic/config/list?pageIndex=1&pageSize=10" -H "Authorization: Bearer $TOKEN" | head -c 300
```
Expected: 返回 `code:200`，data.list 含 5 条参数。

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/src/data/admin/config.ts mock-server/src/routes/admin/config.ts mock-server/src/server.ts && git commit -m "feat(mock): 添加系统参数数据与路由（CRUD + 批量删除 + key 唯一校验）"
```

---

### Task 6: 系统参数前端 API

**Files:**
- Create: `EasyProduct.Admin/src/api/basic/config.ts`

- [ ] **Step 1: 创建 config.ts API**

创建 `EasyProduct.Admin/src/api/basic/config.ts`：

```typescript
// src/api/basic/config.ts
import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { SystemConfig, SystemConfigQuery, SystemConfigCreateParams } from '@/types/basic'

/** 系统参数列表（分页） */
export const getConfigList = (params: SystemConfigQuery) =>
  get<PageResult<SystemConfig>>('/api/admin/basic/config/list', params)

/** 新增系统参数 */
export const createConfig = (data: SystemConfigCreateParams) =>
  post<{ id: string }>('/api/admin/basic/config', data)

/** 编辑系统参数 */
export const updateConfig = (id: string, data: Partial<SystemConfigCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/config/${id}`, data)

/** 删除系统参数 */
export const deleteConfig = (id: string) =>
  del<null>(`/api/admin/basic/config/${id}`)

/** 批量删除系统参数 */
export const deleteConfigBatch = (ids: string[]) =>
  post<null>('/api/admin/basic/config/batch-delete', { ids })
```

- [ ] **Step 2: 类型检查**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check
```
Expected: 零错误。

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/api/basic/config.ts && git commit -m "feat(admin): 添加系统参数 API"
```

---

### Task 7: 系统参数页面（内联编辑）+ i18n + 路由

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/config/index.vue`
- Create: `EasyProduct.Admin/src/views/basic/config/components/ConfigFormDialog.vue`
- Modify: `EasyProduct.Admin/src/router/modules/basic.ts`
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/basic.json` + `en-US/basic.json`
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/menu.json` + `en-US/menu.json`

- [ ] **Step 1: 在 basic.json 追加 config 文案**

zh-CN/basic.json 在顶层 `dict` 节点后追加（注意逗号）：
```json
,
"config": {
  "title": "系统参数",
  "key": "参数键",
  "label": "参数名称",
  "value": "参数值",
  "type": "值类型",
  "remark": "备注",
  "createdAt": "创建时间",
  "updatedAt": "更新时间",
  "add": "新增参数",
  "edit": "编辑",
  "save": "保存",
  "cancel": "取消",
  "inlineEdit": "内联编辑",
  "typeString": "文本",
  "typeNumber": "数字",
  "typeBoolean": "布尔",
  "form": {
    "addTitle": "新增参数",
    "editTitle": "编辑参数",
    "key": "参数键",
    "keyPlaceholder": "请输入参数键",
    "keyRequired": "请输入参数键",
    "keyFormat": "参数键只能包含字母、数字、下划线，且以字母开头",
    "label": "参数名称",
    "labelPlaceholder": "请输入参数名称",
    "labelRequired": "请输入参数名称",
    "value": "参数值",
    "valuePlaceholder": "请输入参数值",
    "valueRequired": "请输入参数值",
    "type": "值类型",
    "typeRequired": "请选择值类型",
    "remark": "备注",
    "remarkPlaceholder": "请输入备注"
  },
  "message": {
    "addSuccess": "新增成功",
    "updateSuccess": "更新成功",
    "deleteSuccess": "删除成功",
    "deleteConfirm": "确认删除参数【{label}】吗？",
    "batchDeleteConfirm": "确认删除选中的 {count} 条参数吗？",
    "keyExists": "参数键已存在"
  }
}
```
en-US/basic.json 对应追加（英文）：
```json
,
"config": {
  "title": "System Config",
  "key": "Key",
  "label": "Label",
  "value": "Value",
  "type": "Type",
  "remark": "Remark",
  "createdAt": "Created At",
  "updatedAt": "Updated At",
  "add": "Add Config",
  "edit": "Edit",
  "save": "Save",
  "cancel": "Cancel",
  "inlineEdit": "Inline Edit",
  "typeString": "String",
  "typeNumber": "Number",
  "typeBoolean": "Boolean",
  "form": {
    "addTitle": "Add Config",
    "editTitle": "Edit Config",
    "key": "Key",
    "keyPlaceholder": "Enter key",
    "keyRequired": "Please enter key",
    "keyFormat": "Key can only contain letters, numbers, underscores, and must start with a letter",
    "label": "Label",
    "labelPlaceholder": "Enter label",
    "labelRequired": "Please enter label",
    "value": "Value",
    "valuePlaceholder": "Enter value",
    "valueRequired": "Please enter value",
    "type": "Type",
    "typeRequired": "Please select type",
    "remark": "Remark",
    "remarkPlaceholder": "Enter remark"
  },
  "message": {
    "addSuccess": "Added successfully",
    "updateSuccess": "Updated successfully",
    "deleteSuccess": "Deleted successfully",
    "deleteConfirm": "Delete config [{label}]?",
    "batchDeleteConfirm": "Delete {count} selected configs?",
    "keyExists": "Config key already exists"
  }
}
```

- [ ] **Step 2: 在 menu.json 追加 config 菜单 key**

zh-CN/menu.json 追加：
```json
, "basicConfig": "系统参数"
```
en-US/menu.json 追加：
```json
, "basicConfig": "System Config"
```

- [ ] **Step 3: 创建 ConfigFormDialog.vue（新增用弹窗）**

创建 `EasyProduct.Admin/src/views/basic/config/components/ConfigFormDialog.vue`：

```vue
<!-- src/views/basic/config/components/ConfigFormDialog.vue -->
<template>
  <el-dialog
    :model-value="modelValue"
    :title="t('basic.config.form.addTitle')"
    width="500px"
    @update:model-value="emit('update:modelValue', $event)"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item :label="t('basic.config.form.key')" prop="key">
        <el-input
          v-model="formData.key"
          :placeholder="t('basic.config.form.keyPlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('basic.config.form.label')" prop="label">
        <el-input
          v-model="formData.label"
          :placeholder="t('basic.config.form.labelPlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('basic.config.form.value')" prop="value">
        <el-input
          v-model="formData.value"
          :placeholder="t('basic.config.form.valuePlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('basic.config.form.type')" prop="type">
        <el-radio-group v-model="formData.type">
          <el-radio value="string">{{ t('basic.config.typeString') }}</el-radio>
          <el-radio value="number">{{ t('basic.config.typeNumber') }}</el-radio>
          <el-radio value="boolean">{{ t('basic.config.typeBoolean') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item :label="t('basic.config.form.remark')" prop="remark">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="3"
          :placeholder="t('basic.config.form.remarkPlaceholder')"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="emit('update:modelValue', false)">
        {{ t('basic.config.cancel') }}
      </el-button>
      <el-button type="primary" :loading="saving" @click="handleSave">
        {{ t('common.button.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { createConfig } from '@/api/basic/config'
import type { SystemConfigCreateParams } from '@/types/basic'

const props = defineProps<{ modelValue: boolean }>()
const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()
const formRef = ref<FormInstance | null>(null)
const saving = ref(false)

const formData = reactive<SystemConfigCreateParams>({
  key: '',
  label: '',
  value: '',
  type: 'string',
  remark: '',
})

const formRules: FormRules = {
  key: [
    { required: true, message: () => t('basic.config.form.keyRequired'), trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9_]*$/, message: () => t('basic.config.form.keyFormat'), trigger: 'blur' },
  ],
  label: [{ required: true, message: () => t('basic.config.form.labelRequired'), trigger: 'blur' }],
  value: [{ required: true, message: () => t('basic.config.form.valueRequired'), trigger: 'blur' }],
  type: [{ required: true, message: () => t('basic.config.form.typeRequired'), trigger: 'change' }],
}

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      formData.key = ''
      formData.label = ''
      formData.value = ''
      formData.type = 'string'
      formData.remark = ''
    }
  },
)

const handleClose = () => {
  formRef.value?.resetFields()
}

const handleSave = async () => {
  if (!formRef.value) return
  try {
    await formRef.value.validate()
  } catch {
    return
  }
  saving.value = true
  try {
    await createConfig(formData)
    ElMessage.success(t('basic.config.message.addSuccess'))
    emit('update:modelValue', false)
    emit('success')
  } catch {
    // 错误已由拦截器处理
  } finally {
    saving.value = false
  }
}
</script>
```

- [ ] **Step 4: 创建系统参数主页面（内联编辑）**

创建 `EasyProduct.Admin/src/views/basic/config/index.vue`：

```vue
<!-- src/views/basic/config/index.vue -->
<template>
  <div class="config-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      :loading="table.loading.value"
      @search="onSearch"
      @reset="onReset"
    >
      <template #toolbar>
        <el-button v-permission="['basic:config:add']" type="primary" @click="handleAdd">
          {{ t('basic.config.add') }}
        </el-button>
        <el-button
          v-permission="['basic:config:delete']"
          type="danger"
          :disabled="!selectedRows.length"
          @click="handleBatchDelete"
        >
          {{ t('common.batchDelete') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <BaseTable
      :loading="table.loading.value"
      :data="table.list.value"
      :total="table.total.value"
      :current-page="table.query.pageIndex"
      :page-size="table.query.pageSize"
      @page-change="table.handlePageChange"
      @size-change="handleSizeChange"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="50" />
      <el-table-column :label="t('basic.config.key')" prop="key" min-width="140" />
      <el-table-column :label="t('basic.config.label')" prop="label" min-width="140" />
      <el-table-column :label="t('basic.config.value')" min-width="160">
        <template #default="{ row }">
          <!-- 内联编辑模式 -->
          <template v-if="editingId === row.id">
            <el-input
              v-if="row.type === 'string'"
              v-model="editValue"
              size="small"
            />
            <el-input-number
              v-else-if="row.type === 'number'"
              v-model="numberEditValue"
              size="small"
              controls-position="right"
            />
            <el-switch
              v-else
              v-model="booleanEditValue"
            />
          </template>
          <!-- 只读模式 -->
          <span v-else>{{ displayValue(row) }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('basic.config.type')" prop="type" width="100" align="center">
        <template #default="{ row }">
          <el-tag size="small" type="info">{{ typeLabel(row.type) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('basic.config.remark')" prop="remark" min-width="160" />
      <el-table-column :label="t('common.actions')" width="160" align="center" fixed="right">
        <template #default="{ row }">
          <template v-if="editingId === row.id">
            <el-button link type="primary" size="small" @click="handleSaveInline(row)">
              {{ t('basic.config.save') }}
            </el-button>
            <el-button link size="small" @click="handleCancelInline">
              {{ t('basic.config.cancel') }}
            </el-button>
          </template>
          <template v-else>
            <el-button
              v-permission="['basic:config:edit']"
              link
              type="primary"
              size="small"
              @click="handleEditInline(row)"
            >
              {{ t('basic.config.inlineEdit') }}
            </el-button>
            <el-button
              v-permission="['basic:config:delete']"
              link
              type="danger"
              size="small"
              @click="handleDelete(row)"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </template>
      </el-table-column>
    </BaseTable>

    <ConfigFormDialog v-model="dialogVisible" @success="table.reload" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import { useLocale } from '@/composables/useLocale'
import { useSearch } from '@/composables/useSearch'
import { useTable } from '@/composables/useTable'
import { getConfigList, updateConfig, deleteConfig, deleteConfigBatch } from '@/api/basic/config'
import type { SystemConfig } from '@/types/basic'
import type { SearchField } from '@/types/search'
import ConfigFormDialog from './components/ConfigFormDialog.vue'

const { t } = useLocale()

// 搜索：复用 useSearch，搜索字段以 i18n key 配置（顶层只调用一次，复用 getSearchParams）
const { searchModel, getSearchParams } = useSearch<{ key?: string; label?: string }>({
  defaultModel: { key: '', label: '' },
})

const searchFields: SearchField[] = [
  { prop: 'key', label: 'basic.config.key', type: 'input', placeholder: 'basic.config.form.keyPlaceholder' },
  { prop: 'label', label: 'basic.config.label', type: 'input', placeholder: 'basic.config.form.labelPlaceholder' },
]

// 列表：useTable
const table = useTable<SystemConfig>((params) => getConfigList(params as never))

const onSearch = (): void => {
  Object.assign(table.query, getSearchParams())
  void table.handleSearch()
}

const onReset = (): void => {
  searchModel.key = ''
  searchModel.label = ''
  Object.assign(table.query, { key: undefined, label: undefined })
  void table.handleSearch()
}

const handleSizeChange = (size: number): void => {
  table.query.pageSize = size
  table.query.pageIndex = 1
  void table.reload()
}

// 多选
const selectedRows = ref<SystemConfig[]>([])
const handleSelectionChange = (rows: unknown[]): void => {
  selectedRows.value = rows as SystemConfig[]
}

// 内联编辑
const editingId = ref<string>('')
const editValue = ref<string>('')
const numberEditValue = ref<number>(0)
const booleanEditValue = ref<boolean>(false)

const displayValue = (row: SystemConfig): string => {
  if (row.type === 'boolean') {
    return row.value === 'true' ? t('common.status.enabled') : t('common.status.disabled')
  }
  return row.value
}

const typeLabel = (type: SystemConfig['type']): string => {
  if (type === 'string') return t('basic.config.typeString')
  if (type === 'number') return t('basic.config.typeNumber')
  return t('basic.config.typeBoolean')
}

const handleEditInline = (row: SystemConfig): void => {
  editingId.value = row.id
  editValue.value = row.value
  numberEditValue.value = Number(row.value) || 0
  booleanEditValue.value = row.value === 'true'
}

const handleCancelInline = (): void => {
  editingId.value = ''
}

const handleSaveInline = async (row: SystemConfig): Promise<void> => {
  const newValue =
    row.type === 'string'
      ? editValue.value
      : row.type === 'number'
        ? String(numberEditValue.value)
        : booleanEditValue.value
        ? 'true'
        : 'false'
  try {
    await updateConfig(row.id, { value: newValue })
    ElMessage.success(t('basic.config.message.updateSuccess'))
    editingId.value = ''
    await table.reload()
  } catch {
    // 错误已由拦截器处理
  }
}

// 新增
const dialogVisible = ref(false)
const handleAdd = (): void => {
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: SystemConfig): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.config.message.deleteConfirm', { label: row.label }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      },
    )
    await deleteConfig(row.id)
    ElMessage.success(t('basic.config.message.deleteSuccess'))
    await table.reload()
  } catch {
    // 用户取消或请求失败
  }
}

const handleBatchDelete = async (): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.config.message.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      },
    )
    await deleteConfigBatch(selectedRows.value.map((r) => r.id))
    ElMessage.success(t('basic.config.message.deleteSuccess'))
    selectedRows.value = []
    await table.reload()
  } catch {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.config-page {
  padding: $spacing-md;
}
</style>
```

- [ ] **Step 5: 在 basic.ts 路由追加 config 子路由**

修改 `EasyProduct.Admin/src/router/modules/basic.ts`：将第 40 行注释 `// 批次 3~4 的路由后续追加（announcement/setting/profile）` 替换为实际路由，在 `dict` 子路由后追加：

```typescript
      },
      {
        path: 'config',
        name: 'basic-config',
        component: () => import('@/views/basic/config/index.vue'),
        meta: { title: 'menu.basicConfig', icon: 'Tools' }
      }
```

确保 `dict` 路由对象末尾逗号正确。最终 dict 节点后接 config 节点。

- [ ] **Step 6: 检查 common.json 是否含 batchDelete/tips/button 等通用 key**

读 `EasyProduct.Admin/src/i18n/zh-CN/common.json`，确认存在 `common.batchDelete`、`common.tips`、`common.button.confirm/cancel`、`common.delete`、`common.status.enabled/disabled`、`common.search`、`common.reset`、`common.inputPlaceholder`、`common.selectPlaceholder`、`common.startDate/endDate`。若缺 `common.batchDelete`，zh 追加 `"batchDelete": "批量删除"`，en 追加 `"batchDelete": "Batch Delete"`。

- [ ] **Step 7: 全量检查 + 构建**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check && pnpm lint && pnpm check:i18n && pnpm build
```
Expected: 四项全绿。

- [ ] **Step 8: 手测页面**

启动 mock + admin，登录后访问 `/basic/config`，验证：列表展示、key/label 搜索、内联编辑（string/number/boolean 三类型控件）、新增弹窗、删除、批量删除、中英切换。

- [ ] **Step 9: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/config EasyProduct.Admin/src/router/modules/basic.ts EasyProduct.Admin/src/i18n/zh-CN/basic.json EasyProduct.Admin/src/i18n/en-US/basic.json EasyProduct.Admin/src/i18n/zh-CN/menu.json EasyProduct.Admin/src/i18n/en-US/menu.json EasyProduct.Admin/src/i18n/zh-CN/common.json EasyProduct.Admin/src/i18n/en-US/common.json && git commit -m "feat(admin): 创建系统参数页面（扁平键值列表 + 内联编辑）"
```

---

## 模块2：工作台轻量首页

### Task 8: 工作台类型 + mock route + 前端 API

**Files:**
- Modify: `EasyProduct.Admin/src/types/basic.ts`
- Create: `mock-server/src/routes/admin/desktop.ts`
- Modify: `mock-server/src/server.ts`
- Create: `EasyProduct.Admin/src/api/basic/desktop.ts`

- [ ] **Step 1: 在 basic.ts 追加工作台类型**

在 `EasyProduct.Admin/src/types/basic.ts` 末尾追加：

```typescript
// ==================== 工作台 ====================

/** 工作台统计卡片数据 */
export interface DesktopOverview {
  userCount: number
  orderCount: number
  salesAmount: number
  todayVisits: number
  recentOrders: DesktopRecentOrder[]
  todos: DesktopTodo[]
}

/** 最近订单（工作台用） */
export interface DesktopRecentOrder {
  id: string
  amount: number
  status: string
  customerName: string
  createTime: string
}

/** 待办（工作台用） */
export interface DesktopTodo {
  id: string
  title: string
  type: string
  createTime: string
}
```

- [ ] **Step 2: 创建工作台 mock 路由**

创建 `mock-server/src/routes/admin/desktop.ts`：

```typescript
// src/routes/admin/desktop.ts
import { Router } from 'express'
import Mock from 'mockjs'
import { ok } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'

export const adminDesktopRouter = Router()

/** 工作台概览（聚合统计 + 最近订单 + 待办，数值 mock 随机） */
adminDesktopRouter.get('/basic/desktop/overview', (_req, res) => {
  const recentOrders = Mock.mock({
    'list|5': [{
      id: '@guid',
      amount: '@float(100, 9999, 2, 2)',
      status: "@pick(['pending', 'paid', 'shipped', 'completed'])",
      customerName: '@cname',
      createTime: '@datetime("yyyy-MM-dd HH:mm:ss")',
    }],
  }).list.map((o: Record<string, unknown>) => ({ ...o, id: guid() }))

  const todos = Mock.mock({
    'list|4': [{
      id: '@guid',
      title: '@ctitle(8, 16)',
      type: "@pick(['order', 'refund', 'audit', 'stock'])",
      createTime: '@datetime("yyyy-MM-dd HH:mm:ss")',
    }],
  }).list.map((t: Record<string, unknown>) => ({ ...t, id: guid(), createTime: isoTime() }))

  res.json(ok({
    userCount: Mock.mock('@integer(100, 9999)'),
    orderCount: Mock.mock('@integer(100, 9999)'),
    salesAmount: Mock.mock('@float(10000, 999999, 2, 2)'),
    todayVisits: Mock.mock('@integer(50, 2000)'),
    recentOrders,
    todos,
  }))
})
```

- [ ] **Step 3: 在 server.ts 注册 desktop 路由**

修改 `mock-server/src/server.ts`：在 `import { adminConfigRouter } from './routes/admin/config.js'` 后追加：
```typescript
import { adminDesktopRouter } from './routes/admin/desktop.js'
```
在 `app.use('/api/admin', ...)` 行的 `adminConfigRouter` 后追加 `adminDesktopRouter`。

- [ ] **Step 4: 创建前端 desktop API**

创建 `EasyProduct.Admin/src/api/basic/desktop.ts`：

```typescript
// src/api/basic/desktop.ts
import { get } from '@/utils/request'
import type { DesktopOverview } from '@/types/basic'

/** 工作台概览 */
export const getDesktopOverview = () =>
  get<DesktopOverview>('/api/admin/basic/desktop/overview')
```

- [ ] **Step 5: 启动 mock 验证**

Run:
```bash
cd D:/4-MyProject/EasyProduct/mock-server && pnpm dev
```
另终端：
```bash
TOKEN=$(curl -s -X POST http://localhost:7700/api/admin/auth/login -H "Content-Type: application/json" -d '{"userName":"admin","password":"admin123"}' | node -pe 'JSON.parse(require("fs").readFileSync(0)).data.accessToken')
curl -s "http://localhost:7700/api/admin/basic/desktop/overview" -H "Authorization: Bearer $TOKEN" | head -c 300
```
Expected: `code:200`，data 含 userCount/orderCount/salesAmount/todayVisits/recentOrders/todos。

- [ ] **Step 6: 类型检查 + Commit**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/types/basic.ts EasyProduct.Admin/src/api/basic/desktop.ts mock-server/src/routes/admin/desktop.ts mock-server/src/server.ts && git commit -m "feat(mock): 添加工作台概览路由 + 前端 desktop API 与类型"
```

---

### Task 9: 工作台轻量首页（重写 desktop 页面）

**Files:**
- Modify: `EasyProduct.Admin/src/views/desktop/index.vue`
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/basic.json` + `en-US/basic.json`（追加 desktop 文案，因 desktop 文案放 basic.json 的 desktop 节点；若已有则合并）

- [ ] **Step 1: 在 basic.json 追加 desktop 文案**

> desktop 文案放 `basic.json` 的 `desktop` 节点（与 menu.desktop 区分；menu.desktop 是侧边栏标题，basic.desktop 是页面内文案）。

zh-CN/basic.json 在 `config` 节点后追加：
```json
,
"desktop": {
  "welcome": "欢迎回来",
  "role": "当前角色",
  "today": "今日",
  "stat": {
    "userCount": "用户总数",
    "orderCount": "订单总数",
    "salesAmount": "销售总额",
    "todayVisits": "今日访客"
  },
  "quickEntry": "快捷入口",
  "recentOrders": "最近订单",
  "todos": "待办事项",
  "order": {
    "amount": "金额",
    "customer": "客户",
    "status": "状态",
    "createTime": "时间"
  },
  "todo": {
    "title": "标题",
    "type": "类型",
    "createTime": "时间"
  }
}
```
en-US/basic.json 对应追加英文：
```json
,
"desktop": {
  "welcome": "Welcome back",
  "role": "Role",
  "today": "Today",
  "stat": {
    "userCount": "Users",
    "orderCount": "Orders",
    "salesAmount": "Sales",
    "todayVisits": "Today Visits"
  },
  "quickEntry": "Quick Entry",
  "recentOrders": "Recent Orders",
  "todos": "Todos",
  "order": {
    "amount": "Amount",
    "customer": "Customer",
    "status": "Status",
    "createTime": "Time"
  },
  "todo": {
    "title": "Title",
    "type": "Type",
    "createTime": "Time"
  }
}
```

- [ ] **Step 2: 重写 desktop/index.vue**

将 `EasyProduct.Admin/src/views/desktop/index.vue` 整文件替换为：

```vue
<!-- src/views/desktop/index.vue -->
<template>
  <div class="desktop-page">
    <!-- 欢迎区 -->
    <el-card shadow="never" class="desktop-page__welcome">
      <div class="welcome-info">
        <h2>{{ t('basic.desktop.welcome') }}，{{ userStore.realName }}</h2>
        <p class="welcome-sub">
          {{ t('basic.desktop.today') }}：{{ today }}
        </p>
      </div>
    </el-card>

    <!-- 统计卡片 -->
    <el-row :gutter="$spacing-md" class="desktop-page__stats">
      <el-col v-for="card in statCards" :key="card.key" :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-card__value">{{ card.value }}</div>
          <div class="stat-card__label">{{ t(card.label) }}</div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="$spacing-md">
      <!-- 最近订单 -->
      <el-col :span="14">
        <el-card shadow="never" class="desktop-page__panel">
          <template #header>{{ t('basic.desktop.recentOrders') }}</template>
          <el-table :data="overview.recentOrders" border size="small">
            <el-table-column prop="customerName" :label="t('basic.desktop.order.customer')" min-width="120" />
            <el-table-column prop="amount" :label="t('basic.desktop.order.amount')" width="120" align="right">
              <template #default="{ row }">¥{{ row.amount }}</template>
            </el-table-column>
            <el-table-column prop="status" :label="t('basic.desktop.order.status')" width="100" align="center">
              <template #default="{ row }">
                <el-tag size="small">{{ row.status }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="createTime" :label="t('basic.desktop.order.createTime')" width="160" />
          </el-table>
        </el-card>
      </el-col>

      <!-- 待办 -->
      <el-col :span="10">
        <el-card shadow="never" class="desktop-page__panel">
          <template #header>{{ t('basic.desktop.todos') }}</template>
          <el-timeline>
            <el-timeline-item
              v-for="todo in overview.todos"
              :key="todo.id"
              :timestamp="todo.createTime"
              placement="top"
            >
              <el-tag size="small" type="info" class="todo-type">{{ todo.type }}</el-tag>
              <span class="todo-title">{{ todo.title }}</span>
            </el-timeline-item>
          </el-timeline>
        </el-card>
      </el-col>
    </el-row>

    <!-- 快捷入口 -->
    <el-card shadow="never" class="desktop-page__quick">
      <template #header>{{ t('basic.desktop.quickEntry') }}</template>
      <el-row :gutter="$spacing-md">
        <el-col v-for="m in QUICK_ENTRIES" :key="m.path" :span="3">
          <el-button class="quick-btn" @click="router.push(m.path)">
            {{ t(m.titleKey) }}
          </el-button>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { useLocale } from '@/composables/useLocale'
import { useUserStore } from '@/stores/user'
import { getDesktopOverview } from '@/api/basic/desktop'
import type { DesktopOverview } from '@/types/basic'

const { t } = useLocale()
const router = useRouter()
const userStore = useUserStore()

const today = dayjs().format('YYYY-MM-DD HH:mm')

const overview = ref<DesktopOverview>({
  userCount: 0,
  orderCount: 0,
  salesAmount: 0,
  todayVisits: 0,
  recentOrders: [],
  todos: [],
})

const QUICK_ENTRIES = [
  { path: '/basic/user', titleKey: 'menu.basicUser' },
  { path: '/basic/role', titleKey: 'menu.basicRole' },
  { path: '/basic/menu', titleKey: 'menu.basicMenu' },
  { path: '/basic/dept', titleKey: 'menu.basicDept' },
  { path: '/basic/dict', titleKey: 'menu.basicDict' },
  { path: '/basic/config', titleKey: 'menu.basicConfig' },
  { path: '/profile', titleKey: 'menu.profile' },
] as const

const statCards = computed(() => [
  { key: 'user', value: overview.value.userCount, label: 'basic.desktop.stat.userCount' },
  { key: 'order', value: overview.value.orderCount, label: 'basic.desktop.stat.orderCount' },
  { key: 'sales', value: `¥${overview.value.salesAmount}`, label: 'basic.desktop.stat.salesAmount' },
  { key: 'visits', value: overview.value.todayVisits, label: 'basic.desktop.stat.todayVisits' },
])

const loadOverview = async (): Promise<void> => {
  try {
    overview.value = await getDesktopOverview()
  } catch {
    // 错误已由拦截器处理
  }
}

onMounted(() => {
  loadOverview()
})
</script>

<style scoped lang="scss">
.desktop-page {
  padding: $spacing-md;

  &__welcome {
    margin-bottom: $spacing-md;

    .welcome-info {
      h2 {
        margin: 0 0 $spacing-xs;
      }

      .welcome-sub {
        color: var(--ep-text-secondary);
        margin: 0;
      }
    }
  }

  &__stats {
    margin-bottom: $spacing-md;

    .stat-card {
      text-align: center;

      &__value {
        font-size: 28px;
        font-weight: 600;
        color: var(--ep-primary);
      }

      &__label {
        color: var(--ep-text-secondary);
        margin-top: $spacing-xs;
      }
    }
  }

  &__panel {
    margin-bottom: $spacing-md;
  }

  &__quick {
    .quick-btn {
      width: 100%;
    }
  }

  .todo-type {
    margin-right: $spacing-xs;
  }
}
</style>
```

- [ ] **Step 3: 类型检查 + lint + i18n + 构建**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check && pnpm lint && pnpm check:i18n && pnpm build
```
Expected: 四项全绿。

- [ ] **Step 4: 手测**

登录后默认跳 `/desktop`，验证：欢迎区显示 realName + 时间、4 统计卡片有值、最近订单表格、待办时间线、快捷入口跳转。

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/desktop/index.vue EasyProduct.Admin/src/i18n/zh-CN/basic.json EasyProduct.Admin/src/i18n/en-US/basic.json && git commit -m "feat(admin): 重写工作台为轻量首页（统计卡片+最近订单+待办+快捷入口）"
```

---

## 模块3：个人中心

### Task 10: 个人中心类型 + mock route + 前端 API

**Files:**
- Modify: `EasyProduct.Admin/src/types/basic.ts`
- Create: `mock-server/src/routes/admin/profile.ts`
- Modify: `mock-server/src/server.ts`
- Create: `EasyProduct.Admin/src/api/basic/profile.ts`

- [ ] **Step 1: 在 basic.ts 追加个人中心类型**

在 `EasyProduct.Admin/src/types/basic.ts` 末尾追加：

```typescript
// ==================== 个人中心 ====================

/** 当前用户信息（个人中心用） */
export interface ProfileInfo {
  id: string
  userName: string
  realName: string
  phone?: string
  email?: string
  avatar?: string
  roles: string[]
  deptName?: string
  lastLoginTime?: string
}

/** 个人中心基本信息更新参数 */
export interface ProfileUpdateParams {
  realName: string
  phone?: string
  email?: string
  avatar?: string
}

/** 修改密码参数 */
export interface ChangePwdParams {
  oldPassword: string
  newPassword: string
}
```

- [ ] **Step 2: 创建个人中心 mock 路由**

> mock 阶段无真实 token 解析，固定「当前用户」为 admin 账号（ADMIN_USERS[0]），改密校验旧密码 = admin123。需引入 ADMIN_USERS。

创建 `mock-server/src/routes/admin/profile.ts`：

```typescript
// src/routes/admin/profile.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { ADMIN_USERS } from '../../data/admin/basic.js'
import { isoTime } from '../../helpers/id.js'

export const adminProfileRouter = Router()

/** 获取当前用户信息（mock 固定返回 admin 账号） */
adminProfileRouter.get('/basic/profile', (_req, res) => {
  const admin = ADMIN_USERS[0]
  res.json(ok({
    id: admin.id,
    userName: admin.userName,
    realName: admin.realName,
    phone: '13800138000',
    email: 'admin@company.com',
    avatar: '',
    roles: ['super'],
    deptName: '总公司',
    lastLoginTime: isoTime(),
  }))
})

/** 更新当前用户基本信息 */
adminProfileRouter.put('/basic/profile', (req, res) => {
  const admin = ADMIN_USERS[0]
  const { realName, phone, email, avatar } = req.body || {}
  if (realName) admin.realName = realName
  // phone/email/avatar 在 mock 阶段不持久化到 ADMIN_USERS（无对应字段），仅返回成功
  res.json(ok(null, '更新成功'))
})

/** 修改密码（校验旧密码 = admin123） */
adminProfileRouter.put('/basic/profile/password', (req, res) => {
  const { oldPassword, newPassword } = req.body || {}
  if (!oldPassword || !newPassword) {
    res.json(fail('原密码和新密码不能为空', 400))
    return
  }
  if (oldPassword !== 'admin123') {
    res.json(fail('原密码不正确', 400))
    return
  }
  if (newPassword.length < 6) {
    res.json(fail('新密码长度不能少于 6 位', 400))
    return
  }
  ADMIN_USERS[0].password = newPassword
  res.json(ok(null, '密码修改成功'))
})
```

> 注：`ADMIN_USERS` 在 `mock-server/src/data/admin/basic.ts`。确认其导出名为 `ADMIN_USERS`（参考 server.ts 已 import 未见，但 auth.ts 用它）。执行时读 `data/admin/basic.ts` 确认导出名，若不同则调整 import。`admin.password` 字段需存在于 ADMIN_USERS 元素（F0 data/basic.ts 定义含 password 字段，确认）。

- [ ] **Step 3: 在 server.ts 注册 profile 路由**

修改 `mock-server/src/server.ts`：在 `import { adminDesktopRouter } from './routes/admin/desktop.js'` 后追加：
```typescript
import { adminProfileRouter } from './routes/admin/profile.js'
```
在 `app.use('/api/admin', ...)` 行的 `adminDesktopRouter` 后追加 `adminProfileRouter`。

- [ ] **Step 4: 创建前端 profile API**

创建 `EasyProduct.Admin/src/api/basic/profile.ts`：

```typescript
// src/api/basic/profile.ts
import { get, put } from '@/utils/request'
import type { ProfileInfo, ProfileUpdateParams, ChangePwdParams } from '@/types/basic'

/** 获取当前用户信息 */
export const getProfile = () =>
  get<ProfileInfo>('/api/admin/basic/profile')

/** 更新当前用户基本信息 */
export const updateProfile = (data: ProfileUpdateParams) =>
  put<null>('/api/admin/basic/profile', data)

/** 修改密码 */
export const changePassword = (data: ChangePwdParams) =>
  put<null>('/api/admin/basic/profile/password', data)
```

- [ ] **Step 5: 启动 mock 验证**

Run:
```bash
cd D:/4-MyProject/EasyProduct/mock-server && pnpm dev
```
另终端：
```bash
TOKEN=$(curl -s -X POST http://localhost:7700/api/admin/auth/login -H "Content-Type: application/json" -d '{"userName":"admin","password":"admin123"}' | node -pe 'JSON.parse(require("fs").readFileSync(0)).data.accessToken')
curl -s "http://localhost:7700/api/admin/basic/profile" -H "Authorization: Bearer $TOKEN" | head -c 200
curl -s -X PUT "http://localhost:7700/api/admin/basic/profile/password" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"oldPassword":"wrong","newPassword":"newpass123"}'
```
Expected: profile 返回 admin 信息；改密旧密码错误返回 `code:400, message:'原密码不正确'`。

- [ ] **Step 6: 类型检查 + Commit**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/types/basic.ts EasyProduct.Admin/src/api/basic/profile.ts mock-server/src/routes/admin/profile.ts mock-server/src/server.ts && git commit -m "feat(mock): 添加个人中心路由（当前用户信息+改密旧密码校验）+ 前端 profile API 与类型"
```

---

### Task 11: 个人中心页面 + topbar 入口 + 路由 + i18n

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/profile/index.vue`
- Modify: `EasyProduct.Admin/src/router/index.ts`
- Modify: `EasyProduct.Admin/src/layouts/components/AppTopbar.vue`
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/basic.json` + `en-US/basic.json`
- Modify: `EasyProduct.Admin/src/i18n/zh-CN/menu.json` + `en-US/menu.json`

- [ ] **Step 1: 在 basic.json 追加 profile 文案**

zh-CN/basic.json 在 `desktop` 节点后追加：
```json
,
"profile": {
  "title": "个人中心",
  "basicInfo": "基本信息",
  "changePassword": "修改密码",
  "userName": "用户名",
  "realName": "真实姓名",
  "phone": "手机号",
  "email": "邮箱",
  "avatar": "头像",
  "role": "角色",
  "dept": "部门",
  "lastLogin": "上次登录",
  "oldPassword": "原密码",
  "newPassword": "新密码",
  "confirmPassword": "确认新密码",
  "oldPasswordPlaceholder": "请输入原密码",
  "newPasswordPlaceholder": "请输入新密码",
  "confirmPasswordPlaceholder": "请再次输入新密码",
  "realNameRequired": "请输入真实姓名",
  "oldPasswordRequired": "请输入原密码",
  "newPasswordRequired": "请输入新密码",
  "newPasswordLength": "新密码长度不能少于 6 位",
  "confirmPasswordRequired": "请再次输入新密码",
  "confirmPasswordMismatch": "两次输入的新密码不一致",
  "message": {
    "updateSuccess": "更新成功",
    "changePwdSuccess": "密码修改成功，请重新登录"
  }
}
```
en-US/basic.json 对应追加英文：
```json
,
"profile": {
  "title": "Profile",
  "basicInfo": "Basic Info",
  "changePassword": "Change Password",
  "userName": "Username",
  "realName": "Real Name",
  "phone": "Phone",
  "email": "Email",
  "avatar": "Avatar",
  "role": "Role",
  "dept": "Department",
  "lastLogin": "Last Login",
  "oldPassword": "Old Password",
  "newPassword": "New Password",
  "confirmPassword": "Confirm Password",
  "oldPasswordPlaceholder": "Enter old password",
  "newPasswordPlaceholder": "Enter new password",
  "confirmPasswordPlaceholder": "Enter new password again",
  "realNameRequired": "Please enter real name",
  "oldPasswordRequired": "Please enter old password",
  "newPasswordRequired": "Please enter new password",
  "newPasswordLength": "New password must be at least 6 characters",
  "confirmPasswordRequired": "Please confirm new password",
  "confirmPasswordMismatch": "Passwords do not match",
  "message": {
    "updateSuccess": "Updated successfully",
    "changePwdSuccess": "Password changed, please sign in again"
  }
}
```

- [ ] **Step 2: 在 menu.json 追加 profile 菜单 key**

zh-CN/menu.json 追加：
```json
, "profile": "个人中心"
```
en-US/menu.json 追加：
```json
, "profile": "Profile"
```

- [ ] **Step 3: 创建个人中心页面**

创建 `EasyProduct.Admin/src/views/basic/profile/index.vue`：

```vue
<!-- src/views/basic/profile/index.vue -->
<template>
  <div class="profile-page">
    <el-card shadow="never" class="profile-page__card">
      <el-row :gutter="$spacing-lg">
        <!-- 左侧：头像 -->
        <el-col :span="6" class="profile-page__avatar">
          <ImageUpload
            v-model="basicForm.avatar"
            circle
          />
          <div class="avatar-name">{{ userStore.realName }}</div>
        </el-col>

        <!-- 右侧：tab -->
        <el-col :span="18">
          <el-tabs v-model="activeTab">
            <!-- 基本信息 -->
            <el-tab-pane :label="t('basic.profile.basicInfo')" name="basic">
              <el-form
                ref="basicFormRef"
                :model="basicForm"
                :rules="basicRules"
                label-width="100px"
              >
                <el-form-item :label="t('basic.profile.userName')">
                  <el-input :model-value="basicForm.userName" disabled />
                </el-form-item>
                <el-form-item :label="t('basic.profile.realName')" prop="realName">
                  <el-input v-model="basicForm.realName" />
                </el-form-item>
                <el-form-item :label="t('basic.profile.phone')" prop="phone">
                  <el-input v-model="basicForm.phone" />
                </el-form-item>
                <el-form-item :label="t('basic.profile.email')" prop="email">
                  <el-input v-model="basicForm.email" />
                </el-form-item>
                <el-form-item>
                  <el-button type="primary" :loading="saving" @click="handleSaveBasic">
                    {{ t('common.button.confirm') }}
                  </el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>

            <!-- 修改密码 -->
            <el-tab-pane :label="t('basic.profile.changePassword')" name="pwd">
              <el-form
                ref="pwdFormRef"
                :model="pwdForm"
                :rules="pwdRules"
                label-width="120px"
              >
                <el-form-item :label="t('basic.profile.oldPassword')" prop="oldPassword">
                  <el-input v-model="pwdForm.oldPassword" type="password" show-password />
                </el-form-item>
                <el-form-item :label="t('basic.profile.newPassword')" prop="newPassword">
                  <el-input v-model="pwdForm.newPassword" type="password" show-password />
                </el-form-item>
                <el-form-item :label="t('basic.profile.confirmPassword')" prop="confirmPassword">
                  <el-input v-model="pwdForm.confirmPassword" type="password" show-password />
                </el-form-item>
                <el-form-item>
                  <el-button type="primary" :loading="pwdSaving" @click="handleChangePwd">
                    {{ t('common.button.confirm') }}
                  </el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
          </el-tabs>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useUserStore } from '@/stores/user'
import { getProfile, updateProfile, changePassword } from '@/api/basic/profile'
import ImageUpload from '@/components/common/ImageUpload.vue'
import type { ProfileInfo, ChangePwdParams } from '@/types/basic'

const { t } = useLocale()
const router = useRouter()
const userStore = useUserStore()

const activeTab = ref<'basic' | 'pwd'>('basic')
const saving = ref(false)
const pwdSaving = ref(false)

const basicFormRef = ref<FormInstance | null>(null)
const pwdFormRef = ref<FormInstance | null>(null)

const basicForm = reactive({
  userName: '',
  realName: '',
  phone: '',
  email: '',
  avatar: '',
})

const pwdForm = reactive<ChangePwdParams & { confirmPassword: string }>({
  oldPassword: '',
  newPassword: '',
  confirmPassword: '',
})

const basicRules: FormRules = {
  realName: [{ required: true, message: () => t('basic.profile.realNameRequired'), trigger: 'blur' }],
}

const pwdRules: FormRules = {
  oldPassword: [{ required: true, message: () => t('basic.profile.oldPasswordRequired'), trigger: 'blur' }],
  newPassword: [
    { required: true, message: () => t('basic.profile.newPasswordRequired'), trigger: 'blur' },
    { min: 6, message: () => t('basic.profile.newPasswordLength'), trigger: 'blur' },
  ],
  confirmPassword: [
    { required: true, message: () => t('basic.profile.confirmPasswordRequired'), trigger: 'blur' },
    {
      validator: (_rule, value, callback) => {
        if (value !== pwdForm.newPassword) {
          callback(new Error(t('basic.profile.confirmPasswordMismatch')))
        } else {
          callback()
        }
      },
      trigger: 'blur',
    },
  ],
}

const loadProfile = async (): Promise<void> => {
  try {
    const data: ProfileInfo = await getProfile()
    basicForm.userName = data.userName
    basicForm.realName = data.realName
    basicForm.phone = data.phone ?? ''
    basicForm.email = data.email ?? ''
    basicForm.avatar = data.avatar ?? ''
  } catch {
    // 错误已由拦截器处理
  }
}

const handleSaveBasic = async (): Promise<void> => {
  if (!basicFormRef.value) return
  try {
    await basicFormRef.value.validate()
  } catch {
    return
  }
  saving.value = true
  try {
    await updateProfile({
      realName: basicForm.realName,
      phone: basicForm.phone,
      email: basicForm.email,
      avatar: basicForm.avatar,
    })
    userStore.realName = basicForm.realName
    ElMessage.success(t('basic.profile.message.updateSuccess'))
  } catch {
    // 错误已由拦截器处理
  } finally {
    saving.value = false
  }
}

const handleChangePwd = async (): Promise<void> => {
  if (!pwdFormRef.value) return
  try {
    await pwdFormRef.value.validate()
  } catch {
    return
  }
  pwdSaving.value = true
  try {
    await changePassword({
      oldPassword: pwdForm.oldPassword,
      newPassword: pwdForm.newPassword,
    })
    ElMessage.success(t('basic.profile.message.changePwdSuccess'))
    userStore.logout()
    router.push('/login')
  } catch {
    // 错误已由拦截器处理
  } finally {
    pwdSaving.value = false
  }
}

onMounted(() => {
  loadProfile()
})
</script>

<style scoped lang="scss">
.profile-page {
  padding: $spacing-md;
  max-width: 800px;

  &__avatar {
    text-align: center;

    .avatar-name {
      margin-top: $spacing-md;
      font-size: $font-size-lg;
      font-weight: 500;
    }
  }
}
</style>
```

- [ ] **Step 4: 在 router/index.ts 追加 /profile 独立路由**

修改 `EasyProduct.Admin/src/router/index.ts`：在 `routes` 数组的 MainLayout children 中（`...basicRoutes,` 之后、`...placeholderRoutes,` 之前）追加：
```typescript
      { path: 'profile', name: 'profile', component: () => import('@/views/basic/profile/index.vue'), meta: { title: 'menu.profile', icon: 'User' } },
```

- [ ] **Step 5: 在 AppTopbar.vue 添加 profile 入口**

修改 `EasyProduct.Admin/src/layouts/components/AppTopbar.vue`：将 `<span class="app-topbar__user">{{ userStore.realName }}</span>` 替换为可点击跳转的入口：
```vue
      <el-button text @click="router.push('/profile')">
        {{ userStore.realName }}
      </el-button>
```
并在 `<script setup>` 中补 `const router = useRouter()`（若已有则不重复；当前 AppTopbar 已 import useRouter 但未声明 router 变量——执行时确认，缺则补 `const router = useRouter()`）。

- [ ] **Step 6: 全量检查 + 构建**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check && pnpm lint && pnpm check:i18n && pnpm build
```
Expected: 四项全绿。

- [ ] **Step 7: 手测**

点击顶栏 realName 跳 `/profile`：基本信息表单（realName 必填）、头像上传、保存后顶栏名字更新；改密 tab，旧密码错误提示、正确后跳登录页。

- [ ] **Step 8: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/profile EasyProduct.Admin/src/router/index.ts EasyProduct.Admin/src/layouts/components/AppTopbar.vue EasyProduct.Admin/src/i18n/zh-CN/basic.json EasyProduct.Admin/src/i18n/en-US/basic.json EasyProduct.Admin/src/i18n/zh-CN/menu.json EasyProduct.Admin/src/i18n/en-US/menu.json && git commit -m "feat(admin): 创建个人中心页面（基本信息+头像+改密）+ 顶栏入口"
```

---

## 模块4：公告管理（CRUD + 富文本，最重）

### Task 12: 公告类型定义

**Files:**
- Modify: `EasyProduct.Admin/src/types/basic.ts`

- [ ] **Step 1: 在 basic.ts 追加公告类型**

在 `EasyProduct.Admin/src/types/basic.ts` 末尾追加：

```typescript
// ==================== 公告管理 ====================

/** 公告类型（通知/系统） */
export type AnnouncementType = 'notice' | 'system'

/** 公告实体 */
export interface Announcement {
  id: string
  title: string
  content: string // html（富文本）
  coverImage?: string
  type: AnnouncementType
  status: 'enabled' | 'disabled'
  isTop: boolean
  publishTime: string
  viewCount: number
  createByName: string
  createdAt: string
  updatedAt: string
}

/** 公告查询参数 */
export interface AnnouncementQuery extends PageQuery {
  title?: string
  type?: string
  status?: string
}

/** 公告创建参数 */
export interface AnnouncementCreateParams {
  title: string
  content: string
  coverImage?: string
  type: AnnouncementType
  status: 'enabled' | 'disabled'
  isTop: boolean
}
```

- [ ] **Step 2: 类型检查 + Commit**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/types/basic.ts && git commit -m "feat(admin): 添加公告管理类型定义"
```

---

### Task 13: 公告 mock data + route

**Files:**
- Create: `mock-server/src/data/admin/announcement.ts`
- Create: `mock-server/src/routes/admin/announcement.ts`
- Modify: `mock-server/src/server.ts`

- [ ] **Step 1: 创建公告种子数据**

创建 `mock-server/src/data/admin/announcement.ts`：

```typescript
// src/data/admin/announcement.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface Announcement {
  id: string
  title: string
  content: string
  coverImage?: string
  type: 'notice' | 'system'
  status: 'enabled' | 'disabled'
  isTop: boolean
  publishTime: string
  viewCount: number
  createByName: string
  createdAt: string
  updatedAt: string
}

const now = () => isoTime()

export const ANNOUNCEMENTS: Announcement[] = [
  {
    id: guid(),
    title: '系统升级公告',
    content: '<p>系统将于本周日凌晨 2:00-4:00 进行升级维护，期间服务暂停。</p>',
    coverImage: '',
    type: 'system',
    status: 'enabled',
    isTop: true,
    publishTime: now(),
    viewCount: 128,
    createByName: '系统管理员',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    title: '五一放假通知',
    content: '<p>五一劳动节放假 5 天，请各部门做好交接。</p>',
    coverImage: '',
    type: 'notice',
    status: 'enabled',
    isTop: false,
    publishTime: now(),
    viewCount: 56,
    createByName: '系统管理员',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    title: '新版本功能预告',
    content: '<p>下个版本将上线工作流引擎与报表中心。</p>',
    coverImage: '',
    type: 'notice',
    status: 'disabled',
    isTop: false,
    publishTime: now(),
    viewCount: 12,
    createByName: '系统管理员',
    createdAt: now(),
    updatedAt: now(),
  },
]
```

- [ ] **Step 2: 创建公告路由**

创建 `mock-server/src/routes/admin/announcement.ts`：

```typescript
// src/routes/admin/announcement.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ANNOUNCEMENTS } from '../../data/admin/announcement.js'
import { guid, isoTime } from '../../helpers/id.js'

export const adminAnnouncementRouter = Router()

/** 公告列表（分页 + title/type/status 筛选） */
adminAnnouncementRouter.get('/basic/announcement/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const type = req.query.type as string | undefined
  const status = req.query.status as string | undefined

  let filtered = ANNOUNCEMENTS
  if (title) filtered = filtered.filter((a) => a.title.includes(title))
  if (type) filtered = filtered.filter((a) => a.type === type)
  if (status) filtered = filtered.filter((a) => a.status === status)

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

/** 公告详情 */
adminAnnouncementRouter.get('/basic/announcement/:id', (req, res) => {
  const item = ANNOUNCEMENTS.find((a) => a.id === req.params.id)
  if (!item) {
    res.json(fail('公告不存在', 404))
    return
  }
  res.json(ok(item))
})

/** 新增公告 */
adminAnnouncementRouter.post('/basic/announcement', (req, res) => {
  const { title, content, coverImage, type, status, isTop } = req.body || {}
  if (!title || !content || !type) {
    res.json(fail('标题、正文、类型不能为空', 400))
    return
  }
  const now = isoTime()
  const item = {
    id: guid(),
    title,
    content,
    coverImage: coverImage || '',
    type,
    status: status || 'enabled',
    isTop: !!isTop,
    publishTime: now,
    viewCount: 0,
    createByName: '系统管理员',
    createdAt: now,
    updatedAt: now,
  }
  ANNOUNCEMENTS.unshift(item)
  res.json(ok({ id: item.id }, '创建成功'))
})

/** 编辑公告 */
adminAnnouncementRouter.put('/basic/announcement/:id', (req, res) => {
  const idx = ANNOUNCEMENTS.findIndex((a) => a.id === req.params.id)
  if (idx === -1) {
    res.json(fail('公告不存在', 404))
    return
  }
  const { title, content, coverImage, type, status, isTop } = req.body || {}
  const now = isoTime()
  ANNOUNCEMENTS[idx] = {
    ...ANNOUNCEMENTS[idx],
    title: title ?? ANNOUNCEMENTS[idx].title,
    content: content ?? ANNOUNCEMENTS[idx].content,
    coverImage: coverImage ?? ANNOUNCEMENTS[idx].coverImage,
    type: type ?? ANNOUNCEMENTS[idx].type,
    status: status ?? ANNOUNCEMENTS[idx].status,
    isTop: isTop ?? ANNOUNCEMENTS[idx].isTop,
    updatedAt: now,
  }
  res.json(ok(null, '更新成功'))
})

/** 删除公告 */
adminAnnouncementRouter.delete('/basic/announcement/:id', (req, res) => {
  const idx = ANNOUNCEMENTS.findIndex((a) => a.id === req.params.id)
  if (idx === -1) {
    res.json(fail('公告不存在', 404))
    return
  }
  ANNOUNCEMENTS.splice(idx, 1)
  res.json(ok(null, '删除成功'))
})

/** 批量删除公告 */
adminAnnouncementRouter.post('/basic/announcement/batch-delete', (req, res) => {
  const ids: string[] = req.body?.ids || []
  if (!Array.isArray(ids) || ids.length === 0) {
    res.json(fail('ids 不能为空', 400))
    return
  }
  for (const id of ids) {
    const idx = ANNOUNCEMENTS.findIndex((a) => a.id === id)
    if (idx !== -1) ANNOUNCEMENTS.splice(idx, 1)
  }
  res.json(ok(null, '删除成功'))
})

/** 发布公告（置 publishTime） */
adminAnnouncementRouter.put('/basic/announcement/:id/publish', (req, res) => {
  const item = ANNOUNCEMENTS.find((a) => a.id === req.params.id)
  if (!item) {
    res.json(fail('公告不存在', 404))
    return
  }
  item.status = 'enabled'
  item.publishTime = isoTime()
  item.updatedAt = isoTime()
  res.json(ok(null, '发布成功'))
})
```

- [ ] **Step 3: 在 server.ts 注册 announcement 路由**

修改 `mock-server/src/server.ts`：在 `import { adminProfileRouter } from './routes/admin/profile.js'` 后追加：
```typescript
import { adminAnnouncementRouter } from './routes/admin/announcement.js'
```
在 `app.use('/api/admin', ...)` 行的 `adminProfileRouter` 后追加 `adminAnnouncementRouter`。

- [ ] **Step 4: 启动 mock 验证**

Run:
```bash
cd D:/4-MyProject/EasyProduct/mock-server && pnpm dev
```
另终端：
```bash
TOKEN=$(curl -s -X POST http://localhost:7700/api/admin/auth/login -H "Content-Type: application/json" -d '{"userName":"admin","password":"admin123"}' | node -pe 'JSON.parse(require("fs").readFileSync(0)).data.accessToken')
curl -s "http://localhost:7700/api/admin/basic/announcement/list?pageIndex=1&pageSize=10" -H "Authorization: Bearer $TOKEN" | head -c 300
```
Expected: `code:200`，data.list 含 3 条公告。

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/src/data/admin/announcement.ts mock-server/src/routes/admin/announcement.ts mock-server/src/server.ts && git commit -m "feat(mock): 添加公告数据与路由（CRUD + 批量删除 + 发布）"
```

---

### Task 14: 公告前端 API

**Files:**
- Create: `EasyProduct.Admin/src/api/basic/announcement.ts`

- [ ] **Step 1: 创建 announcement.ts API**

创建 `EasyProduct.Admin/src/api/basic/announcement.ts`：

```typescript
// src/api/basic/announcement.ts
import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  Announcement,
  AnnouncementQuery,
  AnnouncementCreateParams,
} from '@/types/basic'

/** 公告列表（分页） */
export const getAnnouncementList = (params: AnnouncementQuery) =>
  get<PageResult<Announcement>>('/api/admin/basic/announcement/list', params)

/** 公告详情 */
export const getAnnouncementDetail = (id: string) =>
  get<Announcement>(`/api/admin/basic/announcement/${id}`)

/** 新增公告 */
export const createAnnouncement = (data: AnnouncementCreateParams) =>
  post<{ id: string }>('/api/admin/basic/announcement', data)

/** 编辑公告 */
export const updateAnnouncement = (id: string, data: Partial<AnnouncementCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/announcement/${id}`, data)

/** 删除公告 */
export const deleteAnnouncement = (id: string) =>
  del<null>(`/api/admin/basic/announcement/${id}`)

/** 批量删除公告 */
export const deleteAnnouncementBatch = (ids: string[]) =>
  post<null>('/api/admin/basic/announcement/batch-delete', { ids })

/** 发布公告 */
export const publishAnnouncement = (id: string) =>
  put<null>(`/api/admin/basic/announcement/${id}/publish`)
```

- [ ] **Step 2: 类型检查 + Commit**

```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/api/basic/announcement.ts && git commit -m "feat(admin): 添加公告管理 API"
```

---

### Task 15: 公告表单弹窗（含 RichText + ImageUpload）

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/announcement/components/AnnouncementFormDialog.vue`

- [ ] **Step 1: 在 basic.json 追加 announcement 文案**

zh-CN/basic.json 在 `profile` 节点后追加：
```json
,
"announcement": {
  "title": "公告管理",
  "titleField": "标题",
  "content": "正文",
  "coverImage": "封面图",
  "type": "类型",
  "status": "状态",
  "isTop": "置顶",
  "publishTime": "发布时间",
  "viewCount": "浏览量",
  "createByName": "创建人",
  "createdAt": "创建时间",
  "typeNotice": "通知",
  "typeSystem": "系统",
  "publish": "发布",
  "form": {
    "addTitle": "新增公告",
    "editTitle": "编辑公告",
    "title": "标题",
    "titlePlaceholder": "请输入公告标题",
    "titleRequired": "请输入公告标题",
    "content": "正文",
    "contentRequired": "请输入公告正文",
    "coverImage": "封面图",
    "type": "类型",
    "typeRequired": "请选择类型",
    "status": "状态",
    "isTop": "是否置顶"
  },
  "message": {
    "addSuccess": "新增成功",
    "updateSuccess": "更新成功",
    "deleteSuccess": "删除成功",
    "publishSuccess": "发布成功",
    "deleteConfirm": "确认删除公告【{title}】吗？",
    "batchDeleteConfirm": "确认删除选中的 {count} 条公告吗？"
  }
}
```
en-US/basic.json 对应追加英文：
```json
,
"announcement": {
  "title": "Announcements",
  "titleField": "Title",
  "content": "Content",
  "coverImage": "Cover",
  "type": "Type",
  "status": "Status",
  "isTop": "Top",
  "publishTime": "Publish Time",
  "viewCount": "Views",
  "createByName": "Author",
  "createdAt": "Created At",
  "typeNotice": "Notice",
  "typeSystem": "System",
  "publish": "Publish",
  "form": {
    "addTitle": "Add Announcement",
    "editTitle": "Edit Announcement",
    "title": "Title",
    "titlePlaceholder": "Enter title",
    "titleRequired": "Please enter title",
    "content": "Content",
    "contentRequired": "Please enter content",
    "coverImage": "Cover",
    "type": "Type",
    "typeRequired": "Please select type",
    "status": "Status",
    "isTop": "Top"
  },
  "message": {
    "addSuccess": "Added successfully",
    "updateSuccess": "Updated successfully",
    "deleteSuccess": "Deleted successfully",
    "publishSuccess": "Published successfully",
    "deleteConfirm": "Delete announcement [{title}]?",
    "batchDeleteConfirm": "Delete {count} selected announcements?"
  }
}
```

- [ ] **Step 2: 在 menu.json 追加 announcement 菜单 key**

zh-CN/menu.json 追加：
```json
, "basicAnnouncement": "公告管理"
```
en-US/menu.json 追加：
```json
, "basicAnnouncement": "Announcement"
```

- [ ] **Step 3: 创建 AnnouncementFormDialog.vue**

创建 `EasyProduct.Admin/src/views/basic/announcement/components/AnnouncementFormDialog.vue`：

```vue
<!-- src/views/basic/announcement/components/AnnouncementFormDialog.vue -->
<template>
  <el-dialog
    :model-value="modelValue"
    :title="isEdit ? t('basic.announcement.form.editTitle') : t('basic.announcement.form.addTitle')"
    width="800px"
    @update:model-value="emit('update:modelValue', $event)"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item :label="t('basic.announcement.form.title')" prop="title">
        <el-input
          v-model="formData.title"
          maxlength="200"
          show-word-limit
          :placeholder="t('basic.announcement.form.titlePlaceholder')"
        />
      </el-form-item>
      <el-row :gutter="$spacing-md">
        <el-col :span="12">
          <el-form-item :label="t('basic.announcement.form.type')" prop="type">
            <el-radio-group v-model="formData.type">
              <el-radio value="notice">{{ t('basic.announcement.typeNotice') }}</el-radio>
              <el-radio value="system">{{ t('basic.announcement.typeSystem') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item :label="t('basic.announcement.form.status')" prop="status">
            <el-radio-group v-model="formData.status">
              <el-radio value="enabled">{{ t('common.status.enabled') }}</el-radio>
              <el-radio value="disabled">{{ t('common.status.disabled') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item :label="t('basic.announcement.form.isTop')" prop="isTop">
        <el-switch v-model="formData.isTop" />
      </el-form-item>
      <el-form-item :label="t('basic.announcement.form.coverImage')" prop="coverImage">
        <ImageUpload v-model="formData.coverImage" />
      </el-form-item>
      <el-form-item :label="t('basic.announcement.form.content')" prop="content">
        <RichTextEditor
          v-model="formData.content"
          :placeholder="t('common.richText.placeholder')"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="emit('update:modelValue', false)">
        {{ t('common.cancel') }}
      </el-button>
      <el-button type="primary" :loading="saving" @click="handleSave">
        {{ t('common.button.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import {
  getAnnouncementDetail,
  createAnnouncement,
  updateAnnouncement,
} from '@/api/basic/announcement'
import type { AnnouncementCreateParams } from '@/types/basic'
import ImageUpload from '@/components/common/ImageUpload.vue'
import RichTextEditor from '@/components/common/RichTextEditor.vue'

const props = defineProps<{
  modelValue: boolean
  announcementId?: string
}>()
const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()
const formRef = ref<FormInstance | null>(null)
const saving = ref(false)

const isEdit = computed(() => !!props.announcementId)

const formData = reactive<AnnouncementCreateParams>({
  title: '',
  content: '',
  coverImage: '',
  type: 'notice',
  status: 'enabled',
  isTop: false,
})

const formRules: FormRules = {
  title: [{ required: true, message: () => t('basic.announcement.form.titleRequired'), trigger: 'blur' }],
  type: [{ required: true, message: () => t('basic.announcement.form.typeRequired'), trigger: 'change' }],
  content: [{ required: true, message: () => t('basic.announcement.form.contentRequired'), trigger: 'blur' }],
}

watch(
  () => props.modelValue,
  async (val) => {
    if (!val) return
    if (props.announcementId) {
      await loadDetail(props.announcementId)
    } else {
      resetForm()
    }
  },
)

const loadDetail = async (id: string): Promise<void> => {
  try {
    const data = await getAnnouncementDetail(id)
    formData.title = data.title
    formData.content = data.content
    formData.coverImage = data.coverImage ?? ''
    formData.type = data.type
    formData.status = data.status
    formData.isTop = data.isTop
  } catch {
    // 错误已由拦截器处理
  }
}

const resetForm = (): void => {
  formData.title = ''
  formData.content = ''
  formData.coverImage = ''
  formData.type = 'notice'
  formData.status = 'enabled'
  formData.isTop = false
}

const handleClose = (): void => {
  formRef.value?.resetFields()
}

const handleSave = async (): Promise<void> => {
  if (!formRef.value) return
  try {
    await formRef.value.validate()
  } catch {
    return
  }
  saving.value = true
  try {
    if (isEdit.value) {
      await updateAnnouncement(props.announcementId!, formData)
      ElMessage.success(t('basic.announcement.message.updateSuccess'))
    } else {
      await createAnnouncement(formData)
      ElMessage.success(t('basic.announcement.message.addSuccess'))
    }
    emit('update:modelValue', false)
    emit('success')
  } catch {
    // 错误已由拦截器处理
  } finally {
    saving.value = false
  }
}
</script>
```

> 注：`formData.coverImage` 在 reactive 对象上初始化为 `''`，v-model 直接绑定即可（coverImage 虽为可选字段，但 reactive 已赋初值）。

- [ ] **Step 4: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/announcement/components/AnnouncementFormDialog.vue EasyProduct.Admin/src/i18n/zh-CN/basic.json EasyProduct.Admin/src/i18n/en-US/basic.json EasyProduct.Admin/src/i18n/zh-CN/menu.json EasyProduct.Admin/src/i18n/en-US/menu.json && git commit -m "feat(admin): 创建公告表单弹窗（富文本正文 + 封面图上传）"
```

---

### Task 16: 公告列表页 + 路由

**Files:**
- Create: `EasyProduct.Admin/src/views/basic/announcement/index.vue`
- Modify: `EasyProduct.Admin/src/router/modules/basic.ts`

- [ ] **Step 1: 创建公告列表页**

创建 `EasyProduct.Admin/src/views/basic/announcement/index.vue`：

```vue
<!-- src/views/basic/announcement/index.vue -->
<template>
  <div class="announcement-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      :loading="table.loading.value"
      @search="onSearch"
      @reset="onReset"
    >
      <template #toolbar>
        <el-button v-permission="['basic:announcement:add']" type="primary" @click="handleAdd">
          {{ t('common.add') }}
        </el-button>
        <el-button
          v-permission="['basic:announcement:delete']"
          type="danger"
          :disabled="!selectedRows.length"
          @click="handleBatchDelete"
        >
          {{ t('common.batchDelete') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <BaseTable
      :loading="table.loading.value"
      :data="table.list.value"
      :total="table.total.value"
      :current-page="table.query.pageIndex"
      :page-size="table.query.pageSize"
      @page-change="table.handlePageChange"
      @size-change="handleSizeChange"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="50" />
      <el-table-column :label="t('basic.announcement.titleField')" prop="title" min-width="200">
        <template #default="{ row }">
          <el-button link type="primary" @click="handleView(row)">{{ row.title }}</el-button>
          <el-tag v-if="row.isTop" size="small" type="danger" class="top-tag">
            {{ t('basic.announcement.isTop') }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('basic.announcement.coverImage')" width="100" align="center">
        <template #default="{ row }">
          <el-image
            v-if="row.coverImage"
            :src="row.coverImage"
            fit="cover"
            style="width: 60px; height: 40px"
            :preview-src-list="[row.coverImage]"
            preview-teleported
          />
          <span v-else>-</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('basic.announcement.type')" prop="type" width="100" align="center">
        <template #default="{ row }">
          <el-tag size="small" :type="row.type === 'system' ? 'warning' : 'info'">
            {{ row.type === 'system' ? t('basic.announcement.typeSystem') : t('basic.announcement.typeNotice') }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('basic.announcement.status')" prop="status" width="100" align="center">
        <template #default="{ row }">
          <el-tag size="small" :type="row.status === 'enabled' ? 'success' : 'danger'">
            {{ row.status === 'enabled' ? t('common.status.enabled') : t('common.status.disabled') }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('basic.announcement.viewCount')" prop="viewCount" width="100" align="center" />
      <el-table-column :label="t('basic.announcement.publishTime')" prop="publishTime" width="160" />
      <el-table-column :label="t('common.actions')" width="220" align="center" fixed="right">
        <template #default="{ row }">
          <el-button v-permission="['basic:announcement:edit']" link type="primary" size="small" @click="handleEdit(row)">
            {{ t('common.edit') }}
          </el-button>
          <el-button
            v-if="row.status === 'disabled'"
            v-permission="['basic:announcement:publish']"
            link
            type="success"
            size="small"
            @click="handlePublish(row)"
          >
            {{ t('basic.announcement.publish') }}
          </el-button>
          <el-button v-permission="['basic:announcement:delete']" link type="danger" size="small" @click="handleDelete(row)">
            {{ t('common.delete') }}
          </el-button>
        </template>
      </el-table-column>
    </BaseTable>

    <AnnouncementFormDialog
      v-model="dialogVisible"
      :announcement-id="currentId"
      @success="table.reload"
    />

    <!-- 详情弹窗 -->
    <el-dialog v-model="detailVisible" :title="detail?.title" width="700px">
      <div v-if="detail" class="detail-content" v-html="detail.content" />
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import { useLocale } from '@/composables/useLocale'
import { useSearch } from '@/composables/useSearch'
import { useTable } from '@/composables/useTable'
import {
  getAnnouncementList,
  getAnnouncementDetail,
  deleteAnnouncement,
  deleteAnnouncementBatch,
  publishAnnouncement,
} from '@/api/basic/announcement'
import type { Announcement } from '@/types/basic'
import type { SearchField } from '@/types/search'
import AnnouncementFormDialog from './components/AnnouncementFormDialog.vue'

const { t } = useLocale()

const { searchModel, getSearchParams } = useSearch<{ title?: string; type?: string; status?: string }>({
  defaultModel: { title: '', type: '', status: '' },
})

const searchFields: SearchField[] = [
  { prop: 'title', label: 'basic.announcement.titleField', type: 'input', placeholder: 'basic.announcement.form.titlePlaceholder' },
  {
    prop: 'type',
    label: 'basic.announcement.type',
    type: 'select',
    options: [
      { label: 'basic.announcement.typeNotice', value: 'notice' },
      { label: 'basic.announcement.typeSystem', value: 'system' },
    ],
  },
  {
    prop: 'status',
    label: 'basic.announcement.status',
    type: 'select',
    options: [
      { label: 'common.status.enabled', value: 'enabled' },
      { label: 'common.status.disabled', value: 'disabled' },
    ],
  },
]

const table = useTable<Announcement>((params) => getAnnouncementList(params as never))

const onSearch = (): void => {
  Object.assign(table.query, getSearchParams())
  void table.handleSearch()
}

const onReset = (): void => {
  searchModel.title = ''
  searchModel.type = ''
  searchModel.status = ''
  Object.assign(table.query, { title: undefined, type: undefined, status: undefined })
  void table.handleSearch()
}

const handleSizeChange = (size: number): void => {
  table.query.pageSize = size
  table.query.pageIndex = 1
  void table.reload()
}

// 多选
const selectedRows = ref<Announcement[]>([])
const handleSelectionChange = (rows: unknown[]): void => {
  selectedRows.value = rows as Announcement[]
}

// 弹窗
const dialogVisible = ref(false)
const currentId = ref<string | undefined>(undefined)

const handleAdd = (): void => {
  currentId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Announcement): void => {
  currentId.value = row.id
  dialogVisible.value = true
}

// 详情
const detailVisible = ref(false)
const detail = ref<Announcement | null>(null)

const handleView = async (row: Announcement): Promise<void> => {
  try {
    detail.value = await getAnnouncementDetail(row.id)
    detailVisible.value = true
  } catch {
    // 错误已由拦截器处理
  }
}

// 发布
const handlePublish = async (row: Announcement): Promise<void> => {
  try {
    await publishAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.publishSuccess'))
    await table.reload()
  } catch {
    // 错误已由拦截器处理
  }
}

// 删除
const handleDelete = async (row: Announcement): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.message.deleteConfirm', { title: row.title }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      },
    )
    await deleteAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.deleteSuccess'))
    await table.reload()
  } catch {
    // 用户取消或请求失败
  }
}

const handleBatchDelete = async (): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.message.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      },
    )
    await deleteAnnouncementBatch(selectedRows.value.map((r) => r.id))
    ElMessage.success(t('basic.announcement.message.deleteSuccess'))
    selectedRows.value = []
    await table.reload()
  } catch {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.announcement-page {
  padding: $spacing-md;

  .top-tag {
    margin-left: $spacing-xs;
  }

  .detail-content {
    :deep(p) {
      margin: 0 0 $spacing-sm;
    }
  }
}
</style>
```

- [ ] **Step 2: 在 basic.ts 路由追加 announcement 子路由**

修改 `EasyProduct.Admin/src/router/modules/basic.ts`：在 `config` 子路由后追加：
```typescript
      },
      {
        path: 'announcement',
        name: 'basic-announcement',
        component: () => import('@/views/basic/announcement/index.vue'),
        meta: { title: 'menu.basicAnnouncement', icon: 'Bell' }
      }
```
并删除原第 40 行占位注释 `// 批次 3~4 的路由后续追加（announcement/setting/profile）`（此时 config + announcement 两子路由已落，注释不再需要）。

- [ ] **Step 3: 全量检查 + 构建**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check && pnpm lint && pnpm check:i18n && pnpm build
```
Expected: 四项全绿（确保 AnnouncementFormDialog 的 coverImage `!` 已移除）。

- [ ] **Step 4: 手测**

访问 `/basic/announcement`：列表展示、title/type/status 搜索、新增（富文本+封面图）、编辑、发布（disabled→enabled）、删除、批量删除、点击标题看详情弹窗、中英切换。

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add EasyProduct.Admin/src/views/basic/announcement/index.vue EasyProduct.Admin/src/router/modules/basic.ts && git commit -m "feat(admin): 创建公告管理列表页（搜索+CRUD+发布+详情）"
```

---

## 模块5：收尾

### Task 17: 菜单权限种子 + mock 状态表 + 全量验收

**Files:**
- Modify: `mock-server/src/data/admin/basic.ts`（USER_PERMISSIONS 追加 announcement/config 权限——可选，admin 已是 `*` 通配）
- Modify: `mock-server/src/server.ts`（MOCK_STATUS 注释更新）
- Modify: `mock-server/README.md`（状态表标记 Basic 已完成）

- [ ] **Step 1: 全量构建与检查**

Run:
```bash
cd D:/4-MyProject/EasyProduct/EasyProduct.Admin && pnpm type-check && pnpm lint && pnpm check:i18n && pnpm build
```
Expected: 四项全绿。这是整个批次的最终 DoD 闸门。

- [ ] **Step 2: mock-server 启动 + 全接口冒烟**

Run:
```bash
cd D:/4-MyProject/EasyProduct/mock-server && pnpm dev
```
另终端对每个新接口冒烟（带 token）：file/upload、config/list（POST/PUT/DELETE/batch-delete）、desktop/overview、profile（GET/PUT/password）、announcement（list/detail/POST/PUT/DELETE/batch-delete/publish）。逐个确认 `code:200`，改密旧密码错误返回 `code:400`。

- [ ] **Step 3: 更新 mock-server README 状态表**

修改 `mock-server/README.md`：将 admin Basic 模块状态由 `pending` 标记为 `pending`（仍 mock 生效，待后端 P1 交付后切 `deprecated`）。在状态表后追加备注行：`> Basic 模块 9 页（用户/角色/菜单/部门/字典/公告/系统参数/个人中心/工作台）mock 全量就绪`。

- [ ] **Step 4: 端到端走查**

启动 mock + admin，登录 admin/admin123，依次访问并验证闭环：
- `/desktop` 轻量首页四区
- `/basic/config` 内联编辑三类型
- `/profile` 基本信息+头像+改密
- `/basic/announcement` 富文本 CRUD + 发布 + 详情
- 中英切换全页面无硬编码中文
- 权限按钮：用 sales 账号登录验证 `basic:announcement:*` 等按钮隐藏（sales 权限集无这些 code）

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProduct && git add mock-server/README.md && git commit -m "docs(mock): 更新 README 状态表标记 Basic 模块 mock 全量就绪"
```

---

## 验收清单（批次3完成判定）

- [ ] **功能**
  - [ ] 公告：列表/搜索/新增(富文本+封面)/编辑/发布/删除/批量删除/详情
  - [ ] 系统参数：列表/搜索/内联编辑(string/number/boolean)/新增/删除/批量删除/key 唯一校验
  - [ ] 个人中心：基本信息编辑/头像上传/改密(旧密码校验)/改密成功跳登录
  - [ ] 工作台：欢迎区/4 统计卡片/最近订单/待办/快捷入口
- [ ] **代码**
  - [ ] `pnpm type-check` 零错误
  - [ ] `pnpm lint` 通过
  - [ ] `pnpm check:i18n` 无硬编码中文
  - [ ] `pnpm build` 通过
- [ ] **集成**
  - [ ] 顶栏 realName 跳 `/profile`
  - [ ] `/basic/announcement` 与 `/basic/config` 出现在侧边栏 Basic 菜单下
  - [ ] 中英切换全页面文案正确
  - [ ] 权限按钮显隐正确（admin 全显、sales 隐藏）
- [ ] **通用组件复用**
  - [ ] ImageUpload 被个人中心头像 + 公告封面复用
  - [ ] RichTextEditor 被公告正文使用，销毁无泄漏

---

**实现计划完成！**

**执行方式选择：**

**1. Subagent-Driven（推荐）** — 我逐任务派发新子代理执行，任务间审查，快速迭代

**2. Inline Execution** — 在当前会话用 executing-plans skill 批量执行，带检查点

请选择执行方式。
