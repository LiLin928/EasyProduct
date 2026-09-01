<template>
  <div class="stock-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />
    <el-card class="stock-page__table">
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
          :label="t('crm.stock.warehouse')"
          width="120"
        />
        <el-table-column
          prop="skuCode"
          :label="t('crm.stock.skuCode')"
          width="120"
        />
        <el-table-column
          prop="skuName"
          :label="t('crm.stock.skuName')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="spec"
          :label="t('crm.stock.spec')"
          min-width="140"
          show-overflow-tooltip
        />
        <el-table-column
          prop="unit"
          :label="t('crm.stock.unit')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="available"
          :label="t('crm.stock.available')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="locked"
          :label="t('crm.stock.locked')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="total"
          :label="t('crm.stock.total')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            <span class="text-primary">{{ row.total }}</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="minLimit"
          :label="t('crm.stock.minLimit')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="maxLimit"
          :label="t('crm.stock.maxLimit')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="updatedAt"
          :label="t('crm.stock.updatedAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="120"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openStockRecord(row)"
            >
              {{ t('crm.stock.viewRecord') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <StockRecordDialog
      :visible="recordDialogVisible"
      :stock-info="selectedStock"
      @update:visible="recordDialogVisible = $event"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { getStockList } from '@/api/crm/stock'
import { getWarehouseOptions } from '@/api/crm/warehouse'
import type { Stock } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import StockRecordDialog from './components/StockRecordDialog.vue'

const { t } = useI18n()

const warehouseOptions = ref<Array<{ id: string; name: string; code: string }>>([])
onMounted(async () => {
  try {
    warehouseOptions.value = await getWarehouseOptions()
  } catch {
    // ignore
  }
})

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'warehouseId',
    label: 'crm.stock.searchWarehouse',
    type: 'select',
    options: warehouseOptions.value.map(w => ({ label: w.name, value: w.id })),
  },
  { prop: 'keyword', label: 'crm.stock.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<Stock>(getStockList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { warehouseId: '', keyword: '' },
})

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

// 库存记录弹窗
const recordDialogVisible = ref(false)
const selectedStock = ref<Stock | null>(null)

const openStockRecord = (row: Stock) => {
  selectedStock.value = row
  recordDialogVisible.value = true
}
</script>

<style scoped lang="scss">
.stock-page {
  .text-primary { color: var(--el-color-primary); font-weight: 600; }
}
</style>
