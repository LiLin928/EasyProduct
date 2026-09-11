// src/routes/admin/site-news.ts
// 管理端新闻 Mock API 路由
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { NEWS_LIST, NEWS_CATEGORIES, type News } from '../../data/site.js'

export const adminSiteNewsRouter = Router()

/**
 * GET /api/admin/site/news/list
 * 获取新闻列表（支持筛选、分页、排序）
 */
adminSiteNewsRouter.get('/site/news/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined
  const categoryId = req.query.categoryId as string | undefined
  const status = req.query.status as string | undefined
  const isTop = req.query.isTop as string | undefined
  const startTime = req.query.startTime as string | undefined
  const endTime = req.query.endTime as string | undefined

  let filtered = [...NEWS_LIST]

  // 按关键词搜索
  if (keyword) {
    filtered = filtered.filter((n) => n.title.includes(keyword) || n.summary.includes(keyword))
  }

  // 按分类筛选
  if (categoryId) {
    filtered = filtered.filter((n) => n.categoryId === categoryId)
  }

  // 按状态筛选
  if (status !== undefined) {
    const statusNum = parseInt(status, 10) as 0 | 1
    filtered = filtered.filter((n) => n.status === statusNum)
  }

  // 按置顶状态筛选
  if (isTop !== undefined) {
    const isTopNum = parseInt(isTop, 10) as 0 | 1
    filtered = filtered.filter((n) => n.isTop === isTopNum)
  }

  // 按发布时间范围筛选
  if (startTime) {
    filtered = filtered.filter((n) => n.publishTime && new Date(n.publishTime) >= new Date(startTime))
  }
  if (endTime) {
    filtered = filtered.filter((n) => n.publishTime && new Date(n.publishTime) <= new Date(endTime))
  }

  // 排序：置顶的在前，然后按发布时间倒序，最后按创建时间倒序
  filtered.sort((a, b) => {
    if (a.isTop !== b.isTop) {
      return a.isTop === 1 ? -1 : 1
    }
    const aTime = a.publishTime || a.createdAt
    const bTime = b.publishTime || b.createdAt
    return new Date(bTime).getTime() - new Date(aTime).getTime()
  })

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

/**
 * GET /api/admin/site/news/:id
 * 获取新闻详情
 */
adminSiteNewsRouter.get('/site/news/:id', (req, res) => {
  const { id } = req.params
  const news = NEWS_LIST.find((n) => n.id === id)

  if (!news) {
    res.json(fail('新闻不存在', 404))
    return
  }

  res.json(ok(news))
})

/**
 * POST /api/admin/site/news
 * 创建新闻
 */
adminSiteNewsRouter.post('/site/news', (req, res) => {
  const body = req.body as {
    categoryId: string
    title: string
    summary?: string
    content: string
    coverImage?: string
    author?: string
    source?: string
    publishTime?: string
    isTop?: 0 | 1
    sort?: number
    status?: 0 | 1
  }

  // 验证必填字段
  if (!body.title || !body.title.trim()) {
    res.json(fail('新闻标题不能为空'))
    return
  }

  if (!body.content || !body.content.trim()) {
    res.json(fail('新闻内容不能为空'))
    return
  }

  // 获取分类名称
  const category = NEWS_CATEGORIES.find((c) => c.id === body.categoryId)
  const categoryName = category ? category.categoryName : ''

  const newNews: News = {
    id: guid(),
    categoryId: body.categoryId,
    categoryName: categoryName,
    title: body.title,
    summary: body.summary || '',
    content: body.content,
    coverImage: body.coverImage || '',
    author: body.author || '管理员',
    source: body.source || '原创',
    viewCount: 0,
    publishTime: body.publishTime || '',
    isTop: body.isTop ?? 0,
    sort: body.sort ?? NEWS_LIST.length + 1,
    status: body.status ?? 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001', // Mock: 使用默认用户
  }

  NEWS_LIST.unshift(newNews)

  res.json(ok({ id: newNews.id }, '创建成功'))
})

/**
 * PUT /api/admin/site/news/:id
 * 更新新闻
 */
adminSiteNewsRouter.put('/site/news/:id', (req, res) => {
  const { id } = req.params
  const body = req.body as {
    categoryId?: string
    title?: string
    summary?: string
    content?: string
    coverImage?: string
    author?: string
    source?: string
    publishTime?: string
    isTop?: 0 | 1
    sort?: number
    status?: 0 | 1
  }

  const news = NEWS_LIST.find((n) => n.id === id)

  if (!news) {
    res.json(fail('新闻不存在', 404))
    return
  }

  // 更新分类信息
  if (body.categoryId !== undefined) {
    const category = NEWS_CATEGORIES.find((c) => c.id === body.categoryId)
    news.categoryId = body.categoryId
    news.categoryName = category ? category.categoryName : ''
  }

  // 更新其他字段
  if (body.title !== undefined) news.title = body.title
  if (body.summary !== undefined) news.summary = body.summary
  if (body.content !== undefined) news.content = body.content
  if (body.coverImage !== undefined) news.coverImage = body.coverImage
  if (body.author !== undefined) news.author = body.author
  if (body.source !== undefined) news.source = body.source
  if (body.publishTime !== undefined) news.publishTime = body.publishTime
  if (body.isTop !== undefined) news.isTop = body.isTop
  if (body.sort !== undefined) news.sort = body.sort
  if (body.status !== undefined) news.status = body.status
  news.updatedAt = isoTime()

  res.json(ok(null, '更新成功'))
})

/**
 * DELETE /api/admin/site/news/:id
 * 删除新闻
 */
adminSiteNewsRouter.delete('/site/news/:id', (req, res) => {
  const { id } = req.params
  const index = NEWS_LIST.findIndex((n) => n.id === id)

  if (index === -1) {
    res.json(fail('新闻不存在', 404))
    return
  }

  // 删除新闻
  NEWS_LIST.splice(index, 1)

  res.json(ok(null, '删除成功'))
})

/**
 * POST /api/admin/site/news/batch-delete
 * 批量删除新闻
 */
adminSiteNewsRouter.post('/site/news/batch-delete', (req, res) => {
  const body = req.body as { ids: string[] }

  if (!body.ids || body.ids.length === 0) {
    res.json(fail('请选择要删除的新闻'))
    return
  }

  let successCount = 0
  body.ids.forEach((id) => {
    const index = NEWS_LIST.findIndex((n) => n.id === id)
    if (index !== -1) {
      NEWS_LIST.splice(index, 1)
      successCount++
    }
  })

  res.json(ok(null, `成功删除 ${successCount} 条新闻`))
})

/**
 * PUT /api/admin/site/news/:id/status
 * 更新新闻状态
 */
adminSiteNewsRouter.put('/site/news/:id/status', (req, res) => {
  const { id } = req.params
  const status = req.query.status as string

  if (!status || (status !== '0' && status !== '1')) {
    res.json(fail('状态参数错误'))
    return
  }

  const news = NEWS_LIST.find((n) => n.id === id)

  if (!news) {
    res.json(fail('新闻不存在', 404))
    return
  }

  news.status = parseInt(status, 10) as 0 | 1
  news.updatedAt = isoTime()

  res.json(ok(null, news.status === 1 ? '新闻已启用' : '新闻已禁用'))
})

/**
 * PUT /api/admin/site/news/:id/top
 * 设置新闻置顶
 */
adminSiteNewsRouter.put('/site/news/:id/top', (req, res) => {
  const { id } = req.params
  const isTop = req.query.isTop as string

  if (!isTop || (isTop !== '0' && isTop !== '1')) {
    res.json(fail('置顶参数错误'))
    return
  }

  const news = NEWS_LIST.find((n) => n.id === id)

  if (!news) {
    res.json(fail('新闻不存在', 404))
    return
  }

  news.isTop = parseInt(isTop, 10) as 0 | 1
  news.updatedAt = isoTime()

  res.json(ok(null, news.isTop === 1 ? '新闻已置顶' : '新闻已取消置顶'))
})
