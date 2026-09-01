<script setup lang="ts">
import { ref, watch, markRaw } from 'vue'
import { VueFlow, useVueFlow, type Node, type Edge } from '@vue-flow/core'
import { Background } from '@vue-flow/background'
import { Controls } from '@vue-flow/controls'
import { MiniMap } from '@vue-flow/minimap'
import { useWorkflowEditorStore } from '@/stores/workflowEditor'
import BaseNodeCard from './BaseNodeCard.vue'
import AddButtonEdge from './AddButtonEdge.vue'
import type { WfEdge, NodeType } from '@/types/workflow'
import { NODE_TYPES } from '@/types/workflow'

import '@vue-flow/core/dist/style.css'
import '@vue-flow/core/dist/theme-default.css'
import '@vue-flow/controls/dist/style.css'
import '@vue-flow/minimap/dist/style.css'

const editorStore = useWorkflowEditorStore()
const edgeTypes = {
  default: markRaw(AddButtonEdge)
}

const { onConnect, onNodeDragStop, onNodeDoubleClick, onNodesChange } = useVueFlow()

const nodes = ref<Node[]>([])
const edges = ref<Edge[]>([])

// 同步 Store 数据�?VueFlow
watch(
  () => editorStore.nodes,
  (storeNodes) => {
    nodes.value = storeNodes.map(n => ({
      id: n.id,
      type: n.type,
      position: n.position,
      data: {
        ...n.data,
        name: n.name
      }
    }))
  },
  { immediate: true, deep: true }
)

watch(
  () => editorStore.edges,
  (storeEdges) => {
    edges.value = storeEdges.map(e => ({
      id: e.id,
      source: e.source,
      target: e.target,
      label: e.label,
      sourceHandle: e.sourceHandle
    }))
  },
  { immediate: true, deep: true }
)

// 连线事件
onConnect((params) => {
  const newEdge: WfEdge = {
    id: 'e-' + Date.now(),
    source: params.source,
    target: params.target,
    sourceHandle: params.sourceHandle as 'yes' | 'no' | undefined
  }
  editorStore.addEdge(newEdge)
})

// 节点拖拽停止
onNodeDragStop((event) => {
  const node = event.node
  editorStore.updateNode(node.id, {
    position: { x: node.position.x, y: node.position.y }
  })
})

// 节点双击
onNodeDoubleClick((event) => {
  editorStore.selectedNodeId = event.node.id
})

// 节点变化（删除）
onNodesChange((changes) => {
  changes.forEach(change => {
    if (change.type === 'remove') {
      editorStore.removeNode(change.id)
    }
  })
})

// 拖拽添加节点
function handleDrop(event: DragEvent) {
  const nodeType = event.dataTransfer?.getData('nodeType')
  if (!nodeType) return

  const bounds = (event.currentTarget as HTMLElement).getBoundingClientRect()
  const position = {
    x: event.clientX - bounds.left,
    y: event.clientY - bounds.top
  }

  editorStore.addNode(nodeType, position)
}

function handleDragOver(event: DragEvent) {
  event.preventDefault()
  if (event.dataTransfer) {
    event.dataTransfer.dropEffect = 'move'
  }
}
</script>

<template>
  <div
    class="workflow-canvas"
    @drop="handleDrop"
    @dragover="handleDragOver"
  >
    <VueFlow
      v-model:nodes="nodes"
      v-model:edges="edges"
      :default-zoom="0.7"
      :min-zoom="0.2"
      :max-zoom="4"
      :delete-key-code="['Backspace', 'Delete']"
      :edge-types="edgeTypes"
      fit-view-on-init
      class="vue-flow-canvas"
    >
      <Background
        pattern-gap="20"
        :size="1"
      />
      <Controls />
      <MiniMap />

      <!-- 节点模板注册 -->
      <template
        v-for="type in NODE_TYPES.map(n => n.type)"
        :key="type"
        #[`node-${type}`]="nodeProps"
      >
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
  width: 100px;
  min-width: 80px;
  max-width: 120px;
  padding: 0;
  border: none;
  background: transparent;
}

:deep(.vue-flow__edge-path) {
  stroke: #91d5ff;
  stroke-width: 2;
}

:deep(.vue-flow__edge.selected .vue-flow__edge-path) {
  stroke: #409eff;
  stroke-width: 3;
}
</style>
