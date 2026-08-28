import type { RouteRecordRaw } from 'vue-router'

export const workflowRoutes: RouteRecordRaw[] = [
  {
    path: '/workflow',
    name: 'workflow',
    redirect: '/workflow/my-apply',
    meta: { title: 'menu.workflowRoot', icon: 'Share' },
    children: [
      {
        path: 'my-apply',
        name: 'workflow-my-apply',
        component: () => import('@/views/workflow/my-apply/index.vue'),
        meta: { title: 'menu.workflowMyApply', icon: 'EditPen' },
      },
      {
        path: 'publish',
        name: 'workflow-publish',
        component: () => import('@/views/workflow/publish/index.vue'),
        meta: { title: 'menu.workflowPublish', icon: 'Promotion' },
      },
      {
        path: 'todo',
        name: 'workflow-todo',
        component: () => import('@/views/workflow/todo/index.vue'),
        meta: { title: 'menu.workflowTodo', icon: 'Bell' },
      },
      {
        path: 'done',
        name: 'workflow-done',
        component: () => import('@/views/workflow/done/index.vue'),
        meta: { title: 'menu.workflowDone', icon: 'CircleCheck' },
      },
      {
        path: 'instance',
        name: 'workflow-instance',
        component: () => import('@/views/workflow/instance/index.vue'),
        meta: { title: 'menu.workflowInstance', icon: 'Document' },
      },
      {
        path: 'designer',
        name: 'workflow-designer',
        component: () => import('@/views/workflow/designer/index.vue'),
        meta: { title: 'menu.workflowDesigner', icon: 'SetUp' },
      },
    ],
  },
]
