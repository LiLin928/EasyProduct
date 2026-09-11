// src/routes/site/news.ts
// 官网公开新闻 Mock API 路由
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { NEWS_LIST, NEWS_CATEGORIES } from '../../data/site.js'

export const siteNewsRouter = Router()

/**
 * GET /api/site/news/list
 * 获取新闻列表（只返回启用的新闻，不包含内容字段）
 */
siteNewsRouter.get('/news/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const categoryId = req.query.categoryId as string | undefined

  let filtered = NEWS_LIST.filter((n) => n.status === 1) // 只返回启用的新闻

  // 按分类筛选
  if (categoryId) {
    filtered = filtered.filter((n) => n.categoryId === categoryId)
  }

  // 排序：置顶的在前，然后按发布时间倒序
  filtered.sort((a, b) => {
    if (a.isTop !== b.isTop) {
      return a.isTop === 1 ? -1 : 1
    }
    const aTime = a.publishTime || a.createdAt
    const bTime = b.publishTime || b.createdAt
    return new Date(bTime).getTime() - new Date(aTime).getTime()
  })

  // 不返回内容字段
  const resultList = filtered.map((n) => ({
    id: n.id,
    categoryId: n.categoryId,
    categoryName: n.categoryName,
    title: n.title,
    summary: n.summary,
    coverImage: n.coverImage,
    author: n.author,
    source: n.source,
    viewCount: n.viewCount,
    publishTime: n.publishTime,
    isTop: n.isTop,
  }))

  res.json(ok(paginate(resultList, pageIndex, pageSize)))
})

/**
 * GET /api/site/news/:id
 * 获取新闻详情（自动增加浏览量）
 */
siteNewsRouter.get('/news/:id', (req, res) => {
  const { id } = req.params
  const news = NEWS_LIST.find((n) => n.id === id && n.status === 1)

  if (!news) {
    res.json(fail('新闻不存在', 404))
    return
  }

  // 增加浏览量
  news.viewCount += 1

  res.json(ok(news))
})

/**
 * GET /api/site/news/recommended
 * 获取置顶新闻列表（只返回启用的置顶新闻）
 */
siteNewsRouter.get('/news/recommended', (req, res) => {
  const count = Number(req.query.count ?? 5)

  const filtered = NEWS_LIST.filter((n) => n.status === 1 && n.isTop === 1)
    .sort((a, b) => {
      const aTime = a.publishTime || a.createdAt
      const bTime = b.publishTime || b.createdAt
      return new Date(bTime).getTime() - new Date(aTime).getTime()
    })
    .slice(0, count)

  // 不返回内容字段
  const resultList = filtered.map((n) => ({
    id: n.id,
    categoryId: n.categoryId,
    categoryName: n.categoryName,
    title: n.title,
    summary: n.summary,
    coverImage: n.coverImage,
    author: n.author,
    source: n.source,
    viewCount: n.viewCount,
    publishTime: n.publishTime,
    isTop: n.isTop,
  }))

  res.json(ok(resultList))
})