import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Payment, PaymentStatus } from '@/types/crm'

export interface PaymentQuery {
  pageIndex: number
  pageSize: number
  type?: string
  method?: string
  status?: string
  keyword?: string
}

export const getPaymentList = (params: PaymentQuery) =>
  get<PageResult<Payment>>('/api/admin/crm/payment/list', params)

export const getPaymentById = (id: string) =>
  get<Payment>(`/api/admin/crm/payment/${id}`)

export const createPayment = (data: Partial<Payment>) =>
  post<{ id: string }>('/api/admin/crm/payment', data)

export const updatePayment = (id: string, data: Partial<Payment>) =>
  put<null>(`/api/admin/crm/payment/${id}`, data)

export const updatePaymentStatus = (id: string, data: { status: PaymentStatus }) =>
  post<null>(`/api/admin/crm/payment/${id}/status`, data)

export const deletePayment = (id: string) =>
  del<null>(`/api/admin/crm/payment/${id}`)
