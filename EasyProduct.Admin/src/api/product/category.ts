import { get, post, put, del } from '@/utils/request'
import type { ProductCategory, CategoryParams } from '@/types/product'

/** 获取商品分类树（管理端） */
export const getCategoryTree = () =>
  get<ProductCategory[]>('/api/admin/product/category/list')

/** 创建商品分类 */
export const createCategory = (data: CategoryParams) =>
  post<{ id: string }>('/api/admin/product/category', data)

/** 更新商品分类 */
export const updateCategory = (id: string, data: CategoryParams) =>
  put<null>(`/api/admin/product/category/${id}`, data)

/** 删除商品分类 */
export const deleteCategory = (id: string) =>
  del<null>(`/api/admin/product/category/${id}`)
