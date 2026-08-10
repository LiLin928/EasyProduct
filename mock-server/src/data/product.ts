// src/data/product.ts
// [backend: Site 模块 | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

// 产品分类
export const CATEGORIES = [
  { id: guid(), name: '工业设备', nameEn: 'Industrial Equipment', parentId: '0', sort: 1 },
  { id: guid(), name: '电子产品', nameEn: 'Electronics', parentId: '0', sort: 2 },
  { id: guid(), name: '办公用品', nameEn: 'Office Supplies', parentId: '0', sort: 3 },
]

// 产品列表（mockjs 生成 50 条）
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
    unit: '台',
    specs: JSON.stringify({ weight: '@float(1,100,2,2)kg', size: 'LxWxH mm' }),
    status: 'active',
    createdAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((p: Record<string, unknown>) => ({ ...p, id: guid(), createdAt: isoTime() }))