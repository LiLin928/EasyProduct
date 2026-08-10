// src/routes/admin/role.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ROLES } from '../../data/admin/role.js'
import { guid } from '../../helpers/id.js'

export const adminRoleRouter = Router()

adminRoleRouter.get('/basic/role/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  res.json(ok(paginate(ROLES, pageIndex, pageSize)))
})

adminRoleRouter.get('/basic/role/:id', (req, res) => {
  const role = ROLES.find((r) => r.id === req.params.id)
  if (!role) {
    res.json(fail('角色不存在', 404))
    return
  }
  res.json(ok(role))
})

adminRoleRouter.post('/basic/role', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

adminRoleRouter.put('/basic/role/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

adminRoleRouter.delete('/basic/role/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})
