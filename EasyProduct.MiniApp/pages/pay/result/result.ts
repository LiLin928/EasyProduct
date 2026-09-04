// pages/pay/result/result.ts —— 支付结果页
import { t } from '../../../utils/i18n'
import { getOrderDetail } from '../../../api/order'
import { orderStore } from '../../../stores/order.store'
import type { Order } from '../../../types/order.types'

interface PayResultPageData {
  orderId: string
  order: Order | null
  success: boolean
  loading: boolean
}

Page<PayResultPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    orderId: '',
    order: null,
    success: false,
    loading: false,
  },

  onLoad(options) {
    const { orderId, success } = options
    this.setData({
      orderId,
      success: success === 'true',
    })
    
    if (orderId) {
      this.loadOrderDetail(orderId)
    }
  },

  /** 加载订单详情 */
  async loadOrderDetail(id: string) {
    this.setData({ loading: true })
    try {
      const order = await getOrderDetail(id)
      this.setData({ order })
      orderStore.setCurrentOrder(order)
    } catch (error) {
      console.error('加载订单详情失败:', error)
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 查看订单 */
  onViewOrder() {
    const { orderId } = this.data
    wx.redirectTo({
      url: `/pages/order/detail/detail?id=${orderId}`,
    })
  },

  /** 返回首页 */
  onGoHome() {
    wx.switchTab({
      url: '/pages/index/index',
    })
  },

  /** 继续购物 */
  onContinueShopping() {
    wx.switchTab({
      url: '/pages/index/index',
    })
  },
})
