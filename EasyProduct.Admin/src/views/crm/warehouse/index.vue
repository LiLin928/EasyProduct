<template>
  <div class="warehouse-page">
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
          {{ t('crm.warehouse.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="warehouse-page__table">
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
          :label="t('crm.warehouse.code')"
          width="100"
        />
        <el-table-column
          prop="name"
          :label="t('crm.warehouse.name')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="address"
          :label="t('crm.warehouse.address')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="manager"
          :label="t('crm.warehouse.manager')"
          width="100"
        />
        <el-table-column
          prop="phone"
          :label="t('crm.warehouse.phone')"
          width="130"
        />
        <el-table-column
          prop="status"
          :label="t('crm.warehouse.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="WAREHOUSE_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('crm.warehouse.createdAt')"
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

    <WarehouseFormDialog
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
 import { getWarehouseList, deleteWarehouse } from '@/api/crm/warehouse'
 import type { Warehouse } from '@/types/crm'
 import type { SearchField } from '@/types/search'
 import BaseTable from '@/components/common/BaseTable.vue'
 import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
 import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
 import WarehouseFormDialog from './components/WarehouseFormDialog.vue'

 const { t } = useI18n()

 const WAREHOUSE_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   active: { label: 'crm.warehouse.statusActive', type: 'success' },
   inactive: { label: 'crm.warehouse.statusInactive', type: 'info' },
 }

 const searchFields = computed<SearchField[]>(() => [
   { prop: 'code', label: 'crm.warehouse.searchCode', type: 'input' },
   { prop: 'name', label: 'crm.warehouse.searchName', type: 'input' },
   { prop: 'status', label: 'crm.warehouse.searchStatus', type: 'select', options: [
     { label: 'crm.warehouse.statusActive', value: 'active' },
     { label: 'crm.warehouse.statusInactive', value: 'inactive' },
   ] },
 ])

 const {
   loading, list, total, query, searchModel,
   handleSearch, handleReset, handlePageChange,
   formDialog, openCreate, openEdit,
   handleDelete, reload,
 } = useCrud<Warehouse>(getWarehouseList, {
   defaultSearchModel: { code: '', name: '', status: '' },
   deleteFn: deleteWarehouse,
   deleteConfirmText: t('crm.warehouse.deleteConfirm'),
 })
 </script>

 <style scoped lang="scss">
 .warehouse-page {
 }
 </style>
