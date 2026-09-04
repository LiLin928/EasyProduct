// pages/category/category.ts —— 分类页
import { t } from '../../utils/i18n'
import { getCategories, getProductList } from '../../api/product'
import { cartStore } from '../../stores/cart.store'
import type { Category, Product, ProductQuery } from '../../types/product.types'

interface CategoryPageData {
  categories: Category[]
  currentCategoryId: string
  products: Product[]
  loading: boolean
  hasMore: boolean
  pageIndex: number
  sortField: 'price' | 'sales' | 'createdAt'
  sortOrder: 'asc' | 'desc'
  cartBadge: number
}

Page<CategoryPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    categories: [],
    currentCategoryId: '',
    products: [],
    loading: false,
    hasMore: true,
    pageIndex: 1,
    sortField: 'createdAt',
    sortOrder: 'desc',
    cartBadge: 0,
  },

  async onLoad() {
    await this.loadCategories()
    this.updateCartBadge()
  },

  onShow() {
    this.updateCartBadge()
  },

  /** 加载分类列表 */
  async loadCategories() {
    try {
      const categories = await getCategories()
      // 如果全局有选中分类，使用它，否则使用第一个
      const app = getApp()
      let selectedId = ''
      if (app && (app as any).globalData?.selectedCategoryId) {
        selectedId = (app as any).globalData.selectedCategoryId
        ;(app as any).globalData.selectedCategoryId = '' // 清除
      }

      const currentCategoryId = selectedId || categories[0]?.id || ''

      this.setData({
        categories,
        currentCategoryId,
      })

      if (currentCategoryId) {
        await this.loadProducts(true)
      }
    } catch (error) {
      console.error('加载分类失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    }
  },

  /** 加载商品列表 */
  async loadProducts(reset = false) {
    const { currentCategoryId, pageIndex, sortField, sortOrder, products } = this.data
    if (!currentCategoryId) return

    const currentPage = reset ? 1 : pageIndex

    this.setData({ loading: true })

    try {
      const params: ProductQuery = {
        pageIndex: currentPage,
        pageSize: 10,
        categoryId: currentCategoryId,
        sortField,
        sortOrder,
      }

      const result = await getProductList(params)
      const newProducts = reset ? result.list : [...products, ...result.list]

      this.setData({
        products: newProducts,
        pageIndex: currentPage + 1,
        hasMore: newProducts.length < result.total,
        loading: false,
      })
    } catch (error) {
      console.error('加载商品失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
      this.setData({ loading: false })
    }
  },

  /** 切换分类 */
  onCategoryChange(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    this.setData({
      currentCategoryId: id,
      products: [],
      pageIndex: 1,
      hasMore: true,
    })
    this.loadProducts(true)
  },

  /** 切换排序 */
  onSortChange(e: WechatMiniprogram.TouchEvent) {
    const { field } = e.currentTarget.dataset
    const { sortField, sortOrder } = this.data

    let newOrder: 'asc' | 'desc' = 'desc'
    if (sortField === field) {
      newOrder = sortOrder === 'asc' ? 'desc' : 'asc'
    }

    this.setData({
      sortField: field,
      sortOrder: newOrder,
      products: [],
      pageIndex: 1,
      hasMore: true,
    })
    this.loadProducts(true)
  },

  /** 点击商品 */
  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({ url: `/pages/product/detail/detail?id=${id}` })
  },

  /** 上拉加载更多 */
  onReachBottom() {
    if (this.data.hasMore && !this.data.loading) {
      this.loadProducts()
    }
  },

  /** 下拉刷新 */
  async onPullDownRefresh() {
    await this.loadProducts(true)
    wx.stopPullDownRefresh()
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
})
