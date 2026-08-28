import { get, put } from '@/utils/request'
import type { SiteAbout, AboutParams } from '@/types/site'

/** 获取关于我们内容（管理端） */
export const getAbout = () =>
  get<SiteAbout>('/api/admin/site/about')

/** 更新关于我们内容 */
export const updateAbout = (data: AboutParams) =>
  put<null>('/api/admin/site/about', data)
