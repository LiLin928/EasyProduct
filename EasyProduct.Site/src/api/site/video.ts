import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Video } from '@/types/site'

export const getVideoList = (params: { pageIndex: number; pageSize: number }) =>
  get<PageResult<Video>>('/api/site/video/list', params)
