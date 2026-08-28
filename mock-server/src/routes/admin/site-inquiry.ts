// src/routes/admin/site-inquiry.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { listInquiries, getInquiry, updateInquiryStatus, convertInquiryToCustomer, type InquiryStatus } from '../../store/inquiry.js'

export const adminSiteInquiryRouter = Router()

adminSiteInquiryRouter.get('/site/inquiry/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const status = req.query.status as string | undefined
  const companyName = req.query.companyName as string | undefined

  let filtered = listInquiries()
  if (status) filtered = filtered.filter(i => i.status === status)
  if (companyName) filtered = filtered.filter(i => i.companyName.includes(companyName))
  filtered.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminSiteInquiryRouter.get('/site/inquiry/:id', (req, res) => {
  const inquiry = getInquiry(req.params.id)
  if (!inquiry) { res.json(fail('inquiry not found', 404)); return }
  res.json(ok(inquiry))
})

adminSiteInquiryRouter.put('/site/inquiry/:id/status', (req, res) => {
  const status = req.body.status as InquiryStatus
  const success = updateInquiryStatus(req.params.id, status)
  if (!success) { res.json(fail('inquiry not found', 404)); return }
  res.json(ok(null, 'status updated'))
})

adminSiteInquiryRouter.post('/site/inquiry/:id/convert', (req, res) => {
  const success = convertInquiryToCustomer(req.params.id)
  if (!success) { res.json(fail('inquiry not found or already converted', 404)); return }
  res.json(ok({ customerId: 'mock-customer-' + req.params.id.slice(0, 8) }, 'converted to customer'))
})
