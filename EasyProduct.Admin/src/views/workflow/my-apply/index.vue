<template>
  <div class="my-apply-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    >
      <template #toolbar>
        <el-button
          type="primary"
          @click="openApply"
        >
          {{ t('workflow.myApply.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="my-apply-page__table">
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
          prop="businessType"
          :label="t('workflow.instance.businessType')"
          width="120"
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
          prop="currentNode"
          :label="t('workflow.instance.currentNode')"
          width="120"
        />
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
              v-if="row.status === 'running'"
              link
              type="danger"
              @click="handleCancel(row)"
            >
              {{ t('workflow.myApply.cancel') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <ApplyFormDialog
      :visible="formDialog.visible.value"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { useDialog } from '@/composables/useDialog'
import { getMyApplyList, cancelApply } from '@/api/workflow/my-apply'
import type { WorkflowInstance } from '@/types/workflow'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import ApplyFormDialog from './components/ApplyFormDialog.vue'

const { t } = useI18n()

const INSTANCE_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  running: { label: 'workflow.instance.statusRunning', type: 'warning' },
  approved: { label: 'workflow.instance.statusApproved', type: 'success' },
  rejected: { label: 'workflow.instance.statusRejected', type: 'danger' },
  cancelled: { label: 'workflow.instance.statusCancelled', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'title', label: 'workflow.myApply.searchTitle', type: 'input' },
  { prop: 'status', label: 'workflow.myApply.searchStatus', type: 'select', options: [
    { label: 'workflow.instance.statusRunning', value: 'running' },
    { label: 'workflow.instance.statusApproved', value: 'approved' },
    { label: 'workflow.instance.statusRejected', value: 'rejected' },
    { label: 'workflow.instance.statusCancelled', value: 'cancelled' },
  ] },
  { prop: 'businessType', label: 'workflow.myApply.searchBusinessType', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<WorkflowInstance>(getMyApplyList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { title: '', status: '', businessType: '' },
})

const formDialog = useDialog()

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const openApply = () => {
  formDialog.open()
}

const handleCancel = async (row: WorkflowInstance) => {
  try {
    await ElMessageBox.confirm(t('workflow.myApply.cancelConfirm'), t('common.tips'), { type: 'warning' })
  } catch {
    return
  }
  await cancelApply(row.id)
  ElMessage.success(t('workflow.myApply.cancelSuccess'))
  reload()
}
</script>

<style scoped lang="scss">
.my-apply-page {
}
</style>
