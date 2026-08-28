// src/routes/admin/site-banner.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { BANNERS, type SiteBanner } from '../../data/site.js'

export const adminSiteBannerRouter = Router()

adminSiteBannerRouter.get('/site/banner/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...BANNERS]
  if (title) filtered = filtered.filter(b => b.title.includes(title))
  if (status) filtered = filtered.filter(b => b.status === status)
  filtered.sort((a, b) => a.sort - b.sort)

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminSiteBannerRouter.post('/site/banner', (req, res) => {
  const body = req.body
  if (!body.title?.trim()) { res.json(fail('title required')); return }

  const banner: SiteBanner = {
    id: guid(),
    title: body.title,
    titleEn: body.titleEn || body.title,
    imageUrl: body.imageUrl || '',
    link: body.link || '',
    sort: body.sort ?? BANNERS.length + 1,
    status: 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  BANNERS.push(banner)
  res.json(ok({ id: banner.id }, 'created'))
})

adminSiteBannerRouter.put('/site/banner/:id', (req, res) => {
  const banner = BANNERS.find(b => b.id === req.params.id)
  if (!banner) { res.json(fail('banner not found', 404)); return }

  const body = req.body
  if (body.title) banner.title = body.title
  if (body.titleEn !== undefined) banner.titleEn = body.titleEn
  if (body.imageUrl !== undefined) banner.imageUrl = body.imageUrl
  if (body.link !== undefined) banner.link = body.link
  if (body.sort !== undefined) banner.sort = body.sort
  if (body.status !== undefined) banner.status = body.status
  banner.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminSiteBannerRouter.delete('/site/banner/:id', (req, res) => {
  const idx = BANNERS.findIndex(b => b.id === req.params.id)
  if (idx === -1) { res.json(fail('banner not found', 404)); return }
  BANNERS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
