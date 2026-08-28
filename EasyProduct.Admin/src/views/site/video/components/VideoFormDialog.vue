<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('site.video.edit') : t('site.video.add')"
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
        :label="t('site.video.videoTitle')"
        prop="title"
      >
        <el-input
          v-model="model.title"
          :placeholder="t('site.video.form.titlePlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.video.titleEn')"
        prop="titleEn"
      >
        <el-input
          v-model="model.titleEn"
          :placeholder="t('site.video.form.titleEnPlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.video.coverImage')"
        prop="coverImage"
      >
        <ImageUpload v-model="model.coverImage" />
      </el-form-item>

      <el-form-item
        :label="t('site.video.videoUrl')"
        prop="videoUrl"
      >
        <el-input
          v-model="model.videoUrl"
          :placeholder="t('site.video.form.videoUrlPlaceholder')"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('site.video.duration')"
            prop="duration"
          >
            <el-input-number
              v-model="model.duration"
              :min="0"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('site.video.sort')"
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
import ImageUpload from '@/components/common/ImageUpload.vue'
import { createVideo, updateVideo } from '@/api/site/video'
import type { SiteVideo, SiteVideoStatus } from '@/types/site'

interface Props {
  visible: boolean
  isEdit: boolean
  rowData?: SiteVideo | null
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

interface VideoFormModel {
  title: string
  titleEn: string
  coverImage: string
  videoUrl: string
  duration: number
  sort: number
  status: SiteVideoStatus
}

const model = reactive<VideoFormModel>({
  title: '',
  titleEn: '',
  coverImage: '',
  videoUrl: '',
  duration: 0,
  sort: 1,
  status: 'draft',
})

const rules: FormRules = {
  title: [{ required: true, message: t('site.video.form.titleRequired'), trigger: 'blur' }],
  videoUrl: [{ required: true, message: t('site.video.form.videoUrlRequired'), trigger: 'blur' }],
}

const handleSubmit = async (): Promise<void> => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  saving.value = true
  try {
    if (props.isEdit && props.rowData) {
      await updateVideo(props.rowData.id, model)
      ElMessage.success(t('site.video.message.updateSuccess'))
    } else {
      await createVideo(model)
      ElMessage.success(t('site.video.message.createSuccess'))
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
  model.coverImage = ''
  model.videoUrl = ''
  model.duration = 0
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
      model.coverImage = props.rowData.coverImage
      model.videoUrl = props.rowData.videoUrl
      model.duration = props.rowData.duration
      model.sort = props.rowData.sort
      model.status = props.rowData.status
    }
  },
)
</script>

<style scoped lang="scss">
</style>
