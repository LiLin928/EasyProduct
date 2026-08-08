# EasyProduct Mock 数据开发使用规范

> **适用对象**：`mock-server/` 独立 Mock 服务，及三端（Admin/Site/MiniApp）联调切换
> **上游依据**：整合设计方案第 6 节（数据模型契约基准）、`docs/backend-guidelines.md` 第 5 节（API 设计约定）、`docs/frontend-guidelines.md` 1.1/1.2（数据契约）
> **决策来源**：2026-08-08 评审确认——独立 Mock Server（三端共享）+ mockjs 数据生成

---

## 1. 定位与总则

1. **契约先行**：所有 mock 接口必须以整合设计第 6 节数据模型 + 后端规范第 5 节路由/信封约定为唯一依据。**mock 不是"前端想要什么就造什么"**，它是后端接口的提前实现。
2. **mock 只覆盖后端尚未实现的模块**。后端每交付一个模块，对应 mock 走第 8 节流程下线；P6 冻结时 mock-server 整体退役（保留仓库归档）。
3. 后端已实现的接口**禁止**前端继续指向 mock（防止契约漂移掩盖真实问题）。
4. 阶段对照（整合设计第 10 节）：P0~P1 期间 mock 覆盖 Admin 全部模块；此后随 P2~P5 逐模块收缩，收缩进度登记在第 10 节状态表。

## 2. 架构与启动

- 技术栈：**Node 20 + TypeScript + Express + mockjs**（复用 EasyCRM 数据资产），开发态用 `tsx` 直跑，无需编译步骤。
- 端口：**7700**（后端 7600、前端 Admin 3000、Site 3001，互不冲突）。
- 一个服务同时模拟三分区：`/api/admin/**`、`/api/site/**`、`/api/app/**`，路由与后端逐字一致。

```bash
cd mock-server
pnpm install
pnpm dev          # tsx watch src/server.ts，端口 7700
```

### 2.1 三端接入方式

| 端 | 指向 mock | 指向真后端 |
|----|----------|-----------|
| Admin / Site | `.env.development` 设 `VITE_API_BASE_URL=http://localhost:7700`（或 vite proxy `/api` → 7700） | `VITE_API_BASE_URL=http://localhost:7600`（或 proxy → 7600） |
| MiniApp | `config/env.ts` 中 `dev` 环境 `baseUrl='http://localhost:7700'`（开发者工具勾选"不校验合法域名"） | 切到真后端地址 |

- 切换只允许通过**环境变量/环境配置文件**完成，禁止在业务代码里写死 mock 地址或按 URL 判断走 mock。

## 3. 目录结构

```text
mock-server/
├─ package.json
├─ tsconfig.json
├─ README.md                      # 启动说明 + 模块 mock 状态表（第 10 节模板）
└─ src/
   ├─ server.ts                   # Express 装配：cors、json body、路由挂载、/__mock/reset
   ├─ helpers/
   │  ├─ envelope.ts              # ok / fail / paginate（唯一响应出口）
   │  ├─ auth.ts                  # Token 模拟与分区守卫
   │  └─ id.ts                    # guid()、code()、isoTime() 等生成工具
   ├─ data/                       # 数据工厂（mockjs 模板），按模块分文件
   │  ├─ basic.ts  site.ts  product.ts  mall.ts
   │  ├─ crm/customer.ts  crm/supplier.ts  crm/inventory.ts  ...
   │  ├─ workflow.ts  report.ts  ops.ts
   ├─ store/                      # 内存态（可 CRUD 的"活"数据），模块一个文件
   │  └─ crm.ts / mall.ts / ...
   └─ routes/                     # 路由定义，镜像后端 Controllers 三分区
      ├─ admin/
      │  ├─ basic/auth.ts  basic/dict.ts  basic/user.ts
      │  └─ crm/customer.ts  crm/sales-order.ts  ...
      ├─ site/
      │  └─ news.ts  banner.ts  inquiry.ts  ...
      └─ app/
         └─ product.ts  cart.ts  order.ts  member.ts  ...
```

命名规则：

1. `routes/` 文件与后端 Controller 一一对应：`CustomerController` → `routes/admin/crm/customer.ts`。
2. `data/` 文件与模块对应，导出 mockjs 生成的数组/对象；`store/` 包装成可增删改的内存集合（`let customerStore = [...customerList]`）。
3. 每个路由文件头部注释登记对应后端模块与状态：`// [backend: Crm 模块 | status: pending|deprecated]`。

