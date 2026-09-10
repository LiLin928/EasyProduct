// api/product.ts —— 商品相关 API
import { request } from '../utils/request'
import type {
  Product,
  ProductDetail,
  Category,
  ProductQuery,
  ProductListResult,
  ProductReview,
  ReviewQuery,
  ReviewListResult,
} from '../types/product.types'

const BASE_URL = '/product'

/** 获取商品列表 */
export function getProductList(params: ProductQuery): Promise<ProductListResult> {
  return request<ProductListResult>({
    url: BASE_URL,
    method: 'GET',
    data: params,
  })
}

/** 获取商品详情 */
export function getProductDetail(id: string): Promise<ProductDetail> {
  return request<ProductDetail>({
    url: `${BASE_URL}/${id}`,
    method: 'GET',
  })
}

/** 获取商品分类 */
export function getCategories(): Promise<Category[]> {
  return request<Category[]>({
    url: `${BASE_URL}/categories`,
    method: 'GET',
  })
}

/** 搜索商品 */
export function searchProducts(keyword: string): Promise<Product[]> {
  return request<Product[]>({
    url: `${BASE_URL}/search`,
    method: 'GET',
    data: { keyword },
  })
}

/** 获取热销商品 */
export function getHotProducts(limit: number = 10): Promise<Product[]> {
  return request<Product[]>({
    url: `${BASE_URL}/hot`,
    method: 'GET',
    data: { limit },
  })
}

/** 获取新品推荐 */
export function getNewProducts(limit: number = 10): Promise<Product[]> {
  return request<Product[]>({
    url: `${BASE_URL}/new`,
    method: 'GET',
    data: { limit },
  })
}

/** 获取商品评价 */
export function getProductReviews(params: ReviewQuery): Promise<ReviewListResult> {
  return request<ReviewListResult>({
    url: `${BASE_URL}/${params.productId}/reviews`,
    method: 'GET',
    data: {
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
      rating: params.rating,
      hasImage: params.hasImage,
    },
  })
}
