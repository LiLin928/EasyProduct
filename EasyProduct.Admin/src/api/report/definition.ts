import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { ReportDefinition } from '@/types/report'

export interface DefinitionQuery {
  pageIndex: number
  pageSize: number
  name?: string
  code?: string
  status?: string
  datasourceId?: string
}

export const getDefinitionList = (params: DefinitionQuery) =>
  get<PageResult<ReportDefinition>>('/api/admin/rpt/definition/list', params)

export const getDefinitionById = (id: string) =>
  get<ReportDefinition>(`/api/admin/rpt/definition/${id}`)

export const createDefinition = (data: Partial<ReportDefinition>) =>
  post<{ id: string }>('/api/admin/rpt/definition', data)

export const updateDefinition = (id: string, data: Partial<ReportDefinition>) =>
  put<null>(`/api/admin/rpt/definition/${id}`, data)

export const deleteDefinition = (id: string) =>
  del<null>(`/api/admin/rpt/definition/${id}`)

export const previewDefinition = (id: string) =>
  post<{ columns: Array<{ field: string; label: string }>; rows: Record<string, unknown>[] }>(`/api/admin/rpt/definition/${id}/preview`)
