import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Invoice, InvoiceStatus } from '@/types/crm'

export interface InvoiceQuery {
  pageIndex: number
  pageSize: number
  type?: string
  orderType?: string
  status?: string
  keyword?: string
}

export const getInvoiceList = (params: InvoiceQuery) =>
  get<PageResult<Invoice>>('/api/admin/crm/invoice/list', params)

export const getInvoiceById = (id: string) =>
  get<Invoice>(`/api/admin/crm/invoice/${id}`)

export const createInvoice = (data: Partial<Invoice>) =>
  post<{ id: string }>('/api/admin/crm/invoice', data)

export const updateInvoice = (id: string, data: Partial<Invoice>) =>
  put<null>(`/api/admin/crm/invoice/${id}`, data)

export const updateInvoiceStatus = (id: string, data: { status: InvoiceStatus }) =>
  post<null>(`/api/admin/crm/invoice/${id}/status`, data)

export const deleteInvoice = (id: string) =>
  del<null>(`/api/admin/crm/invoice/${id}`)
