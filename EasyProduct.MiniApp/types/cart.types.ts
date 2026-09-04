// types/cart.types.ts —— 购物车相关类型定义
import type { Product } from './product.types'

/** 购物车项 */
export interface CartItem {
  id: string
  productId: string
  product: Product
  count: number
  selected?: boolean
  createdAt?: string
  updatedAt?: string
}

/** 购物车状态 */
export interface CartState {
  items: CartItem[]
  totalCount: number
  totalPrice: number
  selectedCount: number
  selectedPrice: number
}
