<template>
  <div class="datasource-page">
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
          {{ t('report.datasource.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="datasource-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="name"
          :label="t('report.datasource.name')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('report.datasource.type')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.type"
              :options="DATASOURCE_TYPE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          :label="t('report.datasource.hostPort')"
          min-width="180"
        >
          <template #default="{ row }">
            {{ row.host }}:{{ row.port }}
          </template>
        </el-table-column>
        <el-table-column
          prop="database"
          :label="t('report.datasource.database')"
          width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="username"
          :label="t('report.datasource.username')"
          width="120"
        />
        <el-table-column
          prop="status"
          :label="t('report.datasource.status')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="DATASOURCE_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('report.datasource.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="200"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="success"
              @click="handleTestConnection(row)"
            >
              {{ t('report.datasource.testConnection') }}
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

    <DatasourceFormDialog
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      :row-data="formDialog.payload.value"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { useCrud } from '@/composables/useCrud'
import { getDatasourceList, deleteDatasource, testConnection } from '@/api/report/datasource'
import type { Datasource } from '@/types/report'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import DatasourceFormDialog from './components/DatasourceFormDialog.vue'

const { t } = useI18n()

const DATASOURCE_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  mysql: { label: 'report.datasource.typeMysql', type: 'success' },
  postgresql: { label: 'report.datasource.typePostgresql', type: 'info' },
  sqlserver: { label: 'report.datasource.typeSqlserver', type: 'warning' },
  oracle: { label: 'report.datasource.typeOracle', type: 'danger' },
}

const DATASOURCE_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  connected: { label: 'report.datasource.statusConnected', type: 'success' },
  disconnected: { label: 'report.datasource.statusDisconnected', type: 'info' },
  error: { label: 'report.datasource.statusError', type: 'danger' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'name', label: 'report.datasource.searchName', type: 'input' },
  { prop: 'type', label: 'report.datasource.searchType', type: 'select', options: [
    { label: 'report.datasource.typeMysql', value: 'mysql' },
    { label: 'report.datasource.typePostgresql', value: 'postgresql' },
    { label: 'report.datasource.typeSqlserver', value: 'sqlserver' },
    { label: 'report.datasource.typeOracle', value: 'oracle' },
  ] },
  { prop: 'status', label: 'report.datasource.searchStatus', type: 'select', options: [
    { label: 'report.datasource.statusConnected', value: 'connected' },
    { label: 'report.datasource.statusDisconnected', value: 'disconnected' },
    { label: 'report.datasource.statusError', value: 'error' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<Datasource>(getDatasourceList, {
  defaultSearchModel: { name: '', type: '', status: '' },
  deleteFn: deleteDatasource,
  deleteConfirmText: t('report.datasource.deleteConfirm'),
})

const handleTestConnection = async (row: Datasource) => {
  try {
    const res = await testConnection(row.id)
    if (res.success) {
      ElMessage.success(res.message)
      reload()
    } else {
      ElMessage.error(res.message)
    }
  } catch {
    ElMessage.error(t('common.error'))
  }
}
</script>

<style scoped lang="scss">
.datasource-page {
}
</style>
