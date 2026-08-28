import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Task, TaskStatus } from '@/types/ops'

export interface TaskQuery {
  pageIndex: number
  pageSize: number
  status?: string
  keyword?: string
}

export const getTaskList = (params: TaskQuery) =>
  get<PageResult<Task>>('/api/admin/ops/task/list', params)

export const getTaskById = (id: string) =>
  get<Task>(`/api/admin/ops/task/${id}`)

export const createTask = (data: Partial<Task>) =>
  post<{ id: string }>('/api/admin/ops/task', data)

export const updateTask = (id: string, data: Partial<Task>) =>
  put<null>(`/api/admin/ops/task/${id}`, data)

export const updateTaskStatus = (id: string, data: { status: TaskStatus }) =>
  post<null>(`/api/admin/ops/task/${id}/status`, data)

export const deleteTask = (id: string) =>
  del<null>(`/api/admin/ops/task/${id}`)