## 4. 响应信封与 Helper 规范

所有响应**必须**经 `helpers/envelope.ts` 输出，禁止路由里手写 JSON 结构：

```typescript
// helpers/envelope.ts
export const ok = <T>(data: T, message = '操作成功') =>
  ({ code: 200, message, data, timestamp: Date.now() })

export const fail = (message: string, code = 400) =>
  ({ code, message, data: null, timestamp: Date.now() })

export interface PageData<T> { list: T[]; total: number; pageIndex: number; pageSize: number; totalPages: number; hasNextPage: boolean; hasPrevPage: boolean }

export function paginate<T>(source: T[], pageIndex = 1, pageSize = 10, keyword?: string): PageData<T> {
  const filtered = keyword
    ? source.filter(i => JSON.stringify(i).toLowerCase().includes(keyword.toLowerCase()))
    : source
  const totalPages = Math.ceil(filtered.length / pageSize)
  return {
    list: filtered.slice((pageIndex - 1) * pageSize, pageIndex * pageSize),
    total: filtered.length, pageIndex, pageSize, totalPages,
    hasNextPage: pageIndex < totalPages, hasPrevPage: pageIndex > 1,
  }
}
```

强制点：

1. 分页入参读 `pageIndex/pageSize`（**不是 pageNum**，EasyCRM 旧写法迁入时改写）。
2. code 语义与后端规范 5.3 表完全一致（200/400/401/403/404/500）。
3. HTTP 状态码一律 200（含 fail），与真后端行为一致。

## 5. 数据生成约定（mockjs）

1. **id 一律 GUID**：`"id": "@guid"`；外键引用真实 store 内已有 id（见 5.4），禁止自造不存在的关联 id。
2. **时间字段 ISO 8601**：统一用 `helpers/id.ts` 的 `isoTime()`（如 `2026-08-08T10:00:00`，在最近 N 天内随机），不依赖 mockjs 的格式 token；禁止时间戳数字、禁止 `yyyy-MM-dd` 裸日期（与后端 JSON 输出一致）。
3. **金额**：`"@float(0, 10000, 2, 2)"`，字符串或数字均可，但同一字段全端一致（与前端 types 对齐，统一 number）。
4. **状态/类型字段用小写字符串常量**，取值必须与后端 `Constants` + `basic_dict` 种子一致（如订单 `pending/paid/shipped/finished`），禁止数字码、禁止自造状态值。
5. **列表规模**：列表类数据 23~58 条（保证翻页可测）；明细行 1~5 条。
6. **状态全覆盖**：每个列表内每种状态值至少出现一条，方便前端联调标签/筛选。
7. **中文拟真**：名称用 `@cname/@ctitle/@cword`，公司名带"有限公司/集团"后缀，电话用 `/^1[3-9]\d{9}$/`——继承 EasyCRM 模板风格。
8. 数据工厂只负责"生成一次"；运行时增删改发生在 `store/` 层，保证同一会话内数据自洽（新增后列表能查到、详情能打开）。
### 5.9 跨模块引用完整性（store 共享）

订单、出库单等引用客户/SKU/仓库的数据，**必须从共享 store 中取真实存在的 id**，不得各自独立 mockjs 生成：

```typescript
// routes/admin/crm/sales-order.ts
import { customerStore } from '../../../store/crm'
import { skuStore } from '../../../store/product'

const order = {
  id: guid(),
  customerId: customerStore[0].id,          // 取 store 内真实客户
  items: [{ skuId: skuStore[0].id, qty: 2 }],
  ...
}
```

---

## 6. 认证与权限模拟

1. **管理端**：`POST /api/admin/auth/login` 固定账号 `admin / admin123`，返回 `accessToken + refreshToken + 权限列表`（权限标识与菜单种子数据逐字一致，如 `crm:customer:edit`）；`POST /api/admin/auth/refresh` 换新 access。
2. **小程序端**：`POST /api/app/auth/wx-login` 不校验真实 code，任意 code 返回固定会员的 `memberToken`。
3. **分区守卫模拟**：`/api/admin/**` 校验请求头含 `Authorization: Bearer *`，缺失返回信封 401；非登录态权限校验从简（权限标识检查可选做，默认放行，避免阻塞前端）。
4. 预置账号：1 个超级管理员（全权限）+ 2 个普通角色账号（限定菜单），用于前端权限控制联调。

## 7. 状态管理与重置

