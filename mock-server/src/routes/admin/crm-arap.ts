// src/routes/admin/crm-arap.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ARAPS } from '../../data/crm-finance.js'

export const adminCrmArapRouter = Router()

adminCrmArapRouter.get('/crm/arap/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const orderType = req.query.orderType as string | undefined
  const aging = req.query.aging as string | undefined
  const status = req.query.status as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...ARAPS]
  if (orderType) filtered = filtered.filter(a => a.orderType === orderType)
  if (aging) filtered = filtered.filter(a => a.aging === aging)
  if (status) filtered = filtered.filter(a => a.status === status)
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(a =>
      a.orderNo.toLowerCase().includes(kw) ||
      a.partyName.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmArapRouter.get('/crm/arap/summary', (_req, res) => {
  const totalReceivable = ARAPS
    .filter(a => a.orderType === 'sales')
    .reduce((sum, a) => sum + a.balance, 0)
  const totalPayable = ARAPS
    .filter(a => a.orderType === 'purchase')
    .reduce((sum, a) => sum + a.balance, 0)
  res.json(ok({
    totalReceivable,
    totalPayable,
    totalBalance: totalReceivable - totalPayable,
  }))
})

adminCrmArapRouter.get('/crm/arap/:id', (req, res) => {
  const arap = ARAPS.find(a => a.id === req.params.id)
  if (!arap) { res.json(fail('arap record not found', 404)); return }
  res.json(ok(arap))
})
