import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { WfNode, WfEdge, NodeType, Workflow } from '@/types/workflow'
import { NODE_TYPES } from '@/types/workflow'
import * as workflowApi from '@/api/workflow'

export const useWorkflowEditorStore = defineStore('workflowEditor', () => {
  // 流程基本信息
  const id = ref('')
  const name = ref('')
  const status = ref<'draft' | 'published'>('draft')
  const version = ref(1)
  const dirty = ref(false)

  // 节点和边
  const nodes = ref<WfNode[]>([])
  const edges = ref<WfEdge[]>([])

  // 撤销栈和选中节点
  const undoStack = ref<string[]>([])
  const selectedNodeId = ref('')

  // 添加节点
  function addNode(type: string, position: { x: number; y: number }) {
    const newNode: WfNode = {
      id: 'node-' + Date.now(),
      type: type as NodeType,
      name: NODE_TYPES.find(n => n.type === type)?.name || type,
      position,
      data: { rows: [] }
    }
    nodes.value.push(newNode)
    markDirty()
    saveUndo()
    return newNode
  }

  // 更新节点
  function updateNode(nodeId: string, data: Partial<WfNode>) {
    const node = nodes.value.find(n => n.id === nodeId)
    if (node) {
      Object.assign(node, data)
      markDirty()
    }
  }

  // 删除节点
  function removeNode(nodeId: string) {
    const index = nodes.value.findIndex(n => n.id === nodeId)
    if (index > -1) {
      nodes.value.splice(index, 1)
      // 同时删除相关边
      edges.value = edges.value.filter(e => e.source !== nodeId && e.target !== nodeId)
      markDirty()
      saveUndo()
    }
  }

  // 添加边
  function addEdge(edge: WfEdge) {
    // 检查是否已存在相同的边
    const exists = edges.value.some(e => e.source === edge.source && e.target === edge.target)
    if (exists) return
    edges.value.push(edge)
    markDirty()
    saveUndo()
  }

  // 删除边
  function removeEdge(edgeId: string) {
    const index = edges.value.findIndex(e => e.id === edgeId)
    if (index > -1) {
      edges.value.splice(index, 1)
      markDirty()
      saveUndo()
    }
  }

  // 边中间插入节点
  function insertNodeBetween(edgeId: string, nodeType: string) {
    const edge = edges.value.find(e => e.id === edgeId)
    if (!edge) return

    const sourceNode = nodes.value.find(n => n.id === edge.source)
    const targetNode = nodes.value.find(n => n.id === edge.target)
    if (!sourceNode || !targetNode) return

    const midPos = {
      x: (sourceNode.position.x + targetNode.position.x) / 2,
      y: (sourceNode.position.y + targetNode.position.y) / 2
    }

    const newNode: WfNode = {
      id: 'node-' + Date.now(),
      type: nodeType as NodeType,
      name: NODE_TYPES.find(n => n.type === nodeType)?.name || nodeType,
      position: midPos,
      data: { rows: [] }
    }
    nodes.value.push(newNode)

    // 删除原边，添加两条新边
    edges.value = edges.value.filter(e => e.id !== edgeId)
    edges.value.push(
      {
        id: 'e-' + Date.now() + '-1',
        source: edge.source,
        target: newNode.id,
        sourceHandle: edge.sourceHandle
      },
      {
        id: 'e-' + Date.now() + '-2',
        source: newNode.id,
        target: edge.target
      }
    )

    markDirty()
    saveUndo()
  }

  // 撤销功能
  function saveUndo() {
    const snapshot = JSON.stringify({ nodes: nodes.value, edges: edges.value })
    undoStack.value.push(snapshot)
    if (undoStack.value.length > 50) undoStack.value.shift()
  }

  function undo() {
    if (undoStack.value.length === 0) return
    const snapshot = JSON.parse(undoStack.value.pop()!)
    nodes.value = snapshot.nodes
    edges.value = snapshot.edges
    dirty.value = true
  }

  // 标记脏状态
  function markDirty() {
    dirty.value = true
  }

  // 保存流程
  async function save(): Promise<boolean> {
    if (!id.value) {
      // 新建流程
      const res = await workflowApi.createWorkflow({
        name: name.value,
        nodes: nodes.value,
        edges: edges.value
      })
      if (res.code === 200 && res.data) {
        id.value = res.data.id
        dirty.value = false
        return true
      }
      return false
    }

    // 更新流程
    const res = await workflowApi.updateWorkflow(id.value, {
      name: name.value,
      nodes: nodes.value,
      edges: edges.value
    })
    if (res.code === 200) {
      dirty.value = false
      return true
    }
    return false
  }

  // 发布流程
  async function publish(): Promise<boolean> {
    if (!id.value) return false
    const res = await workflowApi.publishWorkflow(id.value)
    if (res.code === 200 && res.data) {
      status.value = res.data.status
      version.value = res.data.version
      dirty.value = false
      return true
    }
    return false
  }

  // 加载流程
  async function load(workflowId: string): Promise<boolean> {
    const res = await workflowApi.getWorkflowDetail(workflowId)
    if (res.code === 200 && res.data) {
      const wf = res.data
      id.value = wf.id
      name.value = wf.name
      status.value = wf.status
      version.value = wf.version
      nodes.value = wf.nodes || []
      edges.value = wf.edges || []
      dirty.value = false
      undoStack.value = []
      return true
    }
    return false
  }

  // 新建空白流程
  function createNew() {
    id.value = ''
    name.value = '新流程'
    status.value = 'draft'
    version.value = 1
    nodes.value = [
      {
        id: 'node-start',
        type: 'start',
        name: '开始',
        position: { x: 100, y: 200 },
        data: { rows: [] }
      },
      {
        id: 'node-end',
        type: 'end',
        name: '结束',
        position: { x: 400, y: 200 },
        data: { rows: [] }
      }
    ]
    edges.value = []
    dirty.value = true
    undoStack.value = []
  }

  // 自动布局
  function autoLayout() {
    const nodeWidth = 200
    const gap = 100
    const sortedNodes = [...nodes.value].sort((a, b) => {
      const order: Record<string, number> = { start: 0, end: 100 }
      return (order[a.type] || 50) - (order[b.type] || 50)
    })

    sortedNodes.forEach((node, i) => {
      updateNode(node.id, {
        position: { x: 100 + i * (nodeWidth + gap), y: 200 }
      })
    })
    markDirty()
    saveUndo()
  }

  // 获取节点配置
  function getNodeConfig(nodeId: string) {
    const node = nodes.value.find(n => n.id === nodeId)
    return node?.data?.config
  }

  // 更新节点配置
  function updateNodeConfig(nodeId: string, config: any) {
    const node = nodes.value.find(n => n.id === nodeId)
    if (node) {
      node.data.config = config
      // 更新预览行
      node.data.rows = Object.entries(config)
        .filter(([key]) => !key.startsWith('_'))
        .slice(0, 3)
        .map(([key, value]) => [key, String(value)] as [string, string])
      markDirty()
      saveUndo()
    }
  }

  return {
    // State
    id,
    name,
    status,
    version,
    dirty,
    nodes,
    edges,
    undoStack,
    selectedNodeId,

    // Actions
    addNode,
    updateNode,
    removeNode,
    addEdge,
    removeEdge,
    insertNodeBetween,
    markDirty,
    undo,
    save,
    publish,
    load,
    createNew,
    autoLayout,
    getNodeConfig,
    updateNodeConfig
  }
})
