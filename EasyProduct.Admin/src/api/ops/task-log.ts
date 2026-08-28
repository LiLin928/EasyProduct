import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { TaskLog } from '@/types/ops'

export interface TaskLogQuery {
  pageIndex: number
  pageSize: number
  taskId?: string
  status?: string
  startDate?: string
  endDate?: string
  keyword?: string
}

export const getTaskLogList = (params: TaskLogQuery) =>
  get<PageResult<TaskLog>>('/api/admin/ops/task-log/list', params)

export const getTaskLogById = (id: string) =>
  get<TaskLog>(`/api/admin/ops/task-log/${id}`)
