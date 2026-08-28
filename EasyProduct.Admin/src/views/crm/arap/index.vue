<template>
  <div class="arap-page">
    <el-row
      :gutter="16"
      class="arap-page__summary"
    >
      <el-col :span="8">
        <el-card shadow="hover">
          <div class="summary-card">
            <div class="summary-card__label">
              {{ t('crm.arap.summaryReceivable') }}
            </div>
            <div class="summary-card__value text-success">
              {{ formatMoney(summary?.totalReceivable ?? 0) }}
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card shadow="hover">
          <div class="summary-card">
            <div class="summary-card__label">
              {{ t('crm.arap.summaryPayable') }}
            </div>
            <div class="summary-card__value text-danger">
              {{ formatMoney(summary?.totalPayable ?? 0) }}
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card shadow="hover">
          <div class="summary-card">
            <div class="summary-card__label">
              {{ t('crm.arap.summaryBalance') }}
            </div>
            <div
              class="summary-card__value"
              :class="(summary?.totalBalance ?? 0) >= 0 ? 'text-success' : 'text-danger'"
            >
              {{ formatMoney(summary?.totalBalance ?? 0) }}
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="arap-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="orderType"
          :label="t('crm.arap.orderType')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            {{ row.orderType === 'sales' ? t('crm.arap.orderTypeSales') : t('crm.arap.orderTypePurchase') }}
          </template>
        </el-table-column>
        <el-table-column
          prop="orderNo"
          :label="t('crm.arap.orderNo')"
          width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="partyName"
          :label="t('crm.arap.partyName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="receivable"
          :label="t('crm.arap.receivable')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.receivable) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="received"
          :label="t('crm.arap.received')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.received) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="balance"
          :label="t('crm.arap.balance')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            <span :class="row.balance > 0 ? 'text-danger' : 'text-success'">
              {{ formatMoney(row.balance) }}
            </span>
          </template>
        </el-table-column>
        <el-table-column
          prop="aging"
          :label="t('crm.arap.aging')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.aging"
              :options="AGING_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.arap.status')"
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
          :label="t('crm.arap.createdAt')"
          width="170"
        />
      </BaseTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { getArapList, getArapSummary } from '@/api/crm/arap'
import type { Arap, ArapSummary } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const AGING_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  '0-30': { label: 'crm.arap.aging0to30', type: 'success' },
  '31-60': { label: 'crm.arap.aging31to60', type: 'warning' },
  '61-90': { label: 'crm.arap.aging61to90', type: 'danger' },
  '90+': { label: 'crm.arap.aging90plus', type: 'danger' },
}

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  settled: { label: 'crm.arap.statusSettled', type: 'success' },
  unsettled: { label: 'crm.arap.statusUnsettled', type: 'warning' },
}

const summary = ref<ArapSummary | null>(null)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'orderType',
    label: 'crm.arap.searchOrderType',
    type: 'select',
    options: [
      { label: 'crm.arap.orderTypeSales', value: 'sales' },
      { label: 'crm.arap.orderTypePurchase', value: 'purchase' },
    ],
  },
  {
    prop: 'aging',
    label: 'crm.arap.searchAging',
    type: 'select',
    options: [
      { label: 'crm.arap.aging0to30', value: '0-30' },
      { label: 'crm.arap.aging31to60', value: '31-60' },
      { label: 'crm.arap.aging61to90', value: '61-90' },
      { label: 'crm.arap.aging90plus', value: '90+' },
    ],
  },
  {
    prop: 'status',
    label: 'crm.arap.searchStatus',
    type: 'select',
    options: [
      { label: 'crm.arap.statusSettled', value: 'settled' },
      { label: 'crm.arap.statusUnsettled', value: 'unsettled' },
    ],
  },
  { prop: 'keyword', label: 'crm.arap.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<Arap>(getArapList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { orderType: '', aging: '', status: '', keyword: '' },
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

const fetchSummary = async () => {
  try {
    summary.value = await getArapSummary()
  } catch {
    // ignore summary errors
  }
}

onMounted(() => {
  void fetchSummary()
})
</script>

<style scoped lang="scss">
.arap-page {
  &__summary {
    margin-bottom: 16px;
  }
}

.summary-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 8px 0;

  &__label {
    font-size: 14px;
    color: var(--el-text-color-secondary);
    margin-bottom: 8px;
  }

  &__value {
    font-size: 24px;
    font-weight: 700;
  }
}

.text-success {
  color: var(--el-color-success);
}

.text-danger {
  color: var(--el-color-danger);
}
