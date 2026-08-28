import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  SiteVideo,
  VideoQuery,
  VideoParams,
} from '@/types/site'

/** 获取视频列表（管理端） */
export const getVideoList = (params: VideoQuery) =>
  get<PageResult<SiteVideo>>('/api/admin/site/video/list', params)

/** 创建视频 */
export const createVideo = (data: VideoParams) =>
  post<{ id: string }>('/api/admin/site/video', data)

/** 更新视频 */
export const updateVideo = (id: string, data: VideoParams) =>
  put<null>(`/api/admin/site/video/${id}`, data)

/** 删除视频 */
export const deleteVideo = (id: string) =>
  del<null>(`/api/admin/site/video/${id}`)
