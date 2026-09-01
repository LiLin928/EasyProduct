import { Router } from 'express'
import { workflowMocks } from '../data/workflow'

const router = Router()

// 获取工作流列表
router.get('/list', (req, res) => {
  res.json(workflowMocks.getList(req))
})

// 获取工作流详情
router.get('/detail', (req, res) => {
  res.json(workflowMocks.getDetail(req))
})

// 创建工作流
router.post('/create', (req, res) => {
  res.json(workflowMocks.create(req))
})

// 更新工作流
router.post('/update', (req, res) => {
  res.json(workflowMocks.update())
})

// 发布工作流
router.post('/publish', (req, res) => {
  res.json(workflowMocks.publish())
})

// 删除工作流
router.post('/delete', (req, res) => {
  res.json(workflowMocks.delete())
})

// 验证工作流
router.post('/validate', (req, res) => {
  res.json(workflowMocks.validate())
})

// 获取执行历史
router.get('/executions', (req, res) => {
  res.json(workflowMocks.getExecutions())
})

// 执行工作流
router.post('/execute', (req, res) => {
  res.json(workflowMocks.execute())
})

export default router
