# 工作流设计器 VueFlow 迁移方案

> 基于 EasyRag 项目 VueFlow 实现，将 EasyProduct 原生 SVG+DOM 工作流设计器迁移到 VueFlow

## 📋 项目对比

| 特性 | 当前实现 (EasyProduct) | 目标实现 (VueFlow) |
|------|------------------------|--------------------|
| 渲染引擎 | 原生 SVG + DOM | VueFlow (@vue-flow/core) |
| 节点拖拽 | 自定义实现 | VueFlow 内置 |
| 连线绘制 | SVG Path 手动计算 | VueFlow 内置 (贝塞尔曲线) |
| 缩放平移 | 未实现 | 内置支持 |
| 撤销重做 | 未实现 | Pinia Store + 快照 |
| 节点类型 | 5种 (start/approval/condition/cc/end) | 扩展至 10+ 种 |
| 执行调试 | 无 | 完整调试面板 |
| 连线插入节点 | 无 | 边点击插入 |

## 🎯 迁移目标

1. **功能增强**：从基础5节点扩展为完整审批工作流引擎
2. **交互升级**：引入 VueFlow 原生拖拽、缩放、平移
3. **体验优化**：撤销/重做、边插入节点、执行调试
4. **架构统一**：与 EasyRag 技术栈保持一致

---

## 📦 依赖安装

```bash
cd EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap
```

---

## 🗂️ 文件结构变更

```
src/views/workflow/designer/
├── index.vue                 # 主页面（重写）
├── components/
│   ├── WorkflowCanvas.vue    # VueFlow 画布（新增）
│   ├── BaseNodeCard.vue      # 节点卡片（新增）
│   ├── AddButtonEdge.vue     # 可插入节点的边（新增）
│   ├── NodeConfigModal.vue   # 节点配置弹窗（新增）
│   ├── NodePalette.vue       # 节点面板（保留，适配 VueFlow）
│   └── ExecutionPanel.vue    # 执行调试面板（新增）
├── composables/
│   └── useWorkflowDesigner.ts # 设计器逻辑（新增）
└── types/
    └── workflow-designer.ts  # 类型定义（新增/扩展）

src/stores/
├── workflowEditor.ts         # 编辑器 Store（新增）
└── workflowExecution.ts    # 执行 Store（新增）

src/api/
└── workflow.ts             # 工作流 API（新增/扩展）

src/types/
└── workflow.ts             # 全局类型定义（新增）
```

---

## 🔄 数据结构对比

### 当前数据结构 (EasyProduct)

```typescript
interface FlowNode {
  id: string
  type: 'start' | 'approval' | 'condition' | 'cc' | 'end'
  name: string
  x: number
  y: number
  assigneeType?: 'user' | 'role' | 'dept'
  assigneeId?: string
  assigneeName?: string
  conditionExpr?: string
}

interface FlowEdge {
  id: string
  source: string
  target: string
  conditionResult?: 'yes' | 'no'
}
```

### 目标数据结构 (VueFlow)

```typescript
// types/workflow.ts
export type NodeType = 
  | 'start'           // 开始节点
  | 'end'             // 结束节点  
  | 'approval'        // 审批节点
  | 'condition'       // 条件分支
  | 'cc'              // 抄送节点
  | 'copy'            // 复制节点
  | 'parallel'        // 并行分支
  | 'delay'           // 延迟节点
  | 'subprocess'      // 子流程
  | 'service'         // 服务节点

export interface WfNode {
  id: string
  type: NodeType
  name: string
  position: { x: number; y: number }
  data: {
    config?: any              // 节点详细配置
    rows: [string, string][]  // 预览行（显示在卡片上）
  }
}

export interface WfEdge {
  id: string
  source: string
  target: string
  label?: string
  sourceHandle?: 'yes' | 'no' | undefined
}

// 节点类型定义
export interface NodeTypeInfo {
  type: NodeType
  name: string
  group: 'basic' | 'advanced'
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
```

---

## 🎨 组件实现方案

### 1. WorkflowCanvas.vue - VueFlow 画布

