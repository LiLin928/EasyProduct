// src/routes/admin/mall-order.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { isoTime } from '../../helpers/id.js'
import { ORDERS, MEMBERS, type OrderStatus } from '../../data/mall.js'

export const adminMallOrderRouter = Router()

adminMallOrderRouter.get('/mall/order/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const orderNo = req.query.orderNo as string | undefined
  const memberId = req.query.memberId as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...ORDERS]
  if (orderNo) filtered = filtered.filter(o => o.orderNo.includes(orderNo))
  if (memberId) filtered = filtered.filter(o => o.memberId === memberId)
  if (status) filtered = filtered.filter(o => o.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  const pageData = paginate(filtered, pageIndex, pageSize)
  // strip items for list view to keep response small
  pageData.list = pageData.list.map(o => ({ ...o, items: [] }))
  res.json(ok(pageData))
})

adminMallOrderRouter.get('/mall/order/:id', (req, res) => {
  const order = ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('order not found', 404)); return }
  res.json(ok(order))
})

// Order status state machine: pending->paid->shipped->completed; can cancel/refund
const STATUS_TRANSITIONS: Record<string, string[]> = {
  pending: ['paid', 'cancelled'],
  paid: ['shipped', 'refunded'],
  shipped: ['completed', 'refunded'],
  completed: ['refunded'],
  cancelled: [],
  refunded: [],
}

adminMallOrderRouter.put('/mall/order/:id/status', (req, res) => {
  const order = ORDERS.find(o => o.id === req.params.id)
  if (!order) { res.json(fail('order not found', 404)); return }

  const newStatus = req.body.status as OrderStatus
  if (!newStatus) { res.json(fail('status required')); return }

  const allowed = STATUS_TRANSITIONS[order.status] || []
  if (!allowed.includes(newStatus)) {
    res.json(fail(`cannot transition from ${order.status} to ${newStatus}`))
    return
  }

  order.status = newStatus
  order.updatedAt = isoTime()
  if (req.body.remark !== undefined) order.remark = req.body.remark

  // when paid, set payment method
  if (newStatus === 'paid' && !order.paymentMethod) {
    order.paymentMethod = req.body.paymentMethod || 'wechat'
  }

  res.json(ok(null, 'updated'))
})

adminMallOrderRouter.get('/mall/order/member/options', (_req, res) => {
  res.json(ok(MEMBERS.map(m => ({ id: m.id, name: m.nickname }))))
})

adminMallOrderRouter.get('/mall/order/stats', (_req, res) => {
  const stats = {
    total: ORDERS.length,
    pending: ORDERS.filter(o => o.status === 'pending').length,
    paid: ORDERS.filter(o => o.status === 'paid').length,
    shipped: ORDERS.filter(o => o.status === 'shipped').length,
    completed: ORDERS.filter(o => o.status === 'completed').length,
    cancelled: ORDERS.filter(o => o.status === 'cancelled').length,
    refunded: ORDERS.filter(o => o.status === 'refunded').length,
    totalAmount: ORDERS.reduce((s, o) => s + o.payAmount, 0),
  }
  res.json(ok(stats))
})
