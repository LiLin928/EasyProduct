<template>
  <div class="task-page">
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
          {{ t('ops.task.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="task-page__table">
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
          :label="t('ops.task.taskName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="taskGroup"
          :label="t('ops.task.taskGroup')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              size="small"
              type="info"
            >
              {{ row.taskGroup }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="cron"
          :label="t('ops.task.cron')"
          width="140"
        />
        <el-table-column
          prop="className"
          :label="t('ops.task.className')"
          min-width="240"
          show-overflow-tooltip
        />
        <el-table-column
          prop="description"
          :label="t('ops.task.description')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="status"
          :label="t('ops.task.status')"
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
          prop="lastRunTime"
          :label="t('ops.task.lastRunTime')"
          width="170"
        />
        <el-table-column
          prop="nextRunTime"
          :label="t('ops.task.nextRunTime')"
          width="170"
        />
        <el-table-column
          :label="t('common.actions')"
          width="200"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-if="row.status === 'running'"
              link
              type="warning"
              @click="handleToggleStatus(row, 'paused')"
            >
              {{ t('ops.task.actionPause') }}
            </el-button>
            <el-button
              v-if="row.status === 'paused'"
              link
              type="success"
              @click="handleToggleStatus(row, 'running')"
            >
              {{ t('ops.task.actionRun') }}
            </el-button>
            <el-button
              link
              type="primary"
              @click="openEdit(row)"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
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

    <TaskFormDialog
      :visible="formVisible"
      :is-edit="isEdit"
      :row-data="selectedTask"
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
import { getTaskList, deleteTask, updateTaskStatus } from '@/api/ops/task'
import type { Task, TaskStatus } from '@/types/ops'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import TaskFormDialog from './components/TaskFormDialog.vue'

const { t } = useI18n()

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  running: { label: 'ops.task.statusRunning', type: 'success' },
  paused: { label: 'ops.task.statusPaused', type: 'warning' },
}

const formVisible = ref(false)
const isEdit = ref(false)
const selectedTask = ref<Task | undefined>(undefined)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'status',
    label: 'ops.task.searchStatus',
    type: 'select',
    options: [
      { label: 'ops.task.statusRunning', value: 'running' },
      { label: 'ops.task.statusPaused', value: 'paused' },
    ],
  },
  { prop: 'keyword', label: 'ops.task.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<Task>(getTaskList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { status: '', keyword: '' },
})

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const openCreate = () => {
  isEdit.value = false
  selectedTask.value = undefined
  formVisible.value = true
}

const openEdit = (row: Task) => {
  isEdit.value = true
  selectedTask.value = row
  formVisible.value = true
}

const handleToggleStatus = async (row: Task, newStatus: TaskStatus) => {
  try {
    await updateTaskStatus(row.id, { status: newStatus })
    ElMessage.success(t('common.success'))
    await reload()
  } catch {
    // ignore
  }
}

const handleDelete = async (row: Task) => {
  try {
    await ElMessageBox.confirm(t('ops.task.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deleteTask(row.id)
    ElMessage.success(t('common.success'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>

<style scoped lang="scss">
.task-page {
}
</style>
