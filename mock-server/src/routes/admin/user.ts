// src/routes/admin/user.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ADMIN_USERS } from '../../data/admin/user.js'
import { guid } from '../../helpers/id.js'

export const adminUserRouter = Router()

adminUserRouter.get('/basic/user/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const userName = req.query.userName as string | undefined
  const realName = req.query.realName as string | undefined
  const status = req.query.status as string | undefined

  let filtered = ADMIN_USERS

  // 按用户名搜索
  if (userName) {
    filtered = filtered.filter((u) => u.userName.includes(userName))
  }

  // 按真实姓名搜索
  if (realName) {
    filtered = filtered.filter((u) => u.realName.includes(realName))
  }

  // 按状态筛选
  if (status) {
    filtered = filtered.filter((u) => u.status === status)
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminUserRouter.get('/basic/user/:id', (req, res) => {
  const user = ADMIN_USERS.find((u) => u.id === req.params.id)
  if (!user) {
    res.json(fail('用户不存在', 404))
    return
  }
  res.json(ok(user))
})

adminUserRouter.post('/basic/user', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

adminUserRouter.put('/basic/user/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

adminUserRouter.delete('/basic/user/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})

// 重置密码
adminUserRouter.post('/basic/user/:id/reset-password', (req, res) => {
  const user = ADMIN_USERS.find((u) => u.id === req.params.id)
  if (!user) {
    res.json(fail('用户不存在', 404))
    return
  }
  res.json(ok(null, '密码已重置为：123456'))
})