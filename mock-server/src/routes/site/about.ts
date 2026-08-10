// src/routes/site/about.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { ABOUT, CONTACT_INFO } from '../../data/site-full.js'

export const siteAboutRouter = Router()

// 关于单页
siteAboutRouter.get('/about/detail', (_req, res) => {
  res.json(ok(ABOUT))
})

// 联系信息
siteAboutRouter.get('/contact/info', (_req, res) => {
  res.json(ok(CONTACT_INFO))
})