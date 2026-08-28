import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { TASKS } from '../../data/ops-task.js'
import { guid, isoTime } from '../../helpers/id.js'

export const adminOpsTaskRouter = Router()

adminOpsTaskRouter.get('/ops/task/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const status = req.query.status as string | undefined
  const keyword = req.query.keyword as string | undefined

  let filtered = [...TASKS]
  if (status) filtered = filtered.filter(r => r.status === status)
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(r =>
      r.taskName.toLowerCase().includes(kw) ||
      r.className.toLowerCase().includes(kw) ||
      r.taskGroup.toLowerCase().includes(kw),
    )
  }
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminOpsTaskRouter.get('/ops/task/:id', (req, res) => {
  const task = TASKS.find(r => r.id === req.params.id)
  if (!task) { res.json(ok(null)); return }
  res.json(ok(task))
})

adminOpsTaskRouter.post('/ops/task', (req, res) => {
  const body = req.body
  const newTask = {
    id: guid(),
    taskName: body.taskName ?? '',
    taskGroup: body.taskGroup ?? 'system',
    cron: body.cron ?? '0 0 * * * ?',
    className: body.className ?? '',
    methodName: body.methodName ?? 'Execute',
    description: body.description ?? '',
    status: 'paused' as const,
    lastRunTime: '',
    nextRunTime: '',
    createdAt: isoTime(0),
    updatedAt: isoTime(0),
  }
  TASKS.unshift(newTask)
  res.json(ok({ id: newTask.id }))
})

adminOpsTaskRouter.put('/ops/task/:id', (req, res) => {
  const task = TASKS.find(r => r.id === req.params.id)
  if (!task) { res.json(fail('Task not found', 404)); return }
  const body = req.body
  if (body.taskName !== undefined) task.taskName = body.taskName
  if (body.taskGroup !== undefined) task.taskGroup = body.taskGroup
  if (body.cron !== undefined) task.cron = body.cron
  if (body.className !== undefined) task.className = body.className
  if (body.methodName !== undefined) task.methodName = body.methodName
  if (body.description !== undefined) task.description = body.description
  task.updatedAt = isoTime(0)
  res.json(ok(null))
})

adminOpsTaskRouter.post('/ops/task/:id/status', (req, res) => {
  const task = TASKS.find(r => r.id === req.params.id)
  if (!task) { res.json(fail('Task not found', 404)); return }
  const status = req.body.status
  if (status !== 'running' && status !== 'paused') {
    res.json(fail('Invalid status', 400)); return
  }
  task.status = status
  task.updatedAt = isoTime(0)
  res.json(ok(null))
})

adminOpsTaskRouter.delete('/ops/task/:id', (req, res) => {
  const idx = TASKS.findIndex(r => r.id === req.params.id)
  if (idx < 0) { res.json(fail('Task not found', 404)); return }
  TASKS.splice(idx, 1)
  res.json(ok(null))
})
