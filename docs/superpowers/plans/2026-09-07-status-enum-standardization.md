# 状态字段统一规范实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 将所有枚举状态字段和布尔字段统一改为 int 类型，提升性能和一致性

**Architecture:** 分 4 个阶段实施：文档更新 → 后端枚举/实体/DTO 修改 → Mock 数据修改 → 前端类型/常量/组件修改

**Tech Stack:** C# (.NET 8)、SqlSugar、TypeScript (Vue 3)、MockJS

---

## 文件结构

### 后端（C#）
```
EasyProduct.WebApi/src/Core/Enums/
├── Status.cs              # 通用状态枚举
├── OrderStatus.cs         # 订单状态枚举
├── PaymentStatus.cs       # 支付状态枚举
├── MemberStatus.cs        # 会员状态枚举
└── WorkflowStatus.cs      # 流程状态枚举

EasyProduct.Models/Entitys/
├── Basic/Menu.cs          # 修改：status/visible 字段类型
├── Basic/AdminUser.cs     # 修改：status 字段类型
├── Basic/Role.cs          # 修改：status 字段类型
├── Basic/Dept.cs          # 修改：status 字段类型
├── Mall/Order.cs          # 修改：status 字段类型
└── ... 其他实体

EasyProduct.Models/Dto/
├── Basic/MenuDto.cs       # 修改：status/visible 字段类型
├── Mall/OrderDto.cs       # 修改：status 字段类型
└── ... 其他 DTO
```

### 前端（TypeScript）
```
EasyProduct.Admin/src/constants/
└── status.ts              # 新增：状态常量定义

EasyProduct.Admin/src/types/
└── common.ts              # 修改：状态类型定义

EasyProduct.Admin/src/locales/
├── zh-CN/status.ts        # 新增：中文翻译
└── en-US/status.ts        # 新增：英文翻译
```

### 文档
```
docs/api/
├── status-enum-reference.md     # 新增：完整映射表
├── basic-module.md              # 修改：实体定义
├── mall-module.md               # 修改：实体定义
├── crm-module.md                # 修改：实体定义
├── other-modules.md             # 修改：实体定义
├── api-app-module.md            # 修改：实体定义
├── api-site-module.md           # 修改：实体定义
└── README.md                    # 修改：总览
```

### Mock 数据
```
mock-server/src/data/
├── menu.js                # 修改：status/visible 字段值
├── user.js                # 修改：status 字段值
├── order.js               # 修改：status 字段值
└── ... 其他数据文件
```

---

## Phase 1: 文档更新

### Task 1.1: 创建状态枚举映射参考文档

**Files:**
- Create: `docs/api/status-enum-reference.md`

- [ ] **Step 1: 创建状态枚举映射文档**

