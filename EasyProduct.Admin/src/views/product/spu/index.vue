<template>
  <div class="spu-page">
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
          {{ t('product.spu.add') }}
        </el-button>
      </template>
    </BaseSearchForm>

    <el-card class="spu-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="mainImage"
          :label="t('product.spu.mainImage')"
          width="100"
        >
          <template #default="{ row }">
            <el-image
              v-if="row.mainImage"
              :src="row.mainImage"
              :preview-src-list="[row.mainImage]"
              fit="cover"
              style="width: 60px; height: 60px; border-radius: 4px"
            />
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="code"
          :label="t('product.spu.code')"
          width="140"
          show-overflow-tooltip
        />
        <el-table-column
          prop="name"
          :label="t('product.spu.name')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="nameEn"
          :label="t('product.spu.nameEn')"
          min-width="150"
          show-overflow-tooltip
        />
        <el-table-column
          prop="categoryName"
          :label="t('product.spu.category')"
          width="120"
          show-overflow-tooltip
        />
        <el-table-column
          prop="type"
          :label="t('product.spu.type')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            {{ row.type === 'ticket' ? t('product.spu.typeTicket') : t('product.spu.typeMaterial') }}
          </template>
        </el-table-column>
        <el-table-column
          prop="brand"
          :label="t('product.spu.brand')"
          width="100"
          show-overflow-tooltip
        />
        <el-table-column
          prop="status"
          :label="t('product.spu.status')"
          width="80"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="SPU_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('product.spu.createdAt')"
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

    <SpuFormDialog
      :visible="formDialog.visible.value"
      :is-edit="formDialog.isEdit.value"
      :row-data="formDialog.payload.value"
      :categories="categories"
      @update:visible="formDialog.visible.value = $event"
      @success="reload"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCrud } from '@/composables/useCrud'
import { getSpuList, deleteSpu } from '@/api/product/spu'
import { getCategoryTree } from '@/api/product/category'
import type { ProductSpu, ProductCategory } from '@/types/product'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import SpuFormDialog from './components/SpuFormDialog.vue'

const { t } = useI18n()

const categories = ref<ProductCategory[]>([])

const SPU_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'product.spu.statusActive', type: 'success' },
  inactive: { label: 'product.spu.statusInactive', type: 'info' },
}

const categoryOptions = computed(() => {
  return categories.value.map(c => ({ label: c.name, value: c.id }))
})

const searchFields = computed<SearchField[]>(() => [
  { prop: 'name', label: 'product.spu.searchName', type: 'input' },
  { prop: 'code', label: 'product.spu.searchCode', type: 'input' },
  { prop: 'categoryId', label: 'product.spu.searchCategory', type: 'select', options: categoryOptions.value },
  { prop: 'type', label: 'product.spu.searchType', type: 'select', options: [
    { label: 'product.spu.typeTicket', value: 'ticket' },
    { label: 'product.spu.typeMaterial', value: 'material' },
  ] },
  { prop: 'status', label: 'product.spu.searchStatus', type: 'select', options: [
    { label: 'product.spu.statusActive', value: 'active' },
    { label: 'product.spu.statusInactive', value: 'inactive' },
  ] },
])

const {
  loading, list, total, query, searchModel,
  handleSearch, handleReset, handlePageChange,
  formDialog, openCreate, openEdit,
  handleDelete, reload,
} = useCrud<ProductSpu>(getSpuList, {
  defaultSearchModel: { name: '', code: '', categoryId: '', type: '', status: '' },
  deleteFn: deleteSpu,
  deleteConfirmText: t('product.spu.deleteConfirm'),
  deleteSuccessText: t('product.spu.message.deleteSuccess'),
})

// load categories for search + pass to form dialog
onMounted(async () => {
  try {
    categories.value = await getCategoryTree()
  } catch {
    categories.value = []
  }
})
</script>

<style scoped lang="scss">
.spu-page {
}
</style>
