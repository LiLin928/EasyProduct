// src/routes/site/video.ts
import { Router } from 'express'
import { ok, paginate } from '../../helpers/envelope.js'
import { VIDEOS, VIDEO_CATEGORIES } from '../../data/site-full.js'

export const siteVideoRouter = Router()

// 获取视频分类列表
siteVideoRouter.get('/video-category/list', (_req, res) => {
  res.json(ok(VIDEO_CATEGORIES))
})

// 获取视频列表（支持分类筛选）
siteVideoRouter.get('/video/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 9)
  const categoryId = req.query.categoryId as string | undefined
  
  let list = VIDEOS.filter(v => v.status === 'published')
  
  // 按分类筛选
  if (categoryId) {
    list = list.filter(v => v.categoryId === categoryId)
  }
  
  res.json(ok(paginate(list, pageIndex, pageSize)))
})