/** 基础字段配置 */
export interface BaseSearchField {
  prop: string // 字段名
  label: string // 标签（i18n key）
  type: 'input' | 'select' | 'dateRange'
  placeholder?: string // 占位符（可选，默认使用通用占位符）
  clearable?: boolean // 是否可清空（默认 true）
}

/** Input 类型字段 */
export interface InputSearchField extends BaseSearchField {
  type: 'input'
}

/** Select 类型字段 */
export interface SelectSearchField extends BaseSearchField {
  type: 'select'
  options: Array<{ label: string; value: string }>
}

/** DateRange 类型字段 */
export interface DateRangeSearchField extends BaseSearchField {
  type: 'dateRange'
  startPlaceholder?: string
  endPlaceholder?: string
}

/** 联合类型 */
export type SearchField = InputSearchField | SelectSearchField | DateRangeSearchField