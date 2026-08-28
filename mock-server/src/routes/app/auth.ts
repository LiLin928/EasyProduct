// src/routes/app/auth.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { MEMBERS } from '../../data/mall.js'

export const appAuthRouter = Router()

/** 不校验真实 code：任意 code 返回固定会员（mock-guidelines 6.2） */
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
