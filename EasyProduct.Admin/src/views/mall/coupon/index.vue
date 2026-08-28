<template>
  <div class="coupon-page">
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
          {{ t('mall.coupon.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="coupon-page__table">
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
          :label="t('mall.coupon.name')"
          min-width="140"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('mall.coupon.type')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              :type="row.type === 'fixed' ? '' : 'success'"
              size="small"
            >
              {{ row.type === 'fixed' ? t('mall.coupon.typeFixed') : t('mall.coupon.typePercent') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column
          prop="value"
          :label="t('mall.coupon.value')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            {{ row.type === 'fixed' ? `¥${row.value}` : `${(row.value * 10).toFixed(1)}${t('mall.common.discountUnit')}` }}
          </template>
        </el-table-column>
        <el-table-column
          prop="minSpend"
          :label="t('mall.coupon.minSpend')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            ¥{{ row.minSpend }}
          </template>
        </el-table-column>
        <el-table-column
          :label="t('mall.coupon.issuedCount') + '/' + t('mall.coupon.usedCount')"
          width="140"
          align="center"
        >
          <template #default="{ row }">
            {{ row.issuedCount }} / {{ row.usedCount }}
          </template>
        </el-table-column>
        <el-table-column
          prop="startDate"
          :label="t('mall.coupon.startDate')"
          width="120"
        />
        <el-table-column
          prop="endDate"
          :label="t('mall.coupon.endDate')"
          width="120"
        />
        <el-table-column
          prop="status"
          :label="t('mall.coupon.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="COUPON_STATUS_MAP"
            />
          </template>
        </el-table-column>
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

    <CouponFormDialog
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      :row-data="formDialog.payload.value"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCrud } from '@/composables/useCrud'
import { getCouponList, deleteCoupon } from '@/api/mall/coupon'
import type { Coupon } from '@/types/mall'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import CouponFormDialog from './components/CouponFormDialog.vue'

const { t } = useI18n()

const COUPON_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  enabled: { label: 'mall.coupon.statusEnabled', type: 'success' },
  disabled: { label: 'mall.coupon.statusDisabled', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'name', label: 'mall.coupon.searchName', type: 'input' },
  { prop: 'type', label: 'mall.coupon.searchType', type: 'select', options: [
    { label: 'mall.coupon.typeFixed', value: 'fixed' },
    { label: 'mall.coupon.typePercent', value: 'percent' },
  ] },
  { prop: 'status', label: 'mall.coupon.searchStatus', type: 'select', options: [
    { label: 'mall.coupon.statusEnabled', value: 'enabled' },
    { label: 'mall.coupon.statusDisabled', value: 'disabled' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<Coupon>(getCouponList, {
  defaultSearchModel: { name: '', type: '', status: '' },
  deleteFn: deleteCoupon,
  deleteConfirmText: t('mall.coupon.deleteConfirm'),
  deleteSuccessText: t('mall.coupon.message.deleteSuccess'),
})
</script>

<style scoped lang="scss">
.coupon-page {
}
</style>
