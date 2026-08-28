import type { RouteRecordRaw } from 'vue-router'

export const crmRoutes: RouteRecordRaw[] = [
  {
    path: '/crm',
    name: 'crm',
    redirect: '/crm/customer',
    meta: { title: 'menu.crmRoot', icon: 'Briefcase' },
    children: [
      {
        path: 'customer',
        name: 'crm-customer',
        component: () => import('@/views/crm/customer/index.vue'),
        meta: { title: 'menu.crmCustomer', icon: 'User' },
      },
      {
        path: 'supplier',
        name: 'crm-supplier',
        component: () => import('@/views/crm/supplier/index.vue'),
        meta: { title: 'menu.crmSupplier', icon: 'Truck' },
      },
      {
        path: 'currency',
        name: 'crm-currency',
        component: () => import('@/views/crm/currency/index.vue'),
        meta: { title: 'menu.crmCurrency', icon: 'Coins' },
      },
      {
        path: 'tax-rate',
        name: 'crm-tax-rate',
        component: () => import('@/views/crm/tax-rate/index.vue'),
        meta: { title: 'menu.crmTaxRate', icon: 'Percent' },
      },
    ],
  },
]
