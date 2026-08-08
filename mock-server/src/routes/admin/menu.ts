// src/routes/admin/menu.ts
import { Router } from 'express'
import { ok } from '../../helpers/envelope.js'
import { MENU_TREE } from '../../data/basic.js'

export const adminMenuRouter = Router()

adminMenuRouter.get('/basic/menu/list', (_req, res) => res.json(ok(MENU_TREE)))
