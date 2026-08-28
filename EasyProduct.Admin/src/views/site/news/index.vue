<template>
  <div class="news-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    >
      <template #toolbar>
        <el-button
          type="primary"
          @click="handleAdd"
        >
          {{ t('site.news.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="news-page__table">
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
          :label="t('site.news.newsTitle')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="titleEn"
          :label="t('site.news.titleEn')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="status"
          :label="t('site.news.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-tag :type="row.status === 'published' ? 'success' : 'info'">
              {{ row.status === 'published' ? t('site.news.statusPublished') : t('site.news.statusDraft') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="isTop"
          :label="t('site.news.isTop')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              v-if="row.isTop"
              type="warning"
              size="small"
            >
              {{ t('common.button.confirm') }}
            </el-tag>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="viewCount"
          :label="t('site.news.viewCount')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="publishTime"
          :label="t('site.news.publishTime')"
          width="160"
        >
          <template #default="{ row }">
            {{ row.publishTime || '-' }}
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('site.news.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="240"
          fixed="right"
        >
          <template #default="{ row }">
            <template v-if="row.status === 'draft'">
              <el-button
                link
                type="primary"
                @click="handleEdit(row)"
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
              <el-button
                link
                type="success"
                @click="handlePublish(row)"
              >
                {{ t('site.news.publish') }}
              </el-button>
            </template>
            <template v-else>
              <el-button
                link
                type="warning"
                @click="handleUnpublish(row)"
              >
                {{ t('site.news.unpublish') }}
              </el-button>
              <el-button
                link
                :type="row.isTop ? 'info' : 'success'"
                @click="handleSetTop(row)"
              >
                {{ row.isTop ? t('site.news.cancelTop') : t('site.news.top') }}
              </el-button>
            </template>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <NewsFormDialog
      :id="formDialog.payload.value?.id"
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { useDialog } from '@/composables/useDialog'
import { getNewsList, deleteNews, publishNews, unpublishNews, setTopNews } from '@/api/site/news'
import type { SiteNews } from '@/types/site'
import { NEWS_STATUS_OPTIONS } from '@/types/site'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import NewsFormDialog from './components/NewsFormDialog.vue'

const { t } = useI18n()

const searchFields: SearchField[] = [
  { prop: 'title', label: 'site.news.searchTitle', type: 'input' },
  { prop: 'status', label: 'site.news.searchStatus', type: 'select', options: NEWS_STATUS_OPTIONS },
  { prop: 'isTop', label: 'site.news.searchIsTop', type: 'select', options: [
    { label: 'common.button.confirm', value: 'true' },
  ] },
]

const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { title: '', status: '', isTop: '' },
})

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable(getNewsList, { immediate: true })

const handleSearch = (): void => {
  Object.assign(query, getSearchParams())
  tableSearch()
}

const handleReset = (): void => {
  resetModel()
  delete query.title
  delete query.status
  delete query.isTop
  tableReset()
}

const formDialog = useDialog<SiteNews>()

const handleAdd = (): void => {
  formDialog.open()
}

const handleEdit = (row: SiteNews): void => {
  formDialog.open(row)
}

const handleDelete = async (row: SiteNews): Promise<void> => {
  try {
    await ElMessageBox.confirm(t('site.news.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deleteNews(row.id)
    ElMessage.success(t('site.news.message.deleteSuccess'))
    reload()
  } catch {
    // cancelled or failed
  }
}

const handlePublish = async (row: SiteNews): Promise<void> => {
  try {
    await ElMessageBox.confirm(t('site.news.publish') + '?', t('common.tips'), { type: 'info' })
    await publishNews(row.id)
    ElMessage.success(t('site.news.message.publishSuccess'))
    reload()
  } catch {
    // cancelled or failed
  }
}

const handleUnpublish = async (row: SiteNews): Promise<void> => {
  try {
    await ElMessageBox.confirm(t('site.news.unpublish') + '?', t('common.tips'), { type: 'warning' })
    await unpublishNews(row.id)
    ElMessage.success(t('site.news.message.unpublishSuccess'))
    reload()
  } catch {
    // cancelled or failed
  }
}

const handleSetTop = async (row: SiteNews): Promise<void> => {
  try {
    const isTop = !row.isTop
    await setTopNews(row.id, { isTop })
    ElMessage.success(isTop ? t('site.news.message.topSuccess') : t('site.news.message.cancelTopSuccess'))
    reload()
  } catch {
    // failed
  }
}
</script>

<style scoped lang="scss">
.news-page {
}
</style>
