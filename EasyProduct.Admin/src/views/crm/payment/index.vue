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
          prop="paymentNo"
          :label="t('crm.payment.paymentNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('crm.payment.type')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.type"
              :options="TYPE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="orderType"
          :label="t('crm.payment.orderType')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            {{ row.orderType === 'sales' ? t('crm.payment.orderTypeSales') : t('crm.payment.orderTypePurchase') }}
          </template>
        </el-table-column>
        <el-table-column
          prop="orderNo"
          :label="t('crm.payment.orderNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="partyName"
          :label="t('crm.payment.partyName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="amount"
          :label="t('crm.payment.amount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="method"
          :label="t('crm.payment.method')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.method"
              :options="METHOD_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.payment.status')"
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
          prop="createdAt"
          :label="t('crm.payment.createdAt')"
          width="170"
        />
        <el-table-column
          :label="t('common.actions')"
          width="200"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openDetail(row)"
            >
              {{ t('common.detail') }}
            </el-button>
            <el-button
              v-if="row.status === 'draft'"
              link
              type="success"
              @click="handleStatusChange(row, 'confirmed')"
            >
              {{ t('crm.payment.actionConfirm') }}
            </el-button>
            <el-button
              v-if="row.status === 'confirmed'"
              link
              type="warning"
              @click="handleStatusChange(row, 'voided')"
            >
              {{ t('crm.payment.actionVoid') }}
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

    <PaymentDetailDialog
      :visible="detailVisible"
      :payment="selectedPayment"
      @update:visible="detailVisible = $event"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import {
  getPaymentList,
  deletePayment,
  updatePaymentStatus,
} from '@/api/crm/payment'
import type { Payment, PaymentStatus } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import PaymentDetailDialog from './components/PaymentDetailDialog.vue'

const { t } = useI18n()

const TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  receipt: { label: 'crm.payment.typeReceipt', type: 'success' },
  payment: { label: 'crm.payment.typePayment', type: 'warning' },
}

const METHOD_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  cash: { label: 'crm.payment.methodCash', type: 'info' },
  bank: { label: 'crm.payment.methodBank', type: '' },
  wechat: { label: 'crm.payment.methodWechat', type: 'success' },
}

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.payment.statusDraft', type: 'info' },
  confirmed: { label: 'crm.payment.statusConfirmed', type: 'success' },
  voided: { label: 'crm.payment.statusVoided', type: 'danger' },
}

const STATUS_LABEL_MAP: Record<PaymentStatus, string> = {
  draft: 'crm.payment.statusDraft',
  confirmed: 'crm.payment.statusConfirmed',
  voided: 'crm.payment.statusVoided',
}

const detailVisible = ref(false)
const selectedPayment = ref<Payment | null>(null)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'type',
    label: 'crm.payment.searchType',
    type: 'select',
    options: [
      { label: 'crm.payment.typeReceipt', value: 'receipt' },
      { label: 'crm.payment.typePayment', value: 'payment' },
    ],
  },
  {
    prop: 'method',
    label: 'crm.payment.searchMethod',
    type: 'select',
    options: [
      { label: 'crm.payment.methodCash', value: 'cash' },
      { label: 'crm.payment.methodBank', value: 'bank' },
      { label: 'crm.payment.methodWechat', value: 'wechat' },
    ],
  },
  {
    prop: 'status',
    label: 'crm.payment.searchStatus',
    type: 'select',
    options: [
      { label: 'crm.payment.statusDraft', value: 'draft' },
      { label: 'crm.payment.statusConfirmed', value: 'confirmed' },
      { label: 'crm.payment.statusVoided', value: 'voided' },
    ],
  },
  { prop: 'keyword', label: 'crm.payment.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<Payment>(getPaymentList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { type: '', method: '', status: '', keyword: '' },
})

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const formatMoney = (val: number) => {
  return val.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const openDetail = (row: Payment) => {
  selectedPayment.value = row
  detailVisible.value = true
}

const handleStatusChange = async (row: Payment, newStatus: PaymentStatus) => {
  try {
    const msg = t('crm.payment.confirmMessage', { status: t(STATUS_LABEL_MAP[newStatus]) })
    await ElMessageBox.confirm(msg, t('common.tips'), { type: 'warning' })
    await updatePaymentStatus(row.id, { status: newStatus })
    ElMessage.success(t('crm.payment.message.statusUpdateSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}

const handleDelete = async (row: Payment) => {
  try {
    await ElMessageBox.confirm(t('crm.payment.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deletePayment(row.id)
    ElMessage.success(t('crm.payment.message.deleteSuccess'))
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
