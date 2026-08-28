// src/routes/admin/crm-invoice.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { INVOICES, type Invoice } from '../../data/crm-finance.js'

export const adminCrmInvoiceRouter = Router()

adminCrmInvoiceRouter.get('/crm/invoice/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const type = req.query.type as string | undefined
  const orderType = req.query.orderType as string | undefined
  const status = req.query.status as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...INVOICES]
  if (type) filtered = filtered.filter(i => i.type === type)
  if (orderType) filtered = filtered.filter(i => i.orderType === orderType)
  if (status) filtered = filtered.filter(i => i.status === status)
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(i =>
      i.invoiceNo.toLowerCase().includes(kw) ||
      i.orderNo.toLowerCase().includes(kw) ||
      i.partyName.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmInvoiceRouter.get('/crm/invoice/:id', (req, res) => {
  const invoice = INVOICES.find(i => i.id === req.params.id)
  if (!invoice) { res.json(fail('invoice not found', 404)); return }
  res.json(ok(invoice))
})

adminCrmInvoiceRouter.post('/crm/invoice', (req, res) => {
  const body = req.body
  if (!body.partyName?.trim()) { res.json(fail('partyName required')); return }

  const amount = Number(body.amount) || 0
  const taxRate = Number(body.taxRate) || 0
  const taxAmount = Math.round(amount * taxRate) / 100
  const total = amount + taxAmount

  const invoice: Invoice = {
    id: guid(),
    invoiceNo: body.invoiceNo || `INV-${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${String(INVOICES.length + 1).padStart(3, '0')}`,
    type: body.type || 'output',
    orderType: body.orderType || 'sales',
    orderNo: body.orderNo || '',
    partyName: body.partyName,
    amount,
    taxRate,
    taxAmount,
    total,
    issueDate: body.issueDate || isoTime().slice(0, 10),
    status: body.status || 'draft',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  INVOICES.unshift(invoice)
  res.json(ok({ id: invoice.id }, 'created'))
})

adminCrmInvoiceRouter.put('/crm/invoice/:id', (req, res) => {
  const invoice = INVOICES.find(i => i.id === req.params.id)
  if (!invoice) { res.json(fail('invoice not found', 404)); return }
  if (invoice.status === 'voided') { res.json(fail('voided invoice cannot be edited')); return }

  const body = req.body
  if (body.type !== undefined) invoice.type = body.type
  if (body.orderType !== undefined) invoice.orderType = body.orderType
  if (body.orderNo !== undefined) invoice.orderNo = body.orderNo
  if (body.partyName) invoice.partyName = body.partyName
  if (body.amount !== undefined) {
    invoice.amount = Number(body.amount)
    invoice.taxAmount = Math.round(invoice.amount * invoice.taxRate) / 100
    invoice.total = invoice.amount + invoice.taxAmount
  }
  if (body.taxRate !== undefined) {
    invoice.taxRate = Number(body.taxRate)
    invoice.taxAmount = Math.round(invoice.amount * invoice.taxRate) / 100
    invoice.total = invoice.amount + invoice.taxAmount
  }
  if (body.issueDate !== undefined) invoice.issueDate = body.issueDate
  if (body.remark !== undefined) invoice.remark = body.remark
  invoice.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmInvoiceRouter.post('/crm/invoice/:id/status', (req, res) => {
  const invoice = INVOICES.find(i => i.id === req.params.id)
  if (!invoice) { res.json(fail('invoice not found', 404)); return }
  const newStatus = req.body.status as string
  if (!newStatus) { res.json(fail('status required')); return }
  invoice.status = newStatus as Invoice['status']
  invoice.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmInvoiceRouter.delete('/crm/invoice/:id', (req, res) => {
  const idx = INVOICES.findIndex(i => i.id === req.params.id)
  if (idx === -1) { res.json(fail('invoice not found', 404)); return }
  if (INVOICES[idx].status !== 'draft') { res.json(fail('only draft invoice can be deleted')); return }
  INVOICES.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
