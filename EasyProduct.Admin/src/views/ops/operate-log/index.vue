<template>
  <div class="operate-log-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="operate-log-page__table">
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
          :label="t('ops.operateLog.userName')"
          width="100"
        />
        <el-table-column
          prop="module"
          :label="t('ops.operateLog.module')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.module"
              :options="MODULE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="action"
          :label="t('ops.operateLog.action')"
          width="100"
        />
        <el-table-column
          prop="method"
          :label="t('ops.operateLog.method')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              :type="row.method === 'GET' ? 'info' : row.method === 'POST' ? 'success' : row.method === 'PUT' ? 'warning' : 'danger'"
              size="small"
            >
              {{ row.method }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="url"
          :label="t('ops.operateLog.url')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="ip"
          :label="t('ops.operateLog.ip')"
          width="130"
        />
        <el-table-column
          prop="duration"
          :label="t('ops.operateLog.duration')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            {{ row.duration }}ms
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('ops.operateLog.status')"
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
          :label="t('ops.operateLog.createdAt')"
          width="170"
        />
        <el-table-column
          :label="t('common.actions')"
          width="80"
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
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <el-dialog
      :model-value="detailVisible"
      :title="t('ops.operateLog.detail')"
      width="700px"
      append-to-body
      @update:model-value="detailVisible = $event"
    >
      <template v-if="selectedLog">
        <el-descriptions
          :column="2"
          border
        >
          <el-descriptions-item :label="t('ops.operateLog.userName')">
            {{ selectedLog.userName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.operateLog.module')">
            {{ selectedLog.module }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.operateLog.action')">
            {{ selectedLog.action }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.operateLog.method')">
            {{ selectedLog.method }}
          </el-descriptions-item>
          <el-descriptions-item
            :label="t('ops.operateLog.url')"
            :span="2"
          >
            {{ selectedLog.url }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.operateLog.ip')">
            {{ selectedLog.ip }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.operateLog.userAgent')">
            {{ selectedLog.userAgent }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.operateLog.duration')">
            {{ selectedLog.duration }}ms
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.operateLog.status')">
            <BaseStatusTag
              :value="selectedLog.status"
              :options="STATUS_MAP"
            />
          </el-descriptions-item>
          <el-descriptions-item
            :label="t('ops.operateLog.params')"
            :span="2"
          >
            {{ selectedLog.params }}
          </el-descriptions-item>
          <el-descriptions-item
            v-if="selectedLog.errorMessage"
            :label="t('ops.operateLog.errorMessage')"
            :span="2"
          >
            {{ selectedLog.errorMessage }}
          </el-descriptions-item>
          <el-descriptions-item
            :label="t('ops.operateLog.createdAt')"
            :span="2"
          >
            {{ selectedLog.createdAt }}
          </el-descriptions-item>
        </el-descriptions>
      </template>
      <template #footer>
        <el-button @click="detailVisible = false">
          {{ t('common.close') }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { getOperateLogList } from '@/api/ops/operate-log'
import type { OperateLog } from '@/types/ops'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const MODULE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  basic: { label: 'ops.logQuery.moduleBasic', type: '' },
  site: { label: 'ops.logQuery.moduleSite', type: 'info' },
  product: { label: 'ops.logQuery.moduleProduct', type: 'success' },
  mall: { label: 'ops.logQuery.moduleMall', type: 'warning' },
  crm: { label: 'ops.logQuery.moduleCrm', type: 'danger' },
  ops: { label: 'ops.logQuery.moduleOps', type: 'info' },
}

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  success: { label: 'ops.operateLog.statusSuccess', type: 'success' },
  fail: { label: 'ops.operateLog.statusFail', type: 'danger' },
}

const detailVisible = ref(false)
const selectedLog = ref<OperateLog | null>(null)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'module',
    label: 'ops.operateLog.searchModule',
    type: 'select',
    options: [
      { label: 'ops.logQuery.moduleBasic', value: 'basic' },
      { label: 'ops.logQuery.moduleSite', value: 'site' },
      { label: 'ops.logQuery.moduleProduct', value: 'product' },
      { label: 'ops.logQuery.moduleMall', value: 'mall' },
      { label: 'ops.logQuery.moduleCrm', value: 'crm' },
      { label: 'ops.logQuery.moduleOps', value: 'ops' },
    ],
  },
  {
    prop: 'status',
    label: 'ops.operateLog.searchStatus',
    type: 'select',
    options: [
      { label: 'ops.operateLog.statusSuccess', value: 'success' },
      { label: 'ops.operateLog.statusFail', value: 'fail' },
    ],
  },
  { prop: 'dateRange', label: 'ops.operateLog.searchDateRange', type: 'dateRange' },
  { prop: 'keyword', label: 'ops.operateLog.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<OperateLog>(getOperateLogList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { module: '', status: '', dateRange: [], keyword: '' },
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

const openDetail = (row: OperateLog) => {
  selectedLog.value = row
  detailVisible.value = true
}
</script>

<style scoped lang="scss">
.operate-log-page {
}
</style>
