# Admin 端会员收货地址管理实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在 EasyProduct.Admin 后台新增"会员收货地址管理"页面，复用 `/api/app/addresses` 已确立的契约与业务规则，admin 端可读/改/删/设默认全量地址，与小程序端共享 mock 数据。

**Architecture:** 三个交付层——(1) mock-server 端补 6 个 admin 路由并升级 `data/address.ts` 为运行时 `Map<memberId, Address[]>`（与 app 端共享）；(2) Admin 前端补类型/API/i18n/路由/页面，BaseSearchForm + BaseTable + BaseFormDialog 范式；(3) 菜单树 + 角色权限种子更新。前端 0 改动即可对接真后端（路径完全一致）。

**Tech Stack:** Vue 3.4 + TypeScript 5.3 + Element Plus 2.6+ + Pinia + vue-i18n + vue-router；mock-server = Node 20 + Express + tsx；包管理 pnpm。

**Spec:** `docs/superpowers/specs/2026-09-04-miniapp-address-admin-design.md`

**上游依据：**
- `docs/backend-guidelines.md` 第 5、6、9 节
- `docs/frontend-guidelines.md` 第 2 节（Admin 端）
- `docs/mock-guidelines.md` 第 3-7 节
- `docs/AGENTS.md` 自动化命令清单

---

## Global Constraints

1. 统一响应信封 `{code, message, data, timestamp}`，HTTP 一律 200，`code===200` 成功。
2. 路由：`/api/admin/mall/address/...`（admin 端），与 `/api/app/addresses`（app 端，已存在）共享底层数据。
3. 主键 GUID 字符串；JSON camelCase；分页 `pageIndex/pageSize`，出参 `list/total`。
4. 禁用 TS enum；用字符串联合类型 + `as const` 常量对象。
5. `no-explicit-any: error`、`no-console: error`；`vue-tsc --noEmit` 零错误才能提交。
6. 全部可见文案走 i18n，禁止硬编码中文。
7. API 文件 `api/mall/address.ts` 不带 Api 后缀；类型 `types/mall/address.types.ts`；接口无 I 前缀。
8. Conventional Commits，scope = `admin` 或 `mock`。
9. mock 路由与后端规范第 5 节逐字一致；不允许自创字段/接口。
10. `PUT /:id/default` 必须在 `PUT /:id` 之前声明（Express 路由匹配顺序坑，与 app 端同模式）。
11. admin 与 app 共享 `ADDRESSES: Map<memberId, Address[]>` 运行时数据；admin 改了小程序端下次请求立即看到。
12. 默认地址唯一（业务规则：新增/更新 isDefault=true 时清掉同 memberId 其它默认；删除默认地址时自动提升同 memberId 最新一条 createdAt 的为默认）。
13. 手机号正则 `^1[3-9]\d{9}$`；必填字段：`memberId / name / phone / province / city / district / detail`。
14. fullAddress 由服务端拼接 `{province} {city} {district} {detail}`，前端只读。
15. 后端 `EasyProduct.WebApi` 目录当前不存在，本期不交付后端代码；契约与 mock 端完全一致，P3-ext 阶段建实体。

---

## 文件结构

### 新建

| 路径 | 职责 |
|---|---|
| `mock-server/src/routes/admin/mall-address.ts` | admin 端 6 路由（list/详情/POST/PUT/PUT:default/DELETE） |
| `EasyProduct.Admin/src/views/mall/address/index.vue` | 列表页（搜索 + 表格 + 行内操作） |
| `EasyProduct.Admin/src/views/mall/address/components/AddressFormDialog.vue` | 新增/编辑弹窗 |
| `EasyProduct.Admin/src/api/mall/address.ts` | API 层（6 方法） |
| `EasyProduct.Admin/src/types/mall/address.types.ts` | 类型（Address / AddressListItem / AddressQuery / AddressParams） |
| `EasyProduct.Admin/src/router/modules/mall-address.ts` | 路由模块 |
| `docs/superpowers/plans/2026-09-04-miniapp-address-admin.md` | 本计划文件 |

### 修改

| 路径 | 改动 |
|---|---|
| `mock-server/src/data/address.ts` | `seedAddressesByMember` 升级为 `ADDRESSES: Map<string, Address[]>` 运行时存储，app 端 `routes/app/address.ts` 同步改写 |
| `mock-server/src/routes/app/address.ts` | 内部存储从 `seedAddressesByMember` 迁移到 `ADDRESSES`（app 端先已落账，本次同步保持行为不变） |
| `mock-server/src/server.ts` | 注册 `adminMallAddressRouter` 到 `/api/admin` |
| `mock-server/src/data/basic.ts` | `MENU_TREE` mall 节点下追加 `mall-address` 子节点；`USER_PERMISSIONS.sales` 追加 `mall:address:list`、`mall:address:edit` |
| `EasyProduct.Admin/src/router/index.ts` | 挂载新路由模块（需先检查现有结构，必要时并入现有 mall 路由） |
| `EasyProduct.Admin/src/types/mall.ts` | 末尾追加 `export * from './mall/address.types'` |
| `EasyProduct.Admin/src/i18n/zh-CN/menu.json` | 新增 `mallAddress: "收货地址管理"` |
| `EasyProduct.Admin/src/i18n/en-US/menu.json` | 新增 `mallAddress: "Address Management"` |
| `EasyProduct.Admin/src/i18n/zh-CN/mall.json` | 新增 `address.*` 命名空间 |
| `EasyProduct.Admin/src/i18n/en-US/mall.json` | 新增 `address.*` 命名空间 |
| `docs/mock-guidelines.md` | 第 10 节 MOCK_STATUS 增"会员收货地址（Admin 端）" |

---

## 任务列表

| Task | 主题 | 交付物 | 提交类型 |
|---|---|---|---|
| 1 | mock 数据共享层升级 | `data/address.ts` 运行时 Map | `refactor(mock)` |
| 2 | admin 端 mock 路由 6 端点 | `routes/admin/mall-address.ts` + `server.ts` 注册 | `feat(mock)` |
| 3 | mock 端端到端验证 | curl 6 端点 + 业务规则 + 跨端共享 | (无提交) |
| 4 | mock 端菜单 + 权限种子 | `data/basic.ts` 增 `mall-address` + sales 角色权限 | `feat(mock)` |
| 5 | mock 端 commit + push | 3 个 commit | (git) |
| 6 | Admin 类型 + API + i18n | `types/mall/address.types.ts` + `api/mall/address.ts` + 2 个 i18n JSON + menu 2 个 | `feat(admin)` |
| 7 | Admin 路由 + 类型 barrel 导出 | `router/modules/mall-address.ts` + `types/mall.ts` 追加 | `feat(admin)` |
| 8 | Admin 列表页 | `views/mall/address/index.vue` | `feat(admin)` |
| 9 | Admin 表单弹窗 | `views/mall/address/components/AddressFormDialog.vue` | `feat(admin)` |
| 10 | Admin 提交前自检 | type-check + lint + check:i18n | (无提交) |
| 11 | Admin commit + push | 3 个 commit | (git) |
| 12 | 文档收口 | MOCK_STATUS 补登 + spec 落账 + plan 落账 | `docs` |

---

## Task 1: mock 数据共享层升级

**Files:**
- Modify: `D:\4-MyProject\EasyProduct\mock-server\src\data\address.ts`
- Modify: `D:\4-MyProject\EasyProduct\mock-server\src\routes\app\address.ts`

**Interfaces:**
- Consumes: 现有 `Address` 接口 + 5 条种子数据
- Produces: `ADDRESSES: Map<string, Address[]>` 运行时 Map + `getMemberAddresses(memberId)` / `getAddressById(id)` / `addAddress / updateAddress / deleteAddress / setDefaultAddress` 工具函数；`resetAddresses()` 供测试或重置用

**目标：** 把数据从"一次性种子"升级为"运行时可读写"，admin 与 app 端共享同一 Map。

- [ ] **Step 1.1: 改写 `data/address.ts`**

把 `seedAddressesByMember: Map<string, Omit<...>[]>` 改为 `ADDRESSES: Map<string, Address[]>`，并在文件末尾添加辅助函数（同步初始化种子）：

