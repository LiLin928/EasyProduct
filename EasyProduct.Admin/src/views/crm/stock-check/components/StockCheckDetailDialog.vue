<template>
  <el-dialog
    :model-value="visible"
    :title="t('crm.stockCheck.detail')"
    width="800px"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <template v-if="check">
      <el-descriptions
        :column="3"
        border
        class="mb-16"
      >
        <el-descriptions-item :label="t('crm.stockCheck.checkNo')">
          {{ check.checkNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stockCheck.warehouse')">
          {{ check.warehouseName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stockCheck.checker')">
          {{ check.checker }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stockCheck.checkDate')">
          {{ check.checkDate }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stockCheck.status')">
          <BaseStatusTag
            :value="check.status"
            :options="STATUS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.stockCheck.remark')">
          {{ check.remark }}
        </el-descriptions-item>
      </el-descriptions>

      <el-table
        :data="check.items"
        border
        stripe
      >
        <el-table-column
          prop="skuCode"
          :label="t('crm.stockCheck.skuCode')"
          width="120"
        />
        <el-table-column
          prop="skuName"
          :label="t('crm.stockCheck.skuName')"
          min-width="160"
          show-overflow-tooltip
        />
        <el-table-column
          prop="spec"
          :label="t('crm.stockCheck.spec')"
          min-width="120"
          show-overflow-tooltip
        />
        <el-table-column
          prop="unit"
          :label="t('crm.stockCheck.unit')"
          width="80"
          align="center"
        />
        <el-table-column
          prop="systemQty"
          :label="t('crm.stockCheck.systemQty')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="countedQty"
          :label="t('crm.stockCheck.countedQty')"
          width="100"
          align="right"
        />
        <el-table-column
          prop="diff"
          :label="t('crm.stockCheck.diff')"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            <span :class="getDiffClass(row.diff)">
              {{ row.diff > 0 ? `+${row.diff}` : row.diff }}
            </span>
          </template>
        </el-table-column>
      </el-table>
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
 import type { StockCheck } from '@/types/crm'
 import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

defineProps<{
  visible: boolean
  check: StockCheck | null
}>()

 const emit = defineEmits<{
   (e: 'update:visible', value: boolean): void
 }>()

 const { t } = useI18n()

 const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
   draft: { label: 'crm.stockCheck.statusDraft', type: 'info' },
   counting: { label: 'crm.stockCheck.statusCounting', type: 'warning' },
   completed: { label: 'crm.stockCheck.statusCompleted', type: 'success' },
 }

 const getDiffClass = (diff: number): string => {
   if (diff > 0) return 'text-success'
   if (diff < 0) return 'text-danger'
   return ''
 }

 const handleClose = (val: boolean = false) => {
   emit('update:visible', val)
 }
 </script>

 <style scoped lang="scss">
 .mb-16 { margin-bottom: 16px; }
 .text-success { color: var(--el-color-success); font-weight: 600; }
 .text-danger { color: var(--el-color-danger); font-weight: 600; }
 </style>
