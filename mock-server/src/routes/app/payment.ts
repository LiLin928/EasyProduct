// src/routes/app/payment.ts
import { Router } from 'express'
import { ORDERS, PAYMENTS } from '../../data/mall.js'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'

export const appPaymentRouter = Router()

/**
 * 模拟支付（mock-guidelines 11.4：仅成功/失败两种报文，不接真实微信支付）
 * POST /api/app/payment/pay
 * body: { orderId: string, result?: 'success' | 'failed' }
 */
appPaymentRouter.post('/pay', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  const { orderId, result = 'success' } = req.body as { orderId?: string; result?: 'success' | 'failed' }
  if (!orderId) return res.json(fail('orderId 不能为空'))
  const order = ORDERS.find((o) => o.id === orderId && o.memberId === memberId)
  if (!order) return res.json(fail('订单不存在', 404))
  if (order.status !== 'pending') return res.json(fail('订单状态不允许支付'))

  if (result === 'failed') {
    PAYMENTS.push({
      id: guid(), orderId: order.id, orderNo: order.orderNo, amount: order.payAmount,
      method: 'wechat', status: 'failed', transactionId: guid().replace(/-/g, '').toUpperCase().slice(0, 32),
      paidAt: '', createdAt: new Date().toISOString(),
    })
    return res.json(ok({ status: 'failed', orderId: order.id, orderNo: order.orderNo }, '支付失败（mock）'))
  }

  order.status = 'paid'
  order.paymentMethod = 'wechat'
  order.updatedAt = new Date().toISOString()
  const transactionId = guid().replace(/-/g, '').toUpperCase().slice(0, 32)
  PAYMENTS.push({
    id: guid(), orderId: order.id, orderNo: order.orderNo, amount: order.payAmount,
    method: 'wechat', status: 'success', transactionId,
    paidAt: new Date().toISOString(), createdAt: new Date().toISOString(),
  })
  res.json(ok({ status: 'success', orderId: order.id, orderNo: order.orderNo, transactionId, paidAt: order.updatedAt }, '支付成功'))
})
