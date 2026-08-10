// src/api/site/news.ts
import { get } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { NewsItem, NewsQuery } from '@/types/site'

/** 新闻列表 */
export const getNewsList = (params: NewsQuery) =>
  get<PageResult<NewsItem>>('/api/site/news/list', params)

/** 新闻详情 */
export const getNewsDetail = (id: string) =>
  get<NewsItem>(`/api/site/news/${id}`)