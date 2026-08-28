<template>
  <div class="customer-page">
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
          {{ t('crm.customer.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="customer-page__table">
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
          :label="t('crm.customer.code')"
          width="100"
        />
        <el-table-column
          prop="name"
          :label="t('crm.customer.name')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('crm.customer.type')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.type"
              :options="CUSTOMER_TYPE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="contactPerson"
          :label="t('crm.customer.contactPerson')"
          width="100"
        />
        <el-table-column
          prop="phone"
          :label="t('crm.customer.phone')"
          width="130"
        />
        <el-table-column
          prop="salesPersonName"
          :label="t('crm.customer.salesPersonName')"
          width="100"
        />
        <el-table-column
          prop="creditLimit"
          :label="t('crm.customer.creditLimit')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ row.creditLimit ? `Y${row.creditLimit.toLocaleString()}` : '-' }}
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.customer.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="CUSTOMER_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('crm.customer.createdAt')"
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

    <CustomerFormDialog
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
import { getCustomerList, deleteCustomer } from '@/api/crm/customer'
import type { Customer } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import CustomerFormDialog from './components/CustomerFormDialog.vue'

const { t } = useI18n()

const CUSTOMER_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  b2b: { label: 'crm.customer.typeB2b', type: '' },
  retail: { label: 'crm.customer.typeRetail', type: 'info' },
}

const CUSTOMER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'crm.customer.statusActive', type: 'success' },
  inactive: { label: 'crm.customer.statusInactive', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'code', label: 'crm.customer.searchCode', type: 'input' },
  { prop: 'name', label: 'crm.customer.searchName', type: 'input' },
  { prop: 'type', label: 'crm.customer.searchType', type: 'select', options: [
    { label: 'crm.customer.typeB2b', value: 'b2b' },
    { label: 'crm.customer.typeRetail', value: 'retail' },
  ] },
  { prop: 'status', label: 'crm.customer.searchStatus', type: 'select', options: [
    { label: 'crm.customer.statusActive', value: 'active' },
    { label: 'crm.customer.statusInactive', value: 'inactive' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<Customer>(getCustomerList, {
  defaultSearchModel: { code: '', name: '', type: '', status: '' },
  deleteFn: deleteCustomer,
  deleteConfirmText: t('crm.customer.deleteConfirm'),
})
</script>

<style scoped lang="scss">
.customer-page {
}
</style>
