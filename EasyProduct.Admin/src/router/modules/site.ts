import type { RouteRecordRaw } from 'vue-router'

export const siteRoutes: RouteRecordRaw[] = [
  {
    path: '/site',
    name: 'site',
    redirect: '/site/news',
    meta: { title: 'menu.siteRoot', icon: 'Monitor' },
    children: [
      {
        path: 'news',
        name: 'site-news',
        component: () => import('@/views/site/news/index.vue'),
        meta: { title: 'menu.siteNews', icon: 'Document' },
      },
      {
        path: 'category',
        name: 'site-category',
        component: () => import('@/views/site/category/index.vue'),
        meta: { title: 'menu.siteCategory', icon: 'Files' },
      },
      {
        path: 'banner',
        name: 'site-banner',
        component: () => import('@/views/site/banner/index.vue'),
        meta: { title: 'menu.siteBanner', icon: 'Picture' },
      },
      {
        path: 'video',
        name: 'site-video',
        component: () => import('@/views/site/video/index.vue'),
        meta: { title: 'menu.siteVideo', icon: 'VideoCamera' },
      },
      {
        path: 'download',
        name: 'site-download',
        component: () => import('@/views/site/download/index.vue'),
        meta: { title: 'menu.siteDownload', icon: 'Download' },
      },
      {
        path: 'about',
        name: 'site-about',
        component: () => import('@/views/site/about/index.vue'),
        meta: { title: 'menu.siteAbout', icon: 'InfoFilled' },
      },
      {
        path: 'inquiry',
        name: 'site-inquiry',
        component: () => import('@/views/site/inquiry/index.vue'),
        meta: { title: 'menu.siteInquiry', icon: 'ChatLineSquare' },
      },
      {
        path: 'contact',
        name: 'site-contact',
        component: () => import('@/views/site/contact/index.vue'),
        meta: { title: 'menu.siteContact', icon: 'Message' },
      },
    ],
  },
]
