import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Download, DownloadCategory } from '@/types/site'

export const getDownloadList = (params: { pageIndex: number; pageSize: number; categoryId?: string; keyword?: string }) =>
  get<PageResult<Download>>('/api/site/download/list', params)

export const getDownloadCategoryList = () =>
  get<DownloadCategory[]>('/api/site/download-category/list')
