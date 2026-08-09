import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Banner, NewsItem, NewsQuery } from '@/types/site'

/** 首页 Banner 列表 */
export const getBannerList = () => get<Banner[]>('/api/site/banner/list')

/** 新闻分页列表 */
export const getNewsList = (params: NewsQuery) =>
  get<PageResult<NewsItem>>('/api/site/news/list', params)
