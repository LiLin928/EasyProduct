// src/routes/site/video.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { VIDEOS } from '../../data/site-full.js'

export const siteVideoRouter = Router()

// 获取视频列表（官网公开）
siteVideoRouter.get('/video/list/:category?', (req, res) => {
  const category = req.params.category as string | undefined

  // 只返回启用状态的视频
  let list = VIDEOS.filter(v => v.status === 1)

  // 按分类筛选
  if (category) {
    list = list.filter(v => v.category === category)
  }

  // 排序：先按排序号，再按创建时间倒序
  list.sort((a, b) => a.sort - b.sort || new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

  res.json(ok(list))
})

// 获取视频详情并增加播放次数（官网公开）
siteVideoRouter.get('/video/:id', (req, res) => {
  const video = VIDEOS.find(v => v.id === req.params.id && v.status === 1)

  if (!video) {
    res.json(ok(null))
    return
  }

  // 增加播放次数
  video.playCount++

  res.json(ok(video))
})