```vue
<script setup lang="ts">
import { ref, watch, markRaw } from 'vue'
import { VueFlow, useVueFlow, type Node, type Edge } from '@vue-flow/core'
import { Background } from '@vue-flow/background'
import { Controls } from '@vue-flow/controls'
import { MiniMap } from '@vue-flow/minimap'
import { useWorkflowEditorStore } from '@/stores/workflowEditor'
import BaseNodeCard from './BaseNodeCard.vue'
import AddButtonEdge from './AddButtonEdge.vue'
import type { WfEdge } from '@/types/workflow'

import '@vue-flow/core/dist/style.css'
import '@vue-flow/core/dist/theme-default.css'
import '@vue-flow/controls/dist/style.css'
import '@vue-flow/minimap/dist/style.css'

const editorStore = useWorkflowEditorStore()
const { onConnect, onNodeDragStop, onNodeDoubleClick, onNodesChange } = useVueFlow()

const edgeTypes = { default: markRaw(AddButtonEdge) }
const nodes = ref<Node[]>([])
const edges = ref<Edge[]>([])

// 同步 Store 数据到 VueFlow
watch(() => editorStore.nodes, (storeNodes) => {
  nodes.value = storeNodes.map(n => ({
    id: n.id,
    type: n.type,
    position: n.position,
    data: { ...n.data, name: n.name }
  }))
}, { immediate: true, deep: true })

watch(() => editorStore.edges, (storeEdges) => {
  edges.value = storeEdges.map(e => ({
    id: e.id,
    source: e.source,
    target: e.target,
    label: e.label,
    sourceHandle: e.sourceHandle
  }))
}, { immediate: true, deep: true })

// 事件监听
onConnect((params) => {
  editorStore.addEdge({
    id: 'e-' + Date.now(),
    source: params.source,
    target: params.target,
    sourceHandle: params.sourceHandle as 'yes' | 'no' | undefined
  })
})

onNodeDragStop((event) => {
  editorStore.updateNode(event.node.id, {
    position: { x: event.node.position.x, y: event.node.position.y }
  })
})

onNodeDoubleClick((event) => {
  editorStore.selectedNodeId = event.node.id
})

onNodesChange((changes) => {
  changes.forEach(change => {
    if (change.type === 'remove') editorStore.removeNode(change.id)
  })
})

// 拖拽添加节点
function handleDrop(event: DragEvent) {
  const nodeType = event.dataTransfer?.getData('nodeType')
  if (!nodeType) return
  const bounds = (event.currentTarget as HTMLElement).getBoundingClientRect()
  editorStore.addNode(nodeType, {
    x: event.clientX - bounds.left,
    y: event.clientY - bounds.top
  })
}

function handleDragOver(event: DragEvent) {
  event.preventDefault()
  if (event.dataTransfer) event.dataTransfer.dropEffect = 'move'
}
</script>

<template>
  <div class="workflow-canvas" @drop="handleDrop" @dragover="handleDragOver">
    <VueFlow
      v-model:nodes="nodes"
      v-model:edges="edges"
      :default-zoom="1"
      :min-zoom="0.2"
      :max-zoom="4"
      :delete-key-code="['Backspace', 'Delete']"
      :edge-types="edgeTypes"
      fit-view-on-init
      class="vue-flow-canvas"
    >
      <Background pattern-gap="20" :size="1" />
      <Controls />
      <MiniMap />
      
      <!-- 节点模板注册 -->
      <template v-for="type in NODE_TYPES.map(n => n.type)" :key="type" #[`node-${type}`]="nodeProps">
        <BaseNodeCard :node="nodeProps" />
      </template>
    </VueFlow>
  </div>
</template>

<style lang="scss" scoped>
.workflow-canvas {
  width: 100%;
  height: 100%;
}

.vue-flow-canvas {
  background: #fafafa;
}

:deep(.vue-flow__node) {
  padding: 0;
  border: none;
  background: transparent;
}
</style>
```

### 2. BaseNodeCard.vue - 节点卡片

