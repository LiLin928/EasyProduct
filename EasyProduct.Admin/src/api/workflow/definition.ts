import { get, post, put, del } from '@/utils/request'
import type { ApiResponse, PageResult } from '@/types/api'
import type { WorkflowDefinition, Workflow } from '@/types/workflow'

export interface DefinitionQuery {
  pageIndex: number
  pageSize: number
  name?: string
  code?: string
  category?: string
  status?: string
}

// 获取流程定义列表
export const getDefinitionList = (params: DefinitionQuery) =>
  get<PageResult<WorkflowDefinition>>("/api/admin/wf/definition/list", params)

// 获取流程定义详情（用于设计器）
export const getDefinitionDetail = (id: string) =>
  get<Workflow>("/api/admin/wf/definition/detail", { id })

// 创建流程定义
export const createDefinition = (data: {
  name: string
  code: string
  category: string
  description: string
}) => post<{ id: string }>("/api/admin/wf/definition", data)

// 更新流程定义
export const updateDefinition = (id: string, data: Partial<{
  name: string
  code: string
  category: string
  description: string
}>) => put<null>("/api/admin/wf/definition/" + id, data)

// 删除流程定义
export const deleteDefinition = (id: string) =>
  del<null>("/api/admin/wf/definition/" + id)

// 发布流程定义
export const publishDefinition = (id: string) =>
  post<{ status: string; version: number }>("/api/admin/wf/definition/" + id + "/publish")

// 停用流程定义
export const disableDefinition = (id: string) =>
  post<{ status: string }>("/api/admin/wf/definition/" + id + "/disable")

// 保存流程设计图
export const saveDefinitionGraph = (id: string, data: { name: string; nodes: any[]; edges: any[] }) =>
  post<null>("/api/admin/wf/definition/" + id + "/graph", data)
