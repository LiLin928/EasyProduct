// src/data/admin/dept.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface Dept {
  id: string
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  leaderName?: string
  phone?: string
  email?: string
  fullPath?: string
  level?: number
  memberCount?: number
  description?: string
  children?: Dept[]
}

export const DEPTS: Dept[] = [
  {
    id: 'dept-root-001',
    parentId: '0',
    name: '总公司',
    code: 'ROOT',
    sort: 1,
    status: 'enabled',
    leaderName: '张总',
    phone: '13800138000',
    email: 'zhang@company.com',
    fullPath: '总公司',
    level: 1,
    memberCount: 5,
    description: '公司总部',
    children: [
      {
        id: 'dept-tech-001',
        parentId: 'dept-root-001',
        name: '技术部',
        code: 'TECH',
        sort: 1,
        status: 'enabled',
        leaderName: '李经理',
        phone: '13800138001',
        email: 'li@company.com',
        fullPath: '总公司/技术部',
        level: 2,
        memberCount: 15,
        description: '技术研发部门',
        children: [
          {
            id: 'dept-tech-fe-001',
            parentId: 'dept-tech-001',
            name: '前端组',
            code: 'TECH_FE',
            sort: 1,
            status: 'enabled',
            fullPath: '总公司/技术部/前端组',
            level: 3,
            memberCount: 5,
            description: '前端开发团队',
          },
          {
            id: 'dept-tech-be-001',
            parentId: 'dept-tech-001',
            name: '后端组',
            code: 'TECH_BE',
            sort: 2,
            status: 'enabled',
            fullPath: '总公司/技术部/后端组',
            level: 3,
            memberCount: 8,
            description: '后端开发团队',
          },
        ],
      },
      {
        id: 'dept-sales-001',
        parentId: 'dept-root-001',
        name: '销售部',
        code: 'SALES',
        sort: 2,
        status: 'enabled',
        leaderName: '王经理',
        phone: '13800138002',
        email: 'wang@company.com',
        fullPath: '总公司/销售部',
        level: 2,
        memberCount: 10,
        description: '市场销售部门',
      },
    ],
  },
]