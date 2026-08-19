// src/api/basic/announcement.ts
import { get, post, put, del } from '@/utils/request'
import type {
  Announcement,
  AnnouncementQuery,
  CreateAnnouncementParams,
  UpdateAnnouncementParams,
  SetTopParams,
} from '@/types/announcement'

/**
 * 获取公告列表（管理端）
 */
export const getAnnouncementList = (params: AnnouncementQuery) =>
  get<{ list: Announcement[]; total: number }>('/api/admin/basic/announcement', params)

/**
 * 获取公告详情
 */
export const getAnnouncementById = (id: string) =>
  get<Announcement>(`/api/admin/basic/announcement/${id}`)

/**
 * 创建公告
 */
export const createAnnouncement = (data: CreateAnnouncementParams) =>
  post<{ id: string }>('/api/admin/basic/announcement', data)

/**
 * 更新公告
 */
export const updateAnnouncement = (id: string, data: UpdateAnnouncementParams) =>
  put<null>(`/api/admin/basic/announcement/${id}`, data)

/**
 * 删除公告
 */
export const deleteAnnouncement = (id: string) =>
  del<null>(`/api/admin/basic/announcement/${id}`)

/**
 * 发布公告
 */
export const publishAnnouncement = (id: string) =>
  put<null>(`/api/admin/basic/announcement/${id}/publish`)

/**
 * 撤回公告
 */
export const recallAnnouncement = (id: string) =>
  put<null>(`/api/admin/basic/announcement/${id}/recall`)

/**
 * 置顶/取消置顶
 */
export const setTopAnnouncement = (id: string, data: SetTopParams) =>
  put<null>(`/api/admin/basic/announcement/${id}/top`, data)