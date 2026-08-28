// 报表模块类型定义

// ── 数据源管理 ──
export type DatasourceType = 'mysql' | 'postgresql' | 'sqlserver' | 'oracle'

export type DatasourceStatus = 'connected' | 'disconnected' | 'error'

export interface Datasource {
  id: string
  name: string
  type: DatasourceType
  host: string
  port: number
  database: string
  username: string
  password: string
  status: DatasourceStatus
  remark: string
  createdAt: string
  updatedAt: string
}

export const DATASOURCE_TYPE_OPTIONS = [
  { value: 'mysql', labelKey: 'report.datasource.typeMysql' },
  { value: 'postgresql', labelKey: 'report.datasource.typePostgresql' },
  { value: 'sqlserver', labelKey: 'report.datasource.typeSqlserver' },
  { value: 'oracle', labelKey: 'report.datasource.typeOracle' },
] as const

export const DATASOURCE_STATUS_OPTIONS = [
  { value: 'connected', labelKey: 'report.datasource.statusConnected' },
  { value: 'disconnected', labelKey: 'report.datasource.statusDisconnected' },
  { value: 'error', labelKey: 'report.datasource.statusError' },
] as const

// ── 报表定义 ──
export type ChartType = 'table' | 'line' | 'bar' | 'pie'

export type ReportStatus = 'draft' | 'published' | 'archived'

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
  chartType: ChartType
  columns: ReportColumn[]
  status: ReportStatus
  remark: string
  createdAt: string
  updatedAt: string
}

export const CHART_TYPE_OPTIONS = [
  { value: 'table', labelKey: 'report.definition.chartTable' },
  { value: 'line', labelKey: 'report.definition.chartLine' },
  { value: 'bar', labelKey: 'report.definition.chartBar' },
  { value: 'pie', labelKey: 'report.definition.chartPie' },
] as const

export const REPORT_STATUS_OPTIONS = [
  { value: 'draft', labelKey: 'report.definition.statusDraft' },
  { value: 'published', labelKey: 'report.definition.statusPublished' },
  { value: 'archived', labelKey: 'report.definition.statusArchived' },
] as const

// ── 列模板 ──
export type ColumnType = 'string' | 'number' | 'date' | 'currency'

export interface ColumnTemplate {
  id: string
  name: string
  field: string
  type: ColumnType
  width: number
  format: string
  sortable: boolean
  remark: string
  createdAt: string
  updatedAt: string
}

export const COLUMN_TYPE_OPTIONS = [
  { value: 'string', labelKey: 'report.columnTemplate.typeString' },
  { value: 'number', labelKey: 'report.columnTemplate.typeNumber' },
  { value: 'date', labelKey: 'report.columnTemplate.typeDate' },
  { value: 'currency', labelKey: 'report.columnTemplate.typeCurrency' },
] as const
