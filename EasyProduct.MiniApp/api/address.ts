// api/address.ts —— 收货地址相关 API
import { request } from '../utils/request'
import type { Address } from '../types/address.types'

const BASE_URL = '/addresses'

/** 获取地址列表 */
export function getAddressList(): Promise<Address[]> {
  return request<Address[]>({
    url: BASE_URL,
    method: 'GET',
  })
}

/** 获取地址详情
 * @param id 地址ID
 */
export function getAddressDetail(id: string): Promise<Address> {
  return request<Address>({
    url: `${BASE_URL}/${id}`,
    method: 'GET',
  })
}

/** 保存地址（新增或更新）
 * @param data 地址数据
 */
export function saveAddress(data: Address): Promise<Address> {
  if (data.id) {
    return request<Address>({
      url: `${BASE_URL}/${data.id}`,
      method: 'PUT',
      data,
    })
  }
  return request<Address>({
    url: BASE_URL,
    method: 'POST',
    data,
  })
}

/** 删除地址
 * @param id 地址ID
 */
export function deleteAddress(id: string): Promise<void> {
  return request<void>({
    url: `${BASE_URL}/${id}`,
    method: 'DELETE',
  })
}

/** 设置默认地址
 * @param id 地址ID
 */
export function setDefaultAddress(id: string): Promise<void> {
  return request<void>({
    url: `${BASE_URL}/${id}/default`,
    method: 'PUT',
  })
}
