<template>
  <div class="stock-check-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />
    <el-card class="stock-check-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="checkNo"
          :label="t('crm.stockCheck.checkNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="warehouseName"
          :label="t('crm.stockCheck.warehouse')"
          width="120"
        />
        <el-table-column
          prop="checker"
          :label="t('crm.stockCheck.checker')"
          width="100"
        />
        <el-table-column
          prop="checkDate"
          :label="t('crm.stockCheck.checkDate')"
          width="120"
        />
        <el-table-column
          prop="status"
          :label="t('crm.stockCheck.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="remark"
          :label="t('crm.stockCheck.remark')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="createdAt"
          :label="t('crm.stockCheck.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="240"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openDetail(row)"
            >
              {{ t('crm.stockCheck.detail') }}
            </el-button>
            <el-button
              v-if="row.status === 'draft'"
              link
              type="success"
              @click="handleStatusChange(row, 'counting')"
            >
              {{ t('crm.stockCheck.actionCounting') }}
            </el-button>
            <el-button
              v-if="row.status === 'counting'"
              link
              type="success"
              @click="handleStatusChange(row, 'completed')"
            >
              {{ t('crm.stockCheck.actionComplete') }}
            </el-button>
            <el-button
              v-if="row.status === 'draft'"
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
    <StockCheckDetailDialog
      :visible="detailVisible"
      :check="selectedCheck"
      @update:visible="detailVisible = $event"
    />
  </div>
</template>

 <script setup lang="ts">
 import { ref, computed, onMounted } from 'vue'
 import { useI18n } from 'vue-i18n'
 import { ElMessage, ElMessageBox } from 'element-plus'
 import { useTable } from '@/composables/useTable'
 import { useSearch } from '@/composables/useSearch'
 import {
   getStockCheckList,
   getStockCheckById,
   updateStockCheckStatus,
   deleteStockCheck,
 } from '@/api/crm/stock-check'
 import { getWarehouseOptions } from '@/api/crm/warehouse'
 import type { StockCheck, StockCheckStatus } from '@/types/crm'
 import type { SearchField } from '@/types/search'
 import BaseTable from '@/components/common/BaseTable.vue'
 import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
 import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
 import StockCheckDetailDialog from './components/StockCheckDetailDialog.vue'

 const { t } = useI18n()

 const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   draft: { label: 'crm.stockCheck.statusDraft', type: 'info' },
   counting: { label: 'crm.stockCheck.statusCounting', type: 'warning' },
   completed: { label: 'crm.stockCheck.statusCompleted', type: 'success' },
 }

 const warehouseOptions = ref<Array<{ id: string; name: string; code: string }>>([])
 onMounted(async () => {
   try {
     warehouseOptions.value = await getWarehouseOptions()
   } catch {
     // ignore
   }
 })

 const detailVisible = ref(false)
 const selectedCheck = ref<StockCheck | null>(null)

 const searchFields = computed<SearchField[]>(() => [
   {
     prop: 'warehouseId',
     label: 'crm.stockCheck.searchWarehouse',
     type: 'select',
     options: warehouseOptions.value.map(w => ({ label: w.name, value: w.id })),
   },
   {
     prop: 'status',
     label: 'crm.stockCheck.searchStatus',
     type: 'select',
     options: [
       { label: 'crm.stockCheck.statusDraft', value: 'draft' },
       { label: 'crm.stockCheck.statusCounting', value: 'counting' },
       { label: 'crm.stockCheck.statusCompleted', value: 'completed' },
     ],
   },
   { prop: 'checkNo', label: 'crm.stockCheck.searchCheckNo', type: 'input' },
 ])

 const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<StockCheck>(getStockCheckList)
 const { searchModel, resetModel, getSearchParams } = useSearch({
   defaultModel: { warehouseId: '', status: '', checkNo: '' },
 })

 const handleSearch = () => {
   Object.assign(query, getSearchParams())
   void tableSearch()
 }

 const handleReset = () => {
   resetModel()
   void tableReset()
 }

 const openDetail = async (row: StockCheck) => {
   try {
     const detail = await getStockCheckById(row.id)
     selectedCheck.value = detail
     detailVisible.value = true
   } catch {
     ElMessage.error(t('common.loadFailed'))
   }
 }

 const STATUS_LABEL_MAP: Record<StockCheckStatus, string> = {
   draft: 'crm.stockCheck.statusDraft',
   counting: 'crm.stockCheck.statusCounting',
   completed: 'crm.stockCheck.statusCompleted',
 }

 const handleStatusChange = async (row: StockCheck, newStatus: StockCheckStatus) => {
   try {
     const msg = t('crm.stockCheck.confirmMessage', { status: t(STATUS_LABEL_MAP[newStatus]) })
     await ElMessageBox.confirm(msg, t('common.tips'), { type: 'warning' })
     await updateStockCheckStatus(row.id, { status: newStatus })
     ElMessage.success(t('crm.stockCheck.message.statusUpdateSuccess'))
     await reload()
   } catch {
     // user cancelled
   }
 }

 const handleDelete = async (row: StockCheck) => {
   try {
     await ElMessageBox.confirm(t('crm.stockCheck.deleteConfirm'), t('common.tips'), { type: 'warning' })
     await deleteStockCheck(row.id)
     ElMessage.success(t('crm.stockCheck.message.deleteSuccess'))
     await reload()
   } catch {
     // user cancelled
   }
 }
 </script>

 <style scoped lang="scss">
 .stock-check-page {
 }
 </style>
