// src/routes/admin/crm-reversal.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { REVERSALS, type Reversal } from '../../data/crm-reversal.js'

export const adminCrmReversalRouter = Router()

adminCrmReversalRouter.get('/crm/reversal/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const type = req.query.type as string | undefined
  const status = req.query.status as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...REVERSALS]
  if (type) filtered = filtered.filter((i) => i.type === type)
  if (status) filtered = filtered.filter((i) => i.status === status)
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(
      (i) =>
        i.reversalNo.toLowerCase().includes(kw) ||
        i.sourceOrderNo.toLowerCase().includes(kw) ||
        i.partyName.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminCrmReversalRouter.get('/crm/reversal/:id', (req, res) => {
  const reversal = REVERSALS.find((i) => i.id === req.params.id)
  if (!reversal) {
    res.json(fail('reversal not found', 404))
    return
  }
  res.json(ok(reversal))
})

adminCrmReversalRouter.post('/crm/reversal', (req, res) => {
  const body = req.body
  if (!body.type?.trim()) {
    res.json(fail('type required'))
    return
  }

  const reversal: Reversal = {
    id: guid(),
    reversalNo:
      body.reversalNo ||
      `RV-${new Date().toISOString().slice(0, 10).replace(/-/g, '')}-${String(REVERSALS.length + 1).padStart(3, '0')}`,
    type: body.type,
    sourceOrderType: body.sourceOrderType || '',
    sourceOrderNo: body.sourceOrderNo || '',
    partyName: body.partyName || '',
    amount: Number(body.amount) || 0,
    reason: body.reason || '',
    operator: body.operator || '',
    status: body.status || 'draft',
    items: body.items || [],
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  REVERSALS.unshift(reversal)
  res.json(ok({ id: reversal.id }, 'created'))
})

adminCrmReversalRouter.put('/crm/reversal/:id', (req, res) => {
  const reversal = REVERSALS.find((i) => i.id === req.params.id)
  if (!reversal) {
    res.json(fail('reversal not found', 404))
    return
  }
  if (reversal.status !== 'draft') {
    res.json(fail('only draft reversal can be edited'))
    return
  }

  const body = req.body
  if (body.type !== undefined) reversal.type = body.type
  if (body.sourceOrderType !== undefined) reversal.sourceOrderType = body.sourceOrderType
  if (body.sourceOrderNo !== undefined) reversal.sourceOrderNo = body.sourceOrderNo
  if (body.partyName !== undefined) reversal.partyName = body.partyName
  if (body.amount !== undefined) reversal.amount = Number(body.amount)
  if (body.reason !== undefined) reversal.reason = body.reason
  if (body.operator !== undefined) reversal.operator = body.operator
  if (body.items !== undefined) reversal.items = body.items
  reversal.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmReversalRouter.post('/crm/reversal/:id/status', (req, res) => {
  const reversal = REVERSALS.find((i) => i.id === req.params.id)
  if (!reversal) {
    res.json(fail('reversal not found', 404))
    return
  }
  const newStatus = req.body.status as string
  if (!newStatus) {
    res.json(fail('status required'))
    return
  }
  reversal.status = newStatus as Reversal['status']
  reversal.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminCrmReversalRouter.delete('/crm/reversal/:id', (req, res) => {
  const idx = REVERSALS.findIndex((i) => i.id === req.params.id)
  if (idx === -1) {
    res.json(fail('reversal not found', 404))
    return
  }
  if (REVERSALS[idx].status !== 'draft' && REVERSALS[idx].status !== 'rejected') {
    res.json(fail('only draft or rejected reversal can be deleted'))
    return
  }
  REVERSALS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
