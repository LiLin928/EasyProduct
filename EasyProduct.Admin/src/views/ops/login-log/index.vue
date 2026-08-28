<template>
  <div class="login-log-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="login-log-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="userName"
          :label="t('ops.loginLog.userName')"
          width="100"
        />
        <el-table-column
          prop="ip"
          :label="t('ops.loginLog.ip')"
          width="130"
        />
        <el-table-column
          prop="location"
          :label="t('ops.loginLog.location')"
          width="140"
          show-overflow-tooltip
        />
        <el-table-column
          prop="browser"
          :label="t('ops.loginLog.browser')"
          width="140"
        />
        <el-table-column
          prop="os"
          :label="t('ops.loginLog.os')"
          width="120"
        />
        <el-table-column
          prop="status"
          :label="t('ops.loginLog.status')"
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
          prop="message"
          :label="t('ops.loginLog.message')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="createdAt"
          :label="t('ops.loginLog.createdAt')"
          width="170"
        />
      </BaseTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { getLoginLogList } from '@/api/ops/login-log'
import type { LoginLog } from '@/types/ops'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  success: { label: 'ops.loginLog.statusSuccess', type: 'success' },
  fail: { label: 'ops.loginLog.statusFail', type: 'danger' },
}

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'status',
    label: 'ops.loginLog.searchStatus',
    type: 'select',
    options: [
      { label: 'ops.loginLog.statusSuccess', value: 'success' },
      { label: 'ops.loginLog.statusFail', value: 'fail' },
    ],
  },
  { prop: 'dateRange', label: 'ops.loginLog.searchDateRange', type: 'dateRange' },
  { prop: 'keyword', label: 'ops.loginLog.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<LoginLog>(getLoginLogList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { status: '', dateRange: [], keyword: '' },
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
</script>

<style scoped lang="scss">
.login-log-page {
}
