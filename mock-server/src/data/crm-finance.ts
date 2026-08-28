// src/data/crm-finance.ts
// CRM 财务 seed：发票/收付款/应收应付/固定资产

import { guid, isoTime } from '../helpers/id.js'

// ────────────── 发票 ──────────────
export interface Invoice {
  id: string
  invoiceNo: string
  type: 'output' | 'input'
  orderType: 'sales' | 'purchase'
  orderNo: string
  partyName: string
  amount: number
  taxRate: number
  taxAmount: number
  total: number
  issueDate: string
  status: 'draft' | 'issued' | 'voided'
  remark: string
  createdAt: string
  updatedAt: string
}

export const INVOICES: Invoice[] = [
  {
    id: guid(),
    invoiceNo: 'INV-20260820-001',
    type: 'output',
    orderType: 'sales',
    orderNo: 'SO-20260818-001',
    partyName: '深圳科技有限公司',
    amount: 10000,
    taxRate: 13,
    taxAmount: 1300,
    total: 11300,
    issueDate: isoTime(-8).slice(0, 10),
    status: 'issued',
    remark: '销售开票',
    createdAt: isoTime(-8),
    updatedAt: isoTime(-7),
  },
  {
    id: guid(),
    invoiceNo: 'INV-20260822-002',
    type: 'output',
    orderType: 'sales',
    orderNo: 'SO-20260820-003',
    partyName: '上海商贸集团',
    amount: 25000,
    taxRate: 13,
    taxAmount: 3250,
    total: 28250,
    issueDate: isoTime(-6).slice(0, 10),
    status: 'issued',
    remark: '销售开票',
    createdAt: isoTime(-6),
    updatedAt: isoTime(-5),
  },
  {
    id: guid(),
    invoiceNo: 'INV-20260825-003',
    type: 'input',
    orderType: 'purchase',
    orderNo: 'PO-20260824-002',
    partyName: '广州供应商',
    amount: 8000,
    taxRate: 13,
    taxAmount: 1040,
    total: 9040,
    issueDate: isoTime(-3).slice(0, 10),
    status: 'issued',
    remark: '采购进项',
    createdAt: isoTime(-3),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    invoiceNo: 'INV-20260828-004',
    type: 'output',
    orderType: 'sales',
    orderNo: 'SO-20260827-005',
    partyName: '北京电商科技',
    amount: 15000,
    taxRate: 13,
    taxAmount: 1950,
    total: 16950,
    issueDate: isoTime(0).slice(0, 10),
    status: 'draft',
    remark: '待开具',
    createdAt: isoTime(0),
    updatedAt: isoTime(0),
  },
  {
    id: guid(),
    invoiceNo: 'INV-20260815-005',
    type: 'output',
    orderType: 'sales',
    orderNo: 'SO-20260812-008',
    partyName: '深圳科技有限公司',
    amount: 5000,
    taxRate: 13,
    taxAmount: 650,
    total: 5650,
    issueDate: isoTime(-13).slice(0, 10),
    status: 'voided',
    remark: '作废-信息有误',
    createdAt: isoTime(-13),
    updatedAt: isoTime(-10),
  },
]

// ────────────── 收付款 ──────────────
export interface Payment {
  id: string
  paymentNo: string
  type: 'receipt' | 'payment'
  orderType: 'sales' | 'purchase'
  orderNo: string
  partyName: string
  amount: number
  method: 'cash' | 'bank' | 'wechat'
  status: 'draft' | 'confirmed' | 'voided'
  remark: string
  createdAt: string
  updatedAt: string
}

