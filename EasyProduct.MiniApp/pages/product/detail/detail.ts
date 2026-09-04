// pages/product/detail/detail.ts —— 商品详情页
import { t } from '../../../utils/i18n'
import { getProductDetail } from '../../../api/product'
import { cartStore } from '../../../stores/cart.store'
import type { ProductDetail, SpecGroup, SpecOption } from '../../../types/product.types'

interface ProductDetailPageData {
  product: ProductDetail | null
  currentPrice: number
  currentStock: number
  selectedSpecs: Record<string, string>
  quantity: number
  showSpecPopup: boolean
  isAddingToCart: boolean
  isBuying: boolean
}

Page<ProductDetailPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    product: null,
    currentPrice: 0,
    currentStock: 0,
    selectedSpecs: {},
    quantity: 1,
    showSpecPopup: false,
    isAddingToCart: false,
    isBuying: false,
  },

  async onLoad(options) {
    const { id } = options
    if (id) {
      await this.loadProductDetail(id)
    }
  },

  /** 加载商品详情 */
  async loadProductDetail(id: string) {
    wx.showLoading({ title: t('common.loading') })
    try {
      const product = await getProductDetail(id)
      this.setData({
        product,
        currentPrice: product.price,
        currentStock: product.stock,
      })
    } catch (error) {
      console.error('加载商品详情失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    } finally {
      wx.hideLoading()
    }
  },

  /** 选择规格 */
  onSpecSelect(e: WechatMiniprogram.TouchEvent) {
    const { groupId, optionId } = e.currentTarget.dataset
    const { selectedSpecs, product } = this.data

    if (!product) return

    const newSelectedSpecs = { ...selectedSpecs, [groupId]: optionId }
    this.setData({ selectedSpecs: newSelectedSpecs })

    // 计算当前价格和库存（如果有SKU系统）
    this.calculatePriceAndStock(newSelectedSpecs)
  },

  /** 计算价格和库存 */
  calculatePriceAndStock(selectedSpecs: Record<string, string>) {
    const { product } = this.data
    if (!product || !product.specs) return

    // 检查是否选完所有必填规格
    const requiredSpecs = product.specs.filter(s => s.required)
    const hasSelectedAll = requiredSpecs.every(s => selectedSpecs[s.id])

    if (hasSelectedAll) {
      // 这里可以根据 selectedSpecs 查找对应的 SKU
      // 简化处理：使用基础价格
      this.setData({ currentPrice: product.price })
    }
  },

  /** 数量减少 */
  onQuantityMinus() {
    const { quantity } = this.data
    if (quantity > 1) {
      this.setData({ quantity: quantity - 1 })
    }
  },

  /** 数量增加 */
  onQuantityPlus() {
    const { quantity, currentStock } = this.data
    if (quantity < currentStock) {
      this.setData({ quantity: quantity + 1 })
    } else {
      wx.showToast({ title: t('common.product.noStock'), icon: 'none' })
    }
  },

  /** 显示规格选择弹窗 */
  showSpecPopup(e: WechatMiniprogram.TouchEvent) {
    const { type } = e.currentTarget.dataset
    this.setData({
      showSpecPopup: true,
      isBuying: type === 'buy',
      isAddingToCart: type === 'cart',
    })
  },

  /** 关闭规格选择弹窗 */
  closeSpecPopup() {
    this.setData({ showSpecPopup: false })
  },

  /** 确认规格选择 */
  async onSpecConfirm() {
    const { product, selectedSpecs, quantity } = this.data
    if (!product) return

    // 检查必填规格
    if (product.specs) {
      const requiredSpecs = product.specs.filter(s => s.required)
      const hasSelectedAll = requiredSpecs.every(s => selectedSpecs[s.id])
      if (!hasSelectedAll) {
        const unselectedSpec = requiredSpecs.find(s => !selectedSpecs[s.id])
        wx.showToast({ title: '请选择' + (unselectedSpec?.name || '规格'), icon: 'none' })
        return
      }
    }

    this.closeSpecPopup()

    if (this.data.isAddingToCart) {
      await this.addToCart()
    } else if (this.data.isBuying) {
      await this.buyNow()
    }
  },

  /** 加入购物车 */
  async addToCart() {
    const { product, quantity } = this.data
    if (!product) return

    try {
      await cartStore.addToCart(product.id, quantity)
      wx.showToast({ title: t('common.product.addToCartSuccess'), icon: 'success' })
      this.updateCartBadge()
    } catch (error) {
      console.error('加入购物车失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    }
  },

  /** 立即购买 */
  async buyNow() {
    const { product, quantity } = this.data
    if (!product) return

    try {
      // 先加入购物车
      await cartStore.addToCart(product.id, quantity)
      this.updateCartBadge()

      // 获取购物车中选中的商品ID列表（这里简化处理）
      const selectedItems = cartStore.getSelectedItems()
      const cartItemIds = selectedItems.map(item => item.id)

      if (cartItemIds.length === 0) {
        wx.showToast({ title: '请添加商品到购物车', icon: 'none' })
        return
      }

      // 跳转到结算页
      wx.navigateTo({
        url: `/pages/pay/checkout/checkout?cartItemIds=${cartItemIds.join(',')}`,
      })
    } catch (error) {
      console.error('立即购买失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    }
  },

  /** 更新购物车 Badge */
  updateCartBadge() {
    const count = cartStore.getState().totalCount
    if (count > 0) {
      wx.setTabBarBadge({ index: 2, text: String(count > 99 ? '99+' : count) })
    } else {
      wx.removeTabBarBadge({ index: 2 })
    }
  },

  /** 预览图片 */
  previewImage(e: WechatMiniprogram.TouchEvent) {
    const { urls, current } = e.currentTarget.dataset
    wx.previewImage({
      urls,
      current,
    })
  },

  /** 阻止事件冒泡 */
  preventClose() {
    // 阻止关闭弹窗
  },
})
