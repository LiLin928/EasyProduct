// src/composables/useSearch.ts
import { reactive } from 'vue'
import type { UnwrapNestedRefs } from 'vue'

export interface UseSearchOptions<T = Record<string, unknown>> {
  defaultModel?: T
  onSearch?: (model: T) => void
  onReset?: () => void
}

export interface UseSearchReturn<T = Record<string, unknown>> {
  searchModel: UnwrapNestedRefs<T>
  handleSearch: () => void
  handleReset: () => void
  getSearchParams: () => Partial<T>
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
export function useSearch<T extends Record<string, unknown> = Record<string, unknown>>(
  options: UseSearchOptions<T> = {}
): UseSearchReturn<T> {
  const { defaultModel = {} as T, onSearch, onReset } = options

  // 响应式搜索模型
  const searchModel = reactive<T>({ ...defaultModel } as T)

  /**
   * 获取搜索参数（过滤空值）
   */
  const getSearchParams = (): Partial<T> => {
    const params: Partial<T> = {}
    const rawParams = params as Record<string, unknown>

    Object.entries(searchModel as Record<string, unknown>).forEach(([key, value]) => {
      // 过滤空值
      if (value !== '' && value !== null && value !== undefined) {
        // 日期范围处理：仅对特定字段名拆解为 startTime/endTime
        if (Array.isArray(value) && value.length === 2 && DATE_RANGE_FIELDS.has(key)) {
          rawParams['startTime'] = value[0]
          rawParams['endTime'] = value[1]
        } else {
          rawParams[key] = value
        }
      }
    })

    return params
  }

  /**
   * 搜索方法
   */
  const handleSearch = (): void => {
    onSearch?.(getSearchParams() as T)
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