```vue
<script setup lang="ts">
import { computed } from 'vue'
import { Handle, Position, type NodeProps } from '@vue-flow/core'
import type { NodeType } from '@/types/workflow'
import { useWorkflowEditorStore } from '@/stores/workflowEditor'

const props = defineProps<{
  node: NodeProps
  selected?: boolean
}>()

const editorStore = useWorkflowEditorStore()

const nodeConfig: Record<NodeType, { color: string; bgColor: string; icon: string }> = {
  start: { color: '#334155', bgColor: '#f1f5f9', icon: 'VideoPlay' },
  end: { color: '#334155', bgColor: '#f1f5f9', icon: 'VideoPause' },
  approval: { color: '#409EFF', bgColor: '#ecf5ff', icon: 'User' },
  condition: { color: '#CA8A04', bgColor: '#fefce8', icon: 'Share' },
  cc: { color: '#909399', bgColor: '#f4f4f5', icon: 'Message' },
  copy: { color: '#67C23A', bgColor: '#f0f9eb', icon: 'CopyDocument' },
  parallel: { color: '#E6A23C', bgColor: '#fdf6ec', icon: 'Grid' },
  delay: { color: '#F56C6C', bgColor: '#fef0f0', icon: 'Timer' },
  subprocess: { color: '#8E44AD', bgColor: '#f5f0fa', icon: 'SetUp' },
  service: { color: '#17A2B8', bgColor: '#e6f7f9', icon: 'Service' }
}

const config = computed(() => nodeConfig[props.node.type as NodeType] || nodeConfig.start)

const handleStyle = {
  background: '#fff',
  border: '2px solid #409eff',
  width: '10px',
  height: '10px'
}

function openConfig() {
  editorStore.selectedNodeId = props.node.id
}
</script>

<template>
  <div
    class="base-node-card"
    :class="{ selected }"
    :style="{ borderColor: config.color, backgroundColor: config.bgColor }"
  >
    <!-- 设置按钮 -->
    <div class="node-settings-btn" @click.stop="openConfig">
      <el-icon :size="14"><Setting /></el-icon>
    </div>

    <div class="node-header">
      <el-icon :style="{ color: config.color }" :size="18">
        <component :is="config.icon" />
      </el-icon>
      <span class="node-name">{{ node.data?.name || node.type }}</span>
    </div>

    <!-- 配置预览行 -->
    <div v-if="node.data?.rows?.length" class="node-rows">
      <div v-for="(row, i) in node.data.rows" :key="i" class="node-row">
        <span class="row-key">{{ row[0] }}</span>
        <span class="row-value">{{ row[1] }}</span>
      </div>
    </div>

    <!-- 输入连接点 -->
    <Handle v-if="node.type !== 'start'" type="target" :position="Position.Left" :style="handleStyle" />

    <!-- 输出连接点：条件节点双出口 -->
    <template v-if="node.type === 'condition'">
      <Handle id="yes" type="source" :position="Position.Right" :style="{ ...handleStyle, top: '30%' }" />
      <Handle id="no" type="source" :position="Position.Right" :style="{ ...handleStyle, top: '70%' }" />
    </template>
    <Handle v-else-if="node.type !== 'end'" type="source" :position="Position.Right" :style="handleStyle" />
  </div>
</template>

<style lang="scss" scoped>
.base-node-card {
  position: relative;
  min-width: 180px;
  padding: 12px 16px;
  border-radius: 8px;
  border-width: 2px;
  border-style: solid;
  cursor: pointer;
  transition: all 0.2s;

  &.selected {
    box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.5);
  }

  &:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  }

  &:hover .node-settings-btn {
    opacity: 1;
  }
}

.node-settings-btn {
  position: absolute;
  top: 4px;
  right: 4px;
  width: 22px;
  height: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  cursor: pointer;
  opacity: 0;
  transition: opacity 0.2s;
  color: #909399;

  &:hover {
    background: rgba(0, 0, 0, 0.08);
    color: #409eff;
  }
}

.node-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;

  .node-name {
    flex: 1;
    font-size: 14px;
    font-weight: 500;
    color: #303133;
  }
}

.node-rows {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.node-row {
  display: flex;
  gap: 8px;
  font-size: 12px;

  .row-key { color: #909399; min-width: 50px; }
  .row-value { color: #606266; flex: 1; }
}
</style>
```

### 3. AddButtonEdge.vue - 可插入节点的边