```ts
// src/data/address.ts
import { MEMBERS } from './mall.js'

export interface Address {
  id: string
  memberId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
  fullAddress: string
  createdAt: string
  updatedAt: string
}

const buildFullAddress = (a: Pick<Address, 'province' | 'city' | 'district' | 'detail'>): string =>
  `${a.province} ${a.city} ${a.district} ${a.detail}`

const seedTemplate: Omit<Address, 'id' | 'memberId' | 'createdAt' | 'updatedAt'>[] = [
  { name: '张三', phone: '13800138001', province: '广东省', city: '深圳市', district: '南山区', detail: '科技园路 1 号 A 座 1801 室', isDefault: true,  fullAddress: '' },
  { name: '张三', phone: '13800138001', province: '广东省', city: '广州市', district: '天河区', detail: '珠江新城兴民路 222 号 5 栋 2 单元 803', isDefault: false, fullAddress: '' },
  { name: '李四', phone: '13900139002', province: '上海市', city: '上海市', district: '浦东新区', detail: '张江路 1238 号 7 号楼 502', isDefault: true, fullAddress: '' },
  { name: '王五', phone: '13700137003', province: '北京市', city: '北京市', district: '海淀区', detail: '中关村大街 27 号 905 大厦 1106 室', isDefault: true, fullAddress: '' },
  { name: '赵六', phone: '13600136004', province: '浙江省', city: '杭州市', district: '西湖区', detail: '文三路 478 号华星时代广场 12 层', isDefault: true, fullAddress: '' },
]

let _seq = 0
const nextId = (): string => `mock-addr-${Date.now().toString(36)}-${(++_seq).toString(36)}`

/** 运行时地址存储（admin/app 共享） */
export const ADDRESSES = new Map<string, Address[]>()

/** 启动时把种子数据写入 ADDRESSES */
function seedInit(): void {
  const now = new Date().toISOString()
  MEMBERS.slice(0, 5).forEach((m, idx) => {
    const tpl = seedTemplate[idx % seedTemplate.length]
    const a: Address = {
      id: nextId(),
      memberId: m.id,
      ...tpl,
      fullAddress: buildFullAddress(tpl),
      createdAt: now,
      updatedAt: now,
    }
    ADDRESSES.set(m.id, [a])
  })
}

seedInit()

/** 重置回种子（测试用） */
export function resetAddresses(): void {
  ADDRESSES.clear()
  seedInit()
}

/** 获取某会员的全部地址 */
export function getMemberAddresses(memberId: string): Address[] {
  return ADDRESSES.get(memberId) ?? []
}

/** 根据 id 查地址（跨会员） */
export function getAddressById(id: string): Address | undefined {
  for (const list of ADDRESSES.values()) {
    const found = list.find(a => a.id === id)
    if (found) return found
  }
  return undefined
}

/** 新增地址（不处理默认地址切换，调用方负责） */
export function addAddress(memberId: string, data: Omit<Address, 'id' | 'memberId' | 'createdAt' | 'updatedAt' | 'fullAddress'>): Address {
  const now = new Date().toISOString()
  const a: Address = {
    id: nextId(),
    memberId,
    ...data,
    fullAddress: buildFullAddress(data),
    createdAt: now,
    updatedAt: now,
  }
  const list = ADDRESSES.get(memberId) ?? []
  list.unshift(a)
  ADDRESSES.set(memberId, list)
  return a
}

/** 更新地址 */
export function updateAddress(id: string, patch: Partial<Omit<Address, 'id' | 'memberId' | 'createdAt'>>): Address | undefined {
  const cur = getAddressById(id)
  if (!cur) return undefined
  const merged: Address = {
    ...cur,
    ...patch,
    updatedAt: new Date().toISOString(),
  }
  merged.fullAddress = buildFullAddress(merged)
  const list = ADDRESSES.get(cur.memberId) ?? []
  const idx = list.findIndex(a => a.id === id)
  if (idx >= 0) list[idx] = merged
  ADDRESSES.set(cur.memberId, list)
  return merged
}

/** 删除地址（不处理默认提升，调用方负责） */
export function deleteAddress(id: string): boolean {
  for (const [memberId, list] of ADDRESSES.entries()) {
    const idx = list.findIndex(a => a.id === id)
    if (idx >= 0) {
      list.splice(idx, 1)
      if (list.length === 0) ADDRESSES.delete(memberId)
      return true
    }
  }
  return false
}

/** 切换默认地址：把目标设为默认，同 memberId 其它取消默认 */
export function setDefaultAddress(id: string): Address | undefined {
  const target = getAddressById(id)
  if (!target) return undefined
  const list = ADDRESSES.get(target.memberId) ?? []
  for (const a of list) {
    a.isDefault = a.id === id
  }
  target.updatedAt = new Date().toISOString()
  return target
}

/** 全表扁平（admin list 用） */
export function listAllAddresses(): Address[] {
  const out: Address[] = []
  for (const list of ADDRESSES.values()) out.push(...list)
  return out
}
```

- [ ] **Step 1.2: 改写 `routes/app/address.ts` 改用新 API**

把原文件中所有直接读写 `seedAddressesByMember` 的代码改为调用 `ADDRESSES / getMemberAddresses / getAddressById / addAddress / updateAddress / deleteAddress / setDefaultAddress / listAllAddresses`。

要点：
- `GET /` 改为 `res.json(ok(getMemberAddresses(memberId)))`
- `GET /:id` 改为 `getAddressById` 后校验 memberId
- `POST /` 改为：若 `isDefault=true` 先 `setDefaultAddress` 同 memberId 全部清掉（`updateAddress` 之前先 `updateAddress(otherId, { isDefault: false })`），再 `addAddress`
- `PUT /:id` 改为 `updateAddress`；若 `isDefault=true` 同样清同 memberId 其它
- `PUT /:id/default` 改为 `setDefaultAddress`（路由顺序保持）
- `DELETE /:id` 改为：若被删的是默认且同 memberId 还有其它，取 createdAt 最新一条 `setDefaultAddress`，再 `deleteAddress`

保持现有 JSDoc、字段校验、手机号正则、HTTP code 行为 100% 不变。

- [ ] **Step 1.3: tsc 检查**

```bash
cd D:\4-MyProject\EasyProduct\mock-server
.\node_modules\.bin\tsc --noEmit
```

预期：仅预存的 site-full/site-download/site-video/workflow 错误，address/app-address/admin 0 错误。

- [ ] **Step 1.4: app 端回归（不提交，commit 在 Task 5）**

启动 mock-server 后用上一轮已落账的 5 个端点 curl 全部重跑一遍（沿用 commit `8badc30` 的自测清单），确保 0 行为变化。

---

## Task 2: admin 端 mock 路由 6 端点

**Files:**
- Create: `D:\4-MyProject\EasyProduct\mock-server\src\routes\admin\mall-address.ts`
- Modify: `D:\4-MyProject\EasyProduct\mock-server\src\server.ts`

**Interfaces:**
- Consumes: Task 1 的 `ADDRESSES / getMemberAddresses / getAddressById / addAddress / updateAddress / deleteAddress / setDefaultAddress / listAllAddresses`；`MEMBERS` from `./mall.js`（用于冗余字段 JOIN）；`paginate` from helpers/envelope
- Produces: `adminMallAddressRouter` 挂到 `/api/admin`，含 6 端点

- [ ] **Step 2.1: 创建 `routes/admin/mall-address.ts`**

