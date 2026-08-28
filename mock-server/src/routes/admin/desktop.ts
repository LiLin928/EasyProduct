// src/routes/admin/desktop.ts
import { Router } from 'express'
import Mock from 'mockjs'
import { ok } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'

export const adminDesktopRouter = Router()

/** 工作台概览（聚合统计 + 最近订单 + 待办，数值 mock 随机） */
adminDesktopRouter.get('/basic/desktop/overview', (_req, res) => {
  const recentOrders = Mock.mock({
    'list|5': [{
      id: '@guid',
      amount: '@float(100, 9999, 2, 2)',
      status: "@pick(['pending', 'paid', 'shipped', 'completed'])",
      customerName: '@cname',
      createTime: '@datetime("yyyy-MM-dd HH:mm:ss")',
    }],
  }).list.map((o: Record<string, unknown>) => ({ ...o, id: guid() }))

  const todos = Mock.mock({
    'list|4': [{
      id: '@guid',
      title: '@ctitle(8, 16)',
      type: "@pick(['order', 'refund', 'audit', 'stock'])",
      createTime: '@datetime("yyyy-MM-dd HH:mm:ss")',
    }],
  }).list.map((t: Record<string, unknown>) => ({ ...t, id: guid(), createTime: isoTime() }))

  res.json(ok({
    userCount: Mock.mock('@integer(100, 9999)'),
    orderCount: Mock.mock('@integer(100, 9999)'),
    salesAmount: Mock.mock('@float(10000, 999999, 2, 2)'),
    todayVisits: Mock.mock('@integer(50, 2000)'),
    recentOrders,
    todos,
  }))
})

/** Widget: 经營概覽（銷售/采購/利潤 + 月度趨勢） */
adminDesktopRouter.get('/basic/desktop/widget-overview', (_req, res) => {
  const months = ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月']
  const trend = months.map((m) => ({
    month: m,
    sales: Mock.mock('@float(50000, 200000, 2, 2)'),
    purchase: Mock.mock('@float(30000, 150000, 2, 2)'),
    profit: Mock.mock('@float(10000, 80000, 2, 2)'),
  }))
  res.json(ok({
    salesAmount: Mock.mock('@float(500000, 2000000, 2, 2)'),
    purchaseAmount: Mock.mock('@float(300000, 1500000, 2, 2)'),
    profitAmount: Mock.mock('@float(100000, 800000, 2, 2)'),
    trend,
  }))
})

/** Widget: KPI（客戶增長/訂單轉化/庫存周轉） */
adminDesktopRouter.get('/basic/desktop/widget-kpi', (_req, res) => {
  const months = ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月']
  const customerGrowth = months.map((m) => ({ month: m, value: Mock.mock('@integer(50, 500)') }))
  const orderConversion = [
    { name: '已下單', value: Mock.mock('@integer(100, 500)') },
    { name: '已付款', value: Mock.mock('@integer(80, 400)') },
    { name: '已發貨', value: Mock.mock('@integer(60, 300)') },
    { name: '已完成', value: Mock.mock('@integer(40, 200)') },
  ]
  const inventoryTurnover = months.map((m) => ({ month: m, value: Mock.mock('@float(1, 10, 2, 2)') }))
  res.json(ok({ customerGrowth, orderConversion, inventoryTurnover }))
})

/** Widget: 待辦計數 */
adminDesktopRouter.get('/basic/desktop/widget-todo-count', (_req, res) => {
  res.json(ok({
    pendingApproval: Mock.mock('@integer(3, 30)'),
    processingOrder: Mock.mock('@integer(5, 50)'),
    lowStockAlert: Mock.mock('@integer(1, 15)'),
  }))
})

/** Widget: 預警列表 */
adminDesktopRouter.get('/basic/desktop/widget-alerts', (_req, res) => {
  const types = ['stock', 'ar', 'order'] as const
  const levels = ['warning', 'danger'] as const
  const alerts = Mock.mock({
    'list|8': [{
      id: '@guid',
      type: `@pick(['${types[0]}', '${types[1]}', '${types[2]}'])`,
      title: '@ctitle(5, 15)',
      level: `@pick(['${levels[0]}', '${levels[1]}'])`,
      createdAt: '@datetime("yyyy-MM-dd HH:mm:ss")',
    }],
  }).list.map((a: Record<string, unknown>) => ({ ...a, id: guid(), createdAt: isoTime() }))
  res.json(ok(alerts))
})
