// src/data/address.ts
// [backend: Mall/Address module | status: pending]
// 会员收货地址 mock 数据。
// 运行时 Map<memberId, Address[]> 与 app/admin 两端共享;重启即重置(非持久化)。
// 业务规则:默认地址唯一(同 memberId 仅一条 isDefault=true);
// 删除默认地址时自动提升同 memberId 最新一条 createdAt 为默认;
// fullAddress 由服务端拼接 {province} {city} {district} {detail},前端只读。
import { MEMBERS } from './mall.js'
import { guid, isoTime } from '../helpers/id.js'
import { registerReset } from '../helpers/registry.js'

export interface Address {
  id: string
  memberId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string
  isDefault: boolean
  fullAddress: string
  createdAt: string
  updatedAt: string
}

/** 地址表单入参(创建/更新共用) */
export interface AddressInput {
  name?: string
  phone?: string
  province?: string
  city?: string
  district?: string
  detail?: string
  isDefault?: boolean
}

/** 地址创建/更新结果 */
export interface AddressMutationResult {
  ok: boolean
  data?: Address
  /** 删除操作:被删的地址 + 是否因删默认地址而提升了一条新默认 */
  removed?: Address
  promoted?: Address
  message?: string
}

/** 种子地址(每个会员 1-2 条,其中一条默认) */
const seeds: Array<Omit<Address, 'id' | 'memberId' | 'createdAt' | 'updatedAt'>> = [
  { name: '张三', phone: '13800138001', province: '广东省', city: '深圳市', district: '南山区', detail: '科技园路 1 号 A 座 1801 室', isDefault: true,  fullAddress: '' },
  { name: '张三', phone: '13800138001', province: '广东省', city: '广州市', district: '天河区', detail: '珠江新城兴民路 222 号 5 栋 2 单元 803', isDefault: false, fullAddress: '' },
  { name: '李四', phone: '13900139002', province: '上海市', city: '上海市', district: '浦东新区', detail: '张江路 1238 号 7 号楼 502', isDefault: true, fullAddress: '' },
  { name: '王五', phone: '13700137003', province: '北京市', city: '北京市', district: '海淀区', detail: '中关村大街 27 号 905 大厦 1106 室', isDefault: true, fullAddress: '' },
  { name: '赵六', phone: '13600136004', province: '浙江省', city: '杭州市', district: '西湖区', detail: '文三路 478 号华星时代广场 12 层', isDefault: true, fullAddress: '' },
]

/** 拼接完整地址 */
const buildFullAddress = (a: { province: string; city: string; district: string; detail: string }): string =>
  `${a.province} ${a.city} ${a.district} ${a.detail}`

/** 校验手机号 */
const isValidPhone = (phone: string): boolean => /^1[3-9]\d{9}$/.test(phone)

/** 校验地址必填字段 */
const validateAddressInput = (payload: AddressInput): string | null => {
  if (!payload.name || payload.name.trim() === '') return '收货人姓名不能为空'
  if (!payload.phone || !isValidPhone(payload.phone)) return '手机号格式不正确'
  if (!payload.province || !payload.city || !payload.district) return '省市区不能为空'
  if (!payload.detail || payload.detail.trim() === '') return '详细地址不能为空'
  return null
}

/** 按会员 ID 索引的种子(仅用于初始化种子数据) */
export const seedAddressesByMember = new Map<string, Array<Omit<Address, 'id' | 'memberId' | 'createdAt' | 'updatedAt'>>>()

MEMBERS.slice(0, 5).forEach((m, idx) => {
  const a = seeds[idx % seeds.length]
  seedAddressesByMember.set(m.id, [{ ...a, fullAddress: buildFullAddress(a) }])
})

/**
 * 运行时共享地址存储 Map<memberId, Address[]>
 * - 与 app 端(/api/app/addresses)和 admin 端(/api/admin/mall/address)共享
 * - 进程内可变;非持久化;hot reload 会丢数据(开发期行为)
 */
export const ADDRESSES: Map<string, Address[]> = new Map<string, Address[]>()

/** lazy 初始化:首次访问某 memberId 时从种子铺底 */
export function ensureAddressInit(memberId: string): void {
  if (ADDRESSES.has(memberId)) return
  const seedsForMember = seedAddressesByMember.get(memberId) ?? []
  const now = isoTime()
  const initial: Address[] = seedsForMember.map((s) => {
    const id = guid()
    return { ...s, id, memberId, createdAt: now, updatedAt: now }
  })
  ADDRESSES.set(memberId, initial)
}

/** 重置:供 __mock/reset 使用 */
export function resetAddresses(): void {
  ADDRESSES.clear()
}

/** 列出某会员的全部地址(返回副本,避免外部直接改 Map 内部数组) */
export function listAddressesByMember(memberId: string): Address[] {
  ensureAddressInit(memberId)
  return (ADDRESSES.get(memberId) ?? []).slice()
}

