import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Download } from '@/types/site'

export const getDownloadList = (params: { pageIndex: number; pageSize: number }) =>
  get<PageResult<Download>>('/api/site/download/list', params)
