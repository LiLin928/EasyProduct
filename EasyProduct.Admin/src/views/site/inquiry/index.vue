<template>
  <div class="inquiry-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="inquiry-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="companyName"
          :label="t('site.inquiry.companyName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="contactName"
          :label="t('site.inquiry.contactName')"
          width="120"
        />
        <el-table-column
          prop="phone"
          :label="t('site.inquiry.phone')"
          width="140"
        />
        <el-table-column
          prop="email"
          :label="t('site.inquiry.email')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="status"
          :label="t('site.inquiry.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="INQUIRY_STATUS_OPTIONS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('site.inquiry.createdAt')"
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
              v-if="row.status !== 'converted'"
              link
              type="warning"
              @click="handleUpdateStatus(row, 'processing')"
            >
              {{ t('site.inquiry.updateStatus') }}
            </el-button>
            <el-button
              v-if="row.status !== 'converted'"
              link
              type="success"
              @click="handleConvert(row)"
            >
              {{ t('site.inquiry.convertToCustomer') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <!-- Detail Dialog -->
    <el-dialog
      v-model="detailVisible"
      :title="t('site.inquiry.detail')"
      width="700px"
      append-to-body
    >
      <el-descriptions
        v-if="detailData"
        :column="2"
        border
      >
        <el-descriptions-item :label="t('site.inquiry.companyName')">
          {{ detailData.companyName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.inquiry.contactName')">
          {{ detailData.contactName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.inquiry.phone')">
          {{ detailData.phone }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.inquiry.email')">
          {{ detailData.email }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.inquiry.status')">
          <BaseStatusTag
            :value="detailData.status"
            :options="INQUIRY_STATUS_OPTIONS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('site.inquiry.createdAt')">
          {{ detailData.createdAt }}
        </el-descriptions-item>
        <el-descriptions-item
          v-if="detailData.remark"
          :label="t('site.inquiry.inquiryRemark')"
          :span="2"
        >
          {{ detailData.remark }}
        </el-descriptions-item>
      </el-descriptions>

      <el-table
        v-if="detailData?.items?.length"
        :data="detailData.items"
        border
        style="margin-top: 16px"
      >
        <el-table-column
          prop="productName"
          :label="t('site.inquiry.productName')"
          min-width="160"
        />
        <el-table-column
          prop="quantity"
          :label="t('site.inquiry.quantity')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="unit"
          :label="t('site.inquiry.unit')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="remark"
          :label="t('site.inquiry.remark')"
          min-width="120"
          show-overflow-tooltip
        />
      </el-table>

      <template #footer>
        <el-button @click="detailVisible = false">
          {{ t('common.close') }}
        </el-button>
        <el-button
          v-if="detailData && detailData.status !== 'converted'"
          type="success"
          @click="handleConvert(detailData!)"
        >
          {{ t('site.inquiry.convertToCustomer') }}
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
import { getInquiryList, getInquiryById, updateInquiryStatus, convertInquiryToCustomer } from '@/api/site/inquiry'
import type { SiteInquiry, InquiryStatus } from '@/types/site'
import { INQUIRY_STATUS_OPTIONS } from '@/types/site'
import type { SearchField } from '@/types/search'
import type { StatusOption } from '@/components/common/BaseStatusTag.vue'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const { t } = useI18n()

const INQUIRY_STATUS_OPTIONS_MAP: Record<string, StatusOption> = {
  pending: { label: 'site.inquiry.statusPending', type: 'warning' },
  processing: { label: 'site.inquiry.statusProcessing', type: '' },
  completed: { label: 'site.inquiry.statusCompleted', type: 'success' },
  converted: { label: 'site.inquiry.statusConverted', type: 'info' },
}

const searchFields: SearchField[] = [
  { prop: 'companyName', label: 'site.inquiry.searchCompanyName', type: 'input' },
  { prop: 'status', label: 'site.inquiry.searchStatus', type: 'select', options: INQUIRY_STATUS_OPTIONS },
]

const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { companyName: '', status: '' },
})

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable(getInquiryList, { immediate: true })

const handleSearch = (): void => {
  Object.assign(query, getSearchParams())
  tableSearch()
}

const handleReset = (): void => {
  resetModel()
  delete query.companyName
  delete query.status
  tableReset()
}

// Detail dialog
const detailVisible = ref(false)
const detailData = ref<SiteInquiry | null>(null)

const handleDetail = async (row: SiteInquiry): Promise<void> => {
  try {
    const data = await getInquiryById(row.id)
    detailData.value = data
    detailVisible.value = true
  } catch {
    // handled by interceptor
  }
}

const handleUpdateStatus = async (row: SiteInquiry, status: InquiryStatus): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('site.inquiry.updateStatus') + '?',
      t('common.tips'),
      { type: 'info' },
    )
    await updateInquiryStatus(row.id, status)
    ElMessage.success(t('site.inquiry.message.updateStatusSuccess'))
    if (detailData.value?.id === row.id) {
      detailData.value = { ...detailData.value, status }
    }
    reload()
  } catch {
    // cancelled or failed
  }
}

const handleConvert = async (row: SiteInquiry): Promise<void> => {
  try {
    await ElMessageBox.confirm(
      t('site.inquiry.convertConfirm'),
      t('common.tips'),
      { type: 'warning' },
    )
    await convertInquiryToCustomer(row.id)
    ElMessage.success(t('site.inquiry.message.convertSuccess'))
    if (detailData.value?.id === row.id) {
      detailData.value = { ...detailData.value, status: 'converted' }
    }
    reload()
  } catch {
    // cancelled or failed
  }
}
</script>

<style scoped lang="scss">
.inquiry-page {
}
</style>
