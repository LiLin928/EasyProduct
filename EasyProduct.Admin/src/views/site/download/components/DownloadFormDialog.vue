<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('site.download.edit') : t('site.download.add')"
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
      <el-form-item
        :label="t('site.download.downloadTitle')"
        prop="title"
      >
        <el-input
          v-model="model.title"
          :placeholder="t('site.download.form.titlePlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.download.titleEn')"
        prop="titleEn"
      >
        <el-input
          v-model="model.titleEn"
          :placeholder="t('site.download.form.titleEnPlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.download.fileUrl')"
        prop="fileUrl"
      >
        <el-input
          v-model="model.fileUrl"
          :placeholder="t('site.download.form.fileUrlPlaceholder')"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('site.download.fileSize')"
            prop="fileSize"
          >
            <el-input-number
              v-model="model.fileSize"
              :min="0"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('site.download.sort')"
            prop="sort"
          >
            <el-input-number
              v-model="model.sort"
              :min="0"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">
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
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { createDownload, updateDownload } from '@/api/site/download'
import type { SiteDownload, SiteDownloadStatus } from '@/types/site'

interface Props {
  visible: boolean
  isEdit: boolean
  rowData?: SiteDownload | null
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

interface DownloadFormModel {
  title: string
  titleEn: string
  fileUrl: string
  fileSize: number
  sort: number
  status: SiteDownloadStatus
}

const model = reactive<DownloadFormModel>({
  title: '',
  titleEn: '',
  fileUrl: '',
  fileSize: 0,
  sort: 1,
  status: 'draft',
})

const rules: FormRules = {
  title: [{ required: true, message: t('site.download.form.titleRequired'), trigger: 'blur' }],
  fileUrl: [{ required: true, message: t('site.download.form.fileUrlRequired'), trigger: 'blur' }],
}

const handleSubmit = async (): Promise<void> => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  saving.value = true
  try {
    if (props.isEdit && props.rowData) {
      await updateDownload(props.rowData.id, model)
      ElMessage.success(t('site.download.message.updateSuccess'))
    } else {
      await createDownload(model)
      ElMessage.success(t('site.download.message.createSuccess'))
    }
    emit('success')
    handleClose()
  } catch {
    // handled by interceptor
  } finally {
    saving.value = false
  }
}

const handleClose = (): void => {
  formRef.value?.resetFields()
  model.title = ''
  model.titleEn = ''
  model.fileUrl = ''
  model.fileSize = 0
  model.sort = 1
  model.status = 'draft'
  emit('update:visible', false)
}

watch(
  () => props.visible,
  (val) => {
    if (val && props.isEdit && props.rowData) {
      model.title = props.rowData.title
      model.titleEn = props.rowData.titleEn
      model.fileUrl = props.rowData.fileUrl
      model.fileSize = props.rowData.fileSize
      model.sort = props.rowData.sort
      model.status = props.rowData.status
    }
  },
)
</script>

<style scoped lang="scss">
</style>
