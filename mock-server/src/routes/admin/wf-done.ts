import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { TASKS } from '../../data/wf-seed.js'

export const adminWfDoneRouter = Router()

// 已办列表（当前用户非 pending 任务）
adminWfDoneRouter.get('/wf/done/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const instanceTitle = req.query.instanceTitle as string | undefined
  const status = req.query.status as string | undefined

  let filtered = TASKS.filter(t => t.status !== 'pending' && t.assigneeId === 'user-admin')
  if (instanceTitle) filtered = filtered.filter(t => t.instanceTitle.toLowerCase().includes(instanceTitle.toLowerCase()))
  if (status) filtered = filtered.filter(t => t.status === status)
  filtered.sort((a, b) => b.finishedAt.localeCompare(a.finishedAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})
