<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('crm.invoice.edit') : t('crm.invoice.add')"
    width="640px"
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
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('crm.invoice.type')"
            prop="type"
          >
            <el-radio-group v-model="model.type">
              <el-radio value="output">
                {{ t('crm.invoice.typeOutput') }}
              </el-radio>
              <el-radio value="input">
                {{ t('crm.invoice.typeInput') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.invoice.orderType')"
            prop="orderType"
          >
            <el-radio-group v-model="model.orderType">
              <el-radio value="sales">
                {{ t('crm.invoice.orderTypeSales') }}
              </el-radio>
              <el-radio value="purchase">
                {{ t('crm.invoice.orderTypePurchase') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.invoice.orderNo')"
            prop="orderNo"
          >
            <el-input
              v-model="model.orderNo"
              :placeholder="t('crm.invoice.form.orderNoPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.invoice.partyName')"
            prop="partyName"
          >
            <el-input
              v-model="model.partyName"
              :placeholder="t('crm.invoice.form.partyNamePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.invoice.amount')"
            prop="amount"
          >
            <el-input-number
              v-model="model.amount"
              :min="0"
              :precision="2"
              :controls="false"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.invoice.taxRate')"
            prop="taxRate"
          >
            <el-input-number
              v-model="model.taxRate"
              :min="0"
              :max="100"
              :precision="2"
              :controls="false"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.invoice.issueDate')"
            prop="issueDate"
          >
            <el-date-picker
              v-model="model.issueDate"
              type="date"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.invoice.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('crm.invoice.form.remarkPlaceholder')"
            />
          </el-form-item>
        </el-col>
      </el-row>
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
import { ref, reactive, computed, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { createInvoice, updateInvoice } from '@/api/crm/invoice'
import type { Invoice } from '@/types/crm'

const props = defineProps<{
  visible: boolean
  invoice: Invoice | null
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const isEdit = computed(() => !!props.invoice)

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive({
  type: 'output' as 'output' | 'input',
  orderType: 'sales' as 'sales' | 'purchase',
  orderNo: '',
  partyName: '',
  amount: 0,
  taxRate: 13,
  issueDate: new Date().toISOString().slice(0, 10),
  remark: '',
})

const rules: FormRules = {
  partyName: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  amount: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.invoice) {
    model.type = props.invoice.type
    model.orderType = props.invoice.orderType
    model.orderNo = props.invoice.orderNo
    model.partyName = props.invoice.partyName
    model.amount = props.invoice.amount
    model.taxRate = props.invoice.taxRate
    model.issueDate = props.invoice.issueDate
    model.remark = props.invoice.remark
  } else {
    model.type = 'output'
    model.orderType = 'sales'
    model.orderNo = ''
    model.partyName = ''
    model.amount = 0
    model.taxRate = 13
    model.issueDate = new Date().toISOString().slice(0, 10)
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
      if (isEdit.value && props.invoice) {
        await updateInvoice(props.invoice.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createInvoice(model)
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
