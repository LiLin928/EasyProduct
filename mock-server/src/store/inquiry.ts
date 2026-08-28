// src/store/inquiry.ts
// [backend: Site 模块 | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'
import { registerReset } from '../helpers/registry.js'

export interface InquiryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
  remark?: string
}

export type InquiryStatus = 'pending' | 'processing' | 'completed' | 'converted'

export interface Inquiry {
  id: string
  companyName: string
  contactName: string
  phone: string
  email: string
  items: InquiryItem[]
  status: InquiryStatus
  remark?: string
  createdAt: string
  updatedAt: string
}

const inquiries: Inquiry[] = Mock.mock({
  'list|5': [{
    id: '@guid',
    companyName: '@ctitle(4,10)',
    contactName: '@cname',
    phone: /^1[3-9]\d{9}$/,
    email: '@email',
    'items|1-3': [{
      productId: '@guid',
      productName: '@ctitle(5,12)',
      'quantity|1-100': 1,
      unit: '@pick(["台", "套", "件", "个"])',
      remark: '@csentence(5,15)',
    }],
    status: '@pick(["pending", "processing", "completed"])',
    remark: '@csentence(5,20)',
    createdAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
    updatedAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((i: Record<string, unknown>) => ({
  ...i,
  id: guid(),
  createdAt: isoTime(),
  updatedAt: isoTime(),
})) as Inquiry[]

function resetInquiryStore(): void {
  inquiries.length = 0
}

registerReset(resetInquiryStore)

export function createInquiry(data: Omit<Inquiry, 'id' | 'status' | 'createdAt' | 'updatedAt'>): Inquiry {
  const inquiry: Inquiry = {
    ...data,
    id: guid(),
    status: 'pending',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  inquiries.push(inquiry)
  return inquiry
}

export function getInquiry(id: string): Inquiry | undefined {
  return inquiries.find((i) => i.id === id)
}

export function listInquiries(): Inquiry[] {
  return [...inquiries]
}

export function updateInquiryStatus(id: string, status: InquiryStatus): boolean {
  const inquiry = inquiries.find((i) => i.id === id)
  if (!inquiry) return false
  inquiry.status = status
  inquiry.updatedAt = isoTime()
  return true
}

export function convertInquiryToCustomer(id: string): boolean {
  const inquiry = inquiries.find((i) => i.id === id)
  if (!inquiry) return false
  if (inquiry.status === 'converted') return false
  inquiry.status = 'converted'
  inquiry.updatedAt = isoTime()
  return true
}
