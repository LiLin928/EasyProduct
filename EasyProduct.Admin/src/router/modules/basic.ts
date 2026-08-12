import type { RouteRecordRaw } from 'vue-router'

export const basicRoutes: RouteRecordRaw[] = [
  {
    path: '/basic',
    name: 'basic',
    redirect: '/basic/user',
    meta: { title: 'menu.basicRoot', icon: 'Setting' },
    children: [
      {
        path: 'user',
        name: 'basic-user',
        component: () => import('@/views/basic/user/index.vue'),
        meta: { title: 'menu.basicUser', icon: 'User' }
      },
      {
        path: 'role',
        name: 'basic-role',
        component: () => import('@/views/basic/role/index.vue'),
        meta: { title: 'menu.basicRole', icon: 'UserFilled' }
      },
      {
        path: 'menu',
        name: 'basic-menu',
        component: () => import('@/views/basic/menu/index.vue'),
        meta: { title: 'menu.basicMenu', icon: 'Menu' }
      }
      // 批次 2~4 的路由后续追加（dept/dict/announcement/setting/profile）
    ]
  }
]