// src/routes/admin/config.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { SYSTEM_CONFIGS } from '../../data/admin/config.js'
import { guid, isoTime } from '../../helpers/id.js'

export const adminConfigRouter = Router()

/** 系统参数列表（分页 + key/label 筛选） */
adminConfigRouter.get('/basic/config/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const key = req.query.key as string | undefined
  const label = req.query.label as string | undefined

  let filtered = SYSTEM_CONFIGS
  if (key) filtered = filtered.filter((c) => c.key.includes(key))
  if (label) filtered = filtered.filter((c) => c.label.includes(label))

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

/** 新增系统参数（key 唯一校验） */
adminConfigRouter.post('/basic/config', (req, res) => {
  const { key, label, value, type, remark } = req.body || {}
  if (!key || !label || !type) {
    res.json(fail('参数键、名称、值类型不能为空', 400))
    return
  }
  if (SYSTEM_CONFIGS.some((c) => c.key === key)) {
    res.json(fail('参数键已存在', 400))
    return
  }
  const now = isoTime()
  const item = {
    id: guid(),
    key,
    label,
    value: value ?? '',
    type,
    remark: remark ?? '',
    createdAt: now,
    updatedAt: now,
  }
  SYSTEM_CONFIGS.push(item)
  res.json(ok({ id: item.id }, '创建成功'))
})

/** 编辑系统参数 */
adminConfigRouter.put('/basic/config/:id', (req, res) => {
  const idx = SYSTEM_CONFIGS.findIndex((c) => c.id === req.params.id)
  if (idx === -1) {
    res.json(fail('参数不存在', 404))
    return
  }
  const { label, value, type, remark } = req.body || {}
  const now = isoTime()
  SYSTEM_CONFIGS[idx] = {
    ...SYSTEM_CONFIGS[idx],
    label: label ?? SYSTEM_CONFIGS[idx].label,
    value: value ?? SYSTEM_CONFIGS[idx].value,
    type: type ?? SYSTEM_CONFIGS[idx].type,
    remark: remark ?? SYSTEM_CONFIGS[idx].remark,
    updatedAt: now,
  }
  res.json(ok(null, '更新成功'))
})

/** 删除系统参数 */
adminConfigRouter.delete('/basic/config/:id', (req, res) => {
  const idx = SYSTEM_CONFIGS.findIndex((c) => c.id === req.params.id)
  if (idx === -1) {
    res.json(fail('参数不存在', 404))
    return
  }
  SYSTEM_CONFIGS.splice(idx, 1)
  res.json(ok(null, '删除成功'))
})

/** 批量删除系统参数 */
adminConfigRouter.post('/basic/config/batch-delete', (req, res) => {
  const ids: string[] = req.body?.ids || []
  if (!Array.isArray(ids) || ids.length === 0) {
    res.json(fail('ids 不能为空', 400))
    return
  }
  for (const id of ids) {
    const idx = SYSTEM_CONFIGS.findIndex((c) => c.id === id)
    if (idx !== -1) SYSTEM_CONFIGS.splice(idx, 1)
  }
  res.json(ok(null, '删除成功'))
})