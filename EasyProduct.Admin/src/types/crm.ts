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
