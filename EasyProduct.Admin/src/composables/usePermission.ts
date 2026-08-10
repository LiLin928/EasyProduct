// src/composables/usePermission.ts
import { useUserStore } from '@/stores/user'

export interface UsePermissionReturn {
  has: (code: string) => boolean
}

/**
 * 权限判断 composable（签名对齐前端规范 2.4）
 */
export function usePermission(): UsePermissionReturn {
  const userStore = useUserStore()

  const has = (code: string): boolean => {
    return userStore.has(code)
  }

  return {
    has,
  }
}
