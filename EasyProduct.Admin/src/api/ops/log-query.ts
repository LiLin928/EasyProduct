import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { LogQuery } from '@/types/ops'

export interface LogQueryQuery {
  pageIndex: number
  pageSize: number
  module?: string
  level?: string
  startDate?: string
  endDate?: string
  keyword?: string
}

export const getLogQueryList = (params: LogQueryQuery) =>
  get<PageResult<LogQuery>>('/api/admin/ops/log-query/list', params)

export const getLogQueryById = (id: string) =>
  get<LogQuery>(`/api/admin/ops/log-query/${id}`)
