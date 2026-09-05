// src/routes/app/auth.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { MEMBERS } from '../../data/mall.js'

export const appAuthRouter = Router()

/**
 * 手机号密码登录
 * POST /api/app/auth/login
 */
appAuthRouter.post('/auth/login', (req, res) => {
  const { phone, password } = req.body as { phone?: string; password?: string }
  
  if (!phone) {
    return res.json(fail('手机号不能为空', 400))
  }
  if (!/^1[3-9]\d{9}$/.test(phone)) {
    return res.json(fail('手机号格式不正确', 400))
  }
  if (!password) {
    return res.json(fail('密码不能为空', 400))
  }
  if (password.length < 6) {
    return res.json(fail('密码长度至少6位', 400))
  }
  
  // mock 阶段：查找对应手机号的会员，没有则返回第一个
  const member = MEMBERS.find(m => m.phone === phone) || MEMBERS[0]
  
  res.json(ok({
    memberToken: `mock-member-token-${member.id}`,
    member: {
      id: member.id,
      nickName: member.nickname,
      avatar: member.avatar,
      level: member.levelName,
      points: member.points,
    },
  }))
})

/**
 * 微信一键登录
 * POST /api/app/auth/wx-login
 * 不校验真实 code：任意 code 返回固定会员（mock-guidelines 6.2）
 */
appAuthRouter.post('/auth/wx-login', (req, res) => {
  const { code } = req.body as { code?: string }
  if (!code) return res.json(fail('code 不能为空'))
  const member = MEMBERS[0]
  res.json(ok({
    memberToken: `mock-member-token-${member.id}`,
    member: {
      id: member.id,
      nickName: member.nickname,
      avatar: member.avatar,
      level: member.levelName,
      points: member.points,
    },
  }))
})
