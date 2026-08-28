// src/types/mall.ts

import type { PageQuery } from './api'

// ---- Member Level ----

export type LevelStatus = 'enabled' | 'disabled'

export interface MemberLevel {
  id: string
  name: string
  minPoints: number
  discount: number
  sort: number
  status: LevelStatus
  createdAt: string
  updatedAt: string
}

export interface LevelQuery extends PageQuery {
  name?: string
  status?: LevelStatus
}

export interface LevelParams {
  name: string
  minPoints?: number
  discount?: number
  sort?: number
  status?: LevelStatus
}

export const LEVEL_STATUS_OPTIONS = [
  { label: 'mall.level.statusEnabled', value: 'enabled' as const },
  { label: 'mall.level.statusDisabled', value: 'disabled' as const },
]

// ---- Member ----

export type MemberStatus = 'active' | 'inactive'

export interface Member {
  id: string
  nickname: string
  avatar: string
  phone: string
  openid: string
  levelId: string
  levelName: string
  points: number
  totalSpent: number
  orderCount: number
  status: MemberStatus
  createdAt: string
  updatedAt: string
}

export interface MemberQuery extends PageQuery {
  nickname?: string
  phone?: string
  levelId?: string
  status?: MemberStatus
}

export interface MemberParams {
  nickname: string
  phone?: string
  levelId?: string
  status?: MemberStatus
}

export const MEMBER_STATUS_OPTIONS = [
  { label: 'mall.member.statusActive', value: 'active' as const },
  { label: 'mall.member.statusInactive', value: 'inactive' as const },
]

// ---- Points Record ----

export type PointsType = 'earn' | 'spend'
export type PointsSource = 'order' | 'signin' | 'activity' | 'refund' | 'adjust'

export interface PointsRecord {
  id: string
  memberId: string
  memberName: string
  type: PointsType
  amount: number
  source: PointsSource
  description: string
  createdAt: string
}

export interface PointsQuery extends PageQuery {
  memberId?: string
  type?: PointsType
  source?: PointsSource
}

export const POINTS_TYPE_OPTIONS = [
  { label: 'mall.points.typeEarn', value: 'earn' as const },
  { label: 'mall.points.typeSpend', value: 'spend' as const },
]

export const POINTS_SOURCE_OPTIONS = [
  { label: 'mall.points.sourceOrder', value: 'order' as const },
  { label: 'mall.points.sourceSignin', value: 'signin' as const },
  { label: 'mall.points.sourceActivity', value: 'activity' as const },
  { label: 'mall.points.sourceRefund', value: 'refund' as const },
  { label: 'mall.points.sourceAdjust', value: 'adjust' as const },
]

// ---- Coupon ----

export type CouponType = 'fixed' | 'percent'
export type CouponStatus = 'enabled' | 'disabled'

export interface Coupon {
  id: string
  name: string
  type: CouponType
  value: number
  minSpend: number
  totalCount: number
  issuedCount: number
  usedCount: number
  startDate: string
  endDate: string
  status: CouponStatus
  createdAt: string
  updatedAt: string
}

export interface CouponQuery extends PageQuery {
  name?: string
  type?: CouponType
  status?: CouponStatus
}

export interface CouponParams {
  name: string
  type: CouponType
  value: number
  minSpend?: number
  totalCount?: number
  startDate: string
  endDate: string
  status?: CouponStatus
}

export const COUPON_TYPE_OPTIONS = [
  { label: 'mall.coupon.typeFixed', value: 'fixed' as const },
  { label: 'mall.coupon.typePercent', value: 'percent' as const },
]

export const COUPON_STATUS_OPTIONS = [
  { label: 'mall.coupon.statusEnabled', value: 'enabled' as const },
  { label: 'mall.coupon.statusDisabled', value: 'disabled' as const },
]

// ---- Order ----

export type OrderStatus = 'pending' | 'paid' | 'shipped' | 'completed' | 'cancelled' | 'refunded'

export interface OrderItem {
  id: string
  orderId: string
  spuId: string
  spuName: string
  specValues: string
  price: number
  quantity: number
  subtotal: number
}

export interface Order {
  id: string
  orderNo: string
  memberId: string
  memberName: string
  memberPhone: string
  totalAmount: number
  discountAmount: number
  pointsAmount: number
  shippingFee: number
  payAmount: number
  couponId: string
  couponName: string
  status: OrderStatus
  paymentMethod: string
  remark: string
  items: OrderItem[]
  createdAt: string
  updatedAt: string
}

export interface OrderQuery extends PageQuery {
  orderNo?: string
  memberId?: string
  status?: OrderStatus
}

export interface OrderStatusUpdateParams {
  status: OrderStatus
  remark?: string
}

export const ORDER_STATUS_OPTIONS = [
  { label: 'mall.order.statusPending', value: 'pending' as const },
  { label: 'mall.order.statusPaid', value: 'paid' as const },
  { label: 'mall.order.statusShipped', value: 'shipped' as const },
  { label: 'mall.order.statusCompleted', value: 'completed' as const },
  { label: 'mall.order.statusCancelled', value: 'cancelled' as const },
  { label: 'mall.order.statusRefunded', value: 'refunded' as const },
]

// ---- Payment Record ----

export type PaymentStatus = 'pending' | 'success' | 'failed' | 'refunded'

export interface PaymentRecord {
  id: string
  orderId: string
  orderNo: string
  amount: number
  method: string
  status: PaymentStatus
  transactionId: string
  paidAt: string
  createdAt: string
}

export interface PaymentQuery extends PageQuery {
  orderNo?: string
  status?: PaymentStatus
}

export const PAYMENT_STATUS_OPTIONS = [
  { label: 'mall.payment.statusPending', value: 'pending' as const },
  { label: 'mall.payment.statusSuccess', value: 'success' as const },
  { label: 'mall.payment.statusFailed', value: 'failed' as const },
  { label: 'mall.payment.statusRefunded', value: 'refunded' as const },
]
