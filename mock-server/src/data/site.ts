// src/data/site.ts
// [backend: Site 模块 | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

export type BannerStatus = 'enabled' | 'disabled'

export interface SiteBanner {
  id: string
  title: string
  titleEn: string
  imageUrl: string
  link: string
  sort: number
  status: BannerStatus
  createdAt: string
  updatedAt: string
}

export const BANNERS: SiteBanner[] = Mock.mock({
  'list|3': [{
    id: '@guid',
    title: '@ctitle(8,16)',
    titleEn: '@title(3,5)',
    imageUrl: '@image(1920x600)',
    link: '',
  }],
}).list.map((b: Record<string, unknown>, i: number) => ({
  ...b,
  id: guid(),
  sort: i + 1,
  status: 'enabled' as BannerStatus,
  createdAt: isoTime(),
  updatedAt: isoTime(),
})) as SiteBanner[]

export const NEWS = Mock.mock({
  'list|28': [{
    id: '@guid',
    categoryId: '@guid',
    title: '@ctitle(10,24)',
    titleEn: '@title(5,10)',
    summary: '@cparagraph(1,2)',
    coverImage: '@image(640x360)',
    isTop: '@boolean',
    'viewCount|100-9999': 1,
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((n: Record<string, unknown>) => ({ ...n, id: guid(), categoryId: String(n.categoryId).toLowerCase(), publishTime: isoTime() }))