/**
 * 列出某会员的全部地址(默认地址排首,其余按 createdAt 倒序)
 * app 端 GET /api/app/addresses 使用
 */
export function listAddressesByMemberSorted(memberId: string): Address[] {
  return listAddressesByMember(memberId).sort((a, b) => {
    if (a.isDefault !== b.isDefault) return a.isDefault ? -1 : 1
    return a.createdAt < b.createdAt ? 1 : -1
  })
}

/** 查单个地址 */
export function findAddress(memberId: string, id: string): Address | undefined {
  ensureAddressInit(memberId)
  return (ADDRESSES.get(memberId) ?? []).find((a) => a.id === id)
}

/** 校验手机号(导出供路由层复用) */
export function validateAddressPhone(phone: string): boolean {
  return isValidPhone(phone)
}

/**
 * 新增地址。维护"同 memberId 仅一条 isDefault"。
 */
export function addAddress(memberId: string, payload: AddressInput): AddressMutationResult {
  ensureAddressInit(memberId)
  const err = validateAddressInput(payload)
  if (err) return { ok: false, message: err }
  const now = isoTime()
  const list = ADDRESSES.get(memberId) ?? []
  const wantDefault = !!payload.isDefault
  if (wantDefault) {
    for (const a of list) a.isDefault = false
  }
  const created: Address = {
    id: guid(),
    memberId,
    name: String(payload.name ?? '').trim(),
    phone: String(payload.phone ?? ''),
    province: String(payload.province ?? ''),
    city: String(payload.city ?? ''),
    district: String(payload.district ?? ''),
    detail: String(payload.detail ?? '').trim(),
    isDefault: wantDefault,
    fullAddress: buildFullAddress({
      province: String(payload.province ?? ''),
      city: String(payload.city ?? ''),
      district: String(payload.district ?? ''),
      detail: String(payload.detail ?? ''),
    }),
    createdAt: now,
    updatedAt: now,
  }
  list.push(created)
  ADDRESSES.set(memberId, list)
  return { ok: true, data: created }
}

/**
 * 更新地址。若把某地址设为默认,则清掉同 memberId 其它默认。
 */
export function updateAddress(memberId: string, id: string, payload: AddressInput): AddressMutationResult {
  ensureAddressInit(memberId)
  const list = ADDRESSES.get(memberId) ?? []
  const idx = list.findIndex((a) => a.id === id)
  if (idx === -1) return { ok: false, message: '地址不存在' }
  const merged = {
    ...list[idx],
    ...payload,
    id: list[idx].id,
    memberId,
  }
  const err = validateAddressInput(merged)
  if (err) return { ok: false, message: err }
  const now = isoTime()
  const updated: Address = {
    ...merged,
    name: String(merged.name ?? '').trim(),
    detail: String(merged.detail ?? '').trim(),
    fullAddress: buildFullAddress({
      province: String(merged.province ?? ''),
      city: String(merged.city ?? ''),
      district: String(merged.district ?? ''),
      detail: String(merged.detail ?? ''),
    }),
    updatedAt: now,
  }
  list[idx] = updated
  if (updated.isDefault) {
    for (const a of list) {
      if (a.id !== updated.id) a.isDefault = false
      a.updatedAt = now
    }
  }
  ADDRESSES.set(memberId, list)
  return { ok: true, data: updated }
}

/**
 * 设置默认地址。必须先于 updateAddress 提供清晰的"仅切换默认"语义。
 */
export function setDefaultAddress(memberId: string, id: string): AddressMutationResult {
  ensureAddressInit(memberId)
  const list = ADDRESSES.get(memberId) ?? []
  const target = list.find((a) => a.id === id)
  if (!target) return { ok: false, message: '地址不存在' }
  const now = isoTime()
  for (const a of list) {
    a.isDefault = a.id === target.id
    a.updatedAt = now
  }
  ADDRESSES.set(memberId, list)
  return { ok: true, data: target, message: '已设为默认地址' }
}

/**
 * 删除地址。若删的是默认地址,自动把同 memberId 最新一条(createdAt 最大)提升为默认。
 */
export function deleteAddress(memberId: string, id: string): AddressMutationResult {
  ensureAddressInit(memberId)
  const list = ADDRESSES.get(memberId) ?? []
  const idx = list.findIndex((a) => a.id === id)
  if (idx === -1) return { ok: false, message: '地址不存在' }
  const removed = list[idx]
  list.splice(idx, 1)
  let promoted: Address | undefined
  if (removed.isDefault && list.length > 0) {
    const newest = list.reduce((acc, cur) => (acc.createdAt >= cur.createdAt ? acc : cur), list[0])
    newest.isDefault = true
    newest.updatedAt = isoTime()
    promoted = newest
  }
  ADDRESSES.set(memberId, list)
  return { ok: true, removed, promoted, message: '地址已删除' }
}

/** 注册到 __mock/reset */
registerReset(resetAddresses)
