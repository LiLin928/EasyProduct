// src/routes/admin/site-video.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { VIDEOS, type SiteVideo } from '../../data/site-full.js'

export const adminSiteVideoRouter = Router()

// 获取视频列表（分页）
adminSiteVideoRouter.get('/site/video/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const category = req.query.category as string | undefined
  const status = req.query.status ? Number(req.query.status) : undefined
  const videoType = req.query.videoType as string | undefined

  let filtered = [...VIDEOS]

  // 按标题模糊搜索
  if (title) {
    filtered = filtered.filter(v => v.title.includes(title))
  }

  // 按分类筛选
  if (category) {
    filtered = filtered.filter(v => v.category === category)
  }

  // 按状态筛选
  if (status !== undefined) {
    filtered = filtered.filter(v => v.status === status)
  }

  // 按视频类型筛选
  if (videoType) {
    filtered = filtered.filter(v => v.videoType === videoType)
  }

  // 排序：先按排序号，再按创建时间倒序
  filtered.sort((a, b) => a.sort - b.sort || new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 获取视频详情
adminSiteVideoRouter.get('/site/video/:id', (req, res) => {
  const video = VIDEOS.find(v => v.id === req.params.id)

  if (!video) {
    res.json(fail('视频不存在', 404))
    return
  }

  res.json(ok(video))
})

// 创建视频
adminSiteVideoRouter.post('/site/video', (req, res) => {
  const body = req.body

  if (!body.title?.trim()) {
    res.json(fail('视频标题不能为空'))
    return
  }

  const video: SiteVideo = {
    id: guid(),
    title: body.title,
    titleEn: body.titleEn || '',
    description: body.description || '',
    descriptionEn: body.descriptionEn || '',
    coverImage: body.coverImage || '',
    videoUrl: body.videoUrl || '',
    videoType: body.videoType || 'mp4',
    duration: body.duration ?? 0,
    playCount: 0,
    category: body.category || '',
    sort: body.sort ?? VIDEOS.length + 1,
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  }

  VIDEOS.push(video)
  res.json(ok({ id: video.id }, '视频创建成功'))
})

// 更新视频
adminSiteVideoRouter.put('/site/video/:id', (req, res) => {
  const video = VIDEOS.find(v => v.id === req.params.id)

  if (!video) {
    res.json(fail('视频不存在', 404))
    return
  }

  const body = req.body

  if (body.title) video.title = body.title
  if (body.titleEn !== undefined) video.titleEn = body.titleEn
  if (body.description !== undefined) video.description = body.description
  if (body.descriptionEn !== undefined) video.descriptionEn = body.descriptionEn
  if (body.coverImage !== undefined) video.coverImage = body.coverImage
  if (body.videoUrl !== undefined) video.videoUrl = body.videoUrl
  if (body.videoType !== undefined) video.videoType = body.videoType
  if (body.duration !== undefined) video.duration = body.duration
  if (body.category !== undefined) video.category = body.category
  if (body.sort !== undefined) video.sort = body.sort
  if (body.status !== undefined) video.status = body.status

  video.updatedAt = isoTime()
  res.json(ok(null, '视频更新成功'))
})

// 删除视频
adminSiteVideoRouter.delete('/site/video/:id', (req, res) => {
  const idx = VIDEOS.findIndex(v => v.id === req.params.id)

  if (idx === -1) {
    res.json(fail('视频不存在', 404))
    return
  }

  VIDEOS.splice(idx, 1)
  res.json(ok(null, '视频删除成功'))
})

// 批量删除视频
adminSiteVideoRouter.post('/site/video/batch-delete', (req, res) => {
  const ids = req.body as string[]

  if (!Array.isArray(ids) || ids.length === 0) {
    res.json(fail('请选择要删除的视频'))
    return
  }

  let successCount = 0
  ids.forEach(id => {
    const idx = VIDEOS.findIndex(v => v.id === id)
    if (idx !== -1) {
      VIDEOS.splice(idx, 1)
      successCount++
    }
  })

  res.json(ok(null, `成功删除 ${successCount} 个视频`))
})

// 更新视频状态
adminSiteVideoRouter.put('/site/video/:id/status', (req, res) => {
  const video = VIDEOS.find(v => v.id === req.params.id)

  if (!video) {
    res.json(fail('视频不存在', 404))
    return
  }

  const status = Number(req.query.status)
  video.status = status as 0 | 1
  video.updatedAt = isoTime()

  res.json(ok(null, status === 1 ? '视频已启用' : '视频已禁用'))
})