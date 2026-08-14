// src/data/admin/dict.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface DictType {
  id: string
  name: string
  code: string
  status: 'enabled' | 'disabled'
  remark: string
}

export interface DictData {
  id: string
  typeCode: string
  value: string
  labelKey: string
  sort: number
  status: 'enabled' | 'disabled'
}

// 字典类型种子数据
export const DICT_TYPES: DictType[] = [
  {
    id: guid(),
    name: '通用状态',
    code: 'common_status',
    status: 'enabled',
    remark: '通用状态字典',
  },
  {
    id: guid(),
    name: '用户性别',
    code: 'user_gender',
    status: 'enabled',
    remark: '用户性别字典',
  },
  {
    id: guid(),
    name: '订单状态',
    code: 'order_status',
    status: 'enabled',
    remark: '订单状态字典',
  },
  {
    id: guid(),
    name: '支付方式',
    code: 'payment_method',
    status: 'enabled',
    remark: '支付方式字典',
  },
  {
    id: guid(),
    name: '物流状态',
    code: 'logistics_status',
    status: 'disabled',
    remark: '物流状态字典',
  },
]

// 字典数据种子数据
export const DICT_DATA: DictData[] = [
  // 通用状态
  {
    id: guid(),
    typeCode: 'common_status',
    value: 'enabled',
    labelKey: 'common.status.enabled',
    sort: 1,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'common_status',
    value: 'disabled',
    labelKey: 'common.status.disabled',
    sort: 2,
    status: 'disabled',
  },
  // 用户性别
  {
    id: guid(),
    typeCode: 'user_gender',
    value: 'male',
    labelKey: 'common.gender.male',
    sort: 1,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'user_gender',
    value: 'female',
    labelKey: 'common.gender.female',
    sort: 2,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'user_gender',
    value: 'unknown',
    labelKey: 'common.gender.unknown',
    sort: 3,
    status: 'disabled',
  },
  // 订单状态
  {
    id: guid(),
    typeCode: 'order_status',
    value: 'pending',
    labelKey: 'order.status.pending',
    sort: 1,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'order_status',
    value: 'paid',
    labelKey: 'order.status.paid',
    sort: 2,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'order_status',
    value: 'shipped',
    labelKey: 'order.status.shipped',
    sort: 3,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'order_status',
    value: 'completed',
    labelKey: 'order.status.completed',
    sort: 4,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'order_status',
    value: 'cancelled',
    labelKey: 'order.status.cancelled',
    sort: 5,
    status: 'disabled',
  },
  // 支付方式
  {
    id: guid(),
    typeCode: 'payment_method',
    value: 'alipay',
    labelKey: 'payment.method.alipay',
    sort: 1,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'payment_method',
    value: 'wechat',
    labelKey: 'payment.method.wechat',
    sort: 2,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'payment_method',
    value: 'bank',
    labelKey: 'payment.method.bank',
    sort: 3,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'payment_method',
    value: 'cash',
    labelKey: 'payment.method.cash',
    sort: 4,
    status: 'disabled',
  },
  // 物流状态
  {
    id: guid(),
    typeCode: 'logistics_status',
    value: 'pending_pickup',
    labelKey: 'logistics.status.pending_pickup',
    sort: 1,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'logistics_status',
    value: 'in_transit',
    labelKey: 'logistics.status.in_transit',
    sort: 2,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'logistics_status',
    value: 'delivered',
    labelKey: 'logistics.status.delivered',
    sort: 3,
    status: 'enabled',
  },
  {
    id: guid(),
    typeCode: 'logistics_status',
    value: 'exception',
    labelKey: 'logistics.status.exception',
    sort: 4,
    status: 'disabled',
  },
]