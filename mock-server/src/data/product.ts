// src/data/product.ts
// [backend: Product module | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

export type CategoryStatus = 'enabled' | 'disabled'

export interface ProductCategory {
  id: string
  name: string
  nameEn: string
  parentId: string
  sort: number
  status: CategoryStatus
  createdAt: string
  updatedAt: string
}

export const CATEGORIES: ProductCategory[] = [
  { id: guid(), name: 'gongye shebei', nameEn: 'Industrial Equipment', parentId: '0', sort: 1, status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: 'dianzi chanpin', nameEn: 'Electronics', parentId: '0', sort: 2, status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
  { id: guid(), name: 'bangong yongpin', nameEn: 'Office Supplies', parentId: '0', sort: 3, status: 'enabled', createdAt: isoTime(), updatedAt: isoTime() },
]

export const PRODUCTS = Mock.mock({
  'list|50': [{
    id: '@guid',
    categoryId: () => CATEGORIES[Math.floor(Math.random() * CATEGORIES.length)].id,
    code: '@word(8)',
    name: '@ctitle(10,20)',
    nameEn: '@title(5,10)',
    summary: '@cparagraph(1)',
    summaryEn: '@sentence(5,10)',
    coverImage: '@image(800x600)',
    'images|3-6': ['@image(800x600)'],
    'price|100-10000.2': 1,
    unit: 'tai',
    specs: JSON.stringify({ weight: '@float(1,100,2,2)kg', size: 'LxWxH mm' }),
  status: 'active',
  createdAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((p) => ({ ...p, id: guid(), createdAt: isoTime() }))

// ---- SPU / SKU / Channel ----

export type SpuType = 'ticket' | 'material'
export type SpuStatus = 'active' | 'inactive'

export interface SpecGroup {
  name: string
  values: string[]
}

export interface ProductSpu {
  id: string
  code: string
  name: string
  nameEn: string
  categoryId: string
  type: SpuType
  mainImage: string
  images: string[]
  description: string
  unit: string
  brand: string
  specs: SpecGroup[]
  status: SpuStatus
  createdAt: string
  updatedAt: string
}

export type SkuStatus = 'active' | 'inactive'

export interface ProductSku {
  id: string
  spuId: string
  specValues: Record<string, string>
  barcode: string
  retailPrice: number
  memberPrice: number
  b2bPrice: number
  costPrice: number
  stock: number
  status: SkuStatus
  createdAt: string
  updatedAt: string
}

export type ChannelType = 'site' | 'miniapp' | 'b2b'

export interface ProductChannel {
  id: string
  spuId: string
  channel: ChannelType
  published: boolean
  sort: number
  createdAt: string
  updatedAt: string
}

const SPECS: SpecGroup[] = [
  { name: '颜色', values: ['红色', '蓝色', '黑色'] },
  { name: '规格', values: ['标准', '加大'] },
]

function generateSpecCombinations(specs: SpecGroup[]): Record<string, string>[] {
  if (specs.length === 0) return [{}]
  const result: Record<string, string>[] = [{}]
  for (const spec of specs) {
    const next: Record<string, string>[] = []
    for (const prev of result) {
      for (const val of spec.values) {
        next.push({ ...prev, [spec.name]: val })
      }
    }
    result.length = 0
    result.push(...next)
  }
  return result
}

export const SPUS: ProductSpu[] = Mock.mock({
  'list|30': [{
    id: '@guid',
    code: 'SPU@word(6)',
    name: '@ctitle(4,10)',
    nameEn: '@title(3,6)',
    categoryId: () => CATEGORIES[Math.floor(Math.random() * CATEGORIES.length)].id,
    type: '@pick(["ticket", "material"])',
    mainImage: '@image("800x600")',
    'images|3-6': ['@image("800x600")'],
    description: '@cparagraph(3,8)',
    unit: '@pick(["台", "套", "件", "个"])',
    brand: '@pick(["BrandA", "BrandB", "BrandC", "BrandD"])',
    specs: SPECS,
    status: '@pick(["active", "inactive"])',
    createdAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
    updatedAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((s: Record<string, unknown>) => ({
  ...s,
  id: guid(),
  createdAt: isoTime(),
  updatedAt: isoTime(),
})) as ProductSpu[]

export const SKUS: ProductSku[] = SPUS.flatMap((spu) => {
  const combos = generateSpecCombinations(spu.specs)
  return combos.map((combo, idx) => ({
    id: guid(),
    spuId: spu.id,
    specValues: combo,
    barcode: `BAR${Mock.mock('@string("upper", 10)')}`,
    retailPrice: Mock.mock('@float(100, 10000, 2, 2)') as number,
    memberPrice: Mock.mock('@float(80, 8000, 2, 2)') as number,
    b2bPrice: Mock.mock('@float(70, 7000, 2, 2)') as number,
    costPrice: Mock.mock('@float(50, 5000, 2, 2)') as number,
    stock: Mock.mock('@integer(0, 1000)') as number,
    status: (idx % 5 === 0 ? 'inactive' : 'active') as SkuStatus,
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }))
})

export const CHANNELS: ProductChannel[] = SPUS.flatMap((spu) => {
  const channels: ChannelType[] = ['site', 'miniapp', 'b2b']
  return channels.map((ch, idx) => ({
    id: guid(),
    spuId: spu.id,
    channel: ch,
    published: Mock.mock('@boolean(7, 3)') as boolean,
    sort: idx + 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }))
})
