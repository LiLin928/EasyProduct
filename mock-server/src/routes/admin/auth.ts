// src/routes/admin/auth.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'
import { ADMIN_USERS, USER_PERMISSIONS, MENU_TREE } from '../../data/basic.js'

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

// 获取当前用户信息（用于刷新页面后恢复登录状态）
adminAuthRouter.get('/auth/user-info', (req, res) => {
  // Mock: 返回 admin 用户信息
  const user = ADMIN_USERS.find((u) => u.userName === 'admin')
  if (!user) return res.json(fail('用户不存在', 404))
  res.json(ok({
    user: { id: user.id, userName: user.userName, realName: user.realName },
    permissions: USER_PERMISSIONS['admin'] ?? [],
  }))
})

// 获取当前用户菜单树
adminAuthRouter.get('/auth/menu-list', (req, res) => {
  // TODO: 根据用户权限过滤菜单（当前返回所有菜单）
  res.json(ok(MENU_TREE))
})