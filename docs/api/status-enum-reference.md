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