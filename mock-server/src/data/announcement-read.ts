// src/data/announcement-read.ts
// Mock 数据：公告阅读记录
import { guid } from '../helpers/id.js'

/**
 * 公告阅读记录数据结构
 */
export interface AnnouncementRead {
  id: string // GUID
  announcementId: string // 关联的公告 ID（GUID）
  userId: string // 阅读的用户 ID（GUID）
  readAt: string // 阅读时间（ISO 8601）
}

/**
 * 公告阅读记录 Mock 数据
 */
export const ANNOUNCEMENT_READS: AnnouncementRead[] = [
  {
    id: guid(),
    announcementId: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', // 系统升级通知
    userId: 'user-001',
    readAt: '2026-08-15T10:30:00Z',
  },
  {
    id: guid(),
    announcementId: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', // 系统升级通知
    userId: 'user-002',
    readAt: '2026-08-15T11:15:00Z',
  },
  {
    id: guid(),
    announcementId: 'b2c3d4e5-f6a7-8901-bcde-f23456789012', // 新功能上线公告：订单批量导出功能
    userId: 'user-003',
    readAt: '2026-08-16T14:20:00Z',
  },
  {
    id: guid(),
    announcementId: 'c3d4e5f6-a7b8-9012-cdef-345678901234', // 紧急维护通知：数据库异常修复
    userId: 'user-001',
    readAt: '2026-08-18T14:05:00Z',
  },
  {
    id: guid(),
    announcementId: 'e5f6a7b8-c9d0-1234-ef01-567890123456', // 会员积分系统升级公告
    userId: 'user-002',
    readAt: '2026-08-19T09:45:00Z',
  },
]