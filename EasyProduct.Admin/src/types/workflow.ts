// 工作流模块类型定义

// ── 流程定义 ──
export type DefinitionStatus = 'draft' | 'published' | 'disabled'

export interface WorkflowDefinition {
  id: string
  name: string
  code: string
  category: string
  description: string
  status: DefinitionStatus
  version: number
  createdBy: string
  createdAt: string
  updatedAt: string
}

export const DEFINITION_STATUS_OPTIONS = [
  { value: 'draft', labelKey: 'workflow.definition.statusDraft' },
  { value: 'published', labelKey: 'workflow.definition.statusPublished' },
  { value: 'disabled', labelKey: 'workflow.definition.statusDisabled' },
] as const

// ── 流程实例 ──
export type InstanceStatus = 'running' | 'approved' | 'rejected' | 'cancelled'

export interface WorkflowInstance {
  id: string
  definitionName: string
  definitionCode: string
  businessId: string
  businessType: string
  title: string
  applicantId: string
  applicantName: string
  currentNode: string
  status: InstanceStatus
  createdAt: string
  finishedAt: string
}

export const INSTANCE_STATUS_OPTIONS = [
  { value: 'running', labelKey: 'workflow.instance.statusRunning' },
  { value: 'approved', labelKey: 'workflow.instance.statusApproved' },
  { value: 'rejected', labelKey: 'workflow.instance.statusRejected' },
  { value: 'cancelled', labelKey: 'workflow.instance.statusCancelled' },
] as const

// ── 审批任务 ──
export type TaskStatus = 'pending' | 'approved' | 'rejected' | 'transferred'

export interface WorkflowTask {
  id: string
  instanceId: string
  instanceTitle: string
  definitionName: string
  nodeName: string
  assigneeId: string
  assigneeName: string
  applicantName: string
  comment: string
  status: TaskStatus
  createdAt: string
  finishedAt: string
}

export const TASK_STATUS_OPTIONS = [
  { value: 'pending', labelKey: 'workflow.task.statusPending' },
  { value: 'approved', labelKey: 'workflow.task.statusApproved' },
  { value: 'rejected', labelKey: 'workflow.task.statusRejected' },
  { value: 'transferred', labelKey: 'workflow.task.statusTransferred' },
] as const

// ── 流程设计器 ──
export type FlowNodeType = 'start' | 'approval' | 'condition' | 'cc' | 'end'

export interface FlowNode {
  id: string
  type: FlowNodeType
  name: string
  x: number
  y: number
  assigneeType?: 'user' | 'role' | 'dept' | 'self'
  assigneeId?: string
  assigneeName?: string
  formFields?: string[]
}

export interface FlowEdge {
  id: string
  source: string
  target: string
  label?: string
  condition?: string
}

export interface FlowGraph {
  definitionId: string
  definitionName: string
  nodes: FlowNode[]
  edges: FlowEdge[]
}

export const FLOW_NODE_TYPE_OPTIONS = [
  { value: 'start', labelKey: 'workflow.designer.nodeStart', icon: 'Position' },
  { value: 'approval', labelKey: 'workflow.designer.nodeApproval', icon: 'UserFilled' },
  { value: 'condition', labelKey: 'workflow.designer.nodeCondition', icon: 'Switch' },
  { value: 'cc', labelKey: 'workflow.designer.nodeCc', icon: 'Message' },
  { value: 'end', labelKey: 'workflow.designer.nodeEnd', icon: 'CircleClose' },
] as const

export const ASSIGNEE_TYPE_OPTIONS = [
  { value: 'user', labelKey: 'workflow.designer.assigneeUser' },
  { value: 'role', labelKey: 'workflow.designer.assigneeRole' },
  { value: 'dept', labelKey: 'workflow.designer.assigneeDept' },
  { value: 'self', labelKey: 'workflow.designer.assigneeSelf' },
] as const

// ── 审批历史 ──
export interface WorkflowHistory {
  id: string
  instanceId: string
  nodeName: string
  assigneeName: string
  action: string
  comment: string
  createdAt: string
}

export const HISTORY_ACTION_OPTIONS = [
  { value: 'submit', labelKey: 'workflow.history.actionSubmit' },
  { value: 'approve', labelKey: 'workflow.history.actionApprove' },
  { value: 'reject', labelKey: 'workflow.history.actionReject' },
  { value: 'transfer', labelKey: 'workflow.history.actionTransfer' },
  { value: 'cancel', labelKey: 'workflow.history.actionCancel' },
] as const
