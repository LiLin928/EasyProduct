// src/routes/admin/site-download.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { DOWNLOADS, type SiteDownload } from '../../data/site-full.js'

export const adminSiteDownloadRouter = Router()

adminSiteDownloadRouter.get('/site/download/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...DOWNLOADS]
  if (title) filtered = filtered.filter(d => d.title.includes(title))
  if (status) filtered = filtered.filter(d => d.status === status)
  filtered.sort((a, b) => a.sort - b.sort)

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminSiteDownloadRouter.post('/site/download', (req, res) => {
  const body = req.body
  if (!body.title?.trim()) { res.json(fail('title required')); return }

  const dl: SiteDownload = {
    id: guid(),
    title: body.title,
    titleEn: body.titleEn || body.title,
    fileUrl: body.fileUrl || '',
    fileSize: body.fileSize ?? 0,
    downloadCount: 0,
    status: 'draft',
    sort: body.sort ?? DOWNLOADS.length + 1,
    publishTime: '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  DOWNLOADS.push(dl)
  res.json(ok({ id: dl.id }, 'created'))
})

adminSiteDownloadRouter.put('/site/download/:id', (req, res) => {
  const dl = DOWNLOADS.find(d => d.id === req.params.id)
  if (!dl) { res.json(fail('download not found', 404)); return }

  const body = req.body
  if (body.title) dl.title = body.title
  if (body.titleEn !== undefined) dl.titleEn = body.titleEn
  if (body.fileUrl !== undefined) dl.fileUrl = body.fileUrl
  if (body.fileSize !== undefined) dl.fileSize = body.fileSize
  if (body.sort !== undefined) dl.sort = body.sort
  if (body.status !== undefined) {
    dl.status = body.status
    if (body.status === 'published' && !dl.publishTime) dl.publishTime = isoTime()
  }
  dl.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminSiteDownloadRouter.delete('/site/download/:id', (req, res) => {
  const idx = DOWNLOADS.findIndex(d => d.id === req.params.id)
  if (idx === -1) { res.json(fail('download not found', 404)); return }
  DOWNLOADS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
