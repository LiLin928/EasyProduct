// src/store/inquiry.ts
// [backend: Site 模块 | status: pending]
import { guid, isoTime } from '../helpers/id.js'

interface InquiryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
  remark?: string
}

interface Inquiry {
  id: string
  companyName: string
  contactName: string
  phone: string
  email: string
  items: InquiryItem[]
  status: 'pending' | 'processing' | 'completed'
  createdAt: string
}

const inquiries: Inquiry[] = []

export function resetInquiryStore(): void {
  inquiries.length = 0
}

export function createInquiry(data: Omit<Inquiry, 'id' | 'status' | 'createdAt'>): Inquiry {
  const inquiry: Inquiry = {
    ...data,
    id: guid(),
    status: 'pending',
    createdAt: isoTime(),
  }
  inquiries.push(inquiry)
  return inquiry
}

export function getInquiry(id: string): Inquiry | undefined {
  return inquiries.find(i => i.id === id)
}

export function listInquiries(): Inquiry[] {
  return [...inquiries]
}