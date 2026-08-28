<template>
  <div class="banner-page">
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
          {{ t('site.banner.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="banner-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="imageUrl"
          :label="t('site.banner.imageUrl')"
          width="120"
        >
          <template #default="{ row }">
            <el-image
              v-if="row.imageUrl"
              :src="row.imageUrl"
              :preview-src-list="[row.imageUrl]"
              fit="cover"
              style="width: 80px; height: 50px; border-radius: 4px"
            />
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="title"
          :label="t('site.banner.bannerTitle')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="titleEn"
          :label="t('site.banner.titleEn')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="link"
          :label="t('site.banner.link')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="sort"
          :label="t('site.banner.sort')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="status"
          :label="t('site.banner.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="BANNER_STATUS_OPTIONS"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('site.banner.createdAt')"
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

    <BannerFormDialog
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
import { getBannerList, deleteBanner } from '@/api/site/banner'
import type { SiteBanner } from '@/types/site'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import BannerFormDialog from './components/BannerFormDialog.vue'

const { t } = useI18n()

const BANNER_STATUS_OPTIONS: Record<string, { label: string; type: 'success' | 'danger' }> = {
  enabled: { label: 'site.banner.statusEnabled', type: 'success' },
  disabled: { label: 'site.banner.statusDisabled', type: 'danger' },
}

const searchFields: SearchField[] = [
  { prop: 'title', label: 'site.banner.searchTitle', type: 'input' },
  { prop: 'status', label: 'site.banner.searchStatus', type: 'select', options: [
    { label: 'site.banner.statusEnabled', value: 'enabled' },
    { label: 'site.banner.statusDisabled', value: 'disabled' },
  ] },
]

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<SiteBanner>(getBannerList, {
  defaultSearchModel: { title: '', status: '' },
  deleteFn: deleteBanner,
  deleteConfirmText: t('site.banner.deleteConfirm'),
  deleteSuccessText: t('site.banner.message.deleteSuccess'),
})
</script>

<style scoped lang="scss">
.banner-page {
}
</style>
