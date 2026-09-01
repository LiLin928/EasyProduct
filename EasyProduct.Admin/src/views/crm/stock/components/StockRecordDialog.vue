<template>
  <el-dialog
    :model-value="visible"
    :title="dialogTitle"
    width="900px"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <div
      v-if="stockInfo"
      class="stock-info"
    >
      <el-descriptions
        :column="4"
        border
      >
        <el-descriptions-item :label="t('crm.stock.warehouse')">
          {{ stockInfo.warehouseName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stock.skuCode')">
          {{ stockInfo.skuCode }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stock.skuName')">
          {{ stockInfo.skuName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stock.available')">
          <span class="text-primary">{{ stockInfo.available }}</span>
        </el-descriptions-item>
      </el-descriptions>
    </div>

    <el-divider content-position="left">
      {{ t('crm.stock.recordHistory') }}
    </el-divider>

    <BaseTable
      :loading="loading"
      :data="recordList"
      :total="recordTotal"
      :current-page="query.pageIndex"
      :page-size="query.pageSize"
      @page-change="handlePageChange"
    >
      <el-table-column
        prop="createdAt"
        :label="t('crm.stockRecord.createdAt')"
        width="160"
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
      >
        <template #default="{ row }">
          <span :class="row.type === 'in' ? 'text-success' : 'text-warning'">
            {{ row.type === 'in' ? '+' : '-' }}{{ row.quantity }}
          </span>
        </template>
      </el-table-column>
      <el-table-column
        prop="operator"
        :label="t('crm.stockRecord.operator')"
        width="100"
      />
      <el-table-column
        prop="remark"
        :label="t('crm.stockRecord.remark')"
        min-width="150"
        show-overflow-tooltip
      />
    </BaseTable>

    <template #footer>
      <el-button @click="handleClose(false)">
        {{ t('common.close') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { getStockRecordBySku } from '@/api/crm/stock-record'
import type { Stock, StockRecord } from '@/types/crm'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const props = defineProps<{
  visible: boolean
  stockInfo: Stock | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const { t } = useI18n()

const dialogTitle = computed(() => {
  if (!props.stockInfo) return t('crm.stock.record')
  return `${props.stockInfo.skuName} - ${t('crm.stock.record')}`
})

const STOCK_RECORD_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  in: { label: 'crm.stockRecord.typeIn', type: 'success' },
  out: { label: 'crm.stockRecord.typeOut', type: 'warning' },
}

const STOCK_RECORD_SOURCE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  purchase_in: { label: 'crm.stockRecord.sourcePurchaseIn', type: 'success' },
  sales_out: { label: 'crm.stockRecord.sourceSalesOut', type: 'warning' },
  mall_out: { label: 'crm.stockRecord.sourceMallOut', type: '' },
  check_adjust: { label: 'crm.stockRecord.sourceCheckAdjust', type: 'info' },
  reversal_return: { label: 'crm.stockRecord.sourceReversalReturn', type: 'danger' },
}

const query = ref({
  pageIndex: 1,
  pageSize: 10,
})

const { loading, list: recordList, total: recordTotal, handlePageChange, reload } = useTable<StockRecord>(async (params) => {
  if (!props.stockInfo) return { list: [], total: 0 }
  const result = await getStockRecordBySku({
    ...params,
    warehouseId: props.stockInfo.warehouseId,
    skuCode: props.stockInfo.skuCode,
  })
  return result
})

// 监听 visible 变化，打开时重新加载
watch(() => props.visible, (newVisible) => {
  if (newVisible && props.stockInfo) {
    query.value.pageIndex = 1
    void reload()
  }
})

const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}
</script>

<style scoped lang="scss">
.stock-info {
  margin-bottom: 16px;
}

.text-primary {
  color: var(--el-color-primary);
  font-weight: 600;
}

.text-success {
  color: var(--el-color-success);
  font-weight: 600;
}

.text-warning {
  color: var(--el-color-warning);
  font-weight: 600;
}
</style>
