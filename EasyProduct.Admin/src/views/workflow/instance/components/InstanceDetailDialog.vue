<template>
  <el-dialog
    :model-value="visible"
    :title="t('workflow.instance.detail')"
    width="800px"
    append-to-body
    @update:model-value="handleClose"
  >
    <template v-if="instance">
      <el-descriptions :column="2" border>
        <el-descriptions-item :label="t('workflow.instance.title')">
          {{ instance.title }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.definitionName')">
          {{ instance.definitionName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.businessId')">
          {{ instance.businessId }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.businessType')">
          {{ instance.businessType }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.applicantName')">
          {{ instance.applicantName }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.currentNode')">
          {{ instance.currentNode || '-' }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.status')">
          <BaseStatusTag :value="instance.status" :options="INSTANCE_STATUS_MAP" />
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.createdAt')">
          {{ instance.createdAt }}
        </el-descriptions-item>
        <el-descriptions-item :label="t('workflow.instance.finishedAt')">
          {{ instance.finishedAt || '-' }}
        </el-descriptions-item>
      </el-descriptions>

      <el-divider content-position="left">
        {{ t('workflow.instance.history') }}
      </el-divider>

      <el-timeline v-loading="historyLoading">
        <el-timeline-item
          v-for="h in history"
          :key="h.id"
          :timestamp="h.createdAt"
          :type="getTimelineType(h.action)"
        >
          <div class="history-item">
            <span class="history-item__node">{{ h.nodeName }}</span>
            <el-tag :type="getActionTagType(h.action)" size="small">
              {{ t(getActionLabel(h.action)) }}
            </el-tag>
            <span class="history-item__assignee">{{ h.assigneeName }}</span>
          </div>
          <p v-if="h.comment" class="history-item__comment">{{ h.comment }}</p>
        </el-timeline-item>
      </el-timeline>
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
import { getInstanceById, getInstanceHistory } from '@/api/workflow/instance'
import type { WorkflowInstance, WorkflowHistory } from '@/types/workflow'
import BaseStatusTag from '@/components/common/BaseStatusTag.vue'

const props = defineProps<{
  visible: boolean
  instanceId: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
}>()

const { t } = useI18n()

const INSTANCE_STATUS_MAP: Record<string, { label: string; type: '' | 'success' | 'warning' | 'info' | 'danger' }> = {
  running: { label: 'workflow.instance.statusRunning', type: 'warning' },
  approved: { label: 'workflow.instance.statusApproved', type: 'success' },
  rejected: { label: 'workflow.instance.statusRejected', type: 'danger' },
  cancelled: { label: 'workflow.instance.statusCancelled', type: 'info' },
}

const ACTION_LABEL_MAP: Record<string, string> = {
  submit: 'workflow.history.actionSubmit',
  approve: 'workflow.history.actionApprove',
  reject: 'workflow.history.actionReject',
  transfer: 'workflow.history.actionTransfer',
  cancel: 'workflow.history.actionCancel',
}

const instance = ref<WorkflowInstance | null>(null)
const history = ref<WorkflowHistory[]>([])
const historyLoading = ref(false)

const handleClose = (val: boolean) => {
  emit('update:visible', val)
}

const getTimelineType = (action: string): 'primary' | 'success' | 'danger' | 'info' | 'warning' => {
  const map: Record<string, 'primary' | 'success' | 'danger' | 'info' | 'warning'> = {
    submit: 'info',
    approve: 'success',
    reject: 'danger',
    transfer: 'warning',
    cancel: 'info',
  }
  return map[action] ?? 'info'
}

const getActionTagType = (action: string): '' | 'success' | 'warning' | 'info' | 'danger' => {
  const map: Record<string, '' | 'success' | 'warning' | 'info' | 'danger'> = {
    submit: 'info',
    approve: 'success',
    reject: 'danger',
    transfer: 'warning',
    cancel: 'info',
  }
  return map[action] ?? 'info'
}

const getActionLabel = (action: string): string => {
  return ACTION_LABEL_MAP[action] ?? action
}

watch(() => props.visible, async (val) => {
  if (!val || !props.instanceId) return
  historyLoading.value = true
  try {
    instance.value = await getInstanceById(props.instanceId)
    history.value = await getInstanceHistory(props.instanceId)
  } finally {
    historyLoading.value = false
  }
})
</script>

<style scoped lang="scss">
.history-item {
  display: flex;
  align-items: center;
  gap: 8px;

  &__node {
    font-weight: 600;
  }

  &__assignee {
    color: var(--el-text-color-secondary);
  }

  &__comment {
    margin-top: 4px;
    color: var(--el-text-color-regular);
  }
}