```ts
// src/routes/admin/mall-address.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { MEMBERS } from '../../data/mall.js'
import {
  ADDRESSES,
  getAddressById,
  getMemberAddresses,
  addAddress,
  updateAddress,
  deleteAddress,
  setDefaultAddress,
  listAllAddresses,
  type Address,
} from '../../data/address.js'

const PHONE_RE = /^1[3-9]\d{9}$/

interface AddressListItem extends Address {
  memberNickname: string
  memberPhone: string
  memberLevelName: string
}

const memberMap = new Map(MEMBERS.map(m => [m.id, m]))
const memberLevelName = (memberId: string): string => {
  const m = memberMap.get(memberId) as (typeof MEMBERS)[number] | undefined
  return m?.levelName ?? '-'
}

const enrich = (a: Address): AddressListItem => {
  const m = memberMap.get(a.memberId)
  return {
    ...a,
    memberNickname: m?.nickname ?? '会员已注销',
    memberPhone: m?.phone ?? '-',
    memberLevelName: memberLevelName(a.memberId),
  }
}

export const adminMallAddressRouter = Router()

/** 分页列表（admin 端，冗余会员字段） */
adminMallAddressRouter.get('/mall/address/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const memberNickname = (req.query.memberNickname as string | undefined)?.trim()
  const memberPhone = (req.query.memberPhone as string | undefined)?.trim()
  const recipientName = (req.query.recipientName as string | undefined)?.trim()
  const isDefault = req.query.isDefault
  const memberId = (req.query.memberId as string | undefined)?.trim()

  let rows = listAllAddresses()
  if (memberId) rows = rows.filter(a => a.memberId === memberId)
  if (memberNickname) {
    rows = rows.filter(a => {
      const m = memberMap.get(a.memberId)
      return m?.nickname?.includes(memberNickname)
    })
  }
  if (memberPhone) {
    rows = rows.filter(a => {
      const m = memberMap.get(a.memberId)
      return m?.phone?.includes(memberPhone)
    })
  }
  if (recipientName) rows = rows.filter(a => a.name.includes(recipientName))
  if (isDefault === 'true') rows = rows.filter(a => a.isDefault)
  if (isDefault === 'false') rows = rows.filter(a => !a.isDefault)

  rows.sort((a, b) => b.updatedAt.localeCompare(a.updatedAt))
  const enriched = rows.map(enrich)
  res.json(ok(paginate(enriched, pageIndex, pageSize)))
})

/** 详情 */
adminMallAddressRouter.get('/mall/address/:id', (req, res) => {
  const a = getAddressById(req.params.id)
  if (!a) return res.json(fail('地址不存在', 404))
  res.json(ok(enrich(a)))
})

/** 新增 */
adminMallAddressRouter.post('/mall/address', (req, res) => {
  const body = req.body ?? {}
  const memberId = (body.memberId as string | undefined)?.trim()
  const name = (body.name as string | undefined)?.trim()
  const phone = (body.phone as string | undefined)?.trim()
  const province = (body.province as string | undefined)?.trim()
  const city = (body.city as string | undefined)?.trim()
  const district = (body.district as string | undefined)?.trim()
  const detail = (body.detail as string | undefined)?.trim()
  const isDefault = body.isDefault === true

  if (!memberId) return res.json(fail('请选择所属会员', 400))
  if (!memberMap.has(memberId)) return res.json(fail('会员不存在', 400))
  if (!name) return res.json(fail('收货人姓名不能为空', 400))
  if (!phone) return res.json(fail('手机号不能为空', 400))
  if (!PHONE_RE.test(phone)) return res.json(fail('手机号格式不正确', 400))
  if (!province) return res.json(fail('省份不能为空', 400))
  if (!city) return res.json(fail('城市不能为空', 400))
  if (!district) return res.json(fail('区/县不能为空', 400))
  if (!detail) return res.json(fail('详细地址不能为空', 400))

  // 先把同 memberId 其它默认清掉
  if (isDefault) {
    for (const a of getMemberAddresses(memberId)) {
      if (a.isDefault) updateAddress(a.id, { isDefault: false })
    }
  }

  const a = addAddress(memberId, { name, phone, province, city, district, detail, isDefault })
  res.json(ok({ id: a.id }, '地址已新增'))
})

/** 更新 */
adminMallAddressRouter.put('/mall/address/:id', (req, res) => {
  const cur = getAddressById(req.params.id)
  if (!cur) return res.json(fail('地址不存在', 404))

  const body = req.body ?? {}
  const name = (body.name as string | undefined)?.trim()
  const phone = (body.phone as string | undefined)?.trim()
  const province = (body.province as string | undefined)?.trim()
  const city = (body.city as string | undefined)?.trim()
  const district = (body.district as string | undefined)?.trim()
  const detail = (body.detail as string | undefined)?.trim()
  const isDefault = body.isDefault === true

  if (!name) return res.json(fail('收货人姓名不能为空', 400))
  if (!phone) return res.json(fail('手机号不能为空', 400))
  if (!PHONE_RE.test(phone)) return res.json(fail('手机号格式不正确', 400))
  if (!province || !city || !district || !detail) return res.json(fail('省/市/区/详细地址均不能为空', 400))

  if (isDefault) {
    for (const a of getMemberAddresses(cur.memberId)) {
      if (a.isDefault && a.id !== cur.id) updateAddress(a.id, { isDefault: false })
    }
  }

  const a = updateAddress(cur.id, { name, phone, province, city, district, detail, isDefault })
  res.json(ok(a, '地址已更新'))
})

/** 设为默认（必须在 PUT /:id 之前声明！——下面就是踩坑注释） */
adminMallAddressRouter.put('/mall/address/:id/default', (req, res) => {
  const a = setDefaultAddress(req.params.id)
  if (!a) return res.json(fail('地址不存在', 404))
  res.json(ok(a, '已设为默认地址'))
})

/** 删除 */
adminMallAddressRouter.delete('/mall/address/:id', (req, res) => {
  const cur = getAddressById(req.params.id)
  if (!cur) return res.json(fail('地址不存在', 404))

  const wasDefault = cur.isDefault
  const memberId = cur.memberId
  deleteAddress(cur.id)

  // 若删的是默认地址，自动提升同 memberId 下一条 createdAt 最新
  if (wasDefault) {
    const remain = getMemberAddresses(memberId)
    if (remain.length > 0) {
      remain.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
      setDefaultAddress(remain[0].id)
    }
  }
  res.json(ok(null, '地址已删除'))
})
```

- [ ] **Step 2.2: 注册到 `server.ts`**

打开 `D:\4-MyProject\EasyProduct\mock-server\src\server.ts`：
1. import 末尾加 `import { adminMallAddressRouter } from './routes/admin/mall-address.js'`
2. `/api/admin` 这一长串 router 列表中，**在 `adminMallMemberRouter` 之后**插入 `adminMallAddressRouter`（按 mall 子模块顺序）

- [ ] **Step 2.3: tsc 检查**

```bash
cd D:\4-MyProject\EasyProduct\mock-server
.\node_modules\.bin\tsc --noEmit
```

预期：admin 0 错误。

- [ ] **Step 2.4: 启动并 6 端点 curl 自测（留待 Task 3 统一跑）**

---

## Task 3: mock 端端到端验证

**Files:** 无（仅运行验证）

- [ ] **Step 3.1: 启动 mock-server**

```bash
cd D:\4-MyProject\EasyProduct\mock-server
npx tsx watch src/server.ts
```

预期 stdout 包含 "Mock server listening on 7700"。

- [ ] **Step 3.2: 拿 admin token**

```bash
# 查看登录端点
cat src/routes/admin/auth.ts | head -50
# 按实际端点拿 token（一般是 POST /api/admin/auth/login）
curl -s -X POST http://localhost:7700/api/admin/auth/login -H "Content-Type: application/json" -d '{"username":"admin","password":"admin123"}'
```

把 token 存为 `$TOKEN` 变量。

- [ ] **Step 3.3: 跑 6 端点全流程**

```bash
# 1) 列表
curl -s "http://localhost:7700/api/admin/mall/address/list?pageIndex=1&pageSize=10" -H "Authorization: Bearer $TOKEN"

# 2) 拿一个 memberId 用于新增
MEMBER_ID=$(curl -s "http://localhost:7700/api/admin/mall/member/list?pageIndex=1&pageSize=1" -H "Authorization: Bearer $TOKEN" | python -c "import sys,json;print(json.load(sys.stdin)['data']['list'][0]['id'])")

# 3) 新增
NEW=$(curl -s -X POST http://localhost:7700/api/admin/mall/address -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d "{\"memberId\":\"$MEMBER_ID\",\"name\":\"测试\",\"phone\":\"13800138000\",\"province\":\"广东省\",\"city\":\"深圳市\",\"district\":\"福田区\",\"detail\":\"xxx\",\"isDefault\":false}")
echo $NEW
NEW_ID=$(echo $NEW | python -c "import sys,json;print(json.load(sys.stdin)['data']['id'])")

# 4) 详情
curl -s "http://localhost:7700/api/admin/mall/address/$NEW_ID" -H "Authorization: Bearer $TOKEN"

# 5) 更新
curl -s -X PUT "http://localhost:7700/api/admin/mall/address/$NEW_ID" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"name":"测试改","phone":"13900139000","province":"广东省","city":"广州市","district":"天河区","detail":"yyy","isDefault":false}'

# 6) 设为默认（验证路由顺序）
curl -s -X PUT "http://localhost:7700/api/admin/mall/address/$NEW_ID/default" -H "Authorization: Bearer $TOKEN"

# 7) 删默认地址（验证自动提升最新一条）
curl -s -X DELETE "http://localhost:7700/api/admin/mall/address/$NEW_ID" -H "Authorization: Bearer $TOKEN"

# 8) 跨端共享验证
TOKEN_APP=$(curl -s -X POST http://localhost:7700/api/app/auth/wx-login -H "Content-Type: application/json" -d '{"code":"test"}' | python -c "import sys,json;print(json.load(sys.stdin)['data']['memberToken'])")
curl -s http://localhost:7700/api/app/addresses -H "Authorization: Bearer $TOKEN_APP"
```

- [ ] **Step 3.4: 边界场景**

- 不带 token：401 + "请先登录"
- 手机号非法：400 + "手机号格式不正确"
- 必填字段缺失：400 + 对应中文 message
- 不存在 id：404 + "地址不存在"

