<template>
  <div class="tax-rate-page">
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
          {{ t('crm.taxRate.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="tax-rate-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="code"
          :label="t('crm.taxRate.code')"
          width="100"
        />
        <el-table-column
          prop="name"
          :label="t('crm.taxRate.name')"
          min-width="150"
        />
        <el-table-column
          prop="rate"
          :label="t('crm.taxRate.rate')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            {{ row.rate }}%
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.taxRate.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="TAX_RATE_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="remark"
          :label="t('crm.taxRate.remark')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="createdAt"
          :label="t('crm.taxRate.createdAt')"
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

    <TaxRateFormDialog
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
import { getTaxRateList, deleteTaxRate } from '@/api/crm/tax-rate'
import type { TaxRate } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import TaxRateFormDialog from './components/TaxRateFormDialog.vue'

const { t } = useI18n()

const TAX_RATE_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'crm.taxRate.statusActive', type: 'success' },
  inactive: { label: 'crm.taxRate.statusInactive', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'code', label: 'crm.taxRate.searchCode', type: 'input' },
  { prop: 'status', label: 'crm.taxRate.searchStatus', type: 'select', options: [
    { label: 'crm.taxRate.statusActive', value: 'active' },
    { label: 'crm.taxRate.statusInactive', value: 'inactive' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<TaxRate>(getTaxRateList, {
  defaultSearchModel: { code: '', status: '' },
  deleteFn: deleteTaxRate,
  deleteConfirmText: t('crm.taxRate.deleteConfirm'),
})
</script>

<style scoped lang="scss">
.tax-rate-page {
}
</style>
