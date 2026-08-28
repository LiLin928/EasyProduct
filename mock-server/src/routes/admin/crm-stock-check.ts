 // src/routes/admin/crm-stock-check.ts
 import { Router } from 'express'
 import { ok, fail, paginate } from '../../helpers/envelope.js'
 import { guid, isoTime } from '../../helpers/id.js'
 import { STOCK_CHECKS, type StockCheck } from '../../data/crm-inventory.js'

 export const adminCrmStockCheckRouter = Router()

 adminCrmStockCheckRouter.get('/crm/stock-check/list', (req, res) => {
   const pageIndex = Number(req.query.pageIndex ?? 1)
   const pageSize = Number(req.query.pageSize ?? 10)
   const warehouseId = req.query.warehouseId as string | undefined
   const status = req.query.status as string | undefined
   const checkNo = req.query.checkNo as string | undefined

   let filtered = [...STOCK_CHECKS]
   if (warehouseId) filtered = filtered.filter(c => c.warehouseId === warehouseId)
   if (status) filtered = filtered.filter(c => c.status === status)
   if (checkNo) filtered = filtered.filter(c => c.checkNo.toLowerCase().includes(checkNo.toLowerCase()))
   filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

   res.json(ok(paginate(filtered, pageIndex, pageSize)))
 })

 adminCrmStockCheckRouter.get('/crm/stock-check/:id', (req, res) => {
   const check = STOCK_CHECKS.find(c => c.id === req.params.id)
   if (!check) { res.json(fail('stock check not found', 404)); return }
   res.json(ok(check))
 })

 adminCrmStockCheckRouter.post('/crm/stock-check', (req, res) => {
   const body = req.body
   if (!body.warehouseId) { res.json(fail('warehouseId required')); return }

   const check: StockCheck = {
     id: guid(),
     checkNo: body.checkNo || `SC-${isoTime().slice(0, 10).replace(/-/g, '')}-${String(STOCK_CHECKS.length + 1).padStart(3, '0')}`,
     warehouseId: body.warehouseId,
     warehouseName: body.warehouseName || '',
     checker: body.checker || '',
     checkDate: body.checkDate || isoTime().slice(0, 10),
     status: 'draft',
     remark: body.remark || '',
     items: body.items || [],
     createdAt: isoTime(),
     updatedAt: isoTime(),
   }
   check.items.forEach((item: StockCheck['items'][0]) => { item.checkId = check.id })
   STOCK_CHECKS.unshift(check)
   res.json(ok({ id: check.id }, 'created'))
 })

 adminCrmStockCheckRouter.put('/crm/stock-check/:id', (req, res) => {
   const check = STOCK_CHECKS.find(c => c.id === req.params.id)
   if (!check) { res.json(fail('stock check not found', 404)); return }
   if (check.status === 'completed') { res.json(fail('completed check cannot be edited')); return }

   const body = req.body
   if (body.checker !== undefined) check.checker = body.checker
   if (body.checkDate) check.checkDate = body.checkDate
   if (body.remark !== undefined) check.remark = body.remark
   if (body.items) {
     check.items = body.items
     check.items.forEach((item: StockCheck['items'][0]) => { item.checkId = check.id })
   }
   check.updatedAt = isoTime()
   res.json(ok(null, 'updated'))
 })

 adminCrmStockCheckRouter.patch('/crm/stock-check/:id/status', (req, res) => {
   const check = STOCK_CHECKS.find(c => c.id === req.params.id)
   if (!check) { res.json(fail('stock check not found', 404)); return }

   const body = req.body
   const newStatus = body.status
   if (!['draft', 'counting', 'completed'].includes(newStatus)) {
     res.json(fail('invalid status')); return
   }
   check.status = newStatus
   check.updatedAt = isoTime()
   res.json(ok(null, 'status updated'))
 })

 adminCrmStockCheckRouter.delete('/crm/stock-check/:id', (req, res) => {
   const idx = STOCK_CHECKS.findIndex(c => c.id === req.params.id)
   if (idx === -1) { res.json(fail('stock check not found', 404)); return }
   const check = STOCK_CHECKS[idx]
   if (check.status === 'completed') { res.json(fail('completed check cannot be deleted')); return }
   STOCK_CHECKS.splice(idx, 1)
   res.json(ok(null, 'deleted'))
 })
