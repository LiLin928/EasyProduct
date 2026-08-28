import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { TASK_LOGS } from '../../data/ops-task-log.js'

export const adminOpsTaskLogRouter = Router()

adminOpsTaskLogRouter.get('/ops/task-log/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const taskId = req.query.taskId as string | undefined
  const status = req.query.status as string | undefined
  const startDate = req.query.startDate as string | undefined
  const endDate = req.query.endDate as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...TASK_LOGS]
  if (taskId) filtered = filtered.filter(r => r.taskId === taskId)
  if (status) filtered = filtered.filter(r => r.status === status)
  if (startDate) filtered = filtered.filter(r => r.createdAt >= startDate)
  if (endDate) filtered = filtered.filter(r => r.createdAt <= endDate + 'T23:59:59')
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(r =>
      r.taskName.toLowerCase().includes(kw) ||
      r.errorMessage.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminOpsTaskLogRouter.get('/ops/task-log/:id', (req, res) => {
  const record = TASK_LOGS.find(r => r.id === req.params.id)
  if (!record) { res.json(ok(null)); return }
  res.json(ok(record))
})
