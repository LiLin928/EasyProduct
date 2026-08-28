// src/routes/admin/mall-level.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { MEMBER_LEVELS, type MemberLevel } from '../../data/mall.js'

export const adminMallLevelRouter = Router()

adminMallLevelRouter.get('/mall/level/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...MEMBER_LEVELS]
  if (name) filtered = filtered.filter(l => l.name.includes(name))
  if (status) filtered = filtered.filter(l => l.status === status)
  filtered.sort((a, b) => a.sort - b.sort)

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminMallLevelRouter.get('/mall/level/:id', (req, res) => {
  const level = MEMBER_LEVELS.find(l => l.id === req.params.id)
  if (!level) { res.json(fail('level not found', 404)); return }
  res.json(ok(level))
})

adminMallLevelRouter.post('/mall/level', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const level: MemberLevel = {
    id: guid(),
    name: body.name,
    minPoints: body.minPoints ?? 0,
    discount: body.discount ?? 1.0,
    sort: body.sort ?? MEMBER_LEVELS.length + 1,
    status: body.status || 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  MEMBER_LEVELS.push(level)
  res.json(ok({ id: level.id }, 'created'))
})

adminMallLevelRouter.put('/mall/level/:id', (req, res) => {
  const level = MEMBER_LEVELS.find(l => l.id === req.params.id)
  if (!level) { res.json(fail('level not found', 404)); return }

  const body = req.body
  if (body.name) level.name = body.name
  if (body.minPoints !== undefined) level.minPoints = body.minPoints
  if (body.discount !== undefined) level.discount = body.discount
  if (body.sort !== undefined) level.sort = body.sort
  if (body.status !== undefined) level.status = body.status
  level.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminMallLevelRouter.delete('/mall/level/:id', (req, res) => {
  const idx = MEMBER_LEVELS.findIndex(l => l.id === req.params.id)
  if (idx === -1) { res.json(fail('level not found', 404)); return }
  MEMBER_LEVELS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
