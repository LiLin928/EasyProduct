<template>
  <div class="member-page">
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
          {{ t('mall.member.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="member-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="avatar"
          :label="t('mall.member.avatar')"
          width="80"
        >
          <template #default="{ row }">
            <el-avatar
              v-if="row.avatar"
              :src="row.avatar"
              :size="40"
            />
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="nickname"
          :label="t('mall.member.nickname')"
          min-width="120"
          show-overflow-tooltip
        />
        <el-table-column
          prop="phone"
          :label="t('mall.member.phone')"
          width="140"
        />
        <el-table-column
          prop="levelName"
          :label="t('mall.member.levelName')"
          width="120"
        />
        <el-table-column
          prop="points"
          :label="t('mall.member.points')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="totalSpent"
          :label="t('mall.member.totalSpent')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            ¥{{ row.totalSpent.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="orderCount"
          :label="t('mall.member.orderCount')"
          width="100"
          align="center"
        />
        <el-table-column
          prop="status"
          :label="t('mall.member.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="MEMBER_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('mall.member.createdAt')"
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

    <MemberFormDialog
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      :row-data="formDialog.payload.value"
      :levels="levels"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCrud } from '@/composables/useCrud'
import { getMemberList, deleteMember, getMemberLevelOptions } from '@/api/mall/member'
import type { Member } from '@/types/mall'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import MemberFormDialog from './components/MemberFormDialog.vue'

const { t } = useI18n()

const MEMBER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'mall.member.statusActive', type: 'success' },
  inactive: { label: 'mall.member.statusInactive', type: 'danger' },
}

const levels = ref<Array<{ id: string; name: string }>>([])

const searchFields = computed<SearchField[]>(() => [
  { prop: 'nickname', label: 'mall.member.searchNickname', type: 'input' },
  { prop: 'phone', label: 'mall.member.searchPhone', type: 'input' },
  { prop: 'levelId', label: 'mall.member.searchLevel', type: 'select', options: levels.value.map(l => ({ label: l.name, value: l.id })) },
  { prop: 'status', label: 'mall.member.searchStatus', type: 'select', options: [
    { label: 'mall.member.statusActive', value: 'active' },
    { label: 'mall.member.statusInactive', value: 'inactive' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<Member>(getMemberList, {
  defaultSearchModel: { nickname: '', phone: '', levelId: '', status: '' },
  deleteFn: deleteMember,
  deleteConfirmText: t('mall.member.deleteConfirm'),
  deleteSuccessText: t('mall.member.message.deleteSuccess'),
})

onMounted(async () => {
  try {
    levels.value = await getMemberLevelOptions()
  } catch {
    levels.value = []
  }
})
</script>

<style scoped lang="scss">
.member-page {
}
</style>
