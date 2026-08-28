// src/routes/admin/rpt-datasource.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { DATASOURCES, type Datasource } from '../../data/rpt-datasource.js'

export const adminRptDatasourceRouter = Router()

adminRptDatasourceRouter.get('/rpt/datasource/options', (_req, res) => {
  const options = DATASOURCES
    .filter(d => d.status === 'connected')
    .map(d => ({ id: d.id, name: d.name, type: d.type }))
  res.json(ok(options))
})

adminRptDatasourceRouter.get('/rpt/datasource/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const name = req.query.name as string | undefined
  const type = req.query.type as string | undefined
  const status = req.query.status as string | undefined

  let filtered = [...DATASOURCES]
  if (name) filtered = filtered.filter(d => d.name.toLowerCase().includes(name.toLowerCase()))
  if (type) filtered = filtered.filter(d => d.type === type)
  if (status) filtered = filtered.filter(d => d.status === status)
  filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminRptDatasourceRouter.get('/rpt/datasource/:id', (req, res) => {
  const ds = DATASOURCES.find(d => d.id === req.params.id)
  if (!ds) { res.json(fail('datasource not found', 404)); return }
  res.json(ok(ds))
})

adminRptDatasourceRouter.post('/rpt/datasource', (req, res) => {
  const body = req.body
  if (!body.name?.trim()) { res.json(fail('name required')); return }

  const ds: Datasource = {
    id: guid(),
    name: body.name,
    type: body.type || 'mysql',
    host: body.host || '127.0.0.1',
    port: body.port || 3306,
    database: body.database || '',
    username: body.username || '',
    password: body.password || '********',
    status: 'disconnected',
    remark: body.remark || '',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  DATASOURCES.unshift(ds)
  res.json(ok({ id: ds.id }, 'created'))
})

adminRptDatasourceRouter.put('/rpt/datasource/:id', (req, res) => {
  const ds = DATASOURCES.find(d => d.id === req.params.id)
  if (!ds) { res.json(fail('datasource not found', 404)); return }

  const body = req.body
  if (body.name !== undefined) ds.name = body.name
  if (body.type !== undefined) ds.type = body.type
  if (body.host !== undefined) ds.host = body.host
  if (body.port !== undefined) ds.port = body.port
  if (body.database !== undefined) ds.database = body.database
  if (body.username !== undefined) ds.username = body.username
  if (body.password !== undefined) ds.password = body.password
  if (body.status !== undefined) ds.status = body.status
  if (body.remark !== undefined) ds.remark = body.remark
  ds.updatedAt = isoTime()
  res.json(ok(null, 'updated'))
})

adminRptDatasourceRouter.delete('/rpt/datasource/:id', (req, res) => {
  const idx = DATASOURCES.findIndex(d => d.id === req.params.id)
  if (idx === -1) { res.json(fail('datasource not found', 404)); return }
  DATASOURCES.splice(idx, 1)
  res.json(ok(null, 'deleted'))
})

adminRptDatasourceRouter.post('/rpt/datasource/:id/test', (req, res) => {
  const ds = DATASOURCES.find(d => d.id === req.params.id)
  if (!ds) { res.json(fail('datasource not found', 404)); return }
  // mock test connection
  if (ds.status === 'error') {
    ds.status = 'connected'
    ds.updatedAt = isoTime()
    res.json(ok({ success: true, message: 'Connection restored' }))
  } else if (ds.status === 'disconnected') {
    ds.status = 'connected'
    ds.updatedAt = isoTime()
    res.json(ok({ success: true, message: 'Connection established' }))
  } else {
    res.json(ok({ success: true, message: 'Connection is active' }))
  }
})
