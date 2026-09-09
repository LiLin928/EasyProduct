<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? t('site.news.edit') : t('site.news.add')"
    width="80%"
    top="5vh"
    append-to-body
    @update:model-value="handleClose"
  >
    <el-form
      ref="formRef"
      :model="model"
      :rules="rules"
      label-width="120px"
      :disabled="saving"
    >
      <el-form-item
        :label="t('site.news.newsTitle')"
        prop="title"
      >
        <el-input
          v-model="model.title"
          :placeholder="t('site.news.form.titlePlaceholder')"
          maxlength="200"
          show-word-limit
        />
      </el-form-item>

      <el-form-item
        :label="t('site.news.titleEn')"
        prop="titleEn"
      >
        <el-input
          v-model="model.titleEn"
          :placeholder="t('site.news.form.titleEnPlaceholder')"
          maxlength="200"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item
            :label="t('site.news.category')"
            prop="categoryId"
          >
            <el-select
              v-model="model.categoryId"
              :placeholder="t('site.news.form.categoryPlaceholder')"
              clearable
              style="width: 100%"
            >
              <el-option
                v-for="cat in categoryList"
                :key="cat.id"
                :label="cat.name"
                :value="cat.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item :label="t('site.news.isTop')">
            <el-switch
              v-model="model.isTop"
              :active-value="1"
              :inactive-value="0"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item
        :label="t('site.news.summary')"
        prop="summary"
      >
        <el-input
          v-model="model.summary"
          type="textarea"
          :rows="2"
          :placeholder="t('site.news.form.summaryPlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.news.summaryEn')"
        prop="summaryEn"
      >
        <el-input
          v-model="model.summaryEn"
          type="textarea"
          :rows="2"
          :placeholder="t('site.news.form.summaryEnPlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.news.coverImage')"
        prop="coverImage"
      >
        <ImageUpload v-model="model.coverImage" />
      </el-form-item>

      <el-form-item
        :label="t('site.news.content')"
        prop="content"
      >
        <RichTextEditor
          v-model="model.content"
          height="400px"
          :placeholder="t('site.news.form.contentPlaceholder')"
        />
      </el-form-item>

      <el-form-item
        :label="t('site.news.contentEn')"
        prop="contentEn"
      >
        <RichTextEditor
          v-model="model.contentEn"
          height="300px"
          :placeholder="t('site.news.form.contentEnPlaceholder')"
        />
      </el-form-item>
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
import RichTextEditor from '@/components/common/RichTextEditor.vue'
import { createNews, updateNews, getNewsById } from '@/api/site/news'
import { getCategoryTree } from '@/api/site/category'
import type { ProductCategory } from '@/types/site'

interface Props {
  visible: boolean
  id?: string
  isEdit: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:visible': [value: boolean]
  'success': []
}>()

const { t } = useI18n()

const formRef = ref<FormInstance>()
const saving = ref(false)
const categoryList = ref<ProductCategory[]>([])

interface NewsFormModel {
  title: string
  titleEn: string
  summary: string
  summaryEn: string
  content: string
  contentEn: string
  coverImage: string
  categoryId: string
  isTop: 0 | 1
}

const model = reactive<NewsFormModel>({
  title: '',
  titleEn: '',
  summary: '',
  summaryEn: '',
  content: '',
  contentEn: '',
  coverImage: '',
  categoryId: '',
  isTop: 0,
})

const rules: FormRules = {
  title: [
    { required: true, message: t('site.news.form.titleRequired'), trigger: 'blur' },
  ],
}

const loadCategoryList = async (): Promise<void> => {
  try {
    const data = await getCategoryTree()
    categoryList.value = data
  } catch {
    categoryList.value = []
  }
}

const loadDetail = async (): Promise<void> => {
  if (!props.id) return
  try {
    const data = await getNewsById(props.id)
    model.title = data.title
    model.titleEn = data.titleEn
    model.summary = data.summary
    model.summaryEn = data.summaryEn
    model.content = data.content
    model.contentEn = data.contentEn
    model.coverImage = data.coverImage
    model.categoryId = data.categoryId
    model.isTop = data.isTop
  } catch {
    // handled by interceptor
  }
}

const handleSubmit = async (): Promise<void> => {
 const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  saving.value = true
  try {
    if (props.isEdit && props.id) {
      await updateNews(props.id, model)
      ElMessage.success(t('site.news.message.updateSuccess'))
    } else {
      await createNews(model)
      ElMessage.success(t('site.news.message.createSuccess'))
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
  model.summary = ''
  model.summaryEn = ''
  model.content = ''
  model.contentEn = ''
  model.coverImage = ''
  model.categoryId = ''
  model.isTop = 0
  emit('update:visible', false)
}

watch(
  () => props.visible,
  async (val) => {
    if (val) {
      await loadCategoryList()
      if (props.isEdit && props.id) {
        await loadDetail()
      }
    }
  },
)
</script>

<style scoped lang="scss">
</style>
