// src/routes/admin/rpt-definition.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { REPORT_DEFINITIONS, type ReportDefinition } from '../../data/rpt-definition.js'

export const adminRptDefinitionRouter = Router()

adminRptDefinitionRouter.get('/rpt/definition/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const code = req.query.code as string | undefined
  const status = req.query.status as string | undefined
  const datasourceId = req.query.datasourceId as string | undefined

  let filtered = [...REPORT_DEFINITIONS]
  if (name) filtered = filtered.filter(d => d.name.toLowerCase().includes(name.toLowerCase()))
  if (code) filtered = filtered.filter(d => d.code.toLowerCase().includes(code.toLowerCase()))
  if (status) filtered = filtered.filter(d => d.status === status)
  if (datasourceId) filtered = filtered.filter(d => d.datasourceId === datasourceId)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminRptDefinitionRouter.get('/rpt/definition/:id', (req, res) => {
  const def = REPORT_DEFINITIONS.find(d => d.id === req.params.id)
  if (!def) { res.json(fail('definition not found', 404)); return }
  res.json(ok(def))
})

adminRptDefinitionRouter.post('/rpt/definition', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const def: ReportDefinition = {
    id: guid(),
    name: body.name,
    code: body.code || `RPT_${String(REPORT_DEFINITIONS.length + 1).padStart(3, '0')}`,
    datasourceId: body.datasourceId || '',
    datasourceName: body.datasourceName || '',
    sqlTemplate: body.sqlTemplate || '',
    chartType: body.chartType || 'table',
    columns: body.columns || [],
    status: body.status || 'draft',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  REPORT_DEFINITIONS.unshift(def)
  res.json(ok({ id: def.id }, 'created'))
})

adminRptDefinitionRouter.put('/rpt/definition/:id', (req, res) => {
  const def = REPORT_DEFINITIONS.find(d => d.id === req.params.id)
  if (!def) { res.json(fail('definition not found', 404)); return }

  const body = req.body
  if (body.name !== undefined) def.name = body.name
  if (body.code !== undefined) def.code = body.code
  if (body.datasourceId !== undefined) def.datasourceId = body.datasourceId
  if (body.datasourceName !== undefined) def.datasourceName = body.datasourceName
  if (body.sqlTemplate !== undefined) def.sqlTemplate = body.sqlTemplate
  if (body.chartType !== undefined) def.chartType = body.chartType
  if (body.columns !== undefined) def.columns = body.columns
  if (body.status !== undefined) def.status = body.status
  if (body.remark !== undefined) def.remark = body.remark
  def.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminRptDefinitionRouter.delete('/rpt/definition/:id', (req, res) => {
  const idx = REPORT_DEFINITIONS.findIndex(d => d.id === req.params.id)
  if (idx === -1) { res.json(fail('definition not found', 404)); return }
  REPORT_DEFINITIONS.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})

adminRptDefinitionRouter.post('/rpt/definition/:id/preview', (req, res) => {
  const def = REPORT_DEFINITIONS.find(d => d.id === req.params.id)
  if (!def) { res.json(fail('definition not found', 404)); return }

  // mock preview data
  const rows = []
  for (let i = 0; i < 5; i++) {
    const row: Record<string, unknown> = {}
    for (const col of def.columns) {
      if (col.type === 'number') {
        row[col.field] = Math.floor(Math.random() * 1000) + 1
      } else if (col.type === 'currency') {
        row[col.field] = Math.floor(Math.random() * 100000) + 1000
      } else if (col.type === 'date') {
        row[col.field] = isoTime(-i)
      } else {
        row[col.field] = `${col.label}_${i + 1}`
      }
    }
    rows.push(row)
  }

  res.json(ok({ columns: def.columns.map(c => ({ field: c.field, label: c.label })), rows }))
})
