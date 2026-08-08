# EasyProduct 前端先行开发计划（F 系列：mock-server + 三端前端）

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在后端（B 系列）启动前，用独立 mock-server 驱动 EasyProduct 三端前端（Admin/Site/MiniApp）从 0 到可演示，后端交付后按模块无缝切换真接口。

**Architecture:** 独立 mock-server（Node20+Express+mockjs，端口 7700）按后端契约提前实现三分区接口（`/api/admin/**`、`/api/site/**`、`/api/app/**`）；三端前端通过 vite proxy / env 指向 mock，逐模块退役切换。所有代码**参考** EasyWebSite/EasyProject/EasyCRM 三个源项目**重写**，禁止直接复制。

**Tech Stack:** Vue ^3.4 + TypeScript ^5.3（strict）+ Vite ^5.4 + Element Plus ≥2.6（仅 Admin）+ Pinia ^2.1 + vue-router ^4.3 + vue-i18n ^9.9 + axios ^1.6 + ECharts ^6/vue-echarts ^8（Admin 报表/工作台）+ dayjs；MiniApp 为微信原生 + TS + miniprogram-api-typings；mock-server 为 Node 20 + Express + mockjs + tsx；包管理 pnpm，Node ≥20。

**上游依据：**
- `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`（第 6 节数据模型、第 7 节前端整合、第 9 节菜单树、第 10 节阶段规划）
- `docs/frontend-guidelines.md`（强制，冲突以它为准）
- `docs/mock-guidelines.md`（mock 契约与退役流程）
- `docs/backend-guidelines.md` 第 5 节（路由/信封契约基准）

## Global Constraints

1. 统一响应信封 `{code, message, data, timestamp}`，HTTP 一律 200，`code===200` 成功；拦截器自动解包 `data`。
2. 主键/外键一律 GUID 字符串；JSON 字段 camelCase；分页入参 `pageIndex/pageSize`，出参 `list/total`。
3. **禁用 TS enum**：取值集合 = 字符串联合类型 + `as const` 常量对象。
4. `no-explicit-any: error`、`no-console: error`；`vue-tsc --noEmit` 零错误才能提交/构建。
5. 样式仅 SCSS、组件必须 scoped、禁止硬编码色值/间距（走 variables/CSS Variables）；Admin 亮/暗双主题，Site 仅亮色，MiniApp 跟随系统。
6. 所有可见文案走 i18n key，禁止硬编码中文；`npm run check:i18n` 门禁；语言包按模块拆 JSON（`i18n/<lang>/<module>.json`），内置基线 + 远程覆盖（`GET /api/i18n/{lang}/{module}.json`，一期由静态目录托管，mock 阶段由 mock-server 托管）。
7. API 文件 `api/<域>/<模块>.ts` 不带 Api 后缀；类型 `types/<域>.ts`，接口**不加 I 前缀**（MiniApp 新增类型同样无前缀）。
8. pnpm；三个 app 独立 package.json，**不做 monorepo workspace**；Node ≥20（根 `.nvmrc` 锁 20）。
9. 端口：mock 7700、后端 7600、Admin 5173、Site 5174。
10. Conventional Commits + commitlint，scope 用端名：`feat(admin):` `fix(site):` `feat(miniapp):` `feat(mock):` `chore:`。
11. mock 路由与后端规范第 5 节逐字一致（分区前缀、资源名连字符、`/list`、`/{id}`、RESTful 方法）；**禁止自创接口/字段**。
12. **参考源项目重写，禁止直接复制源项目代码**（用户指令，覆盖整合设计 7 节"以旧项目为骨架迁移"表述）。
13. 依赖基线锁定：Vue ^3.4 / TS ^5.3 / Element Plus ≥2.6 / Pinia ^2.1 / vue-router ^4.3 / vue-i18n ^9.9 / axios ^1.6 / ECharts ^6 + vue-echarts ^8 / Vite ^5.4 / dayjs；新增依赖须在 commit 说明理由。
14. vite proxy 目标环境化：`VITE_PROXY_TARGET` 默认 `http://localhost:7700`（mock-first）；后端联调时改 `http://localhost:7600`，或按路径分流（已交付模块→7600，未交付→7700）。此为对前端规范 1.6 "proxy 固定指向 7600" 的阶段化裁决，记录于此。
15. Admin/Site 拦截器共用信封解包逻辑，但提示器不同：Admin 用 ElMessage，Site 用轻量 toast（Site 不引入 Element Plus）——对规范 1.2 表格"共用实现"的落地裁决。
16. 事件函数命名：Web 端 `handle*`，MiniApp `on*`。
17. 每个提交满足 `docs/frontend-guidelines.md` 第 5 节 Definition of Done。

## 阶段总览与细化机制

| 阶段 | 内容 | 演示物 | 本计划粒度 |
|------|------|--------|-----------|
| **F0 框架** | mock-server 核心 + 三端最小可运行骨架 | 三端启动、admin 登录进工作台 | **任务级（下文 Task 1~10，直接执行）** |
| **F1 Site** | 官网 11 门户页 + site 分区 mock 全量 | 官网可浏览、询价提交闭环 | 阶段蓝图（进入前用 writing-plans 细化） |
| **F2 Admin** | 八大模块，序：Basic→Site→Product→Mall→Crm→Workflow→Report→Ops | 各模块逐个可演示 | 阶段蓝图 |
| **F3 MiniApp** | 小程序全页面重写 | 开发者工具下单闭环 | 阶段蓝图 |

> 细化机制沿用整合设计第 10 节约定：进入 F1~F3 前各自再用 writing-plans 拆任务级步骤，避免单次任务过大。F1~F3 蓝图见本文后半部分。

---

## F0 文件结构（本阶段创建的全部文件）

```text
EasyProduct/
├── .nvmrc                                    # Node 20
├── .gitignore                                # 已存在，Task 3 补充 node_modules/dist
├── README.md                                 # 根 README：启动顺序/端口/文档索引
├── package.json                              # 仅 git 钩子：husky+commitlint+lint-staged（非 workspace）
├── commitlint.config.cjs
├── lint-staged.config.cjs
├── .husky/pre-commit                         # npx lint-staged
├── .husky/commit-msg                         # commitlint
├── mock-server/
│   ├── package.json  tsconfig.json  README.md
│   └── src/
│       ├── server.ts
│       ├── helpers/envelope.ts  id.ts  auth.ts  registry.ts
│       ├── i18n/zh-CN/common.json  menu.json   # 语言包托管（模拟 nginx 静态）
│       ├── i18n/en-US/common.json  menu.json
│       ├── data/basic.ts  site.ts
│       └── routes/admin/auth.ts  routes/admin/menu.ts  routes/admin/dict.ts
│           routes/site/home.ts  routes/app/auth.ts  routes/i18n.ts
├── EasyProduct.Admin/
│   ├── package.json  vite.config.ts  tsconfig.json  index.html  env.d.ts
│   ├── .env.development  .eslintrc.cjs  .prettierrc.json
│   ├── scripts/check-chinese.cjs
│   └── src/
│       ├── main.ts  App.vue
│       ├── types/api.ts  types/basic.ts
│       ├── utils/request.ts  utils/auth.ts
│       ├── styles/variables.scss  mixins.scss  index.scss
│       ├── i18n/index.ts  i18n/zh-CN/common.json  menu.json  i18n/en-US/common.json  menu.json
│       ├── stores/user.ts  stores/app.ts
│       ├── api/basic/auth.ts  api/basic/menu.ts  api/basic/dict.ts
│       ├── router/index.ts  router/guards.ts  router/modules/placeholder.ts
│       ├── layouts/MainLayout.vue  layouts/components/AppSidebar.vue  layouts/components/AppTopbar.vue
│       └── views/login/index.vue  views/desktop/index.vue  views/placeholder/index.vue  views/error/404.vue
├── EasyProduct.Site/
│   ├── package.json  vite.config.ts  tsconfig.json  index.html  env.d.ts
│   ├── .env.development  .eslintrc.cjs  .prettierrc.json
│   ├── scripts/check-chinese.cjs
│   └── src/
│       ├── main.ts  App.vue
│       ├── types/api.ts  types/site.ts
│       ├── utils/request.ts  utils/toast.ts
│       ├── assets/styles/variables.scss  mixins.scss  reset.scss  global.scss
│       ├── i18n/index.ts  i18n/zh-CN/common.json  site.json  i18n/en-US/common.json  site.json
│       ├── router/index.ts
│       ├── components/layout/AppLayout.vue  AppNavbar.vue  AppFooter.vue
│       └── views/home/index.vue  views/error/404.vue
└── EasyProduct.MiniApp/
    ├── project.config.json  app.json  app.ts  sitemap.json  tsconfig.json  package.json
    ├── config/env.ts
    ├── utils/request.ts  storage.ts  wx-login.ts  i18n.ts
    ├── stores/base.store.ts
    ├── types/api.types.ts
    ├── i18n/zh-CN/common.json  i18n/en-US/common.json
    └── pages/index/*  pages/category/*  pages/cart/*  pages/profile/*   # tabBar 4 页占位
```

---

### Task 1: mock-server 基座（helpers + 服务骨架）

**Files:**
- Create: `mock-server/package.json`
- Create: `mock-server/tsconfig.json`
- Create: `mock-server/src/helpers/envelope.ts`
- Create: `mock-server/src/helpers/id.ts`
- Create: `mock-server/src/helpers/auth.ts`
- Create: `mock-server/src/helpers/registry.ts`
- Create: `mock-server/src/server.ts`

**Interfaces:**
- Consumes: 无（F0 第一个任务）
- Produces: `ok<T>(data, message?)` / `fail(message, code?)` / `paginate<T>(source, pageIndex, pageSize, keyword?)`（envelope）；`guid()` / `isoTime()` / `code(prefix)`（id）；`adminGuard` / `appGuard`（auth）；`registerReset(fn)` / `resetAll()`（registry）；Express 实例监听 7700。后续 Task 2 与 F1~F3 所有 mock 路由都只消费这些导出。

- [ ] **Step 1.1: 创建 package.json 与 tsconfig**

`mock-server/package.json`：

```json
{
  "name": "easyproduct-mock-server",
  "version": "0.1.0",
  "private": true,
  "type": "module",
  "engines": { "node": ">=20" },
  "scripts": {
    "dev": "tsx watch src/server.ts",
    "start": "tsx src/server.ts"
  },
  "dependencies": {
    "cors": "^2.8.5",
    "express": "^4.19.2",
    "mockjs": "^1.1.0"
  },
  "devDependencies": {
    "@types/cors": "^2.8.17",
    "@types/express": "^4.17.21",
    "@types/mockjs": "^1.0.10",
    "tsx": "^4.7.0",
    "typescript": "^5.3.3"
  }
}
```

`mock-server/tsconfig.json`：

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "NodeNext",
    "moduleResolution": "NodeNext",
    "strict": true,
    "esModuleInterop": true,
    "resolveJsonModule": true,
    "skipLibCheck": true,
    "noEmit": true
  },
  "include": ["src/**/*.ts"]
}
```

- [ ] **Step 1.2: 写 helpers/envelope.ts（唯一响应出口）**

```typescript
// src/helpers/envelope.ts
/** 统一响应信封（与后端规范 5.3 逐字一致） */
export const ok = <T>(data: T, message = '操作成功') =>
  ({ code: 200, message, data, timestamp: Date.now() })

export const fail = (message: string, code = 400) =>
  ({ code, message, data: null, timestamp: Date.now() })

export interface PageData<T> {
  list: T[]
  total: number
  pageIndex: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPrevPage: boolean
}

export function paginate<T>(source: T[], pageIndex = 1, pageSize = 10, keyword?: string): PageData<T> {
  const filtered = keyword
    ? source.filter((i) => JSON.stringify(i).toLowerCase().includes(keyword.toLowerCase()))
    : source
  const totalPages = Math.ceil(filtered.length / pageSize)
  return {
    list: filtered.slice((pageIndex - 1) * pageSize, pageIndex * pageSize),
    total: filtered.length,
    pageIndex,
    pageSize,
    totalPages,
    hasNextPage: pageIndex < totalPages,
    hasPrevPage: pageIndex > 1,
  }
}
```

- [ ] **Step 1.3: 写 helpers/id.ts 与 helpers/registry.ts**

```typescript
// src/helpers/id.ts
import Mock from 'mockjs'

/** GUID 主键（全库主键形态） */
export const guid = (): string => Mock.mock('@guid')

/** ISO 8601 时间（全库时间形态） */
export const isoTime = (): string => new Date().toISOString()

/** 业务编码：前缀 + 时间戳 + 随机数，如 CU20260808123045123 */
export const code = (prefix: string): string =>
  `${prefix}${Mock.mock('@datetime("yyyyMMddHHmmss")')}${Mock.mock('@integer(100, 999)')}`
```

```typescript
// src/helpers/registry.ts
type Resetter = () => void
const resetters: Resetter[] = []

/** store 模块注册自己的重置函数（F1+ 各模块 store 使用） */
export function registerReset(fn: Resetter): void {
  resetters.push(fn)
}

export function resetAll(): void {
  resetters.forEach((fn) => fn())
}
```

- [ ] **Step 1.4: 写 helpers/auth.ts（分区守卫）**

```typescript
// src/helpers/auth.ts
import type { NextFunction, Request, Response } from 'express'
import { fail } from './envelope.js'

const ADMIN_PUBLIC = ['/api/admin/auth/login']
const APP_PUBLIC = ['/api/app/auth/wx-login']

/** /api/admin/** 必须带 Bearer Token（登录接口除外） */
export function adminGuard(req: Request, res: Response, next: NextFunction): void {
  if (ADMIN_PUBLIC.includes(req.originalUrl.split('?')[0])) return next()
  const header = req.headers.authorization
  if (!header || !header.startsWith('Bearer ')) {
    res.json(fail('未登录或登录已过期', 401))
    return
  }
  next()
}

/** /api/app/** 必须带会员 Token（微信登录接口除外） */
export function appGuard(req: Request, res: Response, next: NextFunction): void {
  if (APP_PUBLIC.includes(req.originalUrl.split('?')[0])) return next()
  const header = req.headers.authorization
  if (!header || !header.startsWith('Bearer ')) {
    res.json(fail('请先登录', 401))
    return
  }
  next()
}
```

- [ ] **Step 1.5: 写 server.ts（装配 + /__mock 管理端点）**

```typescript
// src/server.ts
import express from 'express'
import cors from 'cors'
import { ok } from './helpers/envelope.js'
import { resetAll } from './helpers/registry.js'
import { adminGuard, appGuard } from './helpers/auth.js'

const PORT = 7700
const app = express()
app.use(cors())
app.use(express.json())

// 模块 mock 状态表（与 README 一致；新增模块在此登记）
const MOCK_STATUS = [
  { zone: 'admin', module: 'Basic（认证/菜单/字典骨架）', status: 'pending', backendPhase: 'P1' },
  { zone: 'site', module: '官网内容（首页聚合骨架）', status: 'pending', backendPhase: 'P2' },
  { zone: 'app', module: '商城（会员登录骨架）', status: 'pending', backendPhase: 'P3' },
]

