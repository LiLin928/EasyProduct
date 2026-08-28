// src/data/rpt-definition.ts
// 报表定义 seed

import { guid, isoTime } from '../helpers/id.js'

export interface ReportColumn {
  field: string
  label: string
  type: 'string' | 'number' | 'date' | 'currency'
  width: number
  format: string
  sortable: boolean
}

export interface ReportDefinition {
  id: string
  name: string
  code: string
  datasourceId: string
  datasourceName: string
  sqlTemplate: string
  chartType: 'table' | 'line' | 'bar' | 'pie'
  columns: ReportColumn[]
  status: 'draft' | 'published' | 'archived'
  remark: string
  createdAt: string
  updatedAt: string
}

export const REPORT_DEFINITIONS: ReportDefinition[] = [
  {
    id: guid(),
    name: '月度销售汇总',
    code: 'RPT_SALES_MONTHLY',
    datasourceId: 'ds-001',
    datasourceName: '主业务库',
    sqlTemplate: 'SELECT DATE_FORMAT(o.created_at, "%Y-%m") AS month, SUM(o.total_amount) AS total FROM mall_order o WHERE o.status = "paid" GROUP BY month ORDER BY month DESC LIMIT 12',
    chartType: 'line',
    columns: [
      { field: 'month', label: '月份', type: 'string', width: 120, format: '', sortable: true },
      { field: 'total', label: '销售额', type: 'currency', width: 150, format: '¥{0}', sortable: true },
    ],
    status: 'published',
    remark: '按月统计已支付订单销售总额',
    createdAt: isoTime(-20),
    updatedAt: isoTime(-3),
  },
  {
    id: guid(),
    name: '商品销量排行',
    code: 'RPT_PRODUCT_RANKING',
    datasourceId: 'ds-001',
    datasourceName: '主业务库',
    sqlTemplate: 'SELECT p.name AS product, SUM(oi.quantity) AS qty, SUM(oi.amount) AS revenue FROM mall_order_item oi JOIN product_spu p ON oi.spu_id = p.id GROUP BY p.id ORDER BY qty DESC LIMIT 20',
    chartType: 'bar',
    columns: [
      { field: 'product', label: '商品名称', type: 'string', width: 200, format: '', sortable: false },
      { field: 'qty', label: '销量', type: 'number', width: 100, format: '{0}件', sortable: true },
      { field: 'revenue', label: '营收', type: 'currency', width: 150, format: '¥{0}', sortable: true },
    ],
    status: 'published',
    remark: '商品销量与营收排行 TOP 20',
    createdAt: isoTime(-18),
    updatedAt: isoTime(-5),
  },
  {
    id: guid(),
    name: '客户分布分析',
    code: 'RPT_CUSTOMER_DIST',
    datasourceId: 'ds-001',
    datasourceName: '主业务库',
    sqlTemplate: 'SELECT type, COUNT(*) AS count FROM crm_customer WHERE status = "active" GROUP BY type',
    chartType: 'pie',
    columns: [
      { field: 'type', label: '客户类型', type: 'string', width: 120, format: '', sortable: false },
      { field: 'count', label: '数量', type: 'number', width: 100, format: '{0}人', sortable: true },
    ],
    status: 'published',
    remark: 'B2B与零售客户占比分析',
    createdAt: isoTime(-15),
    updatedAt: isoTime(-7),
  },
  {
    id: guid(),
    name: '库存周转率',
    code: 'RPT_STOCK_TURNOVER',
    datasourceId: 'ds-001',
    datasourceName: '主业务库',
    sqlTemplate: 'SELECT w.name AS warehouse, s.sku_name, s.available, s.locked, (SELECT COALESCE(SUM(quantity), 0) FROM crm_stock_record WHERE sku_id = s.sku_id AND warehouse_id = s.warehouse_id AND type = "out" AND created_at > DATE_SUB(NOW(), INTERVAL 30 DAY)) AS out_qty FROM crm_stock s JOIN crm_warehouse w ON s.warehouse_id = w.id ORDER BY out_qty DESC',
    chartType: 'table',
    columns: [
      { field: 'warehouse', label: '仓库', type: 'string', width: 150, format: '', sortable: false },
      { field: 'sku_name', label: '商品', type: 'string', width: 200, format: '', sortable: false },
      { field: 'available', label: '可用库存', type: 'number', width: 100, format: '{0}', sortable: true },
      { field: 'locked', label: '锁定库存', type: 'number', width: 100, format: '{0}', sortable: true },
      { field: 'out_qty', label: '30天出库', type: 'number', width: 120, format: '{0}', sortable: true },
    ],
    status: 'draft',
    remark: '库存周转分析，草稿状态待完善',
    createdAt: isoTime(-10),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    name: '应收账款账龄',
    code: 'RPT_ARAP_AGING',
    datasourceId: 'ds-001',
    datasourceName: '主业务库',
    sqlTemplate: 'SELECT order_type, order_no, party_name, receivable, received, balance, aging FROM crm_arap WHERE status = "unsettled" ORDER BY aging',
    chartType: 'table',
    columns: [
      { field: 'order_type', label: '订单类型', type: 'string', width: 100, format: '', sortable: false },
      { field: 'order_no', label: '订单号', type: 'string', width: 150, format: '', sortable: false },
      { field: 'party_name', label: '往来方', type: 'string', width: 180, format: '', sortable: false },
      { field: 'receivable', label: '应收', type: 'currency', width: 120, format: '¥{0}', sortable: true },
      { field: 'received', label: '已收', type: 'currency', width: 120, format: '¥{0}', sortable: true },
      { field: 'balance', label: '余额', type: 'currency', width: 120, format: '¥{0}', sortable: true },
      { field: 'aging', label: '账龄', type: 'string', width: 80, format: '', sortable: true },
    ],
    status: 'published',
    remark: '未结应收应付账龄分析',
    createdAt: isoTime(-8),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    name: '支付方式统计',
    code: 'RPT_PAYMENT_METHOD',
    datasourceId: 'ds-001',
    datasourceName: '主业务库',
    sqlTemplate: 'SELECT method, COUNT(*) AS count, SUM(amount) AS total FROM mall_payment WHERE status = "paid" GROUP BY method',
    chartType: 'pie',
    columns: [
      { field: 'method', label: '支付方式', type: 'string', width: 120, format: '', sortable: false },
      { field: 'count', label: '笔数', type: 'number', width: 100, format: '{0}笔', sortable: true },
      { field: 'total', label: '金额', type: 'currency', width: 150, format: '¥{0}', sortable: true },
    ],
    status: 'archived',
    remark: '已归档，被新版支付统计替代',
    createdAt: isoTime(-30),
    updatedAt: isoTime(-12),
  },
]
