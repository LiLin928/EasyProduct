import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { isoTime } from '../../helpers/id.js'
import { TASKS } from '../../data/wf-seed.js'

export const adminWfTodoRouter = Router()

// 待办列表（当前用户 pending 任务）
adminWfTodoRouter.get('/wf/todo/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const instanceTitle = req.query.instanceTitle as string | undefined
  const definitionName = req.query.definitionName as string | undefined

  let filtered = TASKS.filter(t => t.status === 'pending' && t.assigneeId === 'user-admin')
  if (instanceTitle) filtered = filtered.filter(t => t.instanceTitle.toLowerCase().includes(instanceTitle.toLowerCase()))
  if (definitionName) filtered = filtered.filter(t => t.definitionName.includes(definitionName))
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 审批通过
adminWfTodoRouter.post('/wf/todo/:id/approve', (req, res) => {
  const task = TASKS.find(t => t.id === req.params.id)
  if (!task) { res.json(fail('task not found', 404)); return }
  task.status = 'approved'
  task.comment = req.body.comment || ''
  task.finishedAt = isoTime()
  res.json(ok(null, 'approved'))
})

// 审批拒绝
adminWfTodoRouter.post('/wf/todo/:id/reject', (req, res) => {
  const task = TASKS.find(t => t.id === req.params.id)
  if (!task) { res.json(fail('task not found', 404)); return }
  task.status = 'rejected'
  task.comment = req.body.comment || ''
  task.finishedAt = isoTime()
  res.json(ok(null, 'rejected'))
})

// 转办
adminWfTodoRouter.post('/wf/todo/:id/transfer', (req, res) => {
  const task = TASKS.find(t => t.id === req.params.id)
  if (!task) { res.json(fail('task not found', 404)); return }
  task.status = 'transferred'
  task.comment = req.body.comment || ''
  task.finishedAt = isoTime()
  res.json(ok(null, 'transferred'))
})
