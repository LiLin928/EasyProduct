import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { i18n } from '@/i18n'

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/components/layout/AppLayout.vue'),
    children: [
      { path: '', name: 'home', component: () => import('@/views/home/index.vue'), meta: { title: 'common.nav.home' } },
      { path: 'products', name: 'products', component: () => import('@/views/products/index.vue'), meta: { title: 'site.products.title' } },
      { path: 'products/:id', name: 'product-detail', component: () => import('@/views/products/detail.vue'), meta: { title: 'site.products.detail' } },
      { path: 'news', name: 'news', component: () => import('@/views/news/index.vue'), meta: { title: 'site.news.title' } },
      { path: 'news/:id', name: 'news-detail', component: () => import('@/views/news/detail.vue'), meta: { title: 'site.news.detail' } },
      { path: 'announcement', name: 'announcement', component: () => import('@/views/announcement/index.vue'), meta: { title: 'site.announcement.title' } },
      { path: 'announcement/:id', name: 'announcement-detail', component: () => import('@/views/announcement/detail.vue'), meta: { title: 'site.announcement.detail' } },
      { path: 'inquiry', name: 'inquiry', component: () => import('@/views/inquiry/index.vue'), meta: { title: 'site.inquiry.title' } },
    ],
  },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/error/404.vue'), meta: { title: 'site.notFound.back' } },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.afterEach((to) => {
  const key = to.meta.title as string | undefined
  const page = key ? i18n.global.t(key) : ''
  document.title = page ? `${page} - EasyProduct` : 'EasyProduct'
})