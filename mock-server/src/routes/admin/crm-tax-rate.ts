// src/routes/admin/crm-tax-rate.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { TAX_RATES, type TaxRate } from '../../data/crm.js'

export const adminCrmTaxRateRouter = Router()

adminCrmTaxRateRouter.get('/crm/tax-rate/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const code = req.query.code as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...TAX_RATES]
  if (code) filtered = filtered.filter(t => t.code.toLowerCase().includes(code.toLowerCase()))
  if (status) filtered = filtered.filter(t => t.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmTaxRateRouter.get('/crm/tax-rate/:id', (req, res) => {
  const taxRate = TAX_RATES.find(t => t.id === req.params.id)
  if (!taxRate) { res.json(fail('tax rate not found', 404)); return }
  res.json(ok(taxRate))
})

adminCrmTaxRateRouter.post('/crm/tax-rate', (req, res) => {
  const body = req.body
  if (!body.code?.trim()) { res.json(fail('code required')); return }
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const taxRate: TaxRate = {
    id: guid(),
    code: body.code,
    name: body.name,
    rate: body.rate ?? 0,
    status: body.status || 'active',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  TAX_RATES.unshift(taxRate)
  res.json(ok({ id: taxRate.id }, 'created'))
})

adminCrmTaxRateRouter.put('/crm/tax-rate/:id', (req, res) => {
  const taxRate = TAX_RATES.find(t => t.id === req.params.id)
  if (!taxRate) { res.json(fail('tax rate not found', 404)); return }

  const body = req.body
  if (body.code) taxRate.code = body.code
  if (body.name) taxRate.name = body.name
  if (body.rate !== undefined) taxRate.rate = body.rate
  if (body.status !== undefined) taxRate.status = body.status
  if (body.remark !== undefined) taxRate.remark = body.remark
  taxRate.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmTaxRateRouter.delete('/crm/tax-rate/:id', (req, res) => {
  const idx = TAX_RATES.findIndex(t => t.id === req.params.id)
  if (idx === -1) { res.json(fail('tax rate not found', 404)); return }
  TAX_RATES.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
