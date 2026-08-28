import type { RouteRecordRaw } from 'vue-router'

export const opsRoutes: RouteRecordRaw[] = [
  {
    path: '/ops',
    name: 'ops',
    redirect: '/ops/operate-log',
    meta: { title: 'menu.opsRoot', icon: 'Setting' },
    children: [
      {
        path: 'operate-log',
        name: 'ops-operate-log',
        component: () => import('@/views/ops/operate-log/index.vue'),
        meta: { title: 'menu.opsOperateLog', icon: 'Document' },
      },
      {
        path: 'login-log',
        name: 'ops-login-log',
        component: () => import('@/views/ops/login-log/index.vue'),
        meta: { title: 'menu.opsLoginLog', icon: 'Key' },
      },
      {
        path: 'task',
        name: 'ops-task',
        component: () => import('@/views/ops/task/index.vue'),
        meta: { title: 'menu.opsTask', icon: 'Timer' },
      },
      {
        path: 'task-log',
        name: 'ops-task-log',
        component: () => import('@/views/ops/task-log/index.vue'),
        meta: { title: 'menu.opsTaskLog', icon: 'List' },
      },
      {
        path: 'log-query',
        name: 'ops-log-query',
        component: () => import('@/views/ops/log-query/index.vue'),
        meta: { title: 'menu.opsLogQuery', icon: 'Search' },
      },
    ],
  },
]
