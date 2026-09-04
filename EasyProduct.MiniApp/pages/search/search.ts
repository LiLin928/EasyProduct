// pages/search/search.ts —— 商品搜索页
import { t } from '../../utils/i18n'
import { searchProducts, getProductList } from '../../api/product'
import type { Product } from '../../types/product.types'

interface SearchPageData {
  keyword: string
  products: Product[]
  loading: boolean
  hasMore: boolean
  pageIndex: number
  pageSize: number
  history: string[]
  hotKeywords: string[]
}

const HOT_KEYWORDS = ['手机', '电脑', '耳机', '手表', '键盘', '鼠标']

Page<SearchPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    keyword: '',
    products: [],
    loading: false,
    hasMore: true,
    pageIndex: 1,
    pageSize: 10,
    history: [],
    hotKeywords: HOT_KEYWORDS,
  },

  onLoad() {
    this.loadSearchHistory()
  },

  /** 加载搜索历史 */
  loadSearchHistory() {
    try {
      const history = wx.getStorageSync('searchHistory')
      if (history) {
        this.setData({ history: JSON.parse(history) })
      }
    } catch (error) {
      console.error('加载搜索历史失败:', error)
    }
  },

  /** 保存搜索历史 */
  saveSearchHistory(keyword: string) {
    if (!keyword.trim()) return
    
    const { history } = this.data
    // 去重并限制数量
    const newHistory = [keyword, ...history.filter(k => k !== keyword)].slice(0, 10)
    
    this.setData({ history: newHistory })
    try {
      wx.setStorageSync('searchHistory', JSON.stringify(newHistory))
    } catch (error) {
      console.error('保存搜索历史失败:', error)
    }
  },

  /** 输入框变化 */
  onKeywordInput(e: WechatMiniprogram.Input) {
    this.setData({ keyword: e.detail.value })
  },

  /** 搜索 */
  async onSearch() {
    const { keyword } = this.data
    if (!keyword.trim()) {
      wx.showToast({ title: '请输入搜索关键词', icon: 'none' })
      return
    }
    
    // 保存搜索历史
    this.saveSearchHistory(keyword)
    
    // 重置分页
    this.setData({
      products: [],
      pageIndex: 1,
      hasMore: true,
    })
    
    await this.loadProducts()
  },

  /** 点击搜索历史或热门关键词 */
  onKeywordTap(e: WechatMiniprogram.TouchEvent) {
    const { keyword } = e.currentTarget.dataset
    this.setData({ keyword })
    this.saveSearchHistory(keyword)
    
    // 重置分页
    this.setData({
      products: [],
      pageIndex: 1,
      hasMore: true,
    })
    
    this.loadProducts()
  },

  /** 清空搜索 */
  onClear() {
    this.setData({
      keyword: '',
      products: [],
    })
  },

  /** 清空历史 */
  onClearHistory() {
    wx.showModal({
      title: '提示',
      content: '确定清空搜索历史吗？',
      success: (res) => {
        if (res.confirm) {
          this.setData({ history: [] })
          wx.removeStorageSync('searchHistory')
        }
      },
    })
  },

  /** 加载商品 */
  async loadProducts() {
    const { keyword, loading, hasMore } = this.data
    if (loading || !hasMore) return

    this.setData({ loading: true })
    try {
      // 使用搜索API
      const products = await searchProducts(keyword)
      
      this.setData({
        products: products,
        hasMore: false, // 搜索API没有分页
      })
    } catch (error) {
      console.error('搜索商品失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 点击商品 */
  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({
      url: `/pages/product/detail/detail?id=${id}`,
    })
  },

  /** 返回首页 */
  onGoHome() {
    wx.switchTab({
      url: '/pages/index/index',
    })
  },
})
