// src/api/site/category.ts
import { get } from '@/utils/request'
import type { ProductCategory } from '@/types/site'

/** 分类列表 */
export const getCategoryList = () =>
  get<ProductCategory[]>('/api/site/category/list')