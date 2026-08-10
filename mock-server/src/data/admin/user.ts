// src/data/admin/user.ts
import Mock from 'mockjs'
import { guid, isoTime } from '../../helpers/id.js'

export interface AdminUser {
  id: string
  userName: string
  password: string
  realName: string
  email: string
  phone: string
  status: 'enabled' | 'disabled'
  deptId: string
  roleIds: string[]
  createdAt: string
  updatedAt: string
}

export const ADMIN_USERS: AdminUser[] = Mock.mock({
  'list|10': [{
    id: '@guid',
    userName: '@word(5,10)',
    password: '123456',
    realName: '@cname',
    email: '@email',
    phone: /^1[3-9]\d{9}$/,
    status: '@pick(["enabled", "disabled"])',
    deptId: '@guid',
    roleIds: () => [guid()],
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }],
}).list

// 添加默认管理员（覆盖第一条）
ADMIN_USERS[0] = {
  id: guid(),
  userName: 'admin',
  password: 'admin123',
  realName: '系统管理员',
  email: 'admin@example.com',
  phone: '13800138000',
  status: 'enabled',
  deptId: guid(),
  roleIds: [guid()],
  createdAt: isoTime(),
  updatedAt: isoTime(),
}
