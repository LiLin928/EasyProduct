// src/data/crm-sales.ts
// CRM 销售订单 seed：引用 crm.ts 的客户/币种/税率

import { guid, isoTime } from '../helpers/id.js'
import { CUSTOMERS, CURRENCIES, TAX_RATES } from './crm.js'

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
  status: 'draft' | 'confirmed' | 'shipped' | 'completed' | 'cancelled'
  items: SalesOrderItem[]
  subtotalAmount: number
  taxAmount: number
  totalAmount: number
  remark: string
  createdAt: string
  updatedAt: string
}

function buildItem(
  productName: string,
  spec: string,
  price: number,
  qty: number,
  taxRateCode: string,
  taxRate: number,
  orderId: string,
): SalesOrderItem {
  const amount = +(price * qty).toFixed(2)
  const taxAmount = +(amount * taxRate / 100).toFixed(2)
  return {
    id: guid(),
    orderId,
    productName,
    spec,
    price,
    quantity: qty,
    taxRateCode,
    taxRate,
    amount,
    taxAmount,
    totalAmount: +(amount + taxAmount).toFixed(2),
  }
}

function buildOrder(
  idx: number,
  customerIdx: number,
  salesPerson: string,
  paymentTerms: string,
  deliveryOffset: number,
  status: SalesOrder['status'],
  items: { name: string; spec: string; price: number; qty: number; taxIdx: number }[],
  remark: string,
  createdOffset: number,
): SalesOrder {
  const cust = CUSTOMERS[customerIdx]
  const cny = CURRENCIES.find(c => c.code === 'CNY') ?? CURRENCIES[0]
  const id = guid()
  const orderItems = items.map(it => {
    const tr = TAX_RATES[it.taxIdx]
    return buildItem(it.name, it.spec, it.price, it.qty, tr.code, tr.rate, id)
  })
  const subtotal = +orderItems.reduce((s, i) => s + i.amount, 0).toFixed(2)
  const tax = +orderItems.reduce((s, i) => s + i.taxAmount, 0).toFixed(2)
  const total = +(subtotal + tax).toFixed(2)
  return {
    id,
    orderNo: `SO-2026-${String(idx).padStart(4, '0')}`,
    customerId: cust.id,
    customerName: cust.name,
    salesPersonName: salesPerson,
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

export const SALES_ORDERS: SalesOrder[] = [
  buildOrder(1, 0, 'Li Ming', 'Net 30', 7, 'confirmed', [
    { name: 'Bluetooth Headset X1', spec: 'Black', price: 85, qty: 200, taxIdx: 0 },
    { name: 'USB-C Charger 65W', spec: 'GaN', price: 45, qty: 500, taxIdx: 0 },
  ], 'Urgent delivery, partial allowed', -10),
  buildOrder(2, 3, 'Li Ming', 'Net 45', 14, 'shipped', [
    { name: 'Wireless Mouse M2', spec: '2.4G', price: 28, qty: 1000, taxIdx: 0 },
    { name: 'Mechanical Keyboard K8', spec: 'RGB Blue', price: 120, qty: 300, taxIdx: 0 },
    { name: 'USB Hub 4-Port', spec: 'Aluminum', price: 18, qty: 800, taxIdx: 0 },
  ], 'Key account PO', -15),
  buildOrder(3, 1, 'Zhao Qiang', 'Net 30', 5, 'draft', [
    { name: 'LED Strip 5m', spec: 'RGB 5050', price: 35, qty: 600, taxIdx: 0 },
  ], '', -3),
  buildOrder(4, 0, 'Li Ming', 'Prepaid', 0, 'completed', [
    { name: 'Power Bank 10000mAh', spec: 'PD 22.5W', price: 55, qty: 400, taxIdx: 0 },
    { name: 'Phone Stand Aluminum', spec: 'Foldable', price: 12, qty: 1000, taxIdx: 0 },
  ], 'Closed', -30),
  buildOrder(5, 2, '', 'COD', 3, 'cancelled', [
    { name: 'Bluetooth Speaker S3', spec: 'IPX7', price: 65, qty: 150, taxIdx: 0 },
  ], 'Customer cancelled', -8),
]
