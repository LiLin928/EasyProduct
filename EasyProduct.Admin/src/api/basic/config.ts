// src/api/basic/config.ts
import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { SystemConfig, SystemConfigQuery, SystemConfigCreateParams } from '@/types/basic'

/** 系统参数列表（分页） */
export const getConfigList = (params: SystemConfigQuery) =>
  get<PageResult<SystemConfig>>('/api/admin/basic/config/list', params)

/** 新增系统参数 */
export const createConfig = (data: SystemConfigCreateParams) =>
  post<{ id: string }>('/api/admin/basic/config', data)

/** 编辑系统参数 */
export const updateConfig = (id: string, data: Partial<SystemConfigCreateParams>) =>
  put<{ id: string }>(`/api/admin/basic/config/${id}`, data)

/** 删除系统参数 */
export const deleteConfig = (id: string) =>
  del<null>(`/api/admin/basic/config/${id}`)

/** 批量删除系统参数 */
export const deleteConfigBatch = (ids: string[]) =>
  post<null>('/api/admin/basic/config/batch-delete', { ids })