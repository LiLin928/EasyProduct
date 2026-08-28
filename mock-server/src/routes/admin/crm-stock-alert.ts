 // src/routes/admin/crm-stock-alert.ts
 import { Router } from 'express'
 import { ok, paginate } from '../../helpers/envelope.js'
 import { isoTime } from '../../helpers/id.js'
 import { STOCK_ALERTS } from '../../data/crm-inventory.js'

 export const adminCrmStockAlertRouter = Router()

 adminCrmStockAlertRouter.get('/crm/stock-alert/list', (req, res) => {
   const pageIndex = Number(req.query.pageIndex ?? 1)
   const pageSize = Number(req.query.pageSize ?? 10)
   const warehouseId = req.query.warehouseId as string | undefined
   const alertType = req.query.alertType as string | undefined
   const status = req.query.status as string | undefined
   const keyword = req.query.keyword as string | undefined

   let filtered = [...STOCK_ALERTS]
   if (warehouseId) filtered = filtered.filter(a => a.warehouseId === warehouseId)
   if (alertType) filtered = filtered.filter(a => a.alertType === alertType)
   if (status) filtered = filtered.filter(a => a.status === status)
   if (keyword) {
     const kw = keyword.toLowerCase()
     filtered = filtered.filter(a =>
       a.skuCode.toLowerCase().includes(kw) ||
       a.skuName.toLowerCase().includes(kw),
     )
   }
   filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

   res.json(ok(paginate(filtered, pageIndex, pageSize)))
 })

 adminCrmStockAlertRouter.patch('/crm/stock-alert/:id/resolve', (req, res) => {
   const alert = STOCK_ALERTS.find(a => a.id === req.params.id)
   if (!alert) { res.json(ok(null)); return }
   alert.status = 'resolved'
   alert.resolvedAt = isoTime()
   res.json(ok(null, 'resolved'))
 })

 adminCrmStockAlertRouter.patch('/crm/stock-alert/batch-resolve', (req, res) => {
   const ids: string[] = req.body.ids || []
   ids.forEach(id => {
     const alert = STOCK_ALERTS.find(a => a.id === id)
     if (alert) {
       alert.status = 'resolved'
       alert.resolvedAt = isoTime()
     }
   })
   res.json(ok(null, 'batch resolved'))
 })