export const PAYMENTS: Payment[] = [
  {
    id: guid(),
    paymentNo: 'PAY-20260820-001',
    type: 'receipt',
    orderType: 'sales',
    orderNo: 'SO-20260818-001',
    partyName: '深圳科技有限公司',
    amount: 11300,
    method: 'bank',
    status: 'confirmed',
    remark: '银行转账收款',
    createdAt: isoTime(-7),
    updatedAt: isoTime(-7),
  },
  {
    id: guid(),
    paymentNo: 'PAY-20260823-002',
    type: 'receipt',
    orderType: 'sales',
    orderNo: 'SO-20260820-003',
    partyName: '上海商贸集团',
    amount: 15000,
    method: 'bank',
    status: 'confirmed',
    remark: '部分收款',
    createdAt: isoTime(-5),
    updatedAt: isoTime(-4),
  },
  {
    id: guid(),
    paymentNo: 'PAY-20260825-003',
    type: 'payment',
    orderType: 'purchase',
    orderNo: 'PO-20260824-002',
    partyName: '广州供应商',
    amount: 9040,
    method: 'bank',
    status: 'confirmed',
    remark: '采购付款',
    createdAt: isoTime(-3),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    paymentNo: 'PAY-20260828-004',
    type: 'receipt',
    orderType: 'sales',
    orderNo: 'SO-20260827-005',
    partyName: '北京电商科技',
    amount: 8000,
    method: 'wechat',
    status: 'draft',
    remark: '微信收款待确认',
    createdAt: isoTime(0),
    updatedAt: isoTime(0),
  },
  {
    id: guid(),
    paymentNo: 'PAY-20260818-005',
    type: 'payment',
    orderType: 'purchase',
    orderNo: 'PO-20260815-006',
    partyName: '东莞原料厂',
    amount: 6000,
    method: 'cash',
    status: 'voided',
    remark: '作废-重复录入',
    createdAt: isoTime(-10),
    updatedAt: isoTime(-8),
  },
]

// ────────────── 应收应付台账 ──────────────
export interface Arap {
  id: string
  orderType: 'sales' | 'purchase'
  orderNo: string
  partyName: string
  receivable: number
  received: number
  balance: number
  aging: '0-30' | '31-60' | '61-90' | '90+'
  status: 'settled' | 'unsettled'
  createdAt: string
}

export const ARAPS: Arap[] = [
  {
    id: guid(),
    orderType: 'sales',
    orderNo: 'SO-20260818-001',
    partyName: '深圳科技有限公司',
    receivable: 11300,
    received: 11300,
    balance: 0,
    aging: '0-30',
    status: 'settled',
    createdAt: isoTime(-10),
  },
  {
    id: guid(),
    orderType: 'sales',
    orderNo: 'SO-20260820-003',
    partyName: '上海商贸集团',
    receivable: 28250,
    received: 15000,
    balance: 13250,
    aging: '0-30',
    status: 'unsettled',
    createdAt: isoTime(-8),
  },
  {
    id: guid(),
    orderType: 'sales',
    orderNo: 'SO-20260812-008',
    partyName: '深圳科技有限公司',
    receivable: 5650,
    received: 0,
    balance: 5650,
    aging: '31-60',
    status: 'unsettled',
    createdAt: isoTime(-16),
  },
  {
    id: guid(),
    orderType: 'purchase',
    orderNo: 'PO-20260824-002',
    partyName: '广州供应商',
    receivable: 9040,
    received: 9040,
    balance: 0,
    aging: '0-30',
    status: 'settled',
    createdAt: isoTime(-4),
  },
  {
    id: guid(),
    orderType: 'purchase',
    orderNo: 'PO-20260815-006',
    partyName: '东莞原料厂',
    receivable: 12000,
    received: 6000,
    balance: 6000,
    aging: '31-60',
    status: 'unsettled',
    createdAt: isoTime(-13),
  },
  {
    id: guid(),
    orderType: 'sales',
    orderNo: 'SO-20260728-012',
    partyName: '杭州网络科技',
    receivable: 18000,
    received: 5000,
    balance: 13000,
    aging: '61-90',
    status: 'unsettled',
    createdAt: isoTime(-31),
  },
  {
    id: guid(),
    orderType: 'sales',
    orderNo: 'SO-20260620-015',
    partyName: '成都传媒集团',
    receivable: 9600,
    received: 0,
    balance: 9600,
    aging: '90+',
    status: 'unsettled',
    createdAt: isoTime(-69),
  },
]

