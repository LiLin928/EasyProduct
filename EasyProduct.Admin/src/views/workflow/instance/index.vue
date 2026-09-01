<template>
  <div class="instance-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="instance-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="title"
          :label="t('workflow.instance.title')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="definitionName"
          :label="t('workflow.instance.definitionName')"
          width="120"
        />
        <el-table-column
          prop="businessId"
          :label="t('workflow.instance.businessId')"
          width="140"
        />
        <el-table-column
          prop="applicantName"
          :label="t('workflow.instance.applicantName')"
          width="100"
        />
        <el-table-column
          prop="status"
          :label="t('workflow.instance.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="INSTANCE_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('workflow.instance.createdAt')"
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
              @click="openDetail(row)"
            >
              {{ t('common.view') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <InstanceDetailDialog
      :visible="detailDialog.visible.value"
      :instance-id="detailDialog.payload.value?.id ?? ''"
      @update:visible="detailDialog.visible.value = $event"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { useDialog } from '@/composables/useDialog'
import { getInstanceList } from '@/api/workflow/instance'
import type { WorkflowInstance } from '@/types/workflow'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import InstanceDetailDialog from './components/InstanceDetailDialog.vue'

const { t } = useI18n()

const INSTANCE_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  running: { label: 'workflow.instance.statusRunning', type: 'warning' },
  approved: { label: 'workflow.instance.statusApproved', type: 'success' },
  rejected: { label: 'workflow.instance.statusRejected', type: 'danger' },
  cancelled: { label: 'workflow.instance.statusCancelled', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'title', label: 'workflow.instance.searchTitle', type: 'input' },
  { prop: 'status', label: 'workflow.instance.searchStatus', type: 'select', options: [
    { label: 'workflow.instance.statusRunning', value: 'running' },
    { label: 'workflow.instance.statusApproved', value: 'approved' },
    { label: 'workflow.instance.statusRejected', value: 'rejected' },
    { label: 'workflow.instance.statusCancelled', value: 'cancelled' },
  ] },
  { prop: 'applicantName', label: 'workflow.instance.searchApplicant', type: 'input' },
  { prop: 'definitionName', label: 'workflow.instance.searchDefinition', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<WorkflowInstance>(getInstanceList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { title: '', status: '', applicantName: '', definitionName: '' },
})

const detailDialog = useDialog<WorkflowInstance>()

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const openDetail = (row: WorkflowInstance) => {
  detailDialog.open(row)
}
</script>

<style scoped lang="scss">
.instance-page {
}
</style>
