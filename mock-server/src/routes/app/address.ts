// src/routes/app/address.ts
// 会员收货地址路由（小程序侧）
// 契约：与 docs/backend-guidelines.md 第 5 节统一信封一致
// 响应：{ code, message, data, timestamp }，HTTP 200，code===200 成功
// 主键：GUID（camelCase），列表直接返回 Address[]
// 鉴权：appGuard 已挂在 /api/app 全局上，由 req.member.id 区分多会员
// 路由顺序：/default 必须先于 /:id 声明（Express 按声明顺序匹配，PUT /:id 会吞掉 /:id/default）
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import { guid, isoTime } from '../../helpers/id.js'
import { seedAddressesByMember, type Address } from '../../data/address.js'

export const appAddressRouter = Router()

/** 按会员 ID 存放的地址列表（运行时修改，重启即重置） */
const addressStore = new Map<string, Address[]>()

/** 从种子初始化 store（首次访问时懒加载） */
function ensureInit(memberId: string): void {
  if (addressStore.has(memberId)) return
  const seeds = seedAddressesByMember.get(memberId) ?? []
  const now = isoTime()
  addressStore.set(
    memberId,
    seeds.map((s) => {
      const id = guid()
      return { ...s, id, memberId, createdAt: now, updatedAt: now }
    }),
  )
}

const getMemberId = (req: any): string | undefined => req.member?.id as string | undefined

/** 校验地址必填字段 */
function validateAddress(payload: Partial<Address>): string | null {
  if (!payload.name || payload.name.trim() === '') return '收货人姓名不能为空'
  if (!payload.phone || !/^1[3-9]\d{9}$/.test(payload.phone)) return '手机号格式不正确'
  if (!payload.province || !payload.city || !payload.district) return '省市区不能为空'
  if (!payload.detail || payload.detail.trim() === '') return '详细地址不能为空'
  return null
}

/** 拼接完整地址 */
function composeFull(a: Pick<Address, 'province' | 'city' | 'district' | 'detail'>): string {
  return `${a.province} ${a.city} ${a.district} ${a.detail}`
}

/**
 * 设置默认收货地址
 * 必须先于 PUT /:id 声明，避免被参数路由吞掉
 * PUT /api/app/addresses/:id/default
 */
appAddressRouter.put('/:id/default', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureInit(memberId)
  const list = addressStore.get(memberId) ?? []
  const target = list.find((a) => a.id === req.params.id)
  if (!target) return res.json(fail('地址不存在', 404))
  const now = isoTime()
  for (const a of list) {
    a.isDefault = a.id === target.id
    a.updatedAt = now
  }
  addressStore.set(memberId, list)
  res.json(ok(target, '已设为默认地址'))
})

/**
 * 获取当前会员的全部收货地址
 * GET /api/app/addresses
 */
appAddressRouter.get('/', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureInit(memberId)
  const list = (addressStore.get(memberId) ?? []).slice().sort((a, b) => {
    if (a.isDefault !== b.isDefault) return a.isDefault ? -1 : 1
    return a.createdAt < b.createdAt ? 1 : -1
  })
  res.json(ok(list))
})

/**
 * 获取单个收货地址详情
 * GET /api/app/addresses/:id
 */
appAddressRouter.get('/:id', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureInit(memberId)
  const found = (addressStore.get(memberId) ?? []).find((a) => a.id === req.params.id)
  if (!found) return res.json(fail('地址不存在', 404))
  res.json(ok(found))
})

/**
 * 新增收货地址
 * POST /api/app/addresses
 */
appAddressRouter.post('/', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureInit(memberId)
  const err = validateAddress(req.body)
  if (err) return res.json(fail(err))
  const now = isoTime()
  const list = addressStore.get(memberId) ?? []
  // 新增即默认：清掉旧的默认标记
  if (req.body.isDefault) {
    for (const a of list) a.isDefault = false
  }
  const created: Address = {
    id: guid(),
    memberId,
    name: String(req.body.name).trim(),
    phone: String(req.body.phone),
    province: String(req.body.province),
    city: String(req.body.city),
    district: String(req.body.district),
    detail: String(req.body.detail).trim(),
    isDefault: !!req.body.isDefault,
    fullAddress: composeFull(req.body),
    createdAt: now,
    updatedAt: now,
  }
  list.push(created)
  addressStore.set(memberId, list)
  res.json(ok(created, '地址已新增'))
})

/**
 * 更新收货地址
 * PUT /api/app/addresses/:id
 */
appAddressRouter.put('/:id', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureInit(memberId)
  const list = addressStore.get(memberId) ?? []
  const idx = list.findIndex((a) => a.id === req.params.id)
  if (idx === -1) return res.json(fail('地址不存在', 404))
  const merged = { ...list[idx], ...req.body, id: list[idx].id, memberId }
  const err = validateAddress(merged)
  if (err) return res.json(fail(err))
  const updated: Address = {
    ...merged,
    name: String(merged.name).trim(),
    detail: String(merged.detail).trim(),
    fullAddress: composeFull(merged),
    updatedAt: isoTime(),
  }
  list[idx] = updated
  // 若此次更新把该地址设为默认，则重置其他
  if (updated.isDefault) {
    const now = isoTime()
    for (const a of list) {
      if (a.id !== updated.id) a.isDefault = false
      a.updatedAt = now
    }
  }
  addressStore.set(memberId, list)
  res.json(ok(updated, '地址已更新'))
})

/**
 * 删除收货地址
 * DELETE /api/app/addresses/:id
 */
appAddressRouter.delete('/:id', (req, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureInit(memberId)
  const list = addressStore.get(memberId) ?? []
  const idx = list.findIndex((a) => a.id === req.params.id)
  if (idx === -1) return res.json(fail('地址不存在', 404))
  const removed = list[idx]
  list.splice(idx, 1)
  // 删除的是默认地址：自动把最新一条提升为默认
  if (removed.isDefault && list.length > 0) {
    list[list.length - 1].isDefault = true
    list[list.length - 1].updatedAt = isoTime()
  }
  addressStore.set(memberId, list)
  res.json(ok(null, '地址已删除'))
})
