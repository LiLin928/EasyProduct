# Admin 端会员收货地址管理（mall/address）

> **日期：** 2026-09-04
> **状态：** 待评审
> **阶段：** F2-? Admin Mall 模块补全
> **上游依据：**
> - `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`
> - `docs/backend-guidelines.md` 第 5、6、7、9 节
> - `docs/frontend-guidelines.md` 第 2 节（Admin 端）
> - `docs/mock-guidelines.md` 第 3-7 节
> - 已实现的小程序端契约：`EasyProduct.MiniApp/types/address.types.ts`、`EasyProduct.MiniApp/api/address.ts`
> - 已实现的 mock：`mock-server/src/data/address.ts`、`mock-server/src/routes/app/address.ts`（commit `8badc30`）

---

## 1. 背景与目标

### 1.1 背景

小程序端 `EasyProduct.MiniApp` 已通过 mock 完成会员收货地址模块（`/api/app/addresses`，6 个端点，commit `8badc30` 已落账并 push origin main），但 Admin 后台尚无对应的"会员收货地址管理"页面。

当前问题：
- 商城客服/运营无法在后台查看某会员有哪些收货地址
- 异常订单处理时无法由客服代为修改地址（用户电话联系不上时）
- 现有 `mall/member` 列表只展示会员基本资料，地址数据缺位
- Admin `views/mall/` 目录下没有 `address/` 子目录，`api/mall/` 也没有 `address.ts`

### 1.2 目标

- 新增 Admin "收货地址管理"页面，作为 mall 顶级独立菜单（与会员/等级/积分/优惠券/订单/支付同级）
- 列表展示全量会员地址，**冗余显示**所属会员的昵称/手机号/等级（一行直观看出"谁的地址"）
- 支持按会员昵称、收货人姓名、手机号、是否默认等筛选
- 支持增/删/改/设默认 4 类操作，**复用** `/api/app/addresses` 已确立的业务规则
- 完整中文 i18n（zh-CN + en-US 同步）
- mock-server admin 端新增对应路由，**与小程序端共用底层数据**（保证 admin 改了之后小程序端立即可见）

### 1.3 范围

**包含**：
- Admin 端新增 1 个列表页（`/mall/address/list`） + 1 个表单弹窗（新增/编辑复用）
- 6 个 admin 端 mock 路由（与小程序端业务规则完全一致）
- 1 个 `types/mall/address.types.ts`
- 1 个 `api/mall/address.ts`
- 菜单注册 + 权限标识 + 角色权限种子
- 中文 i18n keys（zh-CN + en-US）

**不在本批次范围**：
- 后端 WebApi 实体/Controller/Service（`EasyProduct.WebApi` 目录当前不存在，属于 P3-ext 后端任务，本 spec 仅预留契约）
- 地址批量导入/导出（运营需求未明确）
- 地址变更审计日志（独立模块需求，本期只写入 `updatedAt`）
- 行政区划级联选择器（先用三级文本输入框，预留接口）

---

## 2. 关键决策