app.get('/__mock/status', (_req, res) => res.json(ok(MOCK_STATUS)))
app.post('/__mock/reset', (_req, res) => {
  resetAll()
  res.json(ok(null, '已重置'))
})

// Task 2 挂载路由（先占位注释）：
// app.use('/api/admin', adminGuard)  + adminAuthRouter / menuRouter / dictRouter
// app.use('/api/site', siteHomeRouter)
// app.use('/api/app', appGuard) + appAuthRouter
// app.use('/api/i18n', i18nRouter)

app.listen(PORT, () => {
  // eslint-disable-next-line no-console
  console.log(`[mock-server] running at http://localhost:${PORT}`)
})

export { adminGuard, appGuard }
```

> 注：mock-server 不纳入前端 ESLint 门禁（独立工程），console 允许。

- [ ] **Step 1.6: 安装依赖并验证启动**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\mock-server
pnpm install
pnpm dev
# 另开终端验证
Invoke-RestMethod http://localhost:7700/__mock/status
```

Expected: 返回 `code=200`，data 为 3 条状态记录。

- [ ] **Step 1.7: Commit**

```powershell
git add mock-server
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(mock): mock-server 基座（信封/ID/守卫/重置 + 7700 服务骨架）"
```

---

### Task 2: mock-server 骨架路由与种子数据（admin/site/app + i18n 托管）

**Files:**
- Create: `mock-server/src/data/basic.ts`
- Create: `mock-server/src/data/site.ts`
- Create: `mock-server/src/routes/admin/auth.ts`
- Create: `mock-server/src/routes/admin/menu.ts`
- Create: `mock-server/src/routes/admin/dict.ts`
- Create: `mock-server/src/routes/site/home.ts`
- Create: `mock-server/src/routes/app/auth.ts`
- Create: `mock-server/src/routes/i18n.ts`
- Create: `mock-server/src/i18n/zh-CN/common.json`、`zh-CN/menu.json`、`en-US/common.json`、`en-US/menu.json`
- Modify: `mock-server/src/server.ts`（挂载路由）
- Modify: `mock-server/README.md`（启动说明 + 状态表）

**Interfaces:**
- Consumes: Task 1 的 `ok/fail/paginate`、`guid/isoTime`、`adminGuard/appGuard`
- Produces（F0 骨架契约，Task 4/5/7/8 前端消费；后端 P1 交付时按 mock-guidelines 第 8 节对照）：
  - `POST /api/admin/auth/login` `{userName,password}` → `{accessToken, refreshToken, user{id,userName,realName}, permissions[]}`
  - `POST /api/admin/auth/refresh` `{refreshToken}` → `{accessToken}`
  - `GET /api/admin/basic/menu/list` → 菜单树数组
  - `GET /api/admin/basic/dict-data?typeCode=xxx` → 字典项数组 `{value,labelKey,sort}`
  - `GET /api/site/banner/list` → Banner 数组
  - `GET /api/site/news/list?pageIndex&pageSize&keyword` → 分页信封
  - `POST /api/app/auth/wx-login` `{code}` → `{memberToken, member{...}}`
  - `GET /api/i18n/:lang/:module.json` → 原始语言包 JSON（**不套信封**，模拟 nginx 静态文件）

- [ ] **Step 2.1: 种子数据 data/basic.ts**

```typescript
// src/data/basic.ts
// [backend: Basic 模块 | status: pending]
import { guid } from '../helpers/id.js'

export const ADMIN_USERS = [
  { id: guid(), userName: 'admin', password: 'admin123', realName: '系统管理员' },
  { id: guid(), userName: 'sales', password: 'sales123', realName: '销售专员' },
  { id: guid(), userName: 'ops', password: 'ops123', realName: '运维专员' },
]

/** 权限标识：super 用通配 '*'；其余账号 F2 Basic 模块时细化 */
export const USER_PERMISSIONS: Record<string, string[]> = {
  admin: ['*'],
  sales: ['crm:customer:list', 'crm:customer:add', 'mall:order:list'],
  ops: ['ops:log:list'],
}

/** 菜单树骨架：工作台 + 八大模块根（children 由 F2 Basic 模块补全） */
export const MENU_TREE = [
  { id: guid(), parentId: '0', name: 'desktop', path: '/desktop', titleKey: 'menu.desktop', icon: 'Monitor', sort: 1, children: [] },
  { id: guid(), parentId: '0', name: 'basic', path: '/basic', titleKey: 'menu.basic', icon: 'Setting', sort: 2, children: [] },
  { id: guid(), parentId: '0', name: 'product', path: '/product', titleKey: 'menu.product', icon: 'Goods', sort: 3, children: [] },
  { id: guid(), parentId: '0', name: 'site', path: '/site', titleKey: 'menu.site', icon: 'Monitor', sort: 4, children: [] },
  { id: guid(), parentId: '0', name: 'mall', path: '/mall', titleKey: 'menu.mall', icon: 'ShoppingCart', sort: 5, children: [] },
  { id: guid(), parentId: '0', name: 'crm', path: '/crm', titleKey: 'menu.crm', icon: 'User', sort: 6, children: [] },
  { id: guid(), parentId: '0', name: 'workflow', path: '/workflow', titleKey: 'menu.workflow', icon: 'Connection', sort: 7, children: [] },
  { id: guid(), parentId: '0', name: 'report', path: '/report', titleKey: 'menu.report', icon: 'DataLine', sort: 8, children: [] },
  { id: guid(), parentId: '0', name: 'ops', path: '/ops', titleKey: 'menu.ops', icon: 'Document', sort: 9, children: [] },
]

/** 字典种子：labelKey 走 i18n，前端 useDict 用 t(labelKey) 渲染 */
export const DICT_DATA: Record<string, Array<{ value: string; labelKey: string; sort: number }>> = {
  common_status: [
    { value: 'enabled', labelKey: 'common.status.enabled', sort: 1 },
    { value: 'disabled', labelKey: 'common.status.disabled', sort: 2 },
  ],
  customer_type: [
    { value: 'b2b', labelKey: 'common.dict.customerType.b2b', sort: 1 },
    { value: 'retail', labelKey: 'common.dict.customerType.retail', sort: 2 },
  ],
}
```

- [ ] **Step 2.2: 种子数据 data/site.ts**

```typescript
// src/data/site.ts
// [backend: Site 模块 | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

export const BANNERS = Mock.mock({
  'list|3': [{ id: '@guid', title: '@ctitle(8,16)', titleEn: '@title(3,5)', imageUrl: '@image(1920x600)', link: '' }],
}).list

export const NEWS = Mock.mock({
  'list|28': [{
    id: '@guid',
    categoryId: '@guid',
    title: '@ctitle(10,24)',
    titleEn: '@title(5,10)',
    summary: '@cparagraph(1,2)',
    coverImage: '@image(640x360)',
    isTop: '@boolean',
    'viewCount|100-9999': 1,
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((n: Record<string, unknown>) => ({ ...n, id: guid(), publishTime: isoTime() }))
```

- [ ] **Step 2.3: admin 分区路由（auth/menu/dict）**

```typescript
// src/routes/admin/auth.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'
import { ADMIN_USERS, USER_PERMISSIONS } from '../../data/basic.js'

export const adminAuthRouter = Router()

adminAuthRouter.post('/auth/login', (req, res) => {
  const { userName, password } = req.body as { userName?: string; password?: string }
  const user = ADMIN_USERS.find((u) => u.userName === userName && u.password === password)
  if (!user) return res.json(fail('账号或密码错误'))
  res.json(ok({
    accessToken: guid(),
    refreshToken: guid(),
    user: { id: user.id, userName: user.userName, realName: user.realName },
    permissions: USER_PERMISSIONS[user.userName] ?? [],
  }))
})

adminAuthRouter.post('/auth/refresh', (req, res) => {
  const { refreshToken } = req.body as { refreshToken?: string }
  if (!refreshToken) return res.json(fail('refreshToken 不能为空'))
  res.json(ok({ accessToken: guid() }))
})
```

```typescript
// src/routes/admin/menu.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { MENU_TREE } from '../../data/basic.js'

export const adminMenuRouter = Router()

adminMenuRouter.get('/basic/menu/list', (_req, res) => res.json(ok(MENU_TREE)))
```

```typescript
// src/routes/admin/dict.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { DICT_DATA } from '../../data/basic.js'

export const adminDictRouter = Router()

adminDictRouter.get('/basic/dict-data', (req, res) => {
  const typeCode = String(req.query.typeCode ?? '')
  const items = DICT_DATA[typeCode]
  if (!items) return res.json(fail(`字典 ${typeCode} 不存在`, 404))
  res.json(ok(items))
})
```

- [ ] **Step 2.4: site/app 分区路由 + i18n 托管路由**

```typescript
// src/routes/site/home.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { BANNERS, NEWS } from '../../data/site.js'

export const siteHomeRouter = Router()

siteHomeRouter.get('/banner/list', (_req, res) => res.json(ok(BANNERS)))

siteHomeRouter.get('/news/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined
  res.json(ok(paginate(NEWS, pageIndex, pageSize, keyword)))
})
```

```typescript
// src/routes/app/auth.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'

export const appAuthRouter = Router()

/** 不校验真实 code：任意 code 返回固定会员（mock-guidelines 6.2） */
appAuthRouter.post('/auth/wx-login', (req, res) => {
  const { code } = req.body as { code?: string }
  if (!code) return res.json(fail('code 不能为空'))
  res.json(ok({
    memberToken: guid(),
    member: { id: guid(), nickName: '演示会员', avatar: '', level: 'normal', points: 0 },
  }))
})
```

```typescript
// src/routes/i18n.ts
import { Router } from 'express'
import { readFileSync } from 'node:fs'
import { fileURLToPath } from 'node:url'
import path from 'node:path'

export const i18nRouter = Router()
const I18N_DIR = path.join(path.dirname(fileURLToPath(import.meta.url)), '../i18n')

/** 返回原始 JSON（不套信封）——模拟一期 nginx 静态托管 deploy/i18n/ */
i18nRouter.get('/:lang/:file', (req, res) => {
  const { lang, file } = req.params
  if (!/^(zh-CN|en-US)$/.test(lang) || !/^[a-z]+\.json$/.test(file)) {
    res.status(404).end()
    return
  }
  try {
    const json = readFileSync(path.join(I18N_DIR, lang, file), 'utf8')
    res.type('application/json').send(json)
  } catch {
    res.status(404).end()
  }
})
```

- [ ] **Step 2.5: i18n 托管文件（mock-server 侧基线，与前端包内基线同步维护）**

`src/i18n/zh-CN/common.json`：

```json
{
  "app": { "name": "EasyProduct" },
  "button": { "add": "新增", "edit": "编辑", "delete": "删除", "search": "搜索", "reset": "重置", "confirm": "确定", "cancel": "取消" },
  "tip": "提示",
  "confirm": { "delete": "确认删除该记录吗？" },
  "success": "操作成功",
  "required": "该项不能为空",
  "status": { "enabled": "启用", "disabled": "停用" },
  "dict": { "customerType": { "b2b": "B2B客户", "retail": "零售会员" } },
  "login": { "title": "登录", "userName": "账号", "password": "密码", "submit": "登 录", "logout": "退出登录" },
  "pager": { "total": "共 {total} 条" }
}
```

`src/i18n/en-US/common.json`：

```json
{
  "app": { "name": "EasyProduct" },
  "button": { "add": "Add", "edit": "Edit", "delete": "Delete", "search": "Search", "reset": "Reset", "confirm": "OK", "cancel": "Cancel" },
  "tip": "Notice",
  "confirm": { "delete": "Delete this record?" },
  "success": "Success",
  "required": "Required",
  "status": { "enabled": "Enabled", "disabled": "Disabled" },
  "dict": { "customerType": { "b2b": "B2B Customer", "retail": "Retail Member" } },
  "login": { "title": "Sign In", "userName": "Username", "password": "Password", "submit": "Sign In", "logout": "Sign Out" },
  "pager": { "total": "{total} items" }
}
```

`src/i18n/zh-CN/menu.json`：

```json
{
  "desktop": "工作台",
  "basic": "基础管理",
  "product": "商品中心",
  "site": "官网管理",
  "mall": "商城业务",
  "crm": "CRM",
  "workflow": "工作流",
  "report": "报表管理",
  "ops": "运维日志"
}
```

`src/i18n/en-US/menu.json`：

```json
{
  "desktop": "Dashboard",
  "basic": "Basic",
  "product": "Product",
  "site": "Site",
  "mall": "Mall",
  "crm": "CRM",
  "workflow": "Workflow",
  "report": "Report",
  "ops": "Ops"
}
```

- [ ] **Step 2.6: server.ts 挂载路由**

将 server.ts 中 Task 1 的占位注释替换为：

```typescript
import { adminAuthRouter } from './routes/admin/auth.js'
import { adminMenuRouter } from './routes/admin/menu.js'
import { adminDictRouter } from './routes/admin/dict.js'
import { siteHomeRouter } from './routes/site/home.js'
import { appAuthRouter } from './routes/app/auth.js'
import { i18nRouter } from './routes/i18n.js'

app.use('/api/admin', adminGuard, adminAuthRouter, adminMenuRouter, adminDictRouter)
app.use('/api/site', siteHomeRouter)
app.use('/api/app', appGuard, appAuthRouter)
app.use('/api/i18n', i18nRouter)
```

- [ ] **Step 2.7: 写 mock-server/README.md（启动说明 + 模块状态表）**

```markdown
# EasyProduct Mock Server

独立 Mock 服务：按后端契约提前实现三分区接口，三端共享。契约依据 docs/mock-guidelines.md。

## 启动

pnpm install
pnpm dev   # http://localhost:7700

管理端点：GET /__mock/status（状态表）、POST /__mock/reset（重置内存数据）

## 模块 Mock 状态表

| 分区 | 模块 | mock 状态 | 后端交付 | 切换日期 | 经手 |
|------|------|----------|---------|---------|------|
| admin | Basic（认证/菜单/字典骨架） | pending | P1 | — | — |
| site | 官网内容（首页聚合骨架） | pending | P2 | — | — |
| app | 商城（会员登录骨架） | pending | P3 | — | — |

状态值：pending（mock 生效中）→ deprecated（后端已交付、已切换）→ removed（P6 已清理）
```

- [ ] **Step 2.8: 验证骨架契约**

Run:

```powershell
$body = @{ userName = 'admin'; password = 'admin123' } | ConvertTo-Json
$login = Invoke-RestMethod -Method Post -Uri http://localhost:7700/api/admin/auth/login -Body $body -ContentType 'application/json'
$login.data.accessToken            # 期望：GUID 字符串
$token = $login.data.accessToken
Invoke-RestMethod -Uri http://localhost:7700/api/admin/basic/menu/list -Headers @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Uri 'http://localhost:7700/api/site/news/list?pageIndex=1&pageSize=5'
Invoke-RestMethod -Method Post -Uri http://localhost:7700/api/app/auth/wx-login -Body '{"code":"test"}' -ContentType 'application/json'
Invoke-RestMethod http://localhost:7700/api/i18n/zh-CN/common.json
# 无 Token 调管理接口应返回 code=401：
Invoke-RestMethod http://localhost:7700/api/admin/basic/menu/list
```

