// src/routes/site/download.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { DOWNLOADS } from '../../data/site-full.js'

export const siteDownloadRouter = Router()

siteDownloadRouter.get('/download/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  res.json(ok(paginate(DOWNLOADS, pageIndex, pageSize)))
})