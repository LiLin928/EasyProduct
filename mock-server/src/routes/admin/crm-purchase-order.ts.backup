// src/routes/admin/crm-purchase-order.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { SUPPLIERS } from '../../data/crm.js'
import { PURCHASE_ORDERS, type PurchaseOrder, type PurchaseOrderItem } from '../../data/crm-purchase.js'

export const adminCrmPurchaseOrderRouter = Router()

// ── 供应商下拉 ──
adminCrmPurchaseOrderRouter.get('/crm/purchase-order/supplier/options', (_req, res) => {
  const options = SUPPLIERS
    .filter(s => s.status === 'active')
    .map(s => ({ id: s.id, name: s.name }))
  res.json(ok(options))
})

// ── 列表 ──
adminCrmPurchaseOrderRouter.get('/crm/purchase-order/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const orderNo = req.query.orderNo as string | undefined
  const supplierId = req.query.supplierId as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...PURCHASE_ORDERS]
  if (orderNo) filtered = filtered.filter(o => o.orderNo.toLowerCase().includes(orderNo.toLowerCase()))
  if (supplierId) filtered = filtered.filter(o => o.supplierId === supplierId)
  if (status) filtered = filtered.filter(o => o.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// ── 详情 ──
adminCrmPurchaseOrderRouter.get('/crm/purchase-order/:id', (req, res) => {
  const order = PURCHASE_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('purchase order not found', 404)); return }
  res.json(ok(order))
})

// ── 新建 ──
adminCrmPurchaseOrderRouter.post('/crm/purchase-order', (req, res) => {
  const body = req.body
  const sup = SUPPLIERS.find(s => s.id === body.supplierId)
  if (!sup) { res.json(fail('supplier not found', 404)); return }

  const id = guid()
  const items: PurchaseOrderItem[] = (body.items ?? []).map((it: PurchaseOrderItem) => {
    const amount = +(it.price * it.quantity).toFixed(2)
    const taxAmount = +(amount * it.taxRate / 100).toFixed(2)
    return {
      id: guid(),
      orderId: id,
      productName: it.productName,
      spec: it.spec || '',
      price: it.price,
      quantity: it.quantity,
      taxRateCode: it.taxRateCode,
      taxRate: it.taxRate,
      amount,
      taxAmount,
      totalAmount: +(amount + taxAmount).toFixed(2),
    }
  })
  const subtotal = +items.reduce((s, i) => s + i.amount, 0).toFixed(2)
  const tax = +items.reduce((s, i) => s + i.taxAmount, 0).toFixed(2)
  const total = +(subtotal + tax).toFixed(2)

  const order: PurchaseOrder = {
    id,
    orderNo: `PO-2026-${String(PURCHASE_ORDERS.length + 1).padStart(4, '0')}`,
    supplierId: sup.id,
    supplierName: sup.name,
    buyerName: body.buyerName || '',
    currencyCode: body.currencyCode || 'CNY',
    currencySymbol: body.currencySymbol || 'Y',
    paymentTerms: body.paymentTerms || 'Net 30',
    deliveryDate: body.deliveryDate || isoTime(10).slice(0, 10),
    status: 'draft',
    items,
    subtotalAmount: subtotal,
    taxAmount: tax,
    totalAmount: total,
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  PURCHASE_ORDERS.unshift(order)
  res.json(ok({ id: order.id }, 'created'))
})

// ── 更新（仅草稿可改） ──
adminCrmPurchaseOrderRouter.put('/crm/purchase-order/:id', (req, res) => {
  const order = PURCHASE_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('purchase order not found', 404)); return }
  if (order.status !== 'draft') { res.json(fail('only draft orders can be edited')); return }

  const body = req.body
  if (body.buyerName !== undefined) order.buyerName = body.buyerName
  if (body.paymentTerms !== undefined) order.paymentTerms = body.paymentTerms
  if (body.deliveryDate !== undefined) order.deliveryDate = body.deliveryDate
  if (body.remark !== undefined) order.remark = body.remark
  if (body.items !== undefined) {
    order.items = body.items.map((it: PurchaseOrderItem) => {
      const amount = +(it.price * it.quantity).toFixed(2)
      const taxAmount = +(amount * it.taxRate / 100).toFixed(2)
      return {
        ...it,
        id: it.id || guid(),
        orderId: order.id,
        amount,
        taxAmount,
        totalAmount: +(amount + taxAmount).toFixed(2),
      }
    })
    order.subtotalAmount = +order.items.reduce((s, i) => s + i.amount, 0).toFixed(2)
    order.taxAmount = +order.items.reduce((s, i) => s + i.taxAmount, 0).toFixed(2)
    order.totalAmount = +(order.subtotalAmount + order.taxAmount).toFixed(2)
  }
  order.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

// ── 状态变更 ──
adminCrmPurchaseOrderRouter.put('/crm/purchase-order/:id/status', (req, res) => {
  const order = PURCHASE_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('purchase order not found', 404)); return }

  const newStatus = req.body.status as PurchaseOrder['status']
  const valid: Record<PurchaseOrder['status'], PurchaseOrder['status'][]> = {
    draft: ['confirmed', 'cancelled'],
    confirmed: ['received', 'cancelled'],
    received: ['completed'],
    completed: [],
    cancelled: [],
  }
  if (!valid[order.status].includes(newStatus)) {
    res.json(fail(`cannot transition from ${order.status} to ${newStatus}`))
    return
  }
  order.status = newStatus
  order.updatedAt = isoTime()
  res.json(ok(null, 'status updated'))
})

// ── 删除（仅草稿可删） ──
adminCrmPurchaseOrderRouter.delete('/crm/purchase-order/:id', (req, res) => {
  const idx = PURCHASE_ORDERS.findIndex(o => o.id === req.params.id)
  if (idx === -1) { res.json(fail('purchase order not found', 404)); return }
  if (PURCHASE_ORDERS[idx].status !== 'draft') { res.json(fail('only draft orders can be deleted')); return }
  PURCHASE_ORDERS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
