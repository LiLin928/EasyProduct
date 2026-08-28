// src/routes/admin/rpt-column-template.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { COLUMN_TEMPLATES, type ColumnTemplate } from '../../data/rpt-column-template.js'

export const adminRptColumnTemplateRouter = Router()

adminRptColumnTemplateRouter.get('/rpt/column-template/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const type = req.query.type as string | undefined

  let filtered = [...COLUMN_TEMPLATES]
  if (name) filtered = filtered.filter(c => c.name.toLowerCase().includes(name.toLowerCase()))
  if (type) filtered = filtered.filter(c => c.type === type)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminRptColumnTemplateRouter.get('/rpt/column-template/:id', (req, res) => {
  const tpl = COLUMN_TEMPLATES.find(c => c.id === req.params.id)
  if (!tpl) { res.json(fail('column template not found', 404)); return }
  res.json(ok(tpl))
})

adminRptColumnTemplateRouter.post('/rpt/column-template', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const tpl: ColumnTemplate = {
    id: guid(),
    name: body.name,
    field: body.field || '',
    type: body.type || 'string',
    width: body.width || 120,
    format: body.format || '',
    sortable: body.sortable ?? false,
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  COLUMN_TEMPLATES.unshift(tpl)
  res.json(ok({ id: tpl.id }, 'created'))
})

adminRptColumnTemplateRouter.put('/rpt/column-template/:id', (req, res) => {
  const tpl = COLUMN_TEMPLATES.find(c => c.id === req.params.id)
  if (!tpl) { res.json(fail('column template not found', 404)); return }

  const body = req.body
  if (body.name !== undefined) tpl.name = body.name
  if (body.field !== undefined) tpl.field = body.field
  if (body.type !== undefined) tpl.type = body.type
  if (body.width !== undefined) tpl.width = body.width
  if (body.format !== undefined) tpl.format = body.format
  if (body.sortable !== undefined) tpl.sortable = body.sortable
  if (body.remark !== undefined) tpl.remark = body.remark
  tpl.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminRptColumnTemplateRouter.delete('/rpt/column-template/:id', (req, res) => {
  const idx = COLUMN_TEMPLATES.findIndex(c => c.id === req.params.id)
  if (idx === -1) { res.json(fail('column template not found', 404)); return }
  COLUMN_TEMPLATES.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})
