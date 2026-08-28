import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { LOGIN_LOGS } from '../../data/ops-login-log.js'

export const adminOpsLoginLogRouter = Router()

adminOpsLoginLogRouter.get('/ops/login-log/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const status = req.query.status as string | undefined
  const startDate = req.query.startDate as string | undefined
  const endDate = req.query.endDate as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...LOGIN_LOGS]
  if (status) filtered = filtered.filter(r => r.status === status)
  if (startDate) filtered = filtered.filter(r => r.createdAt >= startDate)
  if (endDate) filtered = filtered.filter(r => r.createdAt <= endDate + 'T23:59:59')
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(r =>
      r.userName.toLowerCase().includes(kw) ||
      r.ip.includes(kw) ||
      r.location.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminOpsLoginLogRouter.get('/ops/login-log/:id', (req, res) => {
  const record = LOGIN_LOGS.find(r => r.id === req.params.id)
  if (!record) { res.json(ok(null)); return }
  res.json(ok(record))
})
