// src/routes/admin/mall-coupon.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { COUPONS, type Coupon } from '../../data/mall.js'

export const adminMallCouponRouter = Router()

adminMallCouponRouter.get('/mall/coupon/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const type = req.query.type as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...COUPONS]
  if (name) filtered = filtered.filter(c => c.name.includes(name))
  if (type) filtered = filtered.filter(c => c.type === type)
  if (status) filtered = filtered.filter(c => c.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminMallCouponRouter.get('/mall/coupon/:id', (req, res) => {
  const coupon = COUPONS.find(c => c.id === req.params.id)
  if (!coupon) { res.json(fail('coupon not found', 404)); return }
  res.json(ok(coupon))
})

adminMallCouponRouter.post('/mall/coupon', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }
  if (!body.type) { res.json(fail('type required')); return }
  if (body.value === undefined) { res.json(fail('value required')); return }

  const coupon: Coupon = {
    id: guid(),
    name: body.name,
    type: body.type,
    value: body.value,
    minSpend: body.minSpend ?? 0,
    totalCount: body.totalCount ?? 0,
    issuedCount: 0,
    usedCount: 0,
    startDate: body.startDate || new Date().toISOString().slice(0, 10),
    endDate: body.endDate || new Date(Date.now() + 86400000 * 30).toISOString().slice(0, 10),
    status: body.status || 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  COUPONS.unshift(coupon)
  res.json(ok({ id: coupon.id }, 'created'))
})

adminMallCouponRouter.put('/mall/coupon/:id', (req, res) => {
  const coupon = COUPONS.find(c => c.id === req.params.id)
  if (!coupon) { res.json(fail('coupon not found', 404)); return }

  const body = req.body
  if (body.name) coupon.name = body.name
  if (body.type !== undefined) coupon.type = body.type
  if (body.value !== undefined) coupon.value = body.value
  if (body.minSpend !== undefined) coupon.minSpend = body.minSpend
  if (body.totalCount !== undefined) coupon.totalCount = body.totalCount
  if (body.startDate !== undefined) coupon.startDate = body.startDate
  if (body.endDate !== undefined) coupon.endDate = body.endDate
  if (body.status !== undefined) coupon.status = body.status
  coupon.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminMallCouponRouter.delete('/mall/coupon/:id', (req, res) => {
  const idx = COUPONS.findIndex(c => c.id === req.params.id)
  if (idx === -1) { res.json(fail('coupon not found', 404)); return }
  if (COUPONS[idx].issuedCount > 0) {
    res.json(fail('coupon already issued, cannot delete'))
    return
  }
  COUPONS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
