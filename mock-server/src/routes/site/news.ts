// src/routes/site/news.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { NEWS_FULL } from '../../data/site-full.js'

export const siteNewsRouter = Router()

// 新闻列表
siteNewsRouter.get('/news/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined

  let filtered = NEWS_FULL
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(n =>
      n.title.includes(keyword) ||
      n.titleEn.toLowerCase().includes(kw)
    )
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 新闻详情
siteNewsRouter.get('/news/:id', (req, res) => {
  const news = NEWS_FULL.find(n => n.id === req.params.id)
  if (!news) {
    res.json(fail('新闻不存在', 404))
    return
  }
  res.json(ok(news))
})