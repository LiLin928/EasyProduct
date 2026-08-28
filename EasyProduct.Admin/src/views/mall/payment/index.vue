<template>
  <div class="payment-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="payment-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="orderNo"
          :label="t('mall.payment.orderNo')"
          width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="amount"
          :label="t('mall.payment.amount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            ¥{{ row.amount.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="method"
          :label="t('mall.payment.method')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="status"
          :label="t('mall.payment.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="PAYMENT_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="transactionId"
          :label="t('mall.payment.transactionId')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="paidAt"
          :label="t('mall.payment.paidAt')"
          width="160"
        />
        <el-table-column
          prop="createdAt"
          :label="t('mall.payment.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="100"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-if="row.status === 'success'"
              link
              type="danger"
              @click="handleRefund(row)"
            >
              {{ t('mall.payment.refund') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { getPaymentList, refundPayment } from '@/api/mall/payment'
import type { PaymentRecord } from '@/types/mall'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const PAYMENT_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  pending: { label: 'mall.payment.statusPending', type: 'warning' },
  success: { label: 'mall.payment.statusSuccess', type: 'success' },
  failed: { label: 'mall.payment.statusFailed', type: 'danger' },
  refunded: { label: 'mall.payment.statusRefunded', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'orderNo', label: 'mall.payment.searchOrderNo', type: 'input' },
  { prop: 'status', label: 'mall.payment.searchStatus', type: 'select', options: [
    { label: 'mall.payment.statusPending', value: 'pending' },
    { label: 'mall.payment.statusSuccess', value: 'success' },
    { label: 'mall.payment.statusFailed', value: 'failed' },
    { label: 'mall.payment.statusRefunded', value: 'refunded' },
  ] },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<PaymentRecord>(getPaymentList)

const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { orderNo: '', status: '' },
})

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const handleRefund = async (row: PaymentRecord) => {
  try {
    await ElMessageBox.confirm(t('mall.payment.refundConfirm'), t('common.tips'), { type: 'warning' })
    await refundPayment(row.id)
    ElMessage.success(t('mall.payment.message.refundSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>

<style scoped lang="scss">
.payment-page {
}
</style>
