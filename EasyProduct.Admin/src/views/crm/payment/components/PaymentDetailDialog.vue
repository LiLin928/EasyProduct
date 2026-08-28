<template>
  <el-dialog
    :model-value="visible"
    :title="t('crm.payment.detail')"
    width="640px"
    append-to-body
    @update:model-value="handleClose"
  >
    <template v-if="payment">
      <el-descriptions
        :column="2"
        border
      >
        <el-descriptions-item :label="t('crm.payment.paymentNo')">
          {{ payment.paymentNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.payment.type')">
          <BaseStatusTag
            :value="payment.type"
            :options="TYPE_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.payment.orderType')">
          {{ payment.orderType === 'sales' ? t('crm.payment.orderTypeSales') : t('crm.payment.orderTypePurchase') }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.payment.orderNo')">
          {{ payment.orderNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.payment.partyName')">
          {{ payment.partyName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.payment.amount')">
          {{ formatMoney(payment.amount) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.payment.method')">
          <BaseStatusTag
            :value="payment.method"
            :options="METHOD_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.payment.status')">
          <BaseStatusTag
            :value="payment.status"
            :options="STATUS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item
          :label="t('crm.payment.remark')"
          :span="2"
        >
          {{ payment.remark }}
        </el-descriptions-item>
        <el-descriptions-item
          :label="t('crm.payment.createdAt')"
          :span="2"
        >
          {{ payment.createdAt }}
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
import type { Payment } from '@/types/crm'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

defineProps<{
  visible: boolean
  payment: Payment | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const { t } = useI18n()

const TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  receipt: { label: 'crm.payment.typeReceipt', type: 'success' },
  payment: { label: 'crm.payment.typePayment', type: 'warning' },
}

const METHOD_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  cash: { label: 'crm.payment.methodCash', type: 'info' },
  bank: { label: 'crm.payment.methodBank', type: '' },
  wechat: { label: 'crm.payment.methodWechat', type: 'success' },
}

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  draft: { label: 'crm.payment.statusDraft', type: 'info' },
  confirmed: { label: 'crm.payment.statusConfirmed', type: 'success' },
  voided: { label: 'crm.payment.statusVoided', type: 'danger' },
}

const formatMoney = (val: number) => {
  return val.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}
</script>
