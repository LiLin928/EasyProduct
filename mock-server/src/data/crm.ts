// src/data/crm.ts
// CRM 主数据 seed：客户/供应商/币种/税率

import { guid, isoTime } from '../helpers/id.js'

// ────────────── 客户 ──────────────
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

export const CUSTOMERS: Customer[] = [
  {
    id: guid(),
    code: 'C00001',
    name: 'Shenzhen TechGlow Electronics Co., Ltd.',
    type: 'b2b',
    source: 'inquiry',
    contactPerson: 'Zhang Wei',
    phone: '13800138001',
    email: 'zhangwei@techglow.cn',
    address: 'No. 88 Innovation Road, Nanshan District, Shenzhen',
    salesPersonName: 'Li Ming',
    creditLimit: 500000,
    status: 'active',
    remark: 'VIP customer, annual contract',
    createdAt: isoTime(-30),
    updatedAt: isoTime(-5),
  },
  {
    id: guid(),
    code: 'C00002',
    name: 'Shanghai BrightFuture Trading Co., Ltd.',
    type: 'b2b',
    source: 'manual',
    contactPerson: 'Wang Fang',
    phone: '13900139002',
    email: 'wangfang@brightfuture.cn',
    address: 'No. 66 Nanjing Road, Jing\'an District, Shanghai',
    salesPersonName: 'Zhao Qiang',
    creditLimit: 200000,
    status: 'active',
    remark: '',
    createdAt: isoTime(-20),
    updatedAt: isoTime(-10),
  },
  {
    id: guid(),
    code: 'C00003',
    name: 'Walk-in Customer',
    type: 'retail',
    source: 'register',
    contactPerson: 'Chen Yu',
    phone: '13700137003',
    email: 'chenyu@email.com',
    address: 'No. 8 Renmin Road, Wuhan',
    salesPersonName: '',
    creditLimit: 0,
    status: 'active',
    remark: 'First order auto-created',
    createdAt: isoTime(-15),
    updatedAt: isoTime(-15),
  },
  {
    id: guid(),
    code: 'C00004',
    name: 'Guangzhou Global Source Import & Export Co., Ltd.',
    type: 'b2b',
    source: 'inquiry',
    contactPerson: 'Liu Yang',
    phone: '13600136004',
    email: 'liuyang@globalsource.cn',
    address: 'No. 99 Tianhe Road, Tianhe District, Guangzhou',
    salesPersonName: 'Li Ming',
    creditLimit: 800000,
    status: 'active',
    remark: 'Key account',
    createdAt: isoTime(-25),
    updatedAt: isoTime(-3),
  },
  {
    id: guid(),
    code: 'C00005',
    name: 'Chengdu Local Retail Customer',
    type: 'retail',
    source: 'manual',
    contactPerson: 'Hu Jing',
    phone: '13500135005',
    email: 'hujing@email.com',
    address: 'No. 18 Chunxi Road, Chengdu',
    salesPersonName: 'Zhao Qiang',
    creditLimit: 0,
    status: 'inactive',
    remark: 'Inactive for 6 months',
    createdAt: isoTime(-60),
    updatedAt: isoTime(-40),
  },
]

// ────────────── 供应商 ──────────────
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

