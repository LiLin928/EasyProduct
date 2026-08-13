// src/api/basic/dept.ts
import { get, post, put, del } from '@/utils/request'
import type { Dept, DeptCreateParams, DeptUpdateParams, User } from '@/types/basic'

/** 部门树 */
export const getDeptTree = () =>
  get<Dept[]>('/api/admin/basic/dept/tree')

/** 部门详情 */
export const getDeptDetail = (id: string) =>
  get<Dept>(`/api/admin/basic/dept/${id}`)

/** 部门成员列表 */
export const getDeptUsers = (deptId: string) =>
  get<User[]>(`/api/admin/basic/dept/${deptId}/users`)

/** 新增部门 */
export const createDept = (data: DeptCreateParams) =>
  post<{ id: string }>('/api/admin/basic/dept', data)

/** 编辑部门 */
export const updateDept = (id: string, data: DeptUpdateParams) =>
  put<{ id: string }>(`/api/admin/basic/dept/${id}`, data)

/** 删除部门 */
export const deleteDept = (id: string) =>
  del<null>(`/api/admin/basic/dept/${id}`)