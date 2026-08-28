<template>
  <el-dialog
    :model-value="visible"
    :title="t('workflow.myApply.add')"
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
      <el-form-item :label="t('workflow.myApply.form.definition')" prop="definitionId">
        <el-select
          v-model="form.definitionId"
          :placeholder="t('workflow.myApply.form.definitionPlaceholder')"
          style="width: 100%"
        >
          <el-option
            v-for="item in definitionOptions"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item :label="t('workflow.myApply.form.title')" prop="title">
        <el-input
          v-model="form.title"
          :placeholder="t('workflow.myApply.form.titlePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('workflow.myApply.form.businessType')" prop="businessType">
        <el-input
          v-model="form.businessType"
          :placeholder="t('workflow.myApply.form.businessTypePlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('workflow.myApply.form.formData')" prop="formData">
        <el-input
          v-model="form.formData"
          type="textarea"
          :rows="4"
          :placeholder="t('workflow.myApply.form.formDataPlaceholder')"
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
import { ref, reactive, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { createApply, getDefinitionOptions } from '@/api/workflow/my-apply'

const props = defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const submitting = ref(false)
const definitionOptions = ref<Array<{ id: string; name: string; code: string; category: string }>>([])

const form = reactive({
  definitionId: '',
  title: '',
  businessType: '',
  formData: '',
})

const rules: FormRules = {
  definitionId: [{ required: true, message: t('common.required'), trigger: 'change' }],
  title: [{ required: true, message: t('common.required'), trigger: 'blur' }],
}

const handleClose = (val: boolean) => {
  emit('update:visible', val)
}

const resetForm = () => {
  form.definitionId = ''
  form.title = ''
  form.businessType = ''
  form.formData = ''
  formRef.value?.clearValidate()
}

watch(() => props.visible, (val) => {
  if (val) {
    resetForm()
  }
})

const handleSubmit = async () => {
  if (!formRef.value) return
  const valid = await formRef.value.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    let formDataObj: Record<string, unknown> = {}
    if (form.formData.trim()) {
      try {
        formDataObj = JSON.parse(form.formData)
      } catch {
        formDataObj = { value: form.formData }
      }
    }
    await createApply({
      definitionId: form.definitionId,
      title: form.title,
      businessType: form.businessType,
      formData: formDataObj,
    })
    ElMessage.success(t('common.success'))
    emit('success')
    handleClose(false)
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  getDefinitionOptions().then((res) => {
    definitionOptions.value = res
  })
})
</script>
