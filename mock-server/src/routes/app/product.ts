// src/routes/app/product.ts
import { Router } from 'express'
import { CATEGORIES, CHANNELS, SKUS, SPUS } from '../../data/product.js'
import { fail, ok, paginate } from '../../helpers/envelope.js'

export const appProductRouter = Router()

/** 小程序渠道已上架且 SPU 启用的商品列表（含 SKU 数量与分类名） */
const miniappSpus = SPUS.filter((spu) => {
  const ch = CHANNELS.find((c) => c.spuId === spu.id && c.channel === 'miniapp')
  return ch?.published === true && spu.status === 'active'
}).map((spu) => {
  const category = CATEGORIES.find((c) => c.id === spu.categoryId)
  const skuList = SKUS.filter((s) => s.spuId === spu.id && s.status === 'active')
  const minPrice = skuList.length ? Math.min(...skuList.map((s) => s.retailPrice)) : 0
  return { ...spu, categoryName: category?.name ?? '', categoryNameEn: category?.nameEn ?? '', skuCount: skuList.length, minPrice }
})

/**
 * 获取分类列表（启用状态，含小程序渠道商品数量）
 * GET /api/app/categories
 */
appProductRouter.get('/categories', (_req, res) => {
  const list = CATEGORIES.filter((c) => c.status === 'enabled').map((c) => {
    const productCount = miniappSpus.filter((p) => p.categoryId === c.id).length
    return { id: c.id, name: c.name, nameEn: c.nameEn, sort: c.sort, productCount }
  })
  res.json(ok(list))
})

/**
 * 获取商品列表（分页 + 分类/关键词筛选，仅小程序渠道上架商品）
 * GET /api/app/products
 */
appProductRouter.get('/products', (req, res) => {
  const { pageIndex = '1', pageSize = '10', categoryId, keyword } = req.query as Record<string, string | undefined>
  let filtered = miniappSpus
  if (categoryId) filtered = filtered.filter((p) => p.categoryId === categoryId)
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter((p) => p.name.toLowerCase().includes(kw) || p.nameEn.toLowerCase().includes(kw) || p.code.toLowerCase().includes(kw))
  }
  res.json(ok(paginate(filtered, parseInt(pageIndex, 10), parseInt(pageSize, 10))))
})

/**
 * 获取商品详情（SPU 信息 + 可售 SKU 列表 + 分类信息）
 * GET /api/app/products/:id
 */
appProductRouter.get('/products/:id', (req, res) => {
  const spu = miniappSpus.find((p) => p.id === req.params.id)
  if (!spu) return res.json(fail('商品不存在', 404))
  const skus = SKUS.filter((s) => s.spuId === spu.id && s.status === 'active')
    .map(({ id, specValues, retailPrice, memberPrice, stock }) => ({ id, specValues, retailPrice, memberPrice, stock }))
  res.json(ok({ ...spu, skus }))
})
