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
      {
        path: 'sales-order',
        name: 'crm-sales-order',
        component: () => import('@/views/crm/sales-order/index.vue'),
        meta: { title: 'menu.crmSalesOrder', icon: 'ShoppingCart' },
      },
      {
        path: 'purchase-order',
        name: 'crm-purchase-order',
        component: () => import('@/views/crm/purchase-order/index.vue'),
        meta: { title: 'menu.crmPurchaseOrder', icon: 'ShoppingBag' },
      },

      {
        path: 'warehouse',
        name: 'crm-warehouse',
        component: () => import('@/views/crm/warehouse/index.vue'),
        meta: { title: 'menu.crmWarehouse', icon: 'House' },
      },
      {
        path: 'stock',
        name: 'crm-stock',
        component: () => import('@/views/crm/stock/index.vue'),
        meta: { title: 'menu.crmStock', icon: 'Box' },
      },
      {
        path: 'stock-record',
        name: 'crm-stock-record',
        component: () => import('@/views/crm/stock-record/index.vue'),
        meta: { title: 'menu.crmStockRecord', icon: 'Sort' },
      },
      {
        path: 'stock-check',
        name: 'crm-stock-check',
        component: () => import('@/views/crm/stock-check/index.vue'),
        meta: { title: 'menu.crmStockCheck', icon: 'Scale' },
      },
      {
        path: 'stock-alert',
        name: 'crm-stock-alert',
        component: () => import('@/views/crm/stock-alert/index.vue'),
        meta: { title: 'menu.crmStockAlert', icon: 'Warning' },
      },
    ],
  },
]
