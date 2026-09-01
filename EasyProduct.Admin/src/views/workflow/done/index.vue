<template>
  <div class="done-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="done-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="instanceTitle"
          :label="t('workflow.task.instanceTitle')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="definitionName"
          :label="t('workflow.task.definitionName')"
          width="120"
        />
        <el-table-column
          prop="nodeName"
          :label="t('workflow.task.nodeName')"
          width="140"
        />
        <el-table-column
          prop="applicantName"
          :label="t('workflow.task.applicantName')"
          width="100"
        />
        <el-table-column
          prop="comment"
          :label="t('workflow.task.comment')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="status"
          :label="t('workflow.task.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="TASK_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="finishedAt"
          :label="t('workflow.task.finishedAt')"
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
import { getDoneList } from '@/api/workflow/done'
import type { WorkflowTask } from '@/types/workflow'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const TASK_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  approved: { label: 'workflow.task.statusApproved', type: 'success' },
  rejected: { label: 'workflow.task.statusRejected', type: 'danger' },
  transferred: { label: 'workflow.task.statusTransferred', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'instanceTitle', label: 'workflow.task.searchInstanceTitle', type: 'input' },
  { prop: 'status', label: 'workflow.task.searchStatus', type: 'select', options: [
    { label: 'workflow.task.statusApproved', value: 'approved' },
    { label: 'workflow.task.statusRejected', value: 'rejected' },
    { label: 'workflow.task.statusTransferred', value: 'transferred' },
  ] },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<WorkflowTask>(getDoneList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { instanceTitle: '', status: '' },
})

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}
</script>

<style scoped lang="scss">
.done-page {
}
</style>
