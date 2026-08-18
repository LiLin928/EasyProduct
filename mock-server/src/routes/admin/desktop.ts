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