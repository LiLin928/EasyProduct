import { get, post, put, del } from '@/utils/request'
import type { ProductCategory, CategoryParams } from '@/types/site'

/** 获取分类树（管理端） */
export const getCategoryTree = () =>
  get<ProductCategory[]>('/api/admin/site/category/list')

/** 创建分类 */
export const createCategory = (data: CategoryParams) =>
  post<{ id: string }>('/api/admin/site/category', data)

/** 更新分类 */
export const updateCategory = (id: string, data: CategoryParams) =>
  put<null>(`/api/admin/site/category/${id}`, data)

/** 删除分类 */
export const deleteCategory = (id: string) =>
  del<null>(`/api/admin/site/category/${id}`)
