<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('crm.currency.edit') : t('crm.currency.add')"
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
        :label="t('crm.currency.code')"
        prop="code"
      >
        <el-input
          v-model="model.code"
          :placeholder="t('crm.currency.form.codePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('crm.currency.name')"
        prop="name"
      >
        <el-input
          v-model="model.name"
          :placeholder="t('crm.currency.form.namePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('crm.currency.symbol')"
        prop="symbol"
      >
        <el-input
          v-model="model.symbol"
          :placeholder="t('crm.currency.form.symbolPlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('crm.currency.exchangeRate')"
        prop="exchangeRate"
      >
        <el-input-number
          v-model="model.exchangeRate"
          :min="0"
          :step="0.01"
          style="width: 100%"
          :placeholder="t('crm.currency.form.exchangeRatePlaceholder')"
        />
      </el-form-item>
      <el-form-item
        :label="t('crm.currency.isDefault')"
        prop="isDefault"
      >
        <el-switch v-model="model.isDefault" :active-value="1" :inactive-value="0" />
      </el-form-item>
      <el-form-item
        :label="t('crm.currency.status')"
        prop="status"
      >
        <el-radio-group v-model="model.status">
          <el-radio :value="1">
            {{ t('crm.currency.statusActive') }}
          </el-radio>
          <el-radio :value="0">
            {{ t('crm.currency.statusInactive') }}
          </el-radio>
        </el-radio-group>
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
import { createCurrency, updateCurrency } from '@/api/crm/currency'
import type { Currency } from '@/types/crm'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: Currency
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
  symbol: '',
  exchangeRate: 1,
  isDefault: 0,
  status: 1,
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
    model.symbol = props.rowData.symbol
    model.exchangeRate = props.rowData.exchangeRate
    model.isDefault = props.rowData.isDefault
    model.status = props.rowData.status
  } else {
    model.code = ''
    model.name = ''
    model.symbol = ''
    model.exchangeRate = 1
    model.isDefault = 0
    model.status = 1
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
        await updateCurrency(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createCurrency(model)
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
