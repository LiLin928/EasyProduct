import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Supplier } from '@/types/crm'

export interface SupplierQuery {
  pageIndex: number
  pageSize: number
  code?: string
  name?: string
  status?: string
}

export const getSupplierList = (params: SupplierQuery) =>
  get<PageResult<Supplier>>('/api/admin/crm/supplier/list', params)

export const getSupplierById = (id: string) =>
  get<Supplier>(`/api/admin/crm/supplier/${id}`)

export const createSupplier = (data: Partial<Supplier>) =>
  post<{ id: string }>('/api/admin/crm/supplier', data)

export const updateSupplier = (id: string, data: Partial<Supplier>) =>
  put<null>(`/api/admin/crm/supplier/${id}`, data)

export const deleteSupplier = (id: string) =>
  del<null>(`/api/admin/crm/supplier/${id}`)
