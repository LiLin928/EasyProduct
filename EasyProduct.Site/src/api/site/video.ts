import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Video, VideoCategory } from '@/types/site'

// 获取视频分类列表
export const getVideoCategoryList = () =>
  get<VideoCategory[]>('/api/site/video-category/list')

// 获取视频列表（支持分类筛选）
export const getVideoList = (params: { 
  pageIndex: number
  pageSize: number
  categoryId?: string
  keyword?: string 
}) => get<PageResult<Video>>('/api/site/video/list', params)