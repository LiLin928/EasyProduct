import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { LoginLog } from '@/types/ops'

export interface LoginLogQuery {
  pageIndex: number
  pageSize: number
  status?: string
  startDate?: string
  endDate?: string
  keyword?: string
}

export const getLoginLogList = (params: LoginLogQuery) =>
  get<PageResult<LoginLog>>('/api/admin/ops/login-log/list', params)

export const getLoginLogById = (id: string) =>
  get<LoginLog>(`/api/admin/ops/login-log/${id}`)