```markdown
# 状态枚举完整映射参考表

> 本文档记录所有状态字段和布尔字段的 int 映射规则

---

## 一、通用状态枚举

### Status（通用状态）

| int 值 | 字符串值 | 说明 | 使用场景 |
|--------|---------|------|---------|
| 0 | disabled | 禁用 | 用户、角色、菜单、字典等 |
| 1 | enabled | 启用 | 用户、角色、菜单、字典等 |

---

## 二、业务状态枚举

### OrderStatus（订单状态）

| int 值 | 字符串值 | 说明 |
|--------|---------|------|
| 0 | cancelled | 已取消 |
| 1 | pending | 待支付 |
| 2 | paid | 已支付 |
| 3 | shipped | 已发货 |
| 4 | completed | 已完成 |
| 5 | refunded | 已退款 |

### PaymentStatus（支付状态）

| int 值 | 字符串值 | 说明 |
|--------|---------|------|
| 0 | pending | 待支付 |
| 1 | success | 成功 |
| 2 | failed | 失败 |
| 3 | refunded | 已退款 |

### MemberStatus（会员状态）

| int 值 | 字符串值 | 说明 |
|--------|---------|------|
| 0 | inactive | 不活跃 |
| 1 | active | 活跃 |

### WorkflowStatus（流程状态）

| int 值 | 字符串值 | 说明 |
|--------|---------|------|
| 0 | draft | 草稿 |
| 1 | submitted | 已提交 |
| 2 | approved | 已审批 |
| 3 | completed | 已完成 |
| 4 | cancelled | 已取消 |
| 5 | rejected | 已拒绝 |

---

## 三、布尔字段映射

| int 值 | boolean 值 | 说明 |
|--------|-----------|------|
| 0 | false | 否 |
| 1 | true | 是 |

**常见布尔字段：**
- `isDefault` - 是否默认
- `visible` - 是否可见
- `isTop` - 是否置顶
- `isRead` - 是否已读
- `selected` - 是否选中

---

## 四、模块字段清单

### Basic 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| AdminUser | status | Status | 0=禁用，1=启用 |
| Role | status | Status | 0=禁用，1=启用 |
| Menu | status | Status | 0=禁用，1=启用 |
| Menu | visible | Bool | 0=否，1=是 |
| Dept | status | Status | 0=禁用，1=启用 |
| DictType | status | Status | 0=禁用，1=启用 |
| DictData | status | Status | 0=禁用，1=启用 |

### Mall 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| Member | status | MemberStatus | 0=不活跃，1=活跃 |
| Order | status | OrderStatus | 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款 |
| PaymentRecord | status | PaymentStatus | 0=待支付，1=成功，2=失败，3=已退款 |
| Address | isDefault | Bool | 0=否，1=是 |

### CRM 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| Customer | status | Status | 0=禁用，1=启用 |
| Supplier | status | Status | 0=禁用，1=启用 |
| Currency | isDefault | Bool | 0=否，1=是 |
| Warehouse | status | Status | 0=禁用，1=启用 |
| PurchaseOrder | status | WorkflowStatus | 0=草稿，1=已提交，2=已审批，3=已完成，4=已取消 |
| SalesOrder | status | WorkflowStatus | 0=草稿，1=已提交，2=已审批，3=已完成，4=已取消 |

### Site 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| SiteNews | status | Status | 0=草稿，1=已发布 |
| SiteNews | isTop | Bool | 0=否，1=是 |
| SiteCategory | status | Status | 0=禁用，1=启用 |
| SiteBanner | status | Status | 0=禁用，1=启用 |
| SiteVideo | status | Status | 0=草稿，1=已发布 |
| SiteDownload | status | Status | 0=草稿，1=已发布 |

### Product 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| ProductCategory | status | Status | 0=禁用，1=启用 |
| ProductSpu | status | Status | 0=禁用，1=启用 |
| ProductSku | status | Status | 0=禁用，1=启用 |
| ProductChannel | status | Status | 0=禁用，1=启用 |

### App 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| CartItem | selected | Bool | 0=否，1=是 |
| Order | status | OrderStatus | 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款 |
| PaymentRecord | status | PaymentStatus | 0=待支付，1=成功，2=失败，3=已退款 |
| Address | isDefault | Bool | 0=否，1=是 |

### Workflow 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| WfDefinition | status | Status | 0=草稿，1=已发布，2=已归档 |
| WfInstance | status | Status | 0=运行中，1=已完成，2=已取消，3=已拒绝 |
| WfTask | status | Status | 0=待处理，1=已通过，2=已拒绝，3=已委托 |

### Ops 模块

| 实体 | 字段 | 类型 | 值域 |
|-----|------|------|------|
| OpsOperateLog | status | Status | 0=失败，1=成功 |
| OpsLoginLog | status | Status | 0=失败，1=成功 |
| OpsTask | status | Status | 0=暂停，1=运行中，2=错误 |

---

生成时间: 2026-09-07
```

- [ ] **Step 2: 提交文档**

Run: `git add docs/api/status-enum-reference.md && git commit -m "docs(api): 添加状态枚举完整映射参考表"`
Expected: Commit successful

---

### Task 1.2: 更新 Basic 模块 API 文档

**Files:**
- Modify: `docs/api/basic-module.md:7-117`（实体定义部分）

- [ ] **Step 1: 修改 AdminUser 实体定义**

```typescript
interface AdminUser {
  id: string                    // GUID 主键
  userName: string              // 用户名（登录账号）
  password: string              // 密码
  realName: string              // 真实姓名
  email: string                 // 邮箱
  phone: string                 // 手机号
  status: 0 | 1                 // 状态：0=禁用，1=启用
  deptId: string                // 部门ID
  roleIds: string[]             // 角色ID列表
  createdAt: string             // 创建时间（ISO 8601）
  updatedAt: string             // 更新时间（ISO 8601）
}
```

- [ ] **Step 2: 修改 Role 实体定义**

```typescript
interface Role {
  id: string                    // GUID 主键
  name: string                  // 角色名称
  code: string                  // 角色编码
  status: 0 | 1                 // 状态：0=禁用，1=启用
  sort: number                  // 排序
  remark?: string               // 备注
  menuIds?: string[]            // 菜单ID列表
  createdAt: string
  updatedAt: string
}
```

- [ ] **Step 3: 修改 Menu 实体定义**

```typescript
interface Menu {
  id: string                    // GUID 主键
  parentId: string              // 父菜单ID（'0' 表示根节点）
  name: string                  // 菜单名称（路由name）
  path: string                  // 路由路径
  titleKey: string              // 标题i18n key
  icon: string                  // 图标名称
  sort: number                  // 排序
  status: 0 | 1                 // 状态：0=禁用，1=启用
  visible: 0 | 1                // 是否可见：0=否，1=是
  children?: Menu[]             // 子菜单
}
```

- [ ] **Step 4: 修改 Dept 实体定义**