- [ ] **Step 3.5: 关闭 mock-server**

记录 PID 后 `Stop-Process -Id <pid> -Force`。
---

## Task 4: mock 端菜单 + 权限种子

**Files:**
- Modify: `D:\4-MyProject\EasyProduct\mock-server\src\data\basic.ts`

**Interfaces:**
- Consumes: 现有 `MENU_TREE` mall 节点（`{ name: 'mall', children: [...] }`）和 `USER_PERMISSIONS`
- Produces: `MENU_TREE` 多一个 `mall-address` 子节点；`USER_PERMISSIONS.sales` 多两个权限码

- [ ] **Step 4.1: 在 `MENU_TREE` mall children 末尾追加**

找到 mall 节点的 children 数组，在最后一个子项 `mall-payment` 之后追加：

```ts
{ id: guid(), parentId: '0', name: 'mall-address', path: '/mall/address/list', titleKey: 'menu.mallAddress', icon: 'Location', sort: 7, status: 'enabled', visible: true, children: [] },
```

- [ ] **Step 4.2: `USER_PERMISSIONS.sales` 追加权限**

找到 `sales: ['crm:customer:list', 'crm:customer:add', 'mall:order:list']` 这一行，改为：

```ts
sales: ['crm:customer:list', 'crm:customer:add', 'mall:order:list', 'mall:address:list', 'mall:address:edit'],
```

> 注：admin 账号是 `*` 通配，已经包含全部权限；sales 是客服场景需要看+改地址。

- [ ] **Step 4.3: tsc + 启动 + 验证菜单树**

```bash
cd D:\4-MyProject\EasyProduct\mock-server
.\node_modules\.bin\tsc --noEmit
# 启动
npx tsx src/server.ts &
sleep 2
# 拉菜单树
curl -s http://localhost:7700/api/admin/basic/menu/tree -H "Authorization: Bearer $TOKEN" | python -c "import sys,json;m=json.load(sys.stdin)['data'];mall=[c for c in m if c['name']=='mall'][0];print([c['name'] for c in mall['children']])"
# 应输出 ['mall-member', 'mall-level', 'mall-points', 'mall-coupon', 'mall-order', 'mall-payment', 'mall-address']
```

---

## Task 5: mock 端 commit + push

**Files:** `data/address.ts`、`routes/app/address.ts`、`routes/admin/mall-address.ts`、`server.ts`、`data/basic.ts`

> 当前用户对 `.git/` 目录无 WriteData 权限（Codex 沙盒用户），需 escalated 模式执行 git 命令。

- [ ] **Step 5.1: 关闭 mock-server**

```bash
# 找到 PID 后停掉
Stop-Process -Id <pid> -Force
```

- [ ] **Step 5.2: commit 1（数据层升级）**

```bash
cd D:\4-MyProject\EasyProduct
git add mock-server/src/data/address.ts mock-server/src/routes/app/address.ts
git commit -m "refactor(mock): 收货地址存储从 seed 升级为运行时 Map

- 抽出 ADDRESSES: Map<memberId, Address[]> + 工具函数
- 保持 /api/app/addresses 6 端点行为完全不变
- 为 admin 端路由准备共享数据源"
```

- [ ] **Step 5.3: commit 2（admin 路由）**

```bash
git add mock-server/src/routes/admin/mall-address.ts mock-server/src/server.ts
git commit -m "feat(mock): 新增 /api/admin/mall/address 6 路由

- GET /list 分页(冗余会员昵称/手机号/等级)
- GET /:id 详情
- POST / 新增(必填 + 手机号正则 + 默认地址切换)
- PUT /:id 更新
- PUT /:id/default 设为默认(必须先于 /:id 声明)
- DELETE /:id 删除(默认地址删除时自动提升最新一条)
- 与 /api/app/addresses 共用 ADDRESSES 数据"
```

- [ ] **Step 5.4: commit 3（菜单权限）**

```bash
git add mock-server/src/data/basic.ts
git commit -m "feat(mock): 菜单补 mall-address + sales 角色追加权限

- MENU_TREE 增子节点 path=/mall/address/list sort=7 icon=Location
- USER_PERMISSIONS.sales 追加 mall:address:list / mall:address:edit"
```

- [ ] **Step 5.5: fetch + push**

```bash
git fetch origin main
git push origin main
```

预期：3 个 commit 落到 origin main。

---

## Task 6: Admin 类型 + API + i18n

**Files:**
- Create: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\types\mall\address.types.ts`
- Create: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\api\mall\address.ts`
- Modify: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\i18n\zh-CN\menu.json`
- Modify: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\i18n\en-US\menu.json`
- Modify: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\i18n\zh-CN\mall.json`
- Modify: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\i18n\en-US\mall.json`

**Interfaces:**
- Consumes: `@/types/api` 的 `PageQuery`
- Produces: `Address` / `AddressListItem` / `AddressQuery` / `AddressParams`；6 个 API 函数

- [ ] **Step 6.1: 新建 `types/mall/address.types.ts`**

```ts
// src/types/mall/address.types.ts
import type { PageQuery } from '../api'

/** 地址基础字段（与 mock 端契约完全一致） */
export interface Address {
  id: string
  memberId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
  fullAddress: string
  createdAt: string
  updatedAt: string
}

/** 列表冗余字段（mock JOIN，WebApi 阶段冗余列或 SQL JOIN） */
export interface AddressListItem extends Address {
  memberNickname: string
  memberPhone: string
  memberLevelName: string
}

/** 列表查询参数 */
export interface AddressQuery extends PageQuery {
  memberNickname?: string
  memberPhone?: string
  recipientName?: string
  memberId?: string
  isDefault?: boolean
}

/** 新增/编辑表单参数 */
export interface AddressParams {
  memberId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
}

/** 是否默认下拉选项 */
export const ADDRESS_DEFAULT_OPTIONS = [
  { label: 'mall.address.isDefaultYes', value: true as const },
  { label: 'mall.address.isDefaultNo', value: false as const },
]
```

- [ ] **Step 6.2: 新建 `api/mall/address.ts`**

```ts
// src/api/mall/address.ts
import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Address, AddressListItem, AddressQuery, AddressParams } from '@/types/mall/address.types'

/** 列表（分页 + 筛选，冗余会员字段） */
export const getAddressList = (params: AddressQuery) =>
  get<PageResult<AddressListItem>>('/api/admin/mall/address/list', params)

/** 详情 */
export const getAddressDetail = (id: string) =>
  get<AddressListItem>(`/api/admin/mall/address/${id}`)

/** 新增 */
export const createAddress = (data: AddressParams) =>
  post<{ id: string }>('/api/admin/mall/address', data)

/** 更新 */
export const updateAddress = (id: string, data: AddressParams) =>
  put<null>(`/api/admin/mall/address/${id}`, data)

/** 设为默认 */
export const setAddressDefault = (id: string) =>
  put<null>(`/api/admin/mall/address/${id}/default`)

/** 删除 */
export const deleteAddress = (id: string) =>
  del<null>(`/api/admin/mall/address/${id}`)
```

- [ ] **Step 6.3: `i18n/zh-CN/menu.json` 追加**

在 mall 区域 `mallPayment: "支付记录"` 之后加：

```json
"mallAddress": "收货地址管理",
```

- [ ] **Step 6.4: `i18n/en-US/menu.json` 追加**

```json
"mallAddress": "Address Management",
```

- [ ] **Step 6.5: `i18n/zh-CN/mall.json` 追加 `address` 命名空间**

在 mall 顶层对象下（与 `member` / `coupon` / `level` 等并列），追加：

```json
"address": {
  "title": "收货地址管理",
  "list": "地址列表",
  "add": "新增地址",
  "edit": "编辑地址",
  "delete": "删除地址",
  "deleteConfirm": "确认删除该地址吗？此操作不可恢复。",
  "setDefault": "设为默认",
  "memberNickname": "会员昵称",
  "memberPhone": "会员手机号",
  "memberLevelName": "会员等级",
  "recipientName": "收货人",
  "recipientPhone": "收货人手机号",
  "province": "省",
  "city": "市",
  "district": "区",
  "detail": "详细地址",
  "fullAddress": "完整地址",
  "isDefault": "默认地址",
  "isDefaultYes": "默认",
  "isDefaultNo": "否",
  "createdAt": "创建时间",
  "updatedAt": "更新时间",
  "searchMemberNickname": "会员昵称",
  "searchRecipient": "收货人",
  "searchPhone": "手机号",
  "searchIsDefault": "是否默认",
  "form": {
    "addTitle": "新增地址",
    "editTitle": "编辑地址",
    "memberPlaceholder": "请选择所属会员",
    "memberRequired": "请选择所属会员",
    "namePlaceholder": "请输入收货人姓名",
    "nameRequired": "请输入收货人姓名",
    "phonePlaceholder": "请输入 11 位手机号",
    "phoneRequired": "请输入手机号",
    "phoneInvalid": "手机号格式不正确",
    "provincePlaceholder": "省",
    "provinceRequired": "请输入省份",
    "cityPlaceholder": "市",
    "cityRequired": "请输入城市",
    "districtPlaceholder": "区",
    "districtRequired": "请输入区/县",
    "detailPlaceholder": "街道、楼栋、门牌号",
    "detailRequired": "请输入详细地址"
  },
  "message": {
    "createSuccess": "地址创建成功",
    "updateSuccess": "地址更新成功",
    "deleteSuccess": "地址删除成功",
    "setDefaultSuccess": "已设为默认地址"
  }
}
```

