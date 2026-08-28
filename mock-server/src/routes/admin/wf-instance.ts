import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { INSTANCES, HISTORIES } from '../../data/wf-seed.js'

export const adminWfInstanceRouter = Router()

// 流程实例列表
adminWfInstanceRouter.get('/wf/instance/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const status = req.query.status as string | undefined
  const applicantName = req.query.applicantName as string | undefined
  const definitionName = req.query.definitionName as string | undefined

  let filtered = [...INSTANCES]
  if (title) filtered = filtered.filter(i => i.title.toLowerCase().includes(title.toLowerCase()))
  if (status) filtered = filtered.filter(i => i.status === status)
  if (applicantName) filtered = filtered.filter(i => i.applicantName.includes(applicantName))
  if (definitionName) filtered = filtered.filter(i => i.definitionName.includes(definitionName))
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 流程实例详情
adminWfInstanceRouter.get('/wf/instance/:id', (req, res) => {
  const inst = INSTANCES.find(i => i.id === req.params.id)
  if (!inst) { res.json(fail('instance not found', 404)); return }
  res.json(ok(inst))
})

// 流程实例审批历史
adminWfInstanceRouter.get('/wf/instance/:id/history', (req, res) => {
  const history = HISTORIES
    .filter(h => h.instanceId === req.params.id)
    .sort((a, b) => a.createdAt.localeCompare(b.createdAt))
  res.json(ok(history))
})
