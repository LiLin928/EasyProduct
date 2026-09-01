<!-- BaseFormDialog: el-dialog + el-form wrapper, eliminates form-dialog boilerplate -->
<template>
  <el-dialog
    :model-value="visible"
    :title="title"
    :width="width"
    :close-on-click-modal="closeOnClickModal"
    :close-on-press-escape="closeOnClickModal"
    append-to-body
    @update:model-value="handleVisibleChange"
    @closed="handleClosed"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      :label-width="labelWidth"
      :disabled="loading"
    >
      <slot />
    </el-form>
    <template
      v-if="$slots.footer"
      #footer
    >
      <slot name="footer" />
    </template>
    <template
      v-else
      #footer
    >
      <el-button @click="handleCancel">
        {{ t(cancelText) }}
      </el-button>
      <el-button
        type="primary"
        :loading="loading"
        @click="handleSubmit"
      >
        {{ t(submitText) }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import type { FormInstance, FormRules } from 'element-plus'

interface Props {
  visible: boolean
  title: string
  model: Record<string, unknown>
  rules?: FormRules
  width?: string
  labelWidth?: string | number
  loading?: boolean
  closeOnClickModal?: boolean
  cancelText?: string
  submitText?: string
}

const props = withDefaults(defineProps<Props>(), {
  rules: () => ({}),
  width: '600px',
  labelWidth: '100px',
  loading: false,
  closeOnClickModal: false,
  cancelText: 'common.cancel',
  submitText: 'common.confirm',
})

const emit = defineEmits<{
  'update:visible': [value: boolean]
  'submit': []
  'close': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()

/** Cancel button / close dialog */
const handleCancel = (): void => {
  emit('update:visible', false)
}

/** dialog visible change (overlay click / ESC / X button) */
const handleVisibleChange = (val: boolean): void => {
  emit('update:visible', val)
}

/** dialog close animation finished (parent resets form here) */
const handleClosed = (): void => {
  formRef.value?.clearValidate()
  emit('close')
}

/** Submit button: validate first, then emit submit (parent does API call) */
const handleSubmit = async (): Promise<void> => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  emit('submit')
}

/** Expose formRef for parent to call resetFields etc */
defineExpose({
  formRef,
  resetFields: () => formRef.value?.resetFields(),
  clearValidate: () => formRef.value?.clearValidate(),
})
</script>

<style scoped lang="scss">
:deep(.el-dialog__body) {
  max-height: 60vh;
  overflow-y: auto;
}
</style>
