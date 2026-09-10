// api/coupon.ts —— 优惠券相关 API
import { request } from '../utils/request'

const BASE_URL = '/coupon'

/** 优惠券 */
interface Coupon {
  id: string
  name: string
  type: number
  discount: number
  minAmount: number
  startTime: string
  endTime: string
  status: number
}

/** 用户优惠券 */
interface UserCoupon {
  id: string
  couponId: string
  couponName: string
  couponType: number
  discount: number
  minAmount: number
  startTime: string
  endTime: string
  status: number
  useTime?: string
  orderId?: string
}

/** 获取可领取的优惠券列表 */
export function getAvailableCoupons(): Promise<Coupon[]> {
  return request<Coupon[]>({
    url: `${BASE_URL}/available`,
    method: 'GET',
  })
}

/** 领取优惠券 */
export function claimCoupon(couponId: string): Promise<string> {
  return request<string>({
    url: `${BASE_URL}/${couponId}/claim`,
    method: 'POST',
  })
}

/** 获取我的优惠券列表 */
export function getMyCoupons(status?: number): Promise<UserCoupon[]> {
  return request<UserCoupon[]>({
    url: `${BASE_URL}/my`,
    method: 'GET',
    data: { status },
  })
}

/** 获取用户优惠券详情 */
export function getUserCouponDetail(id: string): Promise<UserCoupon> {
  return request<UserCoupon>({
    url: `${BASE_URL}/user/${id}`,
    method: 'GET',
  })
}