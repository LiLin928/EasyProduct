import type { RouteRecordRaw } from 'vue-router'

export const productRoutes: RouteRecordRaw[] = [
  {
    path: '/product',
    name: 'product',
    redirect: '/product/spu',
    meta: { title: 'menu.productRoot', icon: 'Goods' },
    children: [
      {
        path: 'category',
        name: 'product-category',
        component: () => import('@/views/product/category/index.vue'),
        meta: { title: 'menu.productCategory', icon: 'Files' },
      },
      {
        path: 'spu',
        name: 'product-spu',
        component: () => import('@/views/product/spu/index.vue'),
        meta: { title: 'menu.productSpu', icon: 'Goods' },
      },
      {
        path: 'channel',
        name: 'product-channel',
        component: () => import('@/views/product/channel/index.vue'),
        meta: { title: 'menu.productChannel', icon: 'Share' },
      },
    ],
  },
]
