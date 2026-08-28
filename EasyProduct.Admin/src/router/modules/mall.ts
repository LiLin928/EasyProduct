import type { RouteRecordRaw } from 'vue-router'

export const mallRoutes: RouteRecordRaw[] = [
  {
    path: '/mall',
    name: 'mall',
    redirect: '/mall/member',
    meta: { title: 'menu.mallRoot', icon: 'ShoppingCart' },
    children: [
      {
        path: 'member',
        name: 'mall-member',
        component: () => import('@/views/mall/member/index.vue'),
        meta: { title: 'menu.mallMember', icon: 'User' },
      },
      {
        path: 'level',
        name: 'mall-level',
        component: () => import('@/views/mall/level/index.vue'),
        meta: { title: 'menu.mallLevel', icon: 'Medal' },
      },
      {
        path: 'points',
        name: 'mall-points',
        component: () => import('@/views/mall/points/index.vue'),
        meta: { title: 'menu.mallPoints', icon: 'Coin' },
      },
      {
        path: 'coupon',
        name: 'mall-coupon',
        component: () => import('@/views/mall/coupon/index.vue'),
        meta: { title: 'menu.mallCoupon', icon: 'Ticket' },
      },
      {
        path: 'order',
        name: 'mall-order',
        component: () => import('@/views/mall/order/index.vue'),
        meta: { title: 'menu.mallOrder', icon: 'List' },
      },
      {
        path: 'payment',
        name: 'mall-payment',
        component: () => import('@/views/mall/payment/index.vue'),
        meta: { title: 'menu.mallPayment', icon: 'Wallet' },
      },
    ],
  },
]