| 维度 | 决策 | 理由 |
|---|---|---|
| 入口位置 | mall 顶级独立菜单 `/mall/address/list` | 与会员/等级/积分同级，运营/客服最高频入口；不走 member 详情内嵌 |
| 数据范围 | 地址 + 会员冗余信息（昵称/手机号/等级） | admin 客服场景需要"一眼看出谁的地址"，避免重复跳转 |
| 编辑权限 | 全部可读可改可设默认可删除 | 异常订单客服介入必须能改；与小程序端业务规则完全一致 |
| 业务规则来源 | **复用** `/api/app/addresses` 已实现规则 | 单一事实源：默认地址唯一、删除默认时自动提升最新一条、手机号正则、必填校验 |
| mock 数据共享 | admin 与 app **共用** `address.ts` 的运行时 Map | 避免双写；admin 改了小程序端下次请求立即看到（与 cart.ts 同模式） |
| 后端契约 | 本批次只交付 mock 端，WebApi 实体待建 | 当前 WebApi 目录不存在，先把 mock 端和前端跑通 |
| 行政区划输入 | 三级文本输入框（省/市/区） | 当前无行政区划字典，先 MVP；接口预留 `regionCode` 字段位 |
| 是否支持新增 | 支持 | admin 客服代填/录入新地址场景存在 |
| 列表性能 | 后端分页 `pageIndex/pageSize` + 前端 BaseTable | 与现有 mall/member 完全一致；地址全量约 100 万级，命中分页足够 |
| 冗余字段一致性 | mock 端 JOIN（Map 内存关联） | WebApi 阶段做 SQL JOIN 或冗余列；本期 mock 端直接 `memberMap.get(address.memberId)` |
| 删除策略 | 软删除？不，物理删除 | 与小程序端行为一致；运营需要看回收站的需求未提 |
| i18n 路径 | `mall.address.*` 命名空间 | 与 `mall.member.*`、`mall.coupon.*` 风格一致 |
| 路由路径 | `admin/mall/address/list`（注意：和菜单路径 `/mall/address/list` 区分） | mock 路由走 `adminGuard` 分区，路径对齐 `admin/mall/member/list` |
| 端点命名 | 6 个端点与 `/api/app/addresses` 路径完全一致 | 切换到真后端后 0 改动前端调用层 |

---

## 3. 路由与菜单

### 3.1 菜单结构

```
商城业务 (mallRoot)
├── 会员管理 (mall-member)        /mall/member
├── 会员等级 (mall-level)          /mall/level
├── 积分记录 (mall-points)         /mall/points
├── 优惠券管理 (mall-coupon)       /mall/coupon
├── 商城订单 (mall-order)          /mall/order
├── 支付记录 (mall-payment)        /mall/payment
└── 收货地址管理 (mall-address)    /mall/address/list   ← 新增
```

- 父菜单复用现有 `mall`，sort 设为 7（排在最后）
- 菜单 path：`/mall/address/list`
- 菜单 i18n key：`menu.mallAddress` → "收货地址管理"
- 菜单 icon：`Location`（Element Plus Location 图标）
- 菜单 status：`enabled`，visible：`true`

### 3.2 前端路由

在 `EasyProduct.Admin/src/router/modules/mall.ts`（如不存在则新建）新增：

```ts
{
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
```

### 3.3 权限标识

| 权限码 | 含义 | 适用范围 |
|---|---|---|
| `mall:address:list` | 查看地址列表 | 菜单可见 + 页面进入 |
| `mall:address:query` | 筛选/搜索 | 页面内 |
| `mall:address:detail` | 查看地址详情 | 详情弹窗/查看按钮 |
| `mall:address:add` | 新增地址 | 工具栏"新增"按钮 + 表单提交 |
| `mall:address:edit` | 编辑地址 | 行内"编辑"按钮 + 表单提交 |
| `mall:address:delete` | 删除地址 | 行内"删除"按钮 |
| `mall:address:set-default` | 设为默认地址 | 行内"设为默认"按钮 |

admin 账号 `*` 通配（已存在）；sales 角色追加 `mall:address:list` + `mall:address:edit`（客服场景需要看+改，不能删）；ops 角色不分配。

---

## 4. 数据模型

### 4.1 前端 TypeScript 类型

`EasyProduct.Admin/src/types/mall/address.types.ts`：

```ts
import type { PageQuery } from '../api'

/** 地址基础字段（与小程序端 mock 契约完全一致） */
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

/** admin 列表冗余展示字段（mock JOIN，WebApi 阶段冗余列或 SQL JOIN） */
export interface AddressListItem extends Address {
  memberNickname: string
  memberPhone: string
  memberLevelName: string
}

export interface AddressQuery extends PageQuery {
  memberNickname?: string     // 冗余字段筛选
  memberPhone?: string
  recipientName?: string      // 收货人姓名
  isDefault?: boolean
}

export interface AddressParams {
  memberId: string            // admin 端新增必须指定会员
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
}
```

### 4.2 后端实体（预留，本期不交付）

WebApi 落地时建议 `Mall/Address` 模块：

