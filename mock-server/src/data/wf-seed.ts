// src/data/wf-seed.ts
// 工作流实例+任务+历史 seed

import { guid, isoTime } from '../helpers/id.js'

// ────────────── 实例 ──────────────
export interface WorkflowInstance {
  id: string
  definitionId: string
  definitionName: string
  definitionCode: string
  businessId: string
  businessType: string
  title: string
  applicantId: string
  applicantName: string
  currentNode: string
  status: 'running' | 'approved' | 'rejected' | 'cancelled'
  createdAt: string
  finishedAt: string
}

export const INSTANCES: WorkflowInstance[] = [
  {
    id: guid(),
    definitionId: 'wf-def-001',
    definitionName: '请假申请',
    definitionCode: 'WF_LEAVE',
    businessId: 'LV202608001',
    businessType: 'leave',
    title: '张三-年假申请-3天',
    applicantId: 'user-zhang',
    applicantName: '张三',
    currentNode: '部门经理审批',
    status: 'running',
    createdAt: isoTime(-2),
    finishedAt: '',
  },
  {
    id: guid(),
    definitionId: 'wf-def-002',
    definitionName: '采购审批',
    definitionCode: 'WF_PURCHASE',
    businessId: 'PO202608001',
    businessType: 'purchase',
    title: '李四-办公用品采购-5000元',
    applicantId: 'user-li',
    applicantName: '李四',
    currentNode: '财务审批',
    status: 'running',
    createdAt: isoTime(-3),
    finishedAt: '',
  },
  {
    id: guid(),
    definitionId: 'wf-def-003',
    definitionName: '费用报销',
    definitionCode: 'WF_EXPENSE',
    businessId: 'EX202608001',
    businessType: 'expense',
    title: '王五-差旅费报销-3200元',
    applicantId: 'user-wang',
    applicantName: '王五',
    currentNode: '',
    status: 'approved',
    createdAt: isoTime(-8),
    finishedAt: isoTime(-5),
  },
  {
    id: guid(),
    definitionId: 'wf-def-001',
    definitionName: '请假申请',
    definitionCode: 'WF_LEAVE',
    businessId: 'LV202607015',
    businessType: 'leave',
    title: '赵六-病假申请-2天',
    applicantId: 'user-zhao',
    applicantName: '赵六',
    currentNode: '',
    status: 'approved',
    createdAt: isoTime(-10),
    finishedAt: isoTime(-8),
  },
  {
    id: guid(),
    definitionId: 'wf-def-002',
    definitionName: '采购审批',
    definitionCode: 'WF_PURCHASE',
    businessId: 'PO202607008',
    businessType: 'purchase',
    title: '李四-设备采购-12000元',
    applicantId: 'user-li',
    applicantName: '李四',
    currentNode: '',
    status: 'rejected',
    createdAt: isoTime(-12),
    finishedAt: isoTime(-10),
  },
  {
    id: guid(),
    definitionId: 'wf-def-004',
    definitionName: '合同审批',
    definitionCode: 'WF_CONTRACT',
    businessId: 'CT202608003',
    businessType: 'contract',
    title: '孙七-供应商合同审批',
    applicantId: 'user-sun',
    applicantName: '孙七',
    currentNode: '总经理审批',
    status: 'running',
    createdAt: isoTime(-1),
    finishedAt: '',
  },
  {
    id: guid(),
    definitionId: 'wf-def-005',
    definitionName: '出差申请',
    definitionCode: 'WF_TRAVEL',
    businessId: 'TV202608002',
    businessType: 'travel',
    title: '周八-北京出差申请-3天',
    applicantId: 'user-zhou',
    applicantName: '周八',
    currentNode: '',
    status: 'approved',
    createdAt: isoTime(-6),
    finishedAt: isoTime(-4),
  },
  {
    id: guid(),
    definitionId: 'wf-def-001',
    definitionName: '请假申请',
    definitionCode: 'WF_LEAVE',
    businessId: 'LV202608003',
    businessType: 'leave',
    title: '吴九-事假申请-1天',
    applicantId: 'user-wu',
    applicantName: '吴九',
    currentNode: '',
    status: 'cancelled',
    createdAt: isoTime(-5),
    finishedAt: isoTime(-4),
  },
]

// ────────────── 任务 ──────────────
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
  status: 'pending' | 'approved' | 'rejected' | 'transferred'
  createdAt: string
  finishedAt: string
}

