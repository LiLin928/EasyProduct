// src/composables/useSearch.ts
import { reactive } from 'vue'
import type { UnwrapNestedRefs } from 'vue'

export interface UseSearchOptions {
  defaultModel?: Record<string, unknown>
  onSearch?: (model: Record<string, unknown>) => void
  onReset?: () => void
}

export interface UseSearchReturn {
  searchModel: UnwrapNestedRefs<Record<string, unknown>>
  handleSearch: () => void
  handleReset: () => void
  getSearchParams: () => Record<string, unknown>
}

/**
 * 日期范围字段名称集合
 * 只有这些字段才会被拆解为 startTime/endTime
 */
const DATE_RANGE_FIELDS = new Set([
  'dateRange',
  'timeRange',
  'createTime',
  'updateTime',
  'orderTime',
  'payTime',
  'shipTime',
  'completeTime'
])

/**
 * 搜索表单 composable
 * @param options 配置项
 */
export function useSearch(options: UseSearchOptions = {}): UseSearchReturn {
  const { defaultModel = {}, onSearch, onReset } = options

  // 响应式搜索模型
  const searchModel = reactive<Record<string, unknown>>({ ...defaultModel })

  /**
   * 获取搜索参数（过滤空值）
   */
  const getSearchParams = (): Record<string, unknown> => {
    const params: Record<string, unknown> = {}

    Object.entries(searchModel).forEach(([key, value]) => {
      // 过滤空值
      if (value !== '' && value !== null && value !== undefined) {
        // 日期范围处理：仅对特定字段名拆解为 startTime/endTime
        if (Array.isArray(value) && value.length === 2 && DATE_RANGE_FIELDS.has(key)) {
          params['startTime'] = value[0]
          params['endTime'] = value[1]
        } else {
          params[key] = value
        }
      }
    })

    return params
  }

  /**
   * 搜索方法
   */
  const handleSearch = (): void => {
    onSearch?.(getSearchParams())
  }

  /**
   * 重置方法
   */
  const handleReset = (): void => {
    Object.assign(searchModel, defaultModel)
    onReset?.()
  }

  return {
    searchModel,
    handleSearch,
    handleReset,
    getSearchParams
  }
}