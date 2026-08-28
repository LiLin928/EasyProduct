<template>
  <div class="supplier-page">
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
          {{ t('crm.supplier.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="supplier-page__table">
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
          :label="t('crm.supplier.code')"
          width="100"
        />
        <el-table-column
          prop="name"
          :label="t('crm.supplier.name')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="contactPerson"
          :label="t('crm.supplier.contactPerson')"
          width="100"
        />
        <el-table-column
          prop="phone"
          :label="t('crm.supplier.phone')"
          width="130"
        />
        <el-table-column
          prop="bankName"
          :label="t('crm.supplier.bankName')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="status"
          :label="t('crm.supplier.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="SUPPLIER_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('crm.supplier.createdAt')"
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

    <SupplierFormDialog
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
import { getSupplierList, deleteSupplier } from '@/api/crm/supplier'
import type { Supplier } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import SupplierFormDialog from './components/SupplierFormDialog.vue'

const { t } = useI18n()

const SUPPLIER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'crm.supplier.statusActive', type: 'success' },
  inactive: { label: 'crm.supplier.statusInactive', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'code', label: 'crm.supplier.searchCode', type: 'input' },
  { prop: 'name', label: 'crm.supplier.searchName', type: 'input' },
  { prop: 'status', label: 'crm.supplier.searchStatus', type: 'select', options: [
    { label: 'crm.supplier.statusActive', value: 'active' },
    { label: 'crm.supplier.statusInactive', value: 'inactive' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<Supplier>(getSupplierList, {
  defaultSearchModel: { code: '', name: '', status: '' },
  deleteFn: deleteSupplier,
  deleteConfirmText: t('crm.supplier.deleteConfirm'),
})
</script>

<style scoped lang="scss">
.supplier-page {
}
</style>
