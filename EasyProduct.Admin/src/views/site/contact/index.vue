<template>
  <div class="contact-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="contact-page__table">
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
          :label="t('site.contact.name')"
          width="120"
        />
        <el-table-column
          prop="company"
          :label="t('site.contact.company')"
          min-width="140"
          show-overflow-tooltip
        />
        <el-table-column
          prop="phone"
          :label="t('site.contact.phone')"
          width="140"
        />
        <el-table-column
          prop="email"
          :label="t('site.contact.email')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="subject"
          :label="t('site.contact.subject')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="status"
          :label="t('site.contact.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="CONTACT_STATUS_OPTIONS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('site.contact.createdAt')"
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
              type="primary"
              @click="handleDetail(row)"
            >
              {{ t('common.view') }}
            </el-button>
            <el-button
              v-if="row.status === 'unread'"
              link
              type="warning"
              @click="handleUpdateStatus(row, 'read')"
            >
              {{ t('site.contact.markRead') }}
            </el-button>
            <el-button
              v-if="row.status !== 'archived'"
              link
              type="info"
              @click="handleUpdateStatus(row, 'archived')"
            >
              {{ t('site.contact.archive') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <!-- Detail Dialog -->
    <el-dialog
      v-model="detailVisible"
      :title="t('site.contact.detail')"
      width="600px"
      append-to-body
    >
      <el-descriptions
        v-if="detailData"
        :column="2"
        border
      >
        <el-descriptions-item :label="t('site.contact.name')">
          {{ detailData.name }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.contact.company')">
          {{ detailData.company }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.contact.phone')">
          {{ detailData.phone }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.contact.email')">
          {{ detailData.email }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.contact.status')">
          <BaseStatusTag
            :value="detailData.status"
            :options="CONTACT_STATUS_OPTIONS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.contact.createdAt')">
          {{ detailData.createdAt }}
        </el-descriptions-item>
        <el-descriptions-item
          :label="t('site.contact.subject')"
          :span="2"
        >
          {{ detailData.subject }}
        </el-descriptions-item>
        <el-descriptions-item
          :label="t('site.contact.message')"
          :span="2"
        >
          {{ detailData.message }}
        </el-descriptions-item>
      </el-descriptions>

      <template #footer>
        <el-button @click="detailVisible = false">
          {{ t('common.close') }}
        </el-button>
        <el-button
          v-if="detailData && detailData.status === 'unread'"
          type="warning"
          @click="handleUpdateStatus(detailData!, 'read')"
        >
          {{ t('site.contact.markRead') }}
        </el-button>
        <el-button
          v-if="detailData && detailData.status !== 'archived'"
          type="info"
          @click="handleUpdateStatus(detailData!, 'archived')"
        >
          {{ t('site.contact.archive') }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import { getContactList, getContactById, updateContactStatus } from '@/api/site/contact'
import type { ContactMessage, ContactStatus } from '@/types/site'
import { CONTACT_STATUS_OPTIONS } from '@/types/site'
import type { SearchField } from '@/types/search'
import type { StatusOption } from '@/components/common/BaseStatusTag.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const CONTACT_STATUS_OPTIONS_MAP: Record<string, StatusOption> = {
  unread: { label: 'site.contact.statusUnread', type: 'danger' },
  read: { label: 'site.contact.statusRead', type: 'success' },
  archived: { label: 'site.contact.statusArchived', type: 'info' },
}

const searchFields: SearchField[] = [
  { prop: 'name', label: 'site.contact.searchName', type: 'input' },
  { prop: 'status', label: 'site.contact.searchStatus', type: 'select', options: CONTACT_STATUS_OPTIONS },
]

const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { name: '', status: '' },
})

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable(getContactList, { immediate: true })

const handleSearch = (): void => {
  Object.assign(query, getSearchParams())
  tableSearch()
}

const handleReset = (): void => {
  resetModel()
  delete query.name
  delete query.status
  tableReset()
}

// Detail dialog
const detailVisible = ref(false)
const detailData = ref<ContactMessage | null>(null)

const handleDetail = async (row: ContactMessage): Promise<void> => {
  try {
    const data = await getContactById(row.id)
    detailData.value = data
    detailVisible.value = true
  } catch {
    // handled by interceptor
  }
}

const handleUpdateStatus = async (row: ContactMessage, status: ContactStatus): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      (status === 'read' ? t('site.contact.markRead') : t('site.contact.archive')) + '?',
      t('common.tips'),
      { type: 'info' },
    )
    await updateContactStatus(row.id, status)
    ElMessage.success(t('site.contact.message.updateStatusSuccess'))
    if (detailData.value?.id === row.id) {
      detailData.value = { ...detailData.value, status }
    }
    reload()
  } catch {
    // cancelled or failed
  }
}
</script>

<style scoped lang="scss">
.contact-page {
}
</style>
