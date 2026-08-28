import { get, put, post } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { SiteInquiry, InquiryQuery, InquiryStatus } from '@/types/site'

/** 获取询价单列表（管理端） */
export const getInquiryList = (params: InquiryQuery) =>
  get<PageResult<SiteInquiry>>('/api/admin/site/inquiry/list', params)

/** 获取询价单详情 */
export const getInquiryById = (id: string) =>
  get<SiteInquiry>(`/api/admin/site/inquiry/${id}`)

/** 更新询价单状态 */
export const updateInquiryStatus = (id: string, status: InquiryStatus) =>
  put<null>(`/api/admin/site/inquiry/${id}/status`, { status })

/** 将询价单转为客户 */
export const convertInquiryToCustomer = (id: string) =>
  post<null>(`/api/admin/site/inquiry/${id}/convert`)
