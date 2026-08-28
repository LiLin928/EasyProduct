<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('site.banner.edit') : t('site.banner.add')"
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
        :label="t('site.banner.bannerTitle')"
        prop="title"
      >
        <el-input
          v-model="model.title"
          :placeholder="t('site.banner.form.titlePlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.banner.titleEn')"
        prop="titleEn"
      >
        <el-input
          v-model="model.titleEn"
          :placeholder="t('site.banner.form.titleEnPlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.banner.imageUrl')"
        prop="imageUrl"
      >
        <ImageUpload v-model="model.imageUrl" />
      </el-form-item>

      <el-form-item
        :label="t('site.banner.link')"
        prop="link"
      >
        <el-input
          v-model="model.link"
          :placeholder="t('site.banner.form.linkPlaceholder')"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('site.banner.sort')"
            prop="sort"
          >
            <el-input-number
              v-model="model.sort"
              :min="0"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item
            :label="t('site.banner.status')"
            prop="status"
          >
            <el-radio-group v-model="model.status">
              <el-radio value="enabled">
                {{ t('site.banner.statusEnabled') }}
              </el-radio>
              <el-radio value="disabled">
                {{ t('site.banner.statusDisabled') }}
              </el-radio>
            </el-radio-group>
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
import { createBanner, updateBanner } from '@/api/site/banner'
import type { BannerStatus, SiteBanner } from '@/types/site'

interface Props {
  visible: boolean
  id?: string
  isEdit: boolean
  rowData?: SiteBanner | null
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)

interface BannerFormModel {
  title: string
  titleEn: string
  imageUrl: string
  link: string
  sort: number
  status: BannerStatus
}

const model = reactive<BannerFormModel>({
  title: '',
  titleEn: '',
  imageUrl: '',
  link: '',
  sort: 1,
  status: 'enabled',
})

const rules: FormRules = {
  title: [
    { required: true, message: t('site.banner.form.titleRequired'), trigger: 'blur' },
  ],
  imageUrl: [
    { required: true, message: t('site.banner.form.imageUrlRequired'), trigger: 'change' },
  ],
}

const handleSubmit = async (): Promise<void> => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  saving.value = true
  try {
    if (props.isEdit && props.id) {
      await updateBanner(props.id, model)
      ElMessage.success(t('site.banner.message.updateSuccess'))
    } else {
      await createBanner(model)
      ElMessage.success(t('site.banner.message.createSuccess'))
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
  model.imageUrl = ''
  model.link = ''
  model.sort = 1
  model.status = 'enabled'
  emit('update:visible', false)
}

watch(
  () => props.visible,
  (val) => {
    if (val && props.isEdit && props.rowData) {
      const r = props.rowData as { title: string; titleEn: string; imageUrl: string; link: string; sort: number; status: BannerStatus }
      model.title = r.title
      model.titleEn = r.titleEn
      model.imageUrl = r.imageUrl
      model.link = r.link
      model.sort = r.sort
      model.status = r.status
    }
  },
)
</script>

<style scoped lang="scss">
</style>
