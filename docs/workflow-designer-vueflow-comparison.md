# EasyProduct 工作流设计器升级方案 - VueFlow 对比分析

## 一、方案概述

本文档对比当前工作流设计器与 EasyRag VueFlow 设计方案的差异，提供基于 **VueFlow** 的升级建议。

> **注意**: 不同于之前的 AntV X6 方案，本方案采用 **VueFlow** (@vue-flow/core) - 专为 Vue 设计的流程图引擎。

---

## 二、技术栈对比

| 维度 | 当前设计 | EasyRag (VueFlow) |
|------|----------|-------------------|
| **核心技术** | 纯原生 SVG+DOM | VueFlow (@vue-flow/core) |
| **Vue 集成** | 手动 DOM 操作 | Vue 原生响应式绑定 |
| **节点定义** | 原生 HTML + CSS | Vue 组件插槽 |
| **连线方式** | SVG Path 手动计算 | 自动贝塞尔曲线 |
| **拖拽交互** | 原生鼠标事件 | 内置拖拽支持 |
| **状态管理** | Ref + 手动同步 | Pinia + 双向绑定 |

---

## 三、功能差异对比

### 3.1 节点系统

| 功能 | 当前设计 | EasyRag | 差距 |
|------|----------|---------|------|
| **节点类型** | 5种 | 13种 | 🔴 缺失 8种 |
| **节点分组** | ❌ 无 | ✅ 基础+能力 | 🟡 需增加 |
| **节点颜色** | 单一 | 类型专属配色 | 🟢 易实现 |
| **节点预览** | ❌ 无 | ✅ 配置摘要行 | 🟡 需增加 |
| **节点配置** | 简单表单 | 抽屉式配置 | 🟡 体验差距 |

**EasyRag 节点类型 (13种)**:
```typescript
基础节点: start, end, condition, loop, loop_end, human, variable_assign, template_render
能力节点: llm, rag, code, http, tool
```

### 3.2 画布交互

| 功能 | 当前设计 | EasyRag | 差距 |
|------|----------|---------|------|
| **缩放控制** | ❌ 无 | ✅ Controls 组件 | 🟡 需增加 |
| **背景网格** | ✅ 简单网格 | ✅ Background 组件 | 🟢 相当 |
| **迷你地图** | ❌ 无 | ✅ MiniMap 组件 | 🟡 需增加 |
| **拖拽添加** | ✅ 点击添加 | ✅ 拖拽添加 | 🟢 相当 |
| **画布平移** | ❌ 无 | ✅ 内置支持 | 🟡 需增加 |
| **连线方式** | 两阶段点击 | 拖拽连接 | 🔴 交互差异 |
| **连接端口** | ❌ 节点级 | ✅ Handle 端口 | 🟡 需优化 |

### 3.3 执行调试

| 功能 | 当前设计 | EasyRag | 差距 |
|------|----------|---------|------|
| **执行状态** | ❌ 无 | ✅ 节点级状态 | 🔴 需增加 |
| **执行日志** | ❌ 无 | ✅ 实时日志面板 | 🔴 需增加 |
| **调试模式** | ❌ 无 | ✅ 断点单步 | 🔴 需增加 |
| **执行历史** | ❌ 无 | ✅ 历史记录 | 🟡 可选 |

### 3.4 工具栏

| 功能 | 当前设计 | EasyRag | 差距 |
|------|----------|---------|------|
| **撤销/重做** | ❌ 无 | ✅ 历史栈 | 🔴 需增加 |
| **自动布局** | ❌ 无 | ✅ 水平排列 | 🟡 需增加 |
| **执行按钮** | ❌ 无 | ✅ 执行/调试 | 🔴 需增加 |
| **保存/发布** | ✅ 有 | ✅ 有 | 🟢 相当 |

---

## 四、架构对比

### 4.1 当前架构

```
designer/index.vue
├── 原生 SVG 画布
├── 节点渲染 (v-for + DOM)
├── 连线计算 (手动 Path)
└── 属性面板 (简单表单)
```

### 4.2 EasyRag 架构

```
WorkflowEditorView.vue (页面)
├── WorkflowCanvas.vue (画布)
│   ├── VueFlow (核心组件)
│   ├── Background (背景)
│   ├── Controls (缩放控制)
│   ├── MiniMap (迷你地图)
│   └── BaseNodeCard.vue (节点组件)
├── NodeConfigModal.vue (配置抽屉)
├── ExecutionPanel.vue (执行面板)
└── DebugToolbar.vue (调试工具栏)

stores/workflow.ts (Pinia Store)
├── useWorkflowEditorStore (编辑器状态)
├── useWorkflowExecutionStore (执行状态)
└── useWorkflowListStore (列表状态)
```

---

## 五、VueFlow 核心特性

### 5.1 节点定义方式

**当前设计**:
```vue
<div
  v-for="node in graph.nodes"
  :key="node.id"
  class="canvas-node"
  :style="{ left: node.x + 'px', top: node.y + 'px' }"
  @mousedown.stop="startDrag(node, $event)"
>
  <!-- 节点内容 -->
</div>
```

