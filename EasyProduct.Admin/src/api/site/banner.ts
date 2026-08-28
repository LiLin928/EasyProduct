import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  SiteBanner,
  BannerQuery,
  BannerParams,
} from '@/types/site'

/** 获取Banner列表（管理端） */
export const getBannerList = (params: BannerQuery) =>
  get<PageResult<SiteBanner>>('/api/admin/site/banner/list', params)

/** 创建Banner */
export const createBanner = (data: BannerParams) =>
  post<{ id: string }>('/api/admin/site/banner', data)

/** 更新Banner */
export const updateBanner = (id: string, data: BannerParams) =>
  put<null>(`/api/admin/site/banner/${id}`, data)

/** 删除Banner */
export const deleteBanner = (id: string) =>
  del<null>(`/api/admin/site/banner/${id}`)
