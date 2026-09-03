/** 官网 Banner */
export interface Banner {
  id: string
  title: string
  titleEn: string
  imageUrl: string
  link: string
}

/** 官网新闻 */
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

/** 视频分类 */
export interface VideoCategory {
  id: string
  name: string
  nameEn: string
  sort: number
}

/** 视频 */
export interface Video {
  id: string
  categoryId: string
  title: string
  titleEn: string
  coverImage: string
  videoUrl: string
  duration: number
  viewCount: number
  publishTime: string
}

/** 下载分类 */
export interface DownloadCategory {
  id: string
  name: string
  nameEn: string
  sort: number
}

/** 下载 */
export interface Download {
  id: string
  categoryId: string
  title: string
  titleEn: string
  fileUrl: string
  fileSize: number
  downloadCount: number
  publishTime: string
}

/** 关于 */
export interface About {
  id: string
  title: string
  titleEn: string
  content: string
  contentEn: string
  updatedAt: string
}

/** 联系信息 */
export interface ContactInfo {
  address: string
  addressEn: string
  phone: string
  email: string
  workingHours: string
  workingHoursEn: string
}
