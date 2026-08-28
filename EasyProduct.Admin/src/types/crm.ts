// CRM 主数据类型定义

// ── 客户 ──
export interface Customer {
  id: string
  code: string
  name: string
  type: 'b2b' | 'retail'
  source: 'inquiry' | 'register' | 'manual'
  contactPerson: string
  phone: string
  email: string
  address: string
  salesPersonName: string
  creditLimit: number
  status: 'active' | 'inactive'
  remark: string
  createdAt: string
  updatedAt: string
}

export const CUSTOMER_TYPE_OPTIONS = [
  { value: 'b2b', labelKey: 'crm.customer.typeB2b' },
  { value: 'retail', labelKey: 'crm.customer.typeRetail' },
] as const

export const CUSTOMER_SOURCE_OPTIONS = [
  { value: 'inquiry', labelKey: 'crm.customer.sourceInquiry' },
  { value: 'register', labelKey: 'crm.customer.sourceRegister' },
  { value: 'manual', labelKey: 'crm.customer.sourceManual' },
] as const

export const CUSTOMER_STATUS_OPTIONS = [
  { value: 'active', labelKey: 'crm.customer.statusActive' },
  { value: 'inactive', labelKey: 'crm.customer.statusInactive' },
] as const

// ── 供应商 ──
export interface Supplier {
  id: string
  code: string
  name: string
  contactPerson: string
  phone: string
  email: string
  address: string
  bankName: string
  bankAccount: string
  status: 'active' | 'inactive'
  remark: string
  createdAt: string
  updatedAt: string
}

export const SUPPLIER_STATUS_OPTIONS = [
  { value: 'active', labelKey: 'crm.supplier.statusActive' },
  { value: 'inactive', labelKey: 'crm.supplier.statusInactive' },
] as const

// ── 币种 ──
export interface Currency {
  id: string
  code: string
  name: string
  symbol: string
  exchangeRate: number
  isDefault: boolean
  status: 'active' | 'inactive'
  createdAt: string
  updatedAt: string
}

export const CURRENCY_STATUS_OPTIONS = [
  { value: 'active', labelKey: 'crm.currency.statusActive' },
  { value: 'inactive', labelKey: 'crm.currency.statusInactive' },
] as const

// ── 税率 ──
export interface TaxRate {
  id: string
  code: string
  name: string
  rate: number
  status: 'active' | 'inactive'
  remark: string
  createdAt: string
  updatedAt: string
}

export const TAX_RATE_STATUS_OPTIONS = [
  { value: 'active', labelKey: 'crm.taxRate.statusActive' },
  { value: 'inactive', labelKey: 'crm.taxRate.statusInactive' },
] as const

// ── 销售订单 ──
export type SalesOrderStatus = 'draft' | 'confirmed' | 'shipped' | 'completed' | 'cancelled'

export interface SalesOrderItem {
  id: string
  orderId: string
  productName: string
  spec: string
  price: number
  quantity: number
  taxRateCode: string
  taxRate: number
  amount: number
  taxAmount: number
  totalAmount: number
}

export interface SalesOrder {
  id: string
  orderNo: string
  customerId: string
  customerName: string
  salesPersonName: string
  currencyCode: string
  currencySymbol: string
  paymentTerms: string
  deliveryDate: string
  status: SalesOrderStatus
  items: SalesOrderItem[]
  subtotalAmount: number
  taxAmount: number
  totalAmount: number
  remark: string
  createdAt: string
  updatedAt: string
}

export const SALES_ORDER_STATUS_OPTIONS = [
  { value: 'draft', labelKey: 'crm.salesOrder.statusDraft' },
  { value: 'confirmed', labelKey: 'crm.salesOrder.statusConfirmed' },
  { value: 'shipped', labelKey: 'crm.salesOrder.statusShipped' },
  { value: 'completed', labelKey: 'crm.salesOrder.statusCompleted' },
  { value: 'cancelled', labelKey: 'crm.salesOrder.statusCancelled' },
] as const

// ── 采购订单 ──
export type PurchaseOrderStatus = 'draft' | 'confirmed' | 'received' | 'completed' | 'cancelled'

export interface PurchaseOrderItem {
  id: string
  orderId: string
  productName: string
  spec: string
  price: number
  quantity: number
  taxRateCode: string
  taxRate: number
  amount: number
  taxAmount: number
  totalAmount: number
}

export interface PurchaseOrder {
  id: string
  orderNo: string
  supplierId: string
  supplierName: string
  buyerName: string
  currencyCode: string
  currencySymbol: string
  paymentTerms: string
  deliveryDate: string
  status: PurchaseOrderStatus
  items: PurchaseOrderItem[]
  subtotalAmount: number
  taxAmount: number
  totalAmount: number
  remark: string
  createdAt: string
  updatedAt: string
}

