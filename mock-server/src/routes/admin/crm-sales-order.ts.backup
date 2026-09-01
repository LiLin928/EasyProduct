// src/routes/admin/crm-sales-order.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { CUSTOMERS } from '../../data/crm.js'
import { SALES_ORDERS, type SalesOrder, type SalesOrderItem } from '../../data/crm-sales.js'

export const adminCrmSalesOrderRouter = Router()

// ── 客户下拉 ──
adminCrmSalesOrderRouter.get('/crm/sales-order/customer/options', (_req, res) => {
  const options = CUSTOMERS
    .filter(c => c.status === 'active')
    .map(c => ({ id: c.id, name: c.name }))
  res.json(ok(options))
})

// ── 列表 ──
adminCrmSalesOrderRouter.get('/crm/sales-order/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const orderNo = req.query.orderNo as string | undefined
  const customerId = req.query.customerId as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...SALES_ORDERS]
  if (orderNo) filtered = filtered.filter(o => o.orderNo.toLowerCase().includes(orderNo.toLowerCase()))
  if (customerId) filtered = filtered.filter(o => o.customerId === customerId)
  if (status) filtered = filtered.filter(o => o.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// ── 详情 ──
adminCrmSalesOrderRouter.get('/crm/sales-order/:id', (req, res) => {
  const order = SALES_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('sales order not found', 404)); return }
  res.json(ok(order))
})

// ── 新建 ──
adminCrmSalesOrderRouter.post('/crm/sales-order', (req, res) => {
  const body = req.body
  const cust = CUSTOMERS.find(c => c.id === body.customerId)
  if (!cust) { res.json(fail('customer not found', 404)); return }

  const id = guid()
  const items: SalesOrderItem[] = (body.items ?? []).map((it: SalesOrderItem) => {
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

  const order: SalesOrder = {
    id,
    orderNo: `SO-2026-${String(SALES_ORDERS.length + 1).padStart(4, '0')}`,
    customerId: cust.id,
    customerName: cust.name,
    salesPersonName: body.salesPersonName || cust.salesPersonName || '',
    currencyCode: body.currencyCode || 'CNY',
    currencySymbol: body.currencySymbol || 'Y',
    paymentTerms: body.paymentTerms || 'Net 30',
    deliveryDate: body.deliveryDate || isoTime(7).slice(0, 10),
    status: 'draft',
    items,
    subtotalAmount: subtotal,
    taxAmount: tax,
    totalAmount: total,
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  SALES_ORDERS.unshift(order)
  res.json(ok({ id: order.id }, 'created'))
})

// ── 更新（仅草稿可改） ──
adminCrmSalesOrderRouter.put('/crm/sales-order/:id', (req, res) => {
  const order = SALES_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('sales order not found', 404)); return }
  if (order.status !== 'draft') { res.json(fail('only draft orders can be edited')); return }

  const body = req.body
  if (body.salesPersonName !== undefined) order.salesPersonName = body.salesPersonName
  if (body.paymentTerms !== undefined) order.paymentTerms = body.paymentTerms
  if (body.deliveryDate !== undefined) order.deliveryDate = body.deliveryDate
  if (body.remark !== undefined) order.remark = body.remark
  if (body.items !== undefined) {
    order.items = body.items.map((it: SalesOrderItem) => {
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
adminCrmSalesOrderRouter.put('/crm/sales-order/:id/status', (req, res) => {
  const order = SALES_ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('sales order not found', 404)); return }

  const newStatus = req.body.status as SalesOrder['status']
  const valid: Record<SalesOrder['status'], SalesOrder['status'][]> = {
    draft: ['confirmed', 'cancelled'],
    confirmed: ['shipped', 'cancelled'],
    shipped: ['completed'],
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
adminCrmSalesOrderRouter.delete('/crm/sales-order/:id', (req, res) => {
  const idx = SALES_ORDERS.findIndex(o => o.id === req.params.id)
  if (idx === -1) { res.json(fail('sales order not found', 404)); return }
  if (SALES_ORDERS[idx].status !== 'draft') { res.json(fail('only draft orders can be deleted')); return }
  SALES_ORDERS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
