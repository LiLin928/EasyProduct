import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { TaxRate } from '@/types/crm'

export interface TaxRateQuery {
  pageIndex: number
  pageSize: number
  code?: string
  status?: string
}

export const getTaxRateList = (params: TaxRateQuery) =>
  get<PageResult<TaxRate>>('/api/admin/crm/tax-rate/list', params)

export const getTaxRateById = (id: string) =>
  get<TaxRate>(`/api/admin/crm/tax-rate/${id}`)

export const createTaxRate = (data: Partial<TaxRate>) =>
  post<{ id: string }>('/api/admin/crm/tax-rate', data)

export const updateTaxRate = (id: string, data: Partial<TaxRate>) =>
  put<null>(`/api/admin/crm/tax-rate/${id}`, data)

export const deleteTaxRate = (id: string) =>
  del<null>(`/api/admin/crm/tax-rate/${id}`)
