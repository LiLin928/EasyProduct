<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('ops.task.edit') : t('ops.task.add')"
    width="620px"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="110px"
      :disabled="saving"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('ops.task.taskName')"
            prop="taskName"
          >
            <el-input
              v-model="model.taskName"
              :placeholder="t('ops.task.form.taskNamePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('ops.task.taskGroup')"
            prop="taskGroup"
          >
            <el-select
              v-model="model.taskGroup"
              :placeholder="t('ops.task.form.taskGroupPlaceholder')"
              style="width: 100%"
            >
              <el-option
                label="system"
                value="system"
              />
              <el-option
                label="basic"
                value="basic"
              />
              <el-option
                label="crm"
                value="crm"
              />
              <el-option
                label="mall"
                value="mall"
              />
              <el-option
                label="site"
                value="site"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('ops.task.cron')"
            prop="cron"
          >
            <el-input
              v-model="model.cron"
              :placeholder="t('ops.task.form.cronPlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('ops.task.methodName')"
            prop="methodName"
          >
            <el-input
              v-model="model.methodName"
              :placeholder="t('ops.task.form.methodNamePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('ops.task.className')"
            prop="className"
          >
            <el-input
              v-model="model.className"
              :placeholder="t('ops.task.form.classNamePlaceholder')"
            />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item
            :label="t('ops.task.description')"
            prop="description"
          >
            <el-input
              v-model="model.description"
              type="textarea"
              :rows="2"
              :placeholder="t('ops.task.form.descriptionPlaceholder')"
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
import { createTask, updateTask } from '@/api/ops/task'
import type { Task } from '@/types/ops'

const props = defineProps<{
  visible: boolean
  isEdit: boolean
  rowData?: Task
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

const model = reactive({
  taskName: '',
  taskGroup: 'system',
  cron: '',
  className: '',
  methodName: 'Execute',
  description: '',
})

const rules: FormRules = {
  taskName: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  cron: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
  className: [{ required: true, message: () => t('common.required'), trigger: 'blur' }],
}

watch(() => props.visible, (val) => {
  if (!val) return
  if (props.isEdit && props.rowData) {
    model.taskName = props.rowData.taskName
    model.taskGroup = props.rowData.taskGroup
    model.cron = props.rowData.cron
    model.className = props.rowData.className
    model.methodName = props.rowData.methodName
    model.description = props.rowData.description
  } else {
    model.taskName = ''
    model.taskGroup = 'system'
    model.cron = ''
    model.className = ''
    model.methodName = 'Execute'
    model.description = ''
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
        await updateTask(props.rowData.id, model)
        ElMessage.success(t('common.success'))
      } else {
        await createTask(model)
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
