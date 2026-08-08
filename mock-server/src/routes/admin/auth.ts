// src/routes/admin/auth.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'
import { ADMIN_USERS, USER_PERMISSIONS } from '../../data/basic.js'

export const adminAuthRouter = Router()

adminAuthRouter.post('/auth/login', (req, res) => {
  const { userName, password } = req.body as { userName?: string; password?: string }
  const user = ADMIN_USERS.find((u) => u.userName === userName && u.password === password)
  if (!user) return res.json(fail('账号或密码错误'))
  res.json(ok({
    accessToken: guid(),
    refreshToken: guid(),
    user: { id: user.id, userName: user.userName, realName: user.realName },
    permissions: USER_PERMISSIONS[user.userName] ?? [],
  }))
})

adminAuthRouter.post('/auth/refresh', (req, res) => {
  const { refreshToken } = req.body as { refreshToken?: string }
  if (!refreshToken) return res.json(fail('refreshToken 不能为空'))
  res.json(ok({ accessToken: guid() }))
})
