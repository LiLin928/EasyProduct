// src/routes/admin/mall-member.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { MEMBERS, MEMBER_LEVELS, type Member } from '../../data/mall.js'

export const adminMallMemberRouter = Router()

adminMallMemberRouter.get('/mall/member/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const nickname = req.query.nickname as string | undefined
  const phone = req.query.phone as string | undefined
  const levelId = req.query.levelId as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...MEMBERS]
  if (nickname) filtered = filtered.filter(m => m.nickname.includes(nickname))
  if (phone) filtered = filtered.filter(m => m.phone.includes(phone))
  if (levelId) filtered = filtered.filter(m => m.levelId === levelId)
  if (status) filtered = filtered.filter(m => m.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminMallMemberRouter.get('/mall/member/:id', (req, res) => {
  const member = MEMBERS.find(m => m.id === req.params.id)
  if (!member) { res.json(fail('member not found', 404)); return }
  res.json(ok(member))
})

adminMallMemberRouter.post('/mall/member', (req, res) => {
  const body = req.body
  if (!body.nickname?.trim()) { res.json(fail('nickname required')); return }

  const level = MEMBER_LEVELS.find(l => l.id === body.levelId) || MEMBER_LEVELS[0]
  const member: Member = {
    id: guid(),
    nickname: body.nickname,
    avatar: body.avatar || '',
    phone: body.phone || '',
    openid: '',
    levelId: level.id,
    levelName: level.name,
    points: 0,
    totalSpent: 0,
    orderCount: 0,
    status: body.status || 'active',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  MEMBERS.unshift(member)
  res.json(ok({ id: member.id }, 'created'))
})

adminMallMemberRouter.put('/mall/member/:id', (req, res) => {
  const member = MEMBERS.find(m => m.id === req.params.id)
  if (!member) { res.json(fail('member not found', 404)); return }

  const body = req.body
  if (body.nickname) member.nickname = body.nickname
  if (body.phone !== undefined) member.phone = body.phone
  if (body.levelId !== undefined) {
    const level = MEMBER_LEVELS.find(l => l.id === body.levelId)
    if (level) { member.levelId = level.id; member.levelName = level.name }
  }
  if (body.status !== undefined) member.status = body.status
  member.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminMallMemberRouter.delete('/mall/member/:id', (req, res) => {
  const idx = MEMBERS.findIndex(m => m.id === req.params.id)
  if (idx === -1) { res.json(fail('member not found', 404)); return }
  MEMBERS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})

adminMallMemberRouter.get('/mall/member/level/options', (_req, res) => {
  res.json(ok(MEMBER_LEVELS.map(l => ({ id: l.id, name: l.name }))))
})
