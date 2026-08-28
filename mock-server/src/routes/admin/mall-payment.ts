// src/routes/admin/mall-payment.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { PAYMENTS, type PaymentStatus } from '../../data/mall.js'

export const adminMallPaymentRouter = Router()

adminMallPaymentRouter.get('/mall/payment/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const orderNo = req.query.orderNo as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...PAYMENTS]
  if (orderNo) filtered = filtered.filter(p => p.orderNo.includes(orderNo))
  if (status) filtered = filtered.filter(p => p.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminMallPaymentRouter.get('/mall/payment/:id', (req, res) => {
  const payment = PAYMENTS.find(p => p.id === req.params.id)
  if (!payment) { res.json(fail('payment not found', 404)); return }
  res.json(ok(payment))
})

// Admin can refund a payment
adminMallPaymentRouter.put('/mall/payment/:id/refund', (req, res) => {
  const payment = PAYMENTS.find(p => p.id === req.params.id)
  if (!payment) { res.json(fail('payment not found', 404)); return }
  if (payment.status !== 'success') {
    res.json(fail('only successful payments can be refunded'))
    return
  }

  payment.status = 'refunded' as PaymentStatus
  res.json(ok(null, 'refunded'))
})
