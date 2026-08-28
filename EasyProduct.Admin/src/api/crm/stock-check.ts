 import { get, post, put, del } from '@/utils/request'
 import type { PageResult } from '@/types/api'
 import type { StockCheck, StockCheckStatus } from '@/types/crm'

 export interface StockCheckQuery {
   pageIndex: number
   pageSize: number
   warehouseId?: string
   status?: string
   checkNo?: string
 }

 export const getStockCheckList = (params: StockCheckQuery) =>
   get<PageResult<StockCheck>>('/api/admin/crm/stock-check/list', params)

 export const getStockCheckById = (id: string) =>
   get<StockCheck>(`/api/admin/crm/stock-check/${id}`)

 export const createStockCheck = (data: Partial<StockCheck>) =>
   post<{ id: string }>('/api/admin/crm/stock-check', data)

 export const updateStockCheck = (id: string, data: Partial<StockCheck>) =>
   put<null>(`/api/admin/crm/stock-check/${id}`, data)

 export const updateStockCheckStatus = (id: string, data: { status: StockCheckStatus }) =>
   post<null>(`/api/admin/crm/stock-check/${id}/status`, data)

 export const deleteStockCheck = (id: string) =>
   del<null>(`/api/admin/crm/stock-check/${id}`)