Expected: 各接口信封 `code=200`（最后一项 `code=401`），分页返回 `list/total`。

- [ ] **Step 2.9: Commit**

```powershell
git add mock-server
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(mock): 三端骨架路由（admin 登录/菜单/字典、site 首页聚合、app 微信登录、i18n 托管）"
```

---
### Task 3: 仓库根工程化（Node 锁定 + husky/commitlint/lint-staged）+ .gitignore 补充

**Files:**
- Create: `.nvmrc`
- Create: `package.json`（根，仅 git 钩子，非 workspace）
- Create: `commitlint.config.cjs`
- Create: `lint-staged.config.cjs`
- Create: `.husky/pre-commit`
- Create: `.husky/commit-msg`
- Modify: `.gitignore`（补 node_modules/dist/.env.local 等）

**Interfaces:**
- Consumes: 无
- Produces: 后续所有 commit 走 commitlint 校验；暂存的 Admin/Site 文件自动 eslint --fix。

- [ ] **Step 3.1: .nvmrc 与根 package.json**

`.nvmrc`：

```text
20
```

`package.json`：

```json
{
  "name": "easyproduct",
  "private": true,
  "scripts": {
    "prepare": "husky"
  },
  "devDependencies": {
    "@commitlint/cli": "^19.2.1",
    "@commitlint/config-conventional": "^19.1.0",
    "husky": "^9.0.11",
    "lint-staged": "^15.2.2"
  }
}
```

- [ ] **Step 3.2: commitlint 与 lint-staged 配置**

`commitlint.config.cjs`：

```javascript
module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'scope-enum': [1, 'always', ['admin', 'site', 'miniapp', 'api', 'mock']],
  },
}
```

`lint-staged.config.cjs`：

```javascript
// 按路径把暂存文件路由到对应 app 的 ESLint（三 app 独立 package.json，不做 workspace）
module.exports = {
  'EasyProduct.Admin/**/*.{vue,ts}': () => 'cd EasyProduct.Admin && npx eslint --ext .vue,.ts --fix',
  'EasyProduct.Site/**/*.{vue,ts}': () => 'cd EasyProduct.Site && npx eslint --ext .vue,.ts --fix',
}
```

- [ ] **Step 3.3: husky 钩子**

`.husky/pre-commit`：

```sh
npx lint-staged
```

`.husky/commit-msg`：

```sh
npx --no-install commitlint --edit "$1"
```

- [ ] **Step 3.4: .gitignore 补充（在现有文件末尾追加）**

```text
# deps & build
node_modules/
dist/
*.local
.env.local
# editors & os
.DS_Store
*.log
# wechat devtools
EasyProduct.MiniApp/project.private.config.json
```

- [ ] **Step 3.5: 安装根钩子并验证**

Run:

```powershell
cd D:\4-MyProject\EasyProduct
pnpm install
echo 'bad message' | Out-File -Encoding utf8 test-msg.txt
npx commitlint --config commitlint.config.cjs --file test-msg.txt   # 期望：报错（格式不合规）
Remove-Item test-msg.txt
```

Expected: commitlint 对不合规消息报错，钩子可用。

- [ ] **Step 3.6: Commit**

```powershell
git add .nvmrc package.json pnpm-lock.yaml commitlint.config.cjs lint-staged.config.cjs .husky .gitignore
git -c user.name='lilin' -c user.email='lilin@local' commit -m "chore: 仓库根工程化（husky+commitlint+lint-staged，Node 20 锁定）"
```

---

### Task 4: Admin 工程配置（Vite+TS+ESLint+Prettier+check:i18n）

**Files:**
- Create: `EasyProduct.Admin/package.json`
- Create: `EasyProduct.Admin/vite.config.ts`
- Create: `EasyProduct.Admin/tsconfig.json`
- Create: `EasyProduct.Admin/index.html`
- Create: `EasyProduct.Admin/env.d.ts`
- Create: `EasyProduct.Admin/.env.development`
- Create: `EasyProduct.Admin/.eslintrc.cjs`
- Create: `EasyProduct.Admin/.prettierrc.json`
- Create: `EasyProduct.Admin/scripts/check-chinese.cjs`
- Create: `EasyProduct.Admin/src/main.ts`（最小入口，Task 5 补全）
- Create: `EasyProduct.Admin/src/App.vue`（最小壳）

**Interfaces:**
- Consumes: Task 3 的 lint-staged（会对 Admin 暂存文件跑 eslint）
- Produces: `pnpm dev` 可启动（端口 5173）；`pnpm lint` / `pnpm check:i18n` / `pnpm type-check` 脚本就绪；后续任务的所有源码在此工程内编译。

- [ ] **Step 4.1: package.json**

```json
{
  "name": "easyproduct-admin",
  "version": "0.1.0",
  "private": true,
  "type": "module",
  "engines": { "node": ">=20" },
  "scripts": {
    "dev": "vite --port 5173",
    "build": "vue-tsc --noEmit && vite build",
    "preview": "vite preview",
    "type-check": "vue-tsc --noEmit",
    "lint": "eslint . --ext .vue,.ts --fix",
    "format": "prettier --write src",
    "check:i18n": "node scripts/check-chinese.cjs"
  },
  "dependencies": {
    "@element-plus/icons-vue": "^2.3.1",
    "axios": "^1.6.8",
    "dayjs": "^1.11.10",
    "echarts": "^6.0.0",
    "element-plus": "^2.6.3",
    "pinia": "^2.1.7",
    "vue": "^3.4.21",
    "vue-echarts": "^8.0.1",
    "vue-i18n": "^9.9.1",
    "vue-router": "^4.3.0"
  },
  "devDependencies": {
    "@typescript-eslint/eslint-plugin": "^7.4.0",
    "@typescript-eslint/parser": "^7.4.0",
    "@vitejs/plugin-vue": "^5.0.4",
    "eslint": "^8.57.0",
    "eslint-plugin-vue": "^9.24.0",
    "prettier": "^3.2.5",
    "sass": "^1.72.0",
    "typescript": "^5.3.3",
    "vite": "^5.4.0",
    "vue-tsc": "^2.0.7"
  }
}
```

- [ ] **Step 4.2: vite.config.ts（proxy 目标环境化，F0 指向 mock 7700）**

```typescript
import { fileURLToPath, URL } from 'node:url'
import vue from '@vitejs/plugin-vue'
import { defineConfig, loadEnv } from 'vite'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd())
  return {
    plugins: [vue()],
    resolve: {
      alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) },
    },
    server: {
      port: 5173,
      proxy: {
        // F0~mock 阶段指向 7700；后端联调改 .env.development 的 VITE_PROXY_TARGET=http://localhost:7600
        '/api': {
          target: env.VITE_PROXY_TARGET || 'http://localhost:7700',
          changeOrigin: true,
        },
      },
    },
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: '@use "@/styles/variables.scss" as *;@use "@/styles/mixins.scss" as *;',
        },
      },
    },
  }
})
```

- [ ] **Step 4.3: tsconfig.json 与 env.d.ts**

`tsconfig.json`：

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "ESNext",
    "moduleResolution": "Bundler",
    "lib": ["ES2022", "DOM", "DOM.Iterable"],
    "strict": true,
    "jsx": "preserve",
    "resolveJsonModule": true,
    "isolatedModules": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "noEmit": true,
    "baseUrl": ".",
    "paths": { "@/*": ["src/*"] },
    "types": ["vite/client"]
  },
  "include": ["env.d.ts", "src/**/*.ts", "src/**/*.d.ts", "src/**/*.vue"],
  "exclude": ["node_modules", "dist"]
}
```

`env.d.ts`：

```typescript
/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string
  readonly VITE_PROXY_TARGET: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
```

- [ ] **Step 4.4: index.html 与 .env.development**

`index.html`：

```html
<!doctype html>
<html lang="zh-CN">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>EasyProduct Admin</title>
  </head>
  <body>
    <div id="app"></div>
    <script type="module" src="/src/main.ts"></script>
  </body>
</html>
```

`.env.development`：

```text
# 走 vite proxy（同源），baseURL 留空
VITE_API_BASE_URL=
# mock-first：指向 mock-server；后端交付后按需改 http://localhost:7600
VITE_PROXY_TARGET=http://localhost:7700
```

- [ ] **Step 4.5: .eslintrc.cjs 与 .prettierrc.json（Site 端 Task 6 原样复用这两个文件）**

`.eslintrc.cjs`：

```javascript
module.exports = {
  root: true,
  env: { browser: true, node: true, es2022: true },
  extends: [
    'plugin:vue/vue3-recommended',
    'eslint:recommended',
    'plugin:@typescript-eslint/recommended',
  ],
  parser: 'vue-eslint-parser',
  parserOptions: {
    parser: '@typescript-eslint/parser',
    ecmaVersion: 2022,
    sourceType: 'module',
  },
  plugins: ['@typescript-eslint'],
  rules: {
    '@typescript-eslint/no-explicit-any': 'error',
    'no-console': 'error',
    'vue/multi-word-component-names': 'off',
  },
  ignorePatterns: ['dist', 'node_modules', '*.cjs', 'scripts'],
}
```

`.prettierrc.json`：

```json
{
  "semi": false,
  "singleQuote": true,
  "printWidth": 110,
  "trailingComma": "all",
  "endOfLine": "auto"
}
```

- [ ] **Step 4.6: scripts/check-chinese.cjs（硬编码中文检测门禁）**

```javascript
// 扫描 src 下 .vue/.ts 中的硬编码中文（i18n 语言包目录除外）
const fs = require('fs')
const path = require('path')

const SRC = path.resolve(__dirname, '../src')
const IGNORE_DIRS = ['i18n']
const EXTS = ['.vue', '.ts']
const CJK = /[\u4e00-\u9fa5]/
let hits = 0

function walk(dir) {
  for (const name of fs.readdirSync(dir)) {
    const full = path.join(dir, name)
    const stat = fs.statSync(full)
    if (stat.isDirectory()) {
      if (!IGNORE_DIRS.includes(name)) walk(full)
      continue
    }
    if (!EXTS.includes(path.extname(name))) continue
    const lines = fs.readFileSync(full, 'utf8').split('\n')
    lines.forEach((line, i) => {
      const trimmed = line.trim()
      if (CJK.test(line) && !trimmed.startsWith('//') && !trimmed.startsWith('*')) {
        console.error(`${path.relative(SRC, full)}:${i + 1}: ${trimmed}`)
        hits += 1
      }
    })
  }
}

walk(SRC)
if (hits > 0) {
  console.error(`check:i18n FAILED: ${hits} hardcoded Chinese line(s)`)
  process.exit(1)
}
console.log('check:i18n passed')
```

- [ ] **Step 4.7: 最小入口 main.ts 与 App.vue（Task 5 替换 main.ts 为完整引导）**

`src/main.ts`：

```typescript
import { createApp } from 'vue'
import App from './App.vue'

createApp(App).mount('#app')
```

`src/App.vue`：

```vue
<template>
  <div class="app-boot">EasyProduct Admin</div>
</template>

<script setup lang="ts"></script>

<style scoped lang="scss">
.app-boot {
  padding: 24px;
}
</style>
```

- [ ] **Step 4.8: 安装并验证工程可启动**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm install
pnpm dev        # 浏览器 http://localhost:5173 应显示 "EasyProduct Admin"
pnpm lint
pnpm check:i18n
```

Expected: dev 启动无错误；lint 通过；check:i18n 通过（此时 src 无中文）。

- [ ] **Step 4.9: Commit**

```powershell
git add EasyProduct.Admin
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): 工程配置（Vite5+TS strict+ESLint+Prettier+check:i18n，proxy 指向 mock 7700）"
```

---

### Task 5: Admin 核心层（类型/请求封装/样式/i18n/stores/基础 API）

**Files:**
- Create: `EasyProduct.Admin/src/types/api.ts`
- Create: `EasyProduct.Admin/src/types/basic.ts`
- Create: `EasyProduct.Admin/src/utils/auth.ts`
- Create: `EasyProduct.Admin/src/utils/request.ts`
- Create: `EasyProduct.Admin/src/styles/variables.scss`
- Create: `EasyProduct.Admin/src/styles/mixins.scss`
- Create: `EasyProduct.Admin/src/styles/index.scss`
- Create: `EasyProduct.Admin/src/i18n/index.ts`
- Create: `EasyProduct.Admin/src/i18n/zh-CN/common.json`、`zh-CN/menu.json`、`en-US/common.json`、`en-US/menu.json`
- Create: `EasyProduct.Admin/src/stores/user.ts`
- Create: `EasyProduct.Admin/src/stores/app.ts`
- Create: `EasyProduct.Admin/src/api/basic/auth.ts`
- Create: `EasyProduct.Admin/src/api/basic/menu.ts`
- Create: `EasyProduct.Admin/src/api/basic/dict.ts`

**Interfaces:**
- Consumes: Task 2 的 mock 契约（login/refresh/menu/dict-data/i18n URL 逐字一致）
- Produces: `ApiResponse/PageQuery/PageResult`（types/api）；`LoginParams/LoginResult/MenuItem/DictItem`（types/basic）；`get/post/put/del<T>`（utils/request）；`getAccessToken/setTokens/clearTokens`（utils/auth）；`i18n/loadModuleLocale/setLocale/SUPPORT_LOCALES`（i18n）；`useUserStore`（login/logout/has）与 `useAppStore`（sidebar/theme/locale）；`login/refresh/getMenuList/getDictData`（api/basic/*）。Task 6 的守卫/布局/页面全部消费这些导出。

- [ ] **Step 5.1: types/api.ts 与 types/basic.ts**

```typescript
// src/types/api.ts
/** 统一响应信封（HTTP 一律 200，code===200 成功） */
export interface ApiResponse<T> {
  code: number
  message: string
  data: T
  timestamp: number
}

/** 分页查询参数 */
export interface PageQuery {
  pageIndex: number
  pageSize: number
}

/** 分页响应 */
export interface PageResult<T> {
  list: T[]
  total: number
}
```

```typescript
// src/types/basic.ts
/** 登录入参 */
export interface LoginParams {
  userName: string
  password: string
}

/** 登录结果 */
export interface LoginResult {
  accessToken: string
  refreshToken: string
  user: { id: string; userName: string; realName: string }
  permissions: string[]
}

/** 菜单项（树） */
export interface MenuItem {
  id: string
  parentId: string
  name: string
  path: string
  titleKey: string
  icon: string
  sort: number
  children: MenuItem[]
}

