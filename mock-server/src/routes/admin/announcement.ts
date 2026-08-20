// src/routes/admin/announcement.ts
// 管理端公告 Mock API 路由
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { ANNOUNCEMENTS, type Announcement } from '../../data/announcement.js'
import { ANNOUNCEMENT_READS } from '../../data/announcement-read.js'

export const adminAnnouncementRouter = Router()

/**
 * GET /api/admin/basic/announcement
 * 获取公告列表（支持筛选、分页、排序）
 */
adminAnnouncementRouter.get('/basic/announcement', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const type = req.query.type as string | undefined
  const level = req.query.level as string | undefined
  const status = req.query.status as string | undefined
  const isTop = req.query.isTop as string | undefined

  let filtered = [...ANNOUNCEMENTS]

  // 按标题搜索
  if (title) {
    filtered = filtered.filter((a) => a.title.includes(title))
  }

  // 按类型筛选
  if (type) {
    filtered = filtered.filter((a) => a.type === type)
  }

  // 按级别筛选
  if (level) {
    filtered = filtered.filter((a) => a.level === level)
  }

  // 按状态筛选
  if (status) {
    filtered = filtered.filter((a) => a.status === status)
  }

  // 按置顶状态筛选
  if (isTop !== undefined) {
    const isTopBool = isTop === 'true'
    filtered = filtered.filter((a) => a.isTop === isTopBool)
  }

  // 排序：置顶的在前，然后按发布时间倒序，最后按创建时间倒序
  filtered.sort((a, b) => {
    // 置顶的在前
    if (a.isTop !== b.isTop) {
      return a.isTop ? -1 : 1
    }
    // 按发布时间倒序（未发布的按创建时间）
    const aTime = a.publishTime || a.createdAt
    const bTime = b.publishTime || b.createdAt
    return new Date(bTime).getTime() - new Date(aTime).getTime()
  })

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

/**
 * GET /api/admin/basic/announcement/:id
 * 获取公告详情（含阅读统计）
 */
adminAnnouncementRouter.get('/basic/announcement/:id', (req, res) => {
  const { id } = req.params
  const announcement = ANNOUNCEMENTS.find((a) => a.id === id)

  if (!announcement) {
    res.json(fail('公告不存在', 404))
    return
  }

  // 计算阅读统计
  const reads = ANNOUNCEMENT_READS.filter((r) => r.announcementId === id)
  const readStats = {
    totalReads: reads.length,
    uniqueReaders: reads.length, // 实际应该是去重的用户数，这里简化处理
    readList: reads.slice(0, 10), // 返回最近10条阅读记录
  }

  res.json(ok({
    ...announcement,
    readStats,
  }))
})

/**
 * POST /api/admin/basic/announcement
 * 创建公告
 */
adminAnnouncementRouter.post('/basic/announcement', (req, res) => {
  const body = req.body as {
    title: string
    content: string
    type: 'all' | 'targeted'
    level: 'normal' | 'important' | 'urgent'
    targetRoleIds?: string[]
    attachments?: Array<{ name: string; url: string; size: number }>
  }

  // 验证必填字段
  if (!body.title || !body.title.trim()) {
    res.json(fail('公告标题不能为空'))
    return
  }

  if (!body.content || !body.content.trim()) {
    res.json(fail('公告内容不能为空'))
    return
  }

  // 验证定向公告必须指定目标角色
  if (body.type === 'targeted' && (!body.targetRoleIds || body.targetRoleIds.length === 0)) {
    res.json(fail('定向公告必须指定目标角色'))
    return
  }

  const newAnnouncement: Announcement = {
    id: guid(),
    title: body.title,
    content: body.content,
    type: body.type,
    level: body.level,
    targetRoleIds: body.targetRoleIds,
    status: 'draft',
    isTop: false,
    attachments: body.attachments || [],
    creatorId: 'user-001', // Mock: 使用默认用户
    creatorName: '系统管理员',
    createdAt: isoTime(),
  }

  ANNOUNCEMENTS.push(newAnnouncement)

  res.json(ok({ id: newAnnouncement.id }, '创建成功'))
})

/**
 * PUT /api/admin/basic/announcement/:id
 * 更新公告（仅草稿可编辑）
 */
adminAnnouncementRouter.put('/basic/announcement/:id', (req, res) => {
  const { id } = req.params
  const body = req.body as {
    title: string
    content: string
    type: 'all' | 'targeted'
    level: 'normal' | 'important' | 'urgent'
    targetRoleIds?: string[]
    attachments?: Array<{ name: string; url: string; size: number }>
  }

  const announcement = ANNOUNCEMENTS.find((a) => a.id === id)

  if (!announcement) {
    res.json(fail('公告不存在', 404))
    return
  }

  // 业务规则：仅草稿状态可编辑
  if (announcement.status !== 'draft') {
    res.json(fail('只有草稿状态的公告可以编辑'))
    return
  }

  // 验证必填字段
  if (!body.title || !body.title.trim()) {
    res.json(fail('公告标题不能为空'))
    return
  }

  if (!body.content || !body.content.trim()) {
    res.json(fail('公告内容不能为空'))
    return
  }

  // 验证定向公告必须指定目标角色
  if (body.type === 'targeted' && (!body.targetRoleIds || body.targetRoleIds.length === 0)) {
    res.json(fail('定向公告必须指定目标角色'))
    return
  }

  // 更新公告
  announcement.title = body.title
  announcement.content = body.content
  announcement.type = body.type
  announcement.level = body.level
  announcement.targetRoleIds = body.targetRoleIds
  announcement.attachments = body.attachments || []
  announcement.updatedAt = isoTime()

  res.json(ok(null, '更新成功'))
})

/**
 * DELETE /api/admin/basic/announcement/:id
 * 删除公告（仅草稿、已撤回可删除）
 */
adminAnnouncementRouter.delete('/basic/announcement/:id', (req, res) => {
  const { id } = req.params
  const index = ANNOUNCEMENTS.findIndex((a) => a.id === id)

  if (index === -1) {
    res.json(fail('公告不存在', 404))
    return
  }

  const announcement = ANNOUNCEMENTS[index]

  // 业务规则：仅草稿、已撤回状态可删除
  if (announcement.status !== 'draft' && announcement.status !== 'recalled') {
    res.json(fail('只有草稿或已撤回的公告可以删除'))
    return
  }

  // 删除公告
  ANNOUNCEMENTS.splice(index, 1)

  // 同时删除相关阅读记录
  const readIndices = ANNOUNCEMENT_READS
    .map((r, i) => (r.announcementId === id ? i : -1))
    .filter((i) => i !== -1)
    .reverse()

  readIndices.forEach((i) => ANNOUNCEMENT_READS.splice(i, 1))

  res.json(ok(null, '删除成功'))
})

/**
 * PUT /api/admin/basic/announcement/:id/publish
 * 发布公告
 */
adminAnnouncementRouter.put('/basic/announcement/:id/publish', (req, res) => {
  const { id } = req.params
  const announcement = ANNOUNCEMENTS.find((a) => a.id === id)

  if (!announcement) {
    res.json(fail('公告不存在', 404))
    return
  }

  // 业务规则：仅草稿状态可发布
  if (announcement.status !== 'draft') {
    res.json(fail('只有草稿状态的公告可以发布'))
    return
  }

  // 更新状态为已发布
  announcement.status = 'published'
  announcement.publishTime = isoTime()
  announcement.updatedAt = isoTime()

  res.json(ok(null, '发布成功'))
})

/**
 * PUT /api/admin/basic/announcement/:id/recall
 * 撤回公告
 */
adminAnnouncementRouter.put('/basic/announcement/:id/recall', (req, res) => {
  const { id } = req.params
  const announcement = ANNOUNCEMENTS.find((a) => a.id === id)

  if (!announcement) {
    res.json(fail('公告不存在', 404))
    return
  }

  // 业务规则：仅已发布状态可撤回
  if (announcement.status !== 'published') {
    res.json(fail('只有已发布的公告可以撤回'))
    return
  }

  // 更新状态为已撤回
  announcement.status = 'recalled'
  announcement.recallTime = isoTime()
  announcement.updatedAt = isoTime()

  // 撤回时取消置顶
  if (announcement.isTop) {
    announcement.isTop = false
  }

  res.json(ok(null, '撤回成功'))
})

/**
 * PUT /api/admin/basic/announcement/:id/top
 * 置顶/取消置顶
 */
adminAnnouncementRouter.put('/basic/announcement/:id/top', (req, res) => {
  const { id } = req.params
  const body = req.body as { isTop: boolean }
  const announcement = ANNOUNCEMENTS.find((a) => a.id === id)

  if (!announcement) {
    res.json(fail('公告不存在', 404))
    return
  }

  // 业务规则：仅已发布状态可置顶
  if (announcement.status !== 'published') {
    res.json(fail('只有已发布的公告可以置顶'))
    return
  }

  // 更新置顶状态
  const wasTop = announcement.isTop
  announcement.isTop = body.isTop

  if (body.isTop && !wasTop) {
    announcement.topTime = isoTime()
  } else if (!body.isTop && wasTop) {
    announcement.topTime = undefined
  }

  announcement.updatedAt = isoTime()

  res.json(ok(null, body.isTop ? '置顶成功' : '取消置顶成功'))
})