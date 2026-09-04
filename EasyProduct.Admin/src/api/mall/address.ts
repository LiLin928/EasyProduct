// src/api/mall/address.ts
// 会员收货地址管理 API 层
// 路由:/api/admin/mall/address/**

import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { AddressListItem, AddressQuery, AddressParams } from '@/types/mall/address.types'

/** 地址模块基础路径 */
const BASE_URL = '/api/admin/mall/address'

/**
 * 分页查询会员收货地址列表
 * GET /api/admin/mall/address/list
 */
export const getAddressList = (params: AddressQuery) =>
  get<PageResult<AddressListItem>>(`${BASE_URL}/list`, params)

/**
 * 获取单个收货地址详情
 * GET /api/admin/mall/address/:id
 */
export const getAddressDetail = (id: string) =>
  get<AddressListItem>(`${BASE_URL}/${id}`)

/**
 * 新增会员收货地址
 * POST /api/admin/mall/address
 */
export const createAddress = (data: AddressParams) =>
  post<AddressListItem>(BASE_URL, data)

/**
 * 更新会员收货地址
 * PUT /api/admin/mall/address/:id
 */
export const updateAddress = (id: string, data: AddressParams) =>
  put<AddressListItem>(`${BASE_URL}/${id}`, data)

/**
 * 删除会员收货地址
 * DELETE /api/admin/mall/address/:id
 */
export const deleteAddress = (id: string) =>
  del<null>(`${BASE_URL}/${id}`)

/**
 * 设置默认收货地址
 * PUT /api/admin/mall/address/:id/default
 */
export const setDefaultAddress = (id: string) =>
  put<AddressListItem>(`${BASE_URL}/${id}/default`)