/** 字典项（labelKey 走 i18n 渲染） */
export interface DictItem {
  value: string
  labelKey: string
  sort: number
}
```

- [ ] **Step 5.2: utils/auth.ts 与 utils/request.ts（信封拦截器，规范 1.2）**

```typescript
// src/utils/auth.ts
const ACCESS_TOKEN = 'access_token'
const REFRESH_TOKEN = 'refresh_token'

export const getAccessToken = (): string => localStorage.getItem(ACCESS_TOKEN) ?? ''
export const getRefreshToken = (): string => localStorage.getItem(REFRESH_TOKEN) ?? ''

export function setTokens(accessToken: string, refreshToken: string): void {
  localStorage.setItem(ACCESS_TOKEN, accessToken)
  localStorage.setItem(REFRESH_TOKEN, refreshToken)
}

export function clearTokens(): void {
  localStorage.removeItem(ACCESS_TOKEN)
  localStorage.removeItem(REFRESH_TOKEN)
}
```

```typescript
// src/utils/request.ts
import axios from 'axios'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ApiResponse } from '@/types/api'
import { clearTokens, getAccessToken } from '@/utils/auth'

const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '',
  timeout: 15000,
})

service.interceptors.request.use((config) => {
  const token = getAccessToken()
  if (token) config.headers.Authorization = `Bearer ${token}`
  config.headers['Accept-Language'] = localStorage.getItem('locale') || 'zh-CN'
  return config
})

let tokenExpired = false // 防止 401 多次弹窗

service.interceptors.response.use(
  (response) => {
    const res = response.data as ApiResponse<unknown>
    if (res.code === 200) return res.data as never
    if (res.code === 401 && !tokenExpired) {
      tokenExpired = true
      clearTokens()
      const redirect = encodeURIComponent(window.location.pathname + window.location.search)
      ElMessageBox.confirm('common.error.tokenExpired', 'common.tip', { type: 'warning' })
        .finally(() => {
          tokenExpired = false
          window.location.href = `/login?redirect=${redirect}`
        })
      return Promise.reject(new Error(res.message))
    }
    if (res.code === 403) ElMessage.error('common.error.forbidden')
    else ElMessage.error(res.message || 'common.error.request')
    return Promise.reject(new Error(res.message))
  },
  (error: unknown) => {
    ElMessage.error('common.error.network')
    return Promise.reject(error instanceof Error ? error : new Error(String(error)))
  },
)

export function get<T>(url: string, params?: unknown): Promise<T> {
  return service.get(url, { params }) as Promise<T>
}
export function post<T>(url: string, data?: unknown): Promise<T> {
  return service.post(url, data) as Promise<T>
}
export function put<T>(url: string, data?: unknown): Promise<T> {
  return service.put(url, data) as Promise<T>
}
export function del<T>(url: string, params?: unknown): Promise<T> {
  return service.delete(url, { params }) as Promise<T>
}
```

> 注：拦截器提示用 i18n key，ElMessage 显示原文案需在 Task 6 由 `useLocale` 注入后包装；F0 骨架阶段 ElMessage 直接吃 key 可接受（common.error.* 文案在 Task 6 页面层统一经 t() 处理），若评审要求拦截器内文案也翻译，可在 i18n/index.ts 暴露 `tStandalone()` 供 request.ts 使用，实现代价一行。

- [ ] **Step 5.3: styles 三件套（CSS Variables 亮/暗主题）**

```scss
// src/styles/variables.scss（SCSS 静态常量：间距/圆角/字号/断点）
$spacing-xs: 4px;
$spacing-sm: 8px;
$spacing-md: 16px;
$spacing-lg: 24px;
$spacing-xl: 32px;
$radius-sm: 4px;
$radius-md: 8px;
$font-size-sm: 13px;
$font-size-md: 14px;
$font-size-lg: 16px;
$admin-min-width: 1280px;
```

```scss
// src/styles/mixins.scss
@mixin ellipsis($lines: 1) {
  @if $lines == 1 {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  } @else {
    display: -webkit-box;
    -webkit-box-orient: vertical;
    -webkit-line-clamp: $lines;
    overflow: hidden;
  }
}

@mixin scrollbar-thin {
  &::-webkit-scrollbar {
    width: 6px;
    height: 6px;
  }
  &::-webkit-scrollbar-thumb {
    border-radius: $radius-sm;
    background: var(--ep-scrollbar);
  }
}
```

```scss
// src/styles/index.scss（全局样式唯一入口；主题色一律 CSS Variables）
:root {
  --ep-primary: #409eff;
  --ep-bg: #f5f7fa;
  --ep-bg-card: #ffffff;
  --ep-text: #303133;
  --ep-text-secondary: #909399;
  --ep-border: #dcdfe6;
  --ep-scrollbar: #c0c4cc;
}

[data-theme='dark'] {
  --ep-bg: #141414;
  --ep-bg-card: #1d1e1f;
  --ep-text: #e5eaf3;
  --ep-text-secondary: #a3a6ad;
  --ep-border: #4c4d4f;
  --ep-scrollbar: #6c6e72;
}

html,
body,
#app {
  height: 100%;
  margin: 0;
  min-width: $admin-min-width;
  background: var(--ep-bg);
  color: var(--ep-text);
  font-size: $font-size-md;
}
```

- [ ] **Step 5.4: i18n 基线与加载器（内置基线 + 远程覆盖，规范 1.4）**

`src/i18n/zh-CN/common.json` 与 `en-US/common.json`：内容与 mock-server Step 2.5 的两份 common.json **逐字一致**（前端包内基线 = mock 托管基线），另在 `error` 节点补充三条：

```json
"error": { "tokenExpired": "登录状态已过期，请重新登录", "forbidden": "没有操作权限", "network": "网络错误", "request": "请求失败" }
```

（en-US 对应：`"error": { "tokenExpired": "Session expired, please sign in again", "forbidden": "Permission denied", "network": "Network error", "request": "Request failed" }`）

`src/i18n/zh-CN/menu.json` 与 `en-US/menu.json`：与 Step 2.5 逐字一致。

`src/i18n/index.ts`：

```typescript
import { createI18n } from 'vue-i18n'
import enCommon from './en-US/common.json'
import enMenu from './en-US/menu.json'
import zhCommon from './zh-CN/common.json'
import zhMenu from './zh-CN/menu.json'

export const SUPPORT_LOCALES = ['zh-CN', 'en-US'] as const
export type Locale = (typeof SUPPORT_LOCALES)[number]

const LOCALE_KEY = 'locale'

const messages: Record<Locale, Record<string, unknown>> = {
  'zh-CN': { common: zhCommon, menu: zhMenu },
  'en-US': { common: enCommon, menu: enMenu },
}

export const i18n = createI18n({
  legacy: false,
  locale: (localStorage.getItem(LOCALE_KEY) as Locale) || 'zh-CN',
  fallbackLocale: 'zh-CN',
  messages,
})

const loaded = new Set<string>()

/** 加载业务模块语言包：远程覆盖优先，失败用包内基线（免发布维护机制） */
export async function loadModuleLocale(module: string): Promise<void> {
  const locale = i18n.global.locale.value as Locale
  const key = `${locale}:${module}`
  if (loaded.has(key)) return
  try {
    const res = await fetch(`/api/i18n/${locale}/${module}.json`)
    if (res.ok) {
      const data = (await res.json()) as Record<string, unknown>
      i18n.global.mergeLocaleMessage(locale, { [module]: data })
    }
  } catch {
    // 远程失败：使用包内基线，不阻塞
  }
  loaded.add(key)
}

export function setLocale(locale: Locale): void {
  i18n.global.locale.value = locale
  localStorage.setItem(LOCALE_KEY, locale)
  document.documentElement.lang = locale
}

/** 供非组件上下文（如 utils/request）使用的翻译函数 */
export const tStandalone = (key: string): string => i18n.global.t(key)
```

- [ ] **Step 5.5: stores（user/app）**

```typescript
// src/stores/user.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import { login as loginApi } from '@/api/basic/auth'
import { clearTokens, getAccessToken, setTokens } from '@/utils/auth'

export const useUserStore = defineStore('user', () => {
  const token = ref(getAccessToken())
  const realName = ref('')
  const permissions = ref<string[]>([])

  async function login(userName: string, password: string): Promise<void> {
    const data = await loginApi({ userName, password })
    setTokens(data.accessToken, data.refreshToken)
    token.value = data.accessToken
    realName.value = data.user.realName
    permissions.value = data.permissions
  }

  function logout(): void {
    clearTokens()
    token.value = ''
    realName.value = ''
    permissions.value = []
  }

  /** 按钮级权限判断（F2 usePermission/v-permission 复用此逻辑） */
  function has(code: string): boolean {
    return permissions.value.includes('*') || permissions.value.includes(code)
  }

  return { token, realName, permissions, login, logout, has }
})
```

```typescript
// src/stores/app.ts
import { defineStore } from 'pinia'
import { ref, watch } from 'vue'
import type { Locale } from '@/i18n'
import { setLocale } from '@/i18n'

