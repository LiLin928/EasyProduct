// src/data/basic.ts
// [backend: Basic 模块 | status: pending]
import { guid } from '../helpers/id.js'

export const ADMIN_USERS = [
  { id: guid(), userName: 'admin', password: 'admin123', realName: '系统管理员' },
  { id: guid(), userName: 'sales', password: 'sales123', realName: '销售专员' },
  { id: guid(), userName: 'ops', password: 'ops123', realName: '运维专员' },
]

/** 权限标识：super 用通配 '*'；其余账号 F2 Basic 模块时细化 */
export const USER_PERMISSIONS: Record<string, string[]> = {
  admin: ['*'],
  sales: ['crm:customer:list', 'crm:customer:add', 'mall:order:list'],
  ops: ['ops:log:list'],
}

/** 菜单树：工作台 + 八大模块 */
export const MENU_TREE = [
  { id: guid(), parentId: '0', name: 'desktop', path: '/desktop', titleKey: 'menu.desktop', icon: 'Monitor', sort: 1, children: [] },
  {
    id: guid(),
    parentId: '0',
    name: 'basic',
    path: '/basic',
    titleKey: 'menu.basicRoot',
    icon: 'Setting',
    sort: 2,
    children: [
      { id: guid(), parentId: '0', name: 'basic-user', path: '/basic/user', titleKey: 'menu.basicUser', icon: 'User', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'basic-role', path: '/basic/role', titleKey: 'menu.basicRole', icon: 'UserFilled', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'basic-menu', path: '/basic/menu', titleKey: 'menu.basicMenu', icon: 'Menu', sort: 3, children: [] },
      { id: guid(), parentId: '0', name: 'basic-dept', path: '/basic/dept', titleKey: 'menu.basicDept', icon: 'OfficeBuilding', sort: 4, children: [] },
      { id: guid(), parentId: '0', name: 'basic-dict', path: '/basic/dict', titleKey: 'menu.basicDict', icon: 'Collection', sort: 5, children: [] },
      { id: guid(), parentId: '0', name: 'basic-config', path: '/basic/config', titleKey: 'menu.basicConfig', icon: 'Tools', sort: 6, children: [] },
      { id: guid(), parentId: '0', name: 'basic-announcement', path: '/basic/announcement', titleKey: 'menu.basicAnnouncement', icon: 'Bell', sort: 7, children: [] },
    ]
  },
  {
    id: guid(),
    parentId: '0',
    name: 'product',
    path: '/product',
    titleKey: 'menu.productRoot',
    icon: 'Goods',
    sort: 3,
    children: [
      { id: guid(), parentId: '0', name: 'product-category', path: '/product/category', titleKey: 'menu.productCategory', icon: 'Files', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'product-spu', path: '/product/spu', titleKey: 'menu.productSpu', icon: 'Goods', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'product-channel', path: '/product/channel', titleKey: 'menu.productChannel', icon: 'Share', sort: 3, children: [] },
    ]
  },
  {
    id: guid(),
    parentId: '0',
    name: 'site',
    path: '/site',
    titleKey: 'menu.siteRoot',
    icon: 'Monitor',
    sort: 4,
    children: [
      { id: guid(), parentId: '0', name: 'site-news', path: '/site/news', titleKey: 'menu.siteNews', icon: 'Document', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'site-category', path: '/site/category', titleKey: 'menu.siteCategory', icon: 'Files', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'site-banner', path: '/site/banner', titleKey: 'menu.siteBanner', icon: 'Picture', sort: 3, children: [] },
      { id: guid(), parentId: '0', name: 'site-video', path: '/site/video', titleKey: 'menu.siteVideo', icon: 'VideoCamera', sort: 4, children: [] },
      { id: guid(), parentId: '0', name: 'site-download', path: '/site/download', titleKey: 'menu.siteDownload', icon: 'Download', sort: 5, children: [] },
      { id: guid(), parentId: '0', name: 'site-about', path: '/site/about', titleKey: 'menu.siteAbout', icon: 'InfoFilled', sort: 6, children: [] },
      { id: guid(), parentId: '0', name: 'site-inquiry', path: '/site/inquiry', titleKey: 'menu.siteInquiry', icon: 'ChatLineSquare', sort: 7, children: [] },
      { id: guid(), parentId: '0', name: 'site-contact', path: '/site/contact', titleKey: 'menu.siteContact', icon: 'Message', sort: 8, children: [] },
    ]
  },
  {
    id: guid(),
    parentId: '0',
    name: 'mall',
    path: '/mall',
    titleKey: 'menu.mallRoot',
    icon: 'ShoppingCart',
    sort: 5,
    children: [
      { id: guid(), parentId: '0', name: 'mall-member', path: '/mall/member', titleKey: 'menu.mallMember', icon: 'User', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'mall-level', path: '/mall/level', titleKey: 'menu.mallLevel', icon: 'Medal', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'mall-points', path: '/mall/points', titleKey: 'menu.mallPoints', icon: 'Coin', sort: 3, children: [] },
      { id: guid(), parentId: '0', name: 'mall-coupon', path: '/mall/coupon', titleKey: 'menu.mallCoupon', icon: 'Ticket', sort: 4, children: [] },
      { id: guid(), parentId: '0', name: 'mall-order', path: '/mall/order', titleKey: 'menu.mallOrder', icon: 'List', sort: 5, children: [] },
      { id: guid(), parentId: '0', name: 'mall-payment', path: '/mall/payment', titleKey: 'menu.mallPayment', icon: 'Wallet', sort: 6, children: [] },
    ]
  },
  {
    id: guid(),
    parentId: '0',
    name: 'crm',
    path: '/crm',
    titleKey: 'menu.crmRoot',
    icon: 'Briefcase',
    sort: 6,
    children: [
      { id: guid(), parentId: '0', name: 'crm-customer', path: '/crm/customer', titleKey: 'menu.crmCustomer', icon: 'User', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'crm-supplier', path: '/crm/supplier', titleKey: 'menu.crmSupplier', icon: 'Truck', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'crm-currency', path: '/crm/currency', titleKey: 'menu.crmCurrency', icon: 'Coins', sort: 3, children: [] },
      { id: guid(), parentId: '0', name: 'crm-tax-rate', path: '/crm/tax-rate', titleKey: 'menu.crmTaxRate', icon: 'Percent', sort: 4, children: [] },
      { id: guid(), parentId: '0', name: 'crm-sales-order', path: '/crm/sales-order', titleKey: 'menu.crmSalesOrder', icon: 'ShoppingCart', sort: 5, children: [] },
      { id: guid(), parentId: '0', name: 'crm-purchase-order', path: '/crm/purchase-order', titleKey: 'menu.crmPurchaseOrder', icon: 'ShoppingBag', sort: 6, children: [] },
      { id: guid(), parentId: '0', name: 'crm-warehouse', path: '/crm/warehouse', titleKey: 'menu.crmWarehouse', icon: 'House', sort: 7, children: [] },
      { id: guid(), parentId: '0', name: 'crm-stock', path: '/crm/stock', titleKey: 'menu.crmStock', icon: 'Box', sort: 8, children: [] },
      { id: guid(), parentId: '0', name: 'crm-stock-record', path: '/crm/stock-record', titleKey: 'menu.crmStockRecord', icon: 'Sort', sort: 9, children: [] },
      { id: guid(), parentId: '0', name: 'crm-stock-check', path: '/crm/stock-check', titleKey: 'menu.crmStockCheck', icon: 'Scale', sort: 10, children: [] },
      { id: guid(), parentId: '0', name: 'crm-stock-alert', path: '/crm/stock-alert', titleKey: 'menu.crmStockAlert', icon: 'Warning', sort: 11, children: [] },
      { id: guid(), parentId: '0', name: 'crm-invoice', path: '/crm/invoice', titleKey: 'menu.crmInvoice', icon: 'Document', sort: 12, children: [] },
      { id: guid(), parentId: '0', name: 'crm-payment', path: '/crm/payment', titleKey: 'menu.crmPayment', icon: 'Wallet', sort: 13, children: [] },
      { id: guid(), parentId: '0', name: 'crm-arap', path: '/crm/arap', titleKey: 'menu.crmArap', icon: 'Money', sort: 14, children: [] },
      { id: guid(), parentId: '0', name: 'crm-fixed-asset', path: '/crm/fixed-asset', titleKey: 'menu.crmFixedAsset', icon: 'Box', sort: 15, children: [] },
      { id: guid(), parentId: '0', name: 'crm-reversal', path: '/crm/reversal', titleKey: 'menu.crmReversal', icon: 'RefreshLeft', sort: 16, children: [] },
    ]
  },
  {
    id: guid(),
    parentId: '0',
    name: 'workflow',
    path: '/workflow',
    titleKey: 'menu.workflowRoot',
    icon: 'Connection',
    sort: 7,
    children: [
      { id: guid(), parentId: '0', name: 'workflow-my-apply', path: '/workflow/my-apply', titleKey: 'menu.workflowMyApply', icon: 'EditPen', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'workflow-publish', path: '/workflow/publish', titleKey: 'menu.workflowPublish', icon: 'Promotion', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'workflow-todo', path: '/workflow/todo', titleKey: 'menu.workflowTodo', icon: 'Bell', sort: 3, children: [] },
      { id: guid(), parentId: '0', name: 'workflow-done', path: '/workflow/done', titleKey: 'menu.workflowDone', icon: 'CircleCheck', sort: 4, children: [] },
      { id: guid(), parentId: '0', name: 'workflow-instance', path: '/workflow/instance', titleKey: 'menu.workflowInstance', icon: 'Document', sort: 5, children: [] },
      { id: guid(), parentId: '0', name: 'workflow-designer', path: '/workflow/designer', titleKey: 'menu.workflowDesigner', icon: 'SetUp', sort: 6, children: [] },
    ]
  },
  {
    id: guid(),
    parentId: '0',
    name: 'report',
    path: '/report',
    titleKey: 'menu.reportRoot',
    icon: 'DataAnalysis',
    sort: 8,
    children: [
      { id: guid(), parentId: '0', name: 'report-datasource', path: '/report/datasource', titleKey: 'menu.reportDatasource', icon: 'Connection', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'report-definition', path: '/report/definition', titleKey: 'menu.reportDefinition', icon: 'Document', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'report-column-template', path: '/report/column-template', titleKey: 'menu.reportColumnTemplate', icon: 'Grid', sort: 3, children: [] },
    ]
  },
  {
    id: guid(),
    parentId: '0',
    name: 'ops',
    path: '/ops',
    titleKey: 'menu.opsRoot',
    icon: 'Setting',
    sort: 9,
    children: [
      { id: guid(), parentId: '0', name: 'ops-operate-log', path: '/ops/operate-log', titleKey: 'menu.opsOperateLog', icon: 'Document', sort: 1, children: [] },
      { id: guid(), parentId: '0', name: 'ops-login-log', path: '/ops/login-log', titleKey: 'menu.opsLoginLog', icon: 'Key', sort: 2, children: [] },
      { id: guid(), parentId: '0', name: 'ops-task', path: '/ops/task', titleKey: 'menu.opsTask', icon: 'Timer', sort: 3, children: [] },
      { id: guid(), parentId: '0', name: 'ops-task-log', path: '/ops/task-log', titleKey: 'menu.opsTaskLog', icon: 'List', sort: 4, children: [] },
      { id: guid(), parentId: '0', name: 'ops-log-query', path: '/ops/log-query', titleKey: 'menu.opsLogQuery', icon: 'Search', sort: 5, children: [] },
    ]
  },
]

/** 字典种子：labelKey 走 i18n，前端 useDict 用 t(labelKey) 渲染 */
export const DICT_DATA: Record<string, Array<{ value: string; labelKey: string; sort: number }>> = {
  common_status: [
    { value: 'enabled', labelKey: 'common.status.enabled', sort: 1 },
    { value: 'disabled', labelKey: 'common.status.disabled', sort: 2 },
  ],
  customer_type: [
    { value: 'b2b', labelKey: 'common.dict.customerType.b2b', sort: 1 },
    { value: 'retail', labelKey: 'common.dict.customerType.retail', sort: 2 },
  ],
}