```vue
<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { BaseEdge, EdgeLabelRenderer, getBezierPath, type EdgeProps } from '@vue-flow/core'
import { useWorkflowEditorStore } from '@/stores/workflowEditor'
import { NODE_TYPES } from '@/types/workflow'

const props = defineProps<EdgeProps>()
const editorStore = useWorkflowEditorStore()
const showMenu = ref(false)

const pathData = computed(() => getBezierPath(props))
const basicNodes = NODE_TYPES.filter(n => n.group === 'basic')
const advancedNodes = NODE_TYPES.filter(n => n.group === 'advanced')

function handleInsert(type: string) {
  if (props.id) editorStore.insertNodeBetween(props.id, type)
  showMenu.value = false
}

function handleDocumentClick() { showMenu.value = false }

onMounted(() => document.addEventListener('click', handleDocumentClick))
onUnmounted(() => document.removeEventListener('click', handleDocumentClick))
</script>

<template>
  <BaseEdge :path="pathData[0]" />
  
  <EdgeLabelRenderer>
    <!-- 插入按钮 -->
    <div
      class="edge-plus-wrapper"
      :style="{ transform: `translate(-50%, -50%) translate(${pathData[1]}px, ${pathData[2]}px)` }"
      @click.stop="showMenu = !showMenu"
    >
      <div class="edge-plus-btn">+</div>
    </div>

    <!-- 节点菜单 -->
    <div
      v-if="showMenu"
      class="edge-node-menu"
      :style="{ transform: `translate(-50%, 0) translate(${pathData[1]}px, ${pathData[2] + 15}px)` }"
      @click.stop
    >
      <div class="menu-group">
        <div class="menu-group-title">基础节点</div>
        <div v-for="node in basicNodes" :key="node.type" class="menu-item" @click="handleInsert(node.type)">
          <el-icon :size="14"><component :is="node.icon" /></el-icon>
          <span>{{ node.name }}</span>
        </div>
      </div>
      <div class="menu-group">
        <div class="menu-group-title">高级节点</div>
        <div v-for="node in advancedNodes" :key="node.type" class="menu-item" @click="handleInsert(node.type)">
          <el-icon :size="14"><component :is="node.icon" /></el-icon>
          <span>{{ node.name }}</span>
        </div>
      </div>
    </div>
  </EdgeLabelRenderer>
</template>

<style lang="scss" scoped>
.edge-plus-wrapper {
  position: absolute;
  pointer-events: all;
  cursor: pointer;
  width: 24px;
  height: 24px;
  z-index: 10;
}

.edge-plus-btn {
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: #409eff;
  color: #fff;
  font-size: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.2s;
}

.edge-plus-wrapper:hover .edge-plus-btn {
  opacity: 1;
}

.edge-node-menu {
  position: absolute;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  padding: 8px;
  width: 180px;
  max-height: 320px;
  overflow-y: auto;
  z-index: 1000;
}

.menu-group-title {
  font-size: 12px;
  color: #909399;
  padding: 4px 0;
  font-weight: 600;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 8px;
  cursor: pointer;
  border-radius: 4px;
  font-size: 13px;

  &:hover {
    background: #f0f7ff;
    color: #409eff;
  }
}
</style>
```

---

## 🗃️ Store 设计

### workflowEditor.ts - 编辑器 Store

```typescript
import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { WfNode, WfEdge, NodeType } from '@/types/workflow'
import { NODE_TYPES } from '@/types/workflow'

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
  function updateNode(id: string, data: Partial<WfNode>) {
    const node = nodes.value.find(n => n.id === id)
    if (node) {
      Object.assign(node, data)
      markDirty()
    }
  }

  // 删除节点
  function removeNode(id: string) {
    const index = nodes.value.findIndex(n => n.id === id)
    if (index > -1) {
      nodes.value.splice(index, 1)
      edges.value = edges.value.filter(e => e.source !== id && e.target !== id)
      markDirty()
      saveUndo()
    }
  }

  // 添加边
  function addEdge(edge: WfEdge) {
    edges.value.push(edge)
    markDirty()
    saveUndo()
  }

  // 删除边
  function removeEdge(id: string) {
    const index = edges.value.findIndex(e => e.id === id)
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

    edges.value = edges.value.filter(e => e.id !== edgeId)
    edges.value.push(
      { id: 'e-' + Date.now() + '-1', source: edge.source, target: newNode.id, sourceHandle: edge.sourceHandle },
      { id: 'e-' + Date.now() + '-2', source: newNode.id, target: edge.target }
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

  // 保存和发布
  async function save() {
    // 调用 API 保存
    dirty.value = false
  }

  async function publish() {
    // 调用 API 发布
    status.value = 'published'
    version.value++
    dirty.value = false
  }

  // 加载流程
  async function load(workflowId: string) {
    // 调用 API 加载
    undoStack.value = []
  }

  return {
    id, name, status, version, dirty,
    nodes, edges, undoStack, selectedNodeId,
    addNode, updateNode, removeNode, addEdge, removeEdge,
    insertNodeBetween, markDirty, undo, save, publish, load
  }
})
```

