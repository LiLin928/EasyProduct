// src/directives/permission.ts
import type { Directive, DirectiveBinding } from 'vue'
import { useUserStore } from '@/stores/user'
import { watch } from 'vue'

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

    // 检查权限函数
    const checkPermission = () => {
      const hasPermission = codes.some((code) => userStore.has(code))
      if (!hasPermission) {
        el.style.display = 'none'
      } else {
        el.style.display = ''
      }
    }

    // 立即检查一次
    checkPermission()

    // 监听权限变化（响应式）
    watch(() => userStore.permissions, checkPermission, { deep: true })
  },
}

/**
 * 注册权限指令
 */
export function setupPermissionDirective(app: import('vue').App): void {
  app.directive('permission', permission)
}
