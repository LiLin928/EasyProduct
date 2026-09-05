// src/routes/app/member.ts
import { Router } from 'express'
import { MEMBERS, ORDERS, POINTS_RECORDS } from '../../data/mall.js'
import { fail, ok } from '../../helpers/envelope.js'

export const appMemberRouter = Router()

/**
 * 获取当前会员资料（含等级与订单统计）
 * GET /api/app/member/info
 */
appMemberRouter.get('/info', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  const member = MEMBERS.find((m) => m.id === memberId)
  if (!member) return res.json(fail('会员不存在', 401))
  const myOrders = ORDERS.filter((o) => o.memberId === memberId)
  const pendingCount = myOrders.filter((o) => o.status === 'pending').length
  const paidCount = myOrders.filter((o) => o.status === 'paid' || o.status === 'shipped').length
  const completedCount = myOrders.filter((o) => o.status === 'completed').length
  res.json(ok({
    id: member.id, nickname: member.nickname, avatar: member.avatar, phone: member.phone,
    level: member.levelName, points: member.points, totalSpent: member.totalSpent,
    orderCount: myOrders.length, pendingCount, paidCount, completedCount,
  }))
})

/**
 * 获取当前会员资料（兼容 /profile）
 * GET /api/app/member/profile
 */
appMemberRouter.get('/profile', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  const member = MEMBERS.find((m) => m.id === memberId)
  if (!member) return res.json(fail('会员不存在', 401))
  const myOrders = ORDERS.filter((o) => o.memberId === memberId)
  const pendingCount = myOrders.filter((o) => o.status === 'pending').length
  const paidCount = myOrders.filter((o) => o.status === 'paid' || o.status === 'shipped').length
  const completedCount = myOrders.filter((o) => o.status === 'completed').length
  res.json(ok({
    id: member.id, nickname: member.nickname, avatar: member.avatar, phone: member.phone,
    level: member.levelName, points: member.points, totalSpent: member.totalSpent,
    orderCount: myOrders.length, pendingCount, paidCount, completedCount,
  }))
})

/**
 * 获取当前会员积分记录（分页）
 * GET /api/app/member/points
 */
appMemberRouter.get('/points', (req, res) => {
  const memberId = (req as any).member?.id as string | undefined
  if (!memberId) return res.json(fail('未授权', 401))
  const { pageIndex = '1', pageSize = '10' } = req.query as Record<string, string | undefined>
  const mine = POINTS_RECORDS.filter((p) => p.memberId === memberId)
  const page = parseInt(pageIndex, 10)
  const size = parseInt(pageSize, 10)
  const list = mine.slice((page - 1) * size, page * size)
  res.json(ok({ list, total: mine.length }))
})
