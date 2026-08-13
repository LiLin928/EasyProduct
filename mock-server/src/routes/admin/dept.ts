// src/routes/admin/dept.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { DEPTS } from '../../data/admin/dept.js'
import { guid } from '../../helpers/id.js'

export const adminDeptRouter = Router()

/** 部门树 */
adminDeptRouter.get('/basic/dept/tree', (_req, res) => {
  res.json(ok(DEPTS))
})

/** 部门详情 */
adminDeptRouter.get('/basic/dept/:id', (req, res) => {
  const findDept = (list: any[], id: string): any => {
    for (const item of list) {
      if (item.id === id) return item
      if (item.children) {
        const found = findDept(item.children, id)
        if (found) return found
      }
    }
    return null
  }

  const dept = findDept(DEPTS, req.params.id)
  if (!dept) {
    res.json(fail('部门不存在', 404))
    return
  }
  res.json(ok(dept))
})

/** 部门成员列表 */
adminDeptRouter.get('/basic/dept/:id/users', (req, res) => {
  // 返回模拟成员数据
  res.json(ok([
    {
      id: guid(),
      userName: 'user001',
      realName: '张三',
      phone: '13800138000',
      email: 'zhangsan@company.com',
      status: 'enabled',
      roleNames: ['技术员'],
      deptId: req.params.id,
      deptName: '技术部',
      roleIds: [guid()],
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
    {
      id: guid(),
      userName: 'user002',
      realName: '李四',
      phone: '13800138001',
      email: 'lisi@company.com',
      status: 'disabled',
      roleNames: ['开发工程师'],
      deptId: req.params.id,
      deptName: '技术部',
      roleIds: [guid()],
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
  ]))
})

/** 新增部门 */
adminDeptRouter.post('/basic/dept', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

/** 编辑部门 */
adminDeptRouter.put('/basic/dept/:id', (req, res) => {
  res.json(ok({ id: req.params.id }, '更新成功'))
})

/** 删除部门 */
adminDeptRouter.delete('/basic/dept/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})