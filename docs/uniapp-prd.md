# EasyProduct.UniApp 产品需求文档（PRD）

> 版本：v1.0 ｜ 日期：2026-09-04 ｜ 状态：待评审
> 上一环节：brainstorming 五项关键决策已确认（见 1.3）

---

## 1. 背景与目标

### 1.1 背景

EasyProduct 现有移动端为 `EasyProduct.MiniApp`（微信原生小程序），共 14 个页面，对接 mock-server（端口 7700）的 `/api/app/**` 接口。为利用 uniapp 一套代码多端运行的优势，新建 `EasyProduct.UniApp` 前端项目。

### 1.2 目标

1. 基于 uniapp（Vue3 + TS）重建移动商城前端，功能与 MiniApp 对等。
2. 首期编译到**微信小程序**与 **H5** 两个平台。
3. 数据层完全复用现有 mock-server 的 `/api/app/**` 契约，后端交付后无缝切换。
4. 沉淀项目第四端统一的前端规范实践（i18n、统一响应处理、状态管理）。

### 1.3 关键决策记录

| # | 决策点 | 结论 |
|---|--------|------|
| 1 | 项目定位 | **并行共存**：新需求只做 UniApp，MiniApp 只修 Bug |
| 2 | 目标平台 | **微信小程序 + H5**，架构预留 App 扩展能力 |
| 3 | 功能范围 | **全量迁移** MiniApp 现有 14 个页面 |
| 4 | 技术栈 | **Vue3 + TypeScript + Pinia + Vite + uni-ui** |
| 5 | 多语言 | **保留 zh-CN / en-US 双语 i18n** |

---

## 2. 用户与场景

### 2.1 目标用户

- 微信小程序用户：扫码/搜一搜进入商城，复用微信生态（登录、分享）。
- H5 用户：浏览器直接访问（官网引流、运营落地页、微信内置浏览器分享链接）。

### 2.2 核心用户旅程

浏览商品 → 加入购物车 → 登录 → 管理收货地址 → 下单结算 → 支付 → 查看订单。

---

## 3. 功能范围

首期全量迁移 MiniApp 现有 14 个页面（4 Tab + 10 二级页）：

### 3.1 Tab 页（4 个）

| 页面 | 路由 | 功能要点 |
|------|------|----------|
| 首页 | `pages/index/index` | 轮播图、公告入口、商品推荐流 |
| 分类 | `pages/category/category` | 左侧分类导航 + 右侧商品列表 |
| 购物车 | `pages/cart/cart` | 商品勾选、数量增减、删除、全选、合计、去结算 |
| 我的 | `pages/profile/profile` | 登录/头像昵称、订单快捷入口、积分、地址管理入口、公告 |

### 3.2 二级页面（10 个）

| 页面 | 路由 | 功能要点 |
|------|------|----------|
| 商品详情 | `pages/product/detail/detail` | 商品信息、规格展示、加入购物车、立即购买 |
| 订单列表 | `pages/order/list/list` | 按状态筛选（pending/paid/shipped/completed/cancelled） |
| 订单详情 | `pages/order/detail/detail` | 订单状态、商品明细、金额、取消订单 |
| 地址列表 | `pages/address/list/list` | 地址列表、默认地址标识、删除 |
| 地址编辑 | `pages/address/edit/edit` | 新增/编辑地址、设为默认 |
| 支付结算 | `pages/pay/checkout/checkout` | 地址选择、商品清单、金额汇总、提交订单 |
| 支付结果 | `pages/pay/result/result` | 支付成功/失败状态展示与跳转 |
| 搜索 | `pages/search/search` | 关键词搜索商品、结果列表 |
| 公告列表 | `pages/announcement/index` | 公告分页列表、未读标识 |
| 公告详情 | `pages/announcement/detail` | 公告内容、标记已读 |

### 3.3 平台差异适配

| 能力 | 微信小程序 | H5 |
|------|-----------|-----|
| 登录 | `wx.login` → `/auth/wx-login` | 账号/验证码 mock 登录 → `/auth/h5-login`（见 5.2） |
| 支付 | mock 直接返回成功 | mock 直接返回成功 |
| 分享 | 微信原生分享能力 | 浏览器/微信内置浏览器分享链接 |
| tabBar | uniapp 原生 tabBar 配置 | uniapp 自动渲染 H5 底部导航 |

