import { get, post, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { PointsRecord, PointsQuery } from '@/types/mall'

/** 积分记录列表（分页 + 筛选） */
export const getPointsList = (params: PointsQuery) =>
  get<PageResult<PointsRecord>>('/api/admin/mall/points/list', params)

/** 积分记录详情 */
export const getPointsDetail = (id: string) =>
  get<PointsRecord>(`/api/admin/mall/points/${id}`)

/** 手动调整积分（新增积分记录） */
export const createPointsRecord = (data: {
  memberId: string
  type: 'earn' | 'spend'
  amount: number
  source?: string
  description?: string
}) => post<{ id: string }>('/api/admin/mall/points', data)

/** 删除积分记录 */
export const deletePointsRecord = (id: string) =>
  del<null>(`/api/admin/mall/points/${id}`)

/** 获取会员下拉选项 */
export const getPointsMemberOptions = () =>
  get<Array<{ id: string; name: string }>>('/api/admin/mall/points/member/options')