- 表名 `mall_address`，GUID 主键
- 字段：`id / member_id / name / phone / province / city / district / detail / is_default / created_at / updated_at`
- 索引：`idx_member_id` (member_id)，`uk_member_default` (member_id, is_default) PARTIAL UNIQUE（MySQL 用触发器或代码层保证唯一）
- 不冗余会员字段；列表通过 JOIN `mall_member` 返回 `AddressListItem`

### 4.3 数据库迁移策略

`mall_address` 是新增表，无破坏性变更；落地时与 `mall_member` 在同一 migration 文件中。

---

## 5. API 设计

### 5.1 6 个端点（与 `/api/app/addresses` 路径完全一致，便于切换真后端）

| 方法 | 路径 | 说明 | 权限 |
|---|---|---|---|
| GET | `/api/admin/mall/address/list` | 分页列表，**冗余**会员昵称/手机号/等级 | `mall:address:list` |
| GET | `/api/admin/mall/address/:id` | 地址详情 | `mall:address:detail` |
| POST | `/api/admin/mall/address` | 新增（必须传 memberId） | `mall:address:add` |
| PUT | `/api/admin/mall/address/:id` | 更新全部字段 | `mall:address:edit` |
| PUT | `/api/admin/mall/address/:id/default` | 设为默认（先于 PUT /:id 声明） | `mall:address:set-default` |
| DELETE | `/api/admin/mall/address/:id` | 删除（默认地址删除时自动提升最新一条） | `mall:address:delete` |

> **关键：** `PUT /:id/default` 必须在 `PUT /:id` 之前注册到 router，否则 `/:id` 会吞掉 `/default`（与 app 端同样坑，已踩）。

### 5.2 列表查询参数

```ts
interface AddressQuery {
  pageIndex: number
  pageSize: number
  memberNickname?: string
  memberPhone?: string
  recipientName?: string
  isDefault?: boolean
}
```

返回 `PageData<AddressListItem>`（信封 `{code, message, data: PageData, timestamp}`）。

### 5.3 业务规则（与小程端端 100% 一致）

1. **默认地址唯一**：新增/更新 isDefault=true 时，自动把同 memberId 其它地址 isDefault 改为 false
2. **删除默认地址自动提升**：删除 isDefault=true 的地址时，若同 memberId 还有其它地址，取 createdAt 最新一条提升为默认
3. **手机号正则**：`^1[3-9]\d{9}$`
4. **必填字段**：`memberId / name / phone / province / city / district / detail`
5. **fullAddress 自动拼接**：`{province} {city} {district} {detail}`，服务端填，前端只读
6. **POST 校验顺序**：memberId 存在 → 必填字段 → 手机号正则 → 默认地址切换

### 5.4 admin 与 app 共享内存数据

`mock-server/src/data/address.ts` 已实现：

```ts
/** 运行时地址存储（mock-server 启动后所有 admin/app 端修改都落到这里） */
export const ADDRESSES = new Map<string, Address[]>()  // memberId → Address[]
```

- `routes/app/address.ts` 已读写 `ADDRESSES`
- `routes/admin/mall-address.ts`（本期新增）也读写 `ADDRESSES`
- 重启 mock-server 即重置为种子数据（与 cart.ts 同模式）

---

## 6. UI/交互设计

### 6.1 列表页布局

```
┌─────────────────────────────────────────────────────────────────────┐
│ 搜索: [会员昵称] [收货人] [手机号] [是否默认] [查询] [重置] [+ 新增] │
├─────────────────────────────────────────────────────────────────────┤
│ 会员昵称 │ 会员手机号 │ 等级 │ 收货人 │ 收货人手机 │ 详细地址       │
│          │            │      │        │            │         │ 默认 │ 操作  │
├─────────────────────────────────────────────────────────────────────┤
│ 张三     │ 138****01  │ 金卡 │ 张三   │ 138****01  │ 广东省... │ ✓   │ 改 删 默认│
│ 李四     │ 139****02  │ 银卡 │ 李四   │ 139****02  │ 上海市... │ ✓   │ 改 删 默认│
└─────────────────────────────────────────────────────────────────────┘
                                                    共 N 条  < 1 2 3 >
```

