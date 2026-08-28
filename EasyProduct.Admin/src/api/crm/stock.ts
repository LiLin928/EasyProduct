 import { get } from '@/utils/request'
 import type { PageResult } from '@/types/api'
 import type { Stock } from '@/types/crm'

 export interface StockQuery {
   pageIndex: number
   pageSize: number
   warehouseId?: string
   keyword?: string
 }

 export const getStockList = (params: StockQuery) =>
   get<PageResult<Stock>>('/api/admin/crm/stock/list', params)

 export const getStockById = (id: string) =>
   get<Stock>(`/api/admin/crm/stock/${id}`)
