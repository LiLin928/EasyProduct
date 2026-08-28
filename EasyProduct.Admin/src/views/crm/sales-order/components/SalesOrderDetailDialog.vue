<template>
  <el-dialog
    :model-value="visible"
    :title="t('crm.salesOrder.detail')"
    width="900px"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <template v-if="order">
      <el-descriptions
        :column="3"
        border
      >
        <el-descriptions-item :label="t('crm.salesOrder.orderNo')">
          {{ order.orderNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.customer')">
          {{ order.customerName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.salesPersonName')">
          {{ order.salesPersonName || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.status')">
          <BaseStatusTag
            :value="order.status"
            :options="SALES_ORDER_STATUS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.paymentTerms')">
          {{ order.paymentTerms }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.deliveryDate')">
          {{ order.deliveryDate }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.currency')">
          {{ order.currencyCode }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.createdAt')">
          {{ order.createdAt }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.updatedAt')">
          {{ order.updatedAt }}
        </el-descriptions-item>
        <el-descriptions-item
          v-if="order.remark"
          :label="t('crm.salesOrder.remark')"
          :span="3"
        >
          {{ order.remark }}
        </el-descriptions-item>
      </el-descriptions>
      <el-divider content-position="left">
        {{ t('crm.salesOrder.items') }}
      </el-divider>
      <el-table
        :data="order.items"
        border
      >
        <el-table-column
          type="index"
          width="50"
          align="center"
        />
        <el-table-column
          prop="productName"
          :label="t('crm.salesOrder.productName')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="spec"
          :label="t('crm.salesOrder.spec')"
          width="120"
        />
        <el-table-column
          prop="price"
          :label="t('crm.salesOrder.price')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            {{ order.currencySymbol }}{{ row.price.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="quantity"
          :label="t('crm.salesOrder.quantity')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="taxRateCode"
          :label="t('crm.salesOrder.taxRateCode')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="amount"
          :label="t('crm.salesOrder.amount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ order.currencySymbol }}{{ row.amount.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="taxAmount"
          :label="t('crm.salesOrder.taxAmount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            {{ order.currencySymbol }}{{ row.taxAmount.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="totalAmount"
          :label="t('crm.salesOrder.totalAmount')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            <span class="text-primary">{{ order.currencySymbol }}{{ row.totalAmount.toFixed(2) }}</span>
          </template>
        </el-table-column>
      </el-table>
      <el-divider content-position="left">
        {{ t('crm.salesOrder.subtotalAmount') }}
      </el-divider>
      <el-descriptions
        :column="3"
        border
      >
        <el-descriptions-item :label="t('crm.salesOrder.subtotalAmount')">
          {{ order.currencySymbol }}{{ order.subtotalAmount.toFixed(2) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.taxAmount')">
          {{ order.currencySymbol }}{{ order.taxAmount.toFixed(2) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.salesOrder.totalAmount')">
          <span class="total-amount">{{ order.currencySymbol }}{{ order.totalAmount.toFixed(2) }}</span>
        </el-descriptions-item>
      </el-descriptions>
    </template>
    <template #footer>
      <el-button @click="handleClose(false)">
        {{ t('common.close') }}
      </el-button>
    </template>
  </el-dialog>
</template>
<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { SalesOrder } from '@/types/crm'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'
defineProps<{
  visible: boolean
  order: SalesOrder | null
}>()
const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()
const { t } = useI18n()
const SALES_ORDER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.salesOrder.statusDraft', type: 'info' },
  confirmed: { label: 'crm.salesOrder.statusConfirmed', type: 'warning' },
  shipped: { label: 'crm.salesOrder.statusShipped', type: '' },
  completed: { label: 'crm.salesOrder.statusCompleted', type: 'success' },
  cancelled: { label: 'crm.salesOrder.statusCancelled', type: 'danger' },
}
const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}
</script>
<style scoped lang="scss">
.text-primary { color: var(--el-color-primary); font-weight: 600; }
.total-amount {
  color: var(--el-color-danger);
  font-weight: 700;
  font-size: 16px;
}
</style>
