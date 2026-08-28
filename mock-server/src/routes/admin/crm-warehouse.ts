 // src/routes/admin/crm-warehouse.ts
 import { Router } from 'express'
 import { ok, fail, paginate } from '../../helpers/envelope.js'
 import { guid, isoTime } from '../../helpers/id.js'
 import { WAREHOUSES, type Warehouse } from '../../data/crm-inventory.js'

 export const adminCrmWarehouseRouter = Router()

 adminCrmWarehouseRouter.get('/crm/warehouse/options', (_req, res) => {
   const options = WAREHOUSES
     .filter(w => w.status === 'active')
     .map(w => ({ id: w.id, name: w.name, code: w.code }))
   res.json(ok(options))
 })

 adminCrmWarehouseRouter.get('/crm/warehouse/list', (req, res) => {
   const pageIndex = Number(req.query.pageIndex ?? 1)
   const pageSize = Number(req.query.pageSize ?? 10)
   const code = req.query.code as string | undefined
   const name = req.query.name as string | undefined
   const status = req.query.status as string | undefined

   let filtered = [...WAREHOUSES]
   if (code) filtered = filtered.filter(w => w.code.toLowerCase().includes(code.toLowerCase()))
   if (name) filtered = filtered.filter(w => w.name.toLowerCase().includes(name.toLowerCase()))
   if (status) filtered = filtered.filter(w => w.status === status)
   filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

   res.json(ok(paginate(filtered, pageIndex, pageSize)))
 })

 adminCrmWarehouseRouter.get('/crm/warehouse/:id', (req, res) => {
   const warehouse = WAREHOUSES.find(w => w.id === req.params.id)
   if (!warehouse) { res.json(fail('warehouse not found', 404)); return }
   res.json(ok(warehouse))
 })

 adminCrmWarehouseRouter.post('/crm/warehouse', (req, res) => {
   const body = req.body
   if (!body.name?.trim()) { res.json(fail('name required')); return }

   const warehouse: Warehouse = {
     id: guid(),
     code: body.code || `WH${String(WAREHOUSES.length + 1).padStart(3, '0')}`,
     name: body.name,
     address: body.address || '',
     manager: body.manager || '',
     phone: body.phone || '',
     status: body.status || 'active',
     remark: body.remark || '',
     createdAt: isoTime(),
     updatedAt: isoTime(),
   }
   WAREHOUSES.unshift(warehouse)
   res.json(ok({ id: warehouse.id }, 'created'))
 })

 adminCrmWarehouseRouter.put('/crm/warehouse/:id', (req, res) => {
   const warehouse = WAREHOUSES.find(w => w.id === req.params.id)
   if (!warehouse) { res.json(fail('warehouse not found', 404)); return }

   const body = req.body
   if (body.code !== undefined) warehouse.code = body.code
   if (body.name) warehouse.name = body.name
   if (body.address !== undefined) warehouse.address = body.address
   if (body.manager !== undefined) warehouse.manager = body.manager
   if (body.phone !== undefined) warehouse.phone = body.phone
   if (body.status !== undefined) warehouse.status = body.status
   if (body.remark !== undefined) warehouse.remark = body.remark
   warehouse.updatedAt = isoTime()
   res.json(ok(null, 'updated'))
 })

 adminCrmWarehouseRouter.delete('/crm/warehouse/:id', (req, res) => {
   const idx = WAREHOUSES.findIndex(w => w.id === req.params.id)
   if (idx === -1) { res.json(fail('warehouse not found', 404)); return }
   WAREHOUSES.splice(idx, 1)
   res.json(ok(null, 'deleted'))
 })