```typescript
interface Dept {
  id: string                    // GUID 主键
  parentId: string              // 父部门ID
  name: string                  // 部门名称
  code: string                  // 部门编码
  sort: number                  // 排序
  status: 0 | 1                 // 状态：0=禁用，1=启用
  leaderName?: string           // 部门负责人
  phone?: string                // 联系电话
  email?: string                // 邮箱
  fullPath?: string             // 完整路径（如：总公司/技术部）
  level?: number                // 层级
  memberCount?: number          // 成员数量
  description?: string          // 描述
  children?: Dept[]             // 子部门
}
```

- [ ] **Step 5: 修改 DictType 实体定义**

```typescript
interface DictType {
  id: string                    // GUID 主键
  name: string                  // 字典类型名称
  code: string                  // 字典类型编码（唯一）
  status: 0 | 1                 // 状态：0=禁用，1=启用
  remark: string                // 备注
}
```

- [ ] **Step 6: 修改 DictData 实体定义**

```typescript
interface DictData {
  id: string                    // GUID 主键
  typeCode: string              // 字典类型编码
  value: string                 // 字典值
  labelKey: string              // 标签i18n key
  sort: number                  // 排序
  status: 0 | 1                 // 状态：0=禁用，1=启用
}
```

- [ ] **Step 7: 修改数据库表设计（basic_user）**

```sql
CREATE TABLE basic_user (
  id VARCHAR(36) PRIMARY KEY,
  user_name VARCHAR(50) NOT NULL UNIQUE,
  password VARCHAR(255) NOT NULL,
  real_name VARCHAR(50) NOT NULL,
  email VARCHAR(100),
  phone VARCHAR(20),
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  dept_id VARCHAR(36),
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_dept_id (dept_id)
);
```

- [ ] **Step 8: 修改数据库表设计（basic_menu）**

```sql
CREATE TABLE basic_menu (
  id VARCHAR(36) PRIMARY KEY,
  parent_id VARCHAR(36) DEFAULT '0',
  name VARCHAR(50) NOT NULL,
  path VARCHAR(200),
  title_key VARCHAR(100) NOT NULL,
  icon VARCHAR(50),
  sort INT DEFAULT 0,
  status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  visible INT DEFAULT 1 COMMENT '是否可见：0=否，1=是',
  INDEX idx_parent_id (parent_id)
);
```

- [ ] **Step 9: 更新开发注意事项**

```markdown
## 开发注意事项

1. **认证方式**: Admin JWT，需在请求头携带 `Authorization: Bearer {token}`
2. **权限控制**: 通过 `permissions` 数组控制接口访问权限
3. **菜单过滤**: 登录时根据用户角色过滤菜单
4. **字典使用**: 前端使用 `useDict` composable 获取字典数据
5. **状态字段**: 使用 int 类型（0=禁用，1=启用）
6. **布尔字段**: 使用 int 类型（0=否，1=是）
7. **密码安全**: 密码需加密存储（BCrypt）
8. **审计日志**: 用户操作需记录到操作日志表
```

- [ ] **Step 10: 提交文档更新**

Run: `git add docs/api/basic-module.md && git commit -m "docs(api): 更新 Basic 模块实体定义 - 状态字段改为 int"`
Expected: Commit successful

---

### Task 1.3: 更新 Mall 模块 API 文档

**Files:**
- Modify: `docs/api/mall-module.md:7-128`（实体定义部分）

- [ ] **Step 1: 修改 Member 实体定义**

```typescript
interface Member {
  id: string                    // GUID 主键
  nickname: string              // 昵称
  avatar: string                // 头像URL
  phone: string                 // 手机号
  openid: string                // 微信OpenID
  levelId: string               // 会员等级ID
  levelName: string             // 会员等级名称
  points: number                // 当前积分
  totalSpent: number            // 累计消费金额
  orderCount: number            // 订单数量
  status: 0 | 1                 // 状态：0=不活跃，1=活跃
  createdAt: string
  updatedAt: string
}
```

- [ ] **Step 2: 修改 MemberLevel 实体定义**

```typescript
interface MemberLevel {
  id: string                    // GUID 主键
  name: string                  // 等级名称
  minPoints: number             // 升级所需积分
  discount: number              // 折扣率（如0.95表示95折）
  sort: number                  // 排序
  status: 0 | 1                 // 状态：0=禁用，1=启用
  createdAt: string
  updatedAt: string
}
```

- [ ] **Step 3: 修改 PointsRecord 实体定义**

```typescript
interface PointsRecord {
  id: string                    // GUID 主键
  memberId: string              // 会员ID
  memberName: string            // 会员名称
  type: 0 | 1                   // 类型：0=消费，1=获得
  amount: number                // 积分数量（正数获得，负数消费）
  source: 0 | 1 | 2 | 3 | 4     // 来源：0=订单，1=签到，2=活动，3=退款，4=调整
  description: string           // 描述
  createdAt: string
}
```

