<template>
  <el-dialog
    :model-value="visible"
    :title="t('workflow.todo.title')"
    width="600px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      :model="form"
      label-width="80px"
      :disabled="loading"
    >
      <el-form-item :label="t('workflow.task.comment')">
        <el-input
          v-model="form.comment"
          type="textarea"
          :rows="4"
          :placeholder="t('workflow.todo.commentPlaceholder')"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose(false)">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="danger"
        :loading="rejecting"
        @click="handleReject"
      >
        {{ t('workflow.todo.rejectBtn') }}
      </el-button>
      <el-button
        type="primary"
        :loading="approving"
        @click="handleApprove"
      >
        {{ t('workflow.todo.approveBtn') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { approveTask, rejectTask } from '@/api/workflow/todo'

const props = defineProps<{
  visible: boolean
  taskId: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const form = reactive({ comment: '' })
const approving = ref(false)
const rejecting = ref(false)

const loading = computed(() => approving.value || rejecting.value)

watch(() => props.visible, (val) => {
  if (val) {
    form.comment = ''
  }
})

const handleClose = (val: boolean) => {
  emit('update:visible', val)
}

const handleApprove = async () => {
  if (!form.comment.trim()) {
    ElMessage.warning(t('workflow.todo.commentRequired'))
    return
  }
  try {
    await ElMessageBox.confirm(t('workflow.todo.approveConfirm'), t('common.tips'), { type: 'warning' })
  } catch {
    return
  }
  approving.value = true
  try {
    await approveTask(props.taskId, { comment: form.comment })
    ElMessage.success(t('workflow.todo.approveSuccess'))
    emit('success')
    handleClose(false)
  } finally {
    approving.value = false
  }
}

const handleReject = async () => {
  if (!form.comment.trim()) {
    ElMessage.warning(t('workflow.todo.commentRequired'))
    return
  }
  try {
    await ElMessageBox.confirm(t('workflow.todo.rejectConfirm'), t('common.tips'), { type: 'warning' })
  } catch {
    return
  }
  rejecting.value = true
  try {
    await rejectTask(props.taskId, { comment: form.comment })
    ElMessage.success(t('workflow.todo.rejectSuccess'))
    emit('success')
    handleClose(false)
  } finally {
    rejecting.value = false
  }
}
</script>

