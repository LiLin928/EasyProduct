// src/routes/app/cart.ts
import { Router } from 'express'
import { SKUS, SPUS } from '../../data/product.js'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'

export const appCartRouter = Router()

/** 购物车条目（内存态，按会员 ID 分组；重启即重置） */
export interface CartItem {
  id: string
  memberId: string
  spuId: string
  spuName: string
  mainImage: string
  skuId: string
  specValues: Record<string, string>
  price: number
  quantity: number
  stock: number
  selected: boolean
}

export const carts = new Map<string, CartItem[]>()

const getCart = (memberId: string): CartItem[] => carts.get(memberId) ?? []
const saveCart = (memberId: string, items: CartItem[]): void => {
  carts.set(memberId, items)
}

/** 构建带合计的购物车响应 */
function buildCartResponse(items: CartItem[]) {
  const selected = items.filter((i) => i.selected)
  const totalQuantity = selected.reduce((s, i) => s + i.quantity, 0)
  const totalAmount = Math.round(selected.reduce((s, i) => s + i.price * i.quantity, 0) * 100) / 100
  return { list: items, totalQuantity, totalAmount }
}

/** 获取购物车列表 */
appCartRouter.get('/', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  res.json(ok(buildCartResponse(getCart(memberId))))
})

/** 添加商品到购物车（同 SKU 合并数量） */
appCartRouter.post('/', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  const { skuId, quantity = 1 } = req.body as { skuId?: string; quantity?: number }
  if (!skuId || quantity < 1) return res.json(fail('参数错误'))
  const sku = SKUS.find((s) => s.id === skuId && s.status === 'active')
  if (!sku) return res.json(fail('SKU 不存在或已下架', 404))
  const spu = SPUS.find((p) => p.id === sku.spuId)
  if (!spu) return res.json(fail('商品不存在', 404))
  if (sku.stock < quantity) return res.json(fail('库存不足'))

  const items = getCart(memberId)
  const existing = items.find((i) => i.skuId === skuId)
  if (existing) {
    existing.quantity += quantity
    existing.selected = true
  } else {
    items.push({
      id: guid(), memberId, spuId: spu.id, spuName: spu.name, mainImage: spu.mainImage,
      skuId: sku.id, specValues: sku.specValues, price: sku.memberPrice, quantity, stock: sku.stock, selected: true,
    })
  }
  saveCart(memberId, items)
  res.json(ok(buildCartResponse(items), '已加入购物车'))
})

/** 更新购物车条目（数量/选中状态） */
appCartRouter.put('/:id', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  const item = getCart(memberId).find((i) => i.id === req.params.id)
  if (!item) return res.json(fail('条目不存在', 404))
  const { quantity, selected } = req.body as { quantity?: number; selected?: boolean }
  if (quantity !== undefined) {
    if (quantity < 1) return res.json(fail('数量至少为 1'))
    if (quantity > item.stock) return res.json(fail('超出库存'))
    item.quantity = quantity
  }
  if (selected !== undefined) item.selected = selected
  saveCart(memberId, getCart(memberId))
  res.json(ok(buildCartResponse(getCart(memberId))))
})

/** 清空购物车（放在 /:id 之前避免路由被参数吞掉） */
appCartRouter.delete('/clear', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  saveCart(memberId, [])
  res.json(ok(null, '购物车已清空'))
})

/** 删除购物车条目 */
appCartRouter.delete('/:id', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  const items = getCart(memberId)
  const idx = items.findIndex((i) => i.id === req.params.id)
  if (idx === -1) return res.json(fail('条目不存在', 404))
  items.splice(idx, 1)
  saveCart(memberId, items)
  res.json(ok(buildCartResponse(items), '已删除'))
})