- [ ] **Step 4: 修改 Coupon 实体定义**

```typescript
interface Coupon {
  id: string                    // GUID 主键
  name: string                  // 优惠券名称
  type: 0 | 1                   // 类型：0=固定金额，1=百分比折扣
  value: number                 // 面值（固定金额或折扣率）
  minSpend: number              // 最低消费金额
  totalCount: number            // 发行总量
  issuedCount: number           // 已发放数量
  usedCount: number             // 已使用数量
  startDate: string             // 开始日期（YYYY-MM-DD）
  endDate: string               // 结束日期（YYYY-MM-DD）
  status: 0 | 1                 // 状态：0=禁用，1=启用
  createdAt: string
  updatedAt: string
}
```

- [ ] **Step 5: 修改 Order 实体定义**

```typescript
interface Order {
  id: string                    // GUID 主键
  orderNo: string               // 订单编号
  memberId: string              // 会员ID
  memberName: string            // 会员名称
  memberPhone: string           // 会员手机
  totalAmount: number           // 商品总金额
  discountAmount: number        // 优惠金额
  pointsAmount: number          // 积分抵扣金额
  shippingFee: number           // 运费
  payAmount: number             // 实付金额
  couponId: string              // 优惠券ID
  couponName: string            // 优惠券名称
  status: 0 | 1 | 2 | 3 | 4 | 5 // 订单状态：0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款
  paymentMethod: string         // 支付方式
  remark: string                // 备注
  items: OrderItem[]            // 订单明细
  createdAt: string
  updatedAt: string
}
```

- [ ] **Step 6: 修改 PaymentRecord 实体定义**

```typescript
interface PaymentRecord {
  id: string                    // GUID 主键
  orderId: string               // 订单ID
  orderNo: string               // 订单编号
  amount: number                // 支付金额
  method: string                // 支付方式（wechat/alipay）
  status: 0 | 1 | 2 | 3         // 支付状态：0=待支付，1=成功，2=失败，3=已退款
  transactionId: string         // 第三方交易号
  paidAt: string                // 支付时间
  createdAt: string
}
```

- [ ] **Step 7: 更新数据库表设计（mall_member）**

```sql
CREATE TABLE mall_member (
  id VARCHAR(36) PRIMARY KEY,
  nickname VARCHAR(50),
  avatar VARCHAR(500),
  phone VARCHAR(20),
  openid VARCHAR(100) UNIQUE,
  level_id VARCHAR(36),
  points INT DEFAULT 0,
  total_spent DECIMAL(12,2) DEFAULT 0,
  order_count INT DEFAULT 0,
  status INT DEFAULT 1 COMMENT '状态：0=不活跃，1=活跃',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_phone (phone),
  INDEX idx_openid (openid),
  INDEX idx_level_id (level_id)
);
```

- [ ] **Step 8: 更新数据库表设计（mall_order）**

```sql
CREATE TABLE mall_order (
  id VARCHAR(36) PRIMARY KEY,
  order_no VARCHAR(50) NOT NULL UNIQUE,
  member_id VARCHAR(36) NOT NULL,
  member_name VARCHAR(50),
  member_phone VARCHAR(20),
  total_amount DECIMAL(12,2) NOT NULL,
  discount_amount DECIMAL(12,2) DEFAULT 0,
  points_amount DECIMAL(12,2) DEFAULT 0,
  shipping_fee DECIMAL(10,2) DEFAULT 0,
  pay_amount DECIMAL(12,2) NOT NULL,
  coupon_id VARCHAR(36),
  coupon_name VARCHAR(100),
  status INT NOT NULL COMMENT '订单状态：0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款',
  payment_method VARCHAR(20),
  remark TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_member_id (member_id),
  INDEX idx_status (status),
  INDEX idx_created_at (created_at)
);
```

- [ ] **Step 9: 更新数据库表设计（mall_address）**

```sql
CREATE TABLE mall_address (
  id VARCHAR(36) PRIMARY KEY,
  member_id VARCHAR(36) NOT NULL,
  receiver_name VARCHAR(50) NOT NULL,
  phone VARCHAR(20) NOT NULL,
  province VARCHAR(50),
  city VARCHAR(50),
  district VARCHAR(50),
  detail_address VARCHAR(200),
  is_default INT DEFAULT 0 COMMENT '是否默认：0=否，1=是',
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_member_id (member_id)
);
```

- [ ] **Step 10: 提交文档更新**

Run: `git add docs/api/mall-module.md && git commit -m "docs(api): 更新 Mall 模块实体定义 - 状态字段改为 int"`
Expected: Commit successful

---

### Task 1.4: 更新其他模块文档（批量）

