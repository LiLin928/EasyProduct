// src/composables/useTable.ts
import { ref, reactive } from 'vue'
import type { Ref } from 'vue'
import type { PageQuery, PageResult } from '@/types/api'

export interface UseTableOptions {
  defaultPageSize?: number
  immediate?: boolean
}

export interface UseTableReturn<T> {
  loading: Ref<boolean>
  list: Ref<T[]>
  total: Ref<number>
  query: {
    pageIndex: number
    pageSize: number
    [key: string]: unknown
  }
  handleSearch: () => Promise<void>
  handleReset: () => void
  handlePageChange: (page: number) => Promise<void>
  reload: () => Promise<void>
}

/**
 * 列表页通用 composable（签名对齐前端规范 2.4）
 * @param fetchFn 分页查询函数，接收 PageQuery 返回 Promise<PageResult<T>>
 * @param options 配置项
 */
export function useTable<T>(
  fetchFn: (params: PageQuery & Record<string, unknown>) => Promise<PageResult<T>>,
  options: UseTableOptions = {},
): UseTableReturn<T> {
  const { defaultPageSize = 10, immediate = true } = options

  const loading = ref(false)
  const list = ref<T[]>([]) as Ref<T[]>
  const total = ref(0)
  const query = reactive<PageQuery & Record<string, unknown>>({
    pageIndex: 1,
    pageSize: defaultPageSize,
  })

  const fetchData = async (): Promise<void> => {
    loading.value = true
    try {
      const result = await fetchFn(query)
      list.value = result.list
      total.value = result.total
    } finally {
      loading.value = false
    }
  }

  const handleSearch = async (): Promise<void> => {
    query.pageIndex = 1
    await fetchData()
  }

  const handleReset = (): void => {
    query.pageIndex = 1
    query.pageSize = defaultPageSize
    Object.keys(query).forEach((key) => {
      if (key !== 'pageIndex' && key !== 'pageSize') {
        delete query[key]
      }
    })
  }

  const handlePageChange = async (page: number): Promise<void> => {
    query.pageIndex = page
    await fetchData()
  }

  const reload = async (): Promise<void> => {
    await fetchData()
  }

  if (immediate) {
    fetchData()
  }

  return {
    loading,
    list,
    total,
    query,
    handleSearch,
    handleReset,
    handlePageChange,
    reload,
  }
}