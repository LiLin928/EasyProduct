// src/data/rpt-column-template.ts
// 报表列模板 seed

import { guid, isoTime } from '../helpers/id.js'

export interface ColumnTemplate {
  id: string
  name: string
  field: string
  type: 'string' | 'number' | 'date' | 'currency'
  width: number
  format: string
  sortable: boolean
  remark: string
  createdAt: string
  updatedAt: string
}

export const COLUMN_TEMPLATES: ColumnTemplate[] = [
  {
    id: guid(),
    name: '商品名称',
    field: 'productName',
    type: 'string',
    width: 200,
    format: '',
    sortable: false,
    remark: '通用商品名称列',
    createdAt: isoTime(-20),
    updatedAt: isoTime(-5),
  },
  {
    id: guid(),
    name: '订单号',
    field: 'orderNo',
    type: 'string',
    width: 150,
    format: '',
    sortable: true,
    remark: '订单编号列',
    createdAt: isoTime(-18),
    updatedAt: isoTime(-6),
  },
  {
    id: guid(),
    name: '金额',
    field: 'amount',
    type: 'currency',
    width: 150,
    format: '¥{0}',
    sortable: true,
    remark: '人民币金额列，带货币符号',
    createdAt: isoTime(-15),
    updatedAt: isoTime(-3),
  },
  {
    id: guid(),
    name: '数量',
    field: 'quantity',
    type: 'number',
    width: 100,
    format: '{0}',
    sortable: true,
    remark: '通用数量列',
    createdAt: isoTime(-12),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    name: '日期',
    field: 'createdAt',
    type: 'date',
    width: 160,
    format: 'YYYY-MM-DD HH:mm',
    sortable: true,
    remark: '标准日期时间列',
    createdAt: isoTime(-10),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    name: '客户名称',
    field: 'customerName',
    type: 'string',
    width: 180,
    format: '',
    sortable: false,
    remark: '客户/往来方名称列',
    createdAt: isoTime(-8),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    name: '百分比',
    field: 'percentage',
    type: 'number',
    width: 100,
    format: '{0}%',
    sortable: true,
    remark: '百分比格式列',
    createdAt: isoTime(-5),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    name: '状态',
    field: 'status',
    type: 'string',
    width: 80,
    format: '',
    sortable: false,
    remark: '通用状态列',
    createdAt: isoTime(-3),
    updatedAt: isoTime(-1),
  },
]
