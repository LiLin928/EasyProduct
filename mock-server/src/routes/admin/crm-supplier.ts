// src/routes/admin/crm-supplier.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { SUPPLIERS, type Supplier } from '../../data/crm.js'

export const adminCrmSupplierRouter = Router()

adminCrmSupplierRouter.get('/crm/supplier/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const code = req.query.code as string | undefined
  const name = req.query.name as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...SUPPLIERS]
  if (code) filtered = filtered.filter(s => s.code.toLowerCase().includes(code.toLowerCase()))
  if (name) filtered = filtered.filter(s => s.name.toLowerCase().includes(name.toLowerCase()))
  if (status) filtered = filtered.filter(s => s.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmSupplierRouter.get('/crm/supplier/:id', (req, res) => {
  const supplier = SUPPLIERS.find(s => s.id === req.params.id)
  if (!supplier) { res.json(fail('supplier not found', 404)); return }
  res.json(ok(supplier))
})

adminCrmSupplierRouter.post('/crm/supplier', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const supplier: Supplier = {
    id: guid(),
    code: body.code || `S${String(SUPPLIERS.length + 1).padStart(5, '0')}`,
    name: body.name,
    contactPerson: body.contactPerson || '',
    phone: body.phone || '',
    email: body.email || '',
    address: body.address || '',
    bankName: body.bankName || '',
    bankAccount: body.bankAccount || '',
    status: body.status || 'active',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  SUPPLIERS.unshift(supplier)
  res.json(ok({ id: supplier.id }, 'created'))
})

adminCrmSupplierRouter.put('/crm/supplier/:id', (req, res) => {
  const supplier = SUPPLIERS.find(s => s.id === req.params.id)
  if (!supplier) { res.json(fail('supplier not found', 404)); return }

  const body = req.body
  if (body.code !== undefined) supplier.code = body.code
  if (body.name) supplier.name = body.name
  if (body.contactPerson !== undefined) supplier.contactPerson = body.contactPerson
  if (body.phone !== undefined) supplier.phone = body.phone
  if (body.email !== undefined) supplier.email = body.email
  if (body.address !== undefined) supplier.address = body.address
  if (body.bankName !== undefined) supplier.bankName = body.bankName
  if (body.bankAccount !== undefined) supplier.bankAccount = body.bankAccount
  if (body.status !== undefined) supplier.status = body.status
  if (body.remark !== undefined) supplier.remark = body.remark
  supplier.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmSupplierRouter.delete('/crm/supplier/:id', (req, res) => {
  const idx = SUPPLIERS.findIndex(s => s.id === req.params.id)
  if (idx === -1) { res.json(fail('supplier not found', 404)); return }
  SUPPLIERS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
