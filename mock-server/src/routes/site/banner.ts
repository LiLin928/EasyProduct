// src/routes/site/banner.ts
// 官网公开Banner Mock API 路由
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { BANNERS } from '../../data/site.js'

export const siteBannerRouter = Router()

/**
 * GET /api/site/banner/:position?
 * 根据位置获取Banner列表（只返回启用且在有效期内的Banner）
 */
siteBannerRouter.get('/banner/:position?', (req, res) => {
  const position = req.params.position as string | undefined
  const now = new Date()

  let filtered = BANNERS.filter((b) => {
    // 只返回启用的Banner
    if (b.status !== 1) return false

    // 检查是否在有效期内
    if (b.startTime && new Date(b.startTime) > now) return false
    if (b.endTime && new Date(b.endTime) < now) return false

    // 如果指定了位置，则按位置筛选
    if (position && b.position !== position) return false

    return true
  })

  // 按 sort 字段升序排序
  filtered.sort((a, b) => a.sort - b.sort)

  res.json(ok(filtered))
})