**Files:**
- Modify: `docs/api/crm-module.md`
- Modify: `docs/api/other-modules.md`
- Modify: `docs/api/api-app-module.md`
- Modify: `docs/api/api-site-module.md`
- Modify: `docs/api/README.md`

- [ ] **Step 1: 使用批量替换更新 CRM 模块文档**

Run: `sed -i "s/status: 'active' | 'inactive'/status: 0 | 1  \/\/ 状态：0=不活跃，1=活跃/g" docs/api/crm-module.md`
Expected: In-place replacement

Run: `sed -i "s/status: 'enabled' | 'disabled'/status: 0 | 1  \/\/ 状态：0=禁用，1=启用/g" docs/api/crm-module.md`
Expected: In-place replacement

Run: `sed -i "s/status: 'draft' | 'submitted' | 'approved' | 'received' | 'cancelled'/status: 0 | 1 | 2 | 3 | 4  \/\/ 状态：0=草稿，1=已提交，2=已审批，3=已完成，4=已取消/g" docs/api/crm-module.md`
Expected: In-place replacement

- [ ] **Step 2: 使用批量替换更新 Other 模块文档**

Run: `sed -i "s/isTop: boolean/isTop: 0 | 1  \/\/ 是否置顶：0=否，1=是/g" docs/api/other-modules.md`
Expected: In-place replacement

Run: `sed -i "s/visible: boolean/visible: 0 | 1  \/\/ 是否可见：0=否，1=是/g" docs/api/other-modules.md`
Expected: In-place replacement

Run: `sed -i "s/isDefault: boolean/isDefault: 0 | 1  \/\/ 是否默认：0=否，1=是/g" docs/api/other-modules.md`
Expected: In-place replacement

- [ ] **Step 3: 使用批量替换更新 App 模块文档**

Run: `sed -i "s/selected: boolean/selected: 0 | 1  \/\/ 是否选中：0=否，1=是/g" docs/api/api-app-module.md`
Expected: In-place replacement

Run: `sed -i "s/isRead: boolean/isRead: 0 | 1  \/\/ 是否已读：0=否，1=是/g" docs/api/api-app-module.md`
Expected: In-place replacement

- [ ] **Step 4: 更新 README.md 总览文档**

手动编辑 `docs/api/README.md`，在"开发注意事项"部分添加：

```markdown
## 开发注意事项

1. **契约优先**: 新增/修改 mock 接口必须与后端规范第 5 节路由和信封逐字一致
2. **禁止自创接口**: Mock 接口必须来源于后端需求，不得随意新增
3. **GUID 主键**: 所有实体主键使用 GUID 字符串
4. **JSON camelCase**: 接口返回数据一律使用 camelCase 命名
5. **分页参数**: pageIndex 从 1 开始，pageSize 默认 10
6. **状态字段**: 使用 int 类型（详见 `docs/api/status-enum-reference.md`）
7. **布尔字段**: 使用 int 类型（0=否，1=是）
```

- [ ] **Step 5: 提交所有文档更新**

Run: `git add docs/api/*.md && git commit -m "docs(api): 批量更新所有模块文档 - 状态字段改为 int"`
Expected: Commit successful

---

## Phase 2: 后端代码修改

### Task 2.1: 创建枚举类定义

**Files:**
- Create: `EasyProduct.WebApi/src/Core/Enums/Status.cs`
- Create: `EasyProduct.WebApi/src/Core/Enums/OrderStatus.cs`
- Create: `EasyProduct.WebApi/src/Core/Enums/PaymentStatus.cs`
- Create: `EasyProduct.WebApi/src/Core/Enums/MemberStatus.cs`
- Create: `EasyProduct.WebApi/src/Core/Enums/WorkflowStatus.cs`

- [ ] **Step 1: 创建 Status.cs 枚举类**

```csharp
namespace EasyProduct.Core.Enums;

/// <summary>
/// 通用状态枚举
/// </summary>
public enum Status
{
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 0,
    
    /// <summary>
    /// 启用
    /// </summary>
    Enabled = 1
}
```

- [ ] **Step 2: 创建 OrderStatus.cs 枚举类**

```csharp
namespace EasyProduct.Core.Enums;

/// <summary>
/// 订单状态枚举
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 0,
    
    /// <summary>
    /// 待支付
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// 已支付
    /// </summary>
    Paid = 2,
    
    /// <summary>
    /// 已发货
    /// </summary>
    Shipped = 3,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// 已退款
    /// </summary>
    Refunded = 5
}
```

- [ ] **Step 3: 创建 PaymentStatus.cs 枚举类**

```csharp
namespace EasyProduct.Core.Enums;

/// <summary>
/// 支付状态枚举
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// 待支付
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// 成功
    /// </summary>
    Success = 1,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 2,
    
    /// <summary>
    /// 已退款
    /// </summary>
    Refunded = 3
}
```

- [ ] **Step 4: 创建 MemberStatus.cs 枚举类**

