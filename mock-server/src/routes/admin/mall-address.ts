// src/routes/admin/mall-address.ts
// Admin 端会员收货地址管理路由
// 契约:与 docs/backend-guidelines.md 第 5 节统一信封一致
// 响应:{ code, message, data, timestamp },HTTP 200,code===200 成功
// 主键:GUID(camelCase),分页 pageIndex/pageSize,出参 list/total
// 路由:/api/admin/mall/address/**
// 路由顺序:/:id/default 必须先于 /:id 声明(Express 按声明顺序匹配)
import { Router } from 'express'
import { fail, ok } from '../../helpers/envelope.js'
import {
  ADDRESSES,
  ensureAddressInit,
  addAddress,
  updateAddress,
  deleteAddress,
  setDefaultAddress,
  type Address,
  type AddressInput,
} from '../../data/address.js'
import { MEMBERS } from '../../data/mall.js'

export const adminMallAddressRouter = Router()

/** 构建地址列表项(含会员信息) */
const buildAddressListItem = (addr: Address): AddressListItem => {
  const member = MEMBERS.find((m) => m.id === addr.memberId)
  return {
    ...addr,
    memberName: member?.nickname ?? '',
    memberPhone: member?.phone ?? '',
  }
}

/** 地址列表项类型(含会员信息) */
interface AddressListItem extends Address {
  memberName: string
  memberPhone: string
}

/** 分页参数 */
interface PageQuery {
  pageIndex?: number
  pageSize?: number
}

/** 地址查询参数 */
interface AddressQuery extends PageQuery {
  memberId?: string
  memberName?: string
  phone?: string
  province?: string
  city?: string
  keyword?: string
}

/**
 * 设置默认收货地址(必须先于 PUT /:id 声明,避免被参数路由吞掉)
 * PUT /api/admin/mall/address/:id/default
 */
adminMallAddressRouter.put('/mall/address/:id/default', (req, res) => {
  const { id } = req.params
  // 查找地址所属会员
  let memberId: string | undefined
  for (const [mid, list] of ADDRESSES.entries()) {
    if (list.some((a) => a.id === id)) {
      memberId = mid
      break
    }
  }
  if (!memberId) {
    // 尝试从所有会员的地址中查找
    for (const m of MEMBERS) {
      ensureAddressInit(m.id)
      const list = ADDRESSES.get(m.id) ?? []
      if (list.some((a) => a.id === id)) {
        memberId = m.id
        break
      }
    }
  }
  if (!memberId) return res.json(fail('地址不存在', 404))
  const result = setDefaultAddress(memberId, id)
  if (!result.ok) return res.json(fail(result.message ?? '操作失败', 404))
  return res.json(ok(buildAddressListItem(result.data!), '已设为默认地址'))
})

/**
 * 分页查询会员收货地址列表
 * GET /api/admin/mall/address/list
 */
adminMallAddressRouter.get('/mall/address/list', (req, res) => {
  const query = req.query as AddressQuery
  const pageIndex = Math.max(1, Number(query.pageIndex ?? 1))
  const pageSize = Math.max(1, Math.min(100, Number(query.pageSize ?? 10)))

  // 收集所有会员地址
  const allAddresses: AddressListItem[] = []
  for (const m of MEMBERS) {
    ensureAddressInit(m.id)
    const list = ADDRESSES.get(m.id) ?? []
    for (const addr of list) {
      allAddresses.push(buildAddressListItem(addr))
    }
  }

  // 筛选
  let filtered = allAddresses
  if (query.memberId) {
    filtered = filtered.filter((a) => a.memberId === query.memberId)
  }
  if (query.memberName?.trim()) {
    const kw = query.memberName.trim().toLowerCase()
    filtered = filtered.filter((a) => a.memberName.toLowerCase().includes(kw))
  }
  if (query.phone?.trim()) {
    const kw = query.phone.trim()
    filtered = filtered.filter((a) => a.memberPhone.includes(kw) || a.phone.includes(kw))
  }
  if (query.province?.trim()) {
    const kw = query.province.trim()
    filtered = filtered.filter((a) => a.province.includes(kw))
  }
  if (query.city?.trim()) {
    const kw = query.city.trim()
    filtered = filtered.filter((a) => a.city.includes(kw))
  }
  if (query.keyword?.trim()) {
    const kw = query.keyword.trim().toLowerCase()
    filtered = filtered.filter(
      (a) =>
        a.name.toLowerCase().includes(kw) ||
        a.phone.includes(kw) ||
        a.detail.toLowerCase().includes(kw) ||
        a.fullAddress.toLowerCase().includes(kw)
    )
  }

  // 排序:默认地址优先,然后按创建时间倒序
  filtered.sort((a, b) => {
    if (a.isDefault !== b.isDefault) return a.isDefault ? -1 : 1
    return a.createdAt < b.createdAt ? 1 : -1
  })

  const total = filtered.length
  const start = (pageIndex - 1) * pageSize
  const list = filtered.slice(start, start + pageSize)

  return res.json(ok({ list, total }))
})

