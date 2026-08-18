<!-- src/components/common/ImageUpload.vue -->
<template>
  <div class="image-upload">
    <el-upload
      :show-file-list="false"
      :before-upload="handleBeforeUpload"
      :http-request="handleHttpRequest"
      :disabled="disabled"
      :accept="accept"
    >
      <div
        v-if="modelValue"
        class="image-upload__preview"
        :class="{ 'image-upload__preview--circle': circle }"
      >
        <img
          :src="modelValue"
          alt="preview"
        >
        <div class="image-upload__mask">
          <span>{{ t('common.upload.reupload') }}</span>
        </div>
      </div>
      <div
        v-else
        class="image-upload__placeholder"
        :class="{ 'image-upload__placeholder--circle': circle }"
      >
        <el-icon><Plus /></el-icon>
        <span>{{ t('common.upload.clickToUpload') }}</span>
      </div>
    </el-upload>
    <el-button
      v-if="modelValue && !disabled"
      link
      type="danger"
      size="small"
      class="image-upload__remove"
      @click="handleRemove"
    >
      {{ t('common.upload.remove') }}
    </el-button>
  </div>
</template>

<script setup lang="ts">
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { uploadFile } from '@/api/common/file'

interface Props {
  modelValue: string
  maxSize?: number // MB
  accept?: string
  disabled?: boolean
  circle?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  maxSize: 2,
  accept: 'image/*',
  disabled: false,
  circle: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const { t } = useLocale()

const handleBeforeUpload = (file: File): boolean => {
  const sizeMb = file.size / 1024 / 1024
  if (sizeMb > props.maxSize) {
    ElMessage.warning(t('common.upload.oversize', { size: props.maxSize }))
    return false
  }
  return true
}

const handleHttpRequest = async (options: { file: File }): Promise<void> => {
  try {
    const result = await uploadFile(options.file)
    emit('update:modelValue', result.url)
  } catch {
    // 错误已由拦截器处理
  }
}

const handleRemove = (): void => {
  emit('update:modelValue', '')
}
</script>

<style scoped lang="scss">
.image-upload {
  display: inline-block;

  &__preview,
  &__placeholder {
    width: 120px;
    height: 120px;
    border: 1px dashed var(--ep-border);
    border-radius: $radius-md;
    overflow: hidden;
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;

    &--circle {
      border-radius: 50%;
    }
  }

  &__preview {
    img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .image-upload__mask {
      position: absolute;
      inset: 0;
      background: rgba(0, 0, 0, 0.5);
      color: #fff;
      display: flex;
      align-items: center;
      justify-content: center;
      opacity: 0;
      transition: opacity 0.2s;
    }

    &:hover .image-upload__mask {
      opacity: 1;
    }
  }

  &__placeholder {
    color: var(--ep-text-secondary);
    flex-direction: column;
    gap: $spacing-xs;

    &--circle {
      border-radius: 50%;
    }
  }

  &__remove {
    margin-top: $spacing-xs;
  }
}
</style>
