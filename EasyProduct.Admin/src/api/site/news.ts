import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type {
  SiteNews,
  SiteNewsQuery,
  SiteNewsParams,
} from '@/types/site'

/** 获取新闻列表（管理端） */
export const getNewsList = (params: SiteNewsQuery) =>
  get<PageResult<SiteNews>>('/api/admin/site/news/list', params)

/** 获取新闻详情 */
export const getNewsById = (id: string) =>
  get<SiteNews>(`/api/admin/site/news/${id}`)

/** 创建新闻 */
export const createNews = (data: SiteNewsParams) =>
  post<{ id: string }>('/api/admin/site/news', data)

/** 更新新闻 */
export const updateNews = (id: string, data: SiteNewsParams) =>
  put<null>(`/api/admin/site/news/${id}`, data)

/** 删除新闻 */
export const deleteNews = (id: string) =>
  del<null>(`/api/admin/site/news/${id}`)

/** 发布新闻 */
export const publishNews = (id: string) =>
  put<null>(`/api/admin/site/news/${id}/publish`)

/** 撤回新闻 */
export const unpublishNews = (id: string) =>
  put<null>(`/api/admin/site/news/${id}/unpublish`)

/** 置顶/取消置顶 */
export const setTopNews = (id: string, data: { isTop: boolean }) =>
  put<null>(`/api/admin/site/news/${id}/top`, data)
