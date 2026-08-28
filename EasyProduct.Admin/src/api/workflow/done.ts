import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { WorkflowTask } from '@/types/workflow'

export interface DoneQuery {
  pageIndex: number
  pageSize: number
  instanceTitle?: string
  status?: string
}

export const getDoneList = (params: DoneQuery) =>
  get<PageResult<WorkflowTask>>('/api/admin/wf/done/list', params)
