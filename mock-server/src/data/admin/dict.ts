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
    id: 'dict-type-001',
    name: '通用状态',
    code: 'common_status',
    status: 'enabled',
    remark: '通用状态字典',
  },
  {
    id: 'dict-type-002',
    name: '用户性别',
    code: 'user_gender',
    status: 'enabled',
    remark: '用户性别字典',
  },
  {
    id: 'dict-type-003',
    name: '订单状态',
    code: 'order_status',
    status: 'enabled',
    remark: '订单状态字典',
  },
  {
    id: 'dict-type-004',
    name: '支付方式',
    code: 'payment_method',
    status: 'enabled',
    remark: '支付方式字典',
  },
  {
    id: 'dict-type-005',
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
    id: 'dict-data-001',
    typeCode: 'common_status',
    value: 'enabled',
    labelKey: 'common.status.enabled',
    sort: 1,
    status: 'enabled',
  },
  {
    id: 'dict-data-002',
    typeCode: 'common_status',
    value: 'disabled',
    labelKey: 'common.status.disabled',
    sort: 2,
    status: 'enabled',
  },
  // 用户性别
  {
    id: 'dict-data-003',
    typeCode: 'user_gender',
    value: 'male',
    labelKey: 'common.gender.male',
    sort: 1,
    status: 'enabled',
  },
  {
    id: 'dict-data-004',
    typeCode: 'user_gender',
    value: 'female',
    labelKey: 'common.gender.female',
    sort: 2,
    status: 'enabled',
  },
  // 订单状态
  {
    id: 'dict-data-005',
    typeCode: 'order_status',
    value: 'pending',
    labelKey: 'order.status.pending',
    sort: 1,
    status: 'enabled',
  },
  {
    id: 'dict-data-006',
    typeCode: 'order_status',
    value: 'paid',
    labelKey: 'order.status.paid',
    sort: 2,
    status: 'enabled',
  },
  {
    id: 'dict-data-007',
    typeCode: 'order_status',
    value: 'shipped',
    labelKey: 'order.status.shipped',
    sort: 3,
    status: 'enabled',
  },
  {
    id: 'dict-data-008',
    typeCode: 'order_status',
    value: 'completed',
    labelKey: 'order.status.completed',
    sort: 4,
    status: 'enabled',
  },
]