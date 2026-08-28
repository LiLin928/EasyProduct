// src/data/crm-reversal.ts
// CRM 冲销 seed：统一红冲（商城退款/销售退货/采购退货/单据作废）

import { guid, isoTime } from '../helpers/id.js'

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
  type: 'mall_refund' | 'sales_return' | 'purchase_return' | 'document_void'
  sourceOrderType: string
  sourceOrderNo: string
  partyName: string
  amount: number
  reason: string
  operator: string
  status: 'draft' | 'submitted' | 'approved' | 'rejected' | 'executed'
  items: ReversalItem[]
  createdAt: string
  updatedAt: string
}

const mkItem = (reversalId: string, idx: number, skuCode: string, skuName: string, qty: number, amt: number, reason: string): ReversalItem => ({
  id: guid(),
  reversalId,
  skuCode,
  skuName,
  spec: 'standard',
  unit: 'pcs',
  quantity: qty,
  amount: amt,
  reason,
})

export const REVERSALS: Reversal[] = [
  {
    id: guid(),
    reversalNo: 'RV-20260820-001',
    type: 'mall_refund',
    sourceOrderType: 'mall_order',
    sourceOrderNo: 'MO-20260815-032',
    partyName: 'Zhang Wei',
    amount: 299,
    reason: 'Customer requested refund - product defective',
    operator: 'Li Mei',
    status: 'executed',
    items: [
      mkItem('', 1, 'SKU-001', 'Wireless Mouse', 1, 299, 'Defective on arrival'),
    ],
    createdAt: isoTime(-8),
    updatedAt: isoTime(-5),
  },
  {
    id: guid(),
    reversalNo: 'RV-20260822-002',
    type: 'sales_return',
    sourceOrderType: 'sales_order',
    sourceOrderNo: 'SO-20260820-015',
    partyName: 'Shanghai Tech Co Ltd',
    amount: 5600,
    reason: 'Wrong model shipped - customer return',
    operator: 'Wang Jun',
    status: 'approved',
    items: [
      mkItem('', 1, 'SKU-102', 'Laptop Stand', 20, 2800, 'Wrong model'),
      mkItem('', 2, 'SKU-103', 'USB Hub', 20, 2800, 'Wrong model'),
    ],
    createdAt: isoTime(-6),
    updatedAt: isoTime(-3),
  },
  {
    id: guid(),
    reversalNo: 'RV-20260824-003',
    type: 'purchase_return',
    sourceOrderType: 'purchase_order',
    sourceOrderNo: 'PO-20260820-008',
    partyName: 'Shenzhen Electronics Ltd',
    amount: 12000,
    reason: 'Quality issue - batch returned to supplier',
    operator: 'Chen Hao',
    status: 'submitted',
    items: [
      mkItem('', 1, 'SKU-201', 'Power Supply Unit', 50, 6000, 'Quality defect'),
      mkItem('', 2, 'SKU-202', 'Circuit Board', 50, 6000, 'Quality defect'),
    ],
    createdAt: isoTime(-4),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    reversalNo: 'RV-20260826-004',
    type: 'document_void',
    sourceOrderType: 'invoice',
    sourceOrderNo: 'INV-20260825-012',
    partyName: 'Beijing Trading Co',
    amount: 8900,
    reason: 'Invoice issued in error - void required',
    operator: 'Li Mei',
    status: 'rejected',
    items: [],
    createdAt: isoTime(-2),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    reversalNo: 'RV-20260827-005',
    type: 'sales_return',
    sourceOrderType: 'sales_order',
    sourceOrderNo: 'SO-20260825-021',
    partyName: 'Guangzhou Retail Group',
    amount: 3200,
    reason: 'Partial return - expired warranty claim',
    operator: 'Wang Jun',
    status: 'draft',
    items: [
      mkItem('', 1, 'SKU-105', 'Bluetooth Speaker', 8, 3200, 'Expired warranty'),
    ],
    createdAt: isoTime(-1),
    updatedAt: isoTime(-1),
  },
]

// fix item.reversalId references after creation
REVERSALS.forEach((rv) => {
  rv.items.forEach((it) => { it.reversalId = rv.id })
})
