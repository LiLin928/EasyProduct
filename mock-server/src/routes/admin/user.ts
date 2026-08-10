// src/routes/admin/user.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ADMIN_USERS } from '../../data/admin/user.js'
import { guid } from '../../helpers/id.js'

export const adminUserRouter = Router()

adminUserRouter.get('/basic/user/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined

  let filtered = ADMIN_USERS
  if (keyword) {
    filtered = filtered.filter(
      (u) =>
        u.userName.includes(keyword) ||
        u.realName.includes(keyword) ||
        u.email.includes(keyword),
    )
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
