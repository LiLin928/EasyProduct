<template>
  <div class="todo-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="todo-page__table">
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
          prop="createdAt"
          :label="t('workflow.task.createdAt')"
          width="170"
        />
        <el-table-column
          :label="t('common.actions')"
          width="100"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openApprove(row)"
            >
              {{ t('workflow.task.approve') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <ApproveDialog
      :visible="approveDialog.visible.value"
      :task-id="approveDialog.payload.value?.id ?? ''"
      @update:visible="approveDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { useDialog } from '@/composables/useDialog'
import { getTodoList } from '@/api/workflow/todo'
import type { WorkflowTask } from '@/types/workflow'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import ApproveDialog from './components/ApproveDialog.vue'

const { t } = useI18n()

const searchFields = computed<SearchField[]>(() => [
  { prop: 'instanceTitle', label: 'workflow.task.searchInstanceTitle', type: 'input' },
  { prop: 'definitionName', label: 'workflow.task.searchDefinition', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<WorkflowTask>(getTodoList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { instanceTitle: '', definitionName: '' },
})

const approveDialog = useDialog<WorkflowTask>()

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const openApprove = (row: WorkflowTask) => {
  approveDialog.open(row)
}
</script>

<style scoped lang="scss">
.todo-page {
}
</style>
