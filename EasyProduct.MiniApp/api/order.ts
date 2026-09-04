// api/order.ts —— 订单相关 API
import { request } from '../utils/request'
import type { Order, OrderListResult, CreateOrderParams, OrderQuery } from '../types/order.types'

const BASE_URL = '/orders'

/** 创建订单
 * @param params 创建订单参数
 */
export function createOrder(params: CreateOrderParams): Promise<Order> {
  return request<Order>({
    url: BASE_URL,
    method: 'POST',
    data: params,
  })
}

/** 获取订单列表
 * @param params 查询参数
 */
export function getOrderList(params: OrderQuery): Promise<OrderListResult> {
  return request<OrderListResult>({
    url: BASE_URL,
    method: 'GET',
    data: params,
  })
}

/** 获取订单详情
 * @param id 订单ID
 */
export function getOrderDetail(id: string): Promise<Order> {
  return request<Order>({
    url: `${BASE_URL}/${id}`,
    method: 'GET',
  })
}

/** 取消订单
 * @param id 订单ID
 */
export function cancelOrder(id: string): Promise<void> {
  return request<void>({
    url: `${BASE_URL}/${id}/cancel`,
    method: 'PUT',
  })
}

/** 确认收货
 * @param id 订单ID
 */
export function confirmReceive(id: string): Promise<void> {
  return request<void>({
    url: `${BASE_URL}/${id}/receive`,
    method: 'PUT',
  })
}
