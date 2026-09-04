// pages/cart/cart.ts —— 购物车页
import { t } from '../../utils/i18n'
import { cartStore } from '../../stores/cart.store'
import type { CartItem } from '../../types/cart.types'

interface CartPageData {
  items: CartItem[]
  totalCount: number
  selectedCount: number
  selectedPrice: number
  loading: boolean
  isSelectAll: boolean
  isEditing: boolean
}

Page<CartPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    items: [],
    totalCount: 0,
    selectedCount: 0,
    selectedPrice: 0,
    loading: false,
    isSelectAll: false,
    isEditing: false,
  },

  onLoad() {
    // 监听购物车状态变化
    cartStore.subscribe(this.onCartStateChange.bind(this))
    this.loadCartData()
  },

  onShow() {
    // 每次显示时同步数据
    cartStore.initCart()
    this.updateSelectAllStatus()
  },

  onUnload() {
    // 取消订阅
    // cartStore.unsubscribe(this.onCartStateChange.bind(this))
  },

  /** 购物车状态变化回调 */
  onCartStateChange(state: { items: CartItem[]; selectedCount: number; selectedPrice: number }) {
    const { items, selectedCount, selectedPrice } = state
    const isSelectAll = items.length > 0 && items.every(item => item.selected)
    this.setData({
      items,
      selectedCount,
      selectedPrice,
      isSelectAll,
    })
  },

  /** 加载购物车数据 */
  async loadCartData() {
    this.setData({ loading: true })
    try {
      await cartStore.initCart()
    } catch (error) {
      console.error('加载购物车失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 更新全选状态 */
  updateSelectAllStatus() {
    const { items } = this.data
    const isSelectAll = items.length > 0 && items.every(item => item.selected)
    this.setData({ isSelectAll })
  },

  /** 切换编辑模式 */
  toggleEdit() {
    this.setData({ isEditing: !this.data.isEditing })
  },

  /** 切换选中状态 */
  onToggleSelect(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    cartStore.toggleSelect(id)
    this.updateSelectAllStatus()
  },

  /** 全选/取消全选 */
  onToggleSelectAll() {
    const { items, isSelectAll } = this.data
    const newSelectAll = !isSelectAll
    cartStore.toggleSelectAll(newSelectAll)
    this.setData({ isSelectAll: newSelectAll })
  },

  /** 数量减少 */
  async onQuantityMinus(e: WechatMiniprogram.TouchEvent) {
    const { id, count } = e.currentTarget.dataset
    if (count > 1) {
      await cartStore.updateItemCount(id, count - 1)
    }
  },

  /** 数量增加 */
  async onQuantityPlus(e: WechatMiniprogram.TouchEvent) {
    const { id, count } = e.currentTarget.dataset
    await cartStore.updateItemCount(id, count + 1)
  },

  /** 删除商品 */
  async onDeleteItem(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.showModal({
      title: '提示',
      content: t('common.cart.deleteConfirm'),
      confirmText: t('common.button.delete'),
      confirmColor: '#ff5722',
      success: async (res) => {
        if (res.confirm) {
          await cartStore.removeItem(id)
          this.updateSelectAllStatus()
        }
      },
    })
  },

  /** 批量删除 */
  onBatchDelete() {
    const selectedItems = cartStore.getSelectedItems()
    if (selectedItems.length === 0) {
      wx.showToast({ title: '请选择要删除的商品', icon: 'none' })
      return
    }

    wx.showModal({
      title: '提示',
      content: `确定删除选中的${selectedItems.length}件商品吗？`,
      confirmText: t('common.button.delete'),
      confirmColor: '#ff5722',
      success: async (res) => {
        if (res.confirm) {
          await cartStore.clearSelected()
          this.updateSelectAllStatus()
        }
      },
    })
  },

  /** 点击商品 */
  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({ url: `/pages/product/detail/detail?id=${id}` })
  },

  /** 结算 */
  onCheckout() {
    const selectedItems = cartStore.getSelectedItems()
    if (selectedItems.length === 0) {
      wx.showToast({ title: '请选择要结算的商品', icon: 'none' })
      return
    }

    const cartItemIds = selectedItems.map(item => item.id).join(',')
    wx.navigateTo({
      url: `/pages/pay/checkout/checkout?cartItemIds=${cartItemIds}`,
    })
  },

  /** 去逛逛 */
  onGoShopping() {
    wx.switchTab({ url: '/pages/index/index' })
  },

  /** 下拉刷新 */
  async onPullDownRefresh() {
    await this.loadCartData()
    wx.stopPullDownRefresh()
  },
})