export const useAppStore = defineStore('app', () => {
  const sidebarCollapsed = ref(false)
  const theme = ref<'light' | 'dark'>((localStorage.getItem('theme') as 'light' | 'dark') || 'light')
  const locale = ref<Locale>((localStorage.getItem('locale') as Locale) || 'zh-CN')

  function toggleSidebar(): void {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  function toggleTheme(): void {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }

  function switchLocale(next: Locale): void {
    locale.value = next
    setLocale(next)
  }

  watch(
    theme,
    (v) => {
      document.documentElement.dataset.theme = v
      localStorage.setItem('theme', v)
    },
    { immediate: true },
  )

  return { sidebarCollapsed, theme, locale, toggleSidebar, toggleTheme, switchLocale }
})
```

- [ ] **Step 5.6: api/basic 三文件（auth/menu/dict）**

```typescript
// src/api/basic/auth.ts
import { post } from '@/utils/request'
import type { LoginParams, LoginResult } from '@/types/basic'

/** 管理端登录 */
export const login = (data: LoginParams) => post<LoginResult>('/api/admin/auth/login', data)

/** 刷新 accessToken */
export const refresh = (data: { refreshToken: string }) =>
  post<{ accessToken: string }>('/api/admin/auth/refresh', data)
```

```typescript
// src/api/basic/menu.ts
import { get } from '@/utils/request'
import type { MenuItem } from '@/types/basic'

/** 当前用户菜单树 */
export const getMenuList = () => get<MenuItem[]>('/api/admin/basic/menu/list')
```

```typescript
// src/api/basic/dict.ts
import { get } from '@/utils/request'
import type { DictItem } from '@/types/basic'

/** 按字典类型取字典项 */
export const getDictData = (typeCode: string) =>
  get<DictItem[]>('/api/admin/basic/dict-data', { typeCode })
```

- [ ] **Step 5.7: 验证核心层**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
pnpm lint
pnpm check:i18n
```

Expected: 三项全绿（main.ts 仍是 Task 4 最小入口，核心层已被编译检查覆盖）。

- [ ] **Step 5.8: Commit**

```powershell
git add EasyProduct.Admin
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): 核心层（request 信封拦截/样式变量/模块化 i18n+远程覆盖/user/app stores/basic API）"
```

---
### Task 6: Admin 路由守卫 + 布局 + 登录/工作台/占位/404

**Files:**
- Modify: `EasyProduct.Admin/src/main.ts`（替换为完整引导）
- Create: `EasyProduct.Admin/src/router/index.ts`
- Create: `EasyProduct.Admin/src/router/guards.ts`
- Create: `EasyProduct.Admin/src/router/modules/placeholder.ts`
- Create: `EasyProduct.Admin/src/layouts/MainLayout.vue`
- Create: `EasyProduct.Admin/src/layouts/components/AppSidebar.vue`
- Create: `EasyProduct.Admin/src/layouts/components/AppTopbar.vue`
- Create: `EasyProduct.Admin/src/views/login/index.vue`
- Create: `EasyProduct.Admin/src/views/desktop/index.vue`
- Create: `EasyProduct.Admin/src/views/placeholder/index.vue`
- Create: `EasyProduct.Admin/src/views/error/404.vue`

**Interfaces:**
- Consumes: Task 5 全部核心层（useUserStore/useAppStore/getMenuList/loadModuleLocale/tStandalone）
- Produces: 可登录、可导航的 Admin 骨架；路由表结构（`router/modules/*.ts` 按域拆分）为 F2 各模块追加路由预留；`meta.title`（i18n key）/`meta.icon`/`meta.permission` 约定生效。

- [ ] **Step 6.1: main.ts 完整引导**

```typescript
// src/main.ts
import { createPinia } from 'pinia'
import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'
import { i18n } from './i18n'
import { setupRouter } from './router'
import './styles/index.scss'

const app = createApp(App)
app.use(createPinia())
app.use(i18n)
app.use(ElementPlus)
setupRouter(app)
app.mount('#app')
```

同时把 App.vue 简化为纯出口：

```vue
<template>
  <router-view />
</template>

<script setup lang="ts"></script>
```

- [ ] **Step 6.2: router/modules/placeholder.ts（八大模块占位路由，F2 逐模块替换）**

```typescript
// src/router/modules/placeholder.ts
import type { RouteRecordRaw } from 'vue-router'

const MODULES = ['basic', 'product', 'site', 'mall', 'crm', 'workflow', 'report', 'ops'] as const

export const placeholderRoutes: RouteRecordRaw[] = MODULES.map((name) => ({
  path: `/${name}`,
  name: `placeholder-${name}`,
  component: () => import('@/views/placeholder/index.vue'),
  meta: { title: `menu.${name}`, icon: 'Menu' },
}))
```

- [ ] **Step 6.3: router/index.ts 与 guards.ts**

```typescript
// src/router/index.ts
import type { App } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { setupGuards } from './guards'
import { placeholderRoutes } from './modules/placeholder'

export const routes: RouteRecordRaw[] = [
  { path: '/login', name: 'login', component: () => import('@/views/login/index.vue'), meta: { title: 'common.login.title' } },
  {
    path: '/',
    component: () => import('@/layouts/MainLayout.vue'),
    redirect: '/desktop',
    children: [
      { path: 'desktop', name: 'desktop', component: () => import('@/views/desktop/index.vue'), meta: { title: 'menu.desktop', icon: 'Monitor' } },
      ...placeholderRoutes,
    ],
  },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/error/404.vue'), meta: { title: 'menu.notFound' } },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

export function setupRouter(app: App): void {
  setupGuards(router)
  app.use(router)
}
```

```typescript
// src/router/guards.ts
import type { Router } from 'vue-router'
import { getAccessToken } from '@/utils/auth'
import { i18n } from '@/i18n'

const WHITE_LIST = ['/login']

export function setupGuards(router: Router): void {
  router.beforeEach((to) => {
    const token = getAccessToken()
    if (!token && !WHITE_LIST.includes(to.path)) {
      return { path: '/login', query: { redirect: to.fullPath } }
    }
    if (token && to.path === '/login') return { path: '/desktop' }
    return true
  })

  router.afterEach((to) => {
    const key = to.meta.title as string | undefined
    const title = key ? i18n.global.t(key) : ''
    document.title = title ? `${title} - EasyProduct` : 'EasyProduct'
  })
}
```

- [ ] **Step 6.4: 布局（MainLayout + AppSidebar + AppTopbar）**

```vue
<!-- src/layouts/MainLayout.vue -->
<template>
  <el-container class="main-layout">
    <el-aside :width="appStore.sidebarCollapsed ? '64px' : '220px'">
      <AppSidebar />
    </el-aside>
    <el-container>
      <el-header class="main-layout__header">
        <AppTopbar />
      </el-header>
      <el-main class="main-layout__body">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
import AppSidebar from './components/AppSidebar.vue'
import AppTopbar from './components/AppTopbar.vue'
import { useAppStore } from '@/stores/app'

const appStore = useAppStore()
</script>

<style scoped lang="scss">
.main-layout {
  height: 100%;

  &__header {
    display: flex;
    align-items: center;
    border-bottom: 1px solid var(--ep-border);
    background: var(--ep-bg-card);
  }

  &__body {
    padding: $spacing-md;
  }
}
</style>
```

```vue
<!-- src/layouts/components/AppSidebar.vue -->
<template>
  <div class="app-sidebar">
    <div class="app-sidebar__logo">{{ appStore.sidebarCollapsed ? 'EP' : t('common.app.name') }}</div>
    <el-menu :collapse="appStore.sidebarCollapsed" :default-active="route.path" router class="app-sidebar__menu">
      <el-menu-item v-for="item in menus" :key="item.id" :index="item.path">
        <span>{{ t(item.titleKey) }}</span>
      </el-menu-item>
    </el-menu>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getMenuList } from '@/api/basic/menu'
import { useAppStore } from '@/stores/app'
import { useI18n } from 'vue-i18n'
import type { MenuItem } from '@/types/basic'

const { t } = useI18n()
const route = useRoute()
const appStore = useAppStore()
const menus = ref<MenuItem[]>([])

onMounted(async () => {
  menus.value = await getMenuList()
})
</script>

<style scoped lang="scss">
.app-sidebar {
  height: 100%;
  background: var(--ep-bg-card);
  border-right: 1px solid var(--ep-border);

  &__logo {
    height: 56px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 600;
  }

  &__menu {
    border-right: none;
  }
}
</style>
```

```vue
<!-- src/layouts/components/AppTopbar.vue -->
<template>
  <div class="app-topbar">
    <el-button text @click="appStore.toggleSidebar()">
      {{ appStore.sidebarCollapsed ? '>' : '<' }}
    </el-button>
    <div class="app-topbar__right">
      <el-switch
        :model-value="appStore.locale === 'zh-CN'"
        active-text="中文"
        inactive-text="EN"
        @change="handleLocaleChange"
      />
      <el-button text @click="appStore.toggleTheme()">{{ appStore.theme }}</el-button>
      <span class="app-topbar__user">{{ userStore.realName }}</span>
      <el-button text @click="handleLogout">{{ t('common.login.logout') }}</el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/app'
import { useUserStore } from '@/stores/user'

const { t } = useI18n()
const router = useRouter()
const appStore = useAppStore()
const userStore = useUserStore()

const handleLocaleChange = (zh: boolean | string | number) => {
  appStore.switchLocale(zh ? 'zh-CN' : 'en-US')
}

const handleLogout = () => {
  userStore.logout()
  router.push('/login')
}
</script>

<style scoped lang="scss">
.app-topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  &__right {
    display: flex;
    align-items: center;
    gap: $spacing-md;
  }

  &__user {
    color: var(--ep-text-secondary);
  }
}
</style>
```

- [ ] **Step 6.5: 页面（login/desktop/placeholder/404）**

```vue
<!-- src/views/login/index.vue -->
<template>
  <div class="login-page">
    <el-card class="login-page__card">
      <h2 class="login-page__title">{{ t('common.login.title') }}</h2>
      <el-form ref="formRef" :model="model" :rules="rules" label-width="0" size="large">
        <el-form-item prop="userName">
          <el-input v-model="model.userName" :placeholder="t('common.login.userName')" />
        </el-form-item>
        <el-form-item prop="password">
          <el-input v-model="model.password" type="password" show-password :placeholder="t('common.login.password')" @keyup.enter="handleSubmit" />
        </el-form-item>
        <el-button type="primary" :loading="loading" class="login-page__submit" @click="handleSubmit">
          {{ t('common.login.submit') }}
        </el-button>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { FormInstance, FormRules } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/user'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const userStore = useUserStore()

const formRef = ref<FormInstance>()
const loading = ref(false)
const model = reactive({ userName: 'admin', password: 'admin123' })

const rules: FormRules = {
  userName: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  password: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  loading.value = true
  try {
    await userStore.login(model.userName, model.password)
    const redirect = (route.query.redirect as string) || '/desktop'
    router.push(redirect)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
.login-page {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;

  &__card {
    width: 380px;
  }

  &__title {
    text-align: center;
    margin-bottom: $spacing-lg;
  }

  &__submit {
    width: 100%;
  }
}
</style>
```

```vue
<!-- src/views/desktop/index.vue -->
<template>
  <div class="desktop-page">
    <el-card>
      <template #header>{{ t('menu.desktop') }}</template>
      <p class="desktop-page__hello">{{ userStore.realName }}，EasyProduct</p>
      <el-row :gutter="16">
        <el-col v-for="m in MODULES" :key="m" :span="6">
          <el-card shadow="hover" class="desktop-page__widget">
            {{ t(`menu.${m}`) }}
          </el-card>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/user'

const { t } = useI18n()
const userStore = useUserStore()

/** F2 工作台 Widget 化的八大域占位（经营概览/KPI/待办/预警在 F2-9 落位） */
const MODULES = ['basic', 'product', 'site', 'mall', 'crm', 'workflow', 'report', 'ops'] as const
</script>

<style scoped lang="scss">
.desktop-page {
  &__hello {
    color: var(--ep-text-secondary);
  }

  &__widget {
    margin-bottom: $spacing-md;
    text-align: center;
  }
}
</style>
```

```vue
<!-- src/views/placeholder/index.vue -->
<template>
  <el-empty :description="t('common.placeholder')" />
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
</script>
```

（common.json 补键：zh `"placeholder": "模块建设中"`；en `"placeholder": "Coming soon"`——两侧 common.json 与 mock-server 托管基线同步补。）

```vue
<!-- src/views/error/404.vue -->
<template>
  <div class="not-found">
    <h1>404</h1>
    <router-link to="/desktop">{{ t('menu.desktop') }}</router-link>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
</script>

<style scoped lang="scss">
.not-found {
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: $spacing-md;
}
</style>
```

- [ ] **Step 6.6: 验证 Admin 骨架闭环**

Run（mock-server 保持运行）：

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm dev
```

手工验证：http://localhost:5173 → 未登录跳 /login → admin/admin123 登录 → 工作台显示八大域卡片 → 侧栏菜单 9 项（mock 菜单树）→ 点击 CRM 进占位页 → 直接访问 /xyz 落 404 → 退出回登录页；顶栏中英文切换生效。

```powershell
pnpm type-check
pnpm lint
pnpm check:i18n
```

Expected: 三门禁全绿。

- [ ] **Step 6.7: Commit**

```powershell
git add EasyProduct.Admin
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(admin): 布局/路由守卫/登录/工作台/模块占位/404 骨架闭环"
```

---

### Task 7: Site 工程配置

**Files:**
- Create: `EasyProduct.Site/package.json`、`vite.config.ts`、`tsconfig.json`、`index.html`、`env.d.ts`、`.env.development`、`.eslintrc.cjs`、`.prettierrc.json`、`scripts/check-chinese.cjs`、`src/main.ts`（最小）、`src/App.vue`（最小壳）

**Interfaces:**
- Consumes: Task 3（lint-staged 对 Site 路径的 eslint 路由）
- Produces: `pnpm dev` 端口 5174 可启动；lint/check:i18n/type-check 就绪。

- [ ] **Step 7.1: package.json（无 Element Plus/ECharts/Pinia 先不加，F1 询价篮需要 Pinia 时再加——此处预置 pinia 避免二次改锁）**

```json
{
  "name": "easyproduct-site",
  "version": "0.1.0",
  "private": true,
  "type": "module",
  "engines": { "node": ">=20" },
  "scripts": {
    "dev": "vite --port 5174",
    "build": "vue-tsc --noEmit && vite build",
    "preview": "vite preview",
    "type-check": "vue-tsc --noEmit",
    "lint": "eslint . --ext .vue,.ts --fix",
    "format": "prettier --write src",
    "check:i18n": "node scripts/check-chinese.cjs"
  },
  "dependencies": {
    "axios": "^1.6.8",
    "dayjs": "^1.11.10",
    "pinia": "^2.1.7",
    "vue": "^3.4.21",
    "vue-i18n": "^9.9.1",
    "vue-router": "^4.3.0"
  },
  "devDependencies": {
    "@typescript-eslint/eslint-plugin": "^7.4.0",
    "@typescript-eslint/parser": "^7.4.0",
    "@vitejs/plugin-vue": "^5.0.4",
    "eslint": "^8.57.0",
    "eslint-plugin-vue": "^9.24.0",
    "prettier": "^3.2.5",
    "sass": "^1.72.0",
    "typescript": "^5.3.3",
    "vite": "^5.4.0",
    "vue-tsc": "^2.0.7"
  }
}
```

- [ ] **Step 7.2: vite.config.ts / tsconfig.json / env.d.ts / index.html / .env.development**

`vite.config.ts`：与 Admin（Task 4 Step 4.2）相同结构，改两处：端口 `5174`；`additionalData` 改为 `@use "@/assets/styles/variables.scss" as *;@use "@/assets/styles/mixins.scss" as *;`。

`tsconfig.json`、`env.d.ts`：与 Admin（Step 4.3）逐字一致，直接复制。

`index.html`：与 Admin 相同，title 改为 `EasyProduct`。

`.env.development`：与 Admin（Step 4.4）逐字一致，直接复制。

- [ ] **Step 7.3: .eslintrc.cjs / .prettierrc.json / scripts/check-chinese.cjs**

三个文件与 `EasyProduct.Admin` 同名文件逐字一致：

```powershell
Copy-Item EasyProduct.Admin\.eslintrc.cjs EasyProduct.Site\.eslintrc.cjs
Copy-Item EasyProduct.Admin\.prettierrc.json EasyProduct.Site\.prettierrc.json
Copy-Item EasyProduct.Admin\scripts\check-chinese.cjs EasyProduct.Site\scripts\check-chinese.cjs
```

- [ ] **Step 7.4: 最小入口**

`src/main.ts`：

```typescript
import { createApp } from 'vue'
import App from './App.vue'

createApp(App).mount('#app')
```

`src/App.vue`：

```vue
<template>
  <div class="app-boot">EasyProduct Site</div>
</template>

<script setup lang="ts"></script>

<style scoped lang="scss">
.app-boot {
  padding: 24px;
}
</style>
```

- [ ] **Step 7.5: 安装并验证**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Site
pnpm install
pnpm dev        # http://localhost:5174 显示 "EasyProduct Site"
pnpm lint
pnpm check:i18n
```

- [ ] **Step 7.6: Commit**

```powershell
git add EasyProduct.Site
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(site): 工程配置（Vite5+TS strict+ESLint+check:i18n，端口 5174，proxy 指向 mock 7700）"
```

---

### Task 8: Site 核心层 + 布局骨架 + 占位首页

**Files:**
- Create: `EasyProduct.Site/src/types/api.ts`、`types/site.ts`
- Create: `EasyProduct.Site/src/utils/request.ts`、`utils/toast.ts`
- Create: `EasyProduct.Site/src/assets/styles/variables.scss`、`mixins.scss`、`reset.scss`、`global.scss`
- Create: `EasyProduct.Site/src/i18n/index.ts`、`i18n/zh-CN/common.json`、`zh-CN/site.json`、`en-US/common.json`、`en-US/site.json`
- Create: `EasyProduct.Site/src/router/index.ts`
- Create: `EasyProduct.Site/src/components/layout/AppLayout.vue`、`AppNavbar.vue`、`AppFooter.vue`
- Create: `EasyProduct.Site/src/views/home/index.vue`、`views/error/404.vue`
- Create: `EasyProduct.Site/src/api/site/home.ts`

**Interfaces:**
- Consumes: Task 2 的 site 分区契约（banner/list、news/list）与 i18n 托管路由
- Produces: `get/post<T>`（site 版 request，无登录态）；`showToast`；断点 mixins（mobile/tablet/desktop）；`AppLayout/AppNavbar/AppFooter`；路由骨架（F0：首页+404；F1 扩至 11 页）；`getBannerList/getNewsList`（api/site/home）。F1 全部页面消费这些导出。

- [ ] **Step 8.1: types 与 utils（toast 替代 ElMessage，见 Global Constraints 15）**

```typescript
// src/types/api.ts —— 与 Admin 版逐字一致（复制 EasyProduct.Admin/src/types/api.ts）
```

```typescript
// src/types/site.ts
/** 官网 Banner */
export interface Banner {
  id: string
  title: string
  titleEn: string
  imageUrl: string
  link: string
}

/** 官网新闻（内容中英字段由后端存储，不进语言包——规范 3.3） */
export interface NewsItem {
  id: string
  categoryId: string
  title: string
  titleEn: string
  summary: string
  coverImage: string
  isTop: boolean
  viewCount: number
  publishTime: string
}

/** 新闻查询 */
export interface NewsQuery {
  pageIndex: number
  pageSize: number
  keyword?: string
}
```

```typescript
// src/utils/toast.ts —— Site 轻量提示（不引入 Element Plus）
export function showToast(message: string): void {
  const el = document.createElement('div')
  el.className = 'app-toast'
  el.textContent = message
  document.body.appendChild(el)
  window.setTimeout(() => el.remove(), 2500)
}
```

```typescript
// src/utils/request.ts —— site 版：信封解包 + toast 提示，无登录态逻辑
import axios from 'axios'
import type { ApiResponse } from '@/types/api'
import { showToast } from '@/utils/toast'

const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '',
  timeout: 15000,
})

service.interceptors.request.use((config) => {
  config.headers['Accept-Language'] = localStorage.getItem('locale') || 'zh-CN'
  return config
})

service.interceptors.response.use(
  (response) => {
    const res = response.data as ApiResponse<unknown>
    if (res.code === 200) return res.data as never
    showToast(res.message || 'Error')
    return Promise.reject(new Error(res.message))
  },
  (error: unknown) => {
    showToast('Network Error')
    return Promise.reject(error instanceof Error ? error : new Error(String(error)))
  },
)

export function get<T>(url: string, params?: unknown): Promise<T> {
  return service.get(url, { params }) as Promise<T>
}
export function post<T>(url: string, data?: unknown): Promise<T> {
  return service.post(url, data) as Promise<T>
}
```

- [ ] **Step 8.2: 样式（断点混入 = 规范 1.5 原文）**

```scss
// src/assets/styles/variables.scss
$spacing-xs: 4px;
$spacing-sm: 8px;
$spacing-md: 16px;
$spacing-lg: 24px;
$spacing-xl: 48px;
$radius-sm: 4px;
$radius-md: 8px;
$site-max-width: 1200px;
$color-primary: #1e6fff;
$color-text: #22272e;
$color-text-secondary: #6b7280;
$color-bg: #ffffff;
$color-bg-soft: #f6f8fa;
$color-border: #e5e7eb;
```

```scss
// src/assets/styles/mixins.scss —— 响应式断点（Site 强制使用，规范 1.5）
$breakpoint-tablet: 768px;
$breakpoint-desktop: 1200px;

@mixin mobile {
  @media (max-width: #{$breakpoint-tablet - 1px}) {
    @content;
  }
}

@mixin tablet {
  @media (min-width: $breakpoint-tablet) and (max-width: #{$breakpoint-desktop - 1px}) {
    @content;
  }
}

@mixin desktop {
  @media (min-width: $breakpoint-desktop) {
    @content;
  }
}

@mixin site-container {
  max-width: $site-max-width;
  margin: 0 auto;
  padding: 0 $spacing-md;
}
```

```scss
// src/assets/styles/reset.scss
* {
  box-sizing: border-box;
}

html,
body {
  margin: 0;
  padding: 0;
  color: $color-text;
  background: $color-bg;
  font-family: 'Helvetica Neue', Arial, 'PingFang SC', 'Microsoft YaHei', sans-serif;
}

a {
  color: inherit;
  text-decoration: none;
}

img {
  display: block;
  max-width: 100%;
}
```

```scss
// src/assets/styles/global.scss（全局唯一入口，含 toast 样式）
.app-toast {
  position: fixed;
  top: 24px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  padding: $spacing-sm $spacing-md;
  border-radius: $radius-sm;
  background: rgba(0, 0, 0, 0.75);
  color: #fff;
  font-size: 14px;
}
```

- [ ] **Step 8.3: i18n（启动全量加载，规范 1.4 矩阵）**

`src/i18n/zh-CN/common.json`：

```json
{
  "app": { "name": "EasyProduct" },
  "nav": { "home": "首页" },
  "footer": { "rights": "版权所有" },
  "error": { "network": "网络错误" }
}
```

`src/i18n/en-US/common.json`：

```json
{
  "app": { "name": "EasyProduct" },
  "nav": { "home": "Home" },
  "footer": { "rights": "All rights reserved" },
  "error": { "network": "Network error" }
}
```

`src/i18n/zh-CN/site.json`：

```json
{
  "home": { "newsTitle": "新闻动态", "moreNews": "查看更多" },
  "notFound": { "back": "返回首页" }
}
```

`src/i18n/en-US/site.json`：

```json
{
  "home": { "newsTitle": "News", "moreNews": "More" },
  "notFound": { "back": "Back to Home" }
}
```

`src/i18n/index.ts`：与 Admin 版（Task 5 Step 5.4）相同结构，改两处：`messages` 组合为 `{ common, site }`；删除 `loadModuleLocale`（Site 启动全量加载，只保留远程覆盖刷新函数 `refreshRemoteOverrides()`，启动时顺序拉取 common/site 两份远程包深度合并）。

```typescript
import { createI18n } from 'vue-i18n'
import enCommon from './en-US/common.json'
import enSite from './en-US/site.json'
import zhCommon from './zh-CN/common.json'
import zhSite from './zh-CN/site.json'

export const SUPPORT_LOCALES = ['zh-CN', 'en-US'] as const
export type Locale = (typeof SUPPORT_LOCALES)[number]

const LOCALE_KEY = 'locale'
const MODULES = ['common', 'site'] as const

const messages: Record<Locale, Record<string, unknown>> = {
  'zh-CN': { common: zhCommon, site: zhSite },
  'en-US': { common: enCommon, site: enSite },
}

export const i18n = createI18n({
  legacy: false,
  locale: (localStorage.getItem(LOCALE_KEY) as Locale) || 'zh-CN',
  fallbackLocale: 'zh-CN',
  messages,
})

/** 启动时拉取远程语言包覆盖内置基线（免发布维护；失败静默用基线） */
export async function refreshRemoteOverrides(): Promise<void> {
  const locale = i18n.global.locale.value as Locale
  for (const module of MODULES) {
    try {
      const res = await fetch(`/api/i18n/${locale}/${module}.json`)
      if (res.ok) {
        const data = (await res.json()) as Record<string, unknown>
        i18n.global.mergeLocaleMessage(locale, { [module]: data })
      }
    } catch {
      // 离线/失败：用包内基线
    }
  }
}

export function setLocale(locale: Locale): void {
  i18n.global.locale.value = locale
  localStorage.setItem(LOCALE_KEY, locale)
  document.documentElement.lang = locale
}
```

- [ ] **Step 8.4: api/site/home.ts**

```typescript
// src/api/site/home.ts
import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Banner, NewsItem, NewsQuery } from '@/types/site'

/** 首页 Banner 列表 */
export const getBannerList = () => get<Banner[]>('/api/site/banner/list')

/** 新闻分页列表 */
export const getNewsList = (params: NewsQuery) =>
  get<PageResult<NewsItem>>('/api/site/news/list', params)
```

- [ ] **Step 8.5: 布局组件（AppLayout/AppNavbar/AppFooter）**

```vue
<!-- src/components/layout/AppLayout.vue -->
<template>
  <div class="app-layout">
    <AppNavbar />
    <main class="app-layout__main">
      <router-view />
    </main>
    <AppFooter />
  </div>
</template>

<script setup lang="ts">
import AppFooter from './AppFooter.vue'
import AppNavbar from './AppNavbar.vue'
</script>

<style scoped lang="scss">
.app-layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;

  &__main {
    flex: 1;
  }
}
</style>
```

```vue
<!-- src/components/layout/AppNavbar.vue -->
<template>
  <header class="app-navbar">
    <div class="app-navbar__inner">
      <router-link to="/" class="app-navbar__brand">{{ t('common.app.name') }}</router-link>
      <nav class="app-navbar__links">
        <router-link v-for="item in NAV_ITEMS" :key="item.path" :to="item.path">
          {{ t(item.titleKey) }}
        </router-link>
      </nav>
      <button class="app-navbar__locale" @click="handleToggleLocale">
        {{ locale === 'zh-CN' ? 'EN' : '中文' }}
      </button>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { setLocale } from '@/i18n'

const { t, locale } = useI18n()

/** F0 仅首页；F1 逐个追加（products/news/videos/downloads/about/contact/inquiry） */
const NAV_ITEMS = [{ path: '/', titleKey: 'common.nav.home' }] as const

const handleToggleLocale = () => {
  setLocale(locale.value === 'zh-CN' ? 'en-US' : 'zh-CN')
}
</script>

<style scoped lang="scss">
.app-navbar {
  border-bottom: 1px solid $color-border;
  background: $color-bg;

  &__inner {
    @include site-container;
    display: flex;
    align-items: center;
    gap: $spacing-lg;
    height: 64px;
  }

  &__brand {
    font-weight: 700;
    font-size: 18px;
  }

  &__links {
    display: flex;
    gap: $spacing-md;

    a.router-link-active {
      color: $color-primary;
    }

    @include mobile {
      display: none; // F1 补移动端抽屉导航
    }
  }

  &__locale {
    margin-left: auto;
    border: 1px solid $color-border;
    border-radius: $radius-sm;
    background: transparent;
    padding: $spacing-xs $spacing-sm;
    cursor: pointer;
  }
}
</style>
```

```vue
<!-- src/components/layout/AppFooter.vue -->
<template>
  <footer class="app-footer">
    <div class="app-footer__inner">
      © {{ year }} {{ t('common.app.name') }} · {{ t('common.footer.rights') }}
    </div>
  </footer>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'

const { t } = useI18n()
const year = dayjs().year()
</script>

<style scoped lang="scss">
.app-footer {
  background: $color-bg-soft;
  border-top: 1px solid $color-border;
  color: $color-text-secondary;

  &__inner {
    @include site-container;
    padding-top: $spacing-lg;
    padding-bottom: $spacing-lg;
    text-align: center;
  }
}
</style>
```

- [ ] **Step 8.6: 路由 + 首页占位 + 404**

```typescript
// src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { i18n } from '@/i18n'

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/components/layout/AppLayout.vue'),
    children: [
      { path: '', name: 'home', component: () => import('@/views/home/index.vue'), meta: { title: 'common.nav.home' } },
      // F1 依次追加：/products /products/:id /news /news/:id /videos /downloads /about /contact /inquiry
    ],
  },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/error/404.vue'), meta: { title: 'site.notFound.back' } },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

/** SEO 基础：document.title = 站点名 + 页面名（规范 3.3） */
router.afterEach((to) => {
  const key = to.meta.title as string | undefined
  const page = key ? i18n.global.t(key) : ''
  document.title = page ? `${page} - EasyProduct` : 'EasyProduct'
})
```

```vue
<!-- src/views/home/index.vue -->
<template>
  <div class="home-page">
    <section class="home-page__hero">
      <img v-if="banner" :src="banner.imageUrl" :alt="bannerTitle" class="home-page__banner" />
    </section>
    <section class="home-page__news">
      <div class="home-page__news-head">
        <h2>{{ t('site.home.newsTitle') }}</h2>
        <router-link to="/news">{{ t('site.home.moreNews') }}</router-link>
      </div>
      <ul v-if="news.length" class="home-page__news-list">
        <li v-for="item in news" :key="item.id">
          <router-link :to="`/news/${item.id}`">{{ locale === 'zh-CN' ? item.title : item.titleEn }}</router-link>
        </li>
      </ul>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { getBannerList, getNewsList } from '@/api/site/home'
import type { Banner, NewsItem } from '@/types/site'

const { t, locale } = useI18n()
const banners = ref<Banner[]>([])
const news = ref<NewsItem[]>([])

const banner = computed(() => banners.value[0])
const bannerTitle = computed(() =>
  banner.value ? (locale.value === 'zh-CN' ? banner.value.title : banner.value.titleEn) : '',
)

onMounted(async () => {
  const [bannerList, newsPage] = await Promise.all([
    getBannerList(),
    getNewsList({ pageIndex: 1, pageSize: 5 }),
  ])
  banners.value = bannerList
  news.value = newsPage.list
})
</script>

<style scoped lang="scss">
.home-page {
  @include site-container;
  padding-top: $spacing-lg;
  padding-bottom: $spacing-xl;

  &__banner {
    width: 100%;
    border-radius: $radius-md;
  }

  &__news {
    margin-top: $spacing-xl;
  }

  &__news-head {
    display: flex;
    align-items: baseline;
    justify-content: space-between;
  }

  &__news-list {
    list-style: none;
    padding: 0;

    li {
      padding: $spacing-sm 0;
      border-bottom: 1px solid $color-border;
    }
  }
}
</style>
```

```vue
<!-- src/views/error/404.vue -->
<template>
  <div class="not-found">
    <h1>404</h1>
    <router-link to="/">{{ t('site.notFound.back') }}</router-link>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
</script>

<style scoped lang="scss">
.not-found {
  padding: $spacing-xl * 2;
  text-align: center;
}
</style>
```

- [ ] **Step 8.7: main.ts 完整引导**

```typescript
// src/main.ts
import { createPinia } from 'pinia'
import { createApp } from 'vue'
import App from './App.vue'
import { i18n, refreshRemoteOverrides } from './i18n'
import { router } from './router'
import './assets/styles/reset.scss'
import './assets/styles/global.scss'

const app = createApp(App)
app.use(createPinia())
app.use(i18n)
app.use(router)
app.mount('#app')

refreshRemoteOverrides()
```

App.vue 改为 `<template><router-view /></template>`（同 Admin Step 6.1）。

- [ ] **Step 8.8: 验证 Site 骨架**

Run（mock-server 保持运行）：

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.Site
pnpm dev
```

手工验证：http://localhost:5174 → 首页显示 mock Banner 图 + 5 条新闻（mock site 分区）→ 中英文切换生效 → /xyz 落 404。

```powershell
pnpm type-check
pnpm lint
pnpm check:i18n
```

Expected: 三门禁全绿。

- [ ] **Step 8.9: Commit**

```powershell
git add EasyProduct.Site
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(site): 核心层+布局骨架+占位首页（request/toast/断点/双语/SEO title，接入 mock 首页聚合）"
```

---
### Task 9: MiniApp 基座与六层骨架（tabBar 占位 4 页）

**Files:**
- Create: `EasyProduct.MiniApp/project.config.json`、`app.json`、`app.ts`、`sitemap.json`、`tsconfig.json`、`package.json`
- Create: `EasyProduct.MiniApp/config/env.ts`
- Create: `EasyProduct.MiniApp/utils/storage.ts`、`utils/wx-login.ts`、`utils/request.ts`、`utils/i18n.ts`
- Create: `EasyProduct.MiniApp/stores/base.store.ts`
- Create: `EasyProduct.MiniApp/types/api.types.ts`
- Create: `EasyProduct.MiniApp/i18n/zh-CN/common.json`、`i18n/en-US/common.json`
- Create: `EasyProduct.MiniApp/pages/index/*`、`pages/category/*`、`pages/cart/*`、`pages/profile/*`（各 4 文件：.ts/.wxml/.scss/.json）

**Interfaces:**
- Consumes: Task 2 的 `POST /api/app/auth/wx-login` 契约
- Produces: `ENV`（config/env）；`request<T>`（member_token 注入 + 401 静默重登重放一次）；`silentLogin()`；`t(key)/initI18n()`；`BaseStore<T>`（观察者）；`ApiResponse/PageQuery/PageResult`（无前缀类型）。F3 的 services/stores/adapters 全部消费这些导出。

- [ ] **Step 9.1: 工程配置（project.config.json/app.json/sitemap/tsconfig/package.json）**

`project.config.json`：

```json
{
  "appid": "touristappid",
  "compileType": "miniprogram",
  "libVersion": "3.4.0",
  "setting": {
    "es6": true,
    "enhance": true,
    "useCompilerPlugins": ["typescript", "sass"],
    "urlCheck": false
  },
  "condition": {}
}
```

`app.json`（F0 仅注册 tabBar 4 页；F3 追加 details/order/payment/service）：

```json
{
  "pages": [
    "pages/index/index",
    "pages/category/category",
    "pages/cart/cart",
    "pages/profile/profile"
  ],
  "window": {
    "navigationBarTitleText": "EasyProduct",
    "navigationBarBackgroundColor": "#ffffff",
    "navigationBarTextStyle": "black",
    "backgroundColor": "#f6f8fa"
  },
  "tabBar": {
    "color": "#999999",
    "selectedColor": "#1e6fff",
    "list": [
      { "pagePath": "pages/index/index", "text": "首页" },
      { "pagePath": "pages/category/category", "text": "分类" },
      { "pagePath": "pages/cart/cart", "text": "购物车" },
      { "pagePath": "pages/profile/profile", "text": "我的" }
    ]
  },
  "style": "v2",
  "sitemapLocation": "sitemap.json"
}
```

> tabBar 文案为配置项非代码文案，i18n 无法覆盖 app.json；F3 用自定义 tabBar 组件替换以支持双语（列入 F3-6 任务）。

`sitemap.json`：

```json
{ "desc": "EasyProduct MiniApp", "rules": [{ "action": "allow", "page": "*" }] }
```

`tsconfig.json`：

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "ESNext",
    "moduleResolution": "node",
    "lib": ["ES2022"],
    "strict": true,
    "noEmit": true,
    "skipLibCheck": true,
    "resolveJsonModule": true,
    "types": ["miniprogram-api-typings"]
  },
  "include": ["**/*.ts"],
  "exclude": ["node_modules"]
}
```

`package.json`：

```json
{
  "name": "easyproduct-miniapp",
  "version": "0.1.0",
  "private": true,
  "scripts": {
    "type-check": "tsc --noEmit"
  },
  "devDependencies": {
    "miniprogram-api-typings": "^3.12.2",
    "typescript": "^5.3.3"
  }
}
```

- [ ] **Step 9.2: config/env.ts 与 types/api.types.ts**

```typescript
// config/env.ts —— 替代原 USE_MOCK 硬编码（规范 4.5）
export const ENV = {
  /** dev 可临时开；build 必须 false。F0~F3 阶段走 mock-server */
  useMock: true,
  /** 只调 /api/app/**；mock 阶段指向 7700，联调改真后端域名 */
  apiBase: 'http://localhost:7700/api/app',
}
```

```typescript
// types/api.types.ts —— 新增类型一律无 I 前缀（规范 1.3 + 4.3 注）
/** 统一响应信封 */
export interface ApiResponse<T> {
  code: number
  message: string
  data: T
  timestamp: number
}

