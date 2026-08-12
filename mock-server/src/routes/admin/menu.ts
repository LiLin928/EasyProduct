// src/routes/admin/menu.ts
import { Router } from 'express'
import { ok, fail } from '../../helpers/envelope.js'
import { MENU_TREE } from '../../data/basic.js'
import { guid } from '../../helpers/id.js'

export const adminMenuRouter = Router()

// 菜单树
adminMenuRouter.get('/basic/menu/tree', (_req, res) => res.json(ok(MENU_TREE)))

// 菜单详情
adminMenuRouter.get('/basic/menu/:id', (req, res) => {
  const findMenu = (menus: typeof MENU_TREE): typeof MENU_TREE[0] | undefined => {
    for (const menu of menus) {
      if (menu.id === req.params.id) return menu
      if (menu.children && menu.children.length > 0) {
        const found = findMenu(menu.children)
        if (found) return found
      }
    }
    return undefined
  }
  const menu = findMenu(MENU_TREE)
  if (!menu) {
    res.json(fail('菜单不存在', 404))
    return
  }
  res.json(ok(menu))
})

// 新增菜单
adminMenuRouter.post('/basic/menu', (req, res) => {
  res.json(ok({ id: guid() }, '创建成功'))
})

// 编辑菜单
adminMenuRouter.put('/basic/menu/:id', (req, res) => {
  res.json(ok(null, '更新成功'))
})

// 删除菜单
adminMenuRouter.delete('/basic/menu/:id', (req, res) => {
  res.json(ok(null, '删除成功'))
})

// 更新菜单排序
adminMenuRouter.post('/basic/menu/sort', (req, res) => {
  res.json(ok(null, '排序成功'))
})

// 更新菜单状态
adminMenuRouter.put('/basic/menu/:id/status', (req, res) => {
  res.json(ok(null, '状态更新成功'))
})

// 更新菜单可见性
adminMenuRouter.put('/basic/menu/:id/visible', (req, res) => {
  res.json(ok(null, '可见性更新成功'))
})