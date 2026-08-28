import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { OPERATE_LOGS } from '../../data/ops-operate-log.js'

export const adminOpsOperateLogRouter = Router()

adminOpsOperateLogRouter.get('/ops/operate-log/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const module = req.query.module as string | undefined
  const status = req.query.status as string | undefined
  const startDate = req.query.startDate as string | undefined
  const endDate = req.query.endDate as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...OPERATE_LOGS]
  if (module) filtered = filtered.filter(r => r.module === module)
  if (status) filtered = filtered.filter(r => r.status === status)
  if (startDate) filtered = filtered.filter(r => r.createdAt >= startDate)
  if (endDate) filtered = filtered.filter(r => r.createdAt <= endDate + 'T23:59:59')
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(r =>
      r.userName.toLowerCase().includes(kw) ||
      r.action.toLowerCase().includes(kw) ||
      r.url.toLowerCase().includes(kw) ||
      r.ip.includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminOpsOperateLogRouter.get('/ops/operate-log/:id', (req, res) => {
  const record = OPERATE_LOGS.find(r => r.id === req.params.id)
  if (!record) { res.json(ok(null)); return }
  res.json(ok(record))
})
