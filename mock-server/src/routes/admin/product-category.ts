// src/routes/admin/product-category.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { CATEGORIES, type ProductCategory } from '../../data/product.js'

export const adminProductCategoryRouter = Router()

adminProductCategoryRouter.get('/product/category/list', (_req, res) => {
  res.json(ok(CATEGORIES))
})

adminProductCategoryRouter.post('/product/category', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const cat: ProductCategory = {
    id: guid(),
    name: body.name,
    nameEn: body.nameEn || body.name,
    parentId: body.parentId || '0',
    sort: body.sort ?? CATEGORIES.length + 1,
    status: body.status || 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  CATEGORIES.push(cat)
  res.json(ok({ id: cat.id }, 'created'))
})

adminProductCategoryRouter.put('/product/category/:id', (req, res) => {
  const cat = CATEGORIES.find(c => c.id === req.params.id)
  if (!cat) { res.json(fail('category not found', 404)); return }

  const body = req.body
  if (body.name) cat.name = body.name
  if (body.nameEn !== undefined) cat.nameEn = body.nameEn
  if (body.parentId !== undefined) cat.parentId = body.parentId
  if (body.sort !== undefined) cat.sort = body.sort
  if (body.status !== undefined) cat.status = body.status
  cat.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminProductCategoryRouter.delete('/product/category/:id', (req, res) => {
  const idx = CATEGORIES.findIndex(c => c.id === req.params.id)
  if (idx === -1) { res.json(fail('category not found', 404)); return }

  const hasChildren = CATEGORIES.some(c => c.parentId === req.params.id)
  if (hasChildren) { res.json(fail('has sub-categories, cannot delete')); return }

  CATEGORIES.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
