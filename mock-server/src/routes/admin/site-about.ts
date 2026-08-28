// src/routes/admin/site-about.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { isoTime } from '../../helpers/id.js'
import { ABOUT } from '../../data/site-full.js'

export const adminSiteAboutRouter = Router()

adminSiteAboutRouter.get('/site/about', (_req, res) => {
  res.json(ok(ABOUT))
})

adminSiteAboutRouter.put('/site/about', (req, res) => {
  const body = req.body
  if (body.title !== undefined) ABOUT.title = body.title
  if (body.titleEn !== undefined) ABOUT.titleEn = body.titleEn
  if (body.content !== undefined) ABOUT.content = body.content
  if (body.contentEn !== undefined) ABOUT.contentEn = body.contentEn
  ABOUT.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})
