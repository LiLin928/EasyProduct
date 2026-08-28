// src/routes/admin/wf-designer.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'
import { DESIGNER_GRAPHS, type DesignerGraph } from '../../data/wf-designer.js'

export const adminWfDesignerRouter = Router()

// 获取流程设计器画布数据
adminWfDesignerRouter.get('/wf/designer/graph', (req, res) => {
  const definitionId = req.query.definitionId as string | undefined
  const graph = DESIGNER_GRAPHS['default']
  const result: DesignerGraph = {
    ...graph,
    definitionId: definitionId || graph.definitionId,
  }
  res.json(ok(result))
})

// 保存流程设计器画布数据
adminWfDesignerRouter.post('/wf/designer/graph', (req, res) => {
  const body = req.body as Partial<DesignerGraph>
  if (!body || !Array.isArray(body.nodes) || !Array.isArray(body.edges)) {
    res.json(fail('invalid graph data'))
    return
  }
  const graph: DesignerGraph = {
    definitionId: body.definitionId || '',
    definitionName: body.definitionName || '',
    nodes: body.nodes.map((n) => ({ ...n, id: n.id || guid() })),
    edges: body.edges.map((e) => ({ ...e, id: e.id || guid() })),
  }
  DESIGNER_GRAPHS['default'] = graph
  res.json(ok(null, 'saved'))
})

// 校验流程图（简单校验：必须有 start 和 end 节点）
adminWfDesignerRouter.post('/wf/designer/validate', (req, res) => {
  const body = req.body as Partial<DesignerGraph>
  if (!body || !Array.isArray(body.nodes) || body.nodes.length === 0) {
    res.json(fail('graph is empty'))
    return
  }
  const hasStart = body.nodes.some((n) => n.type === 'start')
  const hasEnd = body.nodes.some((n) => n.type === 'end')
  if (!hasStart) { res.json(fail('missing start node')); return }
  if (!hasEnd) { res.json(fail('missing end node')); return }
  res.json(ok(null, 'valid'))
})