---

## 4. 非功能需求

1. **多语言**：全量文案走 i18n（zh-CN / en-US），语言包从 MiniApp 迁移复用，禁止硬编码中文。
2. **统一响应**：`{code, message, data, timestamp}`，HTTP 一律 200，`code===200` 成功；分页 `pageIndex/pageSize + list/total`；GUID 主键；状态字段小写字符串常量。
3. **数据适配**：dev 阶段 `apiBase` 指向 `http://localhost:7700/api/app`（与 MiniApp `config/env.ts` 同策略）；后端交付后仅改 `apiBase` 即可切换。
4. **性能**：首屏可交互 < 2s（H5 本地 mock 环境）；商品列表分页加载。
5. **兼容性**：微信小程序基础库 ≥ 2.20；H5 支持现代浏览器 + 微信内置浏览器。

---

## 5. API 依赖与 Mock 现状

### 5.1 现有可直接对接的接口（21 个）

| 模块 | 接口 |
|------|------|
| product | `GET /categories`、`GET /products`、`GET /products/:id` |
| cart | `GET /`、`POST /`、`PUT /:id`、`DELETE /:id`、`DELETE /clear` |
| order | `POST /`、`GET /`、`GET /:id`、`POST /:id/cancel` |
| payment | `POST /pay` |
| member | `GET /profile`、`GET /points` |
| auth | `POST /auth/wx-login` |
| announcement | `GET /`、`GET /:id`、`POST /:id/read`、`GET /unread-count` |

### 5.2 Mock 缺口（需先补齐，遵守 mock-guidelines 与后端规范第 5 节）

1. **`/api/app/addresses` 路由缺失**：MiniApp 的 `api/address.ts` 调用 `GET/POST/PUT/DELETE /addresses`，但 mock-server `/api/app` 下无 address 路由。需新增：地址列表/详情/新增/更新/删除。
2. **H5 登录缺失**：现有 `POST /auth/wx-login` 仅支持微信。需新增 H5 登录接口（如 `POST /auth/h5-login`，mock 账号/验证码方式），返回与 wx-login 相同的 token 结构。

> 以上两项为 UniApp 开发的**前置依赖**，需按 mock 规范"契约先行"补齐后再启动对应页面开发。

---

## 6. 项目结构（建议）

```
EasyProduct.UniApp/
├── src/
│   ├── api/            # 6 个模块：address/announcement/cart/member/order/product
│   ├── config/         # env.ts（useMock / apiBase，与 MiniApp 同策略）
│   ├── i18n/           # zh-CN / en-US 语言包
│   ├── pages/          # 与 3.1/3.2 页面清单一致
│   ├── static/         # 图标、图片
│   ├── stores/         # Pinia：user / cart
│   ├── types/          # 接口类型定义
│   └── utils/          # request 封装（统一响应/错误处理/token）
├── pages.json          # 路由 + tabBar（4 Tab）
├── manifest.json       # 小程序 appid + H5 配置
└── uni.scss            # 全局样式变量
```

---

## 7. 验收标准

1. 14 个页面在微信小程序与 H5 双端功能可用，与 MiniApp 对等。
2. 全部数据来自 mock-server，无硬编码假数据。
3. i18n 检查通过：无硬编码中文，zh-CN/en-US 双语完整。
4. `vue-tsc` 零错误、ESLint 通过。
5. 统一响应/分页/状态常量符合 AGENTS.md 规范。

---

## 8. 风险与开放问题

| 风险 | 影响 | 缓解 |
|------|------|------|
| mock 缺口（addresses、h5-login） | 阻塞地址与 H5 登录功能 | 前置补齐，契约先行 |
| uni-ui 在 H5 端个别组件差异 | UI 不一致 | 双端冒烟测试覆盖 |
| MiniApp 与 UniApp 长期并行 | 双端 mock 契约漂移 | mock 契约变更有 review 门槛 |
| 后端交付后切换真接口 | 契约不一致 | mock 与后端规范第 5 节逐字对齐 |

---

## 9. 后续流程

1. 本 PRD 评审通过后 → 调用 writing-plans 技能产出实施计划。
2. 实施计划确认后 → 按"补 mock 缺口 → 项目脚手架 → 页面开发（按 Tab 顺序）"推进。