export const TASKS: WorkflowTask[] = [
  // 待办 (pending)
  {
    id: guid(),
    instanceId: INSTANCES[0].id,
    instanceTitle: INSTANCES[0].title,
    definitionName: INSTANCES[0].definitionName,
    nodeName: '部门经理审批',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '张三',
    comment: '',
    status: 'pending',
    createdAt: isoTime(-2),
    finishedAt: '',
  },
  {
    id: guid(),
    instanceId: INSTANCES[1].id,
    instanceTitle: INSTANCES[1].title,
    definitionName: INSTANCES[1].definitionName,
    nodeName: '财务审批',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '李四',
    comment: '',
    status: 'pending',
    createdAt: isoTime(-3),
    finishedAt: '',
  },
  {
    id: guid(),
    instanceId: INSTANCES[5].id,
    instanceTitle: INSTANCES[5].title,
    definitionName: INSTANCES[5].definitionName,
    nodeName: '总经理审批',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '孙七',
    comment: '',
    status: 'pending',
    createdAt: isoTime(-1),
    finishedAt: '',
  },
  // 已办 (approved/rejected)
  {
    id: guid(),
    instanceId: INSTANCES[2].id,
    instanceTitle: INSTANCES[2].title,
    definitionName: INSTANCES[2].definitionName,
    nodeName: '直属上级审批',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '王五',
    comment: '同意报销',
    status: 'approved',
    createdAt: isoTime(-8),
    finishedAt: isoTime(-7),
  },
  {
    id: guid(),
    instanceId: INSTANCES[2].id,
    instanceTitle: INSTANCES[2].title,
    definitionName: INSTANCES[2].definitionName,
    nodeName: '财务审核',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '王五',
    comment: '金额核对无误',
    status: 'approved',
    createdAt: isoTime(-7),
    finishedAt: isoTime(-5),
  },
  {
    id: guid(),
    instanceId: INSTANCES[3].id,
    instanceTitle: INSTANCES[3].title,
    definitionName: INSTANCES[3].definitionName,
    nodeName: '部门经理审批',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '赵六',
    comment: '同意',
    status: 'approved',
    createdAt: isoTime(-10),
    finishedAt: isoTime(-9),
  },
  {
    id: guid(),
    instanceId: INSTANCES[4].id,
    instanceTitle: INSTANCES[4].title,
    definitionName: INSTANCES[4].definitionName,
    nodeName: '部门经理审批',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '李四',
    comment: '金额超预算，请重新核实',
    status: 'rejected',
    createdAt: isoTime(-12),
    finishedAt: isoTime(-11),
  },
  {
    id: guid(),
    instanceId: INSTANCES[6].id,
    instanceTitle: INSTANCES[6].title,
    definitionName: INSTANCES[6].definitionName,
    nodeName: '部门经理审批',
    assigneeId: 'user-admin',
    assigneeName: '管理员',
    applicantName: '周八',
    comment: '同意出差',
    status: 'approved',
    createdAt: isoTime(-6),
    finishedAt: isoTime(-5),
  },
]

// ────────────── 历史 ──────────────
export interface WorkflowHistory {
  id: string
  instanceId: string
  nodeName: string
  assigneeName: string
  action: string
  comment: string
  createdAt: string
}

export const HISTORIES: WorkflowHistory[] = [
  // 实例1 (请假-张三, running)
  { id: guid(), instanceId: INSTANCES[0].id, nodeName: '发起人', assigneeName: '张三', action: 'submit', comment: '年假3天', createdAt: isoTime(-2) },
  // 实例2 (采购-李四, running)
  { id: guid(), instanceId: INSTANCES[1].id, nodeName: '发起人', assigneeName: '李四', action: 'submit', comment: '办公用品采购', createdAt: isoTime(-3) },
  { id: guid(), instanceId: INSTANCES[1].id, nodeName: '部门经理审批', assigneeName: '管理员', action: 'approve', comment: '同意采购', createdAt: isoTime(-2) },
  // 实例3 (报销-王五, approved)
  { id: guid(), instanceId: INSTANCES[2].id, nodeName: '发起人', assigneeName: '王五', action: 'submit', comment: '差旅费3200元', createdAt: isoTime(-8) },
  { id: guid(), instanceId: INSTANCES[2].id, nodeName: '直属上级审批', assigneeName: '管理员', action: 'approve', comment: '同意报销', createdAt: isoTime(-7) },
  { id: guid(), instanceId: INSTANCES[2].id, nodeName: '财务审核', assigneeName: '管理员', action: 'approve', comment: '金额核对无误', createdAt: isoTime(-5) },
  // 实例4 (请假-赵六, approved)
  { id: guid(), instanceId: INSTANCES[3].id, nodeName: '发起人', assigneeName: '赵六', action: 'submit', comment: '病假2天', createdAt: isoTime(-10) },
  { id: guid(), instanceId: INSTANCES[3].id, nodeName: '部门经理审批', assigneeName: '管理员', action: 'approve', comment: '同意', createdAt: isoTime(-8) },
  // 实例5 (采购-李四, rejected)
  { id: guid(), instanceId: INSTANCES[4].id, nodeName: '发起人', assigneeName: '李四', action: 'submit', comment: '设备采购12000元', createdAt: isoTime(-12) },
  { id: guid(), instanceId: INSTANCES[4].id, nodeName: '部门经理审批', assigneeName: '管理员', action: 'reject', comment: '金额超预算，请重新核实', createdAt: isoTime(-10) },
  // 实例6 (合同-孙七, running)
  { id: guid(), instanceId: INSTANCES[5].id, nodeName: '发起人', assigneeName: '孙七', action: 'submit', comment: '供应商合同审批', createdAt: isoTime(-1) },
  { id: guid(), instanceId: INSTANCES[5].id, nodeName: '部门经理审批', assigneeName: '管理员', action: 'approve', comment: '同意', createdAt: isoTime(-1) },
  // 实例7 (出差-周八, approved)
  { id: guid(), instanceId: INSTANCES[6].id, nodeName: '发起人', assigneeName: '周八', action: 'submit', comment: '北京出差3天', createdAt: isoTime(-6) },
  { id: guid(), instanceId: INSTANCES[6].id, nodeName: '部门经理审批', assigneeName: '管理员', action: 'approve', comment: '同意出差', createdAt: isoTime(-5) },
  // 实例8 (请假-吴九, cancelled)
  { id: guid(), instanceId: INSTANCES[7].id, nodeName: '发起人', assigneeName: '吴九', action: 'submit', comment: '事假1天', createdAt: isoTime(-5) },
  { id: guid(), instanceId: INSTANCES[7].id, nodeName: '发起人', assigneeName: '吴九', action: 'cancel', comment: '临时取消', createdAt: isoTime(-4) },
]
