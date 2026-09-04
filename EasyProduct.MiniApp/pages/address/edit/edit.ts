// pages/address/edit/edit.ts —— 地址编辑页
import { t } from '../../../utils/i18n'
import { getAddressDetail, saveAddress } from '../../../api/address'
import type { Address } from '../../../types/address.types'

interface AddressEditPageData {
  id: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
  loading: boolean
  isNew: boolean
  regionArray: string[][]
  regionIndex: number[]
}

// 省市区的静态数据（实际项目中应该从后端获取）
const PROVINCES = ['北京市', '上海市', '广东省', '浙江省', '江苏省']

const CITIES: Record<string, string[]> = {
  '北京市': ['北京市'],
  '上海市': ['上海市'],
  '广东省': ['广州市', '深圳市', '东莞市', '佛山市'],
  '浙江省': ['杭州市', '宁波市', '温州市'],
  '江苏省': ['南京市', '苏州市', '无锡市'],
}

const DISTRICTS: Record<string, string[]> = {
  '北京市': ['朝阳区', '海淀区', '东城区', '西城区'],
  '上海市': ['浦东新区', '黄浦区', '静安区', '徐汇区'],
  '广州市': ['天河区', '越秀区', '海珠区', '白云区'],
  '深圳市': ['南山区', '福田区', '罗湖区', '宝安区'],
  '东莞市': ['南城街道', '东城街道', '莞城街道'],
  '佛山市': ['禅城区', '南海区', '顺德区'],
  '杭州市': ['西湖区', '上城区', '下城区', '拱墅区'],
  '宁波市': ['海曙区', '江北区', '鄞州区'],
  '温州市': ['鹿城区', '龙湾区', '瓯海区'],
  '南京市': ['鼓楼区', '玄武区', '秦淮区', '建邺区'],
  '苏州市': ['姑苏区', '虎丘区', '吴中区'],
  '无锡市': ['梁溪区', '滨湖区', '锡山区'],
}

Page<AddressEditPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    id: '',
    name: '',
    phone: '',
    province: '',
    city: '',
    district: '',
    detail: '',
    isDefault: false,
    loading: false,
    isNew: true,
    regionArray: [PROVINCES, CITIES['北京市'], DISTRICTS['北京市']],
    regionIndex: [0, 0, 0],
  },

  onLoad(options) {
    const { id } = options
    if (id) {
      this.setData({ isNew: false, id })
      this.loadAddressDetail(id)
    }
  },

  /** 加载地址详情 */
  async loadAddressDetail(id: string) {
    this.setData({ loading: true })
    try {
      const address = await getAddressDetail(id)
      const { province, city, district } = address
      
      // 设置地区选择器
      const provinceIndex = PROVINCES.indexOf(province)
      const cityList = CITIES[province] || CITIES['北京市']
      const cityIndex = cityList.indexOf(city)
      const districtList = DISTRICTS[city] || DISTRICTS['北京市']
      const districtIndex = districtList.indexOf(district)
      
      this.setData({
        id: address.id,
        name: address.name,
        phone: address.phone,
        province,
        city,
        district,
        detail: address.detail,
        isDefault: address.isDefault,
        regionArray: [PROVINCES, cityList, districtList],
        regionIndex: [Math.max(0, provinceIndex), Math.max(0, cityIndex), Math.max(0, districtIndex)],
      })
    } catch (error) {
      console.error('加载地址详情失败:', error)
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 输入框变化处理 */
  onInputChange(e: WechatMiniprogram.Input) {
    const { field } = e.currentTarget.dataset as { field: string }
    this.setData({ [field]: e.detail.value })
  },

  /** 切换默认地址 */
  onDefaultChange(e: WechatMiniprogram.SwitchChange) {
    this.setData({ isDefault: e.detail.value })
  },

  /** 地区选择器变化 */
  onRegionChange(e: WechatMiniprogram.PickerChange) {
    const [provinceIndex, cityIndex, districtIndex] = e.detail.value as number[]
    const province = PROVINCES[provinceIndex]
    const cityList = CITIES[province]
    const city = cityList[cityIndex]
    const districtList = DISTRICTS[city]
    const district = districtList[districtIndex]
    
    this.setData({
      province,
      city,
      district,
      regionIndex: [provinceIndex, cityIndex, districtIndex],
    })
  },

  /** 地区选择器列变化 */
  onRegionColumnChange(e: WechatMiniprogram.PickerColumnChange) {
    const { column, value } = e.detail
    const { regionIndex } = this.data
    
    if (column === 0) {
      // 省变化，更新市和区
      const province = PROVINCES[value]
      const cities = CITIES[province]
      const districts = DISTRICTS[cities[0]]
      this.setData({
        regionArray: [PROVINCES, cities, districts],
        regionIndex: [value, 0, 0],
      })
    } else if (column === 1) {
      // 市变化，更新区
      const province = PROVINCES[regionIndex[0]]
      const cities = CITIES[province]
      const city = cities[value]
      const districts = DISTRICTS[city]
      this.setData({
        regionArray: [PROVINCES, cities, districts],
        regionIndex: [regionIndex[0], value, 0],
      })
    }
  },

  /** 验证表单 */
  validateForm(): boolean {
    const { name, phone, province, city, district, detail } = this.data
    
    if (!name.trim()) {
      wx.showToast({ title: '请输入收货人姓名', icon: 'none' })
      return false
    }
    if (!phone.trim()) {
      wx.showToast({ title: '请输入联系电话', icon: 'none' })
      return false
    }
    if (!/^1[3-9]\d{9}$/.test(phone)) {
      wx.showToast({ title: '请输入正确的手机号', icon: 'none' })
      return false
    }
    if (!province || !city || !district) {
      wx.showToast({ title: '请选择所在地区', icon: 'none' })
      return false
    }
    if (!detail.trim()) {
      wx.showToast({ title: '请输入详细地址', icon: 'none' })
      return false
    }
    return true
  },

  /** 保存地址 */
  async onSave() {
    if (!this.validateForm()) return
    
    const { id, name, phone, province, city, district, detail, isDefault, isNew } = this.data
    
    this.setData({ loading: true })
    try {
      const addressData: Address = {
        id,
        name,
        phone,
        province,
        city,
        district,
        detail,
        isDefault,
      }
      
      await saveAddress(addressData)
      wx.showToast({ title: isNew ? '添加成功' : '保存成功', icon: 'success' })
      setTimeout(() => {
        wx.navigateBack()
      }, 1000)
    } catch (error) {
      console.error('保存地址失败:', error)
      wx.showToast({ title: '保存失败', icon: 'none' })
    } finally {
      this.setData({ loading: false })
    }
  },

  /** 取消 */
  onCancel() {
    wx.navigateBack()
  },
})