export const SUPPLIERS: Supplier[] = [
  {
    id: guid(),
    code: 'S00001',
    name: 'Dongguan Precision Hardware Manufacturing Co., Ltd.',
    contactPerson: 'Sun Kai',
    phone: '15800158001',
    email: 'sunkai@precision-hw.cn',
    address: 'No. 28 Chang\'an Road, Dongguan',
    bankName: 'ICBC Dongguan Branch',
    bankAccount: '6222083602001234567',
    status: 'active',
    remark: 'Main hardware supplier',
    createdAt: isoTime(-40),
    updatedAt: isoTime(-12),
  },
  {
    id: guid(),
    code: 'S00002',
    name: 'Suzhou Electronic Components Co., Ltd.',
    contactPerson: 'Zhou Lin',
    phone: '15900159002',
    email: 'zhoulin@sz-electronics.cn',
    address: 'No. 100 Suzhou Industrial Park, Suzhou',
    bankName: 'Bank of China Suzhou Branch',
    bankAccount: '6217006100123456789',
    status: 'active',
    remark: '',
    createdAt: isoTime(-35),
    updatedAt: isoTime(-8),
  },
  {
    id: guid(),
    code: 'S00003',
    name: 'Yiwu Packaging Materials Factory',
    contactPerson: 'Ma Tao',
    phone: '15700157003',
    email: 'matao@yiwu-packaging.cn',
    address: 'No. 56 Binwang Road, Yiwu',
    bankName: 'ABC Yiwu Branch',
    bankAccount: '6228480123456789012',
    status: 'active',
    remark: 'Packaging supplier',
    createdAt: isoTime(-30),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    code: 'S00004',
    name: 'Ningbo Logistics Equipment Co., Ltd.',
    contactPerson: 'Guo Hui',
    phone: '15600156004',
    email: 'guohui@nb-logistics.cn',
    address: 'No. 77 Beilu Port, Ningbo',
    bankName: 'CCB Ningbo Branch',
    bankAccount: '6217001210098765432',
    status: 'inactive',
    remark: 'Contract ended',
    createdAt: isoTime(-50),
    updatedAt: isoTime(-30),
  },
]

// ────────────── 币种 ──────────────
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

export const CURRENCIES: Currency[] = [
  {
    id: guid(),
    code: 'CNY',
    name: 'Chinese Yuan',
    symbol: 'Y',
    exchangeRate: 1,
    isDefault: true,
    status: 'active',
    createdAt: isoTime(-90),
    updatedAt: isoTime(-90),
  },
  {
    id: guid(),
    code: 'USD',
    name: 'US Dollar',
    symbol: '$',
    exchangeRate: 7.25,
    isDefault: false,
    status: 'active',
    createdAt: isoTime(-90),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    code: 'EUR',
    name: 'Euro',
    symbol: 'EUR',
    exchangeRate: 7.85,
    isDefault: false,
    status: 'active',
    createdAt: isoTime(-90),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    code: 'HKD',
    name: 'Hong Kong Dollar',
    symbol: 'HK$',
    exchangeRate: 0.93,
    isDefault: false,
    status: 'active',
    createdAt: isoTime(-60),
    updatedAt: isoTime(-5),
  },
]

// ────────────── 税率 ──────────────
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

export const TAX_RATES: TaxRate[] = [
  {
    id: guid(),
    code: 'T13',
    name: 'VAT 13%',
    rate: 13,
    status: 'active',
    remark: 'Standard rate for goods',
    createdAt: isoTime(-90),
    updatedAt: isoTime(-90),
  },
  {
    id: guid(),
    code: 'T9',
    name: 'VAT 9%',
    rate: 9,
    status: 'active',
    remark: 'Reduced rate for transport etc.',
    createdAt: isoTime(-90),
    updatedAt: isoTime(-90),
  },
  {
    id: guid(),
    code: 'T6',
    name: 'VAT 6%',
    rate: 6,
    status: 'active',
    remark: 'Reduced rate for services',
    createdAt: isoTime(-90),
    updatedAt: isoTime(-90),
  },
  {
    id: guid(),
    code: 'T0',
    name: 'VAT 0%',
    rate: 0,
    status: 'active',
    remark: 'Export / tax exempt',
    createdAt: isoTime(-90),
    updatedAt: isoTime(-90),
  },
  {
    id: guid(),
    code: 'T3',
    name: 'VAT 3% (Small Scale)',
    rate: 3,
    status: 'active',
    remark: 'Small-scale taxpayer levy rate',
    createdAt: isoTime(-60),
    updatedAt: isoTime(-60),
  },
]
