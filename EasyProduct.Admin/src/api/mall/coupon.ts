import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Coupon, CouponQuery, CouponParams } from '@/types/mall'

/** 优惠券列表（分页 + 筛选） */
export const getCouponList = (params: CouponQuery) =>
  get<PageResult<Coupon>>('/api/admin/mall/coupon/list', params)

/** 优惠券详情 */
export const getCouponDetail = (id: string) =>
  get<Coupon>(`/api/admin/mall/coupon/${id}`)

/** 新建优惠券 */
export const createCoupon = (data: CouponParams) =>
  post<{ id: string }>('/api/admin/mall/coupon', data)

/** 更新优惠券 */
export const updateCoupon = (id: string, data: CouponParams) =>
  put<null>(`/api/admin/mall/coupon/${id}`, data)

/** 删除优惠券 */
export const deleteCoupon = (id: string) =>
  del<null>(`/api/admin/mall/coupon/${id}`)
