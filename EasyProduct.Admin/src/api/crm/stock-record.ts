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

export interface StockRecordBySkuQuery {
  pageIndex: number
  pageSize: number
  warehouseId?: string
  skuCode: string
}

export const getStockRecordList = (params: StockRecordQuery) =>
  get<PageResult<StockRecord>>('/api/admin/crm/stock-record/list', params)

export const getStockRecordById = (id: string) =>
  get<StockRecord>(`/api/admin/crm/stock-record/${id}`)

/**
 * 获取指定SKU的出入库记录
 */
export const getStockRecordBySku = (params: StockRecordBySkuQuery) =>
  get<PageResult<StockRecord>>('/api/admin/crm/stock-record/by-sku', params)
