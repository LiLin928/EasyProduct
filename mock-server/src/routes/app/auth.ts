// src/routes/app/auth.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'

export const appAuthRouter = Router()

/** 不校验真实 code：任意 code 返回固定会员（mock-guidelines 6.2） */
appAuthRouter.post('/auth/wx-login', (req, res) => {
  const { code } = req.body as { code?: string }
  if (!code) return res.json(fail('code 不能为空'))
  res.json(ok({
    memberToken: guid(),
    member: { id: guid(), nickName: '演示会员', avatar: '', level: 'normal', points: 0 },
  }))
})
