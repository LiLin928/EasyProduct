// src/data/wf-designer.ts
// 流程设计器画布数据 seed

import { guid } from '../helpers/id.js'

export interface DesignerNode {
  id: string
  type: 'start' | 'approval' | 'condition' | 'cc' | 'end'
  name: string
  x: number
  y: number
  assigneeType?: 'user' | 'role' | 'dept' | 'self'
  assigneeId?: string
  assigneeName?: string
  formFields?: string[]
}

export interface DesignerEdge {
  id: string
  source: string
  target: string
  label?: string
  condition?: string
}

export interface DesignerGraph {
  definitionId: string
  definitionName: string
  nodes: DesignerNode[]
  edges: DesignerEdge[]
}

// 以请假流程为例的默认画布
const nodeStart = { id: guid(), type: 'start' as const, name: '开始', x: 80, y: 200 }
const nodeApproval1 = { id: guid(), type: 'approval' as const, name: '直属上级审批', x: 320, y: 200, assigneeType: 'role' as const, assigneeId: 'r001', assigneeName: '部门经理', formFields: ['leaveType', 'leaveDays', 'reason'] }
const nodeCondition = { id: guid(), type: 'condition' as const, name: '天数判断', x: 580, y: 200 }
const nodeApproval2 = { id: guid(), type: 'approval' as const, name: '总经理审批', x: 820, y: 100, assigneeType: 'user' as const, assigneeId: 'u001', assigneeName: '张总' }
const nodeCc = { id: guid(), type: 'cc' as const, name: '抄送人事', x: 820, y: 320, assigneeType: 'dept' as const, assigneeId: 'd001', assigneeName: '人事部' }
const nodeEnd = { id: guid(), type: 'end' as const, name: '结束', x: 1060, y: 200 }

const allNodes = [nodeStart, nodeApproval1, nodeCondition, nodeApproval2, nodeCc, nodeEnd]

export const DESIGNER_GRAPHS: Record<string, DesignerGraph> = {
  // 默认绑定第一个定义（请假申请）
  default: {
    definitionId: '',
    definitionName: '请假申请',
    nodes: allNodes,
    edges: [
      { id: guid(), source: nodeStart.id, target: nodeApproval1.id },
      { id: guid(), source: nodeApproval1.id, target: nodeCondition.id },
      { id: guid(), source: nodeCondition.id, target: nodeApproval2.id, label: '>3天', condition: 'leaveDays > 3' },
      { id: guid(), source: nodeCondition.id, target: nodeCc.id, label: '<=3天', condition: 'leaveDays <= 3' },
      { id: guid(), source: nodeApproval2.id, target: nodeEnd.id },
      { id: guid(), source: nodeCc.id, target: nodeEnd.id },
    ],
  },
}
