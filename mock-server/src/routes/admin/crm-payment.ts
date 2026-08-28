// src/routes/admin/crm-payment.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { PAYMENTS, type Payment } from '../../data/crm-finance.js'

export const adminCrmPaymentRouter = Router()

adminCrmPaymentRouter.get('/crm/payment/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const type = req.query.type as string | undefined
  const method = req.query.method as string | undefined
  const status = req.query.status as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...PAYMENTS]
  if (type) filtered = filtered.filter(p => p.type === type)
  if (method) filtered = filtered.filter(p => p.method === method)
  if (status) filtered = filtered.filter(p => p.status === status)
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(p =>
      p.paymentNo.toLowerCase().includes(kw) ||
      p.orderNo.toLowerCase().includes(kw) ||
      p.partyName.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmPaymentRouter.get('/crm/payment/:id', (req, res) => {
  const payment = PAYMENTS.find(p => p.id === req.params.id)
  if (!payment) { res.json(fail('payment not found', 404)); return }
  res.json(ok(payment))
})

adminCrmPaymentRouter.post('/crm/payment', (req, res) => {
  const body = req.body
  if (!body.partyName?.trim()) { res.json(fail('partyName required')); return }

  const payment: Payment = {
    id: guid(),
    paymentNo: body.paymentNo || `PAY-${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${String(PAYMENTS.length + 1).padStart(3, '0')}`,
    type: body.type || 'receipt',
    orderType: body.orderType || 'sales',
    orderNo: body.orderNo || '',
    partyName: body.partyName,
    amount: Number(body.amount) || 0,
    method: body.method || 'bank',
    status: body.status || 'draft',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  PAYMENTS.unshift(payment)
  res.json(ok({ id: payment.id }, 'created'))
})

adminCrmPaymentRouter.put('/crm/payment/:id', (req, res) => {
  const payment = PAYMENTS.find(p => p.id === req.params.id)
  if (!payment) { res.json(fail('payment not found', 404)); return }
  if (payment.status === 'voided') { res.json(fail('voided payment cannot be edited')); return }

  const body = req.body
  if (body.type !== undefined) payment.type = body.type
  if (body.orderType !== undefined) payment.orderType = body.orderType
  if (body.orderNo !== undefined) payment.orderNo = body.orderNo
  if (body.partyName) payment.partyName = body.partyName
  if (body.amount !== undefined) payment.amount = Number(body.amount)
  if (body.method !== undefined) payment.method = body.method
  if (body.remark !== undefined) payment.remark = body.remark
  payment.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmPaymentRouter.post('/crm/payment/:id/status', (req, res) => {
  const payment = PAYMENTS.find(p => p.id === req.params.id)
  if (!payment) { res.json(fail('payment not found', 404)); return }
  const newStatus = req.body.status as string
  if (!newStatus) { res.json(fail('status required')); return }
  payment.status = newStatus as Payment['status']
  payment.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmPaymentRouter.delete('/crm/payment/:id', (req, res) => {
  const idx = PAYMENTS.findIndex(p => p.id === req.params.id)
  if (idx === -1) { res.json(fail('payment not found', 404)); return }
  if (PAYMENTS[idx].status !== 'draft') { res.json(fail('only draft payment can be deleted')); return }
  PAYMENTS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
