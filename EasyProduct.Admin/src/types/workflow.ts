// ========== 向后兼容类型（旧代码使用）==========

/** @deprecated 使用 Workflow 替代 */
export type WorkflowDefinition = Workflow

/** @deprecated 使用 { nodes: WfNode[], edges: WfEdge[] } 替代 */
export interface FlowGraph {
  nodes: FlowNode[]
  edges: FlowEdge[]
}

/** @deprecated 使用 WfNode 替代 */
export type FlowNode = WfNode

/** @deprecated 使用 WfEdge 替代 */
export type FlowEdge = WfEdge

/** @deprecated 使用 NodeType 替代 */
export type FlowNodeType = NodeType

/** 流程节点类型选项（表单使用） */
export const FLOW_NODE_TYPE_OPTIONS = [
  { label: '开始节点', value: 'start' },
  { label: '审批节点', value: 'approval' },
  { label: '抄送节点', value: 'cc' },
  { label: '条件分支', value: 'condition' },
  { label: '结束节点', value: 'end' }
]

/** 办理人类型选项 */
export const ASSIGNEE_TYPE_OPTIONS = [
  { label: '指定人员', value: 'user' },
  { label: '指定角色', value: 'role' },
  { label: '部门主管', value: 'deptLeader' },
  { label: '发起人自选', value: 'self' }
]

// ========== 工作流任务（待办/已办）==========

export type TaskStatus = 'pending' | 'approved' | 'rejected' | 'transferred'

export interface WorkflowTask {
  id: string
  workflowId: string
  workflowName: string
  instanceId: string
  nodeId: string
  nodeName: string
  assigneeId: string
  assigneeName: string
  status: TaskStatus
  comment?: string
  createdAt: string
  completedAt?: string
}

// ========== 流程实例 ==========

export type InstanceStatus = 'running' | 'completed' | 'terminated' | 'suspended'

export interface WorkflowInstance {
  id: string
  workflowId: string
  workflowName: string
  /** @deprecated 使用 workflowName */
  title?: string
  /** @deprecated 使用 workflowName */
  definitionName?: string
  businessKey?: string
  /** @deprecated 使用 businessKey */
  businessId?: string
  businessType?: string
  starterId: string
  starterName: string
  /** @deprecated 使用 starterName */
  applicantName?: string
  status: InstanceStatus
  currentNodeId?: string
  currentNodeName?: string
  /** @deprecated 使用 currentNodeName */
  currentNode?: string
  startTime: string
  /** @deprecated 使用 startTime */
  createdAt?: string
  endTime?: string
  /** @deprecated 使用 endTime */
  finishedAt?: string
  duration?: number
}

export interface WorkflowHistory {
  id: string
  instanceId: string
  nodeId: string
  nodeName: string
  action: 'start' | 'approve' | 'reject' | 'transfer' | 'comment'
  operatorId: string
  operatorName: string
  /** @deprecated 使用 operatorName */
  assigneeName?: string
  comment?: string
  createdAt: string
}

// ========== 工作流节点类型 ==========

export type NodeType =
  | 'start'
  | 'end'
  | 'approval'
  | 'condition'
  | 'cc'
  | 'copy'
  | 'parallel'
  | 'delay'
  | 'subprocess'
  | 'service'

export type NodeGroup = 'basic' | 'advanced'

export interface NodeTypeInfo {
  type: NodeType
  name: string
  group: NodeGroup
  color: string
  icon: string
}

export const NODE_TYPES: NodeTypeInfo[] = [
  { type: 'start', name: '开始', group: 'basic', color: '#334155', icon: 'VideoPlay' },
  { type: 'end', name: '结束', group: 'basic', color: '#334155', icon: 'VideoPause' },
  { type: 'approval', name: '审批', group: 'basic', color: '#409EFF', icon: 'User' },
  { type: 'condition', name: '条件', group: 'basic', color: '#CA8A04', icon: 'Share' },
  { type: 'cc', name: '抄送', group: 'basic', color: '#909399', icon: 'Message' },
  { type: 'copy', name: '复制', group: 'advanced', color: '#67C23A', icon: 'CopyDocument' },
  { type: 'parallel', name: '并行', group: 'advanced', color: '#E6A23C', icon: 'Grid' },
  { type: 'delay', name: '延迟', group: 'advanced', color: '#F56C6C', icon: 'Timer' },
  { type: 'subprocess', name: '子流程', group: 'advanced', color: '#8E44AD', icon: 'SetUp' },
  { type: 'service', name: '服务', group: 'advanced', color: '#17A2B8', icon: 'Service' }
]

// ========== 节点和边定义 ==========

export interface WfNode {
  id: string
  type: NodeType
  name: string
  position: { x: number; y: number }
  data: {
    rows: [string, string][]
    config?: any
    // 向后兼容
    assigneeType?: string
    assigneeName?: string
    x?: number
    y?: number
  }
}

export interface WfEdge {
  id: string
  source: string
  target: string
  label?: string
  sourceHandle?: 'yes' | 'no'
}

// ========== 工作流定义 ==========

export interface Workflow {
  id: string
  name: string
  description?: string
  code?: string
  category?: string
  status: 'draft' | 'published'
  version: number
  icon?: string
  nodes: WfNode[]
  edges: WfEdge[]
  successRate?: number
  lastRun?: string
  createdAt: string
  updatedAt: string
}

// ========== 执行相关 ==========

export type ExecTrigger = 'manual' | 'schedule' | 'api'
export type ExecStatus = 'running' | 'success' | 'error' | 'cancelled'

export interface Execution {
  id: string
  workflowId: string
  workflowName: string
  status: ExecStatus
  trigger: ExecTrigger
  startTime: string
  duration?: number
  nodeProgress: string
}

// ========== 执行状态（编辑器用）==========

export type NodeExecStatus = 'idle' | 'running' | 'success' | 'error' | 'wait'

export interface NodeExecState {
  status: NodeExecStatus
  durationMs?: number
  output?: string
}

export interface ExecState {
  [nodeId: string]: NodeExecState
}
