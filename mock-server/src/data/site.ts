// src/data/site.ts
// [backend: Site module | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

// ---- News Category ----

export type Status = 0 | 1 // 0=禁用，1=启用

export interface NewsCategory {
  id: string
  categoryName: string
  categoryCode: string
  sort: number
  status: Status
  createdAt: string
  updatedAt: string
  createdBy: string
}

export const NEWS_CATEGORIES: NewsCategory[] = [
  {
    id: guid(),
    categoryName: '公司新闻',
    categoryCode: 'company_news',
    sort: 1,
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  {
    id: guid(),
    categoryName: '行业动态',
    categoryCode: 'industry_news',
    sort: 2,
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  {
    id: guid(),
    categoryName: '产品资讯',
    categoryCode: 'product_news',
    sort: 3,
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  {
    id: guid(),
    categoryName: '媒体报道',
    categoryCode: 'media_news',
    sort: 4,
    status: 0, // 禁用状态
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
]

// ---- News ----

export interface News {
  id: string
  categoryId: string
  categoryName: string
  title: string
  summary: string
  content: string
  coverImage: string
  author: string
  source: string
  viewCount: number
  publishTime: string
  isTop: 0 | 1 // 0=不置顶，1=置顶
  sort: number
  status: Status
  createdAt: string
  updatedAt: string
  createdBy: string
}

const newsTitles = [
  '公司成功获得新一轮融资',
  '新产品发布会在京举行',
  '公司荣获行业创新奖',
  '年度销售业绩再创新高',
  '与知名企业达成战略合作',
  '参加国际展会圆满成功',
  '公司技术团队获得专利认证',
  '员工团建活动圆满结束',
  '公司社会责任报告发布',
  '新产品市场反响热烈',
  '公司被评为行业领军企业',
  '技术研讨会成功举办',
  '公司年度总结大会召开',
  '新产品研发取得重大突破',
  '公司品牌形象全面升级',
]

export const NEWS_LIST: News[] = newsTitles.map((title, i) => {
  const category = NEWS_CATEGORIES[i % 3] // 只使用启用的前3个分类
  return {
    id: guid(),
    categoryId: category.id,
    categoryName: category.categoryName,
    title: title,
    summary: Mock.mock('@cparagraph(1, 2)') as string,
    content: Mock.mock('@cparagraph(5, 10)') as string,
    coverImage: `https://picsum.photos/800/400?random=${i}`,
    author: Mock.mock('@cname') as string,
    source: i % 2 === 0 ? '原创' : '转载',
    viewCount: Mock.mock('@integer(100,10000)') as number,
    publishTime: new Date(Date.now() - i * 86400000 * 2).toISOString(),
    isTop: i < 3 ? 1 : 0, // 前3条置顶
    sort: i + 1,
    status: i < 12 ? 1 : 0, // 前12条启用
    createdAt: new Date(Date.now() - i * 86400000 * 3).toISOString(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  }
})

// ---- Banner ----

export type BannerStatus = 'enabled' | 'disabled'

export interface Banner {
  id: string
  title: string
  titleEn: string
  imageUrl: string
  linkUrl: string
  linkType: string // link, product, category, page
  linkParam: string
  position: string // home, product_list, detail
  startTime: string
  endTime: string
  sort: number
  description: string
  status: Status
  createdAt: string
  updatedAt: string
  createdBy: string
}

export const BANNERS: Banner[] = [
  // 首页 Banner
  {
    id: guid(),
    title: '首页轮播图1',
    titleEn: 'Home Banner 1',
    imageUrl: 'https://picsum.photos/1920/600?random=1',
    linkUrl: '/products',
    linkType: 'page',
    linkParam: '/products',
    position: 'home',
    startTime: new Date(Date.now() - 30 * 86400000).toISOString(),
    endTime: new Date(Date.now() + 30 * 86400000).toISOString(),
    sort: 1,
    description: '首页主Banner，展示促销活动',
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  {
    id: guid(),
    title: '首页轮播图2',
    titleEn: 'Home Banner 2',
    imageUrl: 'https://picsum.photos/1920/600?random=2',
    linkUrl: '/about',
    linkType: 'page',
    linkParam: '/about',
    position: 'home',
    startTime: new Date(Date.now() - 30 * 86400000).toISOString(),
    endTime: new Date(Date.now() + 30 * 86400000).toISOString(),
    sort: 2,
    description: '首页Banner，展示品牌故事',
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  {
    id: guid(),
    title: '首页轮播图3',
    titleEn: 'Home Banner 3',
    imageUrl: 'https://picsum.photos/1920/600?random=3',
    linkUrl: '/news',
    linkType: 'page',
    linkParam: '/news',
    position: 'home',
    startTime: new Date(Date.now() - 30 * 86400000).toISOString(),
    endTime: new Date(Date.now() + 30 * 86400000).toISOString(),
    sort: 3,
    description: '首页Banner，展示最新资讯',
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  // 商品列表页 Banner
  {
    id: guid(),
    title: '商品列表Banner',
    titleEn: 'Product List Banner',
    imageUrl: 'https://picsum.photos/1200/300?random=4',
    linkUrl: '/products?category=hot',
    linkType: 'page',
    linkParam: '/products?category=hot',
    position: 'product_list',
    startTime: new Date(Date.now() - 30 * 86400000).toISOString(),
    endTime: new Date(Date.now() + 30 * 86400000).toISOString(),
    sort: 1,
    description: '商品列表页Banner',
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  // 详情页 Banner
  {
    id: guid(),
    title: '详情页Banner',
    titleEn: 'Detail Banner',
    imageUrl: 'https://picsum.photos/800/200?random=5',
    linkUrl: '/contact',
    linkType: 'page',
    linkParam: '/contact',
    position: 'detail',
    startTime: new Date(Date.now() - 30 * 86400000).toISOString(),
    endTime: new Date(Date.now() + 30 * 86400000).toISOString(),
    sort: 1,
    description: '详情页Banner',
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  // 已过期的 Banner
  {
    id: guid(),
    title: '过期Banner',
    titleEn: 'Expired Banner',
    imageUrl: 'https://picsum.photos/1920/600?random=6',
    linkUrl: '/expired',
    linkType: 'page',
    linkParam: '/expired',
    position: 'home',
    startTime: new Date(Date.now() - 60 * 86400000).toISOString(),
    endTime: new Date(Date.now() - 30 * 86400000).toISOString(),
    sort: 99,
    description: '已过期的Banner',
    status: 1,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
  // 禁用的 Banner
  {
    id: guid(),
    title: '禁用Banner',
    titleEn: 'Disabled Banner',
    imageUrl: 'https://picsum.photos/1920/600?random=7',
    linkUrl: '/disabled',
    linkType: 'page',
    linkParam: '/disabled',
    position: 'home',
    startTime: new Date(Date.now() - 30 * 86400000).toISOString(),
    endTime: new Date(Date.now() + 30 * 86400000).toISOString(),
    sort: 99,
    description: '禁用的Banner',
    status: 0,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  },
]
