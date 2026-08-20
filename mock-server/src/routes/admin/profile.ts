// src/routes/admin/profile.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { ADMIN_USERS } from '../../data/admin/user.js'
import { isoTime } from '../../helpers/id.js'

export const adminProfileRouter = Router()

/** 获取当前用户信息（mock 固定返回 admin 账号） */
adminProfileRouter.get('/basic/profile', (_req, res) => {
  const admin = ADMIN_USERS[0]
  res.json(ok({
    id: admin.id,
    userName: admin.userName,
    realName: admin.realName,
    phone: '13800138000',
    email: 'admin@company.com',
    avatar: '',
    roles: ['super'],
    deptName: '总公司',
    lastLoginTime: isoTime(),
  }))
})

/** 更新当前用户基本信息 */
adminProfileRouter.put('/basic/profile', (req, res) => {
  const admin = ADMIN_USERS[0]
  const { realName, phone, email, avatar } = req.body || {}
  if (realName) admin.realName = realName
  // phone/email/avatar 在 mock 阶段不持久化到 ADMIN_USERS（无对应字段），仅返回成功
  res.json(ok(null, '更新成功'))
})

/** 修改密码（校验旧密码 = admin123） */
adminProfileRouter.put('/basic/profile/password', (req, res) => {
  const { oldPassword, newPassword } = req.body || {}
  if (!oldPassword || !newPassword) {
    res.json(fail('原密码和新密码不能为空', 400))
    return
  }
  if (oldPassword !== 'admin123') {
    res.json(fail('原密码不正确', 400))
    return
  }
  if (newPassword.length < 6) {
    res.json(fail('新密码长度不能少于 6 位', 400))
    return
  }
  ADMIN_USERS[0].password = newPassword
  res.json(ok(null, '密码修改成功'))
})