/** 分页查询参数 */
export interface PageQuery {
  pageIndex: number
  pageSize: number
}

/** 分页响应 */
export interface PageResult<T> {
  list: T[]
  total: number
}
```

- [ ] **Step 9.3: utils/storage.ts 与 utils/wx-login.ts**

```typescript
// utils/storage.ts
const MEMBER_TOKEN = 'member_token'
const LOCALE_CACHE = 'i18n_cache'

export const getMemberToken = (): string => wx.getStorageSync(MEMBER_TOKEN) || ''
export const setMemberToken = (token: string): void => wx.setStorageSync(MEMBER_TOKEN, token)
export const clearMemberToken = (): void => wx.removeStorageSync(MEMBER_TOKEN)

export const getLocaleCache = <T>(key: string): T | null => {
  const all = wx.getStorageSync(LOCALE_CACHE) as Record<string, T> | ''
  return all && all[key] ? all[key] : null
}
export const setLocaleCache = (key: string, value: unknown): void => {
  const all = (wx.getStorageSync(LOCALE_CACHE) as Record<string, unknown> | '') || {}
  const next = typeof all === 'string' ? {} : all
  next[key] = value
  wx.setStorageSync(LOCALE_CACHE, next)
}
```

```typescript
// utils/wx-login.ts —— code 换会员 Token（mock 阶段不校验真实 code）
import { ENV } from '../config/env'
import { setMemberToken } from './storage'

