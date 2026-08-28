<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('report.datasource.edit') : t('report.datasource.add')"
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
            :label="t('report.datasource.name')"
            prop="name"
          >
            <el-input
              v-model="model.name"
              :placeholder="t('report.datasource.form.namePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.datasource.type')"
            prop="type"
          >
            <el-select
              v-model="model.type"
              :placeholder="t('report.datasource.form.typePlaceholder')"
              style="width: 100%"
            >
              <el-option
                v-for="opt in typeOptions"
                :key="opt.value"
                :label="t(opt.labelKey)"
                :value="opt.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="16">
          <el-form-item
            :label="t('report.datasource.host')"
            prop="host"
          >
            <el-input
              v-model="model.host"
              :placeholder="t('report.datasource.form.hostPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item
            :label="t('report.datasource.port')"
            prop="port"
          >
            <el-input-number
              v-model="model.port"
              :min="1"
              :max="65535"
              controls-position="right"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.datasource.database')"
            prop="database"
          >
            <el-input
              v-model="model.database"
              :placeholder="t('report.datasource.form.databasePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.datasource.username')"
            prop="username"
          >
            <el-input
              v-model="model.username"
              :placeholder="t('report.datasource.form.usernamePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('report.datasource.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('report.datasource.form.remarkPlaceholder')"
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
import { createDatasource, updateDatasource } from '@/api/report/datasource'
import { DATASOURCE_TYPE_OPTIONS } from '@/types/report'
import type { Datasource } from '@/types/report'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: Datasource
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const typeOptions = DATASOURCE_TYPE_OPTIONS

const model = reactive({
  name: '',
  type: 'mysql' as 'mysql' | 'postgresql' | 'sqlserver' | 'oracle',
  host: '127.0.0.1',
  port: 3306,
  database: '',
  username: '',
  password: '',
  remark: '',
})

const rules: FormRules = {
  name: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  type: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
  host: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.name = props.rowData.name
    model.type = props.rowData.type
    model.host = props.rowData.host
    model.port = props.rowData.port
    model.database = props.rowData.database
    model.username = props.rowData.username
    model.password = props.rowData.password
    model.remark = props.rowData.remark
  } else {
    model.name = ''
    model.type = 'mysql'
    model.host = '127.0.0.1'
    model.port = 3306
    model.database = ''
    model.username = ''
    model.password = ''
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
        await updateDatasource(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createDatasource(model)
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
