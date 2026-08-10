// src/api/site/inquiry.ts
import { post, get } from '@/utils/request'

interface InquiryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
  remark?: string
}

interface InquirySubmit {
  companyName: string
  contactName: string
  phone: string
  email: string
  items: InquiryItem[]
}

interface InquiryResult {
  id: string
  companyName: string
  contactName: string
  phone: string
  email: string
  items: InquiryItem[]
  status: 'pending' | 'processing' | 'completed'
  createdAt: string
}

/** 提交询价 */
export const submitInquiry = (data: InquirySubmit) =>
  post<InquiryResult>('/api/site/inquiry', data)

/** 查询询价单 */
export const getInquiry = (id: string) =>
  get<InquiryResult>(`/api/site/inquiry/${id}`)