```csharp
namespace EasyProduct.Core.Enums;

/// <summary>
/// 会员状态枚举
/// </summary>
public enum MemberStatus
{
    /// <summary>
    /// 不活跃
    /// </summary>
    Inactive = 0,
    
    /// <summary>
    /// 活跃
    /// </summary>
    Active = 1
}
```

- [ ] **Step 5: 创建 WorkflowStatus.cs 枚举类**

```csharp
namespace EasyProduct.Core.Enums;

/// <summary>
/// 流程状态枚举
/// </summary>
public enum WorkflowStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// 已提交
    /// </summary>
    Submitted = 1,
    
    /// <summary>
    /// 已审批
    /// </summary>
    Approved = 2,
    
    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected = 5
}
```

- [ ] **Step 6: 验证编译**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 7: 提交枚举类**

Run: `git add EasyProduct.WebApi/src/Core/Enums/*.cs && git commit -m "feat(api): 添加状态枚举类定义"`
Expected: Commit successful

---

### Task 2.2: 修改 Basic 模块实体类

**Files:**
- Modify: `EasyProduct.Models/Entitys/Basic/Menu.cs`
- Modify: `EasyProduct.Models/Entitys/Basic/AdminUser.cs`
- Modify: `EasyProduct.Models/Entitys/Basic/Role.cs`
- Modify: `EasyProduct.Models/Entitys/Basic/Dept.cs`

**注意：** 由于项目尚未实现后端代码，此任务为示例模板。实际实施时需要根据实际存在的文件路径调整。

- [ ] **Step 1: 修改 Menu 实体（如果存在）**

如果 `EasyProduct.Models/Entitys/Basic/Menu.cs` 存在，修改字段：

```csharp
using EasyProduct.Core.Enums;

[SugarTable("basic_menu")]
public class Menu : BaseEntity
{
    // ... 其他字段
    
    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public Status Status { get; set; }
    
    /// <summary>
    /// 是否可见：0=否，1=是
    /// </summary>
    public int Visible { get; set; } = 1;
}
```

- [ ] **Step 2: 提交实体类修改**

Run: `git add EasyProduct.Models/Entitys/Basic/*.cs && git commit -m "refactor(api): Basic 模块实体类 - 状态字段改为 int"`
Expected: Commit successful

---

### Task 2.3: 修改 DTO 类

**Files:**
- Modify: `EasyProduct.Models/Dto/Basic/MenuDto.cs`
- Modify: `EasyProduct.Models/Dto/Mall/OrderDto.cs`

**注意：** 由于项目尚未实现后端代码，此任务为示例模板。

- [ ] **Step 1: 修改 MenuDto（如果存在）**

```csharp
/// <summary>
/// 菜单DTO
/// </summary>
public class MenuDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; }
    
    // ... 其他字段
    
    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }
    
    /// <summary>
    /// 是否可见：0=否，1=是
    /// </summary>
    public int Visible { get; set; }
}
```

- [ ] **Step 2: 提交 DTO 修改**

Run: `git add EasyProduct.Models/Dto/**/*.cs && git commit -m "refactor(api): DTO 类 - 状态字段改为 int"`
Expected: Commit successful

---

## Phase 3: Mock 数据修改

### Task 3.1: 修改 Basic 模块 Mock 数据

**Files:**
- Modify: `mock-server/src/data/menu.js`
- Modify: `mock-server/src/data/user.js`
- Modify: `mock-server/src/data/role.js`

- [ ] **Step 1: 修改 menu.js Mock 数据**

```javascript
// mock-server/src/data/menu.js

module.exports = [
  {
    id: '1',
    parentId: '0',
    name: 'System',
    path: '/system',
    titleKey: 'menu.system',
    icon: 'setting',
    sort: 1,
    status: 1,      // 1=启用
    visible: 1,     // 1=可见
    children: [
      {
        id: '2',
        parentId: '1',
        name: 'User',
        path: '/system/user',
        titleKey: 'menu.user',
        icon: 'user',
        sort: 1,
        status: 1,  // 1=启用
        visible: 1, // 1=可见
      }
    ]
  }
]
```

- [ ] **Step 2: 修改 user.js Mock 数据**

```javascript
// mock-server/src/data/user.js

module.exports = [
  {
    id: '1',
    userName: 'admin',
    password: '$2a$10$...', // BCrypt hash
    realName: '管理员',
    email: 'admin@example.com',
    phone: '13800138000',
    status: 1,  // 1=启用
    deptId: '1',
    roleIds: ['1'],
    createdAt: '2026-09-07T10:00:00Z',
    updatedAt: '2026-09-07T10:00:00Z',
  }
]
```

- [ ] **Step 3: 提交 Mock 数据修改**

Run: `git add mock-server/src/data/*.js && git commit -m "refactor(mock): Basic 模块 Mock 数据 - 状态字段改为 int"`
Expected: Commit successful

