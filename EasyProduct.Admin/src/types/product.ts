// src/types/product.ts

import type { PageQuery } from './api'

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

export interface CategoryParams {
  name: string
  nameEn?: string
  parentId?: string
  sort?: number
  status?: CategoryStatus
}

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

export interface ProductSpuWithSkus extends ProductSpu {
  skus: ProductSku[]
}

export interface SpuQuery extends PageQuery {
  name?: string
  code?: string
  categoryId?: string
  status?: SpuStatus
  type?: SpuType
}

export interface SpuParams {
  code: string
  name: string
  nameEn?: string
  categoryId?: string
  type?: SpuType
  mainImage?: string
  images?: string[]
  description?: string
  unit?: string
  brand?: string
  specs?: SpecGroup[]
  status?: SpuStatus
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

export interface SkuUpdateParams {
  id: string
  barcode?: string
  retailPrice?: number
  memberPrice?: number
  b2bPrice?: number
  costPrice?: number
  stock?: number
  status?: SkuStatus
}

export type ChannelType = 'site' | 'miniapp' | 'b2b'

export interface ChannelInfo {
  id: string
  channel: ChannelType
  published: boolean
  sort: number
}

export interface ProductChannelRow {
  spuId: string
  spuName: string
  spuCode: string
  mainImage: string
  channels: ChannelInfo[]
}

export interface ChannelQuery extends PageQuery {
  spuName?: string
  channel?: ChannelType
  published?: boolean
}

export interface ChannelToggleParams {
  spuId: string
  channel: ChannelType
  published: boolean
}

export interface ChannelSortParams {
  spuId: string
  channel: ChannelType
  sort: number
}

export const SPU_TYPE_OPTIONS = [
  { label: 'product.spu.typeTicket', value: 'ticket' as const },
  { label: 'product.spu.typeMaterial', value: 'material' as const },
]

export const SPU_STATUS_OPTIONS = [
  { label: 'product.spu.statusActive', value: 'active' as const },
  { label: 'product.spu.statusInactive', value: 'inactive' as const },
]

export const SKU_STATUS_OPTIONS = [
  { label: 'product.sku.statusActive', value: 'active' as const },
  { label: 'product.sku.statusInactive', value: 'inactive' as const },
]

export const CHANNEL_TYPE_OPTIONS = [
  { label: 'product.channel.typeSite', value: 'site' as const },
  { label: 'product.channel.typeMiniapp', value: 'miniapp' as const },
  { label: 'product.channel.typeB2b', value: 'b2b' as const },
]
