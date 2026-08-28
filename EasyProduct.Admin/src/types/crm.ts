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

// ── 发票 ──
export type InvoiceType = 'output' | 'input'
export type InvoiceStatus = 'draft' | 'issued' | 'voided'

export interface Invoice {
  id: string
  invoiceNo: string
  type: InvoiceType
  orderType: 'sales' | 'purchase'
  orderNo: string
  partyName: string
  amount: number
  taxRate: number
  taxAmount: number
  total: number
  issueDate: string
  status: InvoiceStatus
  remark: string
  createdAt: string
  updatedAt: string
}

export const INVOICE_TYPE_OPTIONS = [
  { value: 'output', labelKey: 'crm.invoice.typeOutput' },
  { value: 'input', labelKey: 'crm.invoice.typeInput' },
] as const

export const INVOICE_STATUS_OPTIONS = [
  { value: 'draft', labelKey: 'crm.invoice.statusDraft' },
  { value: 'issued', labelKey: 'crm.invoice.statusIssued' },
  { value: 'voided', labelKey: 'crm.invoice.statusVoided' },
] as const

// ── 收付款 ──
export type PaymentType = 'receipt' | 'payment'
export type PaymentMethod = 'cash' | 'bank' | 'wechat'
export type PaymentStatus = 'draft' | 'confirmed' | 'voided'

export interface Payment {
  id: string
  paymentNo: string
  type: PaymentType
  orderType: 'sales' | 'purchase'
  orderNo: string
  partyName: string
  amount: number
  method: PaymentMethod
  status: PaymentStatus
  remark: string
  createdAt: string
  updatedAt: string
}

export const PAYMENT_TYPE_OPTIONS = [
  { value: 'receipt', labelKey: 'crm.payment.typeReceipt' },
  { value: 'payment', labelKey: 'crm.payment.typePayment' },
] as const

export const PAYMENT_METHOD_OPTIONS = [
  { value: 'cash', labelKey: 'crm.payment.methodCash' },
  { value: 'bank', labelKey: 'crm.payment.methodBank' },
  { value: 'wechat', labelKey: 'crm.payment.methodWechat' },
] as const

export const PAYMENT_STATUS_OPTIONS = [
  { value: 'draft', labelKey: 'crm.payment.statusDraft' },
  { value: 'confirmed', labelKey: 'crm.payment.statusConfirmed' },
  { value: 'voided', labelKey: 'crm.payment.statusVoided' },
] as const

// ── 应收应付台账 ──
export type ArapAging = '0-30' | '31-60' | '61-90' | '90+'
export type ArapStatus = 'settled' | 'unsettled'

export interface Arap {
  id: string
  orderType: 'sales' | 'purchase'
  orderNo: string
  partyName: string
  receivable: number
  received: number
  balance: number
  aging: ArapAging
  status: ArapStatus
  createdAt: string
}

export interface ArapSummary {
  totalReceivable: number
  totalPayable: number
  totalBalance: number
}

export const ARAP_AGING_OPTIONS = [
  { value: '0-30', labelKey: 'crm.arap.aging0to30' },
  { value: '31-60', labelKey: 'crm.arap.aging31to60' },
  { value: '61-90', labelKey: 'crm.arap.aging61to90' },
  { value: '90+', labelKey: 'crm.arap.aging90plus' },
] as const

export const ARAP_STATUS_OPTIONS = [
  { value: 'settled', labelKey: 'crm.arap.statusSettled' },
  { value: 'unsettled', labelKey: 'crm.arap.statusUnsettled' },
] as const

// ── 固定资产 ──
export type FixedAssetStatus = 'active' | 'scrapped'

export interface FixedAsset {
  id: string
  assetNo: string
  name: string
  category: string
  originalValue: number
  purchaseDate: string
  depreciationMethod: 'straight-line'
  salvageValue: number
  usefulYears: number
  currentValue: number
  status: FixedAssetStatus
  remark: string
  createdAt: string
  updatedAt: string
}

export interface AssetDepreciation {
  id: string
  assetId: string
  period: string
  depreciationAmount: number
  accumulatedDepreciation: number
  currentValue: number
  createdAt: string
}

export const FIXED_ASSET_STATUS_OPTIONS = [
  { value: 'active', labelKey: 'crm.fixedAsset.statusActive' },
  { value: 'scrapped', labelKey: 'crm.fixedAsset.statusScrapped' },
] as const

export const ASSET_CATEGORY_OPTIONS = [
  { value: 'electronic', labelKey: 'crm.fixedAsset.categoryElectronic' },
  { value: 'it', labelKey: 'crm.fixedAsset.categoryIT' },
  { value: 'furniture', labelKey: 'crm.fixedAsset.categoryFurniture' },
] as const
// ── 冲销 ──
export type ReversalType = 'mall_refund' | 'sales_return' | 'purchase_return' | 'document_void'
export type ReversalStatus = 'draft' | 'submitted' | 'approved' | 'rejected' | 'executed'

export interface ReversalItem {
  id: string
  reversalId: string
  skuCode: string
  skuName: string
  spec: string
  unit: string
  quantity: number
  amount: number
  reason: string
}

export interface Reversal {
  id: string
  reversalNo: string
  type: ReversalType
  sourceOrderType: string
  sourceOrderNo: string
  partyName: string
  amount: number
  reason: string
  operator: string
  status: ReversalStatus
  items: ReversalItem[]
  createdAt: string
  updatedAt: string
}

export const REVERSAL_TYPE_OPTIONS = [
  { value: 'mall_refund', labelKey: 'crm.reversal.typeMallRefund' },
  { value: 'sales_return', labelKey: 'crm.reversal.typeSalesReturn' },
  { value: 'purchase_return', labelKey: 'crm.reversal.typePurchaseReturn' },
  { value: 'document_void', labelKey: 'crm.reversal.typeDocumentVoid' },
] as const

export const REVERSAL_STATUS_OPTIONS = [
  { value: 'draft', labelKey: 'crm.reversal.statusDraft' },
  { value: 'submitted', labelKey: 'crm.reversal.statusSubmitted' },
  { value: 'approved', labelKey: 'crm.reversal.statusApproved' },
  { value: 'rejected', labelKey: 'crm.reversal.statusRejected' },
  { value: 'executed', labelKey: 'crm.reversal.statusExecuted' },
] as const
