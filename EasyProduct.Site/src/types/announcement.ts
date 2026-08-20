// src/types/announcement.ts

/**
 * 公开公告信息
 */
export interface PublicAnnouncement {
  id: string // GUID
  title: string // 公告标题
  titleEn: string // 公告标题（英文）
  summary: string // 摘要
  summaryEn: string // 摘要（英文）
  content: string // 公告内容（富文本HTML）
  contentEn: string // 公告内容（英文）
  level: 'normal' | 'important' | 'urgent' // 级别
  isTop: boolean // 是否置顶
  publishTime: string // 发布时间
  attachments?: Array<{
    name: string // 附件名称
    url: string // 附件URL
    size: number // 附件大小（字节）
  }>
}

/**
 * 公告查询参数
 */
export interface AnnouncementQuery {
  pageIndex: number
  pageSize: number
  keyword?: string // 关键词搜索（标题）
}