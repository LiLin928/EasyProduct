// src/routes/admin/site-contact.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { listContacts, getContact, updateContactStatus, type ContactMessage } from '../../store/contact.js'

export const adminSiteContactRouter = Router()

adminSiteContactRouter.get('/site/contact/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const status = req.query.status as string | undefined
  const name = req.query.name as string | undefined

  let filtered = listContacts()
  if (status) filtered = filtered.filter(m => m.status === status)
  if (name) filtered = filtered.filter(m => m.name.includes(name))
  filtered.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminSiteContactRouter.get('/site/contact/:id', (req, res) => {
  const msg = getContact(req.params.id)
  if (!msg) { res.json(fail('contact not found', 404)); return }
  if (msg.status === 'unread') updateContactStatus(req.params.id, 'read')
  res.json(ok(msg))
})

adminSiteContactRouter.put('/site/contact/:id/status', (req, res) => {
  const status = req.body.status as ContactMessage['status']
  const success = updateContactStatus(req.params.id, status)
  if (!success) { res.json(fail('contact not found', 404)); return }
  res.json(ok(null, 'status updated'))
})
