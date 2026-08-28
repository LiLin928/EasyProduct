import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { OperateLog } from '@/types/ops'

export interface OperateLogQuery {
  pageIndex: number
  pageSize: number
  module?: string
  status?: string
  startDate?: string
  endDate?: string
  keyword?: string
}

export const getOperateLogList = (params: OperateLogQuery) =>
  get<PageResult<OperateLog>>('/api/admin/ops/operate-log/list', params)

export const getOperateLogById = (id: string) =>
  get<OperateLog>(`/api/admin/ops/operate-log/${id}`)
