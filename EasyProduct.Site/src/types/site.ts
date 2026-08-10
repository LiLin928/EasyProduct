/** 官网 Banner */
export interface Banner {
  id: string
  title: string
  titleEn: string
  imageUrl: string
  link: string
}

/** 官网新闻（内容中英字段由后端存储，不进语言包——规范 3.3） */
export interface NewsItem {
  id: string
  categoryId: string
  title: string
  titleEn: string
  summary: string
  coverImage: string
  isTop: boolean
  viewCount: number
  publishTime: string
}

/** 新闻查询 */
export interface NewsQuery {
  pageIndex: number
  pageSize: number
  keyword?: string
}

/** 产品分类 */
export interface ProductCategory {
  id: string
  name: string
  nameEn: string
  parentId: string
  sort: number
}

/** 产品 */
export interface Product {
  id: string
  categoryId: string
  code: string
  name: string
  nameEn: string
  summary: string
  summaryEn: string
  coverImage: string
  images: string[]
  price: number
  unit: string
  specs: string
  status: string
  createdAt: string
}

/** 产品查询参数 */
export interface ProductQuery {
  pageIndex: number
  pageSize: number
  categoryId?: string
  keyword?: string
}

/** 新闻详情 */
export interface NewsDetail {
  id: string
  categoryId: string
  title: string
  titleEn: string
  summary: string
  summaryEn: string
  content: string
  contentEn: string
  coverImage: string
  isTop: boolean
  viewCount: number
  publishTime: string
}

/** 新闻查询参数 */
export interface NewsQuery {
  pageIndex: number
  pageSize: number
  keyword?: string
}
