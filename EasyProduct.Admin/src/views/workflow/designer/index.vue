<template>
  <div class="designer-page">
    <el-card
      shadow="never"
      class="designer-page__toolbar"
    >
      <div class="toolbar-left">
        <el-button
          :icon="ArrowLeft"
          @click="goBack"
        >
          {{ t('workflow.designer.back') }}
        </el-button>
        <span class="toolbar-title">{{ graph.definitionName || t('workflow.designer.untitled') }}</span>
      </div>
      <div class="toolbar-right">
        <el-button
          :icon="CircleCheck"
          type="success"
          @click="handleValidate"
        >
          {{ t('workflow.designer.validate') }}
        </el-button>
        <el-button
          :icon="Check"
          type="primary"
          @click="handleSave"
        >
          {{ t('workflow.designer.save') }}
        </el-button>
      </div>
    </el-card>
    <div class="designer-page__body">
      <div class="designer-page__palette">
        <div class="palette-title">
          {{ t('workflow.designer.palette') }}
        </div>
        <div
          v-for="item in FLOW_NODE_TYPE_OPTIONS"
          :key="item.value"
          class="palette-item"
          :class="`palette-item--${item.value}`"
          @click="addNode(item.value)"
        >
          <el-icon><component :is="iconMap[item.icon]" /></el-icon>
          <span>{{ t(item.labelKey) }}</span>
        </div>
      </div>
      <div
        ref="canvasRef"
        class="designer-page__canvas"
        @click="clearSelection"
      >
        <svg class="canvas-svg">
          <defs>
            <marker
              id="arrowhead"
              markerWidth="10"
              markerHeight="7"
              refX="9"
              refY="3.5"
              orient="auto"
            >
              <polygon
                points="0 0, 10 3.5, 0 7"
                fill="#909399"
              />
            </marker>
          </defs>
          <path
            v-for="edge in graph.edges"
            :key="edge.id"
            :d="edgePath(edge)"
            class="canvas-edge"
            :class="{ 'canvas-edge--selected': selectedEdgeId === edge.id }"
            stroke="#909399"
            stroke-width="2"
            fill="none"
            marker-end="url(#arrowhead)"
            @click.stop="selectEdge(edge.id)"
          />
        </svg>
        <div
          v-for="node in graph.nodes"
          :key="node.id"
          class="canvas-node"
          :class="[`canvas-node--${node.type}`, { 'canvas-node--selected': selectedNodeId === node.id, 'canvas-node--connect': connectSource === node.id }]"
          :style="{ left: node.x + 'px', top: node.y + 'px' }"
          @mousedown.stop="startDrag(node, $event)"
          @click.stop="selectNode(node.id)"
        >
          <div class="canvas-node__header">
            <el-icon class="canvas-node__icon">
              <component :is="iconMap[iconForType(node.type)]" />
            </el-icon>
            <span class="canvas-node__name">{{ node.name }}</span>
          </div>
          <div
            v-if="node.type !== 'start' && node.type !== 'end'"
            class="canvas-node__body"
          >
            <span
              v-if="node.assigneeName"
              class="canvas-node__assignee"
            >{{ node.assigneeName }}</span>
            <span
              v-else
              class="canvas-node__assignee canvas-node__assignee--empty"
            >{{ t('workflow.designer.noAssignee') }}</span>
          </div>
          <div
            class="canvas-node__connect"
            @click.stop="startConnect(node.id)"
          >
            <el-icon><Connection /></el-icon>
          </div>
          <el-icon
            class="canvas-node__delete"
            @click.stop="deleteNode(node.id)"
          >
            <Close />
          </el-icon>
        </div>
        <div
          v-if="connectSource"
          class="canvas-connect-hint"
        >
          {{ t('workflow.designer.connectHint') }}
          <el-button
            size="small"
            @click.stop="cancelConnect"
          >
            {{ t('common.cancel') }}
          </el-button>
        </div>
      </div>
      <div class="designer-page__props">
        <div class="props-title">
          {{ t('workflow.designer.properties') }}
        </div>
        <el-form
          v-if="selectedNode"
          label-width="80px"
          size="small"
        >
          <el-form-item :label="t('workflow.designer.propType')">
            <el-tag size="small">
              {{ t(typeLabelKey(selectedNode.type)) }}
            </el-tag>
          </el-form-item>
          <el-form-item :label="t('workflow.designer.propName')">
            <el-input v-model="selectedNode.name" />
          </el-form-item>
          <template v-if="selectedNode.type === 'approval' || selectedNode.type === 'cc'">
            <el-form-item :label="t('workflow.designer.propAssigneeType')">
              <el-select
                v-model="selectedNode.assigneeType"
                :placeholder="t('workflow.designer.assigneeTypePlaceholder')"
              >
                <el-option
                  v-for="opt in ASSIGNEE_TYPE_OPTIONS"
                  :key="opt.value"
                  :label="t(opt.labelKey)"
                  :value="opt.value"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="t('workflow.designer.propAssigneeName')">
              <el-input
                v-model="selectedNode.assigneeName"
                :placeholder="t('workflow.designer.assigneeNamePlaceholder')"
              />
            </el-form-item>
          </template>
          <el-form-item :label="t('workflow.designer.propPosition')">
            <el-input-number
              v-model="selectedNode.x"
              :min="0"
              :step="10"
              size="small"
            />
            <el-input-number
              v-model="selectedNode.y"
              :min="0"
              :step="10"
              size="small"
              class="ml-1"
            />
          </el-form-item>
          <el-form-item>
            <el-button
              type="danger"
              size="small"
              @click="deleteNode(selectedNode.id)"
            >
              {{ t('common.delete') }}
            </el-button>
          </el-form-item>
        </el-form>
        <div
          v-else
          class="props-empty"
        >
          {{ t('workflow.designer.selectNode') }}
        </div>
        <el-divider />
        <div class="props-title">
          {{ t('workflow.designer.edges') }}
        </div>
        <div class="props-edges">
          <div
            v-for="edge in graph.edges"
            :key="edge.id"
            class="props-edge"
            :class="{ 'props-edge--selected': selectedEdgeId === edge.id }"
            @click="selectEdge(edge.id)"
          >
            <span class="props-edge__label">{{ nodeNameById(edge.source) }} -&gt; {{ nodeNameById(edge.target) }}</span>
            <el-input
              v-if="selectedEdgeId === edge.id"
              v-model="edge.label"
              size="small"
              :placeholder="t('workflow.designer.edgeLabelPlaceholder')"
              class="props-edge__input"
            />
            <el-icon
              class="props-edge__delete"
              @click.stop="deleteEdge(edge.id)"
            >
              <Delete />
            </el-icon>
          </div>
          <div
            v-if="graph.edges.length === 0"
            class="props-empty"
          >
            {{ t('workflow.designer.noEdges') }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { ArrowLeft, CircleCheck, Check, Close, Delete, Connection, Position, UserFilled, Switch, Message, CircleClose } from '@element-plus/icons-vue'
import { getFlowGraph, saveFlowGraph, validateFlowGraph } from '@/api/workflow/designer'
import { FLOW_NODE_TYPE_OPTIONS, ASSIGNEE_TYPE_OPTIONS, type FlowNode, type FlowEdge, type FlowGraph, type FlowNodeType } from '@/types/workflow'

const { t } = useI18n()
const router = useRouter()

const iconMap: Record<string, unknown> = { Position, UserFilled, Switch, Message, CircleClose, Connection, Close, Delete, ArrowLeft, Check, CircleCheck }

const canvasRef = ref<HTMLElement | null>(null)
const graph = reactive<FlowGraph>({ definitionId: '', definitionName: '', nodes: [], edges: [] })
const selectedNodeId = ref<string | null>(null)
const selectedEdgeId = ref<string | null>(null)
const connectSource = ref<string | null>(null)

let dragNode: FlowNode | null = null
let dragOffsetX = 0
let dragOffsetY = 0

const selectedNode = computed<FlowNode | null>(() => {
  if (!selectedNodeId.value) return null
  return graph.nodes.find(n => n.id === selectedNodeId.value) ?? null
})

const iconForType = (type: FlowNodeType): string => {
  const opt = FLOW_NODE_TYPE_OPTIONS.find(o => o.value === type)
  return opt?.icon ?? 'Position'
}

const typeLabelKey = (type: FlowNodeType): string => {
  const opt = FLOW_NODE_TYPE_OPTIONS.find(o => o.value === type)
  return opt?.labelKey ?? ''
}

const nodeNameById = (id: string): string => {
  const node = graph.nodes.find(n => n.id === id)
  return node?.name ?? '?'
}

let nodeCounter = 0
const addNode = (type: FlowNodeType) => {
  nodeCounter++
  const names: Record<FlowNodeType, string> = {
    start: t('workflow.designer.nodeStart'),
    approval: t('workflow.designer.nodeApproval'),
    condition: t('workflow.designer.nodeCondition'),
    cc: t('workflow.designer.nodeCc'),
    end: t('workflow.designer.nodeEnd'),
  }
  const node: FlowNode = {
    id: `node-${Date.now()}-${nodeCounter}`,
    type,
    name: `${names[type]} ${nodeCounter}`,
    x: 100 + (graph.nodes.length % 4) * 240,
    y: 100 + Math.floor(graph.nodes.length / 4) * 160,
  }
  if (type === 'approval' || type === 'cc') {
    node.assigneeType = 'user'
  }
  graph.nodes.push(node)
  selectedNodeId.value = node.id
  selectedEdgeId.value = null
}

const deleteNode = (id: string) => {
 const idx = graph.nodes.findIndex(n => n.id === id)
 if (idx === -1) return
 graph.nodes.splice(idx, 1)
 graph.edges = graph.edges.filter(e => e.source !== id && e.target !== id)
 if (selectedNodeId.value === id) selectedNodeId.value = null
}

const selectNode = (id: string) => {
 if (connectSource.value && connectSource.value !== id) {
   const newEdge: FlowEdge = {
     id: `edge-${Date.now()}`,
     source: connectSource.value,
     target: id,
   }
   if (!graph.edges.some(e => e.source === newEdge.source && e.target === newEdge.target)) {
     graph.edges.push(newEdge)
   }
   connectSource.value = null
   return
 }
 if (connectSource.value === id) {
   connectSource.value = null
   return
 }
 selectedNodeId.value = id
 selectedEdgeId.value = null
}

const selectEdge = (id: string) => {
 selectedEdgeId.value = id
 selectedNodeId.value = null
}

const clearSelection = () => {
 selectedNodeId.value = null
 selectedEdgeId.value = null
}

const deleteEdge = (id: string) => {
 const idx = graph.edges.findIndex(e => e.id === id)
 if (idx === -1) return
 graph.edges.splice(idx, 1)
 if (selectedEdgeId.value === id) selectedEdgeId.value = null
}

const startDrag = (node: FlowNode, event: MouseEvent) => {
 dragNode = node
 const canvasRect = canvasRef.value?.getBoundingClientRect()
 dragOffsetX = event.clientX - (canvasRect?.left ?? 0) - node.x
 dragOffsetY = event.clientY - (canvasRect?.top ?? 0) - node.y
}

const onDragMove = (event: MouseEvent) => {
 if (!dragNode || !canvasRef.value) return
 const canvasRect = canvasRef.value.getBoundingClientRect()
 dragNode.x = Math.max(0, Math.round(event.clientX - canvasRect.left - dragOffsetX))
 dragNode.y = Math.max(0, Math.round(event.clientY - canvasRect.top - dragOffsetY))
}

const onDragEnd = () => {
 dragNode = null
}

const edgePath = (edge: FlowEdge): string => {
 const src = graph.nodes.find(n => n.id === edge.source)
 const tgt = graph.nodes.find(n => n.id === edge.target)
 if (!src || !tgt) return ''
 const x1 = src.x + 100
 const y1 = src.y + 30
 const x2 = tgt.x
 const y2 = tgt.y + 30
 const midX = (x1 + x2) / 2
 return `M ${x1},${y1} C ${midX},${y1} ${midX},${y2} ${x2},${y2}`
}

const startConnect = (id: string) => {
 connectSource.value = id
 selectedNodeId.value = id
}

const cancelConnect = () => {
 connectSource.value = null
}

const handleSave = async () => {
 try {
 await saveFlowGraph(graph)
 ElMessage.success(t('workflow.designer.saveSuccess'))
 } catch { /* handled by interceptor */ }
}

const handleValidate = async () => {
 try {
 await validateFlowGraph(graph)
 ElMessage.success(t('workflow.designer.validateSuccess'))
 } catch { /* handled by interceptor */ }
}

const goBack = () => router.push('/workflow/publish')

const loadGraph = async () => {
 try {
 const data = await getFlowGraph()
 Object.assign(graph, data)
 } catch { /* handled by interceptor */ }
}

onMounted(() => {
 loadGraph()
 window.addEventListener('mousemove', onDragMove)
 window.addEventListener('mouseup', onDragEnd)
})

onUnmounted(() => {
 window.removeEventListener('mousemove', onDragMove)
 window.removeEventListener('mouseup', onDragEnd)
})
</script>

<style scoped lang="scss">
.designer-page {
  display: flex;
  flex-direction: column;
  height: 100%;

  &__toolbar {
    margin-bottom: $spacing-sm;

    .el-card__body {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: $spacing-xs $spacing-md;
    }
  }

  &__body {
    display: flex;
    flex: 1;
    gap: $spacing-sm;
    min-height: 0;
  }

  &__palette {
    width: 160px;
    background: var(--ep-bg-color);
    border: 1px solid var(--ep-border-color);
    border-radius: 4px;
    padding: $spacing-sm;
    flex-shrink: 0;
    overflow-y: auto;

    .palette-title {
      font-weight: 600;
      margin-bottom: $spacing-sm;
      color: var(--ep-text-primary);
    }

    .palette-item {
      display: flex;
      align-items: center;
      gap: $spacing-xs;
      padding: $spacing-xs $spacing-sm;
      margin-bottom: $spacing-xs;
      border: 1px dashed var(--ep-border-color);
      border-radius: 4px;
      cursor: pointer;
      transition: background 0.2s;

      &:hover {
        background: var(--ep-fill-color-light);
      }

      &--start { border-color: var(--ep-color-success); }
      &--approval { border-color: var(--ep-color-primary); }
      &--condition { border-color: var(--ep-color-warning); }
      &--cc { border-color: var(--ep-color-info); }
      &--end { border-color: var(--ep-color-danger); }
    }
  }

  &__canvas {
    flex: 1;
    position: relative;
    background: var(--ep-fill-color-light);
    border: 1px solid var(--ep-border-color);
    border-radius: 4px;
    overflow: auto;
    min-width: 800px;
    min-height: 500px;
    background-image: radial-gradient(var(--ep-border-color) 1px, transparent 1px);
    background-size: 20px 20px;

    .canvas-svg {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      pointer-events: none;
      z-index: 1;
    }

    .canvas-edge {
      pointer-events: stroke;
      cursor: pointer;

      &:hover { stroke: var(--ep-color-primary); }
      &--selected { stroke: var(--ep-color-primary); stroke-width: 3; }
    }

    .canvas-node {
      position: absolute;
      width: 200px;
      min-height: 60px;
      background: var(--ep-bg-color);
      border: 2px solid var(--ep-border-color);
      border-radius: 6px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
      cursor: move;
      user-select: none;
      z-index: 2;
      display: flex;
      flex-direction: column;

      &--start { border-color: var(--ep-color-success); border-radius: 30px; }
      &--approval { border-color: var(--ep-color-primary); }
      &--condition { border-color: var(--ep-color-warning); transform: rotate(0deg); border-radius: 4px; }
      &--cc { border-color: var(--ep-color-info); border-style: dashed; }
      &--end { border-color: var(--ep-color-danger); border-radius: 30px; }
      &--selected { border-width: 3px; box-shadow: 0 0 0 2px var(--ep-color-primary-light-5); }
      &--connect { box-shadow: 0 0 0 3px var(--ep-color-warning); }

      &__header {
        display: flex;
        align-items: center;
        gap: $spacing-xs;
        padding: $spacing-xs $spacing-sm;
      }

      &__name {
        font-size: 13px;
        font-weight: 600;
      }

      &__body {
        padding: 0 $spacing-sm $spacing-xs;
        font-size: 12px;
        color: var(--ep-text-secondary);
      }

      &__assignee {
        &--empty { color: var(--ep-text-placeholder); }
      }

      &__connect {
        position: absolute;
        right: -10px;
        top: 50%;
        transform: translateY(-50%);
        width: 20px;
        height: 20px;
        border-radius: 50%;
        background: var(--ep-color-primary);
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        font-size: 12px;
      }

      &__delete {
        position: absolute;
        right: -8px;
        top: -8px;
        width: 18px;
        height: 18px;
        border-radius: 50%;
        background: var(--ep-color-danger);
        color: #fff;
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 10px;
      }
    }

    .canvas-connect-hint {
      position: absolute;
      top: $spacing-sm;
      left: 50%;
      transform: translateX(-50%);
      background: var(--ep-color-warning);
      color: #fff;
      padding: $spacing-xs $spacing-md;
      border-radius: 4px;
      display: flex;
      align-items: center;
      gap: $spacing-sm;
      z-index: 10;
      font-size: 13px;
    }
  }

  &__props {
    width: 280px;
    background: var(--ep-bg-color);
    border: 1px solid var(--ep-border-color);
    border-radius: 4px;
    padding: $spacing-sm;
    flex-shrink: 0;
    overflow-y: auto;

    .props-title {
      font-weight: 600;
      margin-bottom: $spacing-sm;
      color: var(--ep-text-primary);
    }

    .props-empty {
      color: var(--ep-text-placeholder);
      font-size: 13px;
      text-align: center;
      padding: $spacing-md 0;
    }

    .props-edges {
      max-height: 300px;
      overflow-y: auto;
    }

    .props-edge {
      display: flex;
      align-items: center;
      gap: $spacing-xs;
      padding: $spacing-xs;
      margin-bottom: $spacing-xs;
      border: 1px solid var(--ep-border-color);
      border-radius: 4px;
      cursor: pointer;
      font-size: 12px;

      &--selected { border-color: var(--ep-color-primary); background: var(--ep-color-primary-light-9); }
      &__label { flex: 1; }
      &__input { flex: 1; }
      &__delete { color: var(--ep-color-danger); cursor: pointer; }
    }
  }
}

.toolbar-left, .toolbar-right {
  display: flex;
  align-items: center;
  gap: $spacing-sm;
}

.toolbar-title {
  font-size: 16px;
  font-weight: 600;
  color: var(--ep-text-primary);
}
</style>
