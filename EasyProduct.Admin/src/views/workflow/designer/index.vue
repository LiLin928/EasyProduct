<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  ArrowLeft,
  CircleCheck,
  Check,
  Delete,
  RefreshLeft,
  MagicStick,
  VideoPlay,
  Aim
} from '@element-plus/icons-vue'
import { useWorkflowEditorStore } from '@/stores/workflowEditor'
import { useWorkflowExecutionStore } from '@/stores/workflowExecution'
import WorkflowCanvas from './components/WorkflowCanvas.vue'
import NodePalette from './components/NodePalette.vue'
import type { NodeType } from '@/types/workflow'
import { NODE_TYPES } from '@/types/workflow'

const route = useRoute()
const router = useRouter()
const editorStore = useWorkflowEditorStore()
const execStore = useWorkflowExecutionStore()

const loading = ref(false)
const saving = ref(false)
const publishing = ref(false)
const executing = ref(false)

// 节点配置表单
const configForm = ref({
  name: '',
  assigneeType: 'user',
  assigneeName: '',
  condition: '',
  delayTime: 0,
  serviceUrl: ''
})

const hasUndo = computed(() => editorStore.undoStack.length > 0)
const selectedNode = computed(() => {
  if (!editorStore.selectedNodeId) return null
  return editorStore.nodes.find(n => n.id === editorStore.selectedNodeId)
})

const nodeTypeInfo = computed(() => {
  if (!selectedNode.value) return null
  return NODE_TYPES.find(n => n.type === selectedNode.value?.type)
})

onMounted(() => {
  const id = route.params.id as string
  if (id === 'new') {
    editorStore.createNew()
  } else if (id) {
    loadWorkflow(id)
  }
})

async function loadWorkflow(id: string) {
  loading.value = true
  try {
    await editorStore.load(id)
  } catch (error) {
    ElMessage.error('加载流程失败')
    router.push('/workflow/designer')
  } finally {
    loading.value = false
  }
}

function goBack() {
  router.push('/workflow/designer')
}

async function handleSave() {
  // If name is empty or default, prompt user
  if (!editorStore.name.trim() || editorStore.name === '新流程') {
    try {
      const result = await ElMessageBox.prompt('请输入流程名称', '保存流程', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        inputValue: editorStore.name === '新流程' ? '' : editorStore.name,
        inputValidator: (val: string) => {
          if (!val.trim()) return '流程名称不能为空'
          return true
        }
      })
      editorStore.name = result.value.trim()
    } catch {
      return
    }
  }
  
  saving.value = true
  try {
    const success = await editorStore.save()
    if (success) {
      ElMessage.success('保存成功')
    } else {
      ElMessage.error('保存失败')
    }
  } finally {
    saving.value = false
  }
}

async function handlePublish() {
  publishing.value = true
  try {
    const success = await editorStore.publish()
    if (success) {
      ElMessage.success('发布成功')
    } else {
      ElMessage.error('发布失败')
    }
  } finally {
    publishing.value = false
  }
}

function handleUndo() {
  editorStore.undo()
  ElMessage.success('已撤销')
}

function handleAutoLayout() {
  editorStore.autoLayout()
  ElMessage.success('自动布局完成')
}

async function handleValidate() {
  ElMessage.success('流程验证通过')
}

async function handleExecute(deAim = false) {
  if (!editorStore.id) {
    ElMessage.warning('请先保存流程')
    return
  }
  executing.value = true
  execStore.startExecution(deAim)

  // 模拟执行
  for (const node of editorStore.nodes) {
    execStore.updateNodeState(node.id, 'running')
    await new Promise(resolve => setTimeout(resolve, 800))
    execStore.updateNodeState(node.id, 'success', Math.floor(Math.random() * 2000) + 500)
  }

  execStore.completeExecution(true)
  executing.value = false
  ElMessage.success('执行完成')
}

function handleDeleteNode() {
  if (selectedNode.value) {
    ElMessageBox.confirm('确定删除该节点吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(() => {
      editorStore.removeNode(selectedNode.value!.id)
      editorStore.selectedNodeId = ''
      ElMessage.success('删除成功')
    })
  }
}

// 保存节点配置
function saveNodeConfig() {
  if (selectedNode.value) {
    editorStore.updateNode(selectedNode.value.id, {
      name: configForm.value.name
    })
    editorStore.updateNodeConfig(selectedNode.value.id, {
      ...selectedNode.value.data?.config,
      ...configForm.value
    })
    ElMessage.success('配置已保存')
  }
}

// 监听选中节点变化，更新表单
import { watch } from 'vue'
watch(selectedNode, (node) => {
  if (node) {
    configForm.value = {
      name: node.name,
      assigneeType: node.data?.config?.assigneeType || 'user',
      assigneeName: node.data?.config?.assigneeName || '',
      condition: node.data?.config?.condition || '',
      delayTime: node.data?.config?.delayTime || 0,
      serviceUrl: node.data?.config?.serviceUrl || ''
    }
  }
}, { immediate: true })
</script>

