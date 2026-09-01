import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { DEFINITIONS, type WorkflowDefinition } from '../../data/wf-definition.js'

export const adminWfDefinitionRouter = Router()

// 流程定义列表
adminWfDefinitionRouter.get('/wf/definition/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const code = req.query.code as string | undefined
  const category = req.query.category as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...DEFINITIONS]
  if (name) filtered = filtered.filter(d => d.name.toLowerCase().includes(name.toLowerCase()))
  if (code) filtered = filtered.filter(d => d.code.toLowerCase().includes(code.toLowerCase()))
  if (category) filtered = filtered.filter(d => d.category.toLowerCase().includes(category.toLowerCase()))
  if (status) filtered = filtered.filter(d => d.status === status)
  filtered.sort((a, b) => b.updatedAt.localeCompare(a.updatedAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 获取流程定义详情
adminWfDefinitionRouter.get('/wf/definition/detail', (req, res) => {
  const id = req.query.id as string
  if (!id) { res.json(fail('id required')); return }
  
  const def = DEFINITIONS.find(d => d.id === id)
  if (!def) { res.json(fail('definition not found', 404)); return }
  
  // 返回完整的流程数据（包含 nodes 和 edges）
  res.json(ok({
    ...def,
    // 如果没有节点数据，提供默认节点
    nodes: (def as any).nodes || [
      {
        id: 'node-start',
        type: 'start',
        name: '开始',
        position: { x: 100, y: 200 },
        data: { rows: [] }
      },
      {
        id: 'node-end',
        type: 'end',
        name: '结束',
        position: { x: 400, y: 200 },
        data: { rows: [] }
      }
    ],
    edges: (def as any).edges || []
  }))
})

// 新建流程定义
adminWfDefinitionRouter.post('/wf/definition', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }
  if (!body.code?.trim()) { res.json(fail('code required')); return }
  if (DEFINITIONS.some(d => d.code === body.code)) { res.json(fail('code already exists')); return }

  const def: WorkflowDefinition = {
    id: guid(),
    name: body.name,
    code: body.code,
    category: body.category || '',
    description: body.description || '',
    status: 'draft',
    version: 1,
    createdBy: 'admin',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  DEFINITIONS.unshift(def)
  res.json(ok({ id: def.id }, 'created'))
})

// 更新流程定义
adminWfDefinitionRouter.put('/wf/definition/:id', (req, res) => {
  const def = DEFINITIONS.find(d => d.id === req.params.id)
  if (!def) { res.json(fail('definition not found', 404)); return }
  if (def.status === 'published') { res.json(fail('published definition cannot be edited')); return }

  const body = req.body
  if (body.name !== undefined) def.name = body.name
  if (body.code !== undefined) def.code = body.code
  if (body.category !== undefined) def.category = body.category
  if (body.description !== undefined) def.description = body.description
  def.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

// 删除流程定义
adminWfDefinitionRouter.delete('/wf/definition/:id', (req, res) => {
  const idx = DEFINITIONS.findIndex(d => d.id === req.params.id)
  if (idx === -1) { res.json(fail('definition not found', 404)); return }
  if (DEFINITIONS[idx].status === 'published') { res.json(fail('published definition cannot be deleted')); return }
  DEFINITIONS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})

// 发布流程定义
adminWfDefinitionRouter.post('/wf/definition/:id/publish', (req, res) => {
  const def = DEFINITIONS.find(d => d.id === req.params.id)
  if (!def) { res.json(fail('definition not found', 404)); return }
  if (def.status === 'published') { res.json(fail('already published')); return }
  def.status = 'published'
  def.version += 1
  def.updatedAt = isoTime()
  res.json(ok({ status: def.status, version: def.version }, 'published'))
})

// 停用流程定义
adminWfDefinitionRouter.post('/wf/definition/:id/disable', (req, res) => {
  const def = DEFINITIONS.find(d => d.id === req.params.id)
  if (!def) { res.json(fail('definition not found', 404)); return }
  if (def.status !== 'published') { res.json(fail('only published definition can be disabled')); return }
  def.status = 'disabled'
  def.updatedAt = isoTime()
  res.json(ok({ status: def.status }, 'disabled'))
})

// 保存流程图
adminWfDefinitionRouter.post('/wf/definition/:id/graph', (req, res) => {
  const def = DEFINITIONS.find(d => d.id === req.params.id)
  if (!def) { res.json(fail('definition not found', 404)); return }
  
  const body = req.body
  // 更新流程图数据
  ;(def as any).nodes = body.nodes
  ;(def as any).edges = body.edges
  if (body.name) def.name = body.name
  def.updatedAt = isoTime()
  
  res.json(ok(null, 'saved'))
})
