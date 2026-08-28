// src/routes/admin/site-video.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { VIDEOS, type SiteVideo } from '../../data/site-full.js'

export const adminSiteVideoRouter = Router()

adminSiteVideoRouter.get('/site/video/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...VIDEOS]
  if (title) filtered = filtered.filter(v => v.title.includes(title))
  if (status) filtered = filtered.filter(v => v.status === status)
  filtered.sort((a, b) => a.sort - b.sort)

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminSiteVideoRouter.post('/site/video', (req, res) => {
  const body = req.body
  if (!body.title?.trim()) { res.json(fail('title required')); return }

  const video: SiteVideo = {
    id: guid(),
    title: body.title,
    titleEn: body.titleEn || body.title,
    coverImage: body.coverImage || '',
    videoUrl: body.videoUrl || '',
    duration: body.duration ?? 0,
    viewCount: 0,
    status: 'draft',
    sort: body.sort ?? VIDEOS.length + 1,
    publishTime: '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  VIDEOS.push(video)
  res.json(ok({ id: video.id }, 'created'))
})

adminSiteVideoRouter.put('/site/video/:id', (req, res) => {
  const video = VIDEOS.find(v => v.id === req.params.id)
  if (!video) { res.json(fail('video not found', 404)); return }

  const body = req.body
  if (body.title) video.title = body.title
  if (body.titleEn !== undefined) video.titleEn = body.titleEn
  if (body.coverImage !== undefined) video.coverImage = body.coverImage
  if (body.videoUrl !== undefined) video.videoUrl = body.videoUrl
  if (body.duration !== undefined) video.duration = body.duration
  if (body.sort !== undefined) video.sort = body.sort
  if (body.status !== undefined) {
    video.status = body.status
    if (body.status === 'published' && !video.publishTime) video.publishTime = isoTime()
  }
  video.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminSiteVideoRouter.delete('/site/video/:id', (req, res) => {
  const idx = VIDEOS.findIndex(v => v.id === req.params.id)
  if (idx === -1) { res.json(fail('video not found', 404)); return }
  VIDEOS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
