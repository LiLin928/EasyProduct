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
      },
      {
        path: 'dept',
        name: 'basic-dept',
        component: () => import('@/views/basic/dept/index.vue'),
        meta: { title: 'menu.basicDept', icon: 'OfficeBuilding' }
      },
      {
        path: 'dict',
        name: 'basic-dict',
        component: () => import('@/views/basic/dict/index.vue'),
        meta: { title: 'menu.basicDict', icon: 'Collection' }
      }
      // 批次 3~4 的路由后续追加（announcement/setting/profile）
    ]
  }
]