<template>
  <div class="level-page">
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
          {{ t('mall.level.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="level-page__table">
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
          :label="t('mall.level.name')"
          min-width="120"
          show-overflow-tooltip
        />
        <el-table-column
          prop="minPoints"
          :label="t('mall.level.minPoints')"
          width="120"
          align="right"
        />
        <el-table-column
          prop="discount"
          :label="t('mall.level.discount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ (row.discount * 10).toFixed(1) }}{{ t('mall.common.discountUnit') }}
          </template>
        </el-table-column>
        <el-table-column
          prop="sort"
          :label="t('mall.level.sort')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="status"
          :label="t('mall.level.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="LEVEL_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('mall.level.createdAt')"
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

    <LevelFormDialog
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
import { getLevelList, deleteLevel } from '@/api/mall/level'
import type { MemberLevel } from '@/types/mall'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import LevelFormDialog from './components/LevelFormDialog.vue'

const { t } = useI18n()

const LEVEL_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  enabled: { label: 'mall.level.statusEnabled', type: 'success' },
  disabled: { label: 'mall.level.statusDisabled', type: 'info' },
}

const searchFields = computed<SearchField[]>(() => [
  { prop: 'name', label: 'mall.level.searchName', type: 'input' },
  { prop: 'status', label: 'mall.level.searchStatus', type: 'select', options: [
    { label: 'mall.level.statusEnabled', value: 'enabled' },
    { label: 'mall.level.statusDisabled', value: 'disabled' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<MemberLevel>(getLevelList, {
  defaultSearchModel: { name: '', status: '' },
  deleteFn: deleteLevel,
  deleteConfirmText: t('mall.level.deleteConfirm'),
  deleteSuccessText: t('mall.level.message.deleteSuccess'),
})
</script>

<style scoped lang="scss">
.level-page {
}
</style>
