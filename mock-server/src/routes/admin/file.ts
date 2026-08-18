// src/routes/admin/file.ts
import { Router } from 'express'
import multer from 'multer'
import { ok } from '../../helpers/envelope.js'

export const adminFileRouter = Router()
const upload = multer({ storage: multer.memoryStorage() })

/** 文件上传（mock 阶段返回 base64 占位 URL，后端交付换真实 OSS） */
adminFileRouter.post('/basic/file/upload', upload.single('file'), (req, res) => {
  if (!req.file) {
    res.json(ok(null, '未接收到文件'))
    return
  }
  const ext = (req.file.originalname.split('.').pop() || 'png').toLowerCase()
  const base64 = `data:image/${ext};base64,${req.file.buffer.toString('base64')}`
  res.json(ok({ url: base64, fileName: req.file.originalname }, '上传成功'))
})
