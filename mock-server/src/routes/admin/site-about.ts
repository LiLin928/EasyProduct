// src/routes/admin/site-about.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { isoTime } from '../../helpers/id.js'
import { ABOUT } from '../../data/site-full.js'

export const adminSiteAboutRouter = Router()

// 获取关于我们详情
adminSiteAboutRouter.get('/site/about/:id', (req, res) => {
  if (req.params.id === ABOUT.id) {
    res.json(ok(ABOUT))
  } else {
    res.json(fail('关于我们不存在', 404))
  }
})

// 更新关于我们
adminSiteAboutRouter.put('/site/about', (req, res) => {
  const body = req.body

  if (body.title !== undefined) ABOUT.title = body.title
  if (body.titleEn !== undefined) ABOUT.titleEn = body.titleEn
  if (body.subtitle !== undefined) ABOUT.subtitle = body.subtitle
  if (body.subtitleEn !== undefined) ABOUT.subtitleEn = body.subtitleEn
  if (body.content !== undefined) ABOUT.content = body.content
  if (body.contentEn !== undefined) ABOUT.contentEn = body.contentEn
  if (body.coverImage !== undefined) ABOUT.coverImage = body.coverImage
  if (body.keywords !== undefined) ABOUT.keywords = body.keywords
  if (body.description !== undefined) ABOUT.description = body.description
  if (body.status !== undefined) ABOUT.status = body.status

  ABOUT.updatedAt = isoTime()
  res.json(ok(null, '关于我们更新成功'))
})

// 更新关于我们状态
adminSiteAboutRouter.put('/site/about/:id/status', (req, res) => {
  const status = Number(req.query.status)

  if (req.params.id !== ABOUT.id) {
    res.json(fail('关于我们不存在', 404))
    return
  }

  ABOUT.status = status as 0 | 1
  ABOUT.updatedAt = isoTime()

  res.json(ok(null, status === 1 ? '关于我们已启用' : '关于我们已禁用'))
})