export const PURCHASE_ORDER_STATUS_OPTIONS = [
  { value: 'draft', labelKey: 'crm.purchaseOrder.statusDraft' },
  { value: 'confirmed', labelKey: 'crm.purchaseOrder.statusConfirmed' },
  { value: 'received', labelKey: 'crm.purchaseOrder.statusReceived' },
 { value: 'completed', labelKey: 'crm.purchaseOrder.statusCompleted' },
 { value: 'cancelled', labelKey: 'crm.purchaseOrder.statusCancelled' },
] as const

 // ── 仓库 ──
 export interface Warehouse {
   id: string
   code: string
   name: string
   address: string
   manager: string
   phone: string
   status: 'active' | 'inactive'
   remark: string
   createdAt: string
   updatedAt: string
 }

 export const WAREHOUSE_STATUS_OPTIONS = [
   { value: 'active', labelKey: 'crm.warehouse.statusActive' },
   { value: 'inactive', labelKey: 'crm.warehouse.statusInactive' },
 ] as const

 // ── 库存 ──
 export interface Stock {
   id: string
   warehouseId: string
   warehouseName: string
   skuCode: string
   skuName: string
   spec: string
   unit: string
   available: number
   locked: number
   total: number
   minLimit: number
   maxLimit: number
   updatedAt: string
 }

 // ── 出入库流水 ──
 export type StockRecordType = 'in' | 'out'
 export type StockRecordSourceType = 'purchase_in' | 'sales_out' | 'mall_out' | 'check_adjust' | 'reversal_return'

 export interface StockRecord {
   id: string
   warehouseId: string
   warehouseName: string
   skuCode: string
   skuName: string
   spec: string
   unit: string
   type: StockRecordType
   sourceType: StockRecordSourceType
   sourceOrderNo: string
   quantity: number
   operator: string
   remark: string
   createdAt: string
 }

 export const STOCK_RECORD_SOURCE_OPTIONS = [
   { value: 'purchase_in', labelKey: 'crm.stockRecord.sourcePurchaseIn' },
   { value: 'sales_out', labelKey: 'crm.stockRecord.sourceSalesOut' },
   { value: 'mall_out', labelKey: 'crm.stockRecord.sourceMallOut' },
   { value: 'check_adjust', labelKey: 'crm.stockRecord.sourceCheckAdjust' },
   { value: 'reversal_return', labelKey: 'crm.stockRecord.sourceReversalReturn' },
 ] as const

 export const STOCK_RECORD_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   in: { label: 'crm.stockRecord.typeIn', type: 'success' },
   out: { label: 'crm.stockRecord.typeOut', type: 'warning' },
 }

 // ── 盘点 ──
 export type StockCheckStatus = 'draft' | 'counting' | 'completed'

 export interface StockCheckItem {
   id: string
   checkId: string
   skuCode: string
   skuName: string
   spec: string
   unit: string
   systemQty: number
   countedQty: number
   diff: number
 }

 export interface StockCheck {
   id: string
   checkNo: string
   warehouseId: string
   warehouseName: string
   checker: string
   checkDate: string
   status: StockCheckStatus
   remark: string
   items: StockCheckItem[]
   createdAt: string
   updatedAt: string
 }

 export const STOCK_CHECK_STATUS_OPTIONS = [
   { value: 'draft', labelKey: 'crm.stockCheck.statusDraft' },
   { value: 'counting', labelKey: 'crm.stockCheck.statusCounting' },
   { value: 'completed', labelKey: 'crm.stockCheck.statusCompleted' },
 ] as const

 // ── 库存预警 ──
 export type StockAlertType = 'low' | 'high'
 export type StockAlertStatus = 'pending' | 'resolved'

 export interface StockAlert {
   id: string
   warehouseId: string
   warehouseName: string
   skuCode: string
   skuName: string
   spec: string
   available: number
   minLimit: number
   maxLimit: number
   alertType: StockAlertType
   status: StockAlertStatus
   createdAt: string
   resolvedAt: string | null
   remark: string
 }

 export const STOCK_ALERT_TYPE_OPTIONS = [
   { value: 'low', labelKey: 'crm.stockAlert.typeLow' },
   { value: 'high', labelKey: 'crm.stockAlert.typeHigh' },
 ] as const

 export const STOCK_ALERT_STATUS_OPTIONS = [
   { value: 'pending', labelKey: 'crm.stockAlert.statusPending' },
   { value: 'resolved', labelKey: 'crm.stockAlert.statusResolved' },
 ] as const
