<template>
  <div class="log-query-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="log-query-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="module"
          :label="t('ops.logQuery.module')"
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
          prop="level"
          :label="t('ops.logQuery.level')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.level"
              :options="LEVEL_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="message"
          :label="t('ops.logQuery.message')"
          min-width="300"
          show-overflow-tooltip
        />
        <el-table-column
          prop="userName"
          :label="t('ops.logQuery.userName')"
          width="100"
        />
        <el-table-column
          prop="ip"
          :label="t('ops.logQuery.ip')"
          width="130"
        />
        <el-table-column
          prop="createdAt"
          :label="t('ops.logQuery.createdAt')"
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
      :title="t('ops.logQuery.detail')"
      width="700px"
      append-to-body
      @update:model-value="detailVisible = $event"
    >
      <template v-if="selectedLog">
        <el-descriptions
          :column="2"
          border
        >
          <el-descriptions-item :label="t('ops.logQuery.module')">
            {{ selectedLog.module }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.logQuery.level')">
            <BaseStatusTag
              :value="selectedLog.level"
              :options="LEVEL_MAP"
            />
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.logQuery.userName')">
            {{ selectedLog.userName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('ops.logQuery.ip')">
            {{ selectedLog.ip }}
          </el-descriptions-item>
          <el-descriptions-item
            :label="t('ops.logQuery.message')"
            :span="2"
          >
            {{ selectedLog.message }}
          </el-descriptions-item>
          <el-descriptions-item
            v-if="selectedLog.stackTrace"
            :label="t('ops.logQuery.stackTrace')"
            :span="2"
          >
            <pre class="stack-trace">{{ selectedLog.stackTrace }}</pre>
          </el-descriptions-item>
          <el-descriptions-item
            :label="t('ops.logQuery.createdAt')"
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
import { getLogQueryList } from '@/api/ops/log-query'
import type { LogQuery } from '@/types/ops'
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

const LEVEL_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  debug: { label: 'ops.logQuery.levelDebug', type: 'info' },
  info: { label: 'ops.logQuery.levelInfo', type: 'success' },
  warn: { label: 'ops.logQuery.levelWarn', type: 'warning' },
  error: { label: 'ops.logQuery.levelError', type: 'danger' },
}

const detailVisible = ref(false)
const selectedLog = ref<LogQuery | null>(null)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'module',
    label: 'ops.logQuery.searchModule',
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
    prop: 'level',
    label: 'ops.logQuery.searchLevel',
    type: 'select',
    options: [
      { label: 'ops.logQuery.levelDebug', value: 'debug' },
      { label: 'ops.logQuery.levelInfo', value: 'info' },
      { label: 'ops.logQuery.levelWarn', value: 'warn' },
      { label: 'ops.logQuery.levelError', value: 'error' },
    ],
  },
  { prop: 'dateRange', label: 'ops.logQuery.searchDateRange', type: 'dateRange' },
  { prop: 'keyword', label: 'ops.logQuery.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange } = useTable<LogQuery>(getLogQueryList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { module: '', level: '', dateRange: [], keyword: '' },
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

const openDetail = (row: LogQuery) => {
  selectedLog.value = row
  detailVisible.value = true
}
</script>

<style scoped lang="scss">
.log-query-page {
}
.stack-trace {
  margin: 0;
  padding: 8px;
  background: var(--el-fill-color-light);
  border-radius: 4px;
  font-size: 12px;
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 200px;
  overflow-y: auto;
}
