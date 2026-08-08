// src/routes/admin/dict.ts
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { DICT_DATA } from '../../data/basic.js'

export const adminDictRouter = Router()

adminDictRouter.get('/basic/dict-data', (req, res) => {
  const typeCode = String(req.query.typeCode ?? '')
  const items = DICT_DATA[typeCode]
  if (!items) return res.json(fail(`字典 ${typeCode} 不存在`, 404))
  res.json(ok(items))
})