---

### Task 3.2: 修改 Mall 模块 Mock 数据

**Files:**
- Modify: `mock-server/src/data/order.js`
- Modify: `mock-server/src/data/member.js`

- [ ] **Step 1: 修改 order.js Mock 数据**

```javascript
// mock-server/src/data/order.js

module.exports = [
  {
    id: '1',
    orderNo: 'ORD-20260001',
    memberId: 'member-001',
    memberName: '张三',
    memberPhone: '13800138000',
    totalAmount: 299.00,
    discountAmount: 0,
    pointsAmount: 0,
    shippingFee: 10.00,
    payAmount: 309.00,
    couponId: null,
    couponName: null,
    status: 2,  // 2=已支付
    paymentMethod: 'wechat',
    remark: '',
    items: [],
    createdAt: '2026-09-07T10:00:00Z',
    updatedAt: '2026-09-07T10:00:00Z',
  }
]
```

- [ ] **Step 2: 修改 member.js Mock 数据**

```javascript
// mock-server/src/data/member.js

module.exports = [
  {
    id: 'member-001',
    nickname: '张三',
    avatar: 'https://example.com/avatar.jpg',
    phone: '13800138000',
    openid: 'openid-001',
    levelId: '1',
    levelName: '普通会员',
    points: 100,
    totalSpent: 1000.00,
    orderCount: 5,
    status: 1,  // 1=活跃
    createdAt: '2026-09-01T10:00:00Z',
    updatedAt: '2026-09-07T10:00:00Z',
  }
]
```

- [ ] **Step 3: 提交 Mock 数据修改**

Run: `git add mock-server/src/data/*.js && git commit -m "refactor(mock): Mall 模块 Mock 数据 - 状态字段改为 int"`
Expected: Commit successful

---

## Phase 4: 前端代码修改

### Task 4.1: 创建状态常量文件

**Files:**
- Create: `EasyProduct.Admin/src/constants/status.ts`

- [ ] **Step 1: 创建 status.ts 常量文件**

```typescript
/**
 * 状态常量定义
 */

/**
 * 通用状态常量
 */
export const STATUS = {
  DISABLED: 0,  // 禁用
  ENABLED: 1,   // 启用
} as const

/**
 * 订单状态常量
 */
export const ORDER_STATUS = {
  CANCELLED: 0,  // 已取消
  PENDING: 1,    // 待支付
  PAID: 2,       // 已支付
  SHIPPED: 3,    // 已发货
  COMPLETED: 4,  // 已完成
  REFUNDED: 5,   // 已退款
} as const

/**
 * 支付状态常量
 */
export const PAYMENT_STATUS = {
  PENDING: 0,    // 待支付
  SUCCESS: 1,    // 成功
  FAILED: 2,     // 失败
  REFUNDED: 3,   // 已退款
} as const

/**
 * 会员状态常量
 */
export const MEMBER_STATUS = {
  INACTIVE: 0,  // 不活跃
  ACTIVE: 1,    // 活跃
} as const

/**
 * 流程状态常量
 */
export const WORKFLOW_STATUS = {
  DRAFT: 0,       // 草稿
  SUBMITTED: 1,   // 已提交
  APPROVED: 2,    // 已审批
  COMPLETED: 3,   // 已完成
  CANCELLED: 4,   // 已取消
  REJECTED: 5,    // 已拒绝
} as const

/**
 * 布尔值常量
 */
export const BOOL = {
  FALSE: 0,  // 否
  TRUE: 1,   // 是
} as const
```

- [ ] **Step 2: 提交常量文件**

Run: `git add EasyProduct.Admin/src/constants/status.ts && git commit -m "feat(admin): 添加状态常量定义"`
Expected: Commit successful

---

### Task 4.2: 修改 TypeScript 类型定义

**Files:**
- Modify: `EasyProduct.Admin/src/types/common.ts`
- Modify: `EasyProduct.Admin/src/types/menu.ts`
- Modify: `EasyProduct.Admin/src/types/order.ts`

- [ ] **Step 1: 修改 common.ts 类型定义**

```typescript
/**
 * 通用状态：0=禁用，1=启用
 */
export type Status = 0 | 1

/**
 * 订单状态
 * 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款
 */
export type OrderStatus = 0 | 1 | 2 | 3 | 4 | 5

/**
 * 支付状态
 * 0=待支付，1=成功，2=失败，3=已退款
 */
export type PaymentStatus = 0 | 1 | 2 | 3

/**
 * 会员状态
 * 0=不活跃，1=活跃
 */
export type MemberStatus = 0 | 1

/**
 * 流程状态
 * 0=草稿，1=已提交，2=已审批，3=已完成，4=已取消，5=已拒绝
 */
export type WorkflowStatus = 0 | 1 | 2 | 3 | 4 | 5

/**
 * 布尔值：0=否，1=是
 */
export type BoolValue = 0 | 1
```

