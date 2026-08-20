// src/api/site/announcement.ts
import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { PublicAnnouncement, AnnouncementQuery } from '@/types/announcement'

/** 公告列表 */
export const getPublicAnnouncementList = (params: AnnouncementQuery) =>
  get<PageResult<PublicAnnouncement>>('/api/site/announcement/list', params)

/** 公告详情 */
export const getPublicAnnouncementById = (id: string) =>
  get<PublicAnnouncement>(`/api/site/announcement/${id}`)