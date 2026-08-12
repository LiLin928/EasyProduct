// src/routes/admin/role.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ROLES } from '../../data/admin/role.js'
import { guid } from '../../helpers/id.js'

export const adminRoleRouter = Router()

adminRoleRouter.get('/basic/role/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const code = req.query.code as string | undefined
  const status = req.query.status as string | undefined

  let filtered = ROLES

  // 按角色名称搜索
  if (name) {
    filtered = filtered.filter((r) => r.name.includes(name))
  }

  // 按角色编码搜索
  if (code) {
    filtered = filtered.filter((r) => r.code.includes(code))
  }

  // 按状态筛选
  if (status) {
    filtered = filtered.filter((r) => r.status === status)
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
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

// 分配菜单
adminRoleRouter.post('/basic/role/:id/menus', (req, res) => {
  const role = ROLES.find((r) => r.id === req.params.id)
  if (!role) {
    res.json(fail('角色不存在', 404))
    return
  }
  res.json(ok(null, '分配菜单成功'))
})

// 获取角色的菜单ID列表
adminRoleRouter.get('/basic/role/:id/menus', (req, res) => {
  const role = ROLES.find((r) => r.id === req.params.id)
  if (!role) {
    res.json(fail('角色不存在', 404))
    return
  }
  res.json(ok(role.menuIds || []))
})