// src/routes/app/announcement.ts —— 小程序公告 Mock API
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { guid } from '../../helpers/id.js'
import type { Request, Response } from 'express'

export const appAnnouncementRouter = Router()

/** 内存存储：用户公告阅读状态（Map<memberId, Set<announcementId>>） */
const readStatus = new Map<string, Set<string>>()

/** 公告数据类型 */
interface AnnouncementData {
  id: string
  title: string
  type: 'system' | 'activity' | 'update'
  summary: string
  content: string
  publishTime: string
  targetType: 'global' | 'member'
  targetIds: string[]
  attachments: Array<{ id: string; name: string; url: string; size: number }>
}

/** 模拟公告数据（全局 + 定向） */
const announcements: AnnouncementData[] = [
  {
    id: 'anno-guid-001',
    title: '系统升级公告',
    type: 'system',
    summary: '系统将于 2026-08-25 进行升级，届时将暂停服务 2 小时。',
    content: '<p>尊敬的用户：</p><p>为了提供更好的服务体验，系统将于 <strong>2026-08-25 02:00-04:00</strong> 进行升级。</p><p>升级期间将暂停服务，请您提前做好准备。</p><p>感谢您的理解与支持！</p>',
    publishTime: '2026-08-20 10:00:00',
    targetType: 'global',
    targetIds: [],
    attachments: [
      { id: 'attach-001', name: '升级说明.pdf', url: '/uploads/upgrade-guide.pdf', size: 1024000 },
    ],
  },
  {
    id: 'anno-guid-002',
    title: '双节促销活动',
    type: 'activity',
    summary: '中秋节、国庆节双节同庆，全场商品 8 折优惠！',
    content: '<p>双节同庆，好礼不停！</p><p>活动时间：<strong>2026-09-15 ~ 2026-10-07</strong></p><p>活动期间全场商品 <strong>8 折</strong>优惠，更有满减福利等你拿！</p>',
    publishTime: '2026-08-18 09:00:00',
    targetType: 'global',
    targetIds: [],
    attachments: [],
  },
  {
    id: 'anno-guid-003',
    title: '会员权益升级通知',
    type: 'update',
    summary: '会员权益全新升级，新增积分兑换功能。',
    content: '<p>尊敬的会员：</p><p>为感谢您的支持，会员权益已全面升级！</p><ul><li>新增积分兑换功能</li><li>专属客服通道</li><li>生日礼包</li></ul>',
    publishTime: '2026-08-15 14:00:00',
    targetType: 'member',
    targetIds: ['member-001', 'member-002'],
    attachments: [],
  },
]

/**
 * 获取用户公告列表（含阅读状态）
 * GET /api/app/announcements
 */
appAnnouncementRouter.get('/', (req: Request, res: Response) => {
  const memberId = (req as any).member?.id
  if (!memberId) {
    return res.json(fail('未授权'))
  }

  const { pageIndex = 1, pageSize = 10, type, isRead } = req.query as {
    pageIndex?: string
    pageSize?: string
    type?: 'system' | 'activity' | 'update'
    isRead?: string
  }

  // 过滤：全局公告 + 定向给该会员的公告
  let filtered = announcements.filter((a) => {
    if (a.targetType === 'global') return true
    if (a.targetType === 'member') return a.targetIds.includes(memberId)
    return false
  })

  // 按类型过滤
  if (type) {
    filtered = filtered.filter((a) => a.type === type)
  }

  // 获取已读集合
  const readSet = readStatus.get(memberId) || new Set<string>()

  // 添加阅读状态
  const withReadStatus = filtered.map((a) => ({
    ...a,
    isRead: readSet.has(a.id),
    readTime: readSet.has(a.id) ? new Date().toISOString() : undefined,
  }))

  // 按已读状态过滤
  let finalList = withReadStatus
  if (isRead !== undefined) {
    const isReadBool = isRead === 'true'
    finalList = withReadStatus.filter((a) => a.isRead === isReadBool)
  }

  // 分页
  const page = parseInt(pageIndex as string, 10)
  const size = parseInt(pageSize as string, 10)
  const start = (page - 1) * size
  const list = finalList.slice(start, start + size)

  res.json(ok({ list, total: finalList.length }))
})

/**
 * 获取公告详情
 * GET /api/app/announcements/:id
 */
appAnnouncementRouter.get('/:id', (req: Request, res: Response) => {
  const memberId = (req as any).member?.id
  if (!memberId) {
    return res.json(fail('未授权'))
  }

  const { id } = req.params
  const announcement = announcements.find((a) => a.id === id)

  if (!announcement) {
    return res.json(fail('公告不存在', 404))
  }

  // 检查权限：全局或定向给该会员
  if (announcement.targetType === 'member' && !announcement.targetIds.includes(memberId)) {
    return res.json(fail('无权查看此公告', 403))
  }

  // 获取已读状态
  const readSet = readStatus.get(memberId) || new Set<string>()
  const isRead = readSet.has(id)

  res.json(
    ok({
      ...announcement,
      isRead,
      readTime: isRead ? new Date().toISOString() : undefined,
    }),
  )
})

/**
 * 标记公告为已读
 * POST /api/app/announcements/:id/read
 */
appAnnouncementRouter.post('/:id/read', (req: Request, res: Response) => {
  const memberId = (req as any).member?.id
  if (!memberId) {
    return res.json(fail('未授权'))
  }

  const { id } = req.params
  const announcement = announcements.find((a) => a.id === id)

  if (!announcement) {
    return res.json(fail('公告不存在', 404))
  }

  // 初始化会员的已读集合
  if (!readStatus.has(memberId)) {
    readStatus.set(memberId, new Set<string>())
  }

  // 添加已读标记
  readStatus.get(memberId)!.add(id)

  res.json(ok(null, '已标记为已读'))
})

/**
 * 获取未读公告数量
 * GET /api/app/announcements/unread-count
 */
appAnnouncementRouter.get('/unread-count', (req: Request, res: Response) => {
  const memberId = (req as any).member?.id
  if (!memberId) {
    return res.json(fail('未授权'))
  }

  // 过滤：全局公告 + 定向给该会员的公告
  const filtered = announcements.filter((a) => {
    if (a.targetType === 'global') return true
    if (a.targetType === 'member') return a.targetIds.includes(memberId)
    return false
  })

  // 获取已读集合
  const readSet = readStatus.get(memberId) || new Set<string>()

  // 计算未读数量
  const unreadCount = filtered.filter((a) => !readSet.has(a.id)).length

  res.json(ok({ count: unreadCount }))
})