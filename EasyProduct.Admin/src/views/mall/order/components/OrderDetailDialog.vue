<template>
  <el-dialog
    :model-value="visible"
    :title="t('mall.order.detail')"
    width="800px"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <template v-if="order">
      <el-descriptions
        :column="3"
        border
      >
        <el-descriptions-item :label="t('mall.order.orderNo')">
          {{ order.orderNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.memberName')">
          {{ order.memberName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.memberPhone')">
          {{ order.memberPhone }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.status')">
          <BaseStatusTag
            :value="order.status"
            :options="ORDER_STATUS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.paymentMethod')">
          {{ order.paymentMethod || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.createdAt')">
          {{ order.createdAt }}
        </el-descriptions-item>
        <el-descriptions-item
          v-if="order.remark"
          :label="t('mall.order.remark')"
          :span="3"
        >
          {{ order.remark }}
        </el-descriptions-item>
      </el-descriptions>

      <el-divider content-position="left">
        {{ t('mall.order.items') }}
      </el-divider>

      <el-table
        :data="order.items"
        border
      >
        <el-table-column
          prop="spuName"
          :label="t('mall.order.spuName')"
          min-width="180"
          show-overflow-tooltip
        />
        <el-table-column
          prop="specValues"
          :label="t('mall.order.specValues')"
          width="120"
        />
        <el-table-column
          prop="price"
          :label="t('mall.order.price')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            ¥{{ row.price.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="quantity"
          :label="t('mall.order.quantity')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="subtotal"
          :label="t('mall.order.subtotal')"
          width="120"
          align="right"
        >
          <template #default="{ row }">
            ¥{{ row.subtotal.toFixed(2) }}
          </template>
        </el-table-column>
      </el-table>

      <el-divider content-position="left">
        {{ t('mall.order.payAmount') }}
      </el-divider>

      <el-descriptions
        :column="2"
        border
      >
        <el-descriptions-item :label="t('mall.order.totalAmount')">
          ¥{{ order.totalAmount.toFixed(2) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.discountAmount')">
          -¥{{ order.discountAmount.toFixed(2) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.pointsAmount')">
          -¥{{ order.pointsAmount.toFixed(2) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.shippingFee')">
          +¥{{ order.shippingFee.toFixed(2) }}
        </el-descriptions-item>
        <el-descriptions-item
          v-if="order.couponName"
          :label="t('mall.order.couponName')"
        >
          {{ order.couponName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('mall.order.payAmount')">
          <span class="pay-amount">¥{{ order.payAmount.toFixed(2) }}</span>
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
import type { Order } from '@/types/mall'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

defineProps<{
  visible: boolean
  order: Order | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const { t } = useI18n()

const ORDER_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  pending: { label: 'mall.order.statusPending', type: 'warning' },
  paid: { label: 'mall.order.statusPaid', type: 'success' },
  shipped: { label: 'mall.order.statusShipped', type: '' },
  completed: { label: 'mall.order.statusCompleted', type: 'success' },
  cancelled: { label: 'mall.order.statusCancelled', type: 'info' },
  refunded: { label: 'mall.order.statusRefunded', type: 'danger' },
}

const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}
</script>

<style scoped lang="scss">
.pay-amount {
  color: var(--el-color-danger);
  font-weight: 700;
  font-size: 16px;
}
</style>