- [ ] **Step 6.6: `i18n/en-US/mall.json` 追加（英文版）**

```json
"address": {
  "title": "Address Management",
  "list": "Address List",
  "add": "Add Address",
  "edit": "Edit Address",
  "delete": "Delete Address",
  "deleteConfirm": "Are you sure you want to delete this address? This action cannot be undone.",
  "setDefault": "Set as Default",
  "memberNickname": "Member",
  "memberPhone": "Member Phone",
  "memberLevelName": "Level",
  "recipientName": "Recipient",
  "recipientPhone": "Recipient Phone",
  "province": "Province",
  "city": "City",
  "district": "District",
  "detail": "Detail",
  "fullAddress": "Full Address",
  "isDefault": "Default",
  "isDefaultYes": "Yes",
  "isDefaultNo": "No",
  "createdAt": "Created At",
  "updatedAt": "Updated At",
  "searchMemberNickname": "Member Nickname",
  "searchRecipient": "Recipient",
  "searchPhone": "Phone",
  "searchIsDefault": "Is Default",
  "form": {
    "addTitle": "Add Address",
    "editTitle": "Edit Address",
    "memberPlaceholder": "Select a member",
    "memberRequired": "Please select a member",
    "namePlaceholder": "Enter recipient name",
    "nameRequired": "Please enter recipient name",
    "phonePlaceholder": "Enter 11-digit phone",
    "phoneRequired": "Please enter phone",
    "phoneInvalid": "Invalid phone format",
    "provincePlaceholder": "Province",
    "provinceRequired": "Please enter province",
    "cityPlaceholder": "City",
    "cityRequired": "Please enter city",
    "districtPlaceholder": "District",
    "districtRequired": "Please enter district",
    "detailPlaceholder": "Street, building, room",
    "detailRequired": "Please enter detail"
  },
  "message": {
    "createSuccess": "Address created",
    "updateSuccess": "Address updated",
    "deleteSuccess": "Address deleted",
    "setDefaultSuccess": "Set as default"
  }
}
```

- [ ] **Step 6.7: type-check**

```bash
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

预期：0 错误（仅 admin 全量预存警告，不应新增）。

---

## Task 7: Admin 路由 + 类型 barrel 导出

**Files:**
- Create: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\router\modules\mall-address.ts`
- Modify: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\router\index.ts`（或并入现有 mall 路由模块）
- Modify: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\types\mall.ts`

**Interfaces:**
- Consumes: `@/views/mall/address/index.vue` 路径
- Produces: 路由 `MallAddressList` 映射到 `/mall/address/list`

- [ ] **Step 7.1: 检查现有 router 结构**

```bash
ls D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\router
cat D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\router\index.ts | head -40
```

- [ ] **Step 7.2: 新建 `router/modules/mall-address.ts`**

```ts
// src/router/modules/mall-address.ts
import type { RouteRecordRaw } from 'vue-router'

const MallAddressRoutes: RouteRecordRaw = {
  path: 'address',
  meta: { titleKey: 'menu.mallAddress', icon: 'Location' },
  children: [
    {
      path: 'list',
      name: 'MallAddressList',
      component: () => import('@/views/mall/address/index.vue'),
      meta: { titleKey: 'menu.mallAddress', permission: 'mall:address:list' },
    },
  ],
}

export default MallAddressRoutes
```

- [ ] **Step 7.3: 挂载到 `router/index.ts`**

打开 `router/index.ts`，找到现有 `const MallRoutes: RouteRecordRaw = { path: 'mall', ... }`（或类似），在它的 `children` 数组中追加导入的新模块：

```ts
import MallAddressRoutes from './modules/mall-address'
// ...
children: [
  // ... 已有 mall-member / mall-level / ...
  MallAddressRoutes,
],
```

如果 `router/index.ts` 没有这种集中模式（比如全是模块化），则创建 `router/modules/mall.ts` 把所有 mall 子模块集中。

- [ ] **Step 7.4: 在 `types/mall.ts` 末尾追加 barrel**

```ts
// src/types/mall.ts（末尾）
export * from './mall/address.types'
```

- [ ] **Step 7.5: type-check**

```bash
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

预期：0 错误（提示 `@/views/mall/address/index.vue` 不存在是预期的，Task 8 创建后消除）。

---

## Task 8: Admin 列表页

**Files:**
- Create: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\views\mall\address\index.vue`

**Interfaces:**
- Consumes: `getAddressList` / `setAddressDefault` / `deleteAddress`；`BaseSearchForm` / `BaseTable` / `BaseStatusTag`
- Produces: 完整列表页

> 模板参考 `views/mall/member/index.vue`（6 段式 import 顺序：vue → vue-router → lib → element-plus → project components → stores/types/i18n → api → 本地）。

- [ ] **Step 8.1: 创建 `views/mall/address/index.vue` 完整代码**

```vue
<template>
  <div class="address-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    >
      <template #toolbar>
        <el-button
          v-permission="'mall:address:add'"
          type="primary"
          @click="openCreate"
        >
          {{ t('mall.address.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="address-page__table">
      <BaseTable
        v-loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="memberNickname"
          :label="t('mall.address.memberNickname')"
          min-width="100"
          show-overflow-tooltip
        />
        <el-table-column
          prop="memberPhone"
          :label="t('mall.address.memberPhone')"
          width="130"
        />
        <el-table-column
          prop="memberLevelName"
          :label="t('mall.address.memberLevelName')"
          width="100"
        />
        <el-table-column
          prop="name"
          :label="t('mall.address.recipientName')"
          width="100"
        />
        <el-table-column
          prop="phone"
          :label="t('mall.address.recipientPhone')"
          width="130"
        />
        <el-table-column
          prop="fullAddress"
          :label="t('mall.address.fullAddress')"
          min-width="240"
          show-overflow-tooltip
        />
        <el-table-column
          :label="t('mall.address.isDefault')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :status="row.isDefault ? 'enabled' : 'disabled'"
              :text="row.isDefault ? t('mall.address.isDefaultYes') : t('mall.address.isDefaultNo')"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="updatedAt"
          :label="t('mall.address.updatedAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.operation')"
          width="260"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-permission="'mall:address:edit'"
              link
              type="primary"
              @click="openEdit(row)"
            >
              {{ t('mall.address.edit') }}
            </el-button>
            <el-button
              v-permission="'mall:address:delete'"
              link
              type="danger"
              @click="confirmDelete(row)"
            >
              {{ t('mall.address.delete') }}
            </el-button>
            <el-button
              v-if="!row.isDefault"
              v-permission="'mall:address:set-default'"
              link
              type="success"
              @click="handleSetDefault(row)"
            >
              {{ t('mall.address.setDefault') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <AddressFormDialog
      v-model:visible="dialogVisible"
      :address="editingAddress"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useI18n } from 'vue-i18n'

import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import AddressFormDialog from './components/AddressFormDialog.vue'

import {
  getAddressList,
  setAddressDefault,
  deleteAddress,
  type AddressListItem,
  type AddressQuery,
} from '@/api/mall/address'
import { ADDRESS_DEFAULT_OPTIONS } from '@/types/mall/address.types'

const { t } = useI18n()

const loading = ref(false)
const list = ref<AddressListItem[]>([])
const total = ref(0)

const query = reactive<AddressQuery>({
  pageIndex: 1,
  pageSize: 10,
})

const searchModel = reactive({
  memberNickname: '',
  recipientName: '',
  memberPhone: '',
  isDefault: undefined as boolean | undefined,
})

const searchFields = [
  { key: 'memberNickname', label: t('mall.address.searchMemberNickname'), type: 'input' as const },
  { key: 'recipientName', label: t('mall.address.searchRecipient'), type: 'input' as const },
  { key: 'memberPhone', label: t('mall.address.searchPhone'), type: 'input' as const },
  { key: 'isDefault', label: t('mall.address.searchIsDefault'), type: 'select' as const, options: ADDRESS_DEFAULT_OPTIONS },
]

const dialogVisible = ref(false)
const editingAddress = ref<AddressListItem | null>(null)

async function loadData(): Promise<void> {
  loading.value = true
  try {
    const params: AddressQuery = {
      pageIndex: query.pageIndex,
      pageSize: query.pageSize,
      memberNickname: searchModel.memberNickname || undefined,
      recipientName: searchModel.recipientName || undefined,
      memberPhone: searchModel.memberPhone || undefined,
      isDefault: searchModel.isDefault,
    }
    const res = await getAddressList(params)
    list.value = res.list
    total.value = res.total
  } finally {
    loading.value = false
  }
}

function handleSearch(): void {
  query.pageIndex = 1
  void loadData()
}

function handleReset(): void {
  searchModel.memberNickname = ''
  searchModel.recipientName = ''
  searchModel.memberPhone = ''
  searchModel.isDefault = undefined
  query.pageIndex = 1
  void loadData()
}

function handlePageChange(page: { pageIndex: number; pageSize: number }): void {
  query.pageIndex = page.pageIndex
  query.pageSize = page.pageSize
  void loadData()
}

function openCreate(): void {
  editingAddress.value = null
  dialogVisible.value = true
}

function openEdit(row: AddressListItem): void {
  editingAddress.value = row
  dialogVisible.value = true
}

async function handleSetDefault(row: AddressListItem): Promise<void> {
  await setAddressDefault(row.id)
  ElMessage.success(t('mall.address.message.setDefaultSuccess'))
  await loadData()
}

async function confirmDelete(row: AddressListItem): Promise<void> {
  try {
    await ElMessageBox.confirm(t('mall.address.deleteConfirm'), t('common.tip'), {
      type: 'warning',
    })
  } catch {
    return
  }
  await deleteAddress(row.id)
  ElMessage.success(t('mall.address.message.deleteSuccess'))
  await loadData()
}

onMounted(() => {
  void loadData()
})
</script>

<style scoped lang="scss">
.address-page {
  &__table {
    margin-top: 16px;
  }
}
</style>
```

