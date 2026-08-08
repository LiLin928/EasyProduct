// src/helpers/auth.ts
import type { NextFunction, Request, Response } from 'express'
import { fail } from './envelope.js'

const ADMIN_PUBLIC = ['/api/admin/auth/login']
const APP_PUBLIC = ['/api/app/auth/wx-login']

/** /api/admin/** 必须带 Bearer Token（登录接口除外） */
export function adminGuard(req: Request, res: Response, next: NextFunction): void {
  if (ADMIN_PUBLIC.includes(req.originalUrl.split('?')[0])) return next()
  const header = req.headers.authorization
  if (!header || !header.startsWith('Bearer ')) {
    res.json(fail('未登录或登录已过期', 401))
    return
  }
  next()
}

/** /api/app/** 必须带会员 Token（微信登录接口除外） */
export function appGuard(req: Request, res: Response, next: NextFunction): void {
  if (APP_PUBLIC.includes(req.originalUrl.split('?')[0])) return next()
  const header = req.headers.authorization
  if (!header || !header.startsWith('Bearer ')) {
    res.json(fail('请先登录', 401))
    return
  }
  next()
}