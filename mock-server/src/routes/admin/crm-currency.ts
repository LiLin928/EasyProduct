// src/routes/admin/crm-currency.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { CURRENCIES, type Currency } from '../../data/crm.js'

export const adminCrmCurrencyRouter = Router()

adminCrmCurrencyRouter.get('/crm/currency/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const code = req.query.code as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...CURRENCIES]
  if (code) filtered = filtered.filter(c => c.code.toLowerCase().includes(code.toLowerCase()))
  if (status) filtered = filtered.filter(c => c.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmCurrencyRouter.get('/crm/currency/:id', (req, res) => {
  const currency = CURRENCIES.find(c => c.id === req.params.id)
  if (!currency) { res.json(fail('currency not found', 404)); return }
  res.json(ok(currency))
})

adminCrmCurrencyRouter.post('/crm/currency', (req, res) => {
  const body = req.body
  if (!body.code?.trim()) { res.json(fail('code required')); return }
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const currency: Currency = {
    id: guid(),
    code: body.code.toUpperCase(),
    name: body.name,
    symbol: body.symbol || '',
    exchangeRate: body.exchangeRate ?? 1,
    isDefault: body.isDefault ?? false,
    status: body.status || 'active',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  if (currency.isDefault) {
    CURRENCIES.forEach(c => { c.isDefault = false })
  }
  CURRENCIES.unshift(currency)
  res.json(ok({ id: currency.id }, 'created'))
})

adminCrmCurrencyRouter.put('/crm/currency/:id', (req, res) => {
  const currency = CURRENCIES.find(c => c.id === req.params.id)
  if (!currency) { res.json(fail('currency not found', 404)); return }

  const body = req.body
  if (body.code) currency.code = body.code.toUpperCase()
  if (body.name) currency.name = body.name
  if (body.symbol !== undefined) currency.symbol = body.symbol
  if (body.exchangeRate !== undefined) currency.exchangeRate = body.exchangeRate
  if (body.isDefault !== undefined) {
    if (body.isDefault) CURRENCIES.forEach(c => { c.isDefault = false })
    currency.isDefault = body.isDefault
  }
  if (body.status !== undefined) currency.status = body.status
  currency.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmCurrencyRouter.delete('/crm/currency/:id', (req, res) => {
  const idx = CURRENCIES.findIndex(c => c.id === req.params.id)
  if (idx === -1) { res.json(fail('currency not found', 404)); return }
  if (CURRENCIES[idx].isDefault) { res.json(fail('cannot delete default currency')); return }
  CURRENCIES.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
