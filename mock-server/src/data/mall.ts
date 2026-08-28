// src/data/mall.ts
// [backend: Mall module | status: pending]
import Mock from 'mockjs'
import { guid, isoTime, code } from '../helpers/id.js'

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

export const MEMBER_LEVELS: MemberLevel[] = [
  { id: guid(), name: '普通会员', minPoints: 0, discount: 1.0, sort: 1, status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: '银卡会员', minPoints: 1000, discount: 0.98, sort: 2, status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: '金卡会员', minPoints: 5000, discount: 0.95, sort: 3, status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: '钻石会员', minPoints: 20000, discount: 0.90, sort: 4, status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
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

const memberNames = Mock.mock({ 'list|20': [{ name: '@cname' }] }).list
export const MEMBERS: Member[] = memberNames.map((m: { name: string }, i: number) => {
  const level = MEMBER_LEVELS[i % MEMBER_LEVELS.length]
  const spent = Mock.mock('@float(0,50000,2,2)') as number
  return {
    id: guid(),
    nickname: m.name,
    avatar: Mock.mock('@image(100x100)') as string,
    phone: Mock.mock(/^1[3-9]\d{9}$/) as string,
    openid: guid(),
    levelId: level.id,
    levelName: level.name,
    points: Mock.mock('@integer(0,20000)') as number,
    totalSpent: spent,
    orderCount: Mock.mock('@integer(0,50)') as number,
    status: 'active' as const,
    createdAt: new Date(Date.now() - (i + 1) * 86400000 * 3).toISOString(),
    updatedAt: isoTime(),
  }
})

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

export const POINTS_RECORDS: PointsRecord[] = MEMBERS.slice(0, 10).flatMap((m, i) => [
  {
    id: guid(),
    memberId: m.id,
    memberName: m.nickname,
    type: 'earn' as const,
    amount: Mock.mock('@integer(10,500)') as number,
    source: 'order' as const,
    description: '购物消费获赠积分',
    createdAt: new Date(Date.now() - (i + 1) * 86400000).toISOString(),
  },
  {
    id: guid(),
    memberId: m.id,
    memberName: m.nickname,
    type: 'spend' as const,
    amount: -(Mock.mock('@integer(10,200)') as number),
    source: 'order' as const,
    description: '下单抵扣积分',
    createdAt: new Date(Date.now() - (i + 2) * 86400000).toISOString(),
  },
])

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

export const COUPONS: Coupon[] = [
  { id: guid(), name: '满100减10', type: 'fixed', value: 10, minSpend: 100, totalCount: 1000, issuedCount: 320, usedCount: 156, startDate: new Date(Date.now() - 86400000 * 7).toISOString().slice(0, 10), endDate: new Date(Date.now() + 86400000 * 30).toISOString().slice(0, 10), status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: '满500减50', type: 'fixed', value: 50, minSpend: 500, totalCount: 500, issuedCount: 210, usedCount: 89, startDate: new Date(Date.now() - 86400000 * 5).toISOString().slice(0, 10), endDate: new Date(Date.now() + 86400000 * 60).toISOString().slice(0, 10), status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: '9折券', type: 'percent', value: 0.9, minSpend: 200, totalCount: 2000, issuedCount: 890, usedCount: 456, startDate: new Date(Date.now() - 86400000 * 10).toISOString().slice(0, 10), endDate: new Date(Date.now() + 86400000 * 15).toISOString().slice(0, 10), status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: '85折券', type: 'percent', value: 0.85, minSpend: 1000, totalCount: 300, issuedCount: 120, usedCount: 34, startDate: new Date(Date.now() - 86400000 * 3).toISOString().slice(0, 10), endDate: new Date(Date.now() + 86400000 * 90).toISOString().slice(0, 10), status: 'disabled', createdAt: isoTime(), updatedAt: isoTime() },
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

const orderStatuses: OrderStatus[] = ['pending', 'paid', 'shipped', 'completed', 'cancelled', 'refunded']
const productNames = ['工业传感器 X100', '智能温控仪', '数据采集模块', 'PLC控制器', '变频器', '伺服电机', '触摸屏 HMI', '继电器模块']

export const ORDERS: Order[] = Array.from({ length: 15 }, (_, i) => {
  const member = MEMBERS[i % MEMBERS.length]
  const status = orderStatuses[i % orderStatuses.length]
  const itemCount = 1 + (i % 3)
  const items: OrderItem[] = Array.from({ length: itemCount }, (_, j) => {
    const price = Mock.mock('@float(50,2000,2,2)') as number
    const qty = 1 + (j % 3)
    return {
      id: guid(),
      orderId: '',
      spuId: guid(),
      spuName: productNames[(i + j) % productNames.length],
      specValues: j === 0 ? '默认规格' : `规格${j + 1}`,
      price,
      quantity: qty,
      subtotal: Math.round(price * qty * 100) / 100,
    }
  })
  const totalAmount = items.reduce((s, it) => s + it.subtotal, 0)
  const discountAmount = i % 4 === 0 ? Mock.mock('@float(10,100,2,2)') as number : 0
  const pointsAmount = i % 3 === 0 ? Mock.mock('@float(5,50,2,2)') as number : 0
  const shippingFee = 10
  const payAmount = Math.round((totalAmount - discountAmount - pointsAmount + shippingFee) * 100) / 100
  const coupon = i % 4 === 0 ? COUPONS[0] : null
  const orderId = guid()
  items.forEach(it => { it.orderId = orderId })
  return {
    id: orderId,
    orderNo: code('ORD'),
    memberId: member.id,
    memberName: member.nickname,
    memberPhone: member.phone,
    totalAmount: Math.round(totalAmount * 100) / 100,
    discountAmount,
    pointsAmount,
    shippingFee,
    payAmount,
    couponId: coupon?.id ?? '',
    couponName: coupon?.name ?? '',
    status,
    paymentMethod: status === 'pending' ? '' : ['wechat', 'alipay'][i % 2],
    remark: i % 5 === 0 ? '加急发货' : '',
    items,
    createdAt: new Date(Date.now() - (i + 1) * 86400000 * 2).toISOString(),
    updatedAt: isoTime(),
  }
})

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

export const PAYMENTS: PaymentRecord[] = ORDERS.filter(o => o.status !== 'pending' && o.status !== 'cancelled').map((o) => ({
  id: guid(),
  orderId: o.id,
  orderNo: o.orderNo,
  amount: o.payAmount,
  method: o.paymentMethod,
  status: o.status === 'refunded' ? 'refunded' : 'success',
  transactionId: guid().replace(/-/g, '').toUpperCase().slice(0, 32),
  paidAt: o.createdAt,
  createdAt: o.createdAt,
}))
