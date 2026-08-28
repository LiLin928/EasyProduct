import type { RouteRecordRaw } from 'vue-router'

export const reportRoutes: RouteRecordRaw[] = [
  {
    path: '/report',
    name: 'report',
    redirect: '/report/datasource',
    meta: { title: 'menu.reportRoot', icon: 'DataAnalysis' },
    children: [
      {
        path: 'datasource',
        name: 'report-datasource',
        component: () => import('@/views/report/datasource/index.vue'),
        meta: { title: 'menu.reportDatasource', icon: 'Connection' },
      },
      {
        path: 'definition',
        name: 'report-definition',
        component: () => import('@/views/report/definition/index.vue'),
        meta: { title: 'menu.reportDefinition', icon: 'Document' },
      },
      {
        path: 'column-template',
        name: 'report-column-template',
        component: () => import('@/views/report/column-template/index.vue'),
        meta: { title: 'menu.reportColumnTemplate', icon: 'Grid' },
      },
    ],
  },
]
