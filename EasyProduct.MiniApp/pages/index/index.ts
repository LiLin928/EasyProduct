// pages/index/index.ts —— 首页
import { t } from '../../utils/i18n'
import { getCategories, getHotProducts, getNewProducts } from '../../api/product'
import { cartStore } from '../../stores/cart.store'
import type { Category, Product } from '../../types/product.types'

interface IndexPageData {
  bannerList: string[]
  categories: Category[]
  hotProducts: Product[]
  newProducts: Product[]
  loading: boolean
  cartBadge: number
}

Page<IndexPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    bannerList: [],
    categories: [],
    hotProducts: [],
    newProducts: [],
    loading: true,
    cartBadge: 0,
  },

  onLoad() {
    this.loadData()
    this.updateCartBadge()
  },

  onShow() {
    this.updateCartBadge()
  },

  /** 加载首页数据 */
  async loadData() {
    this.setData({ loading: true })

    try {
      // Banner 数据（使用静态数据或从配置获取）
      const bannerList = [
        'https://picsum.photos/750/300?random=1',
        'https://picsum.photos/750/300?random=2',
        'https://picsum.photos/750/300?random=3',
      ]

      // 并行获取分类、热销、新品
      const [categories, hotProducts, newProducts] = await Promise.all([
        getCategories(),
        getHotProducts(6),
        getNewProducts(6),
      ])

      this.setData({
        bannerList,
        categories,
        hotProducts,
        newProducts,
        loading: false,
      })
    } catch (error) {
      console.error('加载首页数据失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
      this.setData({ loading: false })
    }
  },

  /** 更新购物车 Badge */
  updateCartBadge() {
    const count = cartStore.getState().totalCount
    this.setData({ cartBadge: count })

    if (count > 0) {
      wx.setTabBarBadge({ index: 2, text: String(count > 99 ? '99+' : count) })
    } else {
      wx.removeTabBarBadge({ index: 2 })
    }
  },

  /** 点击搜索 */
  onSearchTap() {
    wx.navigateTo({ url: '/pages/search/search' })
  },

  /** 点击 Banner */
  onBannerTap(e: WechatMiniprogram.TouchEvent) {
    const { index } = e.currentTarget.dataset
    console.log('点击 Banner:', index)
    // 根据 index 跳转不同页面
  },

  /** 点击分类 */
  onCategoryTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    // 设置全局选中分类，跳转到分类页
    const app = getApp()
    if (app) {
      (app as any).globalData = (app as any).globalData || {}
      ;(app as any).globalData.selectedCategoryId = id
    }
    wx.switchTab({ url: '/pages/category/category' })
  },

  /** 查看更多热销 */
  onViewMoreHot() {
    wx.switchTab({ url: '/pages/category/category' })
  },

  /** 点击商品 */
  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({ url: `/pages/product/detail/detail?id=${id}` })
  },

  /** 下拉刷新 */
  async onPullDownRefresh() {
    await this.loadData()
    wx.stopPullDownRefresh()
  },

  /** 分享给朋友 */
  onShareAppMessage() {
    return {
      title: t('common.app.name'),
      path: '/pages/index/index',
    }
  },
})