- [ ] **Step 2: 修改 menu.ts 类型定义**

```typescript
import type { Status, BoolValue } from './common'

/**
 * 菜单实体
 */
export interface Menu {
  id: string
  parentId: string
  name: string
  path: string
  titleKey: string
  icon: string
  sort: number
  status: Status        // 0=禁用，1=启用
  visible: BoolValue    // 0=不可见，1=可见
  children?: Menu[]
}
```

- [ ] **Step 3: 修改 order.ts 类型定义**

```typescript
import type { OrderStatus } from './common'

/**
 * 订单实体
 */
export interface Order {
  id: string
  orderNo: string
  memberId: string
  memberName: string
  totalAmount: number
  status: OrderStatus   // 0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款
  createdAt: string
  updatedAt: string
}
```

- [ ] **Step 4: 提交类型定义修改**

Run: `git add EasyProduct.Admin/src/types/*.ts && git commit -m "refactor(admin): TypeScript 类型定义 - 状态字段改为 int"`
Expected: Commit successful

---

### Task 4.3: 添加 i18n 翻译

**Files:**
- Create: `EasyProduct.Admin/src/locales/zh-CN/status.ts`
- Create: `EasyProduct.Admin/src/locales/en-US/status.ts`

- [ ] **Step 1: 创建中文翻译文件**

```typescript
export default {
  status: {
    '0': '禁用',
    '1': '启用',
  },
  
  orderStatus: {
    '0': '已取消',
    '1': '待支付',
    '2': '已支付',
    '3': '已发货',
    '4': '已完成',
    '5': '已退款',
  },
  
  paymentStatus: {
    '0': '待支付',
    '1': '支付成功',
    '2': '支付失败',
    '3': '已退款',
  },
  
  memberStatus: {
    '0': '不活跃',
    '1': '活跃',
  },
  
  workflowStatus: {
    '0': '草稿',
    '1': '已提交',
    '2': '已审批',
    '3': '已完成',
    '4': '已取消',
    '5': '已拒绝',
  },
  
  bool: {
    '0': '否',
    '1': '是',
  },
}
```

- [ ] **Step 2: 创建英文翻译文件**

```typescript
export default {
  status: {
    '0': 'Disabled',
    '1': 'Enabled',
  },
  
  orderStatus: {
    '0': 'Cancelled',
    '1': 'Pending',
    '2': 'Paid',
    '3': 'Shipped',
    '4': 'Completed',
    '5': 'Refunded',
  },
  
  paymentStatus: {
    '0': 'Pending',
    '1': 'Success',
    '2': 'Failed',
    '3': 'Refunded',
  },
  
  memberStatus: {
    '0': 'Inactive',
    '1': 'Active',
  },
  
  workflowStatus: {
    '0': 'Draft',
    '1': 'Submitted',
    '2': 'Approved',
    '3': 'Completed',
    '4': 'Cancelled',
    '5': 'Rejected',
  },
  
  bool: {
    '0': 'No',
    '1': 'Yes',
  },
}
```

- [ ] **Step 3: 提交 i18n 文件**

Run: `git add EasyProduct.Admin/src/locales/*/status.ts && git commit -m "feat(admin): 添加状态字段 i18n 翻译"`
Expected: Commit successful

---

## 验收测试

### Task 5.1: 验证后端编译

- [ ] **Step 1: 编译后端项目**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors, 0 warnings

- [ ] **Step 2: 运行后端单元测试（如果有）**

Run: `cd EasyProduct.WebApi && dotnet test`
Expected: All tests passed

---

### Task 5.2: 验证 Mock 接口

- [ ] **Step 1: 启动 Mock 服务器**

Run: `cd mock-server && pnpm dev`
Expected: Server running at http://localhost:7700

- [ ] **Step 2: 测试菜单接口**

Run: `curl http://localhost:7700/api/admin/basic/menu/tree`
Expected: JSON response with `status: 1` (int type)

- [ ] **Step 3: 测试订单接口**

Run: `curl http://localhost:7700/api/admin/mall/order/list`
Expected: JSON response with `status: 2` (int type)

---

### Task 5.3: 验证前端类型检查

- [ ] **Step 1: 运行前端类型检查**

Run: `cd EasyProduct.Admin && pnpm type-check`
Expected: No type errors

- [ ] **Step 2: 运行前端 lint**

Run: `cd EasyProduct.Admin && pnpm lint`
Expected: No lint errors

---

## 完成检查

- [ ] 所有文档已更新
- [ ] 后端枚举类已创建
- [ ] Mock 数据已修改
- [ ] 前端类型和常量已创建
- [ ] i18n 翻译已添加
- [ ] 所有测试通过
- [ ] 代码已提交

---

**计划完成！**