<template>
  <div class="sales-order-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />
    <el-card class="sales-order-page__table">
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
          :label="t('crm.salesOrder.orderNo')"
          width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="customerName"
          :label="t('crm.salesOrder.customer')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="salesPersonName"
          :label="t('crm.salesOrder.salesPersonName')"
          width="100"
        />
        <el-table-column
          prop="paymentTerms"
          :label="t('crm.salesOrder.paymentTerms')"
          width="100"
        />
        <el-table-column
          prop="deliveryDate"
          :label="t('crm.salesOrder.deliveryDate')"
          width="120"
        />
        <el-table-column
          prop="totalAmount"
          :label="t('crm.salesOrder.totalAmount')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            <span class="text-primary">{{ row.currencySymbol }}{{ row.totalAmount.toFixed(2) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('crm.salesOrder.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="SALES_ORDER_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('crm.salesOrder.createdAt')"
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
              {{ t('crm.salesOrder.detail') }}
            </el-button>
            <el-button
              v-if="row.status === 'draft'"
              link
              type="success"
              @click="handleStatusChange(row, 'confirmed')"
            >
              {{ t('crm.salesOrder.actionConfirm') }}
            </el-button>
            <el-button
              v-if="row.status === 'confirmed'"
              link
              type="success"
              @click="handleStatusChange(row, 'shipped')"
            >
              {{ t('crm.salesOrder.actionShip') }}
            </el-button>
            <el-button
              v-if="row.status === 'shipped'"
              link
              type="success"
              @click="handleStatusChange(row, 'completed')"
            >
              {{ t('crm.salesOrder.actionComplete') }}
            </el-button>
            <el-button
              v-if="['draft', 'confirmed'].includes(row.status)"
              link
              type="warning"
              @click="handleStatusChange(row, 'cancelled')"
            >
              {{ t('crm.salesOrder.actionCancel') }}
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
    <SalesOrderDetailDialog
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
  getSalesOrderList,
  getSalesOrderById,
  updateSalesOrderStatus,
  deleteSalesOrder,
  getSalesOrderCustomerOptions,
} from '@/api/crm/sales-order'
import type { SalesOrder, SalesOrderStatus } from '@/types/crm'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import SalesOrderDetailDialog from './components/SalesOrderDetailDialog.vue'
const { t } = useI18n()
const SALES_ORDER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.salesOrder.statusDraft', type: 'info' },
  confirmed: { label: 'crm.salesOrder.statusConfirmed', type: 'warning' },
  shipped: { label: 'crm.salesOrder.statusShipped', type: '' },
  completed: { label: 'crm.salesOrder.statusCompleted', type: 'success' },
  cancelled: { label: 'crm.salesOrder.statusCancelled', type: 'danger' },
}
const detailVisible = ref(false)
const selectedOrder = ref<SalesOrder | null>(null)
const customerOptions = ref<Array<{ id: string; name: string }>>([])
onMounted(async () => {
  try {
    customerOptions.value = await getSalesOrderCustomerOptions()
  } catch {
    // ignore
  }
})
const searchFields = computed<SearchField[]>(() => [
  { prop: 'orderNo', label: 'crm.salesOrder.searchOrderNo', type: 'input' },
  {
    prop: 'customerId',
    label: 'crm.salesOrder.searchCustomer',
    type: 'select',
    options: customerOptions.value.map(c => ({ label: c.name, value: c.id })),
  },
  {
    prop: 'status',
    label: 'crm.salesOrder.searchStatus',
    type: 'select',
    options: [
      { label: 'crm.salesOrder.statusDraft', value: 'draft' },
      { label: 'crm.salesOrder.statusConfirmed', value: 'confirmed' },
      { label: 'crm.salesOrder.statusShipped', value: 'shipped' },
      { label: 'crm.salesOrder.statusCompleted', value: 'completed' },
      { label: 'crm.salesOrder.statusCancelled', value: 'cancelled' },
    ],
  },
])
const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<SalesOrder>(getSalesOrderList)
const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { orderNo: '', customerId: '', status: '' },
})
const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}
const handleReset = () => {
  resetModel()
  void tableReset()
}
const openDetail = async (row: SalesOrder) => {
  try {
    const detail = await getSalesOrderById(row.id)
    selectedOrder.value = detail
    detailVisible.value = true
  } catch {
    ElMessage.error(t('common.loadFailed'))
  }
}
const STATUS_LABEL_MAP: Record<SalesOrderStatus, string> = {
  draft: 'crm.salesOrder.statusDraft',
  confirmed: 'crm.salesOrder.statusConfirmed',
  shipped: 'crm.salesOrder.statusShipped',
  completed: 'crm.salesOrder.statusCompleted',
  cancelled: 'crm.salesOrder.statusCancelled',
}
const handleStatusChange = async (row: SalesOrder, newStatus: SalesOrderStatus) => {
  try {
    const msg = t('crm.salesOrder.confirmMessage', { status: t(STATUS_LABEL_MAP[newStatus]) })
    await ElMessageBox.confirm(msg, t('common.tips'), { type: 'warning' })
    await updateSalesOrderStatus(row.id, { status: newStatus })
    ElMessage.success(t('crm.salesOrder.message.statusUpdateSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
const handleDelete = async (row: SalesOrder) => {
  try {
    await ElMessageBox.confirm(t('crm.salesOrder.deleteConfirm'), t('common.tips'), { type: 'warning' })
    await deleteSalesOrder(row.id)
    ElMessage.success(t('crm.salesOrder.message.deleteSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>
<style scoped lang="scss">
.sales-order-page {
  .text-primary { color: var(--el-color-primary); font-weight: 600; }
}
</style>
