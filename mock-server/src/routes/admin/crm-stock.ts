 // src/routes/admin/crm-stock.ts
 import { Router } from 'express'
 import { ok, paginate } from '../../helpers/envelope.js'
 import { STOCKS } from '../../data/crm-inventory.js'

 export const adminCrmStockRouter = Router()

 adminCrmStockRouter.get('/crm/stock/list', (req, res) => {
   const pageIndex = Number(req.query.pageIndex ?? 1)
   const pageSize = Number(req.query.pageSize ?? 10)
   const warehouseId = req.query.warehouseId as string | undefined
   const keyword = req.query.keyword as string | undefined

   let filtered = [...STOCKS]
   if (warehouseId) filtered = filtered.filter(s => s.warehouseId === warehouseId)
   if (keyword) {
     const kw = keyword.toLowerCase()
     filtered = filtered.filter(s =>
       s.skuCode.toLowerCase().includes(kw) ||
       s.skuName.toLowerCase().includes(kw),
     )
   }
   filtered.sort((a, b) => b.updatedAt.localeCompare(a.updatedAt))

   res.json(ok(paginate(filtered, pageIndex, pageSize)))
 })

 adminCrmStockRouter.get('/crm/stock/:id', (req, res) => {
   const stock = STOCKS.find(s => s.id === req.params.id)
   if (!stock) { res.json(ok(null)); return }
   res.json(ok(stock))
 })
