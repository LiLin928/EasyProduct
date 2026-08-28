import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { INSTANCES, type WorkflowInstance } from '../../data/wf-seed.js'
import { DEFINITIONS } from '../../data/wf-definition.js'

export const adminWfMyApplyRouter = Router()

// 我的申请列表
adminWfMyApplyRouter.get('/wf/my-apply/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const status = req.query.status as string | undefined
  const businessType = req.query.businessType as string | undefined

  let filtered = [...INSTANCES]
  if (title) filtered = filtered.filter(i => i.title.toLowerCase().includes(title.toLowerCase()))
  if (status) filtered = filtered.filter(i => i.status === status)
  if (businessType) filtered = filtered.filter(i => i.businessType === businessType)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 发起申请
adminWfMyApplyRouter.post('/wf/my-apply', (req, res) => {
  const body = req.body
  if (!body.definitionId?.trim()) { res.json(fail('definitionId required')); return }

  const def = DEFINITIONS.find(d => d.id === body.definitionId)
  if (!def) { res.json(fail('definition not found', 404)); return }

  const inst: WorkflowInstance = {
    id: guid(),
    definitionId: def.id,
    definitionName: def.name,
    definitionCode: def.code,
    businessId: `WF${isoTime().slice(0, 10).replace(/-/g, '')}${Math.floor(Math.random() * 1000)}`,
    businessType: body.businessType || def.category,
    title: body.title || def.name + '申请',
    applicantId: 'user-admin',
    applicantName: '管理员',
    currentNode: '部门经理审批',
    status: 'running',
    createdAt: isoTime(),
    finishedAt: '',
  }
  INSTANCES.unshift(inst)
  res.json(ok({ id: inst.id }, 'created'))
})

// 撤销申请
adminWfMyApplyRouter.delete('/wf/my-apply/:id', (req, res) => {
  const inst = INSTANCES.find(i => i.id === req.params.id)
  if (!inst) { res.json(fail('instance not found', 404)); return }
  if (inst.status !== 'running') { res.json(fail('只能撤销运行中的申请')); return }
  inst.status = 'cancelled'
  inst.currentNode = ''
  inst.finishedAt = isoTime()
  res.json(ok(null, 'cancelled'))
})

// 流程定义选项（仅已发布）
adminWfMyApplyRouter.get('/wf/definition/options', (_req, res) => {
  const options = DEFINITIONS
    .filter(d => d.status === 'published')
    .map(d => ({ id: d.id, name: d.name, code: d.code, category: d.category }))
  res.json(ok(options))
})