- 搜索栏使用 `BaseSearchForm`（强制，frontend-guidelines 2.5）
- 表格使用 `BaseTable`，列定义见 6.3
- 默认地址列用 `BaseStatusTag` 渲染（type='success' 显示"是"）
- 行内操作：编辑、删除、设为默认（v-permission 指令控制）

### 6.2 表单弹窗

新增/编辑共用 `BaseFormDialog`（强制，frontend-guidelines 2.5）：

```
新增地址                                          [×]
─────────────────────────────────────────────
会员:        [下拉选择，搜索会员昵称/手机号 ★]  ← 必填，编辑时禁用
收货人:      [输入框                       ]    ← 必填
手机号:      [输入框                       ]    ← 必填，11位手机号正则
省:          [输入框                       ]    ← 必填
市:          [输入框                       ]    ← 必填
区:          [输入框                       ]    ← 必填
详细地址:    [输入框                       ]    ← 必填
设为默认:    [☐]                              ← 同一会员已有默认时校验
─────────────────────────────────────────────
                                       [取消] [确定]
```

会员下拉：调用 `GET /api/admin/mall/member/options`（已存在）回填 `memberId`。

### 6.3 字段宽度建议

| 字段 | 宽度 | 说明 |
|---|---|---|
| 会员昵称 | min-width 100 | 冗余字段 |
| 会员手机号 | width 130 | 冗余字段 |
| 等级 | width 100 | 冗余字段 |
| 收货人姓名 | width 100 | |
| 收货人手机号 | width 130 | |
| 省/市/区 | width 100 each | |
| 详细地址 | min-width 200 show-overflow-tooltip | fullAddress |
| 是否默认 | width 80 center | BaseStatusTag |
| 创建时间 | width 160 | |
| 更新时间 | width 160 | |
| 操作 | width 220 fixed | 3 个按钮 |

### 6.4 删除确认

`el-popconfirm` 内联触发，文案 `mall.address.deleteConfirm`，调用 `/api/admin/mall/address/:id` DELETE。

---

## 7. 文件清单

### 7.1 新建文件

| 路径 | 用途 |
|---|---|
| `EasyProduct.Admin/src/views/mall/address/index.vue` | 列表页 |
| `EasyProduct.Admin/src/views/mall/address/components/AddressFormDialog.vue` | 新增/编辑弹窗 |
| `EasyProduct.Admin/src/api/mall/address.ts` | API 层 |
| `EasyProduct.Admin/src/types/mall/address.types.ts` | TypeScript 类型 |
| `EasyProduct.Admin/src/router/modules/mall-address.ts` | 路由模块（如现有 mall 路由不拆分则并入） |
| `mock-server/src/routes/admin/mall-address.ts` | admin 端 6 端点 |
| `docs/superpowers/specs/2026-09-04-miniapp-address-admin-design.md` | 本 spec |

### 7.2 修改文件

| 路径 | 改动 |
|---|---|
| `mock-server/src/data/address.ts` | 把 `seedAddressesByMember` 升级为运行时 `ADDRESSES: Map<memberId, Address[]>`（已写） |
| `mock-server/src/server.ts` | 注册 `adminMallAddressRouter` 到 `/api/admin`（app 端已注册，不动） |
| `mock-server/src/data/basic.ts` | `MENU_TREE` 增 `mall-address` 节点；`USER_PERMISSIONS.sales` 追加 `mall:address:list`、`mall:address:edit` |
| `EasyProduct.Admin/src/api/basic/menu.ts` | 菜单类型 `MenuItem` 已包含 `titleKey/icon/path`，无需改 |
| `EasyProduct.Admin/src/i18n/zh-CN/menu.json` | 新增 `mallAddress: "收货地址管理"` |
| `EasyProduct.Admin/src/i18n/en-US/menu.json` | 新增 `mallAddress: "Address Management"` |
| `EasyProduct.Admin/src/i18n/zh-CN/mall.json` | 新增 `address.*` 命名空间（见 7.3） |
| `EasyProduct.Admin/src/i18n/en-US/mall.json` | 同上 |
| `EasyProduct.Admin/src/router/index.ts` 或 `router/modules/mall.ts` | 挂载新路由 |
| `EasyProduct.Admin/src/types/mall.ts` | 在末尾追加 export `* from './mall/address.types'` |
| `docs/mock-guidelines.md` | 第 10 节 MOCK_STATUS 增"会员收货地址（Admin 端）" |
| `mock-server/README.md`（如有） | MOCK_STATUS 同步 |

