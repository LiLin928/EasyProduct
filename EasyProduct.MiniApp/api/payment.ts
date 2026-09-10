// api/payment.ts —— 支付相关 API
import { request } from '../utils/request'

const BASE_URL = '/payment'

interface Payment {
  id: string
  orderId: string
  amount: number
  method: number
  status: number
  createTime: string
  payTime?: string
}

interface CreatePaymentParams {
  orderId: string
  method: number
}

interface WxPayParams {
  timeStamp: string
  nonceStr: string
  package: string
  signType: string
  paySign: string
}

export function createPayment(params: CreatePaymentParams): Promise<WxPayParams> {
  return request<WxPayParams>({
    url: BASE_URL,
    method: 'POST',
    data: params,
  })
}

export function getPaymentDetail(id: string): Promise<Payment> {
  return request<Payment>({
    url: `${BASE_URL}/${id}`,
    method: 'GET',
  })
}

export function queryPaymentStatus(id: string): Promise<{ status: string }> {
  return request<{ status: string }>({
    url: `${BASE_URL}/${id}/status`,
    method: 'GET',
  })
}