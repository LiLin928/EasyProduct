// src/routes/site/about.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { ABOUT } from '../../data/site-full.js'

export const siteAboutRouter = Router()

// 获取关于我们详情（官网公开）
siteAboutRouter.get('/about', (_req, res) => {
  // 只返回启用状态的关于我们内容
  if (ABOUT.status === 1) {
    res.json(ok(ABOUT))
  } else {
    res.json(ok(null))
  }
})