---

## 📡 Mock 数据处理

### 1. 工作流列表 Mock

**文件**: `mock-server/src/data/workflow.ts`

```typescript
import { mock } from 'mockjs'

export const workflowMocks = {
  // 获取工作流列表
  'GET /api/admin/workflow/list': mock({
    code: 200,
    message: 'success',
    data: {
      list: [
        {
          id: '@guid',
          name: '请假审批流程',
          description: '员工请假申请审批',
          status: 'published',
          version: 3,
          icon: 'Calendar',
          nodeCount: 5,
          successRate: 98,
          lastRun: '@datetime',
          createdAt: '@datetime',
          updatedAt: '@datetime'
        },
        {
          id: '@guid',
          name: '报销审批流程',
          description: '费用报销审批',
          status: 'draft',
          version: 1,
          icon: 'Money',
          nodeCount: 4,
          successRate: null,
          lastRun: null,
          createdAt: '@datetime',
          updatedAt: '@datetime'
        },
        {
          id: '@guid',
          name: '采购审批流程',
          description: '物品采购申请审批',
          status: 'published',
          version: 2,
          icon: 'ShoppingCart',
          nodeCount: 6,
          successRate: 95,
          lastRun: '@datetime',
          createdAt: '@datetime',
          updatedAt: '@datetime'
        }
      ],
      total: 3
    },
    timestamp: Date.now()
  }),

  // 获取工作流详情
  'GET /api/admin/workflow/detail': (req: any) => {
    const id = req.query.id
    return mock({
      code: 200,
      message: 'success',
      data: {
        id,
        name: '请假审批流程',
        description: '员工请假申请审批',
        status: 'published',
        version: 3,
        nodes: [
          { id: 'node-1', type: 'start', name: '开始', position: { x: 100, y: 100 }, data: { rows: [] } },
          { id: 'node-2', type: 'approval', name: '部门经理审批', position: { x: 350, y: 100 }, data: { rows: [['审批人', '部门经理']] } },
          { id: 'node-3', type: 'condition', name: '请假天数判断', position: { x: 600, y: 100 }, data: { rows: [['条件', '天数 > 3']] } },
          { id: 'node-4', type: 'approval', name: 'HR审批', position: { x: 850, y: 50 }, data: { rows: [['审批人', 'HR']] } },
          { id: 'node-5', type: 'cc', name: '抄送', position: { x: 850, y: 150 }, data: { rows: [['抄送人', '直属领导']] } },
          { id: 'node-6', type: 'end', name: '结束', position: { x: 1100, y: 100 }, data: { rows: [] } }
        ],
        edges: [
          { id: 'e-1', source: 'node-1', target: 'node-2' },
          { id: 'e-2', source: 'node-2', target: 'node-3' },
          { id: 'e-3', source: 'node-3', target: 'node-4', sourceHandle: 'yes', label: '是' },
          { id: 'e-4', source: 'node-3', target: 'node-5', sourceHandle: 'no', label: '否' },
          { id: 'e-5', source: 'node-4', target: 'node-6' },
          { id: 'e-6', source: 'node-5', target: 'node-6' }
        ]
      },
      timestamp: Date.now()
    })
  },

  // 创建工作流
  'POST /api/admin/workflow/create': mock({
    code: 200,
    message: 'success',
    data: {
      id: '@guid',
      name: '新流程',
      status: 'draft',
      version: 1,
      nodes: [{ id: 'node-start', type: 'start', name: '开始', position: { x: 100, y: 100 }, data: { rows: [] } }],
      edges: [],
      createdAt: '@datetime',
      updatedAt: '@datetime'
    },
    timestamp: Date.now()
  }),

  // 更新工作流
  'POST /api/admin/workflow/update': {
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  },

  // 发布工作流
  'POST /api/admin/workflow/publish': mock({
    code: 200,
    message: 'success',
    data: {
      status: 'published',
      version: '@integer(1, 10)'
    },
    timestamp: Date.now()
  }),

  // 删除工作流
  'POST /api/admin/workflow/delete': {
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  },

  // 验证工作流
  'POST /api/admin/workflow/validate': mock({
    code: 200,
    message: 'success',
    data: {
      valid: '@boolean',
      errors: []
    },
    timestamp: Date.now()
  }),

  // 获取执行历史
  'GET /api/admin/workflow/executions': mock({
    code: 200,
    message: 'success',
    data: {
      list: [
        {
          id: '@guid',
          workflowId: '@guid',
          workflowName: '请假审批流程',
          status: 'success',
          trigger: 'manual',
          startTime: '@datetime',
          duration: '@integer(1000, 60000)',
          nodeProgress: '5/5'
        }
      ],
      total: 1
    },
    timestamp: Date.now()
  })
}
```

