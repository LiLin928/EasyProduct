// pages/address/list/list.ts —— 地址列表页
import { t } from '../../../utils/i18n'
import { getAddressList, deleteAddress, setDefaultAddress } from '../../../api/address'
import type { Address } from '../../../types/address.types'

interface AddressListPageData {
  addresses: Address[]
  loading: boolean
  selectedId: string
  isFromCheckout: boolean
}

Page<AddressListPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    addresses: [],
    loading: false,
    selectedId: '',
    isFromCheckout: false,
  },

  onLoad(options) {
    // 判断是否从结算页进入
    const { from, selectedId } = options
    this.setData({
      isFromCheckout: from === 'checkout',
      selectedId: selectedId || '',
    })
  },

  onShow() {
    this.loadAddresses()
  },

  /** 加载地址列表 */
  async loadAddresses() {
    this.setData({ loading: true })
    try {
      const addresses = await getAddressList()
      // 按默认地址排序
      const sorted = addresses.sort((a, b) => (b.isDefault ? 1 : 0) - (a.isDefault ? 1 : 0))
      this.setData({ addresses: sorted })
    } catch (error) {
      console.error('加载地址失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 点击地址（选择或查看） */
  onAddressTap(e: WechatMiniprogram.TouchEvent) {
    const { item } = e.currentTarget.dataset as { item: Address }
    const { isFromCheckout } = this.data

    if (isFromCheckout) {
      // 从结算页进入，返回选中的地址
      const eventChannel = this.getOpenerEventChannel()
      eventChannel.emit('selectAddress', item)
      wx.navigateBack()
    } else {
      // 正常查看，跳转到编辑页
      wx.navigateTo({
        url: `/pages/address/edit/edit?id=${item.id}`,
      })
    }
  },

  /** 新增地址 */
  onAddAddress() {
    wx.navigateTo({
      url: '/pages/address/edit/edit',
    })
  },

  /** 编辑地址 */
  onEdit(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({
      url: `/pages/address/edit/edit?id=${id}`,
    })
  },

  /** 删除地址 */
  onDelete(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.showModal({
      title: '提示',
      content: t('common.address.deleteConfirm'),
      confirmText: t('common.button.delete'),
      confirmColor: '#ff5722',
      success: async (res) => {
        if (res.confirm) {
          try {
            await deleteAddress(id)
            wx.showToast({ title: '删除成功', icon: 'success' })
            this.loadAddresses()
          } catch (error) {
            console.error('删除地址失败:', error)
            wx.showToast({ title: '删除失败', icon: 'none' })
          }
        }
      },
    })
  },

  /** 设为默认 */
  async onSetDefault(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    try {
      await setDefaultAddress(id)
      wx.showToast({ title: '设置成功', icon: 'success' })
      this.loadAddresses()
    } catch (error) {
      console.error('设置默认地址失败:', error)
      wx.showToast({ title: '设置失败', icon: 'none' })
    }
  },
})
