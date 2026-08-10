// src/routes/site/category.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { CATEGORIES } from '../../data/product.js'

export const siteCategoryRouter = Router()

// 分类树（一级分类）
siteCategoryRouter.get('/category/list', (_req, res) => {
  res.json(ok(CATEGORIES))
})