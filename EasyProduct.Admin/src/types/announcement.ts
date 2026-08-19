// src/types/announcement.ts

/**
 * 公告类型枚举
 */
export type AnnouncementType = 'all' | 'targeted'

/**
 * 公告级别枚举
 */
export type AnnouncementLevel = 'normal' | 'important' | 'urgent'

/**
 * 公告状态枚举
 */
export type AnnouncementStatus = 'draft' | 'published' | 'recalled'

/**
 * 附件信息
 */
export interface Attachment {
  name: string
  url: string
  size: number
}

/**
 * 公告实体
 */
export interface Announcement {
  id: string
  title: string
  content: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds?: string[]
  isTop: boolean
  topTime?: string
  publishTime?: string
  recallTime?: string
  status: AnnouncementStatus
  attachments?: Attachment[]
  creatorId: string
  creatorName: string
  createdAt: string
  updatedAt?: string
}

/**
 * 公告查询参数
 */
export interface AnnouncementQuery {
  pageIndex: number
  pageSize: number
  title?: string
  type?: AnnouncementType
  level?: AnnouncementLevel
  status?: AnnouncementStatus
  isTop?: boolean
}

/**
 * 创建公告参数
 */
export interface CreateAnnouncementParams {
  title: string
  content: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds?: string[]
  attachments?: Attachment[]
}

/**
 * 更新公告参数
 */
export interface UpdateAnnouncementParams {
  title: string
  content: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds?: string[]
  attachments?: Attachment[]
}

/**
 * 置顶参数
 */
export interface SetTopParams {
  isTop: boolean
}