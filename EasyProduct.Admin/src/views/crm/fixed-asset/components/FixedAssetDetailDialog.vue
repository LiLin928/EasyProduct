<template>
  <el-dialog
    :model-value="visible"
    :title="t('crm.fixedAsset.detail')"
    width="800px"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <template v-if="asset">
      <el-descriptions
        :column="3"
        border
        class="mb-16"
      >
        <el-descriptions-item :label="t('crm.fixedAsset.assetNo')">
          {{ asset.assetNo }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.name')">
          {{ asset.name }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.category')">
          {{ asset.category }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.originalValue')">
          {{ formatMoney(asset.originalValue) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.purchaseDate')">
          {{ asset.purchaseDate }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.salvageValue')">
          {{ formatMoney(asset.salvageValue) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.usefulYears')">
          {{ asset.usefulYears }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.currentValue')">
          {{ formatMoney(asset.currentValue) }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('crm.fixedAsset.status')">
          <BaseStatusTag
            :value="asset.status"
            :options="STATUS_MAP"
          />
        </el-descriptions-item>
        <el-descriptions-item
          :label="t('crm.fixedAsset.remark')"
          :span="3"
        >
          {{ asset.remark }}
        </el-descriptions-item>
      </el-descriptions>

      <div class="depreciation-title">
        {{ t('crm.fixedAsset.depreciationRecords') }}
      </div>
      <el-table
        v-loading="depreciationLoading"
        :data="depreciations"
        border
        stripe
        max-height="300"
      >
        <el-table-column
          prop="period"
          :label="t('crm.fixedAsset.period')"
          width="120"
        />
        <el-table-column
          prop="depreciationAmount"
          :label="t('crm.fixedAsset.depreciationAmount')"
          width="150"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.depreciationAmount) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="accumulatedDepreciation"
          :label="t('crm.fixedAsset.accumulatedDepreciation')"
          min-width="180"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.accumulatedDepreciation) }}
          </template>
        </el-table-column>
        <el-table-column
          prop="currentValue"
          :label="t('crm.fixedAsset.currentValue')"
          width="150"
          align="right"
        >
          <template #default="{ row }">
            {{ formatMoney(row.currentValue) }}
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
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getFixedAssetDepreciations } from '@/api/crm/fixed-asset'
import type { FixedAsset, AssetDepreciation } from '@/types/crm'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const props = defineProps<{
  visible: boolean
  asset: FixedAsset | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const { t } = useI18n()

const STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  active: { label: 'crm.fixedAsset.statusActive', type: 'success' },
  scrapped: { label: 'crm.fixedAsset.statusScrapped', type: 'danger' },
}

const depreciations = ref<AssetDepreciation[]>([])
const depreciationLoading = ref(false)

const formatMoney = (val: number) => {
  return val.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

watch(() => props.visible, async (val) => {
  if (!val || !props.asset) return
  depreciationLoading.value = true
  try {
    depreciations.value = await getFixedAssetDepreciations(props.asset.id)
  } catch {
    depreciations.value = []
  } finally {
    depreciationLoading.value = false
  }
})

const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}
</script>

<style scoped lang="scss">
.mb-16 { margin-bottom: 16px; }
.depreciation-title {
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 12px;
  color: var(--el-text-color-primary);
}
</style>
