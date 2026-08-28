import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { MemberLevel, LevelQuery, LevelParams } from '@/types/mall'

/** 会员等级列表（分页 + 筛选） */
export const getLevelList = (params: LevelQuery) =>
  get<PageResult<MemberLevel>>('/api/admin/mall/level/list', params)

/** 会员等级详情 */
export const getLevelDetail = (id: string) =>
  get<MemberLevel>(`/api/admin/mall/level/${id}`)

/** 新建会员等级 */
export const createLevel = (data: LevelParams) =>
  post<{ id: string }>('/api/admin/mall/level', data)

/** 更新会员等级 */
export const updateLevel = (id: string, data: LevelParams) =>
  put<null>(`/api/admin/mall/level/${id}`, data)

/** 删除会员等级 */
export const deleteLevel = (id: string) =>
  del<null>(`/api/admin/mall/level/${id}`)
