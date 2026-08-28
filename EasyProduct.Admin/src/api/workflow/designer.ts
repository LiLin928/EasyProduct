import { get, post } from '@/utils/request'
import type { FlowGraph } from '@/types/workflow'

/** 获取流程设计器画布数据 */
export const getFlowGraph = (definitionId?: string) =>
  get<FlowGraph>('/api/admin/wf/designer/graph', { definitionId })

/** 保存流程设计器画布数据 */
export const saveFlowGraph = (data: FlowGraph) =>
  post<null>('/api/admin/wf/designer/graph', data)

/** 校验流程图 */
export const validateFlowGraph = (data: FlowGraph) =>
  post<null>('/api/admin/wf/designer/validate', data)