### 7.3 i18n 键（zh-CN）

```json
{
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
}
```

en-US 同步翻译，命名一致。

---

## 8. 验收清单

### 8.1 功能验收

- [ ] 菜单可见：登录 admin 账号，侧栏"商城业务"下出现"收货地址管理"入口
- [ ] 列表展示：进入页面，默认看到 5 条种子地址（含会员昵称/手机号/等级冗余字段）
- [ ] 搜索：按会员昵称/收货人/手机号/是否默认 任一条件过滤
- [ ] 分页：超过 pageSize 时显示分页器
- [ ] 新增：弹窗选会员 → 填地址 → 提交，列表立即多一条
- [ ] 新增设为默认：勾选后原默认地址的"是否默认"自动变 false
- [ ] 编辑：修改后 `updatedAt` 更新
- [ ] 编辑设为默认：同上切换
- [ ] 设默认：单独点"设为默认"按钮，原默认自动取消
- [ ] 删除非默认：列表少一条，其它不受影响
- [ ] 删除默认：同会员下一条 createdAt 最新的自动提升为默认
- [ ] 校验：必填/手机号正则 都返回 400 + 中文 message
- [ ] 跨端数据共享：admin 在前端删除后，小程序端 `GET /api/app/addresses` 立即少一条
- [ ] 权限控制：sales 账号看不到"删除"按钮（`mall:address:delete` 未授权）

### 8.2 路由顺序正确性

- [ ] `PUT /api/admin/mall/address/:id/default` 不会被 `PUT /:id` 吞掉（与 app 端同样验证）

### 8.3 提交前自检

- [ ] `pnpm type-check` 0 错误
- [ ] `pnpm lint` 0 错误
- [ ] `pnpm check:i18n` 0 硬编码中文
- [ ] mock-server `tsc --noEmit` 0 新错误
- [ ] 端到端 curl 6 端点 + 4 边界场景全过

### 8.4 工程门禁

- [ ] 提交信息遵循 Conventional Commits：`feat(admin): 新增收货地址管理页面` + `feat(mock): 新增 admin 端收货地址路由` + `feat(mock): 菜单/权限种子补 mall-address`
- [ ] 不动 `EasyProduct.WebApi`（目录还不存在）
- [ ] 不动 `EasyProduct.MiniApp`（已落账，与 admin 解耦）

---

## 9. 风险与未决

| 项 | 风险 | 应对 |
|---|---|---|
| 行政区划 | 当前用文本输入，没有省市区级联字典 | 预留 `regionCode` 字段位；后续接 `region:list` 字典 |
| 地址全量扫描性能 | mock 端 `Array.from(map.values()).flat()` 是 O(N)，地址 < 1 万可接受 | 真实 WebApi 阶段用 SQL `LIMIT` + JOIN |
| 会员被删除的孤儿地址 | memberId 引用了不存在的会员 | 列表冗余显示"会员已注销"+ 灰显 + 禁用操作；后续可加外键 |
| admin 与 app 同时改 | 并发时可能丢更新 | mock 端忽略；WebApi 阶段加乐观锁 `updatedAt` |
| i18n 体积 | 一次性加 ~30 个 key | 远低于 mall/member 现有体量，无风险 |

---

## 10. 后续衔接

本 spec 落地的 admin 端 + mock 端，全部前端可独立运行。后续阶段：

1. **P3-ext 后端任务**：建 `EasyProduct.WebApi/Mall/Address` 模块（Controller/Service/Repository + EF Migration + xUnit）
2. **契约切换**：按 `docs/mock-guidelines.md` 第 8 节，将前端请求 baseURL 从 mock 切到真后端，路由路径 0 改动
3. **小程序端同步**：admin 端契约与 app 端完全一致，app 端无需任何改动