 import { get } from '@/utils/request'
 import type { PageResult } from '@/types/api'
 import type { StockRecord } from '@/types/crm'

 export interface StockRecordQuery {
   pageIndex: number
   pageSize: number
   warehouseId?: string
   sourceType?: string
   startDate?: string
   endDate?: string
   keyword?: string
 }

 export const getStockRecordList = (params: StockRecordQuery) =>
   get<PageResult<StockRecord>>('/api/admin/crm/stock-record/list', params)

 export const getStockRecordById = (id: string) =>
   get<StockRecord>(`/api/admin/crm/stock-record/${id}`)
