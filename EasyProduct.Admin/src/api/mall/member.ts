import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Member, MemberQuery, MemberParams } from '@/types/mall'

/** 会员列表（分页 + 筛选） */
export const getMemberList = (params: MemberQuery) =>
  get<PageResult<Member>>('/api/admin/mall/member/list', params)

/** 会员详情 */
export const getMemberDetail = (id: string) =>
  get<Member>(`/api/admin/mall/member/${id}`)

/** 新建会员 */
export const createMember = (data: MemberParams) =>
  post<{ id: string }>('/api/admin/mall/member', data)

/** 更新会员 */
export const updateMember = (id: string, data: MemberParams) =>
  put<null>(`/api/admin/mall/member/${id}`, data)

/** 删除会员 */
export const deleteMember = (id: string) =>
  del<null>(`/api/admin/mall/member/${id}`)

/** 获取会员等级下拉选项 */
export const getMemberLevelOptions = () =>
  get<Array<{ id: string; name: string }>>('/api/admin/mall/member/level/options')
