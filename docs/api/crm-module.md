# CRM 模块接口文档

> 客户关系管理模块，包含客户、供应商、进销存、财务等核心业务功能

## 模块概览

CRM 模块是 EasyProduct 的核心业务模块，分为以下几个子模块：

1. **基础数据**: 客户、供应商、币种、税率
2. **采购管理**: 采购订单、入库、采购退货
3. **销售管理**: 销售订单、出库、销售退货
4. **库存管理**: 仓库、库存、库存流水、盘点、预警
5. **财务管理**: 发票、收付款、应收应付、冲销、固定资产

---

## 实体对象

### 一、基础数据

#### Customer - 客户

```typescript
interface Customer {
  id: string                    // GUID 主键
  code: string                  // 客户编码
  name: string                  // 客户名称
  type: 'b2b' | 'retail'        // 类型：B2B企业/零售
  source: 'inquiry' | 'register' | 'manual' // 来源
  contactPerson: string         // 联系人
  phone: string                 // 电话
  email: string                 // 邮箱
  address: string               // 地址
  salesPersonName: string       // 销售员
  creditLimit: number           // 信用额度
  status: 0 | 1  // 状态：0=不活跃，1=活跃 // 状态
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

#### Supplier - 供应商

```typescript
interface Supplier {
  id: string                    // GUID 主键
  code: string                  // 供应商编码
  name: string                  // 供应商名称
  contactPerson: string         // 联系人
  phone: string                 // 电话
  email: string                 // 邮箱
  address: string               // 地址
  bankName: string              // 开户银行
  bankAccount: string           // 银行账号
  status: 0 | 1  // 状态：0=不活跃，1=活跃 // 状态
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

#### Currency - 币种

```typescript
interface Currency {
  id: string                    // GUID 主键
  code: string                  // 币种编码（如 CNY, USD）
  name: string                  // 币种名称
  symbol: string                // 符号
  exchangeRate: number          // 汇率（相对于本位币）
  isDefault: boolean            // 是否本位币
  status: 0 | 1  // 状态：0=不活跃，1=活跃 // 状态
  createdAt: string
  updatedAt: string
}
```

#### TaxRate - 税率

```typescript
interface TaxRate {
  id: string                    // GUID 主键
  code: string                  // 税率编码
  name: string                  // 税率名称
  rate: number                  // 税率百分比
  status: 0 | 1  // 状态：0=不活跃，1=活跃 // 状态
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

### 二、采购管理

#### PurchaseOrder - 采购订单

```typescript
interface PurchaseOrder {
  id: string                    // GUID 主键
  orderNo: string               // 订单编号
  supplierId: string            // 供应商ID
  supplierName: string          // 供应商名称
  warehouseId: string           // 入库仓库ID
  warehouseName: string         // 仓库名称
  currencyId: string            // 币种ID
  currencyCode: string          // 币种编码
  taxRateId: string             // 税率ID
  taxRate: number               // 税率
  totalAmount: number           // 订单总额
  taxAmount: number             // 税额
  status: 'draft' | 'submitted' | 'approved' | 'received' | 'cancelled' // 状态
  items: PurchaseOrderItem[]    // 订单明细
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}

interface PurchaseOrderItem {
  id: string                    // GUID 主键
  orderId: string               // 订单ID
  spuId: string                 // 商品ID
  spuName: string               // 商品名称
  specValues: string            // 规格值
  quantity: number              // 数量
  unitPrice: number             // 单价
  taxRate: number               // 税率
  taxAmount: number             // 税额
  amount: number                // 金额
  receivedQty: number           // 已入库数量
}
```

### 三、销售管理

#### SalesOrder - 销售订单

```typescript
interface SalesOrder {
  id: string                    // GUID 主键
  orderNo: string               // 订单编号
  customerId: string            // 客户ID
  customerName: string          // 客户名称
  warehouseId: string           // 出库仓库ID
  warehouseName: string         // 仓库名称
  currencyId: string            // 币种ID
  currencyCode: string          // 币种编码
  taxRateId: string             // 税率ID
  taxRate: number               // 税率
  totalAmount: number           // 订单总额
  taxAmount: number             // 税额
  discountAmount: number        // 折扣金额
  payAmount: number             // 应收金额
  status: 'draft' | 'submitted' | 'approved' | 'shipped' | 'completed' | 'cancelled' // 状态
  items: SalesOrderItem[]       // 订单明细
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}

interface SalesOrderItem {
  id: string                    // GUID 主键
  orderId: string               // 订单ID
  spuId: string                 // 商品ID
  spuName: string               // 商品名称
  specValues: string            // 规格值
  quantity: number              // 数量
  unitPrice: number             // 单价
  taxRate: number               // 税率
  taxAmount: number             // 税额
  discountAmount: number        // 折扣金额
  amount: number                // 金额
  shippedQty: number            // 已出库数量
}
```

### 四、库存管理

#### Warehouse - 仓库

```typescript
interface Warehouse {
  id: string                    // GUID 主键
  code: string                  // 仓库编码
  name: string                  // 仓库名称
  type: 'normal' | 'virtual' | 'damage' // 类型：普通/虚拟/残次品
  address: string               // 地址
  managerName: string           // 负责人
  phone: string                 // 电话
  status: 0 | 1  // 状态：0=不活跃，1=活跃 // 状态
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

#### Stock - 库存

```typescript
interface Stock {
  id: string                    // GUID 主键
  warehouseId: string           // 仓库ID
  warehouseName: string         // 仓库名称
  spuId: string                 // 商品ID
  spuName: string               // 商品名称
  specValues: string            // 规格值
  quantity: number              // 库存数量
  lockedQty: number             // 锁定数量
  availableQty: number          // 可用数量
  costPrice: number             // 成本价
  createdAt: string
  updatedAt: string
}
```

#### StockRecord - 库存流水

```typescript
interface StockRecord {
  id: string                    // GUID 主键
  warehouseId: string           // 仓库ID
  spuId: string                 // 商品ID
  specValues: string            // 规格值
  type: 'in' | 'out' | 'check' | 'adjust' // 类型：入库/出库/盘点/调整
  bizType: 'purchase' | 'sales' | 'return' | 'transfer' | 'check' | 'adjust' // 业务类型
  bizId: string                 // 业务单据ID
  bizNo: string                 // 业务单据编号
  quantity: number              // 数量（正数入库，负数出库）
  beforeQty: number             // 变动前数量
  afterQty: number              // 变动后数量
  costPrice: number             // 成本价
  amount: number                // 金额
  createdAt: string
}
```

#### StockCheck - 库存盘点

```typescript
interface StockCheck {
  id: string                    // GUID 主键
  checkNo: string               // 盘点单号
  warehouseId: string           // 仓库ID
  warehouseName: string         // 仓库名称
  status: 'draft' | 'submitted' | 'completed' | 'cancelled' // 状态
  items: StockCheckItem[]       // 盘点明细
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}

interface StockCheckItem {
  id: string                    // GUID 主键
  checkId: string               // 盘点ID
  spuId: string                 // 商品ID
  specValues: string            // 规格值
  systemQty: number             // 系统数量
  actualQty: number             // 实际数量
  diffQty: number               // 差异数量
  costPrice: number             // 成本价
  diffAmount: number            // 差异金额
}
```

#### StockAlert - 库存预警

```typescript
interface StockAlert {
  id: string                    // GUID 主键
  warehouseId: string           // 仓库ID
  spuId: string                 // 商品ID
  specValues: string            // 规格值
  currentQty: number            // 当前库存
  minQty: number                // 最低库存
  maxQty: number                // 最高库存
  alertType: 'low' | 'high'     // 预警类型
  status: 'pending' | 'handled' // 状态
  createdAt: string
}
```

### 五、财务管理

#### Invoice - 发票

```typescript
interface Invoice {
  id: string                    // GUID 主键
  invoiceNo: string             // 发票号码
  invoiceType: 'sales' | 'purchase' // 类型：销售发票/采购发票
  bizId: string                 // 业务单据ID
  bizNo: string                 // 业务单据编号
  customerId: string            // 客户ID（销售发票）
  supplierId: string            // 供应商ID（采购发票）
  amount: number                // 开票金额
  taxAmount: number             // 税额
  totalAmount: number           // 价税合计
  status: 'draft' | 'issued' | 'cancelled' // 状态
  issuedAt: string              // 开票日期
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

#### Payment - 收付款

```typescript
interface Payment {
  id: string                    // GUID 主键
  paymentNo: string             // 收付款单号
  paymentType: 'receive' | 'pay' // 类型：收款/付款
  bizId: string                 // 业务单据ID
  bizNo: string                 // 业务单据编号
  customerId: string            // 客户ID（收款）
  supplierId: string            // 供应商ID（付款）
  amount: number                // 金额
  bankAccountId: string         // 银行账户ID
  bankAccountName: string       // 银行账户名称
  paymentMethod: string         // 支付方式
  status: 'draft' | 'completed' | 'cancelled' // 状态
  paidAt: string                // 支付日期
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

#### Arap - 应收应付

```typescript
interface Arap {
  id: string                    // GUID 主键
  arapType: 'receivable' | 'payable' // 类型：应收/应付
  bizId: string                 // 业务单据ID
  bizNo: string                 // 业务单据编号
  bizType: 'sales' | 'purchase' | 'advance' | 'other' // 业务类型
  customerId: string            // 客户ID（应收）
  supplierId: string            // 供应商ID（应付）
  totalAmount: number           // 总金额
  paidAmount: number            // 已结算金额
  unpaidAmount: number          // 未结算金额
  status: 'unpaid' | 'partial' | 'paid' // 状态
  dueDate: string               // 到期日
  createdAt: string
  updatedAt: string
}
```

#### FixedAsset - 固定资产

```typescript
interface FixedAsset {
  id: string                    // GUID 主键
  assetNo: string               // 资产编号
  name: string                  // 资产名称
  category: string              // 资产类别
  specModel: string             // 规格型号
  unit: string                  // 单位
  quantity: number              // 数量
  originalValue: number         // 原值
  netValue: number              // 净值
  depreciation: number          // 累计折旧
  purchaseDate: string          // 购置日期
  useYears: number              // 使用年限
  department: string            // 使用部门
  managerName: string           // 管理人
  location: string              // 存放地点
  status: 'normal' | 'repair' | 'scrap' // 状态
  remark: string                // 备注
  createdAt: string
  updatedAt: string
}
```

#### Reversal - 冲销记录

```typescript
interface Reversal {
  id: string                    // GUID 主键
  reversalNo: string            // 冲销单号
  reversalType: 'invoice' | 'payment' | 'arap' // 冲销类型
  originalId: string            // 原单据ID
  originalNo: string            // 原单据编号
  reversalAmount: number        // 冲销金额
  reason: string                // 冲销原因
  status: 'draft' | 'approved' | 'cancelled' // 状态
  createdAt: string
  updatedAt: string
}
```

---

## 主要接口列表

### 客户管理
- `GET /api/admin/crm/customer/list` - 客户列表
- `GET /api/admin/crm/customer/:id` - 客户详情
- `POST /api/admin/crm/customer` - 创建客户
- `PUT /api/admin/crm/customer/:id` - 更新客户
- `DELETE /api/admin/crm/customer/:id` - 删除客户

### 供应商管理
- `GET /api/admin/crm/supplier/list` - 供应商列表
- `POST /api/admin/crm/supplier` - 创建供应商
- `PUT /api/admin/crm/supplier/:id` - 更新供应商

### 币种管理
- `GET /api/admin/crm/currency/list` - 币种列表
- `POST /api/admin/crm/currency` - 创建币种
- `PUT /api/admin/crm/currency/:id` - 更新币种

### 税率管理
- `GET /api/admin/crm/tax-rate/list` - 税率列表
- `POST /api/admin/crm/tax-rate` - 创建税率

### 采购订单
- `GET /api/admin/crm/purchase-order/list` - 采购订单列表
- `GET /api/admin/crm/purchase-order/:id` - 订单详情
- `POST /api/admin/crm/purchase-order` - 创建采购订单
- `PUT /api/admin/crm/purchase-order/:id` - 更新订单
- `POST /api/admin/crm/purchase-order/:id/submit` - 提交订单
- `POST /api/admin/crm/purchase-order/:id/approve` - 审批订单
- `POST /api/admin/crm/purchase-order/:id/receive` - 入库

### 销售订单
- `GET /api/admin/crm/sales-order/list` - 销售订单列表
- `GET /api/admin/crm/sales-order/:id` - 订单详情
- `POST /api/admin/crm/sales-order` - 创建销售订单
- `PUT /api/admin/crm/sales-order/:id` - 更新订单
- `POST /api/admin/crm/sales-order/:id/submit` - 提交订单
- `POST /api/admin/crm/sales-order/:id/approve` - 审批订单
- `POST /api/admin/crm/sales-order/:id/ship` - 出库发货

### 仓库管理
- `GET /api/admin/crm/warehouse/list` - 仓库列表
- `POST /api/admin/crm/warehouse` - 创建仓库
- `PUT /api/admin/crm/warehouse/:id` - 更新仓库

### 库存管理
- `GET /api/admin/crm/stock/list` - 库存列表
- `GET /api/admin/crm/stock-record/list` - 库存流水
- `GET /api/admin/crm/stock-check/list` - 盘点列表
- `POST /api/admin/crm/stock-check` - 创建盘点单
- `POST /api/admin/crm/stock-check/:id/complete` - 完成盘点
- `GET /api/admin/crm/stock-alert/list` - 库存预警列表

### 发票管理
- `GET /api/admin/crm/invoice/list` - 发票列表
- `POST /api/admin/crm/invoice` - 创建发票
- `POST /api/admin/crm/invoice/:id/issue` - 开票

### 收付款管理
- `GET /api/admin/crm/payment/list` - 收付款列表
- `POST /api/admin/crm/payment` - 创建收付款
- `POST /api/admin/crm/payment/:id/complete` - 完成支付

### 应收应付
- `GET /api/admin/crm/arap/list` - 应收应付列表
- `GET /api/admin/crm/arap/:id` - 详情

### 固定资产
- `GET /api/admin/crm/fixed-asset/list` - 资产列表
- `POST /api/admin/crm/fixed-asset` - 创建资产
- `PUT /api/admin/crm/fixed-asset/:id` - 更新资产

### 冲销管理
- `GET /api/admin/crm/reversal/list` - 冲销列表
- `POST /api/admin/crm/reversal` - 创建冲销

---

## 开发注意事项

1. **多币种支持**: 所有金额需关联币种，支持多币种核算
2. **税务处理**: 支持增值税，需正确计算税额
3. **库存扣减**: 采用加权平均法计算成本
4. **财务核销**: 收付款需与应收应付核销
5. **审批流程**: 关键业务需走审批流程（可通过工作流引擎）
6. **审计追踪**: 所有操作需记录审计日志

---

生成时间: 2026-09-07