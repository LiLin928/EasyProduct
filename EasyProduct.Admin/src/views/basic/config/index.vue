<!-- src/views/basic/config/index.vue -->
<template>
  <div class="config-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      :loading="table.loading.value"
      @search="onSearch"
      @reset="onReset"
    >
      <template #toolbar>
        <el-button
          v-permission="['basic:config:add']"
          type="primary"
          @click="handleAdd"
        >
          {{ t('basic.config.add') }}
        </el-button>
        <el-button
          v-permission="['basic:config:delete']"
          type="danger"
          :disabled="!selectedRows.length"
          @click="handleBatchDelete"
        >
          {{ t('common.batchDelete') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <BaseTable
      :loading="table.loading.value"
      :data="table.list.value"
      :total="table.total.value"
      :current-page="table.query.pageIndex"
      :page-size="table.query.pageSize"
      @page-change="table.handlePageChange"
      @size-change="handleSizeChange"
      @selection-change="handleSelectionChange"
    >
      <el-table-column
        type="selection"
        width="50"
      />
      <el-table-column
        :label="t('basic.config.key')"
        prop="key"
        min-width="140"
      />
      <el-table-column
        :label="t('basic.config.label')"
        prop="label"
        min-width="140"
      />
      <el-table-column
        :label="t('basic.config.value')"
        min-width="160"
      >
        <template #default="{ row }">
          <!-- 内联编辑模式 -->
          <template v-if="editingId === row.id">
            <el-input
              v-if="row.type === 'string'"
              v-model="editValue"
              size="small"
            />
            <el-input-number
              v-else-if="row.type === 'number'"
              v-model="numberEditValue"
              size="small"
              controls-position="right"
            />
            <el-switch
              v-else
              v-model="booleanEditValue"
            />
          </template>
          <!-- 只读模式 -->
          <span v-else>{{ displayValue(row) }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('basic.config.type')"
        prop="type"
        width="100"
        align="center"
      >
        <template #default="{ row }">
          <el-tag
            size="small"
            type="info"
          >
            {{ typeLabel(row.type) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column
        :label="t('basic.config.remark')"
        prop="remark"
        min-width="160"
      />
      <el-table-column
        :label="t('common.actions')"
        width="160"
        align="center"
        fixed="right"
      >
        <template #default="{ row }">
          <template v-if="editingId === row.id">
            <el-button
              link
              type="primary"
              size="small"
              @click="handleSaveInline(row)"
            >
              {{ t('basic.config.save') }}
            </el-button>
            <el-button
              link
              size="small"
              @click="handleCancelInline"
            >
              {{ t('basic.config.cancel') }}
            </el-button>
          </template>
          <template v-else>
            <el-button
              v-permission="['basic:config:edit']"
              link
              type="primary"
              size="small"
              @click="handleEditInline(row)"
            >
              {{ t('basic.config.inlineEdit') }}
            </el-button>
            <el-button
              v-permission="['basic:config:delete']"
              link
              type="danger"
              size="small"
              @click="handleDelete(row)"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </template>
      </el-table-column>
    </BaseTable>

    <ConfigFormDialog
      v-model="dialogVisible"
      @success="table.reload"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import { useLocale } from '@/composables/useLocale'
import { useSearch } from '@/composables/useSearch'
import { useTable } from '@/composables/useTable'
import { getConfigList, updateConfig, deleteConfig, deleteConfigBatch } from '@/api/basic/config'
import type { SystemConfig } from '@/types/basic'
import type { SearchField } from '@/types/search'
import ConfigFormDialog from './components/ConfigFormDialog.vue'

const { t } = useLocale()

// 搜索：复用 useSearch，搜索字段以 i18n key 配置（顶层只调用一次，复用 getSearchParams）
const { searchModel, getSearchParams } = useSearch<{ key?: string; label?: string }>({
  defaultModel: { key: '', label: '' },
})

const searchFields: SearchField[] = [
  { prop: 'key', label: 'basic.config.key', type: 'input', placeholder: 'basic.config.form.keyPlaceholder' },
  { prop: 'label', label: 'basic.config.label', type: 'input', placeholder: 'basic.config.form.labelPlaceholder' },
]

// 列表：useTable
const table = useTable<SystemConfig>((params) => getConfigList(params as never))

const onSearch = (): void => {
  Object.assign(table.query, getSearchParams())
  void table.handleSearch()
}

const onReset = (): void => {
  searchModel.key = ''
  searchModel.label = ''
  Object.assign(table.query, { key: undefined, label: undefined })
  void table.handleSearch()
}

const handleSizeChange = (size: number): void => {
  table.query.pageSize = size
  table.query.pageIndex = 1
  void table.reload()
}

// 多选
const selectedRows = ref<SystemConfig[]>([])
const handleSelectionChange = (rows: unknown[]): void => {
  selectedRows.value = rows as SystemConfig[]
}

// 内联编辑
const editingId = ref<string>('')
const editValue = ref<string>('')
const numberEditValue = ref<number>(0)
const booleanEditValue = ref<boolean>(false)

const displayValue = (row: SystemConfig): string => {
  if (row.type === 'boolean') {
    return row.value === 'true' ? t('common.status.enabled') : t('common.status.disabled')
  }
  return row.value
}

const typeLabel = (type: SystemConfig['type']): string => {
  if (type === 'string') return t('basic.config.typeString')
  if (type === 'number') return t('basic.config.typeNumber')
  return t('basic.config.typeBoolean')
}

const handleEditInline = (row: SystemConfig): void => {
  editingId.value = row.id
  editValue.value = row.value
  numberEditValue.value = Number(row.value) || 0
  booleanEditValue.value = row.value === 'true'
}

const handleCancelInline = (): void => {
  editingId.value = ''
}

const handleSaveInline = async (row: SystemConfig): Promise<void> => {
  const newValue =
    row.type === 'string'
      ? editValue.value
      : row.type === 'number'
        ? String(numberEditValue.value)
        : booleanEditValue.value
        ? 'true'
        : 'false'
  try {
    await updateConfig(row.id, { value: newValue })
    ElMessage.success(t('basic.config.message.updateSuccess'))
    editingId.value = ''
    await table.reload()
  } catch {
    // 错误已由拦截器处理
  }
}

// 新增
const dialogVisible = ref(false)
const handleAdd = (): void => {
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: SystemConfig): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.config.message.deleteConfirm', { label: row.label }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      },
    )
    await deleteConfig(row.id)
    ElMessage.success(t('basic.config.message.deleteSuccess'))
    await table.reload()
  } catch {
    // 用户取消或请求失败
  }
}

const handleBatchDelete = async (): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.config.message.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.tips'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      },
    )
    await deleteConfigBatch(selectedRows.value.map((r) => r.id))
    ElMessage.success(t('basic.config.message.deleteSuccess'))
    selectedRows.value = []
    await table.reload()
  } catch {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.config-page {
  padding: $spacing-md;
}
</style>