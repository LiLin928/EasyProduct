<template>
  <div class="announcement-page">
    <!-- 搜索表单 -->
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    >
      <template #toolbar>
        <el-button
          v-permission="['basic:announcement:edit']"
          type="primary"
          @click="handleAdd"
        >
          {{ t('common.add') }}
        </el-button>
        <el-button
          v-permission="['basic:announcement:delete']"
          type="danger"
          :disabled="selection.length === 0"
          @click="handleBatchDelete"
        >
          {{ t('common.batchDelete') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <!-- 列表 -->
    <el-card class="announcement-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
        @selection-change="handleSelectionChange"
      >
        <el-table-column
          type="selection"
          width="50"
        />
        <el-table-column
          prop="title"
          :label="t('basic.announcement.announcementTitle')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('basic.announcement.type')"
          width="120"
        >
          <template #default="{ row }">
            <el-tag :type="row.type === 'all' ? 'primary' : 'success'">
              {{ row.type === 'all' ? t('basic.announcement.typeAll') : t('basic.announcement.typeTargeted') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="level"
          :label="t('basic.announcement.level')"
          width="100"
        >
          <template #default="{ row }">
            <el-tag :type="getLevelType(row.level)">
              {{ getLevelLabel(row.level) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('basic.announcement.status')"
          width="100"
        >
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusLabel(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="isTop"
          :label="t('basic.announcement.isTop')"
          width="100"
        >
          <template #default="{ row }">
            <el-tag
              v-if="row.isTop"
              type="warning"
            >
              {{ t('common.yes') }}
            </el-tag>
            <span v-else>{{ t('common.no') }}</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="publishTime"
          :label="t('basic.announcement.publishTime')"
          width="160"
        >
          <template #default="{ row }">
            {{ row.publishTime || '-' }}
          </template>
        </el-table-column>
        <el-table-column
          prop="creatorName"
          :label="t('basic.announcement.creator')"
          width="120"
        />
        <el-table-column
          prop="createdAt"
          :label="t('basic.announcement.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="260"
          fixed="right"
        >
          <template #default="{ row }">
            <!-- 草稿状态 -->
            <template v-if="row.status === 'draft'">
              <el-button
                v-permission="['basic:announcement:edit']"
                link
                type="primary"
                @click="handleEdit(row)"
              >
                {{ t('common.edit') }}
              </el-button>
              <el-button
                v-permission="['basic:announcement:delete']"
                link
                type="danger"
                @click="handleDelete(row)"
              >
                {{ t('common.delete') }}
              </el-button>
              <el-button
                v-permission="['basic:announcement:edit']"
                link
                type="success"
                @click="handlePublish(row)"
              >
                {{ t('basic.announcement.publish') }}
              </el-button>
            </template>

            <!-- 已发布状态 -->
            <template v-else-if="row.status === 'published'">
              <el-button
                link
                type="primary"
                @click="handleView(row)"
              >
                {{ t('common.view') }}
              </el-button>
              <el-button
                v-permission="['basic:announcement:edit']"
                link
                type="warning"
                @click="handleRecall(row)"
              >
                {{ t('basic.announcement.recall') }}
              </el-button>
              <el-button
                v-permission="['basic:announcement:edit']"
                link
                :type="row.isTop ? 'info' : 'success'"
                @click="handleSetTop(row)"
              >
                {{ row.isTop ? t('basic.announcement.cancelTop') : t('basic.announcement.setTop') }}
              </el-button>
            </template>

            <!-- 已撤回状态 -->
            <template v-else-if="row.status === 'recalled'">
              <el-button
                link
                type="primary"
                @click="handleView(row)"
              >
                {{ t('common.view') }}
              </el-button>
              <el-button
                v-permission="['basic:announcement:delete']"
                link
                type="danger"
                @click="handleDelete(row)"
              >
                {{ t('common.delete') }}
              </el-button>
              <el-button
                v-permission="['basic:announcement:edit']"
                link
                type="success"
                @click="handlePublish(row)"
              >
                {{ t('basic.announcement.republish') }}
              </el-button>
            </template>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <!-- 详情弹窗 -->
    <DetailDialog
      :id="detailDialog.payload.value?.id"
      :visible="detailDialog.visible.value"
      @update:visible="detailDialog.visible.value = $event"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useDialog } from '@/composables/useDialog'
import { useSearch } from '@/composables/useSearch'
import {
  getAnnouncementList,
  deleteAnnouncement,
  publishAnnouncement,
  recallAnnouncement,
  setTopAnnouncement
} from '@/api/basic/announcement'
import type { Announcement, AnnouncementStatus, AnnouncementLevel } from '@/types/announcement'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import DetailDialog from './components/DetailDialog.vue'

const { t } = useI18n()

// 搜索字段配置
const searchFields: SearchField[] = [
  {
    prop: 'title',
    label: 'basic.announcement.announcementTitle',
    type: 'input'
  },
  {
    prop: 'type',
    label: 'basic.announcement.type',
    type: 'select',
    options: [
      { label: 'basic.announcement.typeAll', value: 'all' },
      { label: 'basic.announcement.typeTargeted', value: 'targeted' }
    ]
  },
  {
    prop: 'level',
    label: 'basic.announcement.level',
    type: 'select',
    options: [
      { label: 'basic.announcement.levelNormal', value: 'normal' },
      { label: 'basic.announcement.levelImportant', value: 'important' },
      { label: 'basic.announcement.levelUrgent', value: 'urgent' }
    ]
  },
  {
    prop: 'status',
    label: 'basic.announcement.status',
    type: 'select',
    options: [
      { label: 'basic.announcement.statusDraft', value: 'draft' },
      { label: 'basic.announcement.statusPublished', value: 'published' },
      { label: 'basic.announcement.statusRecalled', value: 'recalled' }
    ]
  }
]

// 搜索逻辑
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: {
    title: '',
    type: '',
    level: '',
    status: ''
  }
})

// 列表状态
const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable(
  getAnnouncementList,
  { immediate: true }
)

// 搜索（合并搜索参数到查询）
const handleSearch = (): void => {
  Object.assign(query, getSearchParams())
  tableSearch()
}

// 重置（清空搜索参数并重置）
const handleReset = (): void => {
  resetModel()
  delete query.title
  delete query.type
  delete query.level
  delete query.status
  tableReset()
}

// 多选
const selection = ref<Announcement[]>([])

const handleSelectionChange = (val: unknown[]): void => {
  selection.value = val as Announcement[]
}

// 详情弹窗
const detailDialog = useDialog<Announcement>()

// 新增公告
const handleAdd = (): void => {
  // TODO: 跳转到新增页面或打开新增弹窗
  ElMessage.info(t('common.comingSoon'))
}

// 编辑公告
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const handleEdit = (row: Announcement): void => {
  // TODO: 跳转到编辑页面或打开编辑弹窗，使用 row 参数
  ElMessage.info(t('common.comingSoon'))
}

// 查看公告
const handleView = (row: Announcement): void => {
  detailDialog.open(row)
}

// 删除公告
const handleDelete = async (row: Announcement): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.deleteConfirm'),
      t('common.tips'),
      { type: 'warning' }
    )
    await deleteAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.deleteSuccess'))
    reload()
  } catch (error) {
    // 用户取消或请求失败
  }
}

// 批量删除
const handleBatchDelete = async (): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('common.batchDeleteConfirm', { count: selection.value.length }),
      t('common.tips'),
      { type: 'warning' }
    )
    await Promise.all(selection.value.map(item => deleteAnnouncement(item.id)))
    ElMessage.success(t('common.deleteSuccess'))
    reload()
  } catch (error) {
    // 用户取消或请求失败
  }
}

