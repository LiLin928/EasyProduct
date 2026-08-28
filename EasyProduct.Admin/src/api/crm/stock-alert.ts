 import { get, post } from '@/utils/request'
 import type { PageResult } from '@/types/api'
 import type { StockAlert } from '@/types/crm'

 export interface StockAlertQuery {
   pageIndex: number
   pageSize: number
   warehouseId?: string
   alertType?: string
   status?: string
   keyword?: string
 }

 export const getStockAlertList = (params: StockAlertQuery) =>
   get<PageResult<StockAlert>>('/api/admin/crm/stock-alert/list', params)

 export const resolveStockAlert = (id: string) =>
   post<null>(`/api/admin/crm/stock-alert/${id}/resolve`)

 export const batchResolveStockAlerts = (ids: string[]) =>
   post<null>('/api/admin/crm/stock-alert/batch-resolve', { ids })
