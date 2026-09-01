import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ExecState, NodeExecStatus } from '@/types/workflow'

export const useWorkflowExecutionStore = defineStore('workflowExecution', () => {
  // 执行状态
  const executing = ref(false)
  const debugMode = ref(false)
  const execId = ref('')
  const currentNodeId = ref('')

  // 节点状态映射
  const nodeStates = ref<ExecState>({})

  // 执行日志
  const logs = ref<{ time: string; nodeId: string; level: 'info' | 'success' | 'warning' | 'error'; content: string }[]>([])

  // 初始化
  function reset() {
    executing.value = false
    debugMode.value = false
    execId.value = ''
    currentNodeId.value = ''
    nodeStates.value = {}
    logs.value = []
  }

  // 更新节点状态
  function updateNodeState(nodeId: string, status: NodeExecStatus, durationMs?: number, output?: string) {
    nodeStates.value[nodeId] = { status, durationMs, output }
    if (status === 'running') {
      currentNodeId.value = nodeId
    }
  }

  // 添加日志
  function addLog(nodeId: string, level: 'info' | 'success' | 'warning' | 'error', content: string) {
    logs.value.push({
      time: new Date().toLocaleTimeString(),
      nodeId,
      level,
      content
    })
  }

  // 获取节点状态
  function getNodeStatus(nodeId: string): NodeExecStatus {
    return nodeStates.value[nodeId]?.status || 'idle'
  }

  // 获取节点执行时长
  function getNodeDuration(nodeId: string): number | undefined {
    return nodeStates.value[nodeId]?.durationMs
  }

  // 开始执行
  function startExecution(isDebug = false) {
    reset()
    executing.value = true
    debugMode.value = isDebug
    addLog('', 'info', isDebug ? '开始调试执行...' : '开始执行流程...')
  }

  // 完成执行
  function completeExecution(success: boolean) {
    executing.value = false
    addLog('', success ? 'success' : 'error', success ? '流程执行完成' : '流程执行失败')
  }

  // 中止执行
  function abortExecution() {
    executing.value = false
    addLog('', 'warning', '执行已中止')
  }

  return {
    // State
    executing,
    debugMode,
    execId,
    currentNodeId,
    nodeStates,
    logs,

    // Actions
    reset,
    updateNodeState,
    addLog,
    getNodeStatus,
    getNodeDuration,
    startExecution,
    completeExecution,
    abortExecution
  }
})
