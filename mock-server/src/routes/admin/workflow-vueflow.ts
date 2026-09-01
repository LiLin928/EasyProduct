// src/routes/admin/workflow-vueflow.ts
// VueFlow 工作流 API 路由

import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'
import { WORKFLOW_LIST, EXECUTION_HISTORY, NODE_TYPES, type VueFlowWorkflow } from '../../data/workflow-vueflow.js'

export const adminWorkflowVueflowRouter = Router()

// 获取工作流列表
adminWorkflowVueflowRouter.get('/workflow/list', (req, res) => {
  const { keyword, pageIndex = 1, pageSize = 10 } = req.query as any
  let list = [...WORKFLOW_LIST]

  if (keyword) {
    const kw = keyword.toLowerCase()
    list = list.filter(w =>
      w.name.toLowerCase().includes(kw) ||
      (w.description || '').toLowerCase().includes(kw)
    )
  }

  const start = (Number(pageIndex) - 1) * Number(pageSize)
  const end = start + Number(pageSize)

  res.json(ok({
    list: list.slice(start, end),
    total: list.length
  }))
})

// 获取工作流详情
adminWorkflowVueflowRouter.get('/workflow/detail', (req, res) => {
  const { id } = req.query as any
  const workflow = WORKFLOW_LIST.find(w => w.id === id)
  if (workflow) {
    res.json(ok(workflow))
  } else {
    // 返回第一个作为默认
    res.json(ok(WORKFLOW_LIST[0]))
  }
})

// 创建工作流
adminWorkflowVueflowRouter.post('/workflow/create', (req, res) => {
  const { name, description } = req.body
  const now = new Date().toISOString()
  const newWorkflow: VueFlowWorkflow = {
    id: 'wf-' + guid(),
    name: name || '新流程',
    description,
    status: 'draft',
    version: 1,
    nodes: [
      { id: 'node-' + guid(), type: 'start', name: '开始', position: { x: 100, y: 200 }, data: { rows: [] } },
      { id: 'node-' + guid(), type: 'end', name: '结束', position: { x: 400, y: 200 }, data: { rows: [] } }
    ],
    edges: [],
    createdAt: now,
    updatedAt: now
  }
  WORKFLOW_LIST.push(newWorkflow)
  res.json(ok(newWorkflow))
})

// 更新工作流
adminWorkflowVueflowRouter.post('/workflow/update', (req, res) => {
  const { id, name, nodes, edges } = req.body
  const workflow = WORKFLOW_LIST.find(w => w.id === id)
  if (workflow) {
    if (name) workflow.name = name
    if (nodes) workflow.nodes = nodes
    if (edges) workflow.edges = edges
    workflow.updatedAt = new Date().toISOString()
    res.json(ok(null, '更新成功'))
  } else {
    res.json(fail('工作流不存在'))
  }
})

// 发布工作流
adminWorkflowVueflowRouter.post('/workflow/publish', (req, res) => {
  const { id } = req.body
  const workflow = WORKFLOW_LIST.find(w => w.id === id)
  if (workflow) {
    workflow.status = 'published'
    workflow.version += 1
    workflow.updatedAt = new Date().toISOString()
    res.json(ok({ status: 'published', version: workflow.version }))
  } else {
    res.json(fail('工作流不存在'))
  }
})

// 删除工作流
adminWorkflowVueflowRouter.post('/workflow/delete', (req, res) => {
  const { id } = req.body
  const index = WORKFLOW_LIST.findIndex(w => w.id === id)
  if (index > -1) {
    WORKFLOW_LIST.splice(index, 1)
    res.json(ok(null, '删除成功'))
  } else {
    res.json(fail('工作流不存在'))
  }
})

// 验证工作流
adminWorkflowVueflowRouter.post('/workflow/validate', (req, res) => {
  const { nodes } = req.body
  const hasStart = nodes?.some((n: any) => n.type === 'start')
  const hasEnd = nodes?.some((n: any) => n.type === 'end')
  const errors: string[] = []
  if (!hasStart) errors.push('缺少开始节点')
  if (!hasEnd) errors.push('缺少结束节点')
  res.json(ok({ valid: errors.length === 0, errors }))
})

// 获取执行历史
adminWorkflowVueflowRouter.get('/workflow/executions', (req, res) => {
  res.json(ok({ list: EXECUTION_HISTORY, total: EXECUTION_HISTORY.length }))
})

// 执行工作流
adminWorkflowVueflowRouter.post('/workflow/execute', (req, res) => {
  const { id } = req.body
  res.json(ok({ execId: 'exec-' + guid() }))
})

// 获取节点类型配置
adminWorkflowVueflowRouter.get('/workflow/node-types', (req, res) => {
  res.json(ok(NODE_TYPES))
})