- [ ] **Step 8.2: type-check**

```bash
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

预期：剩 `AddressFormDialog` 不存在的错误（Task 9 解决）。

---

## Task 9: Admin 表单弹窗

**Files:**
- Create: `D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\views\mall\address\components\AddressFormDialog.vue`

**Interfaces:**
- Consumes: `AddressFormDialogProps { visible: boolean; address: AddressListItem | null }`；`createAddress` / `updateAddress` / `getMemberOptions`（已有 `/api/admin/mall/member/level/options` 类似端点；优先用 `getMemberOptions` 如不存在则用 `getMemberList` 取全量作 fallback）
- Produces: `v-model:visible` 双向绑定；`emit('success')` 提交成功后通知父组件刷新

- [ ] **Step 9.1: 检查/补全 member options 端点**

```bash
grep -n "getMemberOptions\|member/options\|member/level/options" D:\4-MyProject\EasyProduct\EasyProduct.Admin\src\api\mall\member.ts
grep -n "member/level/options" D:\4-MyProject\EasyProduct\mock-server\src\routes\admin\mall-member.ts
```

- 如果 `getMemberOptions` 存在：直接用
- 如果只有 `getMemberList`：在 `api/mall/address.ts` 末尾加一个 `getMemberOptions()`，mock 端补一个 `GET /api/admin/mall/member/options`（返回全量 member 简表 `{id, nickname, phone, levelName}`）

- [ ] **Step 9.2: 创建 `AddressFormDialog.vue`**

```vue
<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('mall.address.form.editTitle') : t('mall.address.form.addTitle')"
    width="560"
    :close-on-click-modal="false"
    @update:model-value="(v) => emit('update:visible', v)"
    @closed="handleClosed"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
      label-position="right"
    >
      <el-form-item
        :label="t('mall.address.form.memberPlaceholder')"
        prop="memberId"
      >
        <el-select
          v-model="form.memberId"
          :disabled="isEdit"
          filterable
          :placeholder="t('mall.address.form.memberPlaceholder')"
          style="width: 100%"
        >
          <el-option
            v-for="m in memberOptions"
            :key="m.id"
            :label="`${m.nickname} (${m.phone})`"
            :value="m.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item
        :label="t('mall.address.recipientName')"
        prop="name"
      >
        <el-input
          v-model="form.name"
          :placeholder="t('mall.address.form.namePlaceholder')"
          maxlength="32"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.address.recipientPhone')"
        prop="phone"
      >
        <el-input
          v-model="form.phone"
          :placeholder="t('mall.address.form.phonePlaceholder')"
          maxlength="11"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.address.province')"
        prop="province"
      >
        <el-input
          v-model="form.province"
          :placeholder="t('mall.address.form.provincePlaceholder')"
          maxlength="32"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.address.city')"
        prop="city"
      >
        <el-input
          v-model="form.city"
          :placeholder="t('mall.address.form.cityPlaceholder')"
          maxlength="32"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.address.district')"
        prop="district"
      >
        <el-input
          v-model="form.district"
          :placeholder="t('mall.address.form.districtPlaceholder')"
          maxlength="32"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.address.detail')"
        prop="detail"
      >
        <el-input
          v-model="form.detail"
          :placeholder="t('mall.address.form.detailPlaceholder')"
          maxlength="128"
        />
      </el-form-item>
      <el-form-item
        :label="t('mall.address.isDefault')"
        prop="isDefault"
      >
        <el-switch v-model="form.isDefault" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="emit('update:visible', false)">{{ t('common.cancel') }}</el-button>
      <el-button
        type="primary"
        :loading="submitting"
        @click="handleSubmit"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'

import { createAddress, updateAddress } from '@/api/mall/address'
import { getMemberOptions, type MemberOption } from '@/api/mall/member'
import type { AddressListItem, AddressParams } from '@/types/mall/address.types'

const { t } = useI18n()

const props = defineProps<{
  visible: boolean
  address: AddressListItem | null
}>()

const emit = defineEmits<{
  'update:visible': [v: boolean]
  success: []
}>()

const isEdit = computed(() => props.address !== null)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const memberOptions = ref<MemberOption[]>([])

const form = reactive<AddressParams>({
  memberId: '',
  name: '',
  phone: '',
  province: '',
  city: '',
  district: '',
  detail: '',
  isDefault: false,
})

const rules: FormRules = {
  memberId: [{ required: true, message: t('mall.address.form.memberRequired'), trigger: 'change' }],
  name: [{ required: true, message: t('mall.address.form.nameRequired'), trigger: 'blur' }],
  phone: [
    { required: true, message: t('mall.address.form.phoneRequired'), trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: t('mall.address.form.phoneInvalid'), trigger: 'blur' },
  ],
  province: [{ required: true, message: t('mall.address.form.provinceRequired'), trigger: 'blur' }],
  city: [{ required: true, message: t('mall.address.form.cityRequired'), trigger: 'blur' }],
  district: [{ required: true, message: t('mall.address.form.districtRequired'), trigger: 'blur' }],
  detail: [{ required: true, message: t('mall.address.form.detailRequired'), trigger: 'blur' }],
}

function resetForm(): void {
  form.memberId = ''
  form.name = ''
  form.phone = ''
  form.province = ''
  form.city = ''
  form.district = ''
  form.detail = ''
  form.isDefault = false
  formRef.value?.clearValidate()
}

function fillFromAddress(a: AddressListItem): void {
  form.memberId = a.memberId
  form.name = a.name
  form.phone = a.phone
  form.province = a.province
  form.city = a.city
  form.district = a.district
  form.detail = a.detail
  form.isDefault = a.isDefault
}

watch(
  () => [props.visible, props.address],
  ([vis]) => {
    if (!vis) return
    if (props.address) fillFromAddress(props.address)
    else resetForm()
  },
)

async function loadMemberOptions(): Promise<void> {
  try {
    memberOptions.value = await getMemberOptions()
  } catch {
    memberOptions.value = []
  }
}

async function handleSubmit(): Promise<void> {
  if (!formRef.value) return
  try {
    await formRef.value.validate()
  } catch {
    return
  }
  submitting.value = true
  try {
    if (isEdit.value && props.address) {
      await updateAddress(props.address.id, { ...form })
      ElMessage.success(t('mall.address.message.updateSuccess'))
    } else {
      await createAddress({ ...form })
      ElMessage.success(t('mall.address.message.createSuccess'))
    }
    emit('update:visible', false)
    emit('success')
  } finally {
    submitting.value = false
  }
}

function handleClosed(): void {
  resetForm()
}