**VueFlow 方式**:
```vue
<VueFlow v-model:nodes="nodes" v-model:edges="edges">
  <template #node-start="nodeProps">
    <BaseNodeCard :node="nodeProps" />
  </template>
  <!-- 每种类型一个插槽 -->
</VueFlow>
```

### 5.2 连线方式

**当前设计**: 手动 SVG Path 计算
```typescript
const edgePath = (edge: FlowEdge): string => {
  const src = graph.nodes.find(n => n.id === edge.source)
  const tgt = graph.nodes.find(n => n.id === edge.target)
  const x1 = src.x + 100, y1 = src.y + 30
  const x2 = tgt.x, y2 = tgt.y + 30
  const midX = (x1 + x2) / 2
  return `M ${x1},${y1} C ${midX},${y1} ${midX},${y2} ${x2},${y2}`
}
```

**VueFlow 方式**: 自动处理
```vue
<VueFlow>
  <!-- 自动计算连线路径 -->
</VueFlow>
```

### 5.3 连接端口

**当前设计**: 节点级连接
```vue
<div class="canvas-node__connect" @click.stop="startConnect(node.id)">
  <el-icon><Connection /></el-icon>
</div>
```

**VueFlow 方式**: Handle 组件
```vue
<template>
  <Handle type="target" :position="Position.Left" />
  <div class="node-content">...</div>
  <Handle type="source" :position="Position.Right" />
</template>
```

---

## 六、升级方案

### 6.1 依赖安装

```bash
cd EasyProduct.Admin
pnpm add @vue-flow/core @vue-flow/background @vue-flow/controls @vue-flow/minimap
```

### 6.2 文件结构改造

```
src/views/workflow/designer/
├── index.vue                 # 简化主入口
├── components/
│   ├── WorkflowCanvas.vue    # VueFlow 画布 (新增)
│   ├── NodePalette.vue       # 节点库 (改造)
│   ├── PropertyPanel.vue     # 属性面板 (改造)
│   ├── ExecutionPanel.vue    # 执行面板 (新增)
│   ├── DebugToolbar.vue      # 调试工具栏 (新增)
│   └── BaseNodeCard.vue      # 节点卡片 (新增)
└── stores/
    └── workflowEditor.ts     # Pinia Store (扩展)
```

### 6.3 核心组件设计

#### WorkflowCanvas.vue

```vue
<script setup lang="ts">
import { VueFlow, useVueFlow, type Node, type Edge } from '@vue-flow/core'
import { Background } from '@vue-flow/background'
import { Controls } from '@vue-flow/controls'
import { MiniMap } from '@vue-flow/minimap'
import BaseNodeCard from './BaseNodeCard.vue'
import { useWorkflowEditorStore } from '@/stores/workflow'

const store = useWorkflowEditorStore()
const { onConnect, onNodeDragStop } = useVueFlow()

// 自动同步到 store
onConnect((params) => {
  store.addEdge({
    id: `e-${Date.now()}`,
    source: params.source,
    target: params.target
  })
})

onNodeDragStop(({ node }) => {
  store.updateNode(node.id, {
    position: { x: node.position.x, y: node.position.y }
  })
})
</script>

<template>
  <div class="workflow-canvas">
    <VueFlow
      v-model:nodes="store.nodes"
      v-model:edges="store.edges"
      :default-zoom="1"
      :min-zoom="0.2"
      :max-zoom="4"
      fit-view-on-init
    >
      <Background pattern-gap="20" />
      <Controls />
      <MiniMap />
      
      <!-- 节点类型插槽 -->
      <template #node-start="props">
        <BaseNodeCard :node="props" />
      </template>
      <template #node-approval="props">
        <BaseNodeCard :node="props" />
      </template>
      <!-- 更多节点类型... -->
    </VueFlow>
  </div>
</template>
```

#### BaseNodeCard.vue

```vue
<script setup lang="ts">
import { Handle, Position, type NodeProps } from '@vue-flow/core'
import { computed } from 'vue'
import { useWorkflowEditorStore } from '@/stores/workflow'

const props = defineProps<{
  node: NodeProps
}>()

const store = useWorkflowEditorStore()

const nodeConfig = {
  start: { color: '#334155', bg: '#f1f5f9', icon: 'VideoPlay' },
  approval: { color: '#fa8c16', bg: '#fff7e6', icon: 'UserFilled' },
  condition: { color: '#CA8A04', bg: '#fefce8', icon: 'Share' },
  // ...
}

const config = computed(() => nodeConfig[props.node.type] || nodeConfig.start)

function openConfig() {
  store.selectedNodeId = props.node.id
}
</script>

<template>
  <div
    class="base-node-card"
    :style="{ borderColor: config.color, backgroundColor: config.bg }"
  >
    <!-- 设置按钮 -->
    <div class="settings-btn" @click.stop="openConfig">
      <el-icon><Setting /></el-icon>
    </div>

    <!-- 节点头部 -->
    <div class="node-header">
      <el-icon :style="{ color: config.color }">
        <component :is="config.icon" />
      </el-icon>
      <span class="node-name">{{ node.data?.name || node.type }}</span>
    </div>

    <!-- 输入连接点 -->
    <Handle
      v-if="node.type !== 'start'"
      type="target"
      :position="Position.Left"
    />

    <!-- 输出连接点 -->
    <template v-if="node.type === 'condition'">
      <Handle id="yes" type="source" :position="Position.Right" />
      <Handle id="no" type="source" :position="Position.Right" />
    </template>
    <Handle
      v-else-if="node.type !== 'end'"
      type="source"
      :position="Position.Right"
    />
  </div>
</template>
```

