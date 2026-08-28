import { get, post } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { WorkflowTask } from '@/types/workflow'

export interface TodoQuery {
  pageIndex: number
  pageSize: number
  instanceTitle?: string
  definitionName?: string
}

export const getTodoList = (params: TodoQuery) =>
  get<PageResult<WorkflowTask>>('/api/admin/wf/todo/list', params)

export const approveTask = (id: string, data: { comment: string }) =>
  post<null>(`/api/admin/wf/todo/${id}/approve`, data)

export const rejectTask = (id: string, data: { comment: string }) =>
  post<null>(`/api/admin/wf/todo/${id}/reject`, data)

export const transferTask = (id: string, data: { assigneeId: string; comment: string }) =>
  post<null>(`/api/admin/wf/todo/${id}/transfer`, data)
