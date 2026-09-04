// types/order.types.ts —— 订单相关类型定义
import type { PageResult } from './api.types'
import type { Address } from './address.types'

/** 订单状态 */
export type OrderStatus = 'pending' | 'paid' | 'shipped' | 'completed' | 'cancelled' | 'refunded'

/** 订单状态映射 */
export const OrderStatusMap: Record<OrderStatus, { text: string; color: string }> = {
  pending: { text: '待付款', color: '#ff976a' },
  paid: { text: '待发货', color: '#1989fa' },
  shipped: { text: '待收货', color: '#07c160' },
  completed: { text: '已完成', color: '#07c160' },
  cancelled: { text: '已取消', color: '#999' },
  refunded: { text: '已退款', color: '#999' },
}

/** 订单商品项 */
export interface OrderItem {
  id: string
  productId: string
  productName: string
  productImage: string
  price: number
  count: number
  subtotal: number
}

/** 订单信息 */
export interface Order {
  id: string
  orderNo: string
  status: OrderStatus
  items: OrderItem[]
  totalAmount: number
  address?: Address
  addressId?: string
  paymentTime?: string
  deliveryTime?: string
  completeTime?: string
  remark?: string
  logistics?: {
    company: string
    number: string
    status: string
  }
  createdAt: string
  updatedAt?: string
}

/** 创建订单参数 */
export interface CreateOrderParams {
  cartItemIds: string[]
  addressId: string
  remark?: string
}

/** 订单查询参数 */
export interface OrderQuery {
  pageIndex: number
  pageSize: number
  status?: OrderStatus
}

/** 订单列表结果 */
export type OrderListResult = PageResult<Order>
