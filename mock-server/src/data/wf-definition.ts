// src/data/wf-definition.ts
// 工作流定义 seed

import { guid, isoTime } from '../helpers/id.js'

export interface WorkflowDefinition {
  id: string
  name: string
  code: string
  category: string
  description: string
  status: 'draft' | 'published' | 'disabled'
  version: number
  createdBy: string
  createdAt: string
  updatedAt: string
}

export const DEFINITIONS: WorkflowDefinition[] = [
  {
    id: guid(),
    name: '请假申请',
    code: 'WF_LEAVE',
    category: '行政',
    description: '员工请假审批流程，直属上级审批',
    status: 'published',
    version: 2,
    createdBy: 'admin',
    createdAt: isoTime(-30),
    updatedAt: isoTime(-5),
  },
  {
    id: guid(),
    name: '采购审批',
    code: 'WF_PURCHASE',
    category: '采购',
    description: '采购订单审批流程，部门经理→财务审批',
    status: 'published',
    version: 1,
    createdBy: 'admin',
    createdAt: isoTime(-25),
    updatedAt: isoTime(-10),
  },
  {
    id: guid(),
    name: '费用报销',
    code: 'WF_EXPENSE',
    category: '财务',
    description: '费用报销审批流程，直属上级→财务审核',
    status: 'published',
    version: 3,
    createdBy: 'admin',
    createdAt: isoTime(-20),
    updatedAt: isoTime(-3),
  },
  {
    id: guid(),
    name: '合同审批',
    code: 'WF_CONTRACT',
    category: '法务',
    description: '合同审批流程，部门经理→法务→总经理',
    status: 'published',
    version: 1,
    createdBy: 'admin',
    createdAt: isoTime(-15),
    updatedAt: isoTime(-8),
  },
  {
    id: guid(),
    name: '出差申请',
    code: 'WF_TRAVEL',
    category: '行政',
    description: '出差申请审批流程，直属上级审批',
    status: 'published',
    version: 1,
    createdBy: 'admin',
    createdAt: isoTime(-10),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    name: '加班申请',
    code: 'WF_OVERTIME',
    category: '行政',
    description: '加班申请审批流程',
    status: 'draft',
    version: 1,
    createdBy: 'admin',
    createdAt: isoTime(-5),
    updatedAt: isoTime(-1),
  },
]
