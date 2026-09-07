// src/types/announcement.ts

import type { PageQuery } from './api'

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
  name: string // 附件名称
  url: string // 附件URL
  size: number // 附件大小（字节）
}

/**
 * 公告实体
 */
export interface Announcement {
  id: string // GUID
  title: string // 公告标题
  content: string // 公告内容（富文本HTML）
  type: AnnouncementType // 类型：all-全员，targeted-定向
  level: AnnouncementLevel // 级别：normal-普通，important-重要，urgent-紧急
  targetRoleIds?: string[] // 目标角色ID列表（定向公告时使用）
  isTop: 0 | 1  // 是否置顶：0=否，1=是 // 是否置顶
  topTime?: string // 置顶时间
  publishTime?: string // 发布时间
  recallTime?: string // 撤回时间
  status: AnnouncementStatus // 状态：draft-草稿，published-已发布，recalled-已撤回
  attachments?: Attachment[] // 附件列表
  creatorId: string // 创建人ID
  creatorName: string // 创建人姓名
  createdAt: string // 创建时间
  updatedAt?: string // 更新时间
}

/**
 * 公告查询参数
 */
export interface AnnouncementQuery extends PageQuery {
  title?: string // 公告标题（模糊搜索）
  type?: AnnouncementType // 公告类型
  level?: AnnouncementLevel // 公告级别
  status?: AnnouncementStatus // 公告状态
  isTop?: boolean // 是否置顶
}

/**
 * 创建公告参数
 */
export interface CreateAnnouncementParams {
  title: string // 公告标题
  content: string // 公告内容（富文本HTML）
  type: AnnouncementType // 类型：all-全员，targeted-定向
  level: AnnouncementLevel // 级别：normal-普通，important-重要，urgent-紧急
  targetRoleIds?: string[] // 目标角色ID列表（定向公告时使用）
  attachments?: Attachment[] // 附件列表
}

/**
 * 更新公告参数
 */
export interface UpdateAnnouncementParams {
  title: string // 公告标题
  content: string // 公告内容（富文本HTML）
  type: AnnouncementType // 类型：all-全员，targeted-定向
  level: AnnouncementLevel // 级别：normal-普通，important-重要，urgent-紧急
  targetRoleIds?: string[] // 目标角色ID列表（定向公告时使用）
  attachments?: Attachment[] // 附件列表
}

/**
 * 置顶参数
 */
export interface SetTopParams {
  isTop: 0 | 1  // 是否置顶：0=否，1=是 // 是否置顶
}