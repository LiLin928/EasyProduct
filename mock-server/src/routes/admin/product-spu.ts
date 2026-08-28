// src/routes/admin/product-spu.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import {
  SPUS,
  SKUS,
  type ProductSpu,
  type ProductSku,
  type SpecGroup,
} from '../../data/product.js'

export const adminProductSpuRouter = Router()

adminProductSpuRouter.get('/product/spu/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const code = req.query.code as string | undefined
  const categoryId = req.query.categoryId as string | undefined
  const status = req.query.status as string | undefined
  const type = req.query.type as string | undefined

  let filtered = [...SPUS]
  if (name) filtered = filtered.filter(s => s.name.includes(name) || s.nameEn.toLowerCase().includes(name.toLowerCase()))
  if (code) filtered = filtered.filter(s => s.code.toLowerCase().includes(code.toLowerCase()))
  if (categoryId) filtered = filtered.filter(s => s.categoryId === categoryId)
  if (status) filtered = filtered.filter(s => s.status === status)
  if (type) filtered = filtered.filter(s => s.type === type)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminProductSpuRouter.get('/product/spu/:id', (req, res) => {
  const spu = SPUS.find(s => s.id === req.params.id)
  if (!spu) { res.json(fail('spu not found', 404)); return }
  const skuList = SKUS.filter(k => k.spuId === spu.id)
  res.json(ok({ ...spu, skus: skuList }))
})

adminProductSpuRouter.post('/product/spu', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }
  if (!body.code?.trim()) { res.json(fail('code required')); return }

  const spu: ProductSpu = {
    id: guid(),
    code: body.code,
    name: body.name,
    nameEn: body.nameEn || body.name,
    categoryId: body.categoryId || '',
    type: body.type || 'ticket',
    mainImage: body.mainImage || '',
    images: body.images || [],
    description: body.description || '',
    unit: body.unit || 'ge',
    brand: body.brand || '',
    specs: (body.specs as SpecGroup[]) || [],
    status: body.status || 'active',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  SPUS.push(spu)

  const combos = generateSpecCombinations(spu.specs)
  const newSkus: ProductSku[] = combos.map((combo) => ({
    id: guid(),
    spuId: spu.id,
    specValues: combo,
    barcode: '',
    retailPrice: 0,
    memberPrice: 0,
    b2bPrice: 0,
    costPrice: 0,
    stock: 0,
    status: 'active' as const,
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }))
  SKUS.push(...newSkus)

  res.json(ok({ id: spu.id }, 'created'))
})

adminProductSpuRouter.put('/product/spu/:id', (req, res) => {
  const spu = SPUS.find(s => s.id === req.params.id)
  if (!spu) { res.json(fail('spu not found', 404)); return }

  const body = req.body
  if (body.name) spu.name = body.name
  if (body.nameEn !== undefined) spu.nameEn = body.nameEn
  if (body.categoryId !== undefined) spu.categoryId = body.categoryId
  if (body.type !== undefined) spu.type = body.type
  if (body.mainImage !== undefined) spu.mainImage = body.mainImage
  if (body.images !== undefined) spu.images = body.images
  if (body.description !== undefined) spu.description = body.description
  if (body.unit !== undefined) spu.unit = body.unit
  if (body.brand !== undefined) spu.brand = body.brand
  if (body.specs !== undefined) {
    spu.specs = body.specs
    const oldSkus = SKUS.filter(k => k.spuId === spu.id)
    const oldMap = new Map(oldSkus.map(k => [JSON.stringify(k.specValues), k]))
    const combos = generateSpecCombinations(spu.specs)
    const newSkus: ProductSku[] = combos.map((combo) => {
      const existing = oldMap.get(JSON.stringify(combo))
      if (existing) return existing
      return {
        id: guid(),
        spuId: spu.id,
        specValues: combo,
        barcode: '',
        retailPrice: 0,
        memberPrice: 0,
        b2bPrice: 0,
        costPrice: 0,
        stock: 0,
        status: 'active' as const,
        createdAt: isoTime(),
        updatedAt: isoTime(),
      }
    })
    for (let i = SKUS.length - 1; i >= 0; i--) {
      if (SKUS[i].spuId === spu.id && !newSkus.some(n => n.id === SKUS[i].id)) {
        SKUS.splice(i, 1)
      }
    }
    for (const ns of newSkus) {
      if (!SKUS.some(k => k.id === ns.id)) SKUS.push(ns)
    }
  }
  if (body.status !== undefined) spu.status = body.status
  spu.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminProductSpuRouter.delete('/product/spu/:id', (req, res) => {
  const idx = SPUS.findIndex(s => s.id === req.params.id)
  if (idx === -1) { res.json(fail('spu not found', 404)); return }

  SPUS.splice(idx, 1)
  for (let i = SKUS.length - 1; i >= 0; i--) {
    if (SKUS[i].spuId === req.params.id) SKUS.splice(i, 1)
  }
  res.json(ok(null, 'deleted'))
})

adminProductSpuRouter.put('/product/sku/batch', (req, res) => {
  const items = req.body as ProductSku[]
  if (!Array.isArray(items)) { res.json(fail('items required')); return }

  for (const item of items) {
    const sku = SKUS.find(k => k.id === item.id)
    if (!sku) continue
    sku.barcode = item.barcode ?? sku.barcode
    sku.retailPrice = item.retailPrice ?? sku.retailPrice
    sku.memberPrice = item.memberPrice ?? sku.memberPrice
    sku.b2bPrice = item.b2bPrice ?? sku.b2bPrice
    sku.costPrice = item.costPrice ?? sku.costPrice
    sku.stock = item.stock ?? sku.stock
    sku.status = item.status ?? sku.status
    sku.updatedAt = isoTime()
  }
  res.json(ok(null, 'updated'))
})

adminProductSpuRouter.put('/product/sku/:id', (req, res) => {
  const sku = SKUS.find(k => k.id === req.params.id)
  if (!sku) { res.json(fail('sku not found', 404)); return }

  const body = req.body
  if (body.barcode !== undefined) sku.barcode = body.barcode
  if (body.retailPrice !== undefined) sku.retailPrice = body.retailPrice
  if (body.memberPrice !== undefined) sku.memberPrice = body.memberPrice
  if (body.b2bPrice !== undefined) sku.b2bPrice = body.b2bPrice
  if (body.costPrice !== undefined) sku.costPrice = body.costPrice
  if (body.stock !== undefined) sku.stock = body.stock
  if (body.status !== undefined) sku.status = body.status
  sku.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

function generateSpecCombinations(specs: SpecGroup[]): Record<string, string>[] {
  if (specs.length === 0) return [{}]
  const result: Record<string, string>[] = [{}]
  for (const spec of specs) {
    const next: Record<string, string>[] = []
    for (const prev of result) {
      for (const val of spec.values) {
        next.push({ ...prev, [spec.name]: val })
      }
    }
    result.length = 0
    result.push(...next)
  }
  return result
}
