// src/routes/app/order.ts
import { Router, type Request } from 'express'
import { MEMBERS, ORDERS, type Order, type OrderItem } from '../../data/mall.js'
import { SKUS, SPUS } from '../../data/product.js'
import { fail, ok, paginate } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'
import { code } from '../../helpers/id.js'
import { carts } from './cart.js'

export const appOrderRouter = Router()

const getMemberId = (req: Request): string | undefined => (req as any).member?.id

/** 从请求体解析下单条目，返回规格化后的条目或错误信息 */
function parseOrderItems(body: { items?: Array<{ skuId?: string; quantity?: number }> }): { items: OrderItem[]; error?: string } {
  if (!Array.isArray(body.items) || body.items.length === 0) return { items: [], error: '订单条目不能为空' }
  const items: OrderItem[] = []
  for (const raw of body.items) {
    if (!raw.skuId || !raw.quantity || raw.quantity < 1) return { items: [], error: 'SKU 或数量参数错误' }
    const sku = SKUS.find((s) => s.id === raw.skuId && s.status === 'active')
    if (!sku) return { items: [], error: `SKU ${raw.skuId} 不存在或已下架` }
    if (sku.stock < raw.quantity) return { items: [], error: '库存不足' }
    const spu = SPUS.find((p) => p.id === sku.spuId)
    if (!spu) return { items: [], error: '商品不存在' }
    items.push({
      id: guid(), orderId: '', spuId: spu.id, spuName: spu.name,
      specValues: Object.entries(sku.specValues).map(([k, v]) => `${k}: ${v}`).join('; '),
      price: sku.memberPrice, quantity: raw.quantity,
      subtotal: Math.round(sku.memberPrice * raw.quantity * 100) / 100,
    })
  }
  return { items }
}

/**
 * 创建订单（从购物车结算或直接购买；成功后自动从购物车移除已下单条目）
 * POST /api/app/orders
 */
appOrderRouter.post('/', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('未授权', 401))
  const member = MEMBERS.find((m) => m.id === memberId)
  if (!member) return res.json(fail('会员不存在', 401))

  const { items, error } = parseOrderItems(req.body as { items?: Array<{ skuId?: string; quantity?: number }> })
  if (error) return res.json(fail(error))

  const totalAmount = Math.round(items.reduce((s, it) => s + it.subtotal, 0) * 100) / 100
  const shippingFee = totalAmount >= 500 ? 0 : 10
  const payAmount = Math.round((totalAmount + shippingFee) * 100) / 100
  const orderId = guid()
  items.forEach((it) => { it.orderId = orderId })
  const order: Order = {
    id: orderId, orderNo: code('ORD'), memberId, memberName: member.nickname, memberPhone: member.phone,
    totalAmount, discountAmount: 0, pointsAmount: 0, shippingFee, payAmount, couponId: '', couponName: '',
    status: 'pending', paymentMethod: '', remark: (req.body as { remark?: string }).remark ?? '',
    items, createdAt: new Date().toISOString(), updatedAt: new Date().toISOString(),
  }
  ORDERS.push(order)

  // 从购物车移除已下单条目
  const cart = carts.get(memberId) ?? []
  const orderedSkuIds = new Set((req.body as { items?: Array<{ skuId?: string }> }).items?.map((i) => i.skuId) ?? [])
  carts.set(memberId, cart.filter((ci) => !orderedSkuIds.has(ci.skuId)))

  res.json(ok(order, '下单成功'))
})

/** 获取我的订单列表（分页 + 状态筛选） */
appOrderRouter.get('/', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('未授权', 401))
  const { pageIndex = '1', pageSize = '10', status } = req.query as Record<string, string | undefined>
  let mine = ORDERS.filter((o) => o.memberId === memberId)
  if (status) mine = mine.filter((o) => o.status === status)
  res.json(ok(paginate(mine, parseInt(pageIndex, 10), parseInt(pageSize, 10))))
})

/** 获取订单详情 */
appOrderRouter.get('/:id', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('未授权', 401))
  const order = ORDERS.find((o) => o.id === req.params.id && o.memberId === memberId)
  if (!order) return res.json(fail('订单不存在', 404))
  res.json(ok(order))
})

/** 取消待支付订单 */
appOrderRouter.post('/:id/cancel', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('未授权', 401))
  const order = ORDERS.find((o) => o.id === req.params.id && o.memberId === memberId)
  if (!order) return res.json(fail('订单不存在', 404))
  if (order.status !== 'pending') return res.json(fail('仅待支付订单可取消'))
  order.status = 'cancelled'
  order.updatedAt = new Date().toISOString()
  res.json(ok(order, '订单已取消'))
})
