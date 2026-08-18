// src/api/basic/desktop.ts
import { get } from '@/utils/request'
import type { DesktopOverview } from '@/types/basic'

/** 工作台概览 */
export const getDesktopOverview = () =>
  get<DesktopOverview>('/api/admin/basic/desktop/overview')