// src/composables/useForm.ts
import { ref, reactive } from 'vue'
import type { Ref, UnwrapNestedRefs } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'

export interface UseFormOptions<T> {
  defaultModel?: Partial<T>
  rules?: FormRules
  onSubmit?: (model: T) => Promise<void>
}

export interface UseFormReturn<T> {
  formRef: Ref<FormInstance | undefined>
  model: UnwrapNestedRefs<T>
  rules: FormRules
  validate: () => Promise<boolean>
  resetFields: () => void
  submitLoading: Ref<boolean>
  handleSubmit: () => Promise<void>
}

/**
 * 表单通用 composable（签名对齐前端规范 2.4）
 * @param options 配置项
 */
export function useForm<T extends Record<string, unknown>>(
  options: UseFormOptions<T> = {},
): UseFormReturn<T> {
  const { defaultModel = {}, rules = {}, onSubmit } = options

  const formRef = ref<FormInstance>()
  const model = reactive<T>((defaultModel || {}) as T) as UnwrapNestedRefs<T>
  const submitLoading = ref(false)

  const validate = async (): Promise<boolean> => {
    if (!formRef.value) return false
    try {
      await formRef.value.validate()
      return true
    } catch {
      return false
    }
  }

  const resetFields = (): void => {
    formRef.value?.resetFields()
  }

  const handleSubmit = async (): Promise<void> => {
    const valid = await validate()
    if (!valid) return

    if (!onSubmit) return

    submitLoading.value = true
    try {
      await onSubmit(model as T)
    } finally {
      submitLoading.value = false
    }
  }

  return {
    formRef,
    model,
    rules,
    validate,
    resetFields,
    submitLoading,
    handleSubmit,
  }
}