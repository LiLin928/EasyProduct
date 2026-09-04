// types/product.types.ts —— 商品相关类型定义
import type { PageResult } from './api.types'

/** 商品分类 */
export interface Category {
  id: string
  name: string
  icon?: string
  sort?: number
  description?: string
}

/** 商品媒体类型 */
export type MediaType = 'image' | 'video'

/** 商品媒体项 */
export interface ProductMedia {
  id: string
  type: MediaType
  url: string
  thumbnail?: string
  duration?: number
}

/** 商品规格选项 */
export interface SpecOption {
  id: string
  name: string
  value: string
  stock?: number
  priceAdjust?: number
}

/** 商品规格组 */
export interface SpecGroup {
  id: string
  name: string
  required: boolean
  options: SpecOption[]
}

/** 商品信息 */
export interface Product {
  id: string
  name: string
  description?: string
  price: number
  originalPrice?: number
  image: string
  images?: string[]
  categoryId: string
  category?: Category
  stock: number
  sales?: number
  isHot?: boolean
  isNew?: boolean
  detail?: string
  createdAt?: string
  updatedAt?: string
}

/** 商品详情（扩展） */
export interface ProductDetail extends Product {
  media?: ProductMedia[]
  specs?: SpecGroup[]
  defaultSpecId?: string
  services?: string[]
  relatedProducts?: Product[]
}

/** SKU 信息 */
export interface SKU {
  id: string
  productId: string
  specCombination: string
  price: number
  stock: number
  image?: string
}

/** 商品评价 */
export interface ProductReview {
  id: string
  productId: string
  userId: string
  userName: string
  userAvatar?: string
  rating: number
  content: string
  images?: string[]
  reply?: string
  replyTime?: string
  isAnonymous: boolean
  likes: number
  createdAt: string
}

/** 商品查询参数 */
export interface ProductQuery {
  pageIndex: number
  pageSize: number
  categoryId?: string
  keyword?: string
  isHot?: boolean
  isNew?: boolean
  minPrice?: number
  maxPrice?: number
  sortField?: 'price' | 'sales' | 'createdAt'
  sortOrder?: 'asc' | 'desc'
}

/** 评价查询参数 */
export interface ReviewQuery {
  productId: string
  pageIndex: number
  pageSize: number
  rating?: number
  hasImage?: boolean
}

/** 商品列表结果 */
export type ProductListResult = PageResult<Product>

/** 评价列表结果 */
export type ReviewListResult = PageResult<ProductReview>
