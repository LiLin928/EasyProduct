import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { WorkflowInstance, WorkflowHistory } from '@/types/workflow'

export interface InstanceQuery {
  pageIndex: number
  pageSize: number
  title?: string
  status?: string
  applicantName?: string
  definitionName?: string
}

export const getInstanceList = (params: InstanceQuery) =>
  get<PageResult<WorkflowInstance>>('/api/admin/wf/instance/list', params)

export const getInstanceById = (id: string) =>
  get<WorkflowInstance>(`/api/admin/wf/instance/${id}`)

export const getInstanceHistory = (id: string) =>
  get<WorkflowHistory[]>(`/api/admin/wf/instance/${id}/history`)