### 6.4 Store 扩展

```typescript
// stores/workflow.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { WfNode, WfEdge } from '@/types/workflow'

export const useWorkflowEditorStore = defineStore('workflowEditor', () => {
  // ========== 基础状态 ==========
  const nodes = ref<WfNode[]>([])
  const edges = ref<WfEdge[]>([])
  const selectedNodeId = ref('')
  
  // ========== 历史记录 ==========
  const undoStack = ref<string[]>([])
  const canUndo = computed(() => undoStack.value.length > 0)
  
  // ========== Actions ==========
  function addNode(type: string, position: { x: number; y: number }) {
    const node: WfNode = {
      id: `node-${Date.now()}`,
      type,
      name: getNodeName(type),
      position,
      data: { rows: [] }
    }
    nodes.value.push(node)
    saveUndo()
    return node
  }
  
  function updateNode(id: string, data: Partial<WfNode>) {
    const node = nodes.value.find(n => n.id === id)
    if (node) {
      Object.assign(node, data)
    }
  }
  
  function addEdge(edge: WfEdge) {
    edges.value.push(edge)
    saveUndo()
  }
  
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
  }
  
  return {
    nodes, edges, selectedNodeId, undoStack, canUndo,
    addNode, updateNode, addEdge, undo
  }
})
```

---

## 七、节点类型映射

| 当前类型 | EasyRag 类型 | 说明 |
|----------|-------------|------|
| start | start | ✅ 相同 |
| approval | human | 🟡 需适配 |
| condition | condition | ✅ 相同 |
| cc | - | 🟡 需新增 copyer |
| end | end | ✅ 相同 |
| - | variable_assign | 🔵 可选新增 |
| - | template_render | 🔵 可选新增 |
| - | llm | 🔵 可选新增 |
| - | rag | 🔵 可选新增 |
| - | code | 🔵 可选新增 |
| - | http | 🔵 可选新增 |
| - | tool | 🔵 可选新增 |

---

## 八、迁移路线图

### 阶段1: VueFlow 基础 (3天)
- [ ] 安装依赖
- [ ] 改造 WorkflowCanvas.vue
- [ ] 创建 BaseNodeCard.vue
- [ ] 基础节点渲染

### 阶段2: 节点系统 (2天)
- [ ] 扩展 Store (撤销/重做)
- [ ] 改造 NodePalette.vue
- [ ] 拖拽添加节点
- [ ] 节点分组展示

### 阶段3: 交互完善 (2天)
- [ ] 端口连接 (Handle)
- [ ] 属性面板改造
- [ ] 自动布局
- [ ] 缩放控制

### 阶段4: 可选增强 (2天)
- [ ] 执行状态显示
- [ ] 执行日志面板
- [ ] 调试模式

**总计: ~9天**

---

## 九、VueFlow vs AntV X6 对比

| 特性 | VueFlow | AntV X6 |
|------|---------|---------|
| **Vue 集成** | ⭐⭐⭐ 原生 | ⭐⭐ 适配层 |
| **包体积** | 小 (~50KB) | 大 (~200KB) |
| **功能丰富度** | 中等 | 高 |
| **学习成本** | 低 | 高 |
| **定制化** | 高 (Vue 插槽) | 中等 |
| **社区** | 活跃 | 非常活跃 |
| **中文文档** | 有 | 有 |

**推荐场景**:
- **VueFlow**: Vue 项目、快速开发、轻量级需求
- **AntV X6**: 复杂图编辑、企业级应用、需要高级功能

---

## 十、参考资源

- **VueFlow 文档**: https://vueflow.dev/
- **EasyRag 源码**:
  - `D:\4-MyProject\EasyRag\frontend\src\views\workflow\WorkflowEditorView.vue`
  - `D:\4-MyProject\EasyRag\frontend\src\views\workflow\components\WorkflowCanvas.vue`
  - `D:\4-MyProject\EasyRag\frontend\src\views\workflow\components\BaseNodeCard.vue`
  - `D:\4-MyProject\EasyRag\frontend\src\types\workflow.ts`
  - `D:\4-MyProject\EasyRag\frontend\src\stores\workflow.ts`
