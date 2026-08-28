import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { ColumnTemplate } from '@/types/report'

export interface ColumnTemplateQuery {
  pageIndex: number
  pageSize: number
  name?: string
  type?: string
}

export const getColumnTemplateList = (params: ColumnTemplateQuery) =>
  get<PageResult<ColumnTemplate>>('/api/admin/rpt/column-template/list', params)

export const getColumnTemplateById = (id: string) =>
  get<ColumnTemplate>(`/api/admin/rpt/column-template/${id}`)

export const createColumnTemplate = (data: Partial<ColumnTemplate>) =>
  post<{ id: string }>('/api/admin/rpt/column-template', data)

export const updateColumnTemplate = (id: string, data: Partial<ColumnTemplate>) =>
  put<null>(`/api/admin/rpt/column-template/${id}`, data)

export const deleteColumnTemplate = (id: string) =>
  del<null>(`/api/admin/rpt/column-template/${id}`)
