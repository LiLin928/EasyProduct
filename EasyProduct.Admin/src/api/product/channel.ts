import { get, put } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  ProductChannelRow,
  ChannelQuery,
  ChannelToggleParams,
  ChannelSortParams,
} from '@/types/product'

/** 渠道发布列表（分页） */
export const getChannelList = (params: ChannelQuery) =>
  get<PageResult<ProductChannelRow>>('/api/admin/product/channel/list', params)

/** 切换渠道发布状态 */
export const toggleChannel = (data: ChannelToggleParams) =>
  put<null>('/api/admin/product/channel/toggle', data)

/** 设置渠道排序 */
export const updateChannelSort = (data: ChannelSortParams) =>
  put<null>('/api/admin/product/channel/sort', data)