onMounted(() => {
  void loadMemberOptions()
})
</script>
```

- [ ] **Step 9.3: 在 `api/mall/member.ts` 补 `getMemberOptions`（若需要）**

如端点不存在：

```ts
/** 会员下拉（搜索框/地址表单用） */
export const getMemberOptions = () =>
  get<Array<{ id: string; nickname: string; phone: string; levelName: string }>>(
    '/api/admin/mall/member/options',
  )
```

并在 mock 端 `routes/admin/mall-member.ts` 末尾追加：

```ts
adminMallMemberRouter.get('/mall/member/options', (_req, res) => {
  res.json(ok(MEMBERS.map(m => ({ id: m.id, nickname: m.nickname, phone: m.phone, levelName: m.levelName }))))
})
```

- [ ] **Step 9.4: type-check**

```bash
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

预期：0 错误。

---

## Task 10: Admin 提交前自检

**Files:** 无新文件；仅运行命令验证 Task 6-9 的产出。

> 顺序很重要：先 type-check（最快发现类型错误），再 lint（修风格），再 i18n 检查（修硬编码），最后启动 dev 人工目检。

- [ ] **Step 10.1: type-check（必须 0 错误）**

```bash
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm type-check
```

预期：`Found 0 errors.`。若报错，常见原因：
- `AddressFormDialog.vue` 用到未导入类型 → 补 import
- `address.types.ts` 的 `AddressQuery.startTime/endTime` 类型不匹配 `useSearch` 推导 → 调整 `useSearch` 泛型或字段类型
- `i18n` key 拼写 → 修 key 或补 i18n 资源

- [ ] **Step 10.2: ESLint（必须 0 错误，允许 warning）**

```bash
pnpm lint
```

预期：仅可能存在上游已存在的 warning；本次新增文件不得引入 error。常见修法：
- 多余空行 → `--fix`
- 单引号/分号 → `--fix`
- `any` 类型 → 收紧类型
- `console.log` → 删除或用 `console.warn`（仅在 catch 分支）

- [ ] **Step 10.3: i18n 硬编码检查（必须 0 命中）**

```bash
pnpm check:i18n
```

预期：`0 hardcoded Chinese strings found.`。命中项处理：
- 列表页表头 → 抽 `t('mall.address.column.xxx')`
- 弹窗 label/placeholder → 抽 `t('mall.address.form.xxx')`
- 操作按钮文案 → 抽 `t('common.action.xxx')` 或 `t('mall.address.action.xxx')`
- 错误提示 → 抽 `t('mall.address.message.xxx')`

- [ ] **Step 10.4: 启动 Admin dev 服务器目检**

```bash
# 终端 A：mock-server
cd D:\4-MyProject\EasyProduct\mock-server
pnpm dev

# 终端 B：Admin
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin
pnpm dev
```

打开浏览器（默认 http://localhost:5173），用 sales 角色登录（admin/123456 或 sales/123456）：

- [ ] **Step 10.5: 菜单可见性目检**

  - [ ] 左侧菜单应出现 "收货地址管理" / "Address Management" 顶级入口（在"会员管理"附近）
  - [ ] 切到中文/英文，菜单文案同步切换
  - [ ] 销售角色（sales）登录也能看到该菜单（已配 `mall:address:list`）
  - [ ] 不存在的角色登录不应看到（默认无权限角色被拦截）

- [ ] **Step 10.6: 列表页功能目检**

  - [ ] 进入页面，默认查询返回分页数据（每页 10 条）
  - [ ] 关键词搜索：输入 `张三` / `13800138000` → 命中行过滤正确
  - [ ] 会员下拉筛选：选择一个会员 → 只显示该会员地址
  - [ ] 是否默认筛选：选"是" → 只显示默认地址行
  - [ ] 重置按钮：清空所有查询条件并重新加载
  - [ ] 分页：翻页/改 pageSize → 重新查询
  - [ ] 行操作列：每行有"编辑 / 设为默认 / 删除"三个按钮（默认地址"设为默认"按钮禁用或隐藏）

- [ ] **Step 10.7: 新增地址弹窗目检**

  - [ ] 点"新增地址" → 弹窗打开，标题"新增收货地址"
  - [ ] 必填校验：空会员 / 空姓名 / 错手机号 / 空省市区 → 提交按钮置灰或 toast 报错
  - [ ] 手机号正则：`^1[3-9]\d{9}$` → 错格式拒绝
  - [ ] 提交成功 → 弹窗关闭 + 列表自动刷新
  - [ ] 切到 en-US：弹窗标题、表单 label、按钮文案全部翻译

- [ ] **Step 10.8: 编辑地址弹窗目检**

  - [ ] 点"编辑" → 弹窗打开并预填该行数据
  - [ ] 改某个字段提交 → 列表对应行更新
  - [ ] 把默认地址改为非默认：列表中其它默认地址保持不变（即业务规则只清同 memberId 其它默认）

- [ ] **Step 10.9: 设为默认地址目检**

  - [ ] 点"设为默认" → toast "设置成功" → 该行 `isDefault=true` 高亮 + 其它行默认状态清掉
  - [ ] 仅影响同会员行（不会影响其它会员）

- [ ] **Step 10.10: 删除地址目检**

  - [ ] 点"删除" → 二次确认 `ElMessageBox.confirm`
  - [ ] 取消 → 不删
  - [ ] 确认 → 行消失
  - [ ] 删除的是默认地址：同会员其它地址中最新一条 createdAt 自动升为默认（业务规则）

- [ ] **Step 10.11: 跨端共享验证（关键）**

  - [ ] 浏览器开两个 tab：Admin 列表页 + 小程序调试器（或另一个 Admin 列表页查询该会员）
  - [ ] Admin 改某条地址 → 切到 app 端查询该会员 → 数据已同步
  - [ ] 反之亦然

- [ ] **Step 10.12: 错误码路径**

  - [ ] 列表查询传 `pageSize=999` → 应自动 cap 到 100 或返回 400
  - [ ] 删除不存在的 id → 应返回 `{code: 404, message: "地址不存在"}`
  - [ ] 设为默认不存在的 id → 同上 404

---

## Task 11: Admin commit + push

> ⚠️ `.git/` 目录在 Codex 沙盒中是只读的（用户对 git 索引无 WriteData）。所有 git 命令需要在 escalated 模式下执行；本任务步骤里的 git 命令请用 `sandbox_permissions: "require_escalated"` 调用 exec_command。
>
> 提交分 3 个 commit：(1) mock 端（Task 1+2+4 已合并过 Task 5 提交）、(2) Admin 基础设施、(3) Admin 页面。

- [ ] **Step 11.1: commit 2 —— Admin 基础设施（types + api + i18n + barrel）**

文件列表：
- `EasyProduct.Admin/src/types/mall/address.types.ts`
- `EasyProduct.Admin/src/types/mall.ts`（追加 export *）
- `EasyProduct.Admin/src/api/mall/address.ts`
- `EasyProduct.Admin/src/i18n/zh-CN/menu.json`
- `EasyProduct.Admin/src/i18n/en-US/menu.json`
- `EasyProduct.Admin/src/i18n/zh-CN/mall.json`
- `EasyProduct.Admin/src/i18n/en-US/mall.json`

```bash
git add \
  EasyProduct.Admin/src/types/mall/address.types.ts \
  EasyProduct.Admin/src/types/mall.ts \
  EasyProduct.Admin/src/api/mall/address.ts \
  EasyProduct.Admin/src/i18n/zh-CN/menu.json \
  EasyProduct.Admin/src/i18n/en-US/menu.json \
  EasyProduct.Admin/src/i18n/zh-CN/mall.json \
  EasyProduct.Admin/src/i18n/en-US/mall.json
git commit -m "feat(admin): 收货地址管理 - 类型/API/i18n 基础设施"
```

- [ ] **Step 11.2: commit 3 —— Admin 路由 + 页面（路由 + 列表页 + 弹窗）**

文件列表：
- `EasyProduct.Admin/src/router/modules/mall-address.ts`
- `EasyProduct.Admin/src/router/index.ts`（若修改）
- `EasyProduct.Admin/src/views/mall/address/index.vue`
- `EasyProduct.Admin/src/views/mall/address/components/AddressFormDialog.vue`

```bash
git add \
  EasyProduct.Admin/src/router/modules/mall-address.ts \
  EasyProduct.Admin/src/router/index.ts \
  EasyProduct.Admin/src/views/mall/address/index.vue \
  EasyProduct.Admin/src/views/mall/address/components/AddressFormDialog.vue
git commit -m "feat(admin): 收货地址管理页面 - 列表 + 增改弹窗"
```

- [ ] **Step 11.3: commit 4 —— 会员下拉选项补全（如 Step 9.3 触发了的话）**

