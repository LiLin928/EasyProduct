import { get, post, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { WorkflowInstance } from '@/types/workflow'

export interface MyApplyQuery {
  pageIndex: number
  pageSize: number
  title?: string
  status?: string
  businessType?: string
}

export const getMyApplyList = (params: MyApplyQuery) =>
  get<PageResult<WorkflowInstance>>('/api/admin/wf/my-apply/list', params)

export const createApply = (data: { definitionId: string; title: string; businessType: string; formData: Record<string, unknown> }) =>
  post<{ id: string }>('/api/admin/wf/my-apply', data)

export const cancelApply = (id: string) =>
  del<null>(`/api/admin/wf/my-apply/${id}`)

export const getDefinitionOptions = () =>
  get<Array<{ id: string; name: string; code: string; category: string }>>('/api/admin/wf/definition/options')
