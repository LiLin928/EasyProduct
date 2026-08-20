// src/routes/site/announcement.ts
// 官网公告 Mock API 路由
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { ANNOUNCEMENTS } from '../../data/announcement.js'

export const siteAnnouncementRouter = Router()

/**
 * GET /api/site/announcement/list
 * 获取已发布公告列表（官网公开）
 */
siteAnnouncementRouter.get('/announcement/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const keyword = req.query.keyword as string | undefined

  // 只返回已发布的公告
  let filtered = ANNOUNCEMENTS.filter((a) => a.status === 'published')

  // 关键词搜索
  if (keyword) {
    const kw = keyword.toLowerCase()
    filtered = filtered.filter(
      (a) => a.title.includes(keyword) || a.title.toLowerCase().includes(kw)
    )
  }

  // 排序：置顶的在前，然后按发布时间倒序
  filtered.sort((a, b) => {
    if (a.isTop !== b.isTop) {
      return a.isTop ? -1 : 1
    }
    const aTime = a.publishTime || a.createdAt
    const bTime = b.publishTime || b.createdAt
    return new Date(bTime).getTime() - new Date(aTime).getTime()
  })

  // 转换为公开信息格式
  const publicList = filtered.map((a) => ({
    id: a.id,
    title: a.title,
    titleEn: a.title, // Mock: 中文标题，实际应该有英文翻译
    summary: a.content.substring(0, 100),
    summaryEn: a.content.substring(0, 100), // Mock: 英文摘要
    content: a.content,
    contentEn: a.content, // Mock: 中文内容，实际应该有英文翻译
    level: a.level,
    isTop: a.isTop,
    publishTime: a.publishTime || a.createdAt,
    attachments: a.attachments,
  }))

  res.json(ok(paginate(publicList, pageIndex, pageSize)))
})

/**
 * GET /api/site/announcement/:id
 * 获取公告详情（官网公开）
 */
siteAnnouncementRouter.get('/announcement/:id', (req, res) => {
  const { id } = req.params
  const announcement = ANNOUNCEMENTS.find((a) => a.id === id)

  if (!announcement) {
    res.json(fail('公告不存在', 404))
    return
  }

  // 只返回已发布的公告
  if (announcement.status !== 'published') {
    res.json(fail('公告不存在', 404))
    return
  }

  // 转换为公开信息格式
  const publicAnnouncement = {
    id: announcement.id,
    title: announcement.title,
    titleEn: announcement.title, // Mock: 中文标题
    summary: announcement.content.substring(0, 100),
    summaryEn: announcement.content.substring(0, 100), // Mock: 英文摘要
    content: announcement.content,
    contentEn: announcement.content, // Mock: 中文内容
    level: announcement.level,
    isTop: announcement.isTop,
    publishTime: announcement.publishTime || announcement.createdAt,
    attachments: announcement.attachments,
  }

  res.json(ok(publicAnnouncement))
})