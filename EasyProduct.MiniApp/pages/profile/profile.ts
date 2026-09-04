// pages/profile/profile.ts —— 个人中心页
import { t } from '../../utils/i18n'
import { memberStore } from '../../stores/member.store'
import { getMemberInfo } from '../../api/member'
import type { Member } from '../../types/member.types'

interface ProfilePageData {
  member: Member | null
  isLoggedIn: boolean
  loading: boolean
  orderStats: {
    pending: number
    paid: number
    shipped: number
  }
}

Page<ProfilePageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    member: null,
    isLoggedIn: false,
    loading: false,
    orderStats: {
      pending: 0,
      paid: 0,
      shipped: 0,
    },
  },

  onLoad() {
    // 监听会员状态变化
    memberStore.subscribe(this.onMemberStateChange.bind(this))
    this.checkLoginStatus()
  },

  onShow() {
    this.checkLoginStatus()
    this.loadMemberInfo()
  },

  onUnload() {
    // 取消订阅
    // memberStore.unsubscribe(this.onMemberStateChange.bind(this))
  },

  /** 会员状态变化回调 */
  onMemberStateChange(state: { member: Member | null; isLoggedIn: boolean }) {
    this.setData({
      member: state.member,
      isLoggedIn: state.isLoggedIn,
    })
  },

  /** 检查登录状态 */
  checkLoginStatus() {
    const state = memberStore.getState()
    this.setData({
      isLoggedIn: state.isLoggedIn,
      member: state.member,
    })
  },

  /** 加载会员信息 */
  async loadMemberInfo() {
    if (!this.data.isLoggedIn) return
    
    this.setData({ loading: true })
    try {
      const member = await getMemberInfo()
      memberStore.updateMember(member)
      this.setData({ member })
      
      // 加载订单统计（模拟）
      this.loadOrderStats()
    } catch (error) {
      console.error('加载会员信息失败:', error)
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 加载订单统计 */
  async loadOrderStats() {
    // TODO: 从后端获取订单统计
    // 模拟数据
    this.setData({
      orderStats: {
        pending: 2,
        paid: 1,
        shipped: 3,
      },
    })
  },

  /** 登录 */
  onLogin() {
    wx.showToast({ title: '功能开发中', icon: 'none' })
  },

  /** 查看订单列表 */
  onViewOrders(e: WechatMiniprogram.TouchEvent) {
    const { status } = e.currentTarget.dataset
    if (!this.data.isLoggedIn) {
      this.onLogin()
      return
    }
    wx.navigateTo({
      url: `/pages/order/list/list?status=${status || ''}`,
    })
  },

  /** 收货地址 */
  onAddress() {
    if (!this.data.isLoggedIn) {
      this.onLogin()
      return
    }
    wx.navigateTo({
      url: '/pages/address/list/list',
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

  /** 设置 */
  onSettings() {
    wx.showToast({ title: '功能开发中', icon: 'none' })
  },

  /** 退出登录 */
  onLogout() {
    wx.showModal({
      title: '提示',
      content: '确定退出登录吗？',
      confirmText: '退出',
      confirmColor: '#ff5722',
      success: (res) => {
        if (res.confirm) {
          memberStore.logout()
          wx.showToast({ title: '已退出登录', icon: 'success' })
        }
      },
    })
  },
})
