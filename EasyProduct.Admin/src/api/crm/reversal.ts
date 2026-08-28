import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Reversal, ReversalStatus } from '@/types/crm'

export interface ReversalQuery {
  pageIndex: number
  pageSize: number
  type?: string
  status?: string
  keyword?: string
}

export const getReversalList = (params: ReversalQuery) =>
  get<PageResult<Reversal>>('/api/admin/crm/reversal/list', params)

export const getReversalById = (id: string) =>
  get<Reversal>(`/api/admin/crm/reversal/${id}`)

export const createReversal = (data: Partial<Reversal>) =>
  post<{ id: string }>('/api/admin/crm/reversal', data)

export const updateReversal = (id: string, data: Partial<Reversal>) =>
  put<null>(`/api/admin/crm/reversal/${id}`, data)

export const updateReversalStatus = (id: string, data: { status: ReversalStatus }) =>
  post<null>(`/api/admin/crm/reversal/${id}/status`, data)

export const deleteReversal = (id: string) =>
  del<null>(`/api/admin/crm/reversal/${id}`)
