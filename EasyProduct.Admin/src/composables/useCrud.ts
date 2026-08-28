// src/composables/useCrud.ts
import { ref } from 'vue'
import type { Ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { useDialog } from '@/composables/useDialog'
import type { PageQuery, PageResult } from '@/types/api'

export interface UseCrudOptions {
  defaultSearchModel?: Record<string, unknown>
  defaultPageSize?: number
  immediate?: boolean
  deleteFn: (id: string) => Promise<unknown>
  deleteConfirmText?: string
  deleteSuccessText?: string
  batchDeleteFn?: (ids: string[]) => Promise<unknown>
  batchDeleteConfirmText?: string
}

export interface UseCrudReturn<T> {
  loading: Ref<boolean>
  list: Ref<T[]>
  total: Ref<number>
  query: PageQuery & Record<string, unknown>
  searchModel: Record<string, unknown>
  selection: Ref<T[]>

  handleSearch: () => void
  handleReset: () => void
  handlePageChange: (page: number) => Promise<void>
  handleSelectionChange: (rows: unknown[]) => void
  reload: () => Promise<void>

  formDialog: ReturnType<typeof useDialog<T>>
  openCreate: () => void
  openEdit: (row: T) => void

  handleDelete: (row: T) => Promise<void>
  handleBatchDelete: () => Promise<void>
}

/**
 * CRUD 通用 composable: useTable + useSearch + useDialog + delete 逻辑一体化
 * 适用于标准列表页（搜索 + 表格 + 新增/编辑弹窗 + 删除/批量删除）
 */
export function useCrud<T extends { id: string }>(
  fetchFn: (params: PageQuery & Record<string, unknown>) => Promise<PageResult<T>>,
  options: UseCrudOptions,
): UseCrudReturn<T> {
  const { t } = useI18n()

  const {
    defaultSearchModel = {},
    defaultPageSize = 10,
    immediate = true,
    deleteFn,
    deleteConfirmText,
    deleteSuccessText,
    batchDeleteFn,
    batchDeleteConfirmText,
  } = options

  // 列表
  const table = useTable<T>(fetchFn, { defaultPageSize, immediate })

  // 搜索
  const { searchModel, resetModel, getSearchParams } = useSearch({
    defaultModel: defaultSearchModel,
  })

  // 弹窗
  const formDialog = useDialog<T>()

  // 多选
  const selection = ref<T[]>([]) as Ref<T[]>

  const handleSearch = (): void => {
    Object.assign(table.query, getSearchParams())
    void table.handleSearch()
  }

  const handleReset = (): void => {
    resetModel()
    const keys = Object.keys(defaultSearchModel)
    keys.forEach((key) => {
      delete table.query[key]
    })
    void table.handleReset()
  }

  const handleSelectionChange = (rows: unknown[]): void => {
    selection.value = rows as T[]
  }

  const openCreate = (): void => {
    formDialog.open()
  }

  const openEdit = (row: T): void => {
    formDialog.open(row)
  }

  const handleDelete = async (row: T): Promise<void> => {
    try {
      await ElMessageBox.confirm(
        deleteConfirmText ?? t('common.deleteConfirm'),
        t('common.tips'),
        { type: 'warning' },
      )
      await deleteFn(row.id)
      ElMessage.success(deleteSuccessText ?? t('common.deleteSuccess'))
      await table.reload()
    } catch {
      // user cancelled or request failed
    }
  }

  const handleBatchDelete = async (): Promise<void> => {
    if (!batchDeleteFn) return
    try {
      await ElMessageBox.confirm(
        batchDeleteConfirmText ?? t('common.batchDeleteConfirm', { count: selection.value.length }),
        t('common.tips'),
        { type: 'warning' },
      )
      const ids = selection.value.map((item) => item.id)
      await batchDeleteFn(ids)
      ElMessage.success(deleteSuccessText ?? t('common.deleteSuccess'))
      selection.value = []
      await table.reload()
    } catch {
      // user cancelled or request failed
    }
  }

  return {
    loading: table.loading,
    list: table.list,
    total: table.total,
    query: table.query,
    searchModel,
    selection,
    handleSearch,
    handleReset,
    handlePageChange: table.handlePageChange,
    handleSelectionChange,
    reload: table.reload,
    formDialog,
    openCreate,
    openEdit,
    handleDelete,
    handleBatchDelete,
  }
}
