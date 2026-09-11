// src/routes/admin/site-download.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { DOWNLOADS, type SiteDownload } from '../../data/site-full.js'

export const adminSiteDownloadRouter = Router()

// 获取下载列表（分页）
adminSiteDownloadRouter.get('/site/download/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const title = req.query.title as string | undefined
  const category = req.query.category as string | undefined
  const status = req.query.status ? Number(req.query.status) : undefined
  const fileType = req.query.fileType as string | undefined

  let filtered = [...DOWNLOADS]

  // 按标题模糊搜索
  if (title) {
    filtered = filtered.filter(d => d.title.includes(title))
  }

  // 按分类筛选
  if (category) {
    filtered = filtered.filter(d => d.category === category)
  }

  // 按状态筛选
  if (status !== undefined) {
    filtered = filtered.filter(d => d.status === status)
  }

  // 按文件类型筛选
  if (fileType) {
    filtered = filtered.filter(d => d.fileType === fileType)
  }

  // 排序：先按排序号，再按创建时间倒序
  filtered.sort((a, b) => a.sort - b.sort || new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

// 获取下载详情
adminSiteDownloadRouter.get('/site/download/:id', (req, res) => {
  const download = DOWNLOADS.find(d => d.id === req.params.id)

  if (!download) {
    res.json(fail('下载不存在', 404))
    return
  }

  res.json(ok(download))
})

// 创建下载
adminSiteDownloadRouter.post('/site/download', (req, res) => {
  const body = req.body

  if (!body.title?.trim()) {
    res.json(fail('下载标题不能为空'))
    return
  }

  const dl: SiteDownload = {
    id: guid(),
    title: body.title,
    titleEn: body.titleEn || '',
    description: body.description || '',
    descriptionEn: body.descriptionEn || '',
    fileUrl: body.fileUrl || '',
    fileName: body.fileName || '',
    fileSize: body.fileSize ?? 0,
    fileType: body.fileType || '',
    downloadCount: 0,
    category: body.category || '',
    sort: body.sort ?? DOWNLOADS.length + 1,
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  }

  DOWNLOADS.push(dl)
  res.json(ok({ id: dl.id }, '下载创建成功'))
})

// 更新下载
adminSiteDownloadRouter.put('/site/download/:id', (req, res) => {
  const dl = DOWNLOADS.find(d => d.id === req.params.id)

  if (!dl) {
    res.json(fail('下载不存在', 404))
    return
  }

  const body = req.body

  if (body.title) dl.title = body.title
  if (body.titleEn !== undefined) dl.titleEn = body.titleEn
  if (body.description !== undefined) dl.description = body.description
  if (body.descriptionEn !== undefined) dl.descriptionEn = body.descriptionEn
  if (body.fileUrl !== undefined) dl.fileUrl = body.fileUrl
  if (body.fileName !== undefined) dl.fileName = body.fileName
  if (body.fileSize !== undefined) dl.fileSize = body.fileSize
  if (body.fileType !== undefined) dl.fileType = body.fileType
  if (body.category !== undefined) dl.category = body.category
  if (body.sort !== undefined) dl.sort = body.sort
  if (body.status !== undefined) dl.status = body.status

  dl.updatedAt = isoTime()
  res.json(ok(null, '下载更新成功'))
})

// 删除下载
adminSiteDownloadRouter.delete('/site/download/:id', (req, res) => {
  const idx = DOWNLOADS.findIndex(d => d.id === req.params.id)

  if (idx === -1) {
    res.json(fail('下载不存在', 404))
    return
  }

  DOWNLOADS.splice(idx, 1)
  res.json(ok(null, '下载删除成功'))
})

// 批量删除下载
adminSiteDownloadRouter.post('/site/download/batch-delete', (req, res) => {
  const ids = req.body as string[]

  if (!Array.isArray(ids) || ids.length === 0) {
    res.json(fail('请选择要删除的下载'))
    return
  }

  let successCount = 0
  ids.forEach(id => {
    const idx = DOWNLOADS.findIndex(d => d.id === id)
    if (idx !== -1) {
      DOWNLOADS.splice(idx, 1)
      successCount++
    }
  })

  res.json(ok(null, `成功删除 ${successCount} 个下载`))
})

// 更新下载状态
adminSiteDownloadRouter.put('/site/download/:id/status', (req, res) => {
  const dl = DOWNLOADS.find(d => d.id === req.params.id)

  if (!dl) {
    res.json(fail('下载不存在', 404))
    return
  }

  const status = Number(req.query.status)
  dl.status = status as 0 | 1
  dl.updatedAt = isoTime()

  res.json(ok(null, status === 1 ? '下载已启用' : '下载已禁用'))
})