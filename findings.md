# P4.2 销售订单模块开发发现

> **创建时间：** 2026-09-14

---

## Mockjs 数据结构分析

### 销售订单数据结构

**来源：** `mock-server/src/data/crm-sales.ts`

**主表（SalesOrder）：**
- 17 个字段
- 包含嵌套的 items 数组
- 自动计算金额（小计、税额、总金额）

**明细表（SalesOrderItem）：**
- 14 个字段
- 自动计算金额、税额、总金额

### API 接口设计

**来源：** `mock-server/src/routes/admin/crm-sales-order.ts`

**7 个接口：**
1. GET /crm/sales-order/customer/options - 客户下拉选项
2. GET /crm/sales-order/list - 订单列表
3. GET /crm/sales-order/:id - 订单详情
4. POST /crm/sales-order - 创建订单
5. PUT /crm/sales-order/:id - 更新订单
6. PUT /crm/sales-order/:id/status - 状态变更
7. DELETE /crm/sales-order/:id - 删除订单

---

## 核心业务规则

### 1. 状态流转

**有限状态机：**
```
draft → confirmed → shipped → completed
  ↓        ↓
cancelled  cancelled
```

**规则：**
- 草稿 → 已确认/已取消
- 已确认 → 已发货/已取消
- 已发货 → 已完成
- 已完成 → 终态
- 已取消 → 终态

**限制：**
- 仅草稿状态可修改
- 仅草稿状态可删除

### 2. 库存管理

**发货检查：**
```javascript
function checkStockAvailability(order) {
  for (item of order.items) {
    if (stock.available < item.quantity) {
      return { available: false, insufficientItems: [...] }
    }
  }
}
```

**自动出库：**
```javascript
function processSalesOutbound(order) {
  for (item of order.items) {
    // 1. 创建出库记录
    createStockRecord({ type: 'out', sourceType: 'sales_out' })
    // 2. 扣减库存
    stock.available -= item.quantity
    stock.total -= item.quantity
  }
}
```

### 3. 金额计算

**明细金额：**
```
amount = price × quantity
taxAmount = amount × taxRate / 100
totalAmount = amount + taxAmount
```

**订单金额：**
```
subtotalAmount = sum(items.amount)
taxAmount = sum(items.taxAmount)
totalAmount = subtotalAmount + taxAmount
```

### 4. 订单编号生成

**格式：** `SO-{year}-{sequence:04d}`

**示例：**
- SO-2026-0001
- SO-2026-0002
- SO-2026-0003

---

## 数据一致性要求

### 字段映射

| Mockjs 字段 | 后端字段 | 数据库字段 | 类型 |
|------------|---------|-----------|------|
| id | Id | id | VARCHAR(36) |
| orderNo | OrderNo | order_no | VARCHAR(20) |
| customerId | CustomerId | customer_id | VARCHAR(36) |
| customerName | CustomerName | customer_name | VARCHAR(100) |
| salesPersonName | SalesPersonName | sales_person_name | VARCHAR(50) |
| currencyCode | CurrencyCode | currency_code | VARCHAR(10) |
| currencySymbol | CurrencySymbol | currency_symbol | VARCHAR(10) |
| paymentTerms | PaymentTerms | payment_terms | VARCHAR(50) |
| deliveryDate | DeliveryDate | delivery_date | DATE |
| status | Status | status | VARCHAR(20) |
| items | Items | - | 关联查询 |
| subtotalAmount | SubtotalAmount | subtotal_amount | DECIMAL(18,2) |
| taxAmount | TaxAmount | tax_amount | DECIMAL(18,2) |
| totalAmount | TotalAmount | total_amount | DECIMAL(18,2) |
| remark | Remark | remark | VARCHAR(500) |
| createdAt | CreatedAt | created_at | DATETIME |
| updatedAt | UpdatedAt | updated_at | DATETIME |

### 状态枚举映射

| Mockjs 值 | 枚举名称 | 说明 |
|----------|---------|------|
| 'draft' | Draft | 草稿 |
| 'confirmed' | Confirmed | 已确认 |
| 'shipped' | Shipped | 已发货 |
| 'completed' | Completed | 已完成 |
| 'cancelled' | Cancelled | 已取消 |

---

## 依赖关系

### 外部依赖

```
SalesOrder
  ├── crm_customer（客户）
  ├── crm_currency（币种）
  ├── crm_tax_rate（税率）
  ├── product_sku（商品SKU）
  └── crm_warehouse（仓库）
```

### 库存依赖

```
SalesOrder.shipped
  ├── crm_stock（库存检查）
  └── crm_stock_record（出库记录）
```

**注意：** 库存模块还未开发，需要先检查是否存在库存表和相关服务。

---

## 技术要点

### 1. 事务处理

**需要事务的操作：**
- 创建订单 + 创建明细
- 更新订单 + 更新明细
- 发货 + 出库 + 扣减库存

### 2. 金额精度

**注意事项：**
- 使用 `decimal(18, 2)` 存储金额
- 计算时使用 `decimal` 类型，避免浮点数精度问题
- 返回时使用 `toFixed(2)` 确保两位小数

### 3. 性能优化

**建议：**
- 订单列表不查询明细（懒加载）
- 订单详情才查询明细
- 添加索引：order_no、customer_id、status、created_at

---

## Mockjs 特有逻辑

### 1. 客户名称冗余

**Mockjs 代码：**
```javascript
const cust = CUSTOMERS.find(c => c.id === body.customerId)
order.customerName = cust.name
```

**原因：** 避免频繁关联查询，提高查询性能

**实现：** 创建订单时自动填充客户名称

### 2. 币种默认值

**Mockjs 代码：**
```javascript
const cny = CURRENCIES.find(c => c.code === 'CNY') ?? CURRENCIES[0]
```

**实现：** 默认使用人民币，可切换

### 3. 仓库默认值

**Mockjs 代码：**
```javascript
const defaultWarehouseId = WAREHOUSES[0]?.id ?? ''
```

**实现：** 明细项默认使用第一个仓库，可修改

---

## 错误处理

### 业务异常

| 场景 | 错误信息 | HTTP 状态码 |
|------|---------|------------|
| 订单不存在 | 'sales order not found' | 404 |
| 客户不存在 | 'customer not found' | 404 |
| 非草稿不可修改 | 'only draft orders can be edited' | 400 |
| 非草稿不可删除 | 'only draft orders can be deleted' | 400 |
| 状态流转不合法 | 'cannot transition from X to Y' | 400 |
| 库存不足 | '库存不足: XXX' | 400 |

---

## 待确认问题

1. **库存模块状态**
   - 库存表是否已创建？
   - 库存服务是否已实现？
   - 如果未完成，如何处理出库逻辑？

2. **SKU 管理**
   - SKU 表是否已创建？
   - SKU 服务是否已实现？
   - 订单明细中的 SKU 信息从哪里获取？

3. **业务员管理**
   - 业务员是关联 basic_user 吗？
   - 还是独立的字段？

---

## 参考资料

- Mockjs 数据：`mock-server/src/data/crm-sales.ts`
- Mockjs 路由：`mock-server/src/routes/admin/crm-sales-order.ts`
- 集成设计文档：`docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`
- 后端开发规范：`docs/backend-guidelines.md`