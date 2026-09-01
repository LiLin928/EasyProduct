<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('workflow.definition.edit') : t('workflow.definition.add')"
    width="600px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
      :disabled="submitting"
    >
      <el-form-item
        :label="t('workflow.definition.name')"
        prop="name"
      >
        <el-input
          v-model="form.name"
          :placeholder="t('workflow.definition.namePlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('workflow.definition.code')"
        prop="code"
      >
        <el-input
          v-model="form.code"
          :placeholder="t('workflow.definition.codePlaceholder')"
          :disabled="isEdit"
        />
      </el-form-item>

      <el-form-item
        :label="t('workflow.definition.category')"
        prop="category"
      >
        <el-input
          v-model="form.category"
          :placeholder="t('workflow.definition.categoryPlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('workflow.definition.description')"
        prop="description"
      >
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="4"
          :placeholder="t('workflow.definition.descriptionPlaceholder')"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose(false)">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        :loading="submitting"
        @click="handleSubmit"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { createDefinition, updateDefinition } from '@/api/workflow/definition'
import type { WorkflowDefinition } from '@/types/workflow'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: WorkflowDefinition
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const submitting = ref(false)

const form = reactive({
  name: '',
  code: '',
  category: '',
  description: '',
})

const rules: FormRules = {
  name: [{ required: true, message: t('common.required'), trigger: 'blur' }],
  code: [{ required: true, message: t('common.required'), trigger: 'blur' }],
}

const handleClose = (val: boolean) => {
  emit('update:visible', val)
}

const resetForm = () => {
  form.name = ''
  form.code = ''
  form.category = ''
  form.description = ''
  formRef.value?.clearValidate()
}

watch(() => props.visible, (val) => {
  if (val) {
    if (props.isEdit && props.rowData) {
      form.name = props.rowData.name
      form.code = props.rowData.code ?? ''
      form.category = props.rowData.category ?? ''
      form.description = props.rowData.description ?? ''
    } else {
      resetForm()
    }
  }
})

const handleSubmit = async () => {
  if (!formRef.value) return
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    if (props.isEdit && props.rowData) {
      await updateDefinition(props.rowData.id, {
        name: form.name,
        code: form.code,
        category: form.category,
        description: form.description,
      })
    } else {
      await createDefinition({
        name: form.name,
        code: form.code,
        category: form.category,
        description: form.description,
      })
    }
    ElMessage.success(t('common.success'))
    emit('success')
    handleClose(false)
  } finally {
    submitting.value = false
  }
}
</script>
