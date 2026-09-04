<template>
  <div class="address-page">
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
          {{ t('mall.address.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="address-page__table">
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
          :label="t('mall.address.memberName')"
          min-width="100"
          show-overflow-tooltip
        />
        <el-table-column
          prop="memberPhone"
          :label="t('mall.address.memberPhone')"
          width="130"
        />
        <el-table-column
          prop="name"
          :label="t('mall.address.name')"
          min-width="80"
        />
        <el-table-column
          prop="phone"
          :label="t('mall.address.phone')"
          width="130"
        />
        <el-table-column
          prop="fullAddress"
          :label="t('mall.address.fullAddress')"
          min-width="280"
          show-overflow-tooltip
        />
        <el-table-column
          prop="isDefault"
          :label="t('mall.address.isDefault')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <el-tag
              v-if="row.isDefault"
              type="success"
              size="small"
            >
              {{ t('mall.address.default') }}
            </el-tag>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('mall.address.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="200"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              v-if="!row.isDefault"
              link
              type="primary"
              @click="handleSetDefault(row)"
            >
              {{ t('mall.address.setDefault') }}
            </el-button>
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

    <AddressFormDialog
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
import { getAddressList, deleteAddress, setDefaultAddress } from '@/api/mall/address'
import type { AddressListItem } from '@/types/mall'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import AddressFormDialog from './components/AddressFormDialog.vue'
import { ElMessage, ElMessageBox } from 'element-plus'

const { t } = useI18n()

const searchFields = computed<SearchField[]>(() => [
  { prop: 'memberName', label: 'mall.address.searchMemberName', type: 'input' },
  { prop: 'phone', label: 'mall.address.searchPhone', type: 'input' },
  { prop: 'province', label: 'mall.address.searchProvince', type: 'input' },
  { prop: 'city', label: 'mall.address.searchCity', type: 'input' },
  { prop: 'keyword', label: 'mall.address.searchKeyword', type: 'input' },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<AddressListItem>(getAddressList, {
  defaultSearchModel: { memberName: '', phone: '', province: '', city: '', keyword: '' },
  deleteFn: deleteAddress,
  deleteConfirmText: t('mall.address.deleteConfirm'),
  deleteSuccessText: t('mall.address.message.deleteSuccess'),
})

async function handleSetDefault(row: AddressListItem) {
  try {
    await ElMessageBox.confirm(
      t('mall.address.setDefaultConfirm', { name: row.name }),
      t('common.confirm'),
      { type: 'warning' }
    )
    await setDefaultAddress(row.id)
    ElMessage.success(t('mall.address.message.setDefaultSuccess'))
    reload()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error?.message || t('mall.address.message.setDefaultFailed'))
    }
  }
}
</script>

<style scoped lang="scss">
.address-page {
}
</style>

