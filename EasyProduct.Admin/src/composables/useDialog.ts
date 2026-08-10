// src/composables/useDialog.ts
import { ref } from 'vue'
import type { Ref } from 'vue'

export interface UseDialogReturn<T> {
  visible: Ref<boolean>
  payload: Ref<T | undefined>
  isEdit: Ref<boolean>
  open: (data?: T) => void
  close: () => void
}

/**
 * 弹窗通用 composable（签名对齐前端规范 2.4）
 */
export function useDialog<T = unknown>(): UseDialogReturn<T> {
  const visible = ref(false)
  const payload = ref<T>() as Ref<T | undefined>
  const isEdit = ref(false)

  const open = (data?: T): void => {
    if (data) {
      payload.value = data
      isEdit.value = true
    } else {
      payload.value = undefined
      isEdit.value = false
    }
    visible.value = true
  }

  const close = (): void => {
    visible.value = false
    payload.value = undefined
    isEdit.value = false
  }

  return {
    visible,
    payload,
    isEdit,
    open,
    close,
  }
}