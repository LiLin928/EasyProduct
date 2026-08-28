import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  ProductSpu,
  ProductSpuWithSkus,
  ProductSku,
  SpuQuery,
  SpuParams,
  SkuUpdateParams,
} from '@/types/product'

/** SPU 列表（分页 + 筛选） */
export const getSpuList = (params: SpuQuery) =>
  get<PageResult<ProductSpu>>('/api/admin/product/spu/list', params)

/** SPU 详情（含 SKU 列表） */
export const getSpuDetail = (id: string) =>
  get<ProductSpuWithSkus>(`/api/admin/product/spu/${id}`)

/** 新建 SPU */
export const createSpu = (data: SpuParams) =>
  post<{ id: string }>('/api/admin/product/spu', data)

/** 更新 SPU */
export const updateSpu = (id: string, data: SpuParams) =>
  put<null>(`/api/admin/product/spu/${id}`, data)

/** 删除 SPU */
export const deleteSpu = (id: string) =>
  del<null>(`/api/admin/product/spu/${id}`)

/** 批量更新 SKU */
export const batchUpdateSkus = (data: SkuUpdateParams[]) =>
  put<null>('/api/admin/product/sku/batch', data)

/** 单个 SKU 更新 */
export const updateSku = (id: string, data: SkuUpdateParams) =>
  put<null>(`/api/admin/product/sku/${id}`, data)
