// types/announcement.ts —— 小程序公告相关类型定义
import type { PageQuery } from './api.types'

/** 用户公告（含阅读状态） */
export interface UserAnnouncement {
  id: string
  title: string
  type: 'system' | 'activity' | 'update'
  summary: string
  content: string
  attachments?: AnnouncementAttachment[]
  publishTime: string
  isRead: boolean
  readTime?: string
}

/** 公告附件 */
export interface AnnouncementAttachment {
  id: string
  name: string
  url: string
  size: number
}

/** 公告查询参数 */
export interface AnnouncementQuery extends PageQuery {
  type?: 'system' | 'activity' | 'update'
  isRead?: boolean
  keyword?: string
}

/** 未读数量响应 */
export interface UnreadCountResult {
  count: number
}