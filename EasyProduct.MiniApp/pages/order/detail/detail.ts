// pages/order/detail/detail.ts —— 订单详情页
import { t } from '../../../utils/i18n'
import { getOrderDetail, cancelOrder, confirmReceive } from '../../../api/order'
import { orderStore } from '../../../stores/order.store'
import { OrderStatus, OrderStatusMap, type Order } from '../../../types/order.types'

interface OrderDetailPageData {
  order: Order | null
  loading: boolean
}

Page<OrderDetailPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    order: null,
    loading: false,
  },

  onLoad(options) {
    const { id } = options
    if (id) {
      this.loadOrderDetail(id)
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
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 获取状态文本 */
  getStatusText(status: OrderStatus): string {
    return OrderStatusMap[status]?.text || status
  },

  /** 获取状态颜色 */
  getStatusColor(status: OrderStatus): string {
    return OrderStatusMap[status]?.color || '#999'
  },

  /** 复制订单号 */
  onCopyOrderNo() {
    const { order } = this.data
    if (!order) return
    
    wx.setClipboardData({
      data: order.orderNo,
      success: () => {
        wx.showToast({ title: '已复制', icon: 'success' })
      },
    })
  },

  /** 立即支付 */
  onPayNow() {
    const { order } = this.data
    if (!order) return
    
    wx.navigateTo({
      url: `/pages/pay/checkout/checkout?orderId=${order.id}`,
    })
  },

  /** 取消订单 */
  onCancelOrder() {
    const { order } = this.data
    if (!order) return
    
    wx.showModal({
      title: '提示',
      content: '确定取消该订单吗？',
      confirmText: '确定',
      confirmColor: '#ff5722',
      success: async (res) => {
        if (res.confirm) {
          try {
            await cancelOrder(order.id)
            wx.showToast({ title: '取消成功', icon: 'success' })
            orderStore.updateOrderStatus(order.id, 'cancelled')
            this.loadOrderDetail(order.id)
          } catch (error) {
            console.error('取消订单失败:', error)
            wx.showToast({ title: '取消失败', icon: 'none' })
          }
        }
      },
    })
  },

  /** 确认收货 */
  onConfirmReceive() {
    const { order } = this.data
    if (!order) return
    
    wx.showModal({
      title: '提示',
      content: '确认已收到商品？',
      confirmText: '确认',
      confirmColor: '#1989fa',
      success: async (res) => {
        if (res.confirm) {
          try {
            await confirmReceive(order.id)
            wx.showToast({ title: '确认成功', icon: 'success' })
            orderStore.updateOrderStatus(order.id, 'completed')
            this.loadOrderDetail(order.id)
          } catch (error) {
            console.error('确认收货失败:', error)
            wx.showToast({ title: '确认失败', icon: 'none' })
          }
        }
      },
    })
  },

  /** 再次购买 */
  onBuyAgain() {
    wx.switchTab({
      url: '/pages/index/index',
    })
  },

  /** 联系客服 */
  onContactService() {
    wx.showModal({
      title: '联系客服',
      content: '客服电话：400-888-8888',
      confirmText: '拨打',
      success: (res) => {
        if (res.confirm) {
          wx.makePhoneCall({
            phoneNumber: '400-888-8888',
          })
        }
      },
    })
  },

  /** 查看物流 */
  onViewLogistics() {
    const { order } = this.data
    if (!order?.logistics) {
      wx.showToast({ title: '暂无物流信息', icon: 'none' })
      return
    }
    // 跳转到物流页面
    wx.navigateTo({
      url: `/pages/order/logistics/logistics?id=${order.id}`,
    })
  },
})
