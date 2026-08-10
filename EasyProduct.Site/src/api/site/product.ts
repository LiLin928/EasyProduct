// src/api/site/product.ts
import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Product, ProductQuery } from '@/types/site'

/** 产品列表 */
export const getProductList = (params: ProductQuery) =>
  get<PageResult<Product>>('/api/site/product/list', params)

/** 产品详情 */
export const getProductDetail = (id: string) =>
  get<Product>(`/api/site/product/${id}`)