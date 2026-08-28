<template>
  <div class="reversal-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="reversal-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="reversalNo"
          :label="t('crm.reversal.reversalNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('crm.reversal.type')"
          width="130"
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
          prop="sourceOrderNo"
          :label="t('crm.reversal.sourceOrderNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="partyName"
          :label="t('crm.reversal.partyName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="amount"
          :label="t('crm.reversal.amount')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="operator"
          :label="t('crm.reversal.operator')"
          width="100"
        />
        <el-table-column
          prop="status"
          :label="t('crm.reversal.status')"
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
          :label="t('crm.reversal.createdAt')"
          width="170"
        />
        <el-table-column
          :label="t('common.actions')"
          width="260"
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
              @click="handleStatusChange(row, 'submitted')"
            >
              {{ t('crm.reversal.actionSubmit') }}
            </el-button>
            <el-button
              v-if="row.status === 'submitted'"
              link
              type="success"
              @click="handleStatusChange(row, 'approved')"
            >
              {{ t('crm.reversal.actionApprove') }}
            </el-button>
            <el-button
              v-if="row.status === 'submitted'"
              link
              type="danger"
              @click="handleStatusChange(row, 'rejected')"
            >
              {{ t('crm.reversal.actionReject') }}
            </el-button>
            <el-button
              v-if="row.status === 'approved'"
              link
              type="warning"
              @click="handleStatusChange(row, 'executed')"
            >
              {{ t('crm.reversal.actionExecute') }}
            </el-button>
            <el-button
              v-if="row.status === 'rejected'"
              link
              type="success"
              @click="handleStatusChange(row, 'submitted')"
            >
              {{ t('crm.reversal.actionResubmit') }}
            </el-button>
            <el-button
              v-if="row.status === 'draft' || row.status === 'rejected'"
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

    <ReversalDetailDialog
      :visible="detailVisible"
      :reversal="selectedReversal"
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
  getReversalList,
  deleteReversal,
  updateReversalStatus,
} from '@/api/crm/reversal'
import type { Reversal, ReversalStatus } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import ReversalDetailDialog from './components/ReversalDetailDialog.vue'

const { t } = useI18n()

const TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  mall_refund: { label: 'crm.reversal.typeMallRefund', type: 'warning' },
  sales_return: { label: 'crm.reversal.typeSalesReturn', type: 'danger' },
  purchase_return: { label: 'crm.reversal.typePurchaseReturn', type: 'info' },
  document_void: { label: 'crm.reversal.typeDocumentVoid', type: '' },
}

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.reversal.statusDraft', type: 'info' },
  submitted: { label: 'crm.reversal.statusSubmitted', type: 'warning' },
  approved: { label: 'crm.reversal.statusApproved', type: 'success' },
  rejected: { label: 'crm.reversal.statusRejected', type: 'danger' },
  executed: { label: 'crm.reversal.statusExecuted', type: '' },
}

const STATUS_LABEL_MAP: Record<ReversalStatus, string> = {
  draft: 'crm.reversal.statusDraft',
  submitted: 'crm.reversal.statusSubmitted',
  approved: 'crm.reversal.statusApproved',
  rejected: 'crm.reversal.statusRejected',
  executed: 'crm.reversal.statusExecuted',
}

const detailVisible = ref(false)
const selectedReversal = ref<Reversal | null>(null)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'type',
    label: 'crm.reversal.searchType',
    type: 'select',
    options: [
      { label: 'crm.reversal.typeMallRefund', value: 'mall_refund' },
      { label: 'crm.reversal.typeSalesReturn', value: 'sales_return' },
      { label: 'crm.reversal.typePurchaseReturn', value: 'purchase_return' },
      { label: 'crm.reversal.typeDocumentVoid', value: 'document_void' },
    ],
  },
  {
    prop: 'status',
    label: 'crm.reversal.searchStatus',
    type: 'select',
    options: [
      { label: 'crm.reversal.statusDraft', value: 'draft' },
      { label: 'crm.reversal.statusSubmitted', value: 'submitted' },
      { label: 'crm.reversal.statusApproved', value: 'approved' },
      { label: 'crm.reversal.statusRejected', value: 'rejected' },
      { label: 'crm.reversal.statusExecuted', value: 'executed' },
    ],
  },
  { prop: 'keyword', label: 'crm.reversal.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<Reversal>(getReversalList)
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

const openDetail = (row: Reversal) => {
  selectedReversal.value = row
  detailVisible.value = true
}

const handleStatusChange = async (row: Reversal, newStatus: ReversalStatus) => {
  try {
    const msg = t('crm.reversal.confirmMessage', { status: t(STATUS_LABEL_MAP[newStatus]) })
    await ElMessageBox.confirm(msg, t('common.tips'), { type: 'warning' })
    await updateReversalStatus(row.id, { status: newStatus })
    ElMessage.success(t('crm.reversal.message.statusUpdateSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}

const handleDelete = async (row: Reversal) => {
  try {
    await ElMessageBox.confirm(t('crm.reversal.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deleteReversal(row.id)
    ElMessage.success(t('crm.reversal.message.deleteSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>

<style scoped lang="scss">
.reversal-page {
}