// ────────────── 固定资产 ──────────────
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
  status: 'active' | 'scrapped'
  remark: string
  createdAt: string
  updatedAt: string
}

export const FIXED_ASSETS: FixedAsset[] = [
  {
    id: guid(),
    assetNo: 'FA-0001',
    name: '联想 ThinkPad X1 Carbon 笔记本',
    category: 'electronic',
    originalValue: 12000,
    purchaseDate: isoTime(-365).slice(0, 10),
    depreciationMethod: 'straight-line',
    salvageValue: 1200,
    usefulYears: 3,
    currentValue: 8400,
    status: 'active',
    remark: '研发部使用',
    createdAt: isoTime(-365),
    updatedAt: isoTime(-30),
  },
  {
    id: guid(),
    assetNo: 'FA-0002',
    name: '戴尔 PowerEdge R750 服务器',
    category: 'it',
    originalValue: 45000,
    purchaseDate: isoTime(-200).slice(0, 10),
    depreciationMethod: 'straight-line',
    salvageValue: 4500,
    usefulYears: 5,
    currentValue: 27000,
    status: 'active',
    remark: '机房部署',
    createdAt: isoTime(-200),
    updatedAt: isoTime(-30),
  },
  {
    id: guid(),
    assetNo: 'FA-0003',
    name: '办公桌椅组合（6 套）',
    category: 'furniture',
    originalValue: 18000,
    purchaseDate: isoTime(-500).slice(0, 10),
    depreciationMethod: 'straight-line',
    salvageValue: 1800,
    usefulYears: 5,
    currentValue: 9000,
    status: 'active',
    remark: '办公区配置',
    createdAt: isoTime(-500),
    updatedAt: isoTime(-30),
  },
  {
    id: guid(),
    assetNo: 'FA-0004',
    name: '佳能 imageRUNNER 复印机',
    category: 'electronic',
    originalValue: 8000,
    purchaseDate: isoTime(-730).slice(0, 10),
    depreciationMethod: 'straight-line',
    salvageValue: 800,
    usefulYears: 5,
    currentValue: 0,
    status: 'scrapped',
    remark: '已折旧完毕并报废',
    createdAt: isoTime(-730),
    updatedAt: isoTime(-60),
  },
]

// ────────────── 资产折旧记录 ──────────────
export interface AssetDepreciation {
  id: string
  assetId: string
  period: string
  depreciationAmount: number
  accumulatedDepreciation: number
  currentValue: number
  createdAt: string
}

// 为每个在用资产生成折旧记录
function generateDepreciationRecords(asset: FixedAsset): AssetDepreciation[] {
  const records: AssetDepreciation[] = []
  const monthlyDep = (asset.originalValue - asset.salvageValue) / (asset.usefulYears * 12)
  let accumulated = 0
  let currentVal = asset.originalValue
  const startDate = new Date(asset.purchaseDate)
  const now = new Date()
  let year = startDate.getFullYear()
  let month = startDate.getMonth()

  while (year < now.getFullYear() || (year === now.getFullYear() && month <= now.getMonth())) {
    accumulated += monthlyDep
    currentVal = asset.originalValue - accumulated
    records.push({
      id: guid(),
      assetId: asset.id,
      period: `${year}-${String(month + 1).padStart(2, '0')}`,
      depreciationAmount: Math.round(monthlyDep * 100) / 100,
      accumulatedDepreciation: Math.round(accumulated * 100) / 100,
      currentValue: Math.round(currentVal * 100) / 100,
      createdAt: isoTime(0),
    })
    month++
    if (month >= 12) {
      month = 0
      year++
    }
  }
  return records
}

export const ASSET_DEPRECIATIONS: AssetDepreciation[] = [
  ...generateDepreciationRecords(FIXED_ASSETS[0]),
  ...generateDepreciationRecords(FIXED_ASSETS[1]),
  ...generateDepreciationRecords(FIXED_ASSETS[2]),
]
