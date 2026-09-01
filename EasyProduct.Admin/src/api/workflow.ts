import { get, post } from '@/utils/request'
import type { ApiResponse, PageResult } from '@/types/api'
import type { Workflow, Execution, WfNode, WfEdge } from '@/types/workflow'

// 获取工作流列表
export function getWorkflowList(params: { pageIndex: number; pageSize: number; keyword?: string }): Promise<ApiResponse<PageResult<Workflow>>> {
  return get('/admin/workflow/list', params)
}

// 获取工作流详情
export function getWorkflowDetail(id: string): Promise<ApiResponse<Workflow>> {
  return get('/admin/workflow/detail', { id })
}

// 创建工作流
export function createWorkflow(data: { name: string; description?: string; nodes: WfNode[]; edges: WfEdge[] }): Promise<ApiResponse<Workflow>> {
  return post('/admin/workflow/create', data)
}

// 更新工作流
export function updateWorkflow(id: string, data: { name?: string; description?: string; nodes?: WfNode[]; edges?: WfEdge[] }): Promise<ApiResponse<null>> {
  return post('/admin/workflow/update', { id, ...data })
}

// 发布工作流
export function publishWorkflow(id: string): Promise<ApiResponse<{ status: 'published'; version: number }>> {
  return post('/admin/workflow/publish', { id })
}

// 删除工作流
export function deleteWorkflow(id: string): Promise<ApiResponse<null>> {
  return post('/admin/workflow/delete', { id })
}

// 验证工作流
export function validateWorkflow(id: string): Promise<ApiResponse<{ valid: boolean; errors: string[] }>> {
  return post('/admin/workflow/validate', { id })
}

// 获取执行历史
export function getExecutionList(params: { pageIndex: number; pageSize: number; workflowId?: string }): Promise<ApiResponse<PageResult<Execution>>> {
  return get('/admin/workflow/executions', params)
}

// 执行工作流
export function executeWorkflow(id: string, debug = false): Promise<ApiResponse<{ execId: string }>> {
  return post('/admin/workflow/execute', { id, debug })
}
