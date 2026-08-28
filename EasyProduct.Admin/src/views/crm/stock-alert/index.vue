<template>
  <div class="stock-alert-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    >
      <template #toolbar>
        <el-button
          type="primary"
          :disabled="selection.length === 0"
          @click="handleBatchResolve"
        >
          {{ t('crm.stockAlert.batchResolve') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="stock-alert-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
        @selection-change="handleSelectionChange"
      >
        <el-table-column
          type="selection"
          width="50"
          :selectable="(row: StockAlert) => row.status === 'pending'"
        />
        <el-table-column
          prop="warehouseName"
          :label="t('crm.stockAlert.warehouse')"
          width="120"
        />
        <el-table-column
          prop="skuCode"
          :label="t('crm.stockAlert.skuCode')"
          width="120"
        />
        <el-table-column
          prop="skuName"
          :label="t('crm.stockAlert.skuName')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="spec"
          :label="t('crm.stockAlert.spec')"
          min-width="140"
          show-overflow-tooltip
        />
        <el-table-column
          prop="available"
          :label="t('crm.stockAlert.available')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="alertType"
          :label="t('crm.stockAlert.alertType')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.alertType"
              :options="ALERT_TYPE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.stockAlert.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="ALERT_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('crm.stockAlert.createdAt')"
          width="160"
        />
        <el-table-column
          prop="remark"
          :label="t('crm.stockAlert.remark')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          :label="t('common.actions')"
          width="100"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-if="row.status === 'pending'"
              link
              type="primary"
              @click="handleResolve(row)"
            >
              {{ t('crm.stockAlert.resolve') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>
  </div>
</template>

 <script setup lang="ts">
 import { ref, computed, onMounted } from 'vue'
 import { useI18n } from 'vue-i18n'
 import { ElMessage, ElMessageBox } from 'element-plus'
 import { useTable } from '@/composables/useTable'
 import { useSearch } from '@/composables/useSearch'
 import { getStockAlertList, resolveStockAlert, batchResolveStockAlerts } from '@/api/crm/stock-alert'
 import { getWarehouseOptions } from '@/api/crm/warehouse'
 import type { StockAlert } from '@/types/crm'
 import type { SearchField } from '@/types/search'
 import BaseTable from '@/components/common/BaseTable.vue'
 import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
 import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

 const { t } = useI18n()

 const ALERT_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   low: { label: 'crm.stockAlert.typeLow', type: 'danger' },
   high: { label: 'crm.stockAlert.typeHigh', type: 'warning' },
 }

 const ALERT_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   pending: { label: 'crm.stockAlert.statusPending', type: 'warning' },
   resolved: { label: 'crm.stockAlert.statusResolved', type: 'success' },
 }

 const warehouseOptions = ref<Array<{ id: string; name: string; code: string }>>([])
 onMounted(async () => {
   try {
     warehouseOptions.value = await getWarehouseOptions()
   } catch {
     // ignore
   }
 })

 const selection = ref<StockAlert[]>([])
 const handleSelectionChange = (rows: unknown[]) => {
   selection.value = rows as StockAlert[]
 }

 const searchFields = computed<SearchField[]>(() => [
   {
     prop: 'warehouseId',
     label: 'crm.stockAlert.searchWarehouse',
     type: 'select',
     options: warehouseOptions.value.map(w => ({ label: w.name, value: w.id })),
   },
   {
     prop: 'alertType',
     label: 'crm.stockAlert.searchAlertType',
     type: 'select',
     options: [
       { label: 'crm.stockAlert.typeLow', value: 'low' },
       { label: 'crm.stockAlert.typeHigh', value: 'high' },
     ],
   },
   {
     prop: 'status',
     label: 'crm.stockAlert.searchStatus',
     type: 'select',
     options: [
       { label: 'crm.stockAlert.statusPending', value: 'pending' },
       { label: 'crm.stockAlert.statusResolved', value: 'resolved' },
     ],
   },
   { prop: 'keyword', label: 'crm.stockAlert.searchKeyword', type: 'input' },
 ])

 const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<StockAlert>(getStockAlertList)
 const { searchModel, resetModel, getSearchParams } = useSearch({
   defaultModel: { warehouseId: '', alertType: '', status: '', keyword: '' },
 })

 const handleSearch = () => {
   Object.assign(query, getSearchParams())
   void tableSearch()
 }

 const handleReset = () => {
   resetModel()
   void tableReset()
 }

 const handleResolve = async (row: StockAlert) => {
   try {
     await ElMessageBox.confirm(t('crm.stockAlert.resolveConfirm'), t('common.tips'), { type: 'warning' })
     await resolveStockAlert(row.id)
     ElMessage.success(t('crm.stockAlert.message.resolveSuccess'))
     await reload()
   } catch {
     // user cancelled
   }
 }

 const handleBatchResolve = async () => {
   if (selection.value.length === 0) return
   try {
     await ElMessageBox.confirm(
       t('crm.stockAlert.batchResolveConfirm', { count: selection.value.length }),
       t('common.tips'),
       { type: 'warning' },
     )
     const ids = selection.value.map(a => a.id)
     await batchResolveStockAlerts(ids)
     ElMessage.success(t('crm.stockAlert.message.batchResolveSuccess'))
     selection.value = []
     await reload()
   } catch {
     // user cancelled
   }
 }
 </script>

 <style scoped lang="scss">
 .stock-alert-page {
 }
 </style>
