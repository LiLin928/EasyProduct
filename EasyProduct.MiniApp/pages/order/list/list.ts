// pages/order/list/list.ts —— 订单列表页
import { t } from '../../../utils/i18n'
import { getOrderList, cancelOrder, confirmReceive } from '../../../api/order'
import { orderStore } from '../../../stores/order.store'
import { OrderStatus, OrderStatusMap, type Order } from '../../../types/order.types'

const ORDER_TABS: { status: OrderStatus | ''; label: string }[] = [
  { status: '', label: '全部' },
  { status: 'pending', label: '待付款' },
  { status: 'paid', label: '待发货' },
  { status: 'shipped', label: '待收货' },
  { status: 'completed', label: '已完成' },
]

interface OrderListPageData {
  orders: Order[]
  currentStatus: OrderStatus | ''
  loading: boolean
  hasMore: boolean
  pageIndex: number
  pageSize: number
  tabs: typeof ORDER_TABS
}

Page<OrderListPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    orders: [],
    currentStatus: '',
    loading: false,
    hasMore: true,
    pageIndex: 1,
    pageSize: 10,
    tabs: ORDER_TABS,
  },

  onLoad(options) {
    // 从URL参数获取状态
    const { status } = options
    if (status && ORDER_TABS.some(tab => tab.status === status)) {
      this.setData({ currentStatus: status as OrderStatus })
    }
  },

  onShow() {
    this.refreshOrders()
  },

  /** 切换Tab */
  onTabChange(e: WechatMiniprogram.TouchEvent) {
    const { status } = e.currentTarget.dataset as { status: OrderStatus | '' }
    if (status === this.data.currentStatus) return
    
    this.setData({
      currentStatus: status,
      orders: [],
      pageIndex: 1,
      hasMore: true,
    })
    this.loadOrders()
  },

  /** 加载订单列表 */
  async loadOrders() {
    const { currentStatus, pageIndex, pageSize, loading, hasMore } = this.data
    if (loading || !hasMore) return

    this.setData({ loading: true })
    try {
      const result = await getOrderList({
        pageIndex,
        pageSize,
        status: currentStatus || undefined,
      })
      
      const newOrders = pageIndex === 1 ? result.list : [...this.data.orders, ...result.list]
      this.setData({
        orders: newOrders,
        hasMore: newOrders.length < result.total,
        pageIndex: pageIndex + 1,
      })
      
      // 更新store
      orderStore.setOrders(newOrders)
    } catch (error) {
      console.error('加载订单失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 刷新订单 */
  async refreshOrders() {
    this.setData({
      orders: [],
      pageIndex: 1,
      hasMore: true,
    })
    await this.loadOrders()
  },

  /** 下拉刷新 */
  async onPullDownRefresh() {
    await this.refreshOrders()
    wx.stopPullDownRefresh()
  },

  /** 加载更多 */
  onLoadMore() {
    this.loadOrders()
  },

  /** 获取状态文本 */
  getStatusText(status: OrderStatus): string {
    return OrderStatusMap[status]?.text || status
  },

  /** 获取状态颜色 */
  getStatusColor(status: OrderStatus): string {
    return OrderStatusMap[status]?.color || '#999'
  },

  /** 点击订单 */
  onOrderTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({
      url: `/pages/order/detail/detail?id=${id}`,
    })
  },

  /** 立即支付 */
  onPayNow(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({
      url: `/pages/pay/checkout/checkout?orderId=${id}`,
    })
  },

  /** 取消订单 */
  onCancelOrder(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.showModal({
      title: '提示',
      content: '确定取消该订单吗？',
      confirmText: '确定',
      confirmColor: '#ff5722',
      success: async (res) => {
        if (res.confirm) {
          try {
            await cancelOrder(id)
            wx.showToast({ title: '取消成功', icon: 'success' })
            orderStore.updateOrderStatus(id, 'cancelled')
            this.refreshOrders()
          } catch (error) {
            console.error('取消订单失败:', error)
            wx.showToast({ title: '取消失败', icon: 'none' })
          }
        }
      },
    })
  },

  /** 确认收货 */
  onConfirmReceive(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.showModal({
      title: '提示',
      content: '确认已收到商品？',
      confirmText: '确认',
      confirmColor: '#1989fa',
      success: async (res) => {
        if (res.confirm) {
          try {
            await confirmReceive(id)
            wx.showToast({ title: '确认成功', icon: 'success' })
            orderStore.updateOrderStatus(id, 'completed')
            this.refreshOrders()
          } catch (error) {
            console.error('确认收货失败:', error)
            wx.showToast({ title: '确认失败', icon: 'none' })
          }
        }
      },
    })
  },

  /** 再次购买 */
  onBuyAgain(e: WechatMiniprogram.TouchEvent) {
    const { items } = e.currentTarget.dataset as { items: Order['items'] }
    // 跳转到商品详情或加入购物车
    wx.switchTab({
      url: '/pages/index/index',
    })
  },
})