<template>
  <div class="workflow-designer">
    <!-- 顶部工具栏 -->
    <div class="designer-toolbar">
      <div class="toolbar-left">
        <el-button
          :icon="ArrowLeft"
          @click="goBack"
        >
          返回
        </el-button>
        <span class="workflow-name">{{ editorStore.name }}</span>
        <el-tag
          v-if="editorStore.status === 'published'"
          type="success"
          size="small"
        >
          已发布
        </el-tag>
        <el-tag
          v-else
          type="info"
          size="small"
        >
          草稿
        </el-tag>
        <span class="version">v{{ editorStore.version }}</span>
        <el-tag
          v-if="editorStore.dirty"
          type="warning"
          size="small"
        >
          未保存
        </el-tag>
      </div>
      <div class="toolbar-right">
        <el-button
          :icon="RefreshLeft"
          :disabled="!hasUndo"
          @click="handleUndo"
        >
          撤销
        </el-button>
        <el-button
          :icon="MagicStick"
          @click="handleAutoLayout"
        >
          自动布局
        </el-button>
        <el-button
          :icon="CircleCheck"
          @click="handleValidate"
        >
          验证
        </el-button>
        <el-divider direction="vertical" />
        <el-button
          :icon="VideoPlay"
          type="primary"
          :loading="executing"
          @click="handleExecute(false)"
        >
          执行
        </el-button>
        <el-button
          :icon="Aim"
          type="warning"
          :loading="executing"
          @click="handleExecute(true)"
        >
          调试
        </el-button>
        <el-divider direction="vertical" />
        <el-button
          :icon="Check"
          type="primary"
          :loading="saving"
          @click="handleSave"
        >
          保存
        </el-button>
        <el-button
          type="success"
          :loading="publishing"
          @click="handlePublish"
        >
          发布
        </el-button>
      </div>
    </div>

    <!-- 主内容区 -->
    <div
      v-loading="loading"
      class="designer-main"
    >
      <NodePalette />

      <!-- 画布 -->
      <div class="canvas-wrapper">
        <WorkflowCanvas />
      </div>

      <!-- 右侧属性面板 -->
      <div class="properties-panel">
        <div class="panel-header">
          <h4>属性配置</h4>
        </div>

        <div
          v-if="!selectedNode"
          class="panel-empty"
        >
          请选择节点进行配置
        </div>

        <div
          v-else
          class="panel-content"
        >
          <el-form
            label-width="80px"
            size="small"
          >
            <el-form-item label="节点类型">
              <el-tag
                :color="nodeTypeInfo?.color"
                effect="dark"
              >
                {{ nodeTypeInfo?.name }}
              </el-tag>
            </el-form-item>

            <el-form-item label="节点名称">
              <el-input v-model="configForm.name" />
            </el-form-item>

            <!-- 审批节点配置 -->
            <template v-if="selectedNode.type === 'approval'">
              <el-form-item label="审批类型">
                <el-select
                  v-model="configForm.assigneeType"
                  placeholder="请选择"
                >
                  <el-option
                    label="指定用户"
                    value="user"
                  />
                  <el-option
                    label="指定角色"
                    value="role"
                  />
                  <el-option
                    label="上级主管"
                    value="supervisor"
                  />
                </el-select>
              </el-form-item>
              <el-form-item
                v-if="configForm.assigneeType !== 'supervisor'"
                label="审批人"
              >
                <el-input
                  v-model="configForm.assigneeName"
                  placeholder="请输入审批人"
                />
              </el-form-item>
            </template>

            <!-- 条件节点配置 -->
            <template v-if="selectedNode.type === 'condition'">
              <el-form-item label="条件表达式">
                <el-input
                  v-model="configForm.condition"
                  type="textarea"
                  rows="3"
                  placeholder="例如: days > 3"
                />
              </el-form-item>
            </template>

            <!-- 延迟节点配置 -->
            <template v-if="selectedNode.type === 'delay'">
              <el-form-item label="延迟时间">
                <el-input-number
                  v-model="configForm.delayTime"
                  :min="0"
                  :step="60"
                />
                <span class="unit">秒</span>
              </el-form-item>
            </template>

            <!-- 服务节点配置 -->
            <template v-if="selectedNode.type === 'service'">
              <el-form-item label="服务地址">
                <el-input
                  v-model="configForm.serviceUrl"
                  placeholder="请输入服务地址"
                />
              </el-form-item>
            </template>

            <el-form-item>
              <el-button
                type="primary"
                @click="saveNodeConfig"
              >
                保存配置
              </el-button>
              <el-button
                :icon="Delete"
                type="danger"
                @click="handleDeleteNode"
              >
                删除节点
              </el-button>
            </el-form-item>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.workflow-designer {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f5f7fa;
}

.designer-toolbar {
  height: 56px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 16px;
  background: #fff;
  border-bottom: 1px solid #ebeef5;

  .toolbar-left {
    display: flex;
    align-items: center;
    gap: 12px;

    .workflow-name {
      font-size: 16px;
      font-weight: 500;
      color: #303133;
    }

    .version {
      font-size: 12px;
      color: #909399;
    }
  }

  .toolbar-right {
    display: flex;
    align-items: center;
    gap: 8px;
  }
}

.designer-main {
  flex: 1;
  display: flex;
  overflow: hidden;
}

.canvas-wrapper {
  flex: 1;
  position: relative;
  overflow: hidden;
}

.properties-panel {
  width: 300px;
  background: #fff;
  border-left: 1px solid #ebeef5;
  display: flex;
  flex-direction: column;

  .panel-header {
    padding: 12px 16px;
    border-bottom: 1px solid #ebeef5;

    h4 {
      margin: 0;
      font-size: 14px;
      color: #303133;
    }
  }

  .panel-empty {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #909399;
    font-size: 13px;
  }

  .panel-content {
    flex: 1;
    padding: 16px;
    overflow-y: auto;
  }
}

.unit {
  margin-left: 8px;
  color: #909399;
}
</style>
