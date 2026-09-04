// types/address.types.ts —— 收货地址相关类型定义

/** 地址信息 */
export interface Address {
  id: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
  fullAddress?: string
}
