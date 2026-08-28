<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('report.columnTemplate.edit') : t('report.columnTemplate.add')"
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
            :label="t('report.columnTemplate.name')"
            prop="name"
          >
            <el-input
              v-model="model.name"
              :placeholder="t('report.columnTemplate.form.namePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.columnTemplate.field')"
            prop="field"
          >
            <el-input
              v-model="model.field"
              :placeholder="t('report.columnTemplate.form.fieldPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.columnTemplate.type')"
            prop="type"
          >
            <el-select
              v-model="model.type"
              :placeholder="t('report.columnTemplate.form.typePlaceholder')"
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
        <el-col :span="12">
          <el-form-item
            :label="t('report.columnTemplate.width')"
            prop="width"
          >
            <el-input-number
              v-model="model.width"
              :min="50"
              :max="500"
              controls-position="right"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.columnTemplate.format')"
            prop="format"
          >
            <el-input
              v-model="model.format"
              :placeholder="t('report.columnTemplate.form.formatPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('report.columnTemplate.sortable')"
            prop="sortable"
          >
            <el-switch v-model="model.sortable" />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('report.columnTemplate.remark')"
            prop="remark"
          >
            <el-input
              v-model="model.remark"
              type="textarea"
              :rows="2"
              :placeholder="t('report.columnTemplate.form.remarkPlaceholder')"
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
import { createColumnTemplate, updateColumnTemplate } from '@/api/report/column-template'
import { COLUMN_TYPE_OPTIONS } from '@/types/report'
import type { ColumnTemplate } from '@/types/report'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: ColumnTemplate
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const typeOptions = COLUMN_TYPE_OPTIONS

const model = reactive({
  name: '',
  field: '',
  type: 'string' as 'string' | 'number' | 'date' | 'currency',
  width: 120,
  format: '',
  sortable: false,
  remark: '',
})

const rules: FormRules = {
  name: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  field: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  type: [{ required: true, message: () => t('common.required'), trigger: 'change' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.name = props.rowData.name
    model.field = props.rowData.field
    model.type = props.rowData.type
    model.width = props.rowData.width
    model.format = props.rowData.format
    model.sortable = props.rowData.sortable
    model.remark = props.rowData.remark
  } else {
    model.name = ''
    model.field = ''
    model.type = 'string'
    model.width = 120
    model.format = ''
    model.sortable = false
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
        await updateColumnTemplate(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createColumnTemplate(model)
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
