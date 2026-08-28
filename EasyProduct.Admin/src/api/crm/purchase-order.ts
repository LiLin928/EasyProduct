import { get, post, put, del } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { PurchaseOrder, PurchaseOrderStatus } from '@/types/crm'

export interface PurchaseOrderQuery {
  pageIndex: number
  pageSize: number
  orderNo?: string
  supplierId?: string
  status?: PurchaseOrderStatus
}

export interface PurchaseOrderStatusUpdateParams {
  status: PurchaseOrderStatus
  remark?: string
}

/** 采购订单列表（分页 + 筛选） */
export const getPurchaseOrderList = (params: PurchaseOrderQuery) =>
  get<PageResult<PurchaseOrder>>('/api/admin/crm/purchase-order/list', params)

/** 采购订单详情（含商品明细行） */
export const getPurchaseOrderById = (id: string) =>
  get<PurchaseOrder>(`/api/admin/crm/purchase-order/${id}`)

/** 新建采购订单（含明细行） */
export const createPurchaseOrder = (data: Partial<PurchaseOrder>) =>
  post<{ id: string }>('/api/admin/crm/purchase-order', data)

/** 更新采购订单（草稿状态可改） */
export const updatePurchaseOrder = (id: string, data: Partial<PurchaseOrder>) =>
  put<null>(`/api/admin/crm/purchase-order/${id}`, data)

/** 删除采购订单（仅草稿可删） */
export const deletePurchaseOrder = (id: string) =>
  del<null>(`/api/admin/crm/purchase-order/${id}`)

/** 更新订单状态（状态机：草稿→确认→到货→完成/取消） */
export const updatePurchaseOrderStatus = (id: string, data: PurchaseOrderStatusUpdateParams) =>
  put<null>(`/api/admin/crm/purchase-order/${id}/status`, data)

/** 供应商下拉选项 */
export const getPurchaseOrderSupplierOptions = () =>
  get<Array<{ id: string; name: string }>>('/api/admin/crm/purchase-order/supplier/options')
