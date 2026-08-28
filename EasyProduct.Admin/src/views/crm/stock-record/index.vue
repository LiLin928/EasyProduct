<template>
  <div class="stock-record-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />
    <el-card class="stock-record-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="warehouseName"
          :label="t('crm.stockRecord.warehouse')"
          width="120"
        />
        <el-table-column
          prop="skuCode"
          :label="t('crm.stockRecord.skuCode')"
          width="120"
        />
        <el-table-column
          prop="skuName"
          :label="t('crm.stockRecord.skuName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('crm.stockRecord.type')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.type"
              :options="STOCK_RECORD_TYPE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="sourceType"
          :label="t('crm.stockRecord.sourceType')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.sourceType"
              :options="STOCK_RECORD_SOURCE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="sourceOrderNo"
          :label="t('crm.stockRecord.sourceOrderNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="quantity"
          :label="t('crm.stockRecord.quantity')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="operator"
          :label="t('crm.stockRecord.operator')"
          width="100"
        />
        <el-table-column
          prop="createdAt"
          :label="t('crm.stockRecord.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="80"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openDetail(row)"
            >
              {{ t('crm.stockRecord.detail') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>
    <StockRecordDetailDialog
      :visible="detailVisible"
      :record="selectedRecord"
      @update:visible="detailVisible = $event"
    />
  </div>
</template>

 <script setup lang="ts">
 import { ref, computed, onMounted } from 'vue'
 import { useI18n } from 'vue-i18n'
 import { ElMessage } from 'element-plus'
 import { useTable } from '@/composables/useTable'
 import { useSearch } from '@/composables/useSearch'
 import { getStockRecordList, getStockRecordById } from '@/api/crm/stock-record'
 import { getWarehouseOptions } from '@/api/crm/warehouse'
 import type { StockRecord } from '@/types/crm'
 import { STOCK_RECORD_SOURCE_OPTIONS } from '@/types/crm'
 import type { SearchField } from '@/types/search'
 import BaseTable from '@/components/common/BaseTable.vue'
 import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
 import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
 import StockRecordDetailDialog from './components/StockRecordDetailDialog.vue'

 const { t } = useI18n()

 const STOCK_RECORD_SOURCE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   purchase_in: { label: 'crm.stockRecord.sourcePurchaseIn', type: 'success' },
   sales_out: { label: 'crm.stockRecord.sourceSalesOut', type: 'warning' },
   mall_out: { label: 'crm.stockRecord.sourceMallOut', type: '' },
   check_adjust: { label: 'crm.stockRecord.sourceCheckAdjust', type: 'info' },
   reversal_return: { label: 'crm.stockRecord.sourceReversalReturn', type: 'danger' },
 }

 const STOCK_RECORD_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   in: { label: 'crm.stockRecord.typeIn', type: 'success' },
   out: { label: 'crm.stockRecord.typeOut', type: 'warning' },
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
 const selectedRecord = ref<StockRecord | null>(null)

 const searchFields = computed<SearchField[]>(() => [
   {
     prop: 'warehouseId',
     label: 'crm.stockRecord.searchWarehouse',
     type: 'select',
     options: warehouseOptions.value.map(w => ({ label: w.name, value: w.id })),
   },
   {
     prop: 'sourceType',
     label: 'crm.stockRecord.searchSourceType',
     type: 'select',
     options: STOCK_RECORD_SOURCE_OPTIONS.map(o => ({ label: o.labelKey, value: o.value })),
   },
  { prop: 'dateRange', label: 'crm.stockRecord.searchDateRange', type: 'dateRange' },
   { prop: 'keyword', label: 'crm.stockRecord.searchKeyword', type: 'input' },
 ])

 const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<StockRecord>(getStockRecordList)
 const { searchModel, resetModel, getSearchParams } = useSearch({
   defaultModel: { warehouseId: '', sourceType: '', dateRange: [], keyword: '' },
 })

 const handleSearch = () => {
   const params = getSearchParams() as Record<string, unknown>
   if (params.dateRange && Array.isArray(params.dateRange) && params.dateRange.length === 2) {
     params.startDate = params.dateRange[0]
     params.endDate = params.dateRange[1]
   }
   delete params.dateRange
   Object.assign(query, params)
   void tableSearch()
 }

 const handleReset = () => {
   resetModel()
   delete query.startDate
   delete query.endDate
   void tableReset()
 }

 const openDetail = async (row: StockRecord) => {
   try {
     const detail = await getStockRecordById(row.id)
     selectedRecord.value = detail
     detailVisible.value = true
   } catch {
     ElMessage.error(t('common.loadFailed'))
   }
 }
 </script>

 <style scoped lang="scss">
 .stock-record-page {
 }
 </style>
