// api/cart.ts —— 购物车相关 API
import { request } from '../utils/request'
import type { CartItem } from '../types/cart.types'

const BASE_URL = '/cart'

/** 获取购物车列表 */
export function getCartList(): Promise<CartItem[]> {
  return request<CartItem[]>({
    url: BASE_URL,
    method: 'GET',
  })
}

/** 添加商品到购物车
 * @param productId 商品ID
 * @param count 数量
 */
export function addToCart(productId: string, count: number): Promise<CartItem> {
  return request<CartItem>({
    url: BASE_URL,
    method: 'POST',
    data: { productId, count },
  })
}

/** 更新购物车商品数量
 * @param itemId 购物车项ID
 * @param count 数量
 */
export function updateCartItem(itemId: string, count: number): Promise<CartItem> {
  return request<CartItem>({
    url: `${BASE_URL}/${itemId}`,
    method: 'PUT',
    data: { count },
  })
}

/** 删除购物车商品
 * @param itemId 购物车项ID
 */
export function removeFromCart(itemId: string): Promise<void> {
  return request<void>({
    url: `${BASE_URL}/${itemId}`,
    method: 'DELETE',
  })
}

/** 清空购物车 */
export function clearCart(): Promise<void> {
  return request<void>({
    url: BASE_URL,
    method: 'DELETE',
  })
}
