<template>
  <div class="order-page">
    <BaseSearchForm
      :fields="searchFields"
      :model="searchModel"
      @search="handleSearch"
      @reset="handleReset"
    />

    <el-card class="order-page__table">
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
          :label="t('mall.order.orderNo')"
          width="200"
          show-overflow-tooltip
        />
        <el-table-column
          prop="memberName"
          :label="t('mall.order.memberName')"
          min-width="100"
          show-overflow-tooltip
        />
        <el-table-column
          prop="memberPhone"
          :label="t('mall.order.memberPhone')"
          width="140"
        />
        <el-table-column
          prop="totalAmount"
          :label="t('mall.order.totalAmount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            ¥{{ row.totalAmount.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="payAmount"
          :label="t('mall.order.payAmount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            <span class="text-primary">¥{{ row.payAmount.toFixed(2) }}</span>
          </template>
        </el-table-column>
        <el-table-column
          prop="status"
          :label="t('mall.order.status')"
          width="100"
          align="center"
        >
          <template #default="{ row }">
            <BaseStatusTag
              :value="row.status"
              :options="ORDER_STATUS_MAP"
            />
          </template>
        </el-table-column>
        <el-table-column
          prop="createdAt"
          :label="t('mall.order.createdAt')"
          width="160"
        />
        <el-table-column
          :label="t('common.actions')"
          width="240"
          fixed="right"
        >
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              @click="openDetail(row)"
            >
              {{ t('mall.order.detail') }}
            </el-button>
            <el-button
              v-if="row.status === 'paid'"
              link
              type="success"
              @click="handleStatusChange(row, 'shipped')"
            >
              {{ t('mall.order.actionShip') }}
            </el-button>
            <el-button
              v-if="row.status === 'shipped'"
              link
              type="success"
              @click="handleStatusChange(row, 'completed')"
            >
              {{ t('mall.order.actionComplete') }}
            </el-button>
            <el-button
              v-if="row.status === 'pending'"
              link
              type="warning"
              @click="handleStatusChange(row, 'cancelled')"
            >
              {{ t('mall.order.actionCancel') }}
            </el-button>
            <el-button
              v-if="['paid', 'shipped', 'completed'].includes(row.status)"
              link
              type="danger"
              @click="handleStatusChange(row, 'refunded')"
            >
              {{ t('mall.order.actionRefund') }}
            </el-button>
          </template>
        </el-table-column>
      </BaseTable>
    </el-card>

    <OrderDetailDialog
      :visible="detailVisible"
      :order="selectedOrder"
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
import { getOrderList, getOrderDetail, updateOrderStatus } from '@/api/mall/order'
import type { Order, OrderStatus } from '@/types/mall'
import type { SearchField } from '@/types/search'
import BaseTable from '@/components/common/BaseTable.vue'
import BaseSearchForm from '@/components/common/BaseSearchForm.vue'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
import OrderDetailDialog from './components/OrderDetailDialog.vue'

const { t } = useI18n()

const ORDER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  pending: { label: 'mall.order.statusPending', type: 'warning' },
  paid: { label: 'mall.order.statusPaid', type: 'success' },
  shipped: { label: 'mall.order.statusShipped', type: '' },
  completed: { label: 'mall.order.statusCompleted', type: 'success' },
  cancelled: { label: 'mall.order.statusCancelled', type: 'info' },
  refunded: { label: 'mall.order.statusRefunded', type: 'danger' },
}

const detailVisible = ref(false)
const selectedOrder = ref<Order | null>(null)

const searchFields = computed<SearchField[]>(() => [
  { prop: 'orderNo', label: 'mall.order.searchOrderNo', type: 'input' },
  { prop: 'status', label: 'mall.order.searchStatus', type: 'select', options: [
    { label: 'mall.order.statusPending', value: 'pending' },
    { label: 'mall.order.statusPaid', value: 'paid' },
    { label: 'mall.order.statusShipped', value: 'shipped' },
    { label: 'mall.order.statusCompleted', value: 'completed' },
    { label: 'mall.order.statusCancelled', value: 'cancelled' },
    { label: 'mall.order.statusRefunded', value: 'refunded' },
  ] },
])

const { loading, list, total, query, handleSearch: tableSearch, handleReset: tableReset, handlePageChange, reload } = useTable<Order>(getOrderList)

const { searchModel, resetModel, getSearchParams } = useSearch({
  defaultModel: { orderNo: '', status: '' },
})

const handleSearch = () => {
  Object.assign(query, getSearchParams())
  void tableSearch()
}

const handleReset = () => {
  resetModel()
  void tableReset()
}

const openDetail = async (row: Order) => {
  try {
    const detail = await getOrderDetail(row.id)
    selectedOrder.value = detail
    detailVisible.value = true
  } catch {
    ElMessage.error(t('common.loadFailed'))
  }
}

const handleStatusChange = async (row: Order, newStatus: OrderStatus) => {
  const confirmMap: Partial<Record<OrderStatus, string>> = {
    shipped: t('mall.order.shipConfirm'),
    cancelled: t('mall.order.cancelConfirm'),
    refunded: t('mall.order.refundConfirm'),
    completed: t('mall.order.completeConfirm'),
  }

  try {
    await ElMessageBox.confirm(confirmMap[newStatus] ?? '', t('common.tips'), { type: 'warning' })
    await updateOrderStatus(row.id, { status: newStatus })
    ElMessage.success(t('mall.order.message.statusUpdateSuccess'))
    await reload()
  } catch {
    // user cancelled
  }
}
</script>

<style scoped lang="scss">
.order-page {
  .text-primary { color: var(--el-color-primary); font-weight: 600; }
}
</style>
