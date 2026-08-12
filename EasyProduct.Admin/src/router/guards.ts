import type { Router } from 'vue-router'
import { getAccessToken } from '@/utils/auth'
import { i18n } from '@/i18n'
import { useUserStore } from '@/stores/user'

const WHITE_LIST = ['/login']

export function setupGuards(router: Router): void {
  router.beforeEach(async (to) => {
    const token = getAccessToken()
    const userStore = useUserStore()

    // 如果有 token 但没有用户信息，尝试恢复
    if (token && !userStore.realName) {
      await userStore.restoreUserInfo()
    }

    if (!token && !WHITE_LIST.includes(to.path)) {
      return { path: '/login', query: { redirect: to.fullPath } }
    }
    if (token && to.path === '/login') return { path: '/desktop' }
    return true
  })

  router.afterEach((to) => {
    const key = to.meta.title as string | undefined
    const title = key ? i18n.global.t(key) : ''
    document.title = title ? `${title} - EasyProduct` : 'EasyProduct'
  })
}