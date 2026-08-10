// src/routes/site/contact.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'

export const siteContactRouter = Router()

// 简单内存限流（同一 IP 60秒内最多提交3次）
const rateLimit = new Map<string, number[]>()
const LIMIT = 3
const WINDOW = 60000

siteContactRouter.post('/contact', (req, res) => {
  const ip = req.ip || 'unknown'
  const now = Date.now()

  const times = rateLimit.get(ip) || []
  const recent = times.filter(t => now - t < WINDOW)

  if (recent.length >= LIMIT) {
    res.json(fail('提交过于频繁，请稍后再试', 429))
    return
  }

  recent.push(now)
  rateLimit.set(ip, recent)

  // 模拟成功
  res.json(ok(null, '提交成功'))
})