import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Customer } from '@/types/crm'

export interface CustomerQuery {
  pageIndex: number
  pageSize: number
  code?: string
  name?: string
  type?: string
  status?: string
}

export const getCustomerList = (params: CustomerQuery) =>
  get<PageResult<Customer>>('/api/admin/crm/customer/list', params)

export const getCustomerById = (id: string) =>
  get<Customer>(`/api/admin/crm/customer/${id}`)

export const createCustomer = (data: Partial<Customer>) =>
  post<{ id: string }>('/api/admin/crm/customer', data)

export const updateCustomer = (id: string, data: Partial<Customer>) =>
  put<null>(`/api/admin/crm/customer/${id}`, data)

export const deleteCustomer = (id: string) =>
  del<null>(`/api/admin/crm/customer/${id}`)
