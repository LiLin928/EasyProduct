// src/routes/admin/dept.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { DEPTS } from '../../data/admin/dept.js'
import { guid } from '../../helpers/id.js'

export const adminDeptRouter = Router()

adminDeptRouter.get('/basic/dept/list', (_req, res) => {
  res.json(ok(DEPTS))
})

adminDeptRouter.get('/basic/dept/:id', (req, res) => {
  const dept = DEPTS.find((d) => d.id === req.params.id)
  if (!dept) {
    res.json(fail('部门不存在', 404))
    return
  }
  res.json(ok(dept))
})

adminDeptRouter.post('/basic/dept', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

adminDeptRouter.put('/basic/dept/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

adminDeptRouter.delete('/basic/dept/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})
