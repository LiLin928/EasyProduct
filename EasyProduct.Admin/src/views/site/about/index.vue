<template>
  <div class="about-page">
    <el-card v-loading="loading">
      <el-form
        ref="formRef"
        :model="model"
        label-width="100px"
        :disabled="saving"
      >
        <el-form-item
          :label="t('site.about.aboutTitle')"
          prop="title"
        >
          <el-input
            v-model="model.title"
            :placeholder="t('site.about.form.titlePlaceholder')"
            style="max-width: 600px"
          />
        </el-form-item>

        <el-form-item
          :label="t('site.about.titleEn')"
          prop="titleEn"
        >
          <el-input
            v-model="model.titleEn"
            :placeholder="t('site.about.form.titleEnPlaceholder')"
            style="max-width: 600px"
          />
        </el-form-item>

        <el-form-item
          :label="t('site.about.content')"
          prop="content"
        >
          <RichTextEditor
            v-model="model.content"
            :placeholder="t('site.about.form.contentPlaceholder')"
            style="max-width: 800px"
          />
        </el-form-item>

        <el-form-item
          :label="t('site.about.contentEn')"
          prop="contentEn"
        >
          <RichTextEditor
            v-model="model.contentEn"
            :placeholder="t('site.about.form.contentEnPlaceholder')"
            style="max-width: 800px"
          />
        </el-form-item>

        <el-form-item>
          <el-button
            type="primary"
            :loading="saving"
            @click="handleSave"
          >
            {{ t('site.about.save') }}
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance } from 'element-plus'
import RichTextEditor from '@/components/common/RichTextEditor.vue'
import { getAbout, updateAbout } from '@/api/site/about'
import type { AboutParams } from '@/types/site'

const { t } = useI18n()

const formRef = ref<FormInstance>()
const loading = ref(false)
const saving = ref(false)

interface AboutFormModel {
  title: string
  titleEn: string
  content: string
  contentEn: string
}

const model = reactive<AboutFormModel>({
  title: '',
  titleEn: '',
  content: '',
  contentEn: '',
})

const loadAbout = async (): Promise<void> => {
  loading.value = true
  try {
    const data = await getAbout()
    model.title = data.title
    model.titleEn = data.titleEn
    model.content = data.content
    model.contentEn = data.contentEn
  } catch {
    ElMessage.error(t('site.about.message.loadFailed'))
  } finally {
    loading.value = false
  }
}

const handleSave = async (): Promise<void> => {
  saving.value = true
  try {
    const params: AboutParams = {
      title: model.title,
      titleEn: model.titleEn,
      content: model.content,
      contentEn: model.contentEn,
    }
    await updateAbout(params)
    ElMessage.success(t('site.about.message.updateSuccess'))
  } catch {
    // handled by interceptor
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  void loadAbout()
})
</script>

<style scoped lang="scss">
.about-page {
}
</style>
