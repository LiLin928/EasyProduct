import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { i18n } from '@/i18n'

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/components/layout/AppLayout.vue'),
    children: [
      { path: '', name: 'home', component: () => import('@/views/home/index.vue'), meta: { title: 'common.nav.home' } },
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
