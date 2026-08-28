// 运维管理类型定义

// ── 操作日志 ──
export interface OperateLog {
  id: string
  userName: string
  module: string
  action: string
  method: string
  url: string
  params: string
  ip: string
  userAgent: string
  duration: number
  status: 'success' | 'fail'
  errorMessage: string
  createdAt: string
}

// ── 登录日志 ──
export interface LoginLog {
  id: string
  userName: string
  ip: string
  location: string
  browser: string
  os: string
  status: 'success' | 'fail'
  message: string
  createdAt: string
}

// ── 定时任务 ──
export type TaskStatus = 'running' | 'paused'

export interface Task {
  id: string
  taskName: string
  taskGroup: string
  cron: string
  className: string
  methodName: string
  description: string
  status: TaskStatus
  lastRunTime: string
  nextRunTime: string
  createdAt: string
  updatedAt: string
}

export const TASK_STATUS_OPTIONS = [
  { value: 'running', labelKey: 'ops.task.statusRunning' },
  { value: 'paused', labelKey: 'ops.task.statusPaused' },
] as const

// ── 任务日志 ──
export type TaskLogStatus = 'success' | 'fail' | 'running'

export interface TaskLog {
  id: string
  taskId: string
  taskName: string
  startTime: string
  endTime: string
  duration: number
  status: TaskLogStatus
  errorMessage: string
  createdAt: string
}

export const TASK_LOG_STATUS_OPTIONS = [
  { value: 'success', labelKey: 'ops.taskLog.statusSuccess' },
  { value: 'fail', labelKey: 'ops.taskLog.statusFail' },
  { value: 'running', labelKey: 'ops.taskLog.statusRunning' },
] as const

// ── 统一日志查询 ──
export type LogLevel = 'debug' | 'info' | 'warn' | 'error'

export interface LogQuery {
  id: string
  module: string
  level: LogLevel
  message: string
  stackTrace: string
  userName: string
  ip: string
  createdAt: string
}

export const LOG_LEVEL_OPTIONS = [
  { value: 'debug', labelKey: 'ops.logQuery.levelDebug' },
  { value: 'info', labelKey: 'ops.logQuery.levelInfo' },
  { value: 'warn', labelKey: 'ops.logQuery.levelWarn' },
  { value: 'error', labelKey: 'ops.logQuery.levelError' },
] as const

export const LOG_MODULE_OPTIONS = [
  { value: 'basic', labelKey: 'ops.logQuery.moduleBasic' },
  { value: 'site', labelKey: 'ops.logQuery.moduleSite' },
  { value: 'product', labelKey: 'ops.logQuery.moduleProduct' },
  { value: 'mall', labelKey: 'ops.logQuery.moduleMall' },
  { value: 'crm', labelKey: 'ops.logQuery.moduleCrm' },
  { value: 'ops', labelKey: 'ops.logQuery.moduleOps' },
] as const
