// src/data/admin/role.ts
import Mock from 'mockjs'
import { guid, isoTime } from '../../helpers/id.js'

export interface Role {
  id: string
  name: string
  code: string
  menuIds: string[]
  status: 'enabled' | 'disabled'
  sort: number
  remark: string
  createdAt: string
  updatedAt: string
}

export const ROLES: Role[] = Mock.mock({
  'list|5': [{
    id: '@guid',
    name: '@ctitle(4,8)',
    code: '@word(4,8)',
    'menuIds|1-5': ['@guid'],
    status: '@pick(["enabled", "disabled"])',
    'sort|1-10': 1,
    remark: '@csentence(10,20)',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }],
}).list

// 添加超级管理员角色
ROLES.unshift({
  id: guid(),
  name: '超级管理员',
  code: 'super_admin',
  menuIds: [],
  status: 'enabled',
  sort: 1,
  remark: '拥有所有权限',
  createdAt: isoTime(),
  updatedAt: isoTime(),
})
