// src/data/product.ts
// [backend: Site module | status: pending]
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
