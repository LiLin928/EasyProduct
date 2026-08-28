// src/routes/admin/site-news.ts
// 管理端新闻 Mock API 路由
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { NEWS_FULL, type SiteNews } from '../../data/site-full.js'

export const adminSiteNewsRouter = Router()

adminSiteNewsRouter.get('/site/news/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const status = req.query.status as string | undefined
  const isTop = req.query.isTop as string | undefined

  let filtered = [...NEWS_FULL]

  if (title) {
    const kw = title.toLowerCase()
    filtered = filtered.filter(n =>
      n.title.includes(title) || n.titleEn.toLowerCase().includes(kw)
    )
  }
  if (status) filtered = filtered.filter(n => n.status === status)
  if (isTop !== undefined) {
    const isTopBool = isTop === 'true'
    filtered = filtered.filter(n => n.isTop === isTopBool)
  }

  filtered.sort((a, b) => {
    if (a.isTop !== b.isTop) return a.isTop ? -1 : 1
    return new Date(b.publishTime).getTime() - new Date(a.publishTime).getTime()
  })

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminSiteNewsRouter.get('/site/news/:id', (req, res) => {
  const news = NEWS_FULL.find(n => n.id === req.params.id)
  if (!news) { res.json(fail('news not found', 404)); return }
  res.json(ok(news))
})

adminSiteNewsRouter.post('/site/news', (req, res) => {
  const body = req.body
  if (!body.title?.trim()) { res.json(fail('title required')); return }

  const news: SiteNews = {
    id: guid(),
    categoryId: body.categoryId || guid(),
    title: body.title,
    titleEn: body.titleEn || body.title,
    summary: body.summary || '',
    summaryEn: body.summaryEn || '',
    content: body.content || '',
    contentEn: body.contentEn || '',
    coverImage: body.coverImage || '',
    isTop: false,
    viewCount: 0,
    status: 'draft',
    publishTime: '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  NEWS_FULL.unshift(news)
  res.json(ok({ id: news.id }, 'created'))
})

adminSiteNewsRouter.put('/site/news/:id', (req, res) => {
  const news = NEWS_FULL.find(n => n.id === req.params.id)
  if (!news) { res.json(fail('news not found', 404)); return }
  if (news.status === 'published') { res.json(fail('cannot edit published news')); return }

  const body = req.body
  if (body.title) news.title = body.title
  if (body.titleEn !== undefined) news.titleEn = body.titleEn
  if (body.summary !== undefined) news.summary = body.summary
  if (body.summaryEn !== undefined) news.summaryEn = body.summaryEn
  if (body.content !== undefined) news.content = body.content
  if (body.contentEn !== undefined) news.contentEn = body.contentEn
  if (body.coverImage !== undefined) news.coverImage = body.coverImage
  if (body.categoryId !== undefined) news.categoryId = body.categoryId
  if (body.isTop !== undefined) news.isTop = body.isTop
  news.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminSiteNewsRouter.delete('/site/news/:id', (req, res) => {
  const idx = NEWS_FULL.findIndex(n => n.id === req.params.id)
  if (idx === -1) { res.json(fail('news not found', 404)); return }
  NEWS_FULL.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})

adminSiteNewsRouter.put('/site/news/:id/publish', (req, res) => {
  const news = NEWS_FULL.find(n => n.id === req.params.id)
  if (!news) { res.json(fail('news not found', 404)); return }
  news.status = 'published'
  news.publishTime = isoTime()
  news.updatedAt = isoTime()
  res.json(ok(null, 'published'))
})

adminSiteNewsRouter.put('/site/news/:id/unpublish', (req, res) => {
  const news = NEWS_FULL.find(n => n.id === req.params.id)
  if (!news) { res.json(fail('news not found', 404)); return }
  news.status = 'draft'
  news.publishTime = ''
  news.updatedAt = isoTime()
  res.json(ok(null, 'unpublished'))
})

adminSiteNewsRouter.put('/site/news/:id/top', (req, res) => {
  const news = NEWS_FULL.find(n => n.id === req.params.id)
  if (!news) { res.json(fail('news not found', 404)); return }
  news.isTop = req.body.isTop
  news.updatedAt = isoTime()
  res.json(ok(null, req.body.isTop ? 'top set' : 'top cancelled'))
})
