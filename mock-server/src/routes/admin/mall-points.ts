// src/routes/admin/mall-points.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { POINTS_RECORDS, MEMBERS, type PointsRecord } from '../../data/mall.js'

export const adminMallPointsRouter = Router()

adminMallPointsRouter.get('/mall/points/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const memberId = req.query.memberId as string | undefined
  const type = req.query.type as string | undefined
  const source = req.query.source as string | undefined

  let filtered = [...POINTS_RECORDS]
  if (memberId) filtered = filtered.filter(p => p.memberId === memberId)
  if (type) filtered = filtered.filter(p => p.type === type)
  if (source) filtered = filtered.filter(p => p.source === source)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminMallPointsRouter.get('/mall/points/:id', (req, res) => {
  const record = POINTS_RECORDS.find(p => p.id === req.params.id)
  if (!record) { res.json(fail('record not found', 404)); return }
  res.json(ok(record))
})

adminMallPointsRouter.post('/mall/points', (req, res) => {
  const body = req.body
  if (!body.memberId) { res.json(fail('memberId required')); return }

  const member = MEMBERS.find(m => m.id === body.memberId)
  if (!member) { res.json(fail('member not found', 404)); return }

  const record: PointsRecord = {
    id: guid(),
    memberId: member.id,
    memberName: member.nickname,
    type: body.type || 'earn',
    amount: body.amount || 0,
    source: body.source || 'adjust',
    description: body.description || '后台手动调整',
    createdAt: isoTime(),
  }
  POINTS_RECORDS.unshift(record)
  // sync member points
  member.points += record.amount
  if (member.points < 0) member.points = 0
  member.updatedAt = isoTime()
  res.json(ok({ id: record.id }, 'created'))
})

adminMallPointsRouter.delete('/mall/points/:id', (req, res) => {
  const idx = POINTS_RECORDS.findIndex(p => p.id === req.params.id)
  if (idx === -1) { res.json(fail('record not found', 404)); return }
  const record = POINTS_RECORDS[idx]
  const member = MEMBERS.find(m => m.id === record.memberId)
  if (member) {
    member.points -= record.amount
    if (member.points < 0) member.points = 0
    member.updatedAt = isoTime()
  }
  POINTS_RECORDS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})

adminMallPointsRouter.get('/mall/points/member/options', (_req, res) => {
  res.json(ok(MEMBERS.map(m => ({ id: m.id, name: m.nickname }))))
})
