// stores/cart.store.ts —— 购物车状态管理
import { BaseStore } from './base.store'
import type { CartItem, CartState } from '../types/cart.types'
import { getCartList, addToCart as apiAddToCart, updateCartItem, removeFromCart } from '../api/cart'

export class CartStore extends BaseStore<CartState> {
  private static instance: CartStore

  static getInstance(): CartStore {
    if (!CartStore.instance) {
      CartStore.instance = new CartStore()
    }
    return CartStore.instance
  }

  constructor() {
    super({
      items: [],
      totalCount: 0,
      totalPrice: 0,
      selectedCount: 0,
      selectedPrice: 0,
    })
  }

  /** 初始化购物车（从服务器同步） */
  async initCart(): Promise<void> {
    try {
      const items = await getCartList()
      this.setItems(items.map(item => ({ ...item, selected: true })))
    } catch (error) {
      console.error('初始化购物车失败:', error)
    }
  }

  /** 设置购物车列表 */
  setItems(items: CartItem[]): void {
    this.setState(this.calculateState(items))
  }

  /** 添加商品到购物车 */
  async addToCart(productId: string, count: number = 1): Promise<void> {
    await apiAddToCart(productId, count)
    const items = await getCartList()
    this.setItems(items.map(item => ({ ...item, selected: true })))
  }

  /** 更新商品数量 */
  async updateItemCount(itemId: string, count: number): Promise<void> {
    if (count <= 0) {
      await this.removeItem(itemId)
      return
    }
    await updateCartItem(itemId, count)
    const items = this.state.items.map(item =>
      item.id === itemId ? { ...item, count } : item
    )
    this.setItems(items)
  }

  /** 删除商品 */
  async removeItem(itemId: string): Promise<void> {
    await removeFromCart(itemId)
    const items = this.state.items.filter(item => item.id !== itemId)
    this.setItems(items)
  }

  /** 切换选中状态 */
  toggleSelect(itemId: string): void {
    const items = this.state.items.map(item =>
      item.id === itemId ? { ...item, selected: !item.selected } : item
    )
    this.setItems(items)
  }

  /** 全选/取消全选 */
  toggleSelectAll(selected: boolean): void {
    const items = this.state.items.map(item => ({ ...item, selected }))
    this.setItems(items)
  }

  /** 清空购物车 */
  async clearCart(): Promise<void> {
    const { clearCart: apiClearCart } = await import('../api/cart')
    await apiClearCart()
    this.setItems([])
  }

  /** 清空已选中商品 */
  async clearSelected(): Promise<void> {
    const selectedIds = this.getSelectedItems().map(item => item.id)
    for (const id of selectedIds) {
      await removeFromCart(id)
    }
    const items = this.state.items.filter(item => !item.selected)
    this.setItems(items)
  }

  /** 获取选中的商品 */
  getSelectedItems(): CartItem[] {
    return this.state.items.filter(item => item.selected)
  }

  /** 计算状态 */
  private calculateState(items: CartItem[]): CartState {
    const totalCount = items.reduce((sum, item) => sum + item.count, 0)
    const totalPrice = items.reduce(
      (sum, item) => sum + (item.product?.price || 0) * item.count,
      0
    )

    const selectedItems = items.filter(item => item.selected)
    const selectedCount = selectedItems.reduce((sum, item) => sum + item.count, 0)
    const selectedPrice = selectedItems.reduce(
      (sum, item) => sum + (item.product?.price || 0) * item.count,
      0
    )

    return {
      items,
      totalCount,
      totalPrice,
      selectedCount,
      selectedPrice,
    }
  }
}

export const cartStore = CartStore.getInstance()
