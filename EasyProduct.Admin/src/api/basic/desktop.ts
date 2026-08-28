// src/api/basic/desktop.ts
import { get } from '@/utils/request'
import type {
  DesktopOverview,
  DashboardOverview,
  DashboardKpi,
  DashboardTodoCount,
  DashboardAlert,
} from '@/types/basic'

/** 工作台概览 */
export const getDesktopOverview = () =>
  get<DesktopOverview>('/api/admin/basic/desktop/overview')

/** Widget: 经营概览 */
export const getDashboardOverview = () =>
  get<DashboardOverview>('/api/admin/basic/desktop/widget-overview')

/** Widget: KPI */
export const getDashboardKpi = () =>
  get<DashboardKpi>('/api/admin/basic/desktop/widget-kpi')

/** Widget: 待办计数 */
export const getDashboardTodoCount = () =>
  get<DashboardTodoCount>('/api/admin/basic/desktop/widget-todo-count')

/** Widget: 预警列表 */
export const getDashboardAlerts = () =>
  get<DashboardAlert[]>('/api/admin/basic/desktop/widget-alerts')
