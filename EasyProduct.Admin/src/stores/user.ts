import { defineStore } from 'pinia'
import { ref } from 'vue'
import { login as loginApi } from '@/api/basic/auth'
import { clearTokens, getAccessToken, setTokens } from '@/utils/auth'

export const useUserStore = defineStore('user', () => {
  const token = ref(getAccessToken())
  const realName = ref('')
  const permissions = ref<string[]>([])

  async function login(userName: string, password: string): Promise<void> {
    const data = await loginApi({ userName, password })
    setTokens(data.accessToken, data.refreshToken)
    token.value = data.accessToken
    realName.value = data.user.realName
    permissions.value = data.permissions
  }

  function logout(): void {
    clearTokens()
    token.value = ''
    realName.value = ''
    permissions.value = []
  }

  /** 按钮级权限判断（F2 usePermission/v-permission 复用此逻辑） */
  function has(code: string): boolean {
    return permissions.value.includes('*') || permissions.value.includes(code)
  }

  return { token, realName, permissions, login, logout, has }
})
