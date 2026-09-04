// src/routes/app/address.ts
// 会员收货地址路由(小程序侧)
// 契约:与 docs/backend-guidelines.md 第 5 节统一信封一致
// 响应:{ code, message, data, timestamp },HTTP 200,code===200 成功
// 主键:GUID(camelCase),列表直接返回 Address[]
// 鉴权:appGuard 已挂在 /api/app 全局上,由 req.member.id 区分多会员
// 数据层:使用 data/address.ts 的 ADDRESSES 共享 Map,与 admin 端共享底层数据
// 路由顺序:/default 必须先于 /:id 声明(Express 按声明顺序匹配,PUT /:id 会吞掉 /:id/default)
import { Router, type Request } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import {
  addAddress,
  deleteAddress,
  ensureAddressInit,
  findAddress,
  listAddressesByMemberSorted,
  setDefaultAddress,
  updateAddress,
} from '../../data/address.js'

export const appAddressRouter = Router()

type AuthedRequest = Request & { member?: { id?: string } }
const getMemberId = (req: AuthedRequest): string | undefined => req.member?.id

/**
 * 设置默认收货地址(必须先于 PUT /:id 声明,避免被参数路由吞掉)
 * PUT /api/app/addresses/:id/default
 */
appAddressRouter.put('/:id/default', (req: AuthedRequest, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureAddressInit(memberId)
  const result = setDefaultAddress(memberId, req.params.id)
  if (!result.ok) return res.json(fail(result.message ?? '操作失败', 404))
  return res.json(ok(result.data, result.message))
})

/**
 * 获取当前会员的全部收货地址(默认地址排首)
 * GET /api/app/addresses
 */
appAddressRouter.get('/', (req: AuthedRequest, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureAddressInit(memberId)
  res.json(ok(listAddressesByMemberSorted(memberId)))
})

/**
 * 获取单个收货地址详情
 * GET /api/app/addresses/:id
 */
appAddressRouter.get('/:id', (req: AuthedRequest, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureAddressInit(memberId)
  const found = findAddress(memberId, req.params.id)
  if (!found) return res.json(fail('地址不存在', 404))
  res.json(ok(found))
})

/**
 * 新增收货地址
 * POST /api/app/addresses
 */
appAddressRouter.post('/', (req: AuthedRequest, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureAddressInit(memberId)
  const result = addAddress(memberId, req.body ?? {})
  if (!result.ok) return res.json(fail(result.message ?? '参数错误'))
  return res.json(ok(result.data, '地址已新增'))
})

/**
 * 更新收货地址
 * PUT /api/app/addresses/:id
 */
appAddressRouter.put('/:id', (req: AuthedRequest, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureAddressInit(memberId)
  const result = updateAddress(memberId, req.params.id, req.body ?? {})
  if (!result.ok) {
    const code = result.message === '地址不存在' ? 404 : 400
    return res.json(fail(result.message ?? '参数错误', code))
  }
  return res.json(ok(result.data, '地址已更新'))
})

/**
 * 删除收货地址
 * DELETE /api/app/addresses/:id
 */
appAddressRouter.delete('/:id', (req: AuthedRequest, res) => {
  const memberId = getMemberId(req)
  if (!memberId) return res.json(fail('请先登录', 401))
  ensureAddressInit(memberId)
  const result = deleteAddress(memberId, req.params.id)
  if (!result.ok) return res.json(fail(result.message ?? '操作失败', 404))
  return res.json(ok(null, result.message))
})
