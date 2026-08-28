// src/routes/site/product.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { PRODUCTS } from '../../data/product.js'

export const siteProductRouter = Router()

// 产品列表（支持分类筛选、关键词搜索）
siteProductRouter.get('/product/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const categoryId = req.query.categoryId as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = PRODUCTS
  if (categoryId) {
    filtered = filtered.filter((p: (typeof PRODUCTS)[number]) => p.categoryId === categoryId)
  }
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter((p: (typeof PRODUCTS)[number]) =>
      p.name.includes(keyword) ||
      p.nameEn.toLowerCase().includes(kw) ||
      p.code.toLowerCase().includes(kw)
    )
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 产品详情
siteProductRouter.get('/product/:id', (req, res) => {
  const product = PRODUCTS.find((p: (typeof PRODUCTS)[number]) => p.id === req.params.id)
  if (!product) {
    res.json(fail('产品不存在', 404))
    return
  }
  res.json(ok(product))
})
