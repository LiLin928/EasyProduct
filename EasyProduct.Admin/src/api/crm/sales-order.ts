import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { SalesOrder, SalesOrderStatus } from '@/types/crm'

export interface SalesOrderQuery {
  pageIndex: number
  pageSize: number
  orderNo?: string
  customerId?: string
  status?: SalesOrderStatus
}

export interface SalesOrderStatusUpdateParams {
  status: SalesOrderStatus
  remark?: string
}

/** 销售订单列表（分页 + 筛选） */
export const getSalesOrderList = (params: SalesOrderQuery) =>
  get<PageResult<SalesOrder>>('/api/admin/crm/sales-order/list', params)

/** 销售订单详情（含商品明细行） */
export const getSalesOrderById = (id: string) =>
  get<SalesOrder>(`/api/admin/crm/sales-order/${id}`)

/** 新建销售订单（含明细行） */
export const createSalesOrder = (data: Partial<SalesOrder>) =>
  post<{ id: string }>('/api/admin/crm/sales-order', data)

/** 更新销售订单（草稿状态可改） */
export const updateSalesOrder = (id: string, data: Partial<SalesOrder>) =>
  put<null>(`/api/admin/crm/sales-order/${id}`, data)

/** 删除销售订单（仅草稿可删） */
export const deleteSalesOrder = (id: string) =>
  del<null>(`/api/admin/crm/sales-order/${id}`)

/** 更新订单状态（状态机：草稿→确认→出库→完成/取消） */
export const updateSalesOrderStatus = (id: string, data: SalesOrderStatusUpdateParams) =>
  put<null>(`/api/admin/crm/sales-order/${id}/status`, data)

/** 客户下拉选项 */
export const getSalesOrderCustomerOptions = () =>
  get<Array<{ id: string; name: string }>>('/api/admin/crm/sales-order/customer/options')
