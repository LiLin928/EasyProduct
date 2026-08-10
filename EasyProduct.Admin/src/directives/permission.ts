// src/directives/permission.ts
import type { Directive, DirectiveBinding } from 'vue'
import { useUserStore } from '@/stores/user'

/**
 * 权限指令：无权限时移除元素
 * 用法：v-permission="'user:create'" 或 v-permission="['user:create', 'user:edit']"
 */
export const permission: Directive<HTMLElement, string | string[]> = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    const userStore = useUserStore()
    const { value } = binding

    if (!value) return

    const codes = Array.isArray(value) ? value : [value]
    const hasPermission = codes.some((code) => userStore.has(code))

    if (!hasPermission) {
      el.parentNode?.removeChild(el)
    }
  },
}

/**
 * 注册权限指令
 */
export function setupPermissionDirective(app: import('vue').App): void {
  app.directive('permission', permission)
}