文件列表：
- `EasyProduct.Admin/src/api/mall/member.ts`
- `mock-server/src/routes/admin/mall-member.ts`

```bash
git add \
  EasyProduct.Admin/src/api/mall/member.ts \
  mock-server/src/routes/admin/mall-member.ts
git commit -m "feat(mock): 补 /api/admin/mall/member/options 下拉接口"
```

- [ ] **Step 11.4: fetch + push**

```bash
git fetch origin main
git push origin main
```

预期：3 个 commit 全部推送成功，无冲突。

- [ ] **Step 11.5: 验证远端**

```bash
git log origin/main --oneline -n 5
```

预期：看到以下 4 条（Task 5 已推 1 条 + 本任务推 3 条）：
1. `feat(admin): 收货地址管理页面 - 列表 + 增改弹窗`
2. `feat(admin): 收货地址管理 - 类型/API/i18n 基础设施`
3. `feat(mock): 补 /api/admin/mall/member/options 下拉接口`（若存在）
4. `feat(mock): 新增会员收货地址管理 - 菜单权限种子 + admin 路由`

---

## Task 12: 文档收口

**Files:**
- `docs/mock-guidelines.md`
- `docs/superpowers/specs/2026-09-04-miniapp-address-admin-design.md`
- `docs/superpowers/plans/2026-09-04-miniapp-address-admin.md`（本文件加 ✓ 完成戳）

- [ ] **Step 12.1: `docs/mock-guidelines.md` 第 10 节 MOCK_STATUS 登记**

打开文件，找到第 10 节（MOCK_STATUS 表格），增一行：

```markdown
| admin | 会员收货地址（管理端） | completed | P3-ext |
```

完整行为：当前 admin 路由已上线并完成端到端验证；后端实现标记为 P3-ext（即 EasyProduct.WebApi 目录创建后的下一阶段）。

- [ ] **Step 12.2: spec 文件第 8 节验收清单全部勾选**

打开 `docs/superpowers/specs/2026-09-04-miniapp-address-admin-design.md`，把第 8 节"验收清单"全部 `- [ ]` 改为 `- [x]`。同时在第 1 节末尾追加一行完成时间：

```markdown
> **实施完成日期：** 2026-09-04
> **实施计划：** `docs/superpowers/plans/2026-09-04-miniapp-address-admin.md`
```

- [ ] **Step 12.3: 在本 plan 文件顶部加完成戳**

打开本文件，把头部 `**Status:**` 行（如有）或在 **Tech Stack** 段之后追加：

```markdown
**Status:** ✅ Completed on 2026-09-04
```

- [ ] **Step 12.4: 三文档统一 commit + push**

```bash
git add \
  docs/mock-guidelines.md \
  docs/superpowers/specs/2026-09-04-miniapp-address-admin-design.md \
  docs/superpowers/plans/2026-09-04-miniapp-address-admin.md
git commit -m "docs: 落账 admin 端收货地址管理 spec + plan + MOCK_STATUS"
git push origin main
```

- [ ] **Step 12.5: 最终归档**

- [ ] 在 Codex 当前任务标记完成
- [ ] 在 `docs/superpowers/specs/INDEX.md`（若存在）登记本次 spec 路径与一句话摘要
- [ ] 在团队周报 / Slack 频道贴一句"会员收货地址管理（admin 端）已上线，admin 客服可直接查改会员地址"

---

## 自审：spec 覆盖检查

| spec 节 | 对应 task | 覆盖情况 |
|---|---|---|
| §1 背景与目标 | Task 1 头注释 + Goal 段 | ✅ |
| §2 关键决策 | Task 2 头注释 + Task 4 菜单权限 | ✅ |
| §3 路由与菜单 | Task 4（菜单）+ Task 7（路由） | ✅ |
| §4 数据模型 | Task 1（mock 端）+ Task 6（前端类型） | ✅ |
| §5 API 设计 | Task 2（6 端点）+ Task 3（curl 自测） | ✅ |
| §6 UI/交互 | Task 8（列表页）+ Task 9（弹窗） | ✅ |
| §7 文件清单 | Task 1-12 全部文件清单 | ✅ |
| §8 验收清单 | Task 3（mock 端）+ Task 10（Admin 端） | ✅ |
| §9 风险与权衡 | 留为 P3-ext 后端任务（Task 1 注释里点明） | ✅ |
| §10 后续衔接 | Task 12 文档收口 | ✅ |

> 所有 spec 要求都有对应任务，**无遗漏**。

## 自审：规范约束检查

| 约束（来自 docs/*） | 验证 task |
|---|---|
| 统一响应信封 `{code, message, data, timestamp}` | Task 2 `ok()`/`fail()` 包装 + Task 3 curl 校验 |
| `/api/admin/**` 路径 + 路由顺序 | Task 2 `/:id/default` 在 `/:id` 前声明 |
| GUID 主键 + camelCase + 分页 pageIndex/pageSize | Task 1 type 定义 + Task 2 分页参数 |
| 禁止 enum / `as const` 常量 | Task 6 类型定义 |
| `vue-tsc --noEmit` 0 错误 | Task 10 Step 10.1 |
| `pnpm lint` 0 错误 | Task 10 Step 10.2 |
| `pnpm check:i18n` 0 硬编码 | Task 10 Step 10.3 |
| i18n key 命名空间 `mall.address.*` | Task 6 Step 6.5 |
| 菜单入口顶级 `/mall/address/list` | Task 4 Step 4.1 + Task 7 Step 7.1 |
| Conventional Commits scope=admin/mock | Task 5 + Task 11 |
| BaseSearchForm/BaseTable/BaseFormDialog 强制 | Task 8 + Task 9 |
| 后端方法中文注释 | N/A（本期无后端） |
| mock 与后端规范第 5 节逐字一致 | Task 2 严格按 `docs/backend-guidelines.md` 第 5 节 |

> 所有规范约束都有对应验证步骤。

## 自审：风险与回滚

| 风险 | 缓解 |
|---|---|
| Admin 列表查询 N+1（一次查地址 + 一次查会员 join） | Task 1 mock 端用 `Map<memberId, Member>` 缓存成员元信息；P3-ext 后端用 SqlSugar `Includes` 关联查询 |
| 行政区划文本输入易拼写错误 | spec §9 已标记为 P3-ext 优化项（接入国家统计局区划字典） |
| admin 误删关键地址 | Task 9 Step 9.5 二次确认 + Task 10 Step 10.10 删除默认地址自动提升 |
| 默认地址并发竞争 | P3-ext 后端用事务 + 行锁；本期 mock 端是单线程无竞态 |
| 跨端共享 `ADDRESSES` 全局可变状态被覆盖 | mock-server 是单进程内存，hot reload 会丢数据；已在 Task 1 注释里声明"非持久化" |

## 回滚策略

每个 commit 都是独立可回滚点：
- commit 4（页面）回滚 → 仅前端消失菜单入口
- commit 3（基础设施）回滚 → 前端类型/api import 报错 → 一起回滚 commit 4
- commit 2（mock 路由）回滚 → admin 端请求 404 → 一起回滚 commit 3+4
- commit 1（菜单权限）回滚 → 菜单消失但路由可手输访问 → 安全可控

任意一个 commit 可独立 `git revert <sha>` 回滚，无级联破坏。

---

## Execution Handoff

**Recommended:** Subagent-Driven Development（每个 task 派一个 fresh subagent + 两阶段 review）

参见 `~/.codex/plugins/cache/claude-plugins-official/superpowers/6.3.0/skills/subagent-driven-development/SKILL.md` 工作流。

### 启动指令（推荐）

> 把以下 prompt 复制到新 Codex 任务里执行：

````markdown
使用 superpowers:subagent-driven-development skill 执行计划文件：
`D:\4-MyProject\EasyProduct\docs\superpowers\plans\2026-09-04-miniapp-address-admin.md`

执行模式：
- 每个 task 派一个独立 subagent（model 继承父任务）
- 每个 task 完成后做两阶段 review：(a) 代码规范审查 (b) spec 覆盖审查
- Task 1-5（mock 端）一批，Task 6-12（Admin 端）一批
- 任何 type-check / lint / check:i18n 报错必须修完才能进入下一 task

输出要求：
- 每个 task 完成后输出"已完成 task N / 12" + 该 task 实际新增/修改文件列表
- 全部 task 完成后输出最终 commit hash 列表 + push 状态
````

### 启动指令（备选：inline 执行）

若用户希望当前会话内直接执行，参见 `~/.codex/plugins/cache/claude-plugins-official/superpowers/6.3.0/skills/executing-plans/SKILL.md`，按 task 顺序串行执行，每个 task 后做 checkpoint review。

---

**Plan End.**
