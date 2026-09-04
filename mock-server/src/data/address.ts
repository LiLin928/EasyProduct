// src/data/address.ts
// [backend: Mall/Address module | status: pending]
// 会员收货地址 mock 数据。按会员 ID 分组存放，运行时修改，重启即重置。
import { MEMBERS } from './mall.js'

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

/** 种子地址（每个会员 1-2 条，其中一条默认） */
const seeds: Omit<Address, 'id' | 'memberId' | 'createdAt' | 'updatedAt'>[] = [
  { name: '张三', phone: '13800138001', province: '广东省', city: '深圳市', district: '南山区', detail: '科技园路 1 号 A 座 1801 室', isDefault: true,  fullAddress: '' },
  { name: '张三', phone: '13800138001', province: '广东省', city: '广州市', district: '天河区', detail: '珠江新城兴民路 222 号 5 栋 2 单元 803', isDefault: false, fullAddress: '' },
  { name: '李四', phone: '13900139002', province: '上海市', city: '上海市', district: '浦东新区', detail: '张江路 1238 号 7 号楼 502', isDefault: true, fullAddress: '' },
  { name: '王五', phone: '13700137003', province: '北京市', city: '北京市', district: '海淀区', detail: '中关村大街 27 号 905 大厦 1106 室', isDefault: true, fullAddress: '' },
  { name: '赵六', phone: '13600136004', province: '浙江省', city: '杭州市', district: '西湖区', detail: '文三路 478 号华星时代广场 12 层', isDefault: true, fullAddress: '' },
]

const buildFullAddress = (a: Pick<Address, 'province' | 'city' | 'district' | 'detail'>): string =>
  `${a.province} ${a.city} ${a.district} ${a.detail}`

/** 按会员 ID 索引的地址列表（仅用于初始化种子数据） */
export const seedAddressesByMember = new Map<string, Omit<Address, 'id' | 'memberId' | 'createdAt' | 'updatedAt'>[]>()

MEMBERS.slice(0, 5).forEach((m, idx) => {
  const a = seeds[idx % seeds.length]
  seedAddressesByMember.set(m.id, [{ ...a, fullAddress: buildFullAddress(a) }])
})
