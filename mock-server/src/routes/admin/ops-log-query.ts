import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { LOG_QUERIES } from '../../data/ops-log-query.js'

export const adminOpsLogQueryRouter = Router()

adminOpsLogQueryRouter.get('/ops/log-query/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const module = req.query.module as string | undefined
  const level = req.query.level as string | undefined
  const startDate = req.query.startDate as string | undefined
  const endDate = req.query.endDate as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...LOG_QUERIES]
  if (module) filtered = filtered.filter(r => r.module === module)
  if (level) filtered = filtered.filter(r => r.level === level)
  if (startDate) filtered = filtered.filter(r => r.createdAt >= startDate)
  if (endDate) filtered = filtered.filter(r => r.createdAt <= endDate + 'T23:59:59')
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(r =>
      r.message.toLowerCase().includes(kw) ||
      r.userName.toLowerCase().includes(kw) ||
      r.ip.includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminOpsLogQueryRouter.get('/ops/log-query/:id', (req, res) => {
  const record = LOG_QUERIES.find(r => r.id === req.params.id)
  if (!record) { res.json(ok(null)); return }
  res.json(ok(record))
})
