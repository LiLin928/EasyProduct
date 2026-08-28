import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Arap, ArapSummary } from '@/types/crm'

export interface ArapQuery {
  pageIndex: number
  pageSize: number
  orderType?: string
  aging?: string
  status?: string
  keyword?: string
}

export const getArapList = (params: ArapQuery) =>
  get<PageResult<Arap>>('/api/admin/crm/arap/list', params)

export const getArapById = (id: string) =>
  get<Arap>(`/api/admin/crm/arap/${id}`)

export const getArapSummary = () =>
  get<ArapSummary>('/api/admin/crm/arap/summary')
