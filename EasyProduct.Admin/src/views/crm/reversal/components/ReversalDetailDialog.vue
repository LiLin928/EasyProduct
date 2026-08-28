<template>
  <el-dialog
    :model-value="visible"
    :title="t('crm.reversal.detail')"
    width="850px"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <template v-if="reversal">
      <el-descriptions
        :column="3"
        border
        class="mb-16"
      >
        <el-descriptions-item :label="t('crm.reversal.reversalNo')">
          {{ reversal.reversalNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.type')">
          <BaseStatusTag
            :value="reversal.type"
            :options="TYPE_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.status')">
          <BaseStatusTag
            :value="reversal.status"
            :options="STATUS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.sourceOrderType')">
          {{ reversal.sourceOrderType }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.sourceOrderNo')">
          {{ reversal.sourceOrderNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.partyName')">
          {{ reversal.partyName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.amount')">
          {{ formatMoney(reversal.amount) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.operator')">
          {{ reversal.operator }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.reversal.createdAt')">
          {{ reversal.createdAt }}
        </el-descriptions-item>
        <el-descriptions-item
          :label="t('crm.reversal.reason')"
          :span="3"
        >
          {{ reversal.reason }}
        </el-descriptions-item>
      </el-descriptions>

      <div class="items-title">
        {{ t('crm.reversal.items') }}
      </div>
      <el-table
        :data="reversal.items"
        border
        stripe
        max-height="300"
      >
        <el-table-column
          prop="skuCode"
          :label="t('crm.reversal.skuCode')"
          width="140"
        />
        <el-table-column
          prop="skuName"
          :label="t('crm.reversal.skuName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="spec"
          :label="t('crm.reversal.spec')"
          width="120"
        />
        <el-table-column
          prop="unit"
          :label="t('crm.reversal.unit')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="quantity"
          :label="t('crm.reversal.quantity')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="amount"
          :label="t('crm.reversal.amount')"
          width="130"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="reason"
          :label="t('crm.reversal.reason')"
          min-width="140"
          show-overflow-tooltip
        />
      </el-table>

      <div class="total-amount">
        {{ t('crm.reversal.totalAmount') }}: {{ formatMoney(reversal.amount) }}
      </div>
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
import type { Reversal } from '@/types/crm'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

defineProps<{
  visible: boolean
  reversal: Reversal | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const { t } = useI18n()

const TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  mall_refund: { label: 'crm.reversal.typeMallRefund', type: 'warning' },
  sales_return: { label: 'crm.reversal.typeSalesReturn', type: 'danger' },
  purchase_return: { label: 'crm.reversal.typePurchaseReturn', type: 'info' },
  document_void: { label: 'crm.reversal.typeDocumentVoid', type: '' },
}

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.reversal.statusDraft', type: 'info' },
  submitted: { label: 'crm.reversal.statusSubmitted', type: 'warning' },
  approved: { label: 'crm.reversal.statusApproved', type: 'success' },
  rejected: { label: 'crm.reversal.statusRejected', type: 'danger' },
  executed: { label: 'crm.reversal.statusExecuted', type: '' },
}

const formatMoney = (val: number) => {
  return val.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}
</script>

<style scoped lang="scss">
.mb-16 { margin-bottom: 16px; }
.items-title {
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 12px;
  color: var(--el-text-color-primary);
}
.total-amount {
  margin-top: 16px;
  text-align: right;
  font-size: 15px;
  font-weight: 600;
  color: var(--el-color-danger);
}
</style>
