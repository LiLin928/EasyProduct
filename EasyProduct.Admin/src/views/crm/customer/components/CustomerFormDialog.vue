<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('crm.customer.edit') : t('crm.customer.add')"
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
            :label="t('crm.customer.code')"
            prop="code"
          >
            <el-input
              v-model="model.code"
              :placeholder="t('crm.customer.form.codePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.name')"
            prop="name"
          >
            <el-input
              v-model="model.name"
              :placeholder="t('crm.customer.form.namePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.type')"
            prop="type"
          >
            <el-select
              v-model="model.type"
              style="width: 100%"
            >
              <el-option
                :label="t('crm.customer.typeB2b')"
                value="b2b"
              />
              <el-option
                :label="t('crm.customer.typeRetail')"
                value="retail"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.source')"
            prop="source"
          >
            <el-select
              v-model="model.source"
              style="width: 100%"
            >
              <el-option
                :label="t('crm.customer.sourceInquiry')"
                value="inquiry"
              />
              <el-option
                :label="t('crm.customer.sourceRegister')"
                value="register"
              />
              <el-option
                :label="t('crm.customer.sourceManual')"
                value="manual"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.contactPerson')"
            prop="contactPerson"
          >
            <el-input
              v-model="model.contactPerson"
              :placeholder="t('crm.customer.form.contactPersonPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.phone')"
            prop="phone"
          >
            <el-input
              v-model="model.phone"
              :placeholder="t('crm.customer.form.phonePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.email')"
            prop="email"
          >
            <el-input
              v-model="model.email"
              :placeholder="t('crm.customer.form.emailPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.salesPersonName')"
            prop="salesPersonName"
          >
            <el-input
              v-model="model.salesPersonName"
              :placeholder="t('crm.customer.form.salesPersonNamePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.creditLimit')"
            prop="creditLimit"
          >
            <el-input-number
              v-model="model.creditLimit"
              :min="0"
              style="width: 100%"
              :placeholder="t('crm.customer.form.creditLimitPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.customer.status')"
            prop="status"
          >
            <el-radio-group v-model="model.status">
              <el-radio value="active">
                {{ t('crm.customer.statusActive') }}
              </el-radio>
              <el-radio value="inactive">
                {{ t('crm.customer.statusInactive') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.customer.address')"
            prop="address"
          >
            <el-input
              v-model="model.address"
              :placeholder="t('crm.customer.form.addressPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.customer.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('crm.customer.form.remarkPlaceholder')"
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
import { createCustomer, updateCustomer } from '@/api/crm/customer'
import type { Customer } from '@/types/crm'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: Customer
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
  type: 'b2b' as 'b2b' | 'retail',
  source: 'manual' as 'inquiry' | 'register' | 'manual',
  contactPerson: '',
  phone: '',
  email: '',
  address: '',
  salesPersonName: '',
  creditLimit: 0,
  status: 'active' as 'active' | 'inactive',
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
    model.type = props.rowData.type
    model.source = props.rowData.source
    model.contactPerson = props.rowData.contactPerson
    model.phone = props.rowData.phone
    model.email = props.rowData.email
    model.address = props.rowData.address
    model.salesPersonName = props.rowData.salesPersonName
    model.creditLimit = props.rowData.creditLimit
    model.status = props.rowData.status
    model.remark = props.rowData.remark
  } else {
    model.code = ''
    model.name = ''
    model.type = 'b2b'
    model.source = 'manual'
    model.contactPerson = ''
    model.phone = ''
    model.email = ''
    model.address = ''
    model.salesPersonName = ''
    model.creditLimit = 0
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
        await updateCustomer(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createCustomer(model)
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
