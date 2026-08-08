// src/routes/site/home.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { BANNERS, NEWS } from '../../data/site.js'

export const siteHomeRouter = Router()

siteHomeRouter.get('/banner/list', (_req, res) => res.json(ok(BANNERS)))

siteHomeRouter.get('/news/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined
  res.json(ok(paginate(NEWS, pageIndex, pageSize, keyword)))
})
