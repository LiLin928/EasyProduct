import { get, put } from '@/utils/request'
import type { PageResult } from '@/types/api'
import type { PaymentRecord, PaymentQuery } from '@/types/mall'

/** 支付记录列表（分页 + 筛选） */
export const getPaymentList = (params: PaymentQuery) =>
  get<PageResult<PaymentRecord>>('/api/admin/mall/payment/list', params)

/** 支付记录详情 */
export const getPaymentDetail = (id: string) =>
  get<PaymentRecord>(`/api/admin/mall/payment/${id}`)

/** 退款（将成功支付记录标记为已退款） */
export const refundPayment = (id: string) =>
  put<null>(`/api/admin/mall/payment/${id}/refund`)
