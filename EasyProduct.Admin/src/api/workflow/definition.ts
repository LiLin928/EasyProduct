import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { WorkflowDefinition } from '@/types/workflow'

export interface DefinitionQuery {
  pageIndex: number
  pageSize: number
  name?: string
  code?: string
  category?: string
  status?: string
}

export const getDefinitionList = (params: DefinitionQuery) =>
  get<PageResult<WorkflowDefinition>>('/api/admin/wf/definition/list', params)

export const createDefinition = (data: {
  name: string
  code: string
  category: string
  description: string
}) => post<{ id: string }>('/api/admin/wf/definition', data)

export const updateDefinition = (id: string, data: Partial<{
  name: string
  code: string
  category: string
  description: string
}>) => put<null>(`/api/admin/wf/definition/${id}`, data)

export const deleteDefinition = (id: string) =>
  del<null>(`/api/admin/wf/definition/${id}`)

export const publishDefinition = (id: string) =>
  post<null>(`/api/admin/wf/definition/${id}/publish`)

export const disableDefinition = (id: string) =>
  post<null>(`/api/admin/wf/definition/${id}/disable`)
