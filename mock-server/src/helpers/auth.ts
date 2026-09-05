// src/helpers/auth.ts
import type { NextFunction, Request, Response } from 'express'
import { fail } from './envelope.js'
import { MEMBERS } from '../data/mall.js'

const ADMIN_PUBLIC = ['/api/admin/auth/login']
const APP_PUBLIC = ['/api/app/auth/login', '/api/app/auth/wx-login', '/api/app/categories', '/api/app/products', '/api/app/products/categories', '/api/app/products/products', '/api/app/products/new', '/api/app/products/hot']

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
  // mock 阶段：token 格式为 mock-member-token-{memberId}，解析出固定会员身份
  const token = header.slice('Bearer '.length)
  const memberId = token.replace(/^mock-member-token-/, '')
  const member = MEMBERS.find((m) => m.id === memberId)
  if (member) {
    ;(req as any).member = { id: member.id, nickname: member.nickname, level: member.levelName }
  }
  next()
}




