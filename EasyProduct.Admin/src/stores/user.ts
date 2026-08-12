import { defineStore } from 'pinia'
import { ref } from 'vue'
import { login as loginApi, getUserInfo as getUserInfoApi } from '@/api/basic/auth'
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

  /** 恢复用户信息（刷新页面后调用） */
  async function restoreUserInfo(): Promise<void> {
    if (!token.value) return

    try {
      const data = await getUserInfoApi()
      realName.value = data.user.realName
      permissions.value = data.permissions
    } catch (error) {
      // 恢复失败，清空登录状态
      logout()
    }
  }

  /** 按钮级权限判断（F2 usePermission/v-permission 复用此逻辑） */
  function has(code: string): boolean {
    return permissions.value.includes('*') || permissions.value.includes(code)
  }

  return { token, realName, permissions, login, logout, restoreUserInfo, has }
})