<template>
  <el-dialog
    :model-value="visible"
    :title="t('crm.stockRecord.detail')"
    width="600px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-descriptions
      v-if="record"
      :column="2"
      border
    >
      <el-descriptions-item :label="t('crm.stockRecord.warehouse')">
        {{ record.warehouseName }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.skuCode')">
        {{ record.skuCode }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.skuName')">
        {{ record.skuName }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.spec')">
        {{ record.spec }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.unit')">
        {{ record.unit }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.type')">
        <BaseStatusTag
          :value="record.type"
          :options="TYPE_MAP"
        />
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.sourceType')">
        <BaseStatusTag
          :value="record.sourceType"
          :options="SOURCE_MAP"
        />
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.sourceOrderNo')">
        {{ record.sourceOrderNo }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.quantity')">
        {{ record.quantity }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.operator')">
        {{ record.operator }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.createdAt')">
        {{ record.createdAt }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('crm.stockRecord.remark')">
        {{ record.remark }}
      </el-descriptions-item>
    </el-descriptions>
    <template #footer>
      <el-button @click="handleClose(false)">
        {{ t('common.close') }}
      </el-button>
    </template>
  </el-dialog>
</template>

 <script setup lang="ts">
 import { useI18n } from 'vue-i18n'
 import type { StockRecord } from '@/types/crm'
 import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

defineProps<{
  visible: boolean
  record: StockRecord | null
}>()

 const emit = defineEmits<{
   (e: 'update:visible', value: boolean): void
 }>()

 const { t } = useI18n()

 const TYPE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   in: { label: 'crm.stockRecord.typeIn', type: 'success' },
   out: { label: 'crm.stockRecord.typeOut', type: 'warning' },
 }

 const SOURCE_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   purchase_in: { label: 'crm.stockRecord.sourcePurchaseIn', type: 'success' },
   sales_out: { label: 'crm.stockRecord.sourceSalesOut', type: 'warning' },
   mall_out: { label: 'crm.stockRecord.sourceMallOut', type: '' },
   check_adjust: { label: 'crm.stockRecord.sourceCheckAdjust', type: 'info' },
   reversal_return: { label: 'crm.stockRecord.sourceReversalReturn', type: 'danger' },
 }

 const handleClose = (val: boolean = false) => {
   emit('update:visible', val)
 }
 </script>
