<script setup lang="ts">
import { NODE_TYPES } from '@/types/workflow'

const basicNodes = NODE_TYPES.filter(n => n.group === 'basic')
const advancedNodes = NODE_TYPES.filter(n => n.group === 'advanced')

function handleDragStart(event: DragEvent, type: string) {
  event.dataTransfer?.setData('nodeType', type)
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move'
  }
}
</script>

<template>
  <div class="node-palette">
    <div class="palette-header">
      <h4>{{ $t('workflow.designer.palette') }}</h4>
    </div>

    <div class="palette-section">
      <div class="section-title">
        基础节点
      </div>
      <div class="node-list">
        <div
          v-for="node in basicNodes"
          :key="node.type"
          class="palette-item"
          :style="{ borderColor: node.color }"
          draggable="true"
          @dragstart="(e) => handleDragStart(e, node.type)"
        >
          <el-icon
            :style="{ color: node.color }"
            :size="16"
          >
            <component :is="node.icon" />
          </el-icon>
          <span class="item-name">{{ node.name }}</span>
        </div>
      </div>
    </div>

    <div class="palette-section">
      <div class="section-title">
        高级节点
      </div>
      <div class="node-list">
        <div
          v-for="node in advancedNodes"
          :key="node.type"
          class="palette-item"
          :style="{ borderColor: node.color }"
          draggable="true"
          @dragstart="(e) => handleDragStart(e, node.type)"
        >
          <el-icon
            :style="{ color: node.color }"
            :size="16"
          >
            <component :is="node.icon" />
          </el-icon>
          <span class="item-name">{{ node.name }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.node-palette {
  width: 240px;
  background: #fff;
  border-right: 1px solid #ebeef5;
  display: flex;
  flex-direction: column;
}

.palette-header {
  padding: 12px 16px;
  border-bottom: 1px solid #ebeef5;

  h4 {
    margin: 0;
    font-size: 14px;
    color: #303133;
  }
}

.palette-section {
  padding: 12px 16px;

  &:not(:first-child) {
    border-top: 1px solid #f0f0f0;
  }
}

.section-title {
  font-size: 12px;
  color: #909399;
  margin-bottom: 8px;
  font-weight: 500;
}

.node-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.palette-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border: 1px solid;
  border-radius: 6px;
  cursor: grab;
  background: #fff;
  transition: all 0.2s;

  &:hover {
    background: #f5f7fa;
    transform: translateX(2px);
  }

  &:active {
    cursor: grabbing;
  }

  .item-name {
    font-size: 13px;
    color: #303133;
  }
}
</style>
