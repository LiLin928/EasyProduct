// src/routes/site/video.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { VIDEOS } from '../../data/site-full.js'

export const siteVideoRouter = Router()

siteVideoRouter.get('/video/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  res.json(ok(paginate(VIDEOS, pageIndex, pageSize)))
})