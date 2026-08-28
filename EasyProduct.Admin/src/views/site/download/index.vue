<template>
  <div class="download-page">
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
          {{ t('site.download.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="download-page__table">
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
          :label="t('site.download.downloadTitle')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="titleEn"
          :label="t('site.download.titleEn')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="fileUrl"
          :label="t('site.download.fileUrl')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="fileSize"
          :label="t('site.download.fileSize')"
          width="120"
          align="center"
        />
        <el-table-column
          prop="downloadCount"
          :label="t('site.download.downloadCount')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="sort"
          :label="t('site.download.sort')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="status"
          :label="t('site.download.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="DOWNLOAD_STATUS_OPTIONS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('site.download.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="150"
          fixed="right"
        >
          <template #default="{ row }">
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

    <DownloadFormDialog
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      :row-data="formDialog.payload.value"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useCrud } from '@/composables/useCrud'
import { getDownloadList, deleteDownload } from '@/api/site/download'
import type { SiteDownload } from '@/types/site'
import { DOWNLOAD_STATUS_OPTIONS } from '@/types/site'
import type { SearchField } from '@/types/search'
import type { StatusOption } from '@/components/common/BaseStatusTag.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import DownloadFormDialog from './components/DownloadFormDialog.vue'

const { t } = useI18n()

const DOWNLOAD_STATUS_OPTIONS_MAP: Record<string, StatusOption> = {
  draft: { label: 'site.download.statusDraft', type: 'info' },
  published: { label: 'site.download.statusPublished', type: 'success' },
}

const searchFields: SearchField[] = [
  { prop: 'title', label: 'site.download.searchTitle', type: 'input' },
  { prop: 'status', label: 'site.download.searchStatus', type: 'select', options: DOWNLOAD_STATUS_OPTIONS },
]

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<SiteDownload>(getDownloadList, {
  defaultSearchModel: { title: '', status: '' },
  deleteFn: deleteDownload,
  deleteConfirmText: t('site.download.deleteConfirm'),
  deleteSuccessText: t('site.download.message.deleteSuccess'),
})
</script>

<style scoped lang="scss">
.download-page {
}
</style>
