// src/routes/admin/dict.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { DICT_TYPES, DICT_DATA } from '../../data/admin/dict.js'
import { guid } from '../../helpers/id.js'

export const adminDictRouter = Router()

// ========== 字典类型管理 ==========

/** 字典类型列表（分页） */
adminDictRouter.get('/basic/dict-type/list', (req, res) => {
  const { pageIndex = 1, pageSize = 10, name, code } = req.query

  let filtered = [...DICT_TYPES]
  if (name) {
    filtered = filtered.filter(item => item.name.includes(name as string))
  }
  if (code) {
    filtered = filtered.filter(item => item.code.includes(code as string))
  }

  const start = (Number(pageIndex) - 1) * Number(pageSize)
  const end = start + Number(pageSize)
  const list = filtered.slice(start, end)

  res.json(ok({ list, total: filtered.length }))
})

/** 新增字典类型 */
adminDictRouter.post('/basic/dict-type', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

/** 编辑字典类型 */
adminDictRouter.put('/basic/dict-type/:id', (req, res) => {
  res.json(ok({ id: req.params.id }, '更新成功'))
})

/** 删除字典类型 */
adminDictRouter.delete('/basic/dict-type/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})

// ========== 字典数据管理 ==========

/** 按字典类型取字典项 */
adminDictRouter.get('/basic/dict-data', (req, res) => {
  const typeCode = String(req.query.typeCode ?? '')
  const items = DICT_DATA.filter(item => item.typeCode === typeCode && item.status === 'enabled')

  if (items.length === 0 && !DICT_TYPES.some(t => t.code === typeCode)) {
    return res.json(fail(`字典类型 ${typeCode} 不存在`, 404))
  }

  // 转换为前端期望的 DictItem 格式
  const dictItems = items
    .sort((a, b) => a.sort - b.sort)
    .map(item => ({
      value: item.value,
      labelKey: item.labelKey
    }))

  res.json(ok(dictItems))
})

/** 字典数据列表（分页） */
adminDictRouter.get('/basic/dict-data/list', (req, res) => {
  const { typeCode, pageIndex = 1, pageSize = 10 } = req.query

  let filtered = DICT_DATA
  if (typeCode) {
    filtered = filtered.filter(item => item.typeCode === typeCode)
  }

  const start = (Number(pageIndex) - 1) * Number(pageSize)
  const end = start + Number(pageSize)
  const list = filtered.slice(start, end)

  res.json(ok({ list, total: filtered.length }))
})

/** 新增字典数据 */
adminDictRouter.post('/basic/dict-data', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

/** 编辑字典数据 */
adminDictRouter.put('/basic/dict-data/:id', (req, res) => {
  res.json(ok({ id: req.params.id }, '更新成功'))
})

/** 删除字典数据 */
adminDictRouter.delete('/basic/dict-data/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})

/** 批量删除字典数据 */
adminDictRouter.post('/basic/dict-data/batch-delete', (req, res) => {
  res.json(ok(null, '删除成功'))
})