/**
 * 获取单个收货地址详情
 * GET /api/admin/mall/address/:id
 */
adminMallAddressRouter.get('/mall/address/:id', (req, res) => {
  const { id } = req.params
  // 查找地址
  let found: Address | undefined
  for (const list of ADDRESSES.values()) {
    found = list.find((a) => a.id === id)
    if (found) break
  }
  if (!found) {
    // 尝试初始化所有会员地址后再查找
    for (const m of MEMBERS) {
      ensureAddressInit(m.id)
      const list = ADDRESSES.get(m.id) ?? []
      found = list.find((a) => a.id === id)
      if (found) break
    }
  }
  if (!found) return res.json(fail('地址不存在', 404))
  return res.json(ok(buildAddressListItem(found)))
})

/**
 * 新增会员收货地址
 * POST /api/admin/mall/address
 */
adminMallAddressRouter.post('/mall/address', (req, res) => {
  const body = req.body as AddressInput & { memberId?: string }
  const memberId = body.memberId
  if (!memberId) return res.json(fail('会员ID不能为空', 400))
  // 检查会员是否存在
  const member = MEMBERS.find((m) => m.id === memberId)
  if (!member) return res.json(fail('会员不存在', 404))

  ensureAddressInit(memberId)
  const result = addAddress(memberId, body)
  if (!result.ok) return res.json(fail(result.message ?? '参数错误', 400))
  return res.json(ok(buildAddressListItem(result.data!), '地址已新增'))
})

/**
 * 更新会员收货地址
 * PUT /api/admin/mall/address/:id
 */
adminMallAddressRouter.put('/mall/address/:id', (req, res) => {
  const { id } = req.params
  const body = req.body as AddressInput & { memberId?: string }
  // 查找地址所属会员
  let memberId: string | undefined
  for (const [mid, list] of ADDRESSES.entries()) {
    if (list.some((a) => a.id === id)) {
      memberId = mid
      break
    }
  }
  if (!memberId && body.memberId) {
    // 尝试从 body 获取
    memberId = body.memberId
  }
  if (!memberId) {
    // 尝试初始化所有会员地址后再查找
    for (const m of MEMBERS) {
      ensureAddressInit(m.id)
      const list = ADDRESSES.get(m.id) ?? []
      if (list.some((a) => a.id === id)) {
        memberId = m.id
        break
      }
    }
  }
  if (!memberId) return res.json(fail('地址不存在', 404))

  const result = updateAddress(memberId, id, body)
  if (!result.ok) {
    const code = result.message === '地址不存在' ? 404 : 400
    return res.json(fail(result.message ?? '参数错误', code))
  }
  return res.json(ok(buildAddressListItem(result.data!), '地址已更新'))
})

/**
 * 删除会员收货地址
 * DELETE /api/admin/mall/address/:id
 */
adminMallAddressRouter.delete('/mall/address/:id', (req, res) => {
  const { id } = req.params
  // 查找地址所属会员
  let memberId: string | undefined
  for (const [mid, list] of ADDRESSES.entries()) {
    if (list.some((a) => a.id === id)) {
      memberId = mid
      break
    }
  }
  if (!memberId) {
    // 尝试初始化所有会员地址后再查找
    for (const m of MEMBERS) {
      ensureAddressInit(m.id)
      const list = ADDRESSES.get(m.id) ?? []
      if (list.some((a) => a.id === id)) {
        memberId = m.id
        break
      }
    }
  }
  if (!memberId) return res.json(fail('地址不存在', 404))

  const result = deleteAddress(memberId, id)
  if (!result.ok) return res.json(fail(result.message ?? '操作失败', 404))
  return res.json(ok(null, result.message))
})
