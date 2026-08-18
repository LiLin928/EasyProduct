<!-- src/views/basic/config/components/ConfigFormDialog.vue -->
<template>
  <el-dialog
    :model-value="modelValue"
    :title="t('basic.config.form.addTitle')"
    width="500px"
    @update:model-value="emit('update:modelValue', $event)"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item
        :label="t('basic.config.form.key')"
        prop="key"
      >
        <el-input
          v-model="formData.key"
          :placeholder="t('basic.config.form.keyPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.config.form.label')"
        prop="label"
      >
        <el-input
          v-model="formData.label"
          :placeholder="t('basic.config.form.labelPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.config.form.value')"
        prop="value"
      >
        <el-input
          v-model="formData.value"
          :placeholder="t('basic.config.form.valuePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('basic.config.form.type')"
        prop="type"
      >
        <el-radio-group v-model="formData.type">
          <el-radio value="string">
            {{ t('basic.config.typeString') }}
          </el-radio>
          <el-radio value="number">
            {{ t('basic.config.typeNumber') }}
          </el-radio>
          <el-radio value="boolean">
            {{ t('basic.config.typeBoolean') }}
          </el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item
        :label="t('basic.config.form.remark')"
        prop="remark"
      >
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="3"
          :placeholder="t('basic.config.form.remarkPlaceholder')"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="emit('update:modelValue', false)">
        {{ t('basic.config.cancel') }}
      </el-button>
      <el-button
        type="primary"
        :loading="saving"
        @click="handleSave"
      >
        {{ t('common.button.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { createConfig } from '@/api/basic/config'
import type { SystemConfigCreateParams } from '@/types/basic'

const props = defineProps<{ modelValue: boolean }>()
const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()
const formRef = ref<FormInstance | null>(null)
const saving = ref(false)

const formData = reactive<SystemConfigCreateParams>({
  key: '',
  label: '',
  value: '',
  type: 'string',
  remark: '',
})

const formRules: FormRules = {
  key: [
    { required: true, message: () => t('basic.config.form.keyRequired'), trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9_]*$/, message: () => t('basic.config.form.keyFormat'), trigger: 'blur' },
  ],
  label: [{ required: true, message: () => t('basic.config.form.labelRequired'), trigger: 'blur' }],
  value: [{ required: true, message: () => t('basic.config.form.valueRequired'), trigger: 'blur' }],
  type: [{ required: true, message: () => t('basic.config.form.typeRequired'), trigger: 'change' }],
}

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      formData.key = ''
      formData.label = ''
      formData.value = ''
      formData.type = 'string'
      formData.remark = ''
    }
  },
)

const handleClose = () => {
  formRef.value?.resetFields()
}

const handleSave = async () => {
  if (!formRef.value) return
  try {
    await formRef.value.validate()
  } catch {
    return
  }
  saving.value = true
  try {
    await createConfig(formData)
    ElMessage.success(t('basic.config.message.addSuccess'))
    emit('update:modelValue', false)
    emit('success')
  } catch {
    // 错误已由拦截器处理
  } finally {
    saving.value = false
  }
}
</script>