### 2. Mock Server 路由注册

**文件**: `mock-server/src/routes/workflow.ts`

```typescript
import { Router } from 'express'
import { workflowMocks } from '../data/workflow'
import { createMockHandler } from '../utils/mockHandler'

const router = Router()

// 注册所有工作流路由
Object.entries(workflowMocks).forEach(([key, handler]) => {
  const [method, path] = key.split(' ')
  const routePath = path.replace('/api/admin/workflow', '')
  
  if (method === 'GET') {
    router.get(routePath, createMockHandler(handler))
  } else if (method === 'POST') {
    router.post(routePath, createMockHandler(handler))
  } else if (method === 'PUT') {
    router.put(routePath, createMockHandler(handler))
  } else if (method === 'DELETE') {
    router.delete(routePath, createMockHandler(handler))
  }
})

export default router
```

---

## 📊 迁移时间表

| 阶段 | 任务 | 预估时间 |
|------|------|---------|
| 1 | 安装依赖、创建类型定义 | 0.5天 |
| 2 | 实现 Store (workflowEditor, workflowExecution) | 1天 |
| 3 | 实现 WorkflowCanvas.vue | 1天 |
| 4 | 实现 BaseNodeCard.vue | 0.5天 |
| 5 | 实现 AddButtonEdge.vue | 0.5天 |
| 6 | 重构 designer/index.vue | 1天 |
| 7 | 实现 NodeConfigModal 等辅助组件 | 1天 |
| 8 | Mock 数据处理 | 0.5天 |
| 9 | API 层实现 | 0.5天 |
| 10 | 测试与优化 | 1天 |
| **总计** | | **~7.5天** |

---

## 🔧 快速开始

### 1. 安装依赖
```bash
cd EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap
```

### 2. 创建类型定义
```bash
# src/types/workflow.ts
# 复制上面的类型定义
```

### 3. 创建 Store
```bash
# src/stores/workflowEditor.ts
# src/stores/workflowExecution.ts
# 复制上面的 Store 代码
```

### 4. 创建组件
```bash
# 复制上面的组件代码到对应文件
```

### 5. 启动测试
```bash
# 启动 Mock 服务器
cd mock-server && pnpm dev

# 启动前端
cd EasyProduct.Admin && pnpm dev
```

---

## ✅ 检查清单

- [ ] @vue-flow/core 安装完成
- [ ] 类型定义创建完成
- [ ] workflowEditor Store 实现
- [ ] WorkflowCanvas 组件实现
- [ ] BaseNodeCard 组件实现
- [ ] AddButtonEdge 组件实现
- [ ] designer/index.vue 重构
- [ ] Mock 数据配置
- [ ] API 层实现
- [ ] i18n 翻译添加
- [ ] 撤销/重做功能测试
- [ ] 边插入节点功能测试
