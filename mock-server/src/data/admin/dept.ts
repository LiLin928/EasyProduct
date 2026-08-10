// src/data/admin/dept.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface Dept {
  id: string
  parentId: string
  name: string
  code: string
  sort: number
  status: 'enabled' | 'disabled'
  createdAt: string
  updatedAt: string
}

export const DEPTS: Dept[] = [
  {
    id: guid(),
    parentId: '0',
    name: '总公司',
    code: 'ROOT',
    sort: 1,
    status: 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  },
  {
    id: guid(),
    parentId: '0',
    name: '技术部',
    code: 'TECH',
    sort: 2,
    status: 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  },
  {
    id: guid(),
    parentId: '0',
    name: '销售部',
    code: 'SALES',
    sort: 3,
    status: 'enabled',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  },
]
