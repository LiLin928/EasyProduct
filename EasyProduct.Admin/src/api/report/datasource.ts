import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Datasource } from '@/types/report'

export interface DatasourceQuery {
  pageIndex: number
  pageSize: number
  name?: string
  type?: string
  status?: string
}

export const getDatasourceList = (params: DatasourceQuery) =>
  get<PageResult<Datasource>>('/api/admin/rpt/datasource/list', params)

export const getDatasourceById = (id: string) =>
  get<Datasource>(`/api/admin/rpt/datasource/${id}`)

export const getDatasourceOptions = () =>
  get<Array<{ id: string; name: string; type: string }>>('/api/admin/rpt/datasource/options')

export const createDatasource = (data: Partial<Datasource>) =>
  post<{ id: string }>('/api/admin/rpt/datasource', data)

export const updateDatasource = (id: string, data: Partial<Datasource>) =>
  put<null>(`/api/admin/rpt/datasource/${id}`, data)

export const deleteDatasource = (id: string) =>
  del<null>(`/api/admin/rpt/datasource/${id}`)

export const testConnection = (id: string) =>
  post<{ success: boolean; message: string }>(`/api/admin/rpt/datasource/${id}/test`)
