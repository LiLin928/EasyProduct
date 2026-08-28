import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  SiteDownload,
  DownloadQuery,
  DownloadParams,
} from '@/types/site'

/** 获取下载资源列表（管理端） */
export const getDownloadList = (params: DownloadQuery) =>
  get<PageResult<SiteDownload>>('/api/admin/site/download/list', params)

/** 创建下载资源 */
export const createDownload = (data: DownloadParams) =>
  post<{ id: string }>('/api/admin/site/download', data)

/** 更新下载资源 */
export const updateDownload = (id: string, data: DownloadParams) =>
  put<null>(`/api/admin/site/download/${id}`, data)

/** 删除下载资源 */
export const deleteDownload = (id: string) =>
  del<null>(`/api/admin/site/download/${id}`)
