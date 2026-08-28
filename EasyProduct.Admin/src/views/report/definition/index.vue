<template>
  <div class="definition-page">
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
          {{ t('report.definition.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="definition-page__table">
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
          :label="t('report.definition.name')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="code"
          :label="t('report.definition.code')"
          width="180"
        />
        <el-table-column
          prop="datasourceName"
          :label="t('report.definition.datasource')"
          width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="chartType"
          :label="t('report.definition.chartType')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.chartType"
              :options="CHART_TYPE_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('report.definition.status')"
          width="120"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="REPORT_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('report.definition.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="220"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="success"
              @click="handlePreview(row)"
            >
              {{ t('report.definition.preview') }}
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

    <DefinitionFormDialog
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      :row-data="formDialog.payload.value"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />

    <el-dialog
      v-model="previewVisible"
      :title="t('report.definition.previewTitle')"
      width="900px"
      append-to-body
    >
      <el-table
        v-if="previewData"
        :data="previewData.rows"
        border
        max-height="400"
      >
        <el-table-column
          v-for="col in previewData.columns"
          :key="col.field"
          :prop="col.field"
          :label="col.label"
          min-width="120"
        />
      </el-table>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { useCrud } from '@/composables/useCrud'
import { getDefinitionList, deleteDefinition, previewDefinition } from '@/api/report/definition'
import type { ReportDefinition } from '@/types/report'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import DefinitionFormDialog from './components/DefinitionFormDialog.vue'

const { t } = useI18n()

const CHART_TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  table: { label: 'report.definition.chartTable', type: 'info' },
  line: { label: 'report.definition.chartLine', type: 'success' },
  bar: { label: 'report.definition.chartBar', type: 'warning' },
  pie: { label: 'report.definition.chartPie', type: 'danger' },
}

const REPORT_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'report.definition.statusDraft', type: 'info' },
  published: { label: 'report.definition.statusPublished', type: 'success' },
  archived: { label: 'report.definition.statusArchived', type: 'warning' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'name', label: 'report.definition.searchName', type: 'input' },
  { prop: 'code', label: 'report.definition.searchCode', type: 'input' },
  { prop: 'status', label: 'report.definition.searchStatus', type: 'select', options: [
    { label: 'report.definition.statusDraft', value: 'draft' },
    { label: 'report.definition.statusPublished', value: 'published' },
    { label: 'report.definition.statusArchived', value: 'archived' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<ReportDefinition>(getDefinitionList, {
  defaultSearchModel: { name: '', code: '', status: '' },
  deleteFn: deleteDefinition,
  deleteConfirmText: t('report.definition.deleteConfirm'),
})

const previewVisible = ref(false)
const previewData = ref<{ columns: Array<{ field: string; label: string }>; rows: Record<string, unknown>[] } | null>(null)

const handlePreview = async (row: ReportDefinition) => {
  try {
    const data = await previewDefinition(row.id)
    previewData.value = data
    previewVisible.value = true
  } catch {
    ElMessage.error(t('common.error'))
  }
}
</script>

<style scoped lang="scss">
.definition-page {
}
</style>
