// src/data/crm-purchase.ts
// CRM 采购订单 seed：引用 crm.ts 的供应商/币种/税率

import { guid, isoTime } from '../helpers/id.js'
import { SUPPLIERS, CURRENCIES, TAX_RATES } from './crm.js'
import { WAREHOUSES } from './crm-inventory.js'

export interface PurchaseOrderItem {
  id: string
  orderId: string
  skuCode: string
  skuName: string
  productName: string
  spec: string
  price: number
  quantity: number
  taxRateCode: string
  taxRate: number
  amount: number
  taxAmount: number
  totalAmount: number
  warehouseId: string
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
  status: 'draft' | 'confirmed' | 'received' | 'completed' | 'cancelled'
  items: PurchaseOrderItem[]
  subtotalAmount: number
  taxAmount: number
  totalAmount: number
  remark: string
  createdAt: string
  updatedAt: string
}

function buildItem(
  skuCode: string,
  skuName: string,
  productName: string,
  spec: string,
  price: number,
  qty: number,
  taxRateCode: string,
  taxRate: number,
  orderId: string,
  warehouseId: string,
): PurchaseOrderItem {
  const amount = +(price * qty).toFixed(2)
  const taxAmount = +(amount * taxRate / 100).toFixed(2)
  return {
    id: guid(),
    orderId,
    skuCode,
    skuName,
    productName,
    spec,
    price,
    quantity: qty,
    taxRateCode,
    taxRate,
    amount,
    taxAmount,
    totalAmount: +(amount + taxAmount).toFixed(2),
    warehouseId,
  }
}

function buildOrder(
  idx: number,
  supplierIdx: number,
  buyer: string,
  paymentTerms: string,
  deliveryOffset: number,
  status: PurchaseOrder['status'],
  items: { skuCode: string; skuName: string; name: string; spec: string; price: number; qty: number; taxIdx: number }[],
  remark: string,
  createdOffset: number,
): PurchaseOrder {
  const sup = SUPPLIERS[supplierIdx]
  const cny = CURRENCIES.find(c => c.code === 'CNY') ?? CURRENCIES[0]
  const defaultWarehouseId = WAREHOUSES[0]?.id ?? ''
  const id = guid()
  const orderItems = items.map(it => {
    const tr = TAX_RATES[it.taxIdx]
    return buildItem(it.skuCode, it.skuName, it.name, it.spec, it.price, it.qty, tr.code, tr.rate, id, defaultWarehouseId)
  })
  const subtotal = +orderItems.reduce((s, i) => s + i.amount, 0).toFixed(2)
  const tax = +orderItems.reduce((s, i) => s + i.taxAmount, 0).toFixed(2)
  const total = +(subtotal + tax).toFixed(2)
  return {
    id,
    orderNo: `PO-2026-${String(idx).padStart(4, '0')}`,
    supplierId: sup.id,
    supplierName: sup.name,
    buyerName: buyer,
    currencyCode: cny.code,
    currencySymbol: cny.symbol,
    paymentTerms,
    deliveryDate: isoTime(deliveryOffset).slice(0, 10),
    status,
    items: orderItems,
    subtotalAmount: subtotal,
    taxAmount: tax,
    totalAmount: total,
    remark,
    createdAt: isoTime(createdOffset),
    updatedAt: isoTime(createdOffset < -1 ? -1 : 0),
  }
}

export const PURCHASE_ORDERS: PurchaseOrder[] = [
  buildOrder(1, 0, 'Wu Jing', 'Net 60', 10, 'confirmed', [
    { skuCode: 'SKU-001', skuName: '蓝牙耳机 Pro', name: 'Steel Bracket A', spec: 'Galvanized', price: 12, qty: 5000, taxIdx: 0 },
    { skuCode: 'SKU-002', skuName: 'USB-C 充电器 65W', name: 'Screw M4x20', spec: 'Stainless 304', price: 0.5, qty: 50000, taxIdx: 0 },
  ], 'Bulk hardware order', -12),
  buildOrder(2, 1, 'Wu Jing', 'Net 30', 7, 'received', [
    { skuCode: 'SKU-003', skuName: '无线键盘', name: 'Resistor 10K 0603', spec: '1%', price: 0.02, qty: 200000, taxIdx: 0 },
    { skuCode: 'SKU-004', skuName: '便携移动电源 20000mAh', name: 'Capacitor 100uF', spec: '16V', price: 0.15, qty: 50000, taxIdx: 0 },
    { skuCode: 'SKU-005', skuName: '蓝牙音箱 S3', name: 'IC MCU STM32', spec: 'LQFP64', price: 8.5, qty: 10000, taxIdx: 0 },
  ], 'Components restock', -18),
  buildOrder(3, 2, 'Xu Min', 'Net 30', 5, 'draft', [
    { skuCode: 'SKU-006', skuName: '纸箱 40x30x30', name: 'Carton Box 40x30x30', spec: '5-ply', price: 3.5, qty: 10000, taxIdx: 0 },
    { skuCode: 'SKU-007', skuName: '气泡膜 1m', name: 'Bubble Wrap 1m', spec: '100m roll', price: 25, qty: 200, taxIdx: 0 },
  ], '', -2),
  buildOrder(4, 0, 'Wu Jing', 'Prepaid', 0, 'completed', [
    { skuCode: 'SKU-008', skuName: '铝型材 40x40', name: 'Aluminum Profile 40x40', spec: '1m', price: 18, qty: 3000, taxIdx: 0 },
  ], 'Closed', -35),
  buildOrder(5, 1, 'Xu Min', 'Net 45', 14, 'cancelled', [
    { skuCode: 'SKU-009', skuName: '双面板 PCB', name: 'PCB Board Double', spec: 'FR4 1.6mm', price: 5, qty: 8000, taxIdx: 0 },
  ], 'Supplier price increased', -6),
]
