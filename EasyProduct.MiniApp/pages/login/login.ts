// pages/login/login.ts - 登录页面
import { ENV } from '../../config/env'
import { setMemberToken, setMemberInfo } from '../../utils/storage'
import type { ApiResponse } from '../../types/api.types'

interface LoginData {
  phone: string
  password: string
}

interface LoginResult {
  token: string
  memberId: string
  openId: string
  nickname?: string
  avatar?: string
}

Page({
  data: {
    phone: '',
    password: '',
    agreeTerms: false,
    isPhoneValid: false,
    isPasswordValid: false,
    loading: false,
    errorMsg: '',
    fromPage: '',
    redirectUrl: '',
  },

  onLoad(options: { from?: string; redirect?: string }) {
    this.setData({
      fromPage: options.from || '',
      redirectUrl: options.redirect || '',
    })
  },

  // 手机号输入
  onPhoneInput(e: WechatMiniprogram.Input) {
    const phone = e.detail.value
    const isPhoneValid = /^1[3-9]\d{9}$/.test(phone)
    this.setData({ phone, isPhoneValid, errorMsg: '' })
  },

  // 密码输入
  onPasswordInput(e: WechatMiniprogram.Input) {
    const password = e.detail.value
    const isPasswordValid = password.length >= 6
    this.setData({ password, isPasswordValid, errorMsg: '' })
  },

  // 清除手机号
  clearPhone() {
    this.setData({ phone: '', isPhoneValid: false })
  },

  // 清除密码
  clearPassword() {
    this.setData({ password: '', isPasswordValid: false })
  },

  // 切换协议同意状态
  toggleTerms() {
    this.setData({ agreeTerms: !this.data.agreeTerms })
  },

  // 手机号密码登录
  async onPhoneLogin() {
    if (this.data.loading) return
    
    const { phone, password, agreeTerms } = this.data
    
    // 校验
    if (!phone) {
      this.setData({ errorMsg: '请输入手机号' })
      return
    }
    if (!/^1[3-9]\d{9}$/.test(phone)) {
      this.setData({ errorMsg: '手机号格式不正确' })
      return
    }
    if (!password) {
      this.setData({ errorMsg: '请输入密码' })
      return
    }
    if (password.length < 6) {
      this.setData({ errorMsg: '密码长度至少6位' })
      return
    }
    if (!agreeTerms) {
      this.setData({ errorMsg: '请先同意用户协议和隐私政策' })
      return
    }

    this.setData({ loading: true, errorMsg: '' })

    try {
      const res = await new Promise<WechatMiniprogram.RequestSuccessCallbackResult>((resolve, reject) => {
        wx.request({
          url: `${ENV.apiBase}/auth/login`,
          method: 'POST',
          data: { phone, password },
          success: resolve,
          fail: reject,
        })
      })

      const envelope = res.data as ApiResponse<LoginResult>
      
      if (envelope.code !== 200) {
        this.setData({ errorMsg: envelope.message || '登录失败', loading: false })
        return
      }

      // 保存登录信息
      setMemberToken(envelope.data.token)
      setMemberInfo({
        id: envelope.data.memberId,
        nickName: envelope.data.nickname || '微信用户',
        avatar: envelope.data.avatar || '',
        level: '',
        points: 0,
      })

      wx.showToast({ title: '登录成功', icon: 'success' })
      
      // 延迟跳转到目标页面
      setTimeout(() => {
        this.navigateBackOrHome()
      }, 800)
    } catch (err) {
      this.setData({ errorMsg: '网络错误，请重试', loading: false })
    }
  },

  // 微信一键登录
  async onWxLogin() {
    if (this.data.loading) return
    if (!this.data.agreeTerms) {
      this.setData({ errorMsg: '请先同意用户协议和隐私政策' })
      return
    }

    this.setData({ loading: true, errorMsg: '' })

    try {
      // 获取微信登录 code
      const { code } = await wx.login()
      
      const res = await new Promise<WechatMiniprogram.RequestSuccessCallbackResult>((resolve, reject) => {
        wx.request({
          url: `${ENV.apiBase}/auth/wx-login`,
          method: 'POST',
          data: { code },
          success: resolve,
          fail: reject,
        })
      })

      const envelope = res.data as ApiResponse<LoginResult>
      
      if (envelope.code !== 200) {
        this.setData({ errorMsg: envelope.message || '登录失败', loading: false })
        return
      }

      // 保存登录信息
      setMemberToken(envelope.data.token)
      setMemberInfo({
        id: envelope.data.memberId,
        nickName: envelope.data.nickname || '微信用户',
        avatar: envelope.data.avatar || '',
        level: '',
        points: 0,
      })

      wx.showToast({ title: '登录成功', icon: 'success' })
      
      setTimeout(() => {
        this.navigateBackOrHome()
      }, 800)
    } catch (err) {
      this.setData({ errorMsg: '网络错误，请重试', loading: false })
    }
  },

  // 返回或跳转到首页
  navigateBackOrHome() {
    const { fromPage, redirectUrl } = this.data
    
    if (redirectUrl) {
      // 如果有指定跳转URL
      wx.navigateTo({ url: redirectUrl })
    } else if (fromPage) {
      // 返回来源页面
      wx.navigateBack({ delta: 1 })
    } else {
      // 跳转到首页
      wx.switchTab({ url: '/pages/index/index' })
    }
  },

  // 返回上一页
  onBack() {
    wx.navigateBack({ delta: 1 })
  },

  // 跳转到注册页面（预留）
  goToRegister() {
    wx.showToast({ title: '注册功能开发中', icon: 'none' })
  },

  // 跳转到找回密码页面（预留）
  goToForgot() {
    wx.showToast({ title: '找回密码功能开发中', icon: 'none' })
  },

  // 查看用户协议
  onViewTerms() {
    wx.showModal({
      title: '用户协议',
      content: '这里是用户协议内容...',
      showCancel: false,
    })
  },

  // 查看隐私政策
  onViewPrivacy() {
    wx.showModal({
      title: '隐私政策',
      content: '这里是隐私政策内容...',
      showCancel: false,
    })
  },
})
