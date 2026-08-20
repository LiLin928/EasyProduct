// api/announcement.ts —— 小程序公告 API
import { request } from '../utils/request'
import type { PageResult } from '../types/api.types'
import type { UserAnnouncement, AnnouncementQuery, UnreadCountResult } from '../types/announcement'

const BASE_URL = '/app/announcements'

/** 获取用户公告列表（含阅读状态） */
export function getUserAnnouncementList(params: AnnouncementQuery): Promise<PageResult<UserAnnouncement>> {
  return request<PageResult<UserAnnouncement>>({
    url: BASE_URL,
    method: 'GET',
    data: params,
  })
}

/** 获取公告详情 */
export function getUserAnnouncementById(id: string): Promise<UserAnnouncement> {
  return request<UserAnnouncement>({
    url: `${BASE_URL}/${id}`,
    method: 'GET',
  })
}

/** 标记公告为已读 */
export function markAnnouncementAsRead(id: string): Promise<void> {
  return request<void>({
    url: `${BASE_URL}/${id}/read`,
    method: 'POST',
  })
}

/** 获取未读公告数量 */
export function getUnreadCount(): Promise<UnreadCountResult> {
  return request<UnreadCountResult>({
    url: `${BASE_URL}/unread-count`,
    method: 'GET',
  })
}