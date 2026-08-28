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
