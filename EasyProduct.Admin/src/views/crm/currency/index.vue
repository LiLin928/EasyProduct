<template>
  <div class="currency-page">
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
          {{ t('crm.currency.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="currency-page__table">
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
          :label="t('crm.currency.code')"
          width="100"
        />
        <el-table-column
          prop="name"
          :label="t('crm.currency.name')"
          min-width="120"
        />
        <el-table-column
          prop="symbol"
          :label="t('crm.currency.symbol')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="exchangeRate"
          :label="t('crm.currency.exchangeRate')"
          width="120"
          align="right"
        />
        <el-table-column
          prop="isDefault"
          :label="t('crm.currency.isDefault')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              v-if="row.isDefault"
              type="success"
              size="small"
            >
              {{ t('common.confirm') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.currency.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="CURRENCY_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('crm.currency.createdAt')"
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
              :disabled="row.isDefault"
              @click="handleDelete(row)"
            >
              {{ t('common.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <CurrencyFormDialog
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
import { getCurrencyList, deleteCurrency } from '@/api/crm/currency'
import type { Currency } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import CurrencyFormDialog from './components/CurrencyFormDialog.vue'

const { t } = useI18n()

const CURRENCY_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'crm.currency.statusActive', type: 'success' },
  inactive: { label: 'crm.currency.statusInactive', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'code', label: 'crm.currency.searchCode', type: 'input' },
  { prop: 'status', label: 'crm.currency.searchStatus', type: 'select', options: [
    { label: 'crm.currency.statusActive', value: 'active' },
    { label: 'crm.currency.statusInactive', value: 'inactive' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<Currency>(getCurrencyList, {
  defaultSearchModel: { code: '', status: '' },
  deleteFn: deleteCurrency,
  deleteConfirmText: t('crm.currency.deleteConfirm'),
})
</script>

<style scoped lang="scss">
.currency-page {
}
</style>
