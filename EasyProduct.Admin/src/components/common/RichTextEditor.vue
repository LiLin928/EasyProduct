<!-- src/components/common/RichTextEditor.vue -->
<template>
  <div class="rich-text-editor">
    <Toolbar
      :editor="editorRef"
      :default-config="toolbarConfig"
      :mode="mode"
      class="rich-text-editor__toolbar"
    />
    <Editor
      v-model="valueHtml"
      :default-config="editorConfig"
      :mode="mode"
      :style="{ height }"
      @on-created="handleCreated"
      @on-change="handleChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, shallowRef, onBeforeUnmount, watch } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import '@wangeditor/editor/dist/css/style.css'
import { useLocale } from '@/composables/useLocale'
import { uploadFile } from '@/api/common/file'

interface Props {
  modelValue: string
  height?: string
  placeholder?: string
  disabled?: boolean
  mode?: 'default' | 'simple'
}

const props = withDefaults(defineProps<Props>(), {
  height: '300px',
  placeholder: '',
  disabled: false,
  mode: 'default',
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const { t } = useLocale()

const editorRef = shallowRef()
const valueHtml = ref<string>(props.modelValue)

watch(
  () => props.modelValue,
  (val) => {
    if (val !== valueHtml.value) {
      valueHtml.value = val
    }
  },
)

const toolbarConfig = {
  excludeKeys: ['group-video', 'fullScreen'],
}

const editorConfig = {
  placeholder: props.placeholder || t('common.richText.placeholder'),
  readOnly: props.disabled,
  MENU_CONF: {
    uploadImage: {
      async customUpload(file: File, insertFn: (url: string) => void) {
        try {
          const result = await uploadFile(file)
          insertFn(result.url)
        } catch {
          // 错误已由拦截器处理
        }
      },
    },
  },
}

const handleCreated = (editor: unknown) => {
  editorRef.value = editor
}

const handleChange = (editor: { getHtml: () => string }) => {
  emit('update:modelValue', editor.getHtml())
}

onBeforeUnmount(() => {
  const editor = editorRef.value as { destroy: () => void } | undefined
  editor?.destroy()
})
</script>

<style scoped lang="scss">
.rich-text-editor {
  border: 1px solid var(--ep-border);
  border-radius: $radius-md;
  overflow: hidden;

  &__toolbar {
    border-bottom: 1px solid var(--ep-border);
  }
}
</style>