<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('crm.supplier.edit') : t('crm.supplier.add')"
    width="700px"
    top="5vh"
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
            :label="t('crm.supplier.code')"
            prop="code"
          >
            <el-input
              v-model="model.code"
              :placeholder="t('crm.supplier.form.codePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.supplier.name')"
            prop="name"
          >
            <el-input
              v-model="model.name"
              :placeholder="t('crm.supplier.form.namePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.supplier.contactPerson')"
            prop="contactPerson"
          >
            <el-input
              v-model="model.contactPerson"
              :placeholder="t('crm.supplier.form.contactPersonPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.supplier.phone')"
            prop="phone"
          >
            <el-input
              v-model="model.phone"
              :placeholder="t('crm.supplier.form.phonePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.supplier.email')"
            prop="email"
          >
            <el-input
              v-model="model.email"
              :placeholder="t('crm.supplier.form.emailPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.supplier.bankName')"
            prop="bankName"
          >
            <el-input
              v-model="model.bankName"
              :placeholder="t('crm.supplier.form.bankNamePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.supplier.bankAccount')"
            prop="bankAccount"
          >
            <el-input
              v-model="model.bankAccount"
              :placeholder="t('crm.supplier.form.bankAccountPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.supplier.status')"
            prop="status"
          >
            <el-radio-group v-model="model.status">
              <el-radio :value="1">
                {{ t('crm.supplier.statusActive') }}
              </el-radio>
              <el-radio :value="0">
                {{ t('crm.supplier.statusInactive') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.supplier.address')"
            prop="address"
          >
            <el-input
              v-model="model.address"
              :placeholder="t('crm.supplier.form.addressPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.supplier.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('crm.supplier.form.remarkPlaceholder')"
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
import { ref, reactive, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { createSupplier, updateSupplier } from '@/api/crm/supplier'
import type { Supplier } from '@/types/crm'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: Supplier
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive<Partial<Supplier>>({
  code: '',
  name: '',
  contactPerson: '',
  phone: '',
  email: '',
  address: '',
  bankName: '',
  bankAccount: '',
  status: 1,
  remark: '',
})

const rules: FormRules = {
  name: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  code: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.code = props.rowData.code
    model.name = props.rowData.name
    model.contactPerson = props.rowData.contactPerson
    model.phone = props.rowData.phone
    model.email = props.rowData.email
    model.address = props.rowData.address
    model.bankName = props.rowData.bankName
    model.bankAccount = props.rowData.bankAccount
    model.status = props.rowData.status
    model.remark = props.rowData.remark
  } else {
    model.code = ''
    model.name = ''
    model.contactPerson = ''
    model.phone = ''
    model.email = ''
    model.address = ''
    model.bankName = ''
    model.bankAccount = ''
    model.status = 1
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
        await updateSupplier(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createSupplier(model)
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