1. 数据一律**内存态**（进程内变量），重启即重置为 data/ 工厂初始值——mock 不承担持久化职责。
2. 提供 `POST /__mock/reset`：所有 store 恢复初始值，返回 ok；前端联调脚本/Playwright 用例可调用它准备干净环境。
3. 提供 `GET /__mock/status`：返回各模块 mock 状态表（与 README 第 10 节一致），便于快速确认哪些接口还是 mock。
4. 禁止在 mock-server 内接数据库、读写文件做持久化。

## 8. Mock → 真实切换流程（后端交付一个模块后）

1. **对照**：后端模块完成 → 导出 `swagger.json` → 按第 9 节清单与 mock 逐接口、逐字段对照，不一致处**以后端为准修正 mock**（若后端违反规范，修后端）。
2. **切换**：前端把该模块请求指向真后端（env 切换是全局的；逐模块切换期间允许 vite proxy 按路径分流：已交付模块 → 7600，未交付 → 7700）。
3. **标记**：mock 路由文件头状态改 `deprecated`，README 状态表同步更新（日期 + 经手人）。
4. **清理**：P6 冻结时删除全部 deprecated 路由文件，mock-server 归档（整合设计 P6 交付物）。

## 9. 契约一致性验收清单（mock ↔ 真后端）

每个模块切换前逐项打勾：

- [ ] URL 逐字一致（含分区前缀 `/api/admin|site|app`、资源名连字符、`/list`、`/{id}`）
- [ ] HTTP 方法一致（GET list/detail、POST 创建、PUT 更新、DELETE 删除）
- [ ] 信封结构一致：`{code,message,data,timestamp}`，code 取值语义一致
- [ ] 分页入参 `pageIndex/pageSize`，出参含 `list/total`
- [ ] 字段名全部 camelCase，无多余字段、无缺失字段（对照 swagger.json）
- [ ] id 为 GUID 字符串；时间为 ISO 8601
- [ ] 状态/枚举字符串值与 `basic_dict` 种子、前端常量三方一致
- [ ] 错误场景一致：404（不存在）、400（校验失败）的 message 形态一致
- [ ] 前端切换后冒烟：列表/详情/创建/编辑/删除走一遍无报错

## 10. 模块 Mock 状态表（README 模板）

| 分区 | 模块 | mock 状态 | 后端交付 | 切换日期 | 经手 |
|------|------|----------|---------|---------|------|
| admin | Basic（用户/角色/菜单/字典） | pending | P1 | — | — |
| admin | Ops（日志/任务） | pending | P1 | — | — |
| admin | Product（商品中心） | pending | P2 | — | — |
| site | 官网内容（新闻/Banner/询价） | pending | P2 | — | — |
| app | 商城（会员/购物车/订单） | pending | P3 | — | — |
| admin | Crm（主数据/销售/采购/库存/发票/收付款/应收应付/固定资产/冲销） | pending | P4 | — | — |
| admin | Workflow | pending | P5 | — | — |
| admin | Report | pending | P5 | — | — |

> 状态值：`pending`（mock 生效中）→ `deprecated`（后端已交付、已切换）→ `removed`（P6 已清理）

## 11. 禁止事项

1. 禁止自创后端规范之外的接口、字段、业务流程（mock 的职责是"提前实现契约"，不是"发明需求"）。
2. 禁止 URL 与后端路由不一致（大小写、连字符、复数都算不一致）。
3. 禁止前端代码依赖 mock 特有字段/行为（如 mock 返回的调试字段）。
4. 禁止在 mock 里实现真实微信支付、真实短信等第三方调用——第三方回调只造成功/失败两种报文。
5. 禁止把生产/真实环境数据导入 mock（脱敏风险）；数据一律 mockjs 生成。

---

## 附录：与 EasyCRM 旧 mock 的迁移对照

| EasyCRM 现状 | EasyProduct mock-server 调整 |
|--------------|------------------------------|
| vite-plugin-mock 内嵌（仅 Web 可用） | 独立 Express 服务，三端共享（小程序可用） |
| `mock/server/*` + `mock/data/*` | `src/routes/**` + `src/data/**`（结构保留，路径重组） |
| 分页参数 `pageNum` | 统一 `pageIndex` |
| URL `/api/master/customer/*` | 按新契约 `/api/admin/crm/customer/*` |
| 状态值基本为字符串 | 保留，并与 `basic_dict` 种子对齐校准 |
| org/finance/accounting/cost 模块数据 | 丢弃（整合设计已裁） |