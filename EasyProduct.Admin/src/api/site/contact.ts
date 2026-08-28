import { get, put } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { ContactMessage, ContactQuery, ContactStatus } from '@/types/site'

/** 获取联系留言列表（管理端） */
export const getContactList = (params: ContactQuery) =>
  get<PageResult<ContactMessage>>('/api/admin/site/contact/list', params)

/** 获取联系留言详情 */
export const getContactById = (id: string) =>
  get<ContactMessage>(`/api/admin/site/contact/${id}`)

/** 更新联系留言状态 */
export const updateContactStatus = (id: string, status: ContactStatus) =>
  put<null>(`/api/admin/site/contact/${id}/status`, { status })
