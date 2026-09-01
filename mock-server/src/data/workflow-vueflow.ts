// src/data/workflow-vueflow.ts
// VueFlow 工作流设计器数据

import { guid } from '../helpers/id.js'

// VueFlow 格式的节点
export interface VueFlowNode {
  id: string
  type: string
  name: string
  position: { x: number; y: number }
  data: {
    rows: [string, string][]
    config?: any
  }
}

// VueFlow 格式的边
export interface VueFlowEdge {
  id: string
  source: string
  target: string
  label?: string
  sourceHandle?: 'yes' | 'no'
}

// VueFlow 流程定义
export interface VueFlowWorkflow {
  id: string
  name: string
  description?: string
  status: 'draft' | 'published'
  version: number
  icon?: string
  nodes: VueFlowNode[]
  edges: VueFlowEdge[]
  successRate?: number
  lastRun?: string
  createdAt: string
  updatedAt: string
}

// 节点类型配置
export const NODE_TYPES = [
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

// 工作流列表
export const WORKFLOW_LIST: VueFlowWorkflow[] = [
  {
    id: 'wf-001',
    name: '请假审批流程',
    description: '员工请假申请审批流程',
    status: 'published',
    version: 3,
    icon: 'Calendar',
    nodes: [
      { id: 'node-1', type: 'start', name: '开始', position: { x: 100, y: 200 }, data: { rows: [] } },
      { id: 'node-2', type: 'approval', name: '部门经理审批', position: { x: 350, y: 200 }, data: { rows: [['审批人', '部门经理']], config: { assigneeType: 'role', assigneeName: '部门经理' } } },
      { id: 'node-3', type: 'condition', name: '请假天数判断', position: { x: 600, y: 200 }, data: { rows: [['条件', '天数 > 3']], config: { condition: 'days > 3' } } },
      { id: 'node-4', type: 'approval', name: 'HR审批', position: { x: 850, y: 100 }, data: { rows: [['审批人', 'HR经理']], config: { assigneeType: 'role', assigneeName: 'HR经理' } } },
      { id: 'node-5', type: 'cc', name: '抄送通知', position: { x: 850, y: 300 }, data: { rows: [['抄送人', '直属领导']], config: { ccUsers: ['直属领导'] } } },
      { id: 'node-6', type: 'end', name: '结束', position: { x: 1100, y: 200 }, data: { rows: [] } }
    ],
    edges: [
      { id: 'e-1', source: 'node-1', target: 'node-2' },
      { id: 'e-2', source: 'node-2', target: 'node-3' },
      { id: 'e-3', source: 'node-3', target: 'node-4', sourceHandle: 'yes', label: '是' },
      { id: 'e-4', source: 'node-3', target: 'node-5', sourceHandle: 'no', label: '否' },
      { id: 'e-5', source: 'node-4', target: 'node-6' },
      { id: 'e-6', source: 'node-5', target: 'node-6' }
    ],
    successRate: 98,
    lastRun: '2024-01-15T10:30:00',
    createdAt: '2024-01-01T09:00:00',
    updatedAt: '2024-01-15T10:30:00'
  },
  {
    id: 'wf-002',
    name: '报销审批流程',
    description: '费用报销审批流程',
    status: 'draft',
    version: 1,
    icon: 'Money',
    nodes: [
      { id: 'node-1', type: 'start', name: '开始', position: { x: 100, y: 200 }, data: { rows: [] } },
      { id: 'node-2', type: 'approval', name: '直属领导审批', position: { x: 350, y: 200 }, data: { rows: [['审批人', '直属领导']] } },
      { id: 'node-3', type: 'condition', name: '金额判断', position: { x: 600, y: 200 }, data: { rows: [['条件', '金额 > 1000']] } },
      { id: 'node-4', type: 'approval', name: '财务审批', position: { x: 850, y: 200 }, data: { rows: [['审批人', '财务']] } },
      { id: 'node-5', type: 'end', name: '结束', position: { x: 1100, y: 200 }, data: { rows: [] } }
    ],
    edges: [
      { id: 'e-1', source: 'node-1', target: 'node-2' },
      { id: 'e-2', source: 'node-2', target: 'node-3' },
      { id: 'e-3', source: 'node-3', target: 'node-4', sourceHandle: 'yes', label: '是' },
      { id: 'e-4', source: 'node-3', target: 'node-5', sourceHandle: 'no', label: '否' },
      { id: 'e-5', source: 'node-4', target: 'node-5' }
    ],
    createdAt: '2024-01-10T14:20:00',
    updatedAt: '2024-01-10T14:20:00'
  },
  {
    id: 'wf-003',
    name: '采购审批流程',
    description: '物品采购申请审批',
    status: 'published',
    version: 2,
    icon: 'ShoppingCart',
    nodes: [
      { id: 'node-1', type: 'start', name: '开始', position: { x: 100, y: 200 }, data: { rows: [] } },
      { id: 'node-2', type: 'approval', name: '部门审批', position: { x: 350, y: 200 }, data: { rows: [['审批人', '部门经理']] } },
      { id: 'node-3', type: 'approval', name: '采购审批', position: { x: 600, y: 200 }, data: { rows: [['审批人', '采购经理']] } },
      { id: 'node-4', type: 'cc', name: '抄送财务', position: { x: 850, y: 200 }, data: { rows: [['抄送人', '财务']] } },
      { id: 'node-5', type: 'end', name: '结束', position: { x: 1100, y: 200 }, data: { rows: [] } }
    ],
    edges: [
      { id: 'e-1', source: 'node-1', target: 'node-2' },
      { id: 'e-2', source: 'node-2', target: 'node-3' },
      { id: 'e-3', source: 'node-3', target: 'node-4' },
      { id: 'e-4', source: 'node-4', target: 'node-5' }
    ],
    successRate: 95,
    lastRun: '2024-01-14T16:45:00',
    createdAt: '2024-01-05T11:00:00',
    updatedAt: '2024-01-14T16:45:00'
  }
]

// 执行历史
export const EXECUTION_HISTORY = [
  {
    id: 'exec-001',
    workflowId: 'wf-001',
    workflowName: '请假审批流程',
    status: 'success',
    trigger: 'manual',
    startTime: '2024-01-15T10:30:00',
    duration: 12500,
    nodeProgress: '6/6'
  },
  {
    id: 'exec-002',
    workflowId: 'wf-001',
    workflowName: '请假审批流程',
    status: 'success',
    trigger: 'api',
    startTime: '2024-01-14T15:20:00',
    duration: 8200,
    nodeProgress: '6/6'
  },
  {
    id: 'exec-003',
    workflowId: 'wf-003',
    workflowName: '采购审批流程',
    status: 'error',
    trigger: 'manual',
    startTime: '2024-01-14T16:45:00',
    duration: 5600,
    nodeProgress: '4/5'
  }
]
