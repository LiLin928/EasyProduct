<template>
  <div class="fixed-asset-page">
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
          {{ t('crm.fixedAsset.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="fixed-asset-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="assetNo"
          :label="t('crm.fixedAsset.assetNo')"
          width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="name"
          :label="t('crm.fixedAsset.name')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="category"
          :label="t('crm.fixedAsset.category')"
          width="120"
          align="center"
        />
        <el-table-column
          prop="originalValue"
          :label="t('crm.fixedAsset.originalValue')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.originalValue) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="purchaseDate"
          :label="t('crm.fixedAsset.purchaseDate')"
          width="120"
        />
        <el-table-column
          prop="salvageValue"
          :label="t('crm.fixedAsset.salvageValue')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.salvageValue) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="usefulYears"
          :label="t('crm.fixedAsset.usefulYears')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="currentValue"
          :label="t('crm.fixedAsset.currentValue')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.currentValue) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.fixedAsset.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          :label="t('common.actions')"
          width="220"
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
            <el-button
              v-if="row.status === 'active'"
              link
              type="primary"
              @click="openEdit(row)"
            >
              {{ t('common.edit') }}
            </el-button>
            <el-button
              v-if="row.status === 'active'"
              link
              type="warning"
              @click="handleStatusChange(row, 'scrapped')"
            >
              {{ t('crm.fixedAsset.actionScrap') }}
            </el-button>
            <el-button
              v-if="row.status === 'active'"
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

    <FixedAssetFormDialog
      :visible="formVisible"
      :asset="selectedAsset"
      @update:visible="formVisible = $event"
      @success="reload"
    />

    <FixedAssetDetailDialog
      :visible="detailVisible"
      :asset="selectedAsset"
      @update:visible="detailVisible = $event"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import {
  getFixedAssetList,
  deleteFixedAsset,
  updateFixedAssetStatus,
} from '@/api/crm/fixed-asset'
import { ASSET_CATEGORY_OPTIONS } from '@/types/crm'
import type { FixedAsset, FixedAssetStatus } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import FixedAssetFormDialog from './components/FixedAssetFormDialog.vue'
import FixedAssetDetailDialog from './components/FixedAssetDetailDialog.vue'

const { t } = useI18n()

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'crm.fixedAsset.statusActive', type: 'success' },
  scrapped: { label: 'crm.fixedAsset.statusScrapped', type: 'danger' },
}

const STATUS_LABEL_MAP: Record<FixedAssetStatus, string> = {
  active: 'crm.fixedAsset.statusActive',
  scrapped: 'crm.fixedAsset.statusScrapped',
}

const formVisible = ref(false)
const detailVisible = ref(false)
const selectedAsset = ref<FixedAsset | null>(null)

const searchFields = computed<SearchField[]>(() => [
  {
    prop: 'category',
    label: 'crm.fixedAsset.searchCategory',
    type: 'select',
    options: ASSET_CATEGORY_OPTIONS.map((opt) => ({
      label: opt.labelKey,
      value: opt.value,
    })),
  },
  {
    prop: 'status',
    label: 'crm.fixedAsset.searchStatus',
    type: 'select',
    options: [
      { label: 'crm.fixedAsset.statusActive', value: 'active' },
      { label: 'crm.fixedAsset.statusScrapped', value: 'scrapped' },
    ],
  },
  { prop: 'keyword', label: 'crm.fixedAsset.searchKeyword', type: 'input' },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<FixedAsset>(getFixedAssetList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { category: '', status: '', keyword: '' },
})

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const formatMoney = (val: number) => {
  return val.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const openCreate = () => {
  selectedAsset.value = null
  formVisible.value = true
}

const openEdit = (row: FixedAsset) => {
  selectedAsset.value = row
  formVisible.value = true
}

const openDetail = (row: FixedAsset) => {
  selectedAsset.value = row
  detailVisible.value = true
}

const handleStatusChange = async (row: FixedAsset, newStatus: FixedAssetStatus) => {
  try {
    const msg = t('crm.fixedAsset.confirmMessage', { status: t(STATUS_LABEL_MAP[newStatus]) })
    await ElMessageBox.confirm(msg, t('common.tips'), { type: 'warning' })
    await updateFixedAssetStatus(row.id, { status: newStatus })
    ElMessage.success(t('crm.fixedAsset.message.statusUpdateSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}

const handleDelete = async (row: FixedAsset) => {
  try {
    await ElMessageBox.confirm(t('crm.fixedAsset.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deleteFixedAsset(row.id)
    ElMessage.success(t('crm.fixedAsset.message.deleteSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>

<style scoped lang="scss">
.fixed-asset-page {
}
