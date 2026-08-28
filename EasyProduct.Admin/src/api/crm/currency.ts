import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { Currency } from '@/types/crm'

export interface CurrencyQuery {
  pageIndex: number
  pageSize: number
  code?: string
  status?: string
}

export const getCurrencyList = (params: CurrencyQuery) =>
  get<PageResult<Currency>>('/api/admin/crm/currency/list', params)

export const getCurrencyById = (id: string) =>
  get<Currency>(`/api/admin/crm/currency/${id}`)

export const createCurrency = (data: Partial<Currency>) =>
  post<{ id: string }>('/api/admin/crm/currency', data)

export const updateCurrency = (id: string, data: Partial<Currency>) =>
  put<null>(`/api/admin/crm/currency/${id}`, data)

export const deleteCurrency = (id: string) =>
  del<null>(`/api/admin/crm/currency/${id}`)