export async function silentLogin(): Promise<string> {
  const { code } = await wx.login()
  const res = await new Promise<WechatMiniprogram.RequestSuccessCallbackResult>((resolve, reject) => {
    wx.request({
      url: `${ENV.apiBase}/auth/wx-login`,
      method: 'POST',
      data: { code },
      success: resolve,
      fail: reject,
    })
  })
  const envelope = res.data as { code: number; data: { memberToken: string } }
  if (envelope.code !== 200) throw new Error('wx-login failed')
  setMemberToken(envelope.data.memberToken)
  return envelope.data.memberToken
}
```

- [ ] **Step 9.4: utils/request.ts（信封解包 + 401 静默重登重放一次）**

```typescript
// utils/request.ts
import { ENV } from '../config/env'
import { getMemberToken } from './storage'
import { silentLogin } from './wx-login'
import type { ApiResponse } from '../types/api.types'

interface RequestOptions {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  data?: unknown
  /** 是否需要会员 Token（默认 true；wx-login 自身传 false） */
  needAuth?: boolean
}

async function rawRequest<T>(options: RequestOptions, token: string): Promise<T> {
  const res = await new Promise<WechatMiniprogram.RequestSuccessCallbackResult>((resolve, reject) => {
    wx.request({
      url: `${ENV.apiBase}${options.url}`,
      method: options.method ?? 'GET',
      data: options.data,
      header: token ? { Authorization: `Bearer ${token}` } : {},
      success: resolve,
      fail: () => reject(new Error('network error')),
    })
  })
  const envelope = res.data as ApiResponse<T>
  if (envelope.code === 200) return envelope.data
  if (envelope.code === 401) throw Object.assign(new Error(envelope.message), { code: 401 })
  wx.showToast({ title: envelope.message, icon: 'none' })
  throw new Error(envelope.message)
}

export async function request<T>(options: RequestOptions): Promise<T> {
  const needAuth = options.needAuth !== false
  let token = needAuth ? getMemberToken() : ''
  try {
    return await rawRequest<T>(options, token)
  } catch (err) {
    if (needAuth && err instanceof Error && 'code' in err && err.code === 401) {
      token = await silentLogin() // 静默重登后重放一次
      return rawRequest<T>(options, token)
    }
    throw err
  }
}
```

- [ ] **Step 9.5: utils/i18n.ts（轻量 t()：内置基线 + 远程覆盖 + storage 缓存，规范 4.6）**

```typescript
// utils/i18n.ts
import zhCommon from '../i18n/zh-CN/common.json'
import enCommon from '../i18n/en-US/common.json'
import { getLocaleCache, setLocaleCache } from './storage'

export type MiniLocale = 'zh-CN' | 'en-US'

const BASELINE: Record<MiniLocale, Record<string, unknown>> = {
  'zh-CN': { common: zhCommon },
  'en-US': { common: enCommon },
}

let currentLocale: MiniLocale = 'zh-CN'
let merged: Record<string, unknown> = {}

function deepMerge(target: Record<string, unknown>, source: Record<string, unknown>): Record<string, unknown> {
  for (const key of Object.keys(source)) {
    const sv = source[key]
    const tv = target[key]
    if (sv && typeof sv === 'object' && !Array.isArray(sv) && tv && typeof tv === 'object') {
      target[key] = deepMerge({ ...(tv as Record<string, unknown>) }, sv as Record<string, unknown>)
    } else {
      target[key] = sv
    }
  }
  return target
}

function lookup(obj: Record<string, unknown>, keyPath: string): string {
  const parts = keyPath.split('.')
  let cur: unknown = obj
  for (const p of parts) {
    if (!cur || typeof cur !== 'object') return keyPath
    cur = (cur as Record<string, unknown>)[p]
  }
  return typeof cur === 'string' ? cur : keyPath
}

export function getLocale(): MiniLocale {
  return currentLocale
}

/** 取文案：key 形如 common.button.confirm */
export function t(key: string): string {
  return lookup(merged, key)
}

/** 启动时调用：系统语言 → 缓存远程包 → 内置基线合并远程覆盖 */
export async function initI18n(): Promise<void> {
  const sys = wx.getLocale()
  currentLocale = sys.startsWith('en') ? 'en-US' : 'zh-CN'
  merged = deepMerge({}, BASELINE[currentLocale])

  const cacheKey = `remote:${currentLocale}`
  const cached = getLocaleCache<Record<string, unknown>>(cacheKey)
  if (cached) merged = deepMerge(merged, cached)

  try {
    const remote = await new Promise<Record<string, unknown>>((resolve, reject) => {
      wx.request({
        // 语言包托管在 /api/i18n（非 /api/app 分区），mock/生产同源
        url: `http://localhost:7700/api/i18n/${currentLocale}/common.json`,
        success: (res) => resolve(res.data as Record<string, unknown>),
        fail: reject,
      })
    })
    merged = deepMerge(merged, { common: remote })
    setLocaleCache(cacheKey, { common: remote })
  } catch {
    // 离线：内置基线 + 上次缓存
  }
}
```

`i18n/zh-CN/common.json`：

```json
{
  "app": { "name": "EasyProduct" },
  "button": { "confirm": "确定", "cancel": "取消" },
  "loading": "加载中…",
  "loadFailed": "加载失败",
  "tab": { "home": "首页", "category": "分类", "cart": "购物车", "profile": "我的" }
}
```

`i18n/en-US/common.json`：

```json
{
  "app": { "name": "EasyProduct" },
  "button": { "confirm": "OK", "cancel": "Cancel" },
  "loading": "Loading…",
  "loadFailed": "Load failed",
  "tab": { "home": "Home", "category": "Category", "cart": "Cart", "profile": "Profile" }
}
```

- [ ] **Step 9.6: stores/base.store.ts（观察者基类，规范 4.4）**

```typescript
// stores/base.store.ts
type Listener<T> = (state: T) => void

/** 全局状态基类：getState/setState/subscribe（F3 cart/member store 继承它） */
export class BaseStore<T> {
  private state: T
  private listeners: Array<Listener<T>> = []

  constructor(initial: T) {
    this.state = initial
  }

  getState(): T {
    return this.state
  }

  setState(partial: Partial<T>): void {
    this.state = { ...this.state, ...partial }
    this.listeners.forEach((fn) => fn(this.state))
  }

  subscribe(fn: Listener<T>): () => void {
    this.listeners.push(fn)
    return () => {
      this.listeners = this.listeners.filter((l) => l !== fn)
    }
  }
}
```

- [ ] **Step 9.7: app.ts + tabBar 4 页占位**

```typescript
// app.ts
import { initI18n } from './utils/i18n'

App({
  onLaunch() {
    initI18n()
  },
})
```

4 个占位页结构相同（以 index 为例，其余三页替换文案 key 与类名）：

`pages/index/index.ts`：

```typescript
import { t } from '../../utils/i18n'

Page({
  data: { title: '' },
  onShow() {
    this.setData({ title: t('common.tab.home') })
  },
})
```

`pages/index/index.wxml`：

```xml
<view class="page">
  <view class="page__title">{{ title }}</view>
</view>
```

`pages/index/index.scss`：

```scss
.page {
  padding: 48rpx 32rpx;

  &__title {
    font-size: 36rpx;
    font-weight: 600;
  }
}
```

`pages/index/index.json`：

```json
{ "usingComponents": {} }
```

（category/cart/profile 三页同构：title key 分别为 `common.tab.category` / `common.tab.cart` / `common.tab.profile`。）

- [ ] **Step 9.8: 验证 MiniApp 骨架**

Run:

```powershell
cd D:\4-MyProject\EasyProduct\EasyProduct.MiniApp
pnpm install
pnpm type-check
```

再用微信开发者工具导入 `EasyProduct.MiniApp` 目录（勾选"不校验合法域名"）：编译通过、tabBar 4 页可切换、每页显示对应多语言标题。

Expected: tsc 零错误；开发者工具无编译报错。

- [ ] **Step 9.9: Commit**

```powershell
git add EasyProduct.MiniApp
git -c user.name='lilin' -c user.email='lilin@local' commit -m "feat(miniapp): 基座与六层骨架（env/request 静默重登/轻量 i18n/BaseStore/tabBar 占位 4 页）"
```

---

### Task 10: F0 整体验收（根 README + 四端启动演练 + DoD 全绿）

**Files:**
- Create: `README.md`（仓库根）
- Verify: mock-server、EasyProduct.Admin、EasyProduct.Site、EasyProduct.MiniApp

**Interfaces:**
- Consumes: Task 1~9 全部产物
- Produces: F0 完成的判定依据；根 README 作为后续所有开发者的启动手册。

- [ ] **Step 10.1: 根 README.md**

```markdown
# EasyProduct

EasyWebSite + EasyProject + EasyCRM 三项目整合后的模块化单体（官网 + 管理后台 + 小程序商城）。

## 目录

| 目录 | 说明 |
|------|------|
| EasyProduct.WebApi/ | 后端（.NET 8，B 系列计划另立，尚未创建） |
| EasyProduct.Admin/ | PC 管理后台（Vue3 + Element Plus），端口 5173 |
| EasyProduct.Site/ | 官网门户（Vue3 响应式），端口 5174 |
| EasyProduct.MiniApp/ | 微信小程序（原生 + TS），微信开发者工具打开 |
| mock-server/ | 独立 Mock 服务（契约先行），端口 7700 |
| docs/ | 设计方案与开发规范（先读 AGENTS.md） |

## 启动（前端/mock 阶段）

1. `cd mock-server && pnpm install && pnpm dev`（7700）
2. `cd EasyProduct.Admin && pnpm install && pnpm dev`（5173，登录 admin/admin123）
3. `cd EasyProduct.Site && pnpm install && pnpm dev`（5174）
4. 微信开发者工具导入 `EasyProduct.MiniApp`（勾选"不校验合法域名"）

后端联调时：Admin/Site 改 `.env.development` 的 `VITE_PROXY_TARGET=http://localhost:7600`；MiniApp 改 `config/env.ts`。

## 文档

