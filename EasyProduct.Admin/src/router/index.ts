import type { App } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { setupGuards } from './guards'
import { placeholderRoutes } from './modules/placeholder'
import { basicRoutes } from './modules/basic'
import { siteRoutes } from './modules/site'
import { productRoutes } from './modules/product'
import { mallRoutes } from './modules/mall'
import { crmRoutes } from './modules/crm'
import { opsRoutes } from './modules/ops'
import { reportRoutes } from './modules/report'
import { workflowRoutes } from './modules/workflow'

export const routes: RouteRecordRaw[] = [
  { path: '/login', name: 'login', component: () => import('@/views/login/index.vue'), meta: { title: 'common.login.title' } },
  {
    path: '/',
    component: () => import('@/layouts/MainLayout.vue'),
    redirect: '/desktop',
    children: [
      { path: 'desktop', name: 'desktop', component: () => import('@/views/desktop/index.vue'), meta: { title: 'menu.desktop', icon: 'Monitor' } },
      { path: "profile", name: "profile", component: () => import("@/views/basic/profile/index.vue"), meta: { title: "menu.profile", icon: "User" } },
      ...basicRoutes,
      ...siteRoutes,
      ...productRoutes,
      ...mallRoutes,
      ...crmRoutes,
     ...opsRoutes,
      ...reportRoutes,
      ...workflowRoutes,
      ...placeholderRoutes,
    ],
  },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/error/404.vue'), meta: { title: 'menu.notFound' } },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

export function setupRouter(app: App): void {
  setupGuards(router)
  app.use(router)
}
