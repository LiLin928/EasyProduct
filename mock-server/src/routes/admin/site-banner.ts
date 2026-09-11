// src/routes/admin/site-banner.ts
// 管理端Banner Mock API 路由
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { BANNERS, type Banner } from '../../data/site.js'

export const adminSiteBannerRouter = Router()

/**
 * GET /api/admin/site/banner/list
 * 获取Banner列表（支持筛选、分页、排序）
 */
adminSiteBannerRouter.get('/site/banner/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const position = req.query.position as string | undefined
  const status = req.query.status as string | undefined
  const startTime = req.query.startTime as string | undefined
  const endTime = req.query.endTime as string | undefined

  let filtered = [...BANNERS]

  // 按标题搜索
  if (title) {
    filtered = filtered.filter((b) => b.title.includes(title))
  }

  // 按显示位置筛选
  if (position) {
    filtered = filtered.filter((b) => b.position === position)
  }

  // 按状态筛选
  if (status !== undefined) {
    const statusNum = parseInt(status, 10) as 0 | 1
    filtered = filtered.filter((b) => b.status === statusNum)
  }

  // 按时间范围筛选
  if (startTime) {
    filtered = filtered.filter((b) => b.startTime && new Date(b.startTime) >= new Date(startTime))
  }
  if (endTime) {
    filtered = filtered.filter((b) => b.endTime && new Date(b.endTime) <= new Date(endTime))
  }

  // 排序：按 sort 字段升序
  filtered.sort((a, b) => a.sort - b.sort)

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

/**
 * GET /api/admin/site/banner/:id
 * 获取Banner详情
 */
adminSiteBannerRouter.get('/site/banner/:id', (req, res) => {
  const { id } = req.params
  const banner = BANNERS.find((b) => b.id === id)

  if (!banner) {
    res.json(fail('Banner不存在', 404))
    return
  }

  res.json(ok(banner))
})

/**
 * POST /api/admin/site/banner
 * 创建Banner
 */
adminSiteBannerRouter.post('/site/banner', (req, res) => {
  const body = req.body as {
    title: string
    titleEn?: string
    imageUrl: string
    linkUrl?: string
    linkType?: string
    linkParam?: string
    position?: string
    startTime?: string
    endTime?: string
    sort?: number
    description?: string
    status?: 0 | 1
  }

  // 验证必填字段
  if (!body.title || !body.title.trim()) {
    res.json(fail('Banner标题不能为空'))
    return
  }

  if (!body.imageUrl || !body.imageUrl.trim()) {
    res.json(fail('Banner图片不能为空'))
    return
  }

  const newBanner: Banner = {
    id: guid(),
    title: body.title,
    titleEn: body.titleEn || body.title,
    imageUrl: body.imageUrl,
    linkUrl: body.linkUrl || '',
    linkType: body.linkType || 'page',
    linkParam: body.linkParam || '',
    position: body.position || 'home',
    startTime: body.startTime || new Date(Date.now() - 30 * 86400000).toISOString(),
    endTime: body.endTime || new Date(Date.now() + 365 * 86400000).toISOString(),
    sort: body.sort ?? BANNERS.length + 1,
    description: body.description || '',
    status: body.status ?? 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001', // Mock: 使用默认用户
  }

  BANNERS.push(newBanner)

  res.json(ok({ id: newBanner.id }, '创建成功'))
})

/**
 * PUT /api/admin/site/banner/:id
 * 更新Banner
 */
adminSiteBannerRouter.put('/site/banner/:id', (req, res) => {
  const { id } = req.params
  const body = req.body as {
    title?: string
    titleEn?: string
    imageUrl?: string
    linkUrl?: string
    linkType?: string
    linkParam?: string
    position?: string
    startTime?: string
    endTime?: string
    sort?: number
    description?: string
    status?: 0 | 1
  }

  const banner = BANNERS.find((b) => b.id === id)

  if (!banner) {
    res.json(fail('Banner不存在', 404))
    return
  }

  // 更新字段
  if (body.title !== undefined) banner.title = body.title
  if (body.titleEn !== undefined) banner.titleEn = body.titleEn
  if (body.imageUrl !== undefined) banner.imageUrl = body.imageUrl
  if (body.linkUrl !== undefined) banner.linkUrl = body.linkUrl
  if (body.linkType !== undefined) banner.linkType = body.linkType
  if (body.linkParam !== undefined) banner.linkParam = body.linkParam
  if (body.position !== undefined) banner.position = body.position
  if (body.startTime !== undefined) banner.startTime = body.startTime
  if (body.endTime !== undefined) banner.endTime = body.endTime
  if (body.sort !== undefined) banner.sort = body.sort
  if (body.description !== undefined) banner.description = body.description
  if (body.status !== undefined) banner.status = body.status
  banner.updatedAt = isoTime()

  res.json(ok(null, '更新成功'))
})

/**
 * DELETE /api/admin/site/banner/:id
 * 删除Banner
 */
adminSiteBannerRouter.delete('/site/banner/:id', (req, res) => {
  const { id } = req.params
  const index = BANNERS.findIndex((b) => b.id === id)

  if (index === -1) {
    res.json(fail('Banner不存在', 404))
    return
  }

  // 删除Banner
  BANNERS.splice(index, 1)

  res.json(ok(null, '删除成功'))
})

/**
 * POST /api/admin/site/banner/batch-delete
 * 批量删除Banner
 */
adminSiteBannerRouter.post('/site/banner/batch-delete', (req, res) => {
  const body = req.body as { ids: string[] }

  if (!body.ids || body.ids.length === 0) {
    res.json(fail('请选择要删除的Banner'))
    return
  }

  let successCount = 0
  body.ids.forEach((id) => {
    const index = BANNERS.findIndex((b) => b.id === id)
    if (index !== -1) {
      BANNERS.splice(index, 1)
      successCount++
    }
  })

  res.json(ok(null, `成功删除 ${successCount} 个Banner`))
})

/**
 * PUT /api/admin/site/banner/:id/status
 * 更新Banner状态
 */
adminSiteBannerRouter.put('/site/banner/:id/status', (req, res) => {
  const { id } = req.params
  const status = req.query.status as string

  if (!status || (status !== '0' && status !== '1')) {
    res.json(fail('状态参数错误'))
    return
  }

  const banner = BANNERS.find((b) => b.id === id)

  if (!banner) {
    res.json(fail('Banner不存在', 404))
    return
  }

  banner.status = parseInt(status, 10) as 0 | 1
  banner.updatedAt = isoTime()

  res.json(ok(null, banner.status === 1 ? 'Banner已启用' : 'Banner已禁用'))
})
