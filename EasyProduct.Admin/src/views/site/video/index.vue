<template>
  <div class="video-page">
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
          {{ t('site.video.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="video-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="coverImage"
          :label="t('site.video.coverImage')"
          width="100"
        >
          <template #default="{ row }">
            <el-image
              v-if="row.coverImage"
              :src="row.coverImage"
              :preview-src-list="[row.coverImage]"
              fit="cover"
              style="width: 60px; height: 40px; border-radius: 4px"
            />
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="title"
          :label="t('site.video.videoTitle')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="titleEn"
          :label="t('site.video.titleEn')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="videoUrl"
          :label="t('site.video.videoUrl')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="duration"
          :label="t('site.video.duration')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="viewCount"
          :label="t('site.video.viewCount')"
          width="90"
          align="center"
        />
        <el-table-column
          prop="sort"
          :label="t('site.video.sort')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="status"
          :label="t('site.video.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="VIDEO_STATUS_OPTIONS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('site.video.createdAt')"
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

    <VideoFormDialog
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
import { getVideoList, deleteVideo } from '@/api/site/video'
import type { SiteVideo } from '@/types/site'
import { VIDEO_STATUS_OPTIONS } from '@/types/site'
import type { SearchField } from '@/types/search'
import type { StatusOption } from '@/components/common/BaseStatusTag.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import VideoFormDialog from './components/VideoFormDialog.vue'

const { t } = useI18n()

const VIDEO_STATUS_OPTIONS_MAP: Record<string, StatusOption> = {
  draft: { label: 'site.video.statusDraft', type: 'info' },
  published: { label: 'site.video.statusPublished', type: 'success' },
}

const searchFields: SearchField[] = [
  { prop: 'title', label: 'site.video.searchTitle', type: 'input' },
  { prop: 'status', label: 'site.video.searchStatus', type: 'select', options: VIDEO_STATUS_OPTIONS },
]

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<SiteVideo>(getVideoList, {
  defaultSearchModel: { title: '', status: '' },
  deleteFn: deleteVideo,
  deleteConfirmText: t('site.video.deleteConfirm'),
  deleteSuccessText: t('site.video.message.deleteSuccess'),
})
</script>

<style scoped lang="scss">
.video-page {
}
</style>
