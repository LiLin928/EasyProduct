// src/routes/admin/crm-customer.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { CUSTOMERS, type Customer } from '../../data/crm.js'

export const adminCrmCustomerRouter = Router()

adminCrmCustomerRouter.get('/crm/customer/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const code = req.query.code as string | undefined
  const name = req.query.name as string | undefined
  const type = req.query.type as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...CUSTOMERS]
  if (code) filtered = filtered.filter(c => c.code.toLowerCase().includes(code.toLowerCase()))
  if (name) filtered = filtered.filter(c => c.name.toLowerCase().includes(name.toLowerCase()))
  if (type) filtered = filtered.filter(c => c.type === type)
  if (status) filtered = filtered.filter(c => c.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmCustomerRouter.get('/crm/customer/:id', (req, res) => {
  const customer = CUSTOMERS.find(c => c.id === req.params.id)
  if (!customer) { res.json(fail('customer not found', 404)); return }
  res.json(ok(customer))
})

adminCrmCustomerRouter.post('/crm/customer', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const customer: Customer = {
    id: guid(),
    code: body.code || `C${String(CUSTOMERS.length + 1).padStart(5, '0')}`,
    name: body.name,
    type: body.type || 'b2b',
    source: body.source || 'manual',
    contactPerson: body.contactPerson || '',
    phone: body.phone || '',
    email: body.email || '',
    address: body.address || '',
    salesPersonName: body.salesPersonName || '',
    creditLimit: body.creditLimit ?? 0,
    status: body.status || 'active',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  CUSTOMERS.unshift(customer)
  res.json(ok({ id: customer.id }, 'created'))
})

adminCrmCustomerRouter.put('/crm/customer/:id', (req, res) => {
  const customer = CUSTOMERS.find(c => c.id === req.params.id)
  if (!customer) { res.json(fail('customer not found', 404)); return }

  const body = req.body
  if (body.code !== undefined) customer.code = body.code
  if (body.name) customer.name = body.name
  if (body.type) customer.type = body.type
  if (body.source) customer.source = body.source
  if (body.contactPerson !== undefined) customer.contactPerson = body.contactPerson
  if (body.phone !== undefined) customer.phone = body.phone
  if (body.email !== undefined) customer.email = body.email
  if (body.address !== undefined) customer.address = body.address
  if (body.salesPersonName !== undefined) customer.salesPersonName = body.salesPersonName
  if (body.creditLimit !== undefined) customer.creditLimit = body.creditLimit
  if (body.status !== undefined) customer.status = body.status
  if (body.remark !== undefined) customer.remark = body.remark
  customer.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmCustomerRouter.delete('/crm/customer/:id', (req, res) => {
  const idx = CUSTOMERS.findIndex(c => c.id === req.params.id)
  if (idx === -1) { res.json(fail('customer not found', 404)); return }
  CUSTOMERS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
