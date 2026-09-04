// stores/order.store.ts —— 订单状态管理
import { BaseStore } from './base.store'
import type { Order, OrderStatus } from '../types/order.types'

interface OrderState {
  orders: Order[]
  currentOrder: Order | null
  loading: boolean
}

export class OrderStore extends BaseStore<OrderState> {
  private static instance: OrderStore

  static getInstance(): OrderStore {
    if (!OrderStore.instance) {
      OrderStore.instance = new OrderStore()
    }
    return OrderStore.instance
  }

  constructor() {
    super({
      orders: [],
      currentOrder: null,
      loading: false,
    })
  }

  setLoading(loading: boolean): void {
    this.setState({ loading })
  }

  setOrders(orders: Order[]): void {
    this.setState({ orders })
  }

  addOrder(order: Order): void {
    this.setState({ orders: [order, ...this.state.orders] })
  }

  setCurrentOrder(order: Order | null): void {
    this.setState({ currentOrder: order })
  }

  updateOrderStatus(id: string, status: OrderStatus): void {
    const orders = this.state.orders.map(order =>
      order.id === id ? { ...order, status } : order
    )
    const currentOrder = this.state.currentOrder?.id === id
      ? { ...this.state.currentOrder, status }
      : this.state.currentOrder
    this.setState({ orders, currentOrder })
  }

  getOrdersByStatus(status?: OrderStatus): Order[] {
    if (!status) return this.state.orders
    return this.state.orders.filter(order => order.status === status)
  }
}

export const orderStore = OrderStore.getInstance()
