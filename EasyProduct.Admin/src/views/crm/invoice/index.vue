<template>
  <div class="invoice-page">
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
          {{ t('crm.invoice.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="invoice-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="invoiceNo"
          :label="t('crm.invoice.invoiceNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('crm.invoice.type')"
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
          prop="orderNo"
          :label="t('crm.invoice.orderNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="partyName"
          :label="t('crm.invoice.partyName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="amount"
          :label="t('crm.invoice.amount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="taxAmount"
          :label="t('crm.invoice.taxAmount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.taxAmount) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="total"
          :label="t('crm.invoice.total')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.total) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="issueDate"
          :label="t('crm.invoice.issueDate')"
          width="120"
        />
        <el-table-column
          prop="status"
          :label="t('crm.invoice.status')"
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
          :label="t('common.actions')"
          width="200"
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
              v-if="row.status === 'draft'"
              link
              type="success"
              @click="handleStatusChange(row, 'issued')"
            >
              {{ t('crm.invoice.actionIssue') }}
            </el-button>
            <el-button
              v-if="row.status === 'issued'"
              link
              type="warning"
              @click="handleStatusChange(row, 'voided')"
            >
              {{ t('crm.invoice.actionVoid') }}
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

    <InvoiceFormDialog
      :visible="formVisible"
      :invoice="selectedInvoice"
      @update:visible="formVisible = $event"
      @success="reload"
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
  getInvoiceList,
  deleteInvoice,
  updateInvoiceStatus,
} from '@/api/crm/invoice'
import type { Invoice, InvoiceStatus } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import InvoiceFormDialog from './components/InvoiceFormDialog.vue'

const { t } = useI18n()

const TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  output: { label: 'crm.invoice.typeOutput', type: 'success' },
  input: { label: 'crm.invoice.typeInput', type: 'warning' },
}

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.invoice.statusDraft', type: 'info' },
  issued: { label: 'crm.invoice.statusIssued', type: 'success' },
  voided: { label: 'crm.invoice.statusVoided', type: 'danger' },
}

const STATUS_LABEL_MAP: Record<InvoiceStatus, string> = {
  draft: 'crm.invoice.statusDraft',
  issued: 'crm.invoice.statusIssued',
  voided: 'crm.invoice.statusVoided',
}

const formVisible = ref(false)
const selectedInvoice = ref<Invoice | null>(null)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'type',
    label: 'crm.invoice.searchType',
    type: 'select',
    options: [
      { label: 'crm.invoice.typeOutput', value: 'output' },
      { label: 'crm.invoice.typeInput', value: 'input' },
    ],
  },
  {
    prop: 'status',
    label: 'crm.invoice.searchStatus',
    type: 'select',
    options: [
      { label: 'crm.invoice.statusDraft', value: 'draft' },
      { label: 'crm.invoice.statusIssued', value: 'issued' },
      { label: 'crm.invoice.statusVoided', value: 'voided' },
    ],
  },
  { prop: 'keyword', label: 'crm.invoice.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<Invoice>(getInvoiceList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { type: '', status: '', keyword: '' },
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

const openCreate = () => {
  selectedInvoice.value = null
  formVisible.value = true
}

const openEdit = (row: Invoice) => {
  selectedInvoice.value = row
  formVisible.value = true
}

const handleStatusChange = async (row: Invoice, newStatus: InvoiceStatus) => {
  try {
    const msg = t('crm.invoice.confirmMessage', { status: t(STATUS_LABEL_MAP[newStatus]) })
    await ElMessageBox.confirm(msg, t('common.tips'), { type: 'warning' })
    await updateInvoiceStatus(row.id, { status: newStatus })
    ElMessage.success(t('crm.invoice.message.statusUpdateSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}

const handleDelete = async (row: Invoice) => {
  try {
    await ElMessageBox.confirm(t('crm.invoice.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deleteInvoice(row.id)
    ElMessage.success(t('crm.invoice.message.deleteSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>

<style scoped lang="scss">
.invoice-page {
}
</style>
