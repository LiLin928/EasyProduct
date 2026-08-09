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
