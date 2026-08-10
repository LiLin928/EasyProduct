// src/routes/site/inquiry.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { createInquiry, getInquiry } from '../../store/inquiry.js'

export const siteInquiryRouter = Router()

// 提交询价
siteInquiryRouter.post('/inquiry', (req, res) => {
  const { companyName, contactName, phone, email, items } = req.body

  if (!companyName || !contactName || !phone || !email || !items || !items.length) {
    res.json(fail('请填写完整信息'))
    return
  }

  const inquiry = createInquiry({ companyName, contactName, phone, email, items })
  res.json(ok(inquiry, '询价提交成功'))
})

// 查询询价单
siteInquiryRouter.get('/inquiry/:id', (req, res) => {
  const inquiry = getInquiry(req.params.id)
  if (!inquiry) {
    res.json(fail('询价单不存在', 404))
    return
  }
  res.json(ok(inquiry))
})