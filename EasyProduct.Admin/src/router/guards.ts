import type { Router } from 'vue-router'
import { getAccessToken } from '@/utils/auth'
import { i18n } from '@/i18n'

const WHITE_LIST = ['/login']

export function setupGuards(router: Router): void {
  router.beforeEach((to) => {
    const token = getAccessToken()
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
