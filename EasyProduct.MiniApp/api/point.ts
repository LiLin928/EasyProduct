// api/point.ts —— 积分相关 API
import { request } from '../utils/request'
import type { ApiResponse } from '../types/api.types'

const BASE_URL = '/point'

/** 积分余额 */
interface PointBalance {
  memberId: string
  totalPoints: number
  availablePoints: number
  frozenPoints: number
}

/** 积分流水 */
interface PointRecord {
  id: string
  memberId: string
  type: number
  source: number
  points: number
  balance: number
  remark?: string
  relatedId?: string
  createTime: string
}

/** 积分流水查询参数 */
interface PointRecordQuery {
  type?: number
  source?: number
  startTime?: string
  endTime?: string
  pageIndex?: number
  pageSize?: number
}

/** 分页结果 */
interface PageResult<T> {
  list: T[]
  total: number
}

/** 获取我的积分余额 */
export function getPointBalance(): Promise<PointBalance> {
  return request<PointBalance>({
    url: `${BASE_URL}/balance`,
    method: 'GET',
  })
}

/** 获取我的积分流水 */
export function getPointRecords(params: PointRecordQuery): Promise<PageResult<PointRecord>> {
  return request<PageResult<PointRecord>>({
    url: `${BASE_URL}/record/list`,
    method: 'GET',
    data: params,
  })
}

/** 获取我的积分兑换记录 */
export function getPointExchanges(pageIndex: number = 1, pageSize: number = 10): Promise<PageResult<any>> {
  return request<PageResult<any>>({
    url: `${BASE_URL}/exchange/list`,
    method: 'GET',
    data: { pageIndex, pageSize },
  })
}

/** 获取我的积分统计 */
export function getPointStatistics(): Promise<Record<string, any>> {
  return request<Record<string, any>>({
    url: `${BASE_URL}/statistics`,
    method: 'GET',
  })
}

/** 签到赠送积分 */
export function checkIn(): Promise<number> {
  return request<number>({
    url: `${BASE_URL}/check-in`,
    method: 'POST',
  })
}