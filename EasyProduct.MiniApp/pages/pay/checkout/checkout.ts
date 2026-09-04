// pages/pay/checkout/checkout.ts —— 订单结算页
import { t } from '../../../utils/i18n'
import { createOrder } from '../../../api/order'
import { cartStore } from '../../../stores/cart.store'
import { orderStore } from '../../../stores/order.store'
import type { CartItem } from '../../../types/cart.types'
import type { Address } from '../../../types/address.types'

interface CheckoutPageData {
  cartItemIds: string[]
  selectedItems: CartItem[]
  selectedAddress: Address | null
  remark: string
  loading: boolean
  submitting: boolean
  totalAmount: number
}

Page<CheckoutPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    cartItemIds: [],
    selectedItems: [],
    selectedAddress: null,
    remark: '',
    loading: false,
    submitting: false,
    totalAmount: 0,
  },

  onLoad(options) {
    // 从URL参数获取购物车商品ID
    const { cartItemIds } = options
    if (cartItemIds) {
      this.setData({ cartItemIds: cartItemIds.split(',') })
      this.loadCartItems()
    }
  },

  onShow() {
    // 检查是否有从地址列表页返回的选中地址
    const eventChannel = this.getOpenerEventChannel()
    if (eventChannel && typeof eventChannel.on === 'function') {
      eventChannel.on('selectAddress', (address: Address) => {
        this.setData({ selectedAddress: address })
      })
    }
  },

  /** 加载购物车商品 */
  loadCartItems() {
    const { cartItemIds } = this.data
    const allItems = cartStore.getSelectedItems()
    
    // 如果购物车已选中了这些商品，直接使用
    if (allItems.length > 0) {
      this.setData({ selectedItems: allItems })
      this.calculateTotal()
      return
    }
    
    // 否则从购物车store加载所有并筛选
    cartStore.initCart().then(() => {
      const items = cartStore.getState().items.filter(item => 
        cartItemIds.includes(item.id)
      )
      this.setData({ selectedItems: items })
      this.calculateTotal()
    })
  },

  /** 计算总金额 */
  calculateTotal() {
    const { selectedItems } = this.data
    const total = selectedItems.reduce((sum, item) => {
      const subtotal = (item.product?.price || 0) * item.count
      return sum + subtotal
    }, 0)
    this.setData({ totalAmount: total })
  },

  /** 选择地址 */
  onSelectAddress() {
    const { selectedAddress } = this.data
    wx.navigateTo({
      url: `/pages/address/list/list?from=checkout&selectedId=${selectedAddress?.id || ''}`,
      events: {
        selectAddress: (address: Address) => {
          this.setData({ selectedAddress: address })
        }
      }
    })
  },

  /** 输入备注 */
  onRemarkInput(e: WechatMiniprogram.TextareaInput) {
    this.setData({ remark: e.detail.value })
  },

  /** 提交订单 */
  async onSubmitOrder() {
    const { selectedItems, selectedAddress, remark, submitting } = this.data
    
    if (submitting) return
    
    if (selectedItems.length === 0) {
      wx.showToast({ title: '请选择商品', icon: 'none' })
      return
    }
    
    if (!selectedAddress) {
      wx.showToast({ title: '请选择收货地址', icon: 'none' })
      return
    }
    
    this.setData({ submitting: true })
    
    try {
      const order = await createOrder({
        cartItemIds: selectedItems.map(item => item.id),
        addressId: selectedAddress.id,
        remark,
      })
      
      // 更新store
      orderStore.addOrder(order)
      
      // 清空已选中的购物车商品
      await cartStore.clearSelected()
      
      // 跳转到支付结果页
      wx.redirectTo({
        url: `/pages/pay/result/result?orderId=${order.id}&success=true`,
      })
    } catch (error) {
      console.error('创建订单失败:', error)
      wx.showToast({ title: '创建订单失败', icon: 'none' })
      this.setData({ submitting: false })
    }
  },

  /** 去购物 */
  onGoShopping() {
    wx.switchTab({ url: '/pages/index/index' })
  },
})
