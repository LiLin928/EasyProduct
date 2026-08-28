import type { RouteRecordRaw } from 'vue-router'

const MODULES = ['crm', 'workflow', 'report', 'ops'] as const

export const placeholderRoutes: RouteRecordRaw[] = MODULES.map((name) => ({
  path: `/${name}`,
  name: `placeholder-${name}`,
  component: () => import('@/views/placeholder/index.vue'),
  meta: { title: `menu.${name}`, icon: 'Menu' },
}))
