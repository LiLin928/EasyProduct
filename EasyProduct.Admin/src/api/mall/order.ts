import { get, put } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Order, OrderQuery, OrderStatusUpdateParams } from '@/types/mall'

/** 商城订单列表（分页 + 筛选） */
export const getOrderList = (params: OrderQuery) =>
  get<PageResult<Order>>('/api/admin/mall/order/list', params)

/** 商城订单详情（含商品明细） */
export const getOrderDetail = (id: string) =>
  get<Order>(`/api/admin/mall/order/${id}`)

/** 更新订单状态（状态机） */
export const updateOrderStatus = (id: string, data: OrderStatusUpdateParams) =>
  put<null>(`/api/admin/mall/order/${id}/status`, data)

/** 获取会员下拉选项 */
export const getOrderMemberOptions = () =>
  get<Array<{ id: string; name: string }>>('/api/admin/mall/order/member/options')

/** 获取订单统计 */
export const getOrderStats = () =>
  get<{
    total: number
    pending: number
    paid: number
    shipped: number
    completed: number
    cancelled: number
    refunded: number
    totalAmount: number
  }>('/api/admin/mall/order/stats')
