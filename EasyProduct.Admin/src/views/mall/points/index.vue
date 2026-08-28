<template>
  <div class="points-page">
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
          {{ t('mall.points.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="points-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="memberName"
          :label="t('mall.points.memberName')"
          min-width="120"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('mall.points.type')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              :type="row.type === 'earn' ? 'success' : 'warning'"
              size="small"
            >
              {{ row.type === 'earn' ? t('mall.points.typeEarn') : t('mall.points.typeSpend') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="amount"
          :label="t('mall.points.amount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            <span :class="row.amount > 0 ? 'text-success' : 'text-danger'">
              {{ row.amount > 0 ? '+' : '' }}{{ row.amount }}
            </span>
          </template>
        </el-table-column>
        <el-table-column
          prop="source"
          :label="t('mall.points.source')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            {{ sourceLabel(row.source) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="description"
          :label="t('mall.points.description')"
          min-width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="createdAt"
          :label="t('mall.points.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="100"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="danger"
              @click="handleDelete(row)"
            >
              {{ t('mall.points.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <PointsFormDialog
      :visible="formDialog.visible.value"
      :members="members"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCrud } from '@/composables/useCrud'
import { getPointsList, deletePointsRecord, getPointsMemberOptions } from '@/api/mall/points'
import type { PointsRecord, PointsSource } from '@/types/mall'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import PointsFormDialog from './components/PointsFormDialog.vue'

const { t } = useI18n()

const SOURCE_LABELS: Record<PointsSource, string> = {
  order: 'mall.points.sourceOrder',
  signin: 'mall.points.sourceSignin',
  activity: 'mall.points.sourceActivity',
  refund: 'mall.points.sourceRefund',
  adjust: 'mall.points.sourceAdjust',
}

const sourceLabel = (source: PointsSource) => t(SOURCE_LABELS[source] || source)

const members = ref<Array<{ id: string; name: string }>>([])

const searchFields = computed<SearchField[]>(() => [
  { prop: 'memberId', label: 'mall.points.searchMember', type: 'select', options: members.value.map(m => ({ label: m.name, value: m.id })) },
  { prop: 'type', label: 'mall.points.searchType', type: 'select', options: [
    { label: 'mall.points.typeEarn', value: 'earn' },
    { label: 'mall.points.typeSpend', value: 'spend' },
  ] },
  { prop: 'source', label: 'mall.points.searchSource', type: 'select', options: [
    { label: 'mall.points.sourceOrder', value: 'order' },
    { label: 'mall.points.sourceSignin', value: 'signin' },
    { label: 'mall.points.sourceActivity', value: 'activity' },
    { label: 'mall.points.sourceRefund', value: 'refund' },
    { label: 'mall.points.sourceAdjust', value: 'adjust' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate,
  handleDelete, reload,
} = useCrud<PointsRecord>(getPointsList, {
  defaultSearchModel: { memberId: '', type: '', source: '' },
  deleteFn: deletePointsRecord,
  deleteConfirmText: t('mall.points.deleteConfirm'),
  deleteSuccessText: t('mall.points.message.deleteSuccess'),
})

onMounted(async () => {
  try {
    members.value = await getPointsMemberOptions()
  } catch {
    members.value = []
  }
})
</script>

<style scoped lang="scss">
.points-page {
  .text-success { color: var(--el-color-success); }
  .text-danger { color: var(--el-color-danger); }
}
</style>