- 整合设计方案：docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md
- 前端规范：docs/frontend-guidelines.md
- 后端规范：docs/backend-guidelines.md
- Mock 规范：docs/mock-guidelines.md
- 前端先行开发计划：docs/superpowers/plans/2026-08-08-frontend-first-development.md
```

- [ ] **Step 10.2: F0 验收走查（逐项打勾）**

1. mock-server：`/__mock/status` 200；`/__mock/reset` 200；无 Token 调 `/api/admin/basic/menu/list` 返回信封 401。
2. Admin：未登录访问任意页 → /login；admin/admin123 登录 → 工作台；侧栏 9 项菜单；中/英切换 + 亮/暗切换；/xyz → 404。
3. Site：首页渲染 mock Banner + 5 条新闻；中英切换；/xyz → 404。
4. MiniApp：开发者工具编译通过；tabBar 4 页切换；文案随系统语言。
5. 门禁：Admin/Site 各自 `pnpm type-check`、`pnpm lint`、`pnpm check:i18n` 全绿；MiniApp `pnpm type-check` 零错误。
6. 提交一条错误格式的 commit message（如 `update xxx`）验证 commitlint 拦截，随后放弃该提交：

```powershell
git commit --allow-empty -m "update something"   # 期望被 commit-msg 钩子拒绝
```

- [ ] **Step 10.3: Commit**

```powershell
git add README.md
git -c user.name='lilin' -c user.email='lilin@local' commit -m "docs: F0 整体验收（根 README 启动手册 + 四端走查清单）"
```

**F0 阶段完成判定**：Task 1~10 全部 checkbox 完成，验收走查 6 项通过。此后进入 F1（先按 F1 蓝图用 writing-plans 细化任务级步骤）。

---
---

## F1 阶段蓝图：EasyProduct.Site（11 门户页）

> 进入 F1 前，以本节为输入用 writing-plans 产出任务级计划。范围与验收已在评审中确认，细化时不得扩缩范围。

**范围（整合设计 7.2）**：首页 / 产品列表 / 产品详情 / 新闻列表 / 新闻详情 / 视频 / 下载 / 关于 / 联系 / 询价 / 404，共 11 页；API 全走 `/api/site/**`；产品与新闻内容的中英字段由后端存储（name/nameEn），不进语言包。

**文件结构增量（在 F0 Site 骨架之上）**：

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

**任务表（细化基线）**：

| 任务 | 内容 | 关键产出 | 验收 |
|------|------|---------|------|
| F1-0 | mock-server site 分区全量 | 产品/分类/新闻/视频/下载/关于/联系/询价路由 + mockjs 数据；询价 POST 落 store、GET 可查 | 契约清单对照后端规范 §5 逐字通过；`/__mock/reset` 覆盖 inquiry store |
| F1-1 | 通用组件库 | AppCarousel/AppPagination/AppEmpty/AppLoading/AppSkeleton（scoped + 变量化，禁硬编码色值） | 各组件在首页/列表页被真实使用 |
| F1-2 | 产品域 | 产品列表（ProductFilter 分类筛选 + ProductGrid + 分页）+ 详情（图集/参数/加入询价篮） | 断点三态（mobile/tablet/desktop）网格 1/2/4 列 |
| F1-3 | 新闻域 | 列表（分页 + 置顶标识）+ 详情（正文 + 面包屑 + 上一篇/下一篇） | 中英内容字段随语言切换 |
| F1-4 | 视频/下载 | 视频列表（封面 + 弹窗播放）、下载列表（文件名/大小/下载次数 + 下载按钮） | 移动端可操作 |
| F1-5 | 关于/联系 | 关于单页（后端 about 单页数据）、联系页（表单提交 → `/api/site/contact`，防重复提交） | 联系表单限流错误文案正确 |
| F1-6 | 询价闭环 | useInquiry + stores/inquiry 询价篮（增删改数量）+ 询价页提交 → `POST /api/site/inquiry`（含明细行） | 提交成功态 + 防重复；mock store 中可查到 site_inquiry 记录 |
| F1-7 | 收尾 | 导航补全 8 项 + 移动端抽屉导航；SEO（每页 title、图片 alt）；`check:i18n` 全量 | 11 页走查清单逐项打勾；三门禁全绿 |

**F1 验收**：《Site 页面走查清单》（11 页 × PC/移动 × 中英）全部打勾；询价提交在 mock 端可验证；`pnpm build` 成功。

---

## F2 阶段蓝图：EasyProduct.Admin（八大模块）

> 进入 F2 前用 writing-plans 细化。模块顺序已评审锁定：**Basic → Site → Product → Mall → Crm → Workflow → Report → Ops**（对齐后端 P1~P5 交付序，mock 逐模块退役）。

**通用开工项（F2-0，先于所有模块）**：

| 项 | 内容 |
|----|------|
| Composables | `useTable`（loading/list/total/query/handleSearch/handleReset/handlePageChange/reload）、`useForm`（formRef/model/rules/validate/resetFields/submitLoading/handleSubmit）、`useDialog`（visible/payload/isEdit/open/close）、`useDict`（按 typeCode 取 options、value→label，走 i18n labelKey）、`useLocale`、`usePermission`（has + `v-permission` 指令）——签名逐字对齐前端规范 2.4 |
| 通用组件 | `BaseTable`（列配置 + 分页插槽）、`ImageUpload`（上传 → `/api/admin/file/upload` mock）、富文本占位组件（WangEditor 在首个富文本需求出现时引入并说明依赖理由） |
| 动态菜单 | Basic 模块落地后，路由由静态 placeholder 切换为菜单驱动：登录后拉菜单树 → 动态注册子路由（F2-1 内完成，替换 router/modules/placeholder.ts） |
| mock 补齐 | 每个模块开发前，先在 mock-server 补该模块 admin 分区路由 + data + store（含 `/__mock/reset` 注册），契约对照后端规范 §5 |
| 页面模板 | 所有列表页遵循前端规范 2.5 六段式 import 模板；弹窗 vs 路由跳转按 2.3 决策表 |

**任务表（细化基线）**：

| 任务 | 模块 | 页面 | 要点 |
|------|------|------|------|
| F2-0 | 封装层 | — | 上表全部；useTable 列表页 5 行起步成立 |
| F2-1 | Basic | 用户/部门/角色/菜单/字典/公告/工作台布局/系统参数/个人中心（9 页） | RBAC 全套；菜单管理落权限标识 `模块:页面:操作` 种子；动态路由切换；个人中心改密 |
| F2-2 | Site 管理 | 新闻/分类/Banner/视频/下载/关于/询价处理/留言（8 页） | 询价列表 + 一键转客户（调 crm 预创建接口，F2-5 前用 mock 占位） |
| F2-3 | Product | 商品分类/商品管理(SPU+SKU)/渠道发布（3 页） | SPU+SKU 编辑（规格组合生成 SKU）；渠道发布开关：官网/小程序/B2B |
| F2-4 | Mall | 会员/等级/积分/优惠券/商城订单/支付记录（6 页） | 商城订单状态机：待付款→已付款→发货→完成（可取消/退款入口→F2-5 冲销） |
| F2-5 | Crm | EasyCRM 10 模块 56 页重写入 `views/crm/**` | 拆 6 子批：①主数据（客户/供应商/币种/税率）②销售订单 ③采购订单 ④库存（仓库/库存/出入库/盘点/预警）⑤发票/收付款/应收应付/固定资产 ⑥冲销；ECharts 5.5→6 对齐；去 mockjs 依赖 |
| F2-6 | Workflow | 流程设计/发布/我的申请/待我审批/已办/实例查询（6 页）+ 请假示范业务 | 设计器画布为最重子任务，细化时单独成任务 |
| F2-7 | Report | 数据源/报表定义/列模板（3 页） | 报表预览用 ECharts 按需引入（vue-echarts + echarts/core） |
| F2-8 | Ops | 操作日志/登录日志/定时任务/任务日志/日志查询（5 页） | 日志查询走只读列表 |
| F2-9 | 工作台整合 | EasyCRM dashboard 4 页 → desktop Widget | 经营概览/KPI/待办/预警四 Widget 复用 PCWeb Widget 容器概念重写 |

**Crm 子批页面映射（EasyCRM → views/crm/**，细化时逐页列清单）**：master（customer/supplier/currency/tax-rate）、sales（order/detail）、purchase（order/detail）、inventory（warehouse/stock/in-out/check/alert）、finance（invoice/payment/arap/fixed-asset）、reversal（list/detail，含商城退款类型）。

**F2 验收**：每模块完成即演示（登录→列表→详情→增删改→权限按钮显隐→中英切换）；模块级《页面走查清单》打勾；三门禁全绿；mock 状态表同步更新。

---
## F3 阶段蓝图：EasyProduct.MiniApp（全页面重写）

> 进入 F3 前用 writing-plans 细化。范围（整合设计 7.3）：首页/分类/详情/购物车/下单/支付/我的/客服聊天，共 8 页；数据源 `product_channel` 渠道=小程序；微信登录 code→会员 JWT；支付只 mock 报文（不接真实微信支付）。

**文件结构增量（在 F0 六层骨架之上）**：

```text
EasyProduct.MiniApp/
├── types/product.types.ts  cart.types.ts  order.types.ts  member.types.ts
├── adapters/api.adapter.ts                 # 唯一数据源：utils/request 调 /api/app/**
├── services/auth.service.ts  product.service.ts  cart.service.ts  order.service.ts  member.service.ts
├── stores/cart.store.ts  member.store.ts
├── components/product-card/  price/  quantity-stepper/  empty/  custom-tabbar/
├── pages/index/*  category/*  details/*  cart/*  order/*  payment/*  profile/*  service/*
└── i18n/*/common.json（追加业务键组）
mock-server/src/
├── data/product.ts  mall.ts
├── store/cart.ts  order.ts
└── routes/app/product.ts  cart.ts  order.ts  member.ts  payment.ts
```

**任务表（细化基线）**：

| 任务 | 内容 | 要点 |
|------|------|------|
| F3-0 | mock-server app 分区全量 | 商品列表/详情（渠道=小程序过滤）、分类、购物车 CRUD、下单、支付（仅成功/失败两种报文，mock-guidelines 11.4）、会员信息/订单列表 |
| F3-1 | 三层落齐 | types 四文件（无前缀）；services 五单例；cart.store/member.store 继承 BaseStore；adapters/api.adapter 唯一出口 |
| F3-2 | 浏览链路 | 首页（Banner+分类+热销）、分类页（侧栏分类+商品列表）、详情页（图集/SKU 选择/加购/立即购买）；setData 路径更新纪律（规范 4.3） |
| F3-3 | 交易链路 | 购物车（选中/数量/删除/结算）、下单页（地址/备注/优惠占位）、订单列表与状态机 |
| F3-4 | 支付与登录收尾 | 支付页（mock 支付 → 成功/失败报文 → 结果页）；401 静默重登全链路回归；token 过期边界用例 |
| F3-5 | 我的/客服 | 个人中心（会员信息/积分/订单入口/语言切换）、客服聊天页（本地消息列表 + mock 自动回复） |
| F3-6 | i18n 与收尾 | 自定义 tabBar（双语）；全部文案 check（小程序版 check:i18n 脚本，扫描 pages/**/*.ts 与 .wxml）；分包预案评估（主包 ≤10 页暂不分包） |

**F3 验收**：微信开发者工具下单闭环走查（浏览→加购→结算→下单→支付 mock 成功→订单可见→我的页展示）；中英双语切换；`pnpm type-check` 零错误；包体 < 1.5MB。

---

## 验收与 mock 退役机制（贯穿 F0~F3）

**每提交 Definition of Done**（前端规范 §5 原文）：

1. `vue-tsc --noEmit` 零错误（Admin/Site）；小程序 `tsc` 零错误
2. ESLint 通过（含 no-explicit-any、no-console）
3. `npm run check:i18n` 无硬编码中文（三端）
4. 新增文案 zh-CN/en-US 同步
5. 样式 scoped、无硬编码色值
6. 新页面遵守弹窗/路由决策、禁用 el-drawer（Admin）
7. commit message 符合 Conventional Commits（commitlint 强制）
8. 不主动提交无关改动

**mock 退役流程**（mock-guidelines §8，后端每交付一个模块执行一次）：

1. **对照**：后端模块完成 → 导出 swagger.json → 按 mock-guidelines §9 清单逐接口、逐字段对照（URL/方法/信封/分页/camelCase/GUID/ISO 时间/状态值三方一致/错误形态），不一致以后端为准修 mock，后端违规修后端。
2. **切换**：vite proxy 按路径分流——已交付模块 → 7600，未交付 → 7700（Admin/Site）；MiniApp 在 `config/env.ts` 切 baseUrl。
3. **标记**：mock 路由文件头状态改 `deprecated`，mock-server/README 状态表登记（日期+经手人）。
4. **清理**：B 系列 P6 冻结时删除全部 deprecated 路由，mock-server 归档。

**阶段演示物对照**：

| 阶段 | 演示物 | 判定 |
|------|--------|------|
| F0 | 三端 + mock 四端启动 | Task 10 走查 6 项通过 |
| F1 | 官网 11 页 + 询价闭环 | 走查清单全勾 + build 成功 |
| F2 | 八大模块逐个演示 | 模块走查清单 + mock 状态表更新 |
| F3 | 小程序下单闭环 | 开发者工具走查 + type-check 零错误 |

---

## Self-Review 记录（writing-plans 自审）

**1. Spec 覆盖检查**：

| 上游要求 | 落点 |
|----------|------|
| 整合设计 §7.1 Admin（PCWeb 骨架加法/CRM 迁移/菜单重组/权限标识） | F2 任务表（F2-0 封装层、F2-1 Basic 权限种子、F2-5 Crm 重写、F2-9 dashboard 并入） |
| 整合设计 §7.2 Site（11 页/删 admin/改指 /api/site/询价落 site_inquiry） | F1 范围声明 + F1-6 询价闭环 |
| 整合设计 §7.3 MiniApp（全页面/product_channel/微信登录/支付） | F3 任务表（F3-0 渠道过滤、F3-4 登录支付） |
| 整合设计 §10 "每阶段进场前 writing-plans 拆细" | F1~F3 蓝图 + 细化机制声明 |
| 前端规范 §1 契约（信封/GUID/camelCase/分页/禁 enum/SCSS/i18n） | Global Constraints 1~6、16、17；Task 5/8/9 代码逐条体现 |
| 前端规范 §2.4 composables 签名 | F2-0 明确"签名逐字对齐规范 2.4" |
| 前端规范 §3.3 Site 专项（断点/双语/询价防重/SEO） | Task 8 mixins + F1-6/F1-7 |
| 前端规范 §4 MiniApp 六层/命名/setData 纪律 | Task 9 + F3-2 |
| mock 规范 §2~§7（独立服务/7700/三分区/信封 helper/store/reset/状态表） | Task 1~2 全量落地 |
| mock 规范 §8~§10 退役与状态表 | "验收与 mock 退役机制"节 |
| 用户指令：参考重写不复制 | Global Constraints 12 |
| 用户指令：顺序 框架→Site→Admin→MiniApp | 阶段总览表 |
| 用户指令：Admin 模块序 Basic→…→Ops | F2 蓝图开头声明 |

**2. 占位符扫描**：全文无 TBD/TODO/"类似 Task N"式省略；Site/MiniApp 中与 Admin 相同的配置文件以"复制命令/逐字一致"显式表达（可执行，无歧义）。

**3. 类型/命名一致性**：`get/post/put/del<T>`（Admin）、`get/post<T>`（Site）、`request<T>`（MiniApp）三端签名全文一致；`PageQuery{pageIndex,pageSize}` / `PageResult{list,total}` 三端一致；mock 路由 URL（/api/admin/auth/login、/api/admin/basic/menu/list、/api/admin/basic/dict-data、/api/site/banner/list、/api/site/news/list、/api/app/auth/wx-login、/api/i18n/:lang/:file）在 Task 2 定义、Task 5/8/9 消费处逐字一致；`useUserStore.has` 与 mock `permissions: ['*']` 通配约定一致。

**已知待后续阶段解决项（非本计划缺陷）**：
- request.ts 拦截器文案走 i18n key 的 ElMessage 直显问题：F2-0 封装层落地时统一用 `tStandalone()` 包装（Task 5 Step 5.2 已留一行实现路径）。
- app.json tabBar 文案无法 i18n：F3-6 自定义 tabBar 解决。
- Site 移动端抽屉导航：F1-7 落地（F0 仅隐藏导航链接，非缺陷）。

---

*计划版本：v1.0（2026-08-08）。F0 执行期间如发现契约级问题，先修本计划再改代码。*