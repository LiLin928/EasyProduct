<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('crm.taxRate.edit') : t('crm.taxRate.add')"
    width="500px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="100px"
      :disabled="saving"
    >
      <el-form-item
        :label="t('crm.taxRate.code')"
        prop="code"
      >
        <el-input
          v-model="model.code"
          :placeholder="t('crm.taxRate.form.codePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('crm.taxRate.name')"
        prop="name"
      >
        <el-input
          v-model="model.name"
          :placeholder="t('crm.taxRate.form.namePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('crm.taxRate.rate')"
        prop="rate"
      >
        <el-input-number
          v-model="model.rate"
          :min="0"
          :max="100"
          :step="1"
          style="width: 100%"
          :placeholder="t('crm.taxRate.form.ratePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('crm.taxRate.status')"
        prop="status"
      >
        <el-radio-group v-model="model.status">
          <el-radio value="active">
            {{ t('crm.taxRate.statusActive') }}
          </el-radio>
          <el-radio value="inactive">
            {{ t('crm.taxRate.statusInactive') }}
          </el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item
        :label="t('crm.taxRate.remark')"
        prop="remark"
      >
        <el-input
          v-model="model.remark"
          type="textarea"
          :rows="2"
          :placeholder="t('crm.taxRate.form.remarkPlaceholder')"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose(false)">
        {{ t('common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        :loading="saving"
        @click="handleSubmit"
      >
        {{ t('common.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { createTaxRate, updateTaxRate } from '@/api/crm/tax-rate'
import type { TaxRate } from '@/types/crm'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: TaxRate
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive({
  code: '',
  name: '',
  rate: 0,
  status: 'active' as 'active' | 'inactive',
  remark: '',
})

const rules: FormRules = {
  code: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  name: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.code = props.rowData.code
    model.name = props.rowData.name
    model.rate = props.rowData.rate
    model.status = props.rowData.status
    model.remark = props.rowData.remark
  } else {
    model.code = ''
    model.name = ''
    model.rate = 0
    model.status = 'active'
    model.remark = ''
  }
  formRef.value?.clearValidate()
})

const handleClose = (val: boolean = false) => {
  emit('update:visible', val)
}

const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (!valid) return
    saving.value = true
    try {
      if (props.isEdit && props.rowData) {
        await updateTaxRate(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createTaxRate(model)
        ElMessage.success(t('common.success'))
      }
      emit('success')
      handleClose(false)
    } finally {
      saving.value = false
    }
  })
}
</script>
