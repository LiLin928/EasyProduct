<template>
  <div class="column-template-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    >
      <template #toolbar>
        <el-button
          type="primary"
          @click="openCreate"
        >
          {{ t('report.columnTemplate.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="column-template-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="name"
          :label="t('report.columnTemplate.name')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="field"
          :label="t('report.columnTemplate.field')"
          width="150"
        />
        <el-table-column
          prop="type"
          :label="t('report.columnTemplate.type')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.type"
              :options="COLUMN_TYPE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="width"
          :label="t('report.columnTemplate.width')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="format"
          :label="t('report.columnTemplate.format')"
          width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="sortable"
          :label="t('report.columnTemplate.sortable')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              :type="row.sortable ? 'success' : 'info'"
              size="small"
            >
              {{ row.sortable ? t('common.yes') : t('common.no') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="remark"
          :label="t('report.columnTemplate.remark')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="createdAt"
          :label="t('report.columnTemplate.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="150"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openEdit(row)"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              link
              type="danger"
              @click="handleDelete(row)"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <ColumnTemplateFormDialog
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      :row-data="formDialog.payload.value"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCrud } from '@/composables/useCrud'
import { getColumnTemplateList, deleteColumnTemplate } from '@/api/report/column-template'
import type { ColumnTemplate } from '@/types/report'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import ColumnTemplateFormDialog from './components/ColumnTemplateFormDialog.vue'

const { t } = useI18n()

const COLUMN_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  string: { label: 'report.columnTemplate.typeString', type: 'info' },
  number: { label: 'report.columnTemplate.typeNumber', type: 'success' },
  date: { label: 'report.columnTemplate.typeDate', type: 'warning' },
  currency: { label: 'report.columnTemplate.typeCurrency', type: 'danger' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'name', label: 'report.columnTemplate.searchName', type: 'input' },
  { prop: 'type', label: 'report.columnTemplate.searchType', type: 'select', options: [
    { label: 'report.columnTemplate.typeString', value: 'string' },
    { label: 'report.columnTemplate.typeNumber', value: 'number' },
    { label: 'report.columnTemplate.typeDate', value: 'date' },
    { label: 'report.columnTemplate.typeCurrency', value: 'currency' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<ColumnTemplate>(getColumnTemplateList, {
  defaultSearchModel: { name: '', type: '' },
  deleteFn: deleteColumnTemplate,
  deleteConfirmText: t('report.columnTemplate.deleteConfirm'),
})
</script>

<style scoped lang="scss">
.column-template-page {
}
</style>
