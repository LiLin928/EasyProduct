<template>
  <div class="task-log-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="task-log-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="taskName"
          :label="t('ops.taskLog.taskName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="startTime"
          :label="t('ops.taskLog.startTime')"
          width="170"
        />
        <el-table-column
          prop="endTime"
          :label="t('ops.taskLog.endTime')"
          width="170"
        />
        <el-table-column
          prop="duration"
          :label="t('ops.taskLog.duration')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            {{ row.duration }}ms
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('ops.taskLog.status')"
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
          prop="errorMessage"
          :label="t('ops.taskLog.errorMessage')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="createdAt"
          :label="t('ops.taskLog.createdAt')"
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
import { getTaskLogList } from '@/api/ops/task-log'
import type { TaskLog } from '@/types/ops'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  success: { label: 'ops.taskLog.statusSuccess', type: 'success' },
  fail: { label: 'ops.taskLog.statusFail', type: 'danger' },
  running: { label: 'ops.taskLog.statusRunning', type: 'warning' },
}

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'status',
    label: 'ops.taskLog.searchStatus',
    type: 'select',
    options: [
      { label: 'ops.taskLog.statusSuccess', value: 'success' },
      { label: 'ops.taskLog.statusFail', value: 'fail' },
      { label: 'ops.taskLog.statusRunning', value: 'running' },
    ],
  },
  { prop: 'dateRange', label: 'ops.taskLog.searchDateRange', type: 'dateRange' },
  { prop: 'keyword', label: 'ops.taskLog.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<TaskLog>(getTaskLogList)
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
.task-log-page {
}
</style>
