// src/types/mall/address.types.ts
// 会员收货地址类型定义
// 约束:禁用 TS enum,使用字符串联合类型 + as const 常量对象
// 主键:GUID 字符串;JSON camelCase;分页 pageIndex/pageSize,出参 list/total

import type { PageQuery, PageResult } from '../api'

/** 收货地址实体 */
export interface Address {
  id: string
  memberId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
  fullAddress: string
  createdAt: string
  updatedAt: string
}

/** 地址列表项(含会员信息) */
export interface AddressListItem extends Address {
  memberName: string
  memberPhone: string
}

/** 地址查询参数 */
export interface AddressQuery extends PageQuery {
  memberId?: string
  memberName?: string
  phone?: string
  province?: string
  city?: string
  keyword?: string
}

/** 地址分页结果 */
export type AddressPageResult = PageResult<AddressListItem>

/** 地址表单参数(新增/更新共用) */
export interface AddressParams {
  id?: string
  memberId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault?: boolean
}

/** 地址表单验证规则 */
export const ADDRESS_RULES = {
  memberId: { required: true, message: 'mall.address.memberIdRequired' },
  name: { required: true, message: 'mall.address.nameRequired', max: 50 },
  phone: { required: true, message: 'mall.address.phoneRequired', pattern: /^1[3-9]\d{9}$/ },
  province: { required: true, message: 'mall.address.provinceRequired' },
  city: { required: true, message: 'mall.address.cityRequired' },
  district: { required: true, message: 'mall.address.districtRequired' },
  detail: { required: true, message: 'mall.address.detailRequired', max: 200 },
} as const