// 发布公告
const handlePublish = async (row: Announcement): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.publish') + '?',
      t('common.tips'),
      { type: 'info' }
    )
    await publishAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.publishSuccess'))
    reload()
  } catch (error) {
    // 用户取消或请求失败
  }
}

// 撤回公告
const handleRecall = async (row: Announcement): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('basic.announcement.recall') + '?',
      t('common.tips'),
      { type: 'warning' }
    )
    await recallAnnouncement(row.id)
    ElMessage.success(t('basic.announcement.message.recallSuccess'))
    reload()
  } catch (error) {
    // 用户取消或请求失败
  }
}

// 置顶/取消置顶
const handleSetTop = async (row: Announcement): Promise<void> => {
  try {
    const isTop = !row.isTop
    await setTopAnnouncement(row.id, { isTop })
    ElMessage.success(
      isTop
        ? t('basic.announcement.message.setTopSuccess')
        : t('basic.announcement.message.cancelTopSuccess')
    )
    reload()
  } catch (error) {
    // 请求失败
  }
}

// 获取级别标签类型
const getLevelType = (level: AnnouncementLevel): 'info' | 'warning' | 'danger' => {
  const map: Record<AnnouncementLevel, 'info' | 'warning' | 'danger'> = {
    normal: 'info',
    important: 'warning',
    urgent: 'danger'
  }
  return map[level] || 'info'
}

// 获取级别标签文本
const getLevelLabel = (level: AnnouncementLevel): string => {
  const map: Record<AnnouncementLevel, string> = {
    normal: t('basic.announcement.levelNormal'),
    important: t('basic.announcement.levelImportant'),
    urgent: t('basic.announcement.levelUrgent')
  }
  return map[level] || level
}

// 获取状态标签类型
const getStatusType = (status: AnnouncementStatus): 'info' | 'success' | 'info' => {
  const map: Record<AnnouncementStatus, 'info' | 'success' | 'info'> = {
    draft: 'info',
    published: 'success',
    recalled: 'info'
  }
  return map[status] || 'info'
}

// 获取状态标签文本
const getStatusLabel = (status: AnnouncementStatus): string => {
  const map: Record<AnnouncementStatus, string> = {
    draft: t('basic.announcement.statusDraft'),
    published: t('basic.announcement.statusPublished'),
    recalled: t('basic.announcement.statusRecalled')
  }
  return map[status] || status
}
</script>

<style scoped lang="scss">
.announcement-page {
}
</style>