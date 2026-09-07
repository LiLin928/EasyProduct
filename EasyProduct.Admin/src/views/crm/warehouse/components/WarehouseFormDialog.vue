<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('crm.warehouse.edit') : t('crm.warehouse.add')"
    width="600px"
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
            :label="t('crm.warehouse.code')"
            prop="code"
          >
            <el-input
              v-model="model.code"
              :placeholder="t('crm.warehouse.form.codePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.warehouse.name')"
            prop="name"
          >
            <el-input
              v-model="model.name"
              :placeholder="t('crm.warehouse.form.namePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.warehouse.manager')"
            prop="manager"
          >
            <el-input
              v-model="model.manager"
              :placeholder="t('crm.warehouse.form.managerPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.warehouse.phone')"
            prop="phone"
          >
            <el-input
              v-model="model.phone"
              :placeholder="t('crm.warehouse.form.phonePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.warehouse.address')"
            prop="address"
          >
            <el-input
              v-model="model.address"
              :placeholder="t('crm.warehouse.form.addressPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('crm.warehouse.status')"
            prop="status"
          >
            <el-radio-group v-model="model.status">
              <el-radio value="active">
                {{ t('crm.warehouse.statusActive') }}
              </el-radio>
              <el-radio value="inactive">
                {{ t('crm.warehouse.statusInactive') }}
              </el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('crm.warehouse.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('crm.warehouse.form.remarkPlaceholder')"
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
 import { createWarehouse, updateWarehouse } from '@/api/crm/warehouse'
 import type { Warehouse } from '@/types/crm'

 const props = defineProps<{
   visible: boolean
   isEdit: boolean
   rowData?: Warehouse
 }>()

 const emit = defineEmits<{
   (e: 'update:visible', value: boolean): void
   (e: 'success'): void
 }>()

 const { t } = useI18n()

 const formRef = ref<FormInstance>()
 const saving = ref(false)

 const model = reactive<Partial<Warehouse>>({
   code: '',
   name: '',
   address: '',
   manager: '',
   phone: '',
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
     model.address = props.rowData.address
     model.manager = props.rowData.manager
     model.phone = props.rowData.phone
     model.status = props.rowData.status
     model.remark = props.rowData.remark
   } else {
     model.code = ''
     model.name = ''
     model.address = ''
     model.manager = ''
     model.phone = ''
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
         await updateWarehouse(props.rowData.id, model)
         ElMessage.success(t('common.success'))
       } else {
         await createWarehouse(model)
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
