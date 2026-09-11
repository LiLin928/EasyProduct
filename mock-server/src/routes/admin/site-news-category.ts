// src/routes/admin/site-news-category.ts
// 管理端新闻分类 Mock API 路由
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { NEWS_CATEGORIES, type NewsCategory } from '../../data/site.js'

export const adminSiteNewsCategoryRouter = Router()

/**
 * GET /api/admin/site/news-category/list
 * 获取新闻分类列表（支持筛选、分页、排序）
 */
adminSiteNewsCategoryRouter.get('/site/news-category/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const categoryName = req.query.categoryName as string | undefined
  const categoryCode = req.query.categoryCode as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...NEWS_CATEGORIES]

  // 按分类名称搜索
  if (categoryName) {
    filtered = filtered.filter((c) => c.categoryName.includes(categoryName))
  }

  // 按分类编码搜索
  if (categoryCode) {
    filtered = filtered.filter((c) => c.categoryCode && c.categoryCode.includes(categoryCode))
  }

  // 按状态筛选
  if (status !== undefined) {
    const statusNum = parseInt(status, 10) as 0 | 1
    filtered = filtered.filter((c) => c.status === statusNum)
  }

  // 排序：按 sort 字段升序
  filtered.sort((a, b) => a.sort - b.sort)

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

/**
 * GET /api/admin/site/news-category/all
 * 获取所有启用的新闻分类（用于下拉选择）
 */
adminSiteNewsCategoryRouter.get('/site/news-category/all', (req, res) => {
  const filtered = NEWS_CATEGORIES.filter((c) => c.status === 1).sort((a, b) => a.sort - b.sort)
  res.json(ok(filtered))
})

/**
 * GET /api/admin/site/news-category/:id
 * 获取新闻分类详情
 */
adminSiteNewsCategoryRouter.get('/site/news-category/:id', (req, res) => {
  const { id } = req.params
  const category = NEWS_CATEGORIES.find((c) => c.id === id)

  if (!category) {
    res.json(fail('新闻分类不存在', 404))
    return
  }

  res.json(ok(category))
})

/**
 * POST /api/admin/site/news-category
 * 创建新闻分类
 */
adminSiteNewsCategoryRouter.post('/site/news-category', (req, res) => {
  const body = req.body as {
    categoryName: string
    categoryCode?: string
    sort?: number
    status?: 0 | 1
  }

  // 验证必填字段
  if (!body.categoryName || !body.categoryName.trim()) {
    res.json(fail('分类名称不能为空'))
    return
  }

  // 检查分类名称是否已存在
  if (NEWS_CATEGORIES.some((c) => c.categoryName === body.categoryName)) {
    res.json(fail('分类名称已存在'))
    return
  }

  // 检查分类编码是否已存在
  if (body.categoryCode && NEWS_CATEGORIES.some((c) => c.categoryCode === body.categoryCode)) {
    res.json(fail('分类编码已存在'))
    return
  }

  const newCategory: NewsCategory = {
    id: guid(),
    categoryName: body.categoryName,
    categoryCode: body.categoryCode || '',
    sort: body.sort ?? NEWS_CATEGORIES.length + 1,
    status: body.status ?? 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001', // Mock: 使用默认用户
  }

  NEWS_CATEGORIES.push(newCategory)

  res.json(ok({ id: newCategory.id }, '创建成功'))
})

/**
 * PUT /api/admin/site/news-category/:id
 * 更新新闻分类
 */
adminSiteNewsCategoryRouter.put('/site/news-category/:id', (req, res) => {
  const { id } = req.params
  const body = req.body as {
    categoryName?: string
    categoryCode?: string
    sort?: number
    status?: 0 | 1
  }

  const category = NEWS_CATEGORIES.find((c) => c.id === id)

  if (!category) {
    res.json(fail('新闻分类不存在', 404))
    return
  }

  // 检查分类名称是否已存在（排除自己）
  if (body.categoryName && body.categoryName !== category.categoryName) {
    if (NEWS_CATEGORIES.some((c) => c.categoryName === body.categoryName)) {
      res.json(fail('分类名称已存在'))
      return
    }
    category.categoryName = body.categoryName
  }

  // 检查分类编码是否已存在（排除自己）
  if (body.categoryCode !== undefined && body.categoryCode !== category.categoryCode) {
    if (body.categoryCode && NEWS_CATEGORIES.some((c) => c.categoryCode === body.categoryCode)) {
      res.json(fail('分类编码已存在'))
      return
    }
    category.categoryCode = body.categoryCode
  }

  // 更新其他字段
  if (body.sort !== undefined) category.sort = body.sort
  if (body.status !== undefined) category.status = body.status
  category.updatedAt = isoTime()

  res.json(ok(null, '更新成功'))
})

/**
 * DELETE /api/admin/site/news-category/:id
 * 删除新闻分类
 */
adminSiteNewsCategoryRouter.delete('/site/news-category/:id', (req, res) => {
  const { id } = req.params
  const index = NEWS_CATEGORIES.findIndex((c) => c.id === id)

  if (index === -1) {
    res.json(fail('新闻分类不存在', 404))
    return
  }

  // 删除分类
  NEWS_CATEGORIES.splice(index, 1)

  res.json(ok(null, '删除成功'))
})