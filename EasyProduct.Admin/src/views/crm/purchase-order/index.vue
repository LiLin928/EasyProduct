<template>
  <div class="purchase-order-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />
    <el-card class="purchase-order-page__table">
      <BaseTable
        :loading="loading"
        :data="list"
        :total="total"
        :current-page="query.pageIndex"
        :page-size="query.pageSize"
        @page-change="handlePageChange"
      >
        <el-table-column
          prop="orderNo"
          :label="t('crm.purchaseOrder.orderNo')"
          width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="supplierName"
          :label="t('crm.purchaseOrder.supplier')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="buyerName"
          :label="t('crm.purchaseOrder.buyerName')"
          width="100"
        />
        <el-table-column
          prop="paymentTerms"
          :label="t('crm.purchaseOrder.paymentTerms')"
          width="100"
        />
        <el-table-column
          prop="deliveryDate"
          :label="t('crm.purchaseOrder.deliveryDate')"
          width="120"
        />
        <el-table-column
          prop="totalAmount"
          :label="t('crm.purchaseOrder.totalAmount')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            <span class="text-primary">{{ row.currencySymbol }}{{ row.totalAmount.toFixed(2) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.purchaseOrder.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="PURCHASE_ORDER_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('crm.purchaseOrder.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="280"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openDetail(row)"
            >
              {{ t('crm.purchaseOrder.detail') }}
            </el-button>
            <el-button
              v-if="row.status === 'draft'"
              link
              type="success"
              @click="handleStatusChange(row, 'confirmed')"
            >
              {{ t('crm.purchaseOrder.actionConfirm') }}
            </el-button>
            <el-button
              v-if="row.status === 'confirmed'"
              link
              type="success"
              @click="handleStatusChange(row, 'received')"
            >
              {{ t('crm.purchaseOrder.actionReceive') }}
            </el-button>
            <el-button
              v-if="row.status === 'received'"
              link
              type="success"
              @click="handleStatusChange(row, 'completed')"
            >
              {{ t('crm.purchaseOrder.actionComplete') }}
            </el-button>
            <el-button
              v-if="['draft', 'confirmed'].includes(row.status)"
              link
              type="warning"
              @click="handleStatusChange(row, 'cancelled')"
            >
              {{ t('crm.purchaseOrder.actionCancel') }}
            </el-button>
            <el-button
              v-if="row.status === 'draft'"
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
    <PurchaseOrderDetailDialog
      :visible="detailVisible"
      :order="selectedOrder"
      @update:visible="detailVisible = $event"
    />
  </div>
</template>
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useTable } from '@/composables/useTable'
import { useSearch } from '@/composables/useSearch'
import {
  getPurchaseOrderList,
  getPurchaseOrderById,
  updatePurchaseOrderStatus,
  deletePurchaseOrder,
  getPurchaseOrderSupplierOptions,
} from '@/api/crm/purchase-order'
import type { PurchaseOrder, PurchaseOrderStatus } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import PurchaseOrderDetailDialog from './components/PurchaseOrderDetailDialog.vue'
const { t } = useI18n()
const PURCHASE_ORDER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.purchaseOrder.statusDraft', type: 'info' },
  confirmed: { label: 'crm.purchaseOrder.statusConfirmed', type: 'warning' },
  received: { label: 'crm.purchaseOrder.statusReceived', type: '' },
  completed: { label: 'crm.purchaseOrder.statusCompleted', type: 'success' },
  cancelled: { label: 'crm.purchaseOrder.statusCancelled', type: 'danger' },
}
const detailVisible = ref(false)
const selectedOrder = ref<PurchaseOrder | null>(null)
const supplierOptions = ref<Array<{ id: string; name: string }>>([])
onMounted(async () => {
  try {
    supplierOptions.value = await getPurchaseOrderSupplierOptions()
  } catch {
    // ignore
  }
})
const searchFields = computed<SearchField[]>(() => [
  { prop: 'orderNo', label: 'crm.purchaseOrder.searchOrderNo', type: 'input' },
  {
    prop: 'supplierId',
    label: 'crm.purchaseOrder.searchSupplier',
    type: 'select',
    options: supplierOptions.value.map(s => ({ label: s.name, value: s.id })),
  },
  {
    prop: 'status',
    label: 'crm.purchaseOrder.searchStatus',
    type: 'select',
    options: [
      { label: 'crm.purchaseOrder.statusDraft', value: 'draft' },
      { label: 'crm.purchaseOrder.statusConfirmed', value: 'confirmed' },
      { label: 'crm.purchaseOrder.statusReceived', value: 'received' },
      { label: 'crm.purchaseOrder.statusCompleted', value: 'completed' },
      { label: 'crm.purchaseOrder.statusCancelled', value: 'cancelled' },
    ],
  },
])
const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<PurchaseOrder>(getPurchaseOrderList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { orderNo: '', supplierId: '', status: '' },
})
const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}
const handleReset = () => {
  resetModel()
  void tableReset()
}
const openDetail = async (row: PurchaseOrder) => {
  try {
    const detail = await getPurchaseOrderById(row.id)
    selectedOrder.value = detail
    detailVisible.value = true
  } catch {
    ElMessage.error(t('common.loadFailed'))
  }
}
const STATUS_LABEL_MAP: Record<PurchaseOrderStatus, string> = {
  draft: 'crm.purchaseOrder.statusDraft',
  confirmed: 'crm.purchaseOrder.statusConfirmed',
  received: 'crm.purchaseOrder.statusReceived',
  completed: 'crm.purchaseOrder.statusCompleted',
  cancelled: 'crm.purchaseOrder.statusCancelled',
}
const handleStatusChange = async (row: PurchaseOrder, newStatus: PurchaseOrderStatus) => {
  try {
    const msg = t('crm.purchaseOrder.confirmMessage', { status: t(STATUS_LABEL_MAP[newStatus]) })
    await ElMessageBox.confirm(msg, t('common.tips'), { type: 'warning' })
    await updatePurchaseOrderStatus(row.id, { status: newStatus })
    ElMessage.success(t('crm.purchaseOrder.message.statusUpdateSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
const handleDelete = async (row: PurchaseOrder) => {
  try {
    await ElMessageBox.confirm(t('crm.purchaseOrder.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deletePurchaseOrder(row.id)
    ElMessage.success(t('crm.purchaseOrder.message.deleteSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>
<style scoped lang="scss">
.purchase-order-page {
  .text-primary { color: var(--el-color-primary); font-weight: 600; }
}
</style>
