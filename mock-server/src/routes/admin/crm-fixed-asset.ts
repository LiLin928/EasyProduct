// src/routes/admin/crm-fixed-asset.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { FIXED_ASSETS, ASSET_DEPRECIATIONS, type FixedAsset } from '../../data/crm-finance.js'

export const adminCrmFixedAssetRouter = Router()

adminCrmFixedAssetRouter.get('/crm/fixed-asset/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const category = req.query.category as string | undefined
  const status = req.query.status as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...FIXED_ASSETS]
  if (category) filtered = filtered.filter(a => a.category === category)
  if (status) filtered = filtered.filter(a => a.status === status)
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(a =>
      a.assetNo.toLowerCase().includes(kw) ||
      a.name.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmFixedAssetRouter.get('/crm/fixed-asset/:id', (req, res) => {
  const asset = FIXED_ASSETS.find(a => a.id === req.params.id)
  if (!asset) { res.json(fail('fixed asset not found', 404)); return }
  res.json(ok(asset))
})

adminCrmFixedAssetRouter.get('/crm/fixed-asset/:id/depreciations', (req, res) => {
  const asset = FIXED_ASSETS.find(a => a.id === req.params.id)
  if (!asset) { res.json(fail('fixed asset not found', 404)); return }
  const records = ASSET_DEPRECIATIONS
    .filter(d => d.assetId === req.params.id)
    .sort((a, b) => a.period.localeCompare(b.period))
  res.json(ok(records))
})

adminCrmFixedAssetRouter.post('/crm/fixed-asset', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const originalValue = Number(body.originalValue) || 0
  const salvageValue = Number(body.salvageValue) || 0
  const usefulYears = Number(body.usefulYears) || 1

  const asset: FixedAsset = {
    id: guid(),
    assetNo: body.assetNo || `FA-${String(FIXED_ASSETS.length + 1).padStart(4, '0')}`,
    name: body.name,
    category: body.category || '',
    originalValue,
    purchaseDate: body.purchaseDate || isoTime().slice(0, 10),
    depreciationMethod: 'straight-line',
    salvageValue,
    usefulYears,
    currentValue: originalValue,
    status: 'active',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  FIXED_ASSETS.unshift(asset)
  res.json(ok({ id: asset.id }, 'created'))
})

adminCrmFixedAssetRouter.put('/crm/fixed-asset/:id', (req, res) => {
  const asset = FIXED_ASSETS.find(a => a.id === req.params.id)
  if (!asset) { res.json(fail('fixed asset not found', 404)); return }
  if (asset.status === 'scrapped') { res.json(fail('scrapped asset cannot be edited')); return }

  const body = req.body
  if (body.assetNo !== undefined) asset.assetNo = body.assetNo
  if (body.name) asset.name = body.name
  if (body.category !== undefined) asset.category = body.category
  if (body.originalValue !== undefined) asset.originalValue = Number(body.originalValue)
  if (body.purchaseDate !== undefined) asset.purchaseDate = body.purchaseDate
  if (body.salvageValue !== undefined) asset.salvageValue = Number(body.salvageValue)
  if (body.usefulYears !== undefined) asset.usefulYears = Number(body.usefulYears)
  if (body.remark !== undefined) asset.remark = body.remark
  asset.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmFixedAssetRouter.post('/crm/fixed-asset/:id/status', (req, res) => {
  const asset = FIXED_ASSETS.find(a => a.id === req.params.id)
  if (!asset) { res.json(fail('fixed asset not found', 404)); return }
  const newStatus = req.body.status as string
  if (!newStatus) { res.json(fail('status required')); return }
  asset.status = newStatus as FixedAsset['status']
  asset.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmFixedAssetRouter.delete('/crm/fixed-asset/:id', (req, res) => {
  const idx = FIXED_ASSETS.findIndex(a => a.id === req.params.id)
  if (idx === -1) { res.json(fail('fixed asset not found', 404)); return }
  if (FIXED_ASSETS[idx].status === 'scrapped') { res.json(fail('scrapped asset cannot be deleted')); return }
  FIXED_ASSETS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
