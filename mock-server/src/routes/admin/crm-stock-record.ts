// src/routes/admin/crm-stock-record.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { STOCK_RECORDS } from '../../data/crm-inventory.js'

export const adminCrmStockRecordRouter = Router()

adminCrmStockRecordRouter.get('/crm/stock-record/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const warehouseId = req.query.warehouseId as string | undefined
  const sourceType = req.query.sourceType as string | undefined
  const startDate = req.query.startDate as string | undefined
  const endDate = req.query.endDate as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...STOCK_RECORDS]
  if (warehouseId) filtered = filtered.filter(r => r.warehouseId === warehouseId)
  if (sourceType) filtered = filtered.filter(r => r.sourceType === sourceType)
  if (startDate) filtered = filtered.filter(r => r.createdAt >= startDate)
  if (endDate) filtered = filtered.filter(r => r.createdAt <= endDate + 'T23:59:59')
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(r =>
      r.skuCode.toLowerCase().includes(kw) ||
      r.skuName.toLowerCase().includes(kw) ||
      r.sourceOrderNo.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmStockRecordRouter.get('/crm/stock-record/:id', (req, res) => {
  const record = STOCK_RECORDS.find(r => r.id === req.params.id)
  if (!record) { res.json(ok(null)); return }
  res.json(ok(record))
})

/**
 * 获取指定SKU的出入库记录
 * 用于库存管理页面查看单个SKU的出入库历史
 */
adminCrmStockRecordRouter.get('/crm/stock-record/by-sku', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const warehouseId = req.query.warehouseId as string | undefined
  const skuCode = req.query.skuCode as string | undefined

  if (!skuCode) {
    res.json(ok({ list: [], total: 0 }))
    return
  }

  let filtered = [...STOCK_RECORDS]
  // 筛选指定SKU
  filtered = filtered.filter(r => r.skuCode === skuCode)
  // 筛选指定仓库（可选）
  if (warehouseId) {
    filtered = filtered.filter(r => r.warehouseId === warehouseId)
  }
  // 按时间倒序
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})
