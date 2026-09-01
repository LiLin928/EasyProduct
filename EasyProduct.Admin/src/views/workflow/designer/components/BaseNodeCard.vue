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

// 节点类型配置
const nodeConfig: Record<NodeType, { color: string; bgColor: string; icon: string; borderRadius?: string }> = {
  start: { color: '#334155', bgColor: '#f1f5f9', icon: 'VideoPlay', borderRadius: '8px' },
  end: { color: '#334155', bgColor: '#f1f5f9', icon: 'VideoPause', borderRadius: '8px' },
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

// Handle 样式
const handleStyle = {
  background: '#fff',
  border: '2px solid #409eff',
  width: '10px',
  height: '10px'
}

// 打开节点配置
function openConfig() {
  editorStore.selectedNodeId = props.node.id
}
</script>

<template>
  <div
    class="base-node-card"
    :class="{ selected: props.selected }"
    :style="{
      borderColor: config.color,
      backgroundColor: config.bgColor,
      borderRadius: config.borderRadius || '8px'
    }"
  >
    <!-- 设置按钮 -->
    <div
      class="node-settings-btn"
      @click.stop="openConfig"
      @mousedown.stop
    >
      <el-icon :size="14">
        <Setting />
      </el-icon>
    </div>

    <!-- 节点头部 -->
    <div class="node-header">
      <el-icon
        :style="{ color: config.color }"
        :size="14"
      >
        <component :is="config.icon" />
      </el-icon>
      <span class="node-name">{{ node.data?.name || node.type }}</span>
    </div>

    <!-- 配置预览行 -->
    <div
      v-if="node.data?.rows?.length"
      class="node-rows"
    >
      <div
        v-for="(row, i) in node.data.rows"
        :key="i"
        class="node-row"
      >
        <span class="row-key">{{ row[0] }}</span>
        <span class="row-value">{{ row[1] }}</span>
      </div>
    </div>

    <!-- 输入连接点（开始节点无输入） -->
    <Handle
      v-if="node.type !== 'start'"
      type="target"
      :position="Position.Left"
      :style="handleStyle"
    />

    <!-- 输出连接点（条件节点双出口，结束节点无输出） -->
    <template v-if="node.type === 'condition'">
      <Handle
        id="yes"
        type="source"
        :position="Position.Right"
        :style="{ ...handleStyle, top: '30%' }"
      />
      <Handle
        id="no"
        type="source"
        :position="Position.Right"
        :style="{ ...handleStyle, top: '70%' }"
      />
    </template>
    <Handle
      v-else-if="node.type !== 'end'"
      type="source"
      :position="Position.Right"
      :style="handleStyle"
    />
  </div>
</template>

<style lang="scss" scoped>
.base-node-card {
  position: relative;
  width: 100px;
  min-width: 100px;
  max-width: 120px;
  padding: 3px 6px;
  border-width: 2px;
  border-style: solid;
  cursor: pointer;
  transition: all 0.2s;

  &.selected {
    box-shadow: 0 0 0 3px rgba(64, 158, 255, 0.4);
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
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  cursor: pointer;
  opacity: 0;
  transition: opacity 0.2s, background 0.15s;
  color: #909399;
  z-index: 5;

  &:hover {
    background: rgba(0, 0, 0, 0.08);
    color: #409eff;
  }
}

.node-header {
  display: flex;
  align-items: center;
  gap: 2px;
  margin-bottom: 0;

  .node-name {
    flex: 1;
    font-size: 10px;
    font-weight: 500;
    color: #303133;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}

.node-rows {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.node-row {
  display: flex;
  gap: 6px;
  font-size: 8px;
  line-height: 1.4;

  .row-key {
    color: #909399;
    min-width: 40px;
  }

  .row-value {
    color: #606266;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}
</style>
