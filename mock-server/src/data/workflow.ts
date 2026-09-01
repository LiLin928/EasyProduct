import { mock } from 'mockjs'

// 工作流列表数据
export const workflowListData = {
  code: 200,
  message: 'success',
  data: {
    list: [
      {
        id: 'wf-001',
        name: '请假审批流程',
        description: '员工请假申请审批流程',
        status: 'published',
        version: 3,
        icon: 'Calendar',
        nodeCount: 5,
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
        nodeCount: 4,
        successRate: null,
        lastRun: null,
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
        nodeCount: 6,
        successRate: 95,
        lastRun: '2024-01-14T16:45:00',
        createdAt: '2024-01-05T11:00:00',
        updatedAt: '2024-01-14T16:45:00'
      },
      {
        id: 'wf-004',
        name: '入职审批流程',
        description: '新员工入职审批',
        status: 'published',
        version: 1,
        icon: 'UserFilled',
        nodeCount: 4,
        successRate: 100,
        lastRun: '2024-01-13T09:30:00',
        createdAt: '2024-01-08T10:00:00',
        updatedAt: '2024-01-13T09:30:00'
      },
      {
        id: 'wf-005',
        name: '合同审批流程',
        description: '合同签订审批',
        status: 'draft',
        version: 2,
        icon: 'Document',
        nodeCount: 5,
        successRate: null,
        lastRun: null,
        createdAt: '2024-01-12T15:30:00',
        updatedAt: '2024-01-12T15:30:00'
      }
    ],
    total: 5
  },
  timestamp: Date.now()
}

// 工作流详情数据
export const workflowDetailData: Record<string, any> = {
  'wf-001': {
    code: 200,
    message: 'success',
    data: {
      id: 'wf-001',
      name: '请假审批流程',
      description: '员工请假申请审批流程',
      status: 'published',
      version: 3,
      icon: 'Calendar',
      nodes: [
        {
          id: 'node-1',
          type: 'start',
          name: '开始',
          position: { x: 100, y: 200 },
          data: { rows: [] }
        },
        {
          id: 'node-2',
          type: 'approval',
          name: '部门经理审批',
          position: { x: 350, y: 200 },
          data: {
            rows: [['审批人', '部门经理']],
            config: { assigneeType: 'role', assigneeName: '部门经理' }
          }
        },
        {
          id: 'node-3',
          type: 'condition',
          name: '请假天数判断',
          position: { x: 600, y: 200 },
          data: {
            rows: [['条件', '天数 > 3']],
            config: { condition: 'days > 3' }
          }
        },
        {
          id: 'node-4',
          type: 'approval',
          name: 'HR审批',
          position: { x: 850, y: 100 },
          data: {
            rows: [['审批人', 'HR经理']],
            config: { assigneeType: 'role', assigneeName: 'HR经理' }
          }
        },
        {
          id: 'node-5',
          type: 'cc',
          name: '抄送通知',
          position: { x: 850, y: 300 },
          data: {
            rows: [['抄送人', '直属领导']],
            config: { ccUsers: ['直属领导'] }
          }
        },
        {
          id: 'node-6',
          type: 'end',
          name: '结束',
          position: { x: 1100, y: 200 },
          data: { rows: [] }
        }
      ],
      edges: [
        { id: 'e-1', source: 'node-1', target: 'node-2' },
        { id: 'e-2', source: 'node-2', target: 'node-3' },
        { id: 'e-3', source: 'node-3', target: 'node-4', sourceHandle: 'yes', label: '是' },
        { id: 'e-4', source: 'node-3', target: 'node-5', sourceHandle: 'no', label: '否' },
        { id: 'e-5', source: 'node-4', target: 'node-6' },
        { id: 'e-6', source: 'node-5', target: 'node-6' }
      ],
      createdAt: '2024-01-01T09:00:00',
      updatedAt: '2024-01-15T10:30:00'
    },
    timestamp: Date.now()
  },
  'wf-002': {
    code: 200,
    message: 'success',
    data: {
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
    timestamp: Date.now()
  }
}

// 执行历史数据
export const executionHistoryData = {
  code: 200,
  message: 'success',
  data: {
    list: [
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
        nodeProgress: '4/6'
      }
    ],
    total: 3
  },
  timestamp: Date.now()
}

// Mock 处理器
export const workflowMocks = {
  // 获取工作流列表
  getList: (req: any) => {
    const { keyword, pageIndex = 1, pageSize = 10 } = req.query || {}
    let list = [...workflowListData.data.list]

    if (keyword) {
      const kw = keyword.toLowerCase()
      list = list.filter(w =>
        w.name.toLowerCase().includes(kw) ||
        (w.description || '').toLowerCase().includes(kw)
      )
    }

    const start = (pageIndex - 1) * pageSize
    const end = start + pageSize

    return {
      code: 200,
      message: 'success',
      data: {
        list: list.slice(start, end),
        total: list.length
      },
      timestamp: Date.now()
    }
  },

  // 获取工作流详情
  getDetail: (req: any) => {
    const { id } = req.query || {}
    if (id && workflowDetailData[id]) {
      return workflowDetailData[id]
    }
    // 默认返回第一个
    return workflowDetailData['wf-001']
  },

  // 创建工作流
  create: (req: any) => {
    const { name } = req.body || {}
    return {
      code: 200,
      message: 'success',
      data: {
        id: 'wf-' + Date.now(),
        name: name || '新流程',
        status: 'draft',
        version: 1,
        nodes: [
          { id: 'node-start', type: 'start', name: '开始', position: { x: 100, y: 200 }, data: { rows: [] } },
          { id: 'node-end', type: 'end', name: '结束', position: { x: 400, y: 200 }, data: { rows: [] } }
        ],
        edges: [],
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      },
      timestamp: Date.now()
    }
  },

  // 更新工作流
  update: () => ({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  }),

  // 发布工作流
  publish: () => ({
    code: 200,
    message: 'success',
    data: {
      status: 'published',
      version: Math.floor(Math.random() * 10) + 1
    },
    timestamp: Date.now()
  }),

  // 删除工作流
  delete: () => ({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  }),

  // 验证工作流
  validate: () => ({
    code: 200,
    message: 'success',
    data: {
      valid: true,
      errors: []
    },
    timestamp: Date.now()
  }),

  // 获取执行历史
  getExecutions: () => executionHistoryData,

  // 执行工作流
  execute: () => ({
    code: 200,
    message: 'success',
    data: {
      execId: 'exec-' + Date.now()
    },
    timestamp: Date.now()
  })
}
