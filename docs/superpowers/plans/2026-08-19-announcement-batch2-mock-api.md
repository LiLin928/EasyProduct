# 公告管理 - 批次 2：管理端 Mock API 实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现公告管理的管理端 Mock API，支持完整的 CRUD 和状态流转逻辑

**Architecture:** Node.js + Express，遵循 EasyProduct Mock 规范

**Tech Stack:** Node.js, Express, mockjs

---

## 文件结构

**创建文件：**
- `mock-server/data/announcement.js` - 公告 Mock 数据
- `mock-server/data/announcement-read.js` - 阅读记录 Mock 数据
- `mock-server/routes/admin/basic/announcement.js` - 管理端 Mock API 路由

**修改文件：**
- `mock-server/app.js` - 注册公告路由

---

## Task 1: 创建公告 Mock 数据

**Files:**
- Create: `mock-server/data/announcement.js`

- [ ] **Step 1: 创建公告 Mock 数据文件**

```javascript
// mock-server/data/announcement.js
module.exports = [
  {
    id: 'ann-001',
    title: '系统升级通知',
    content: '<p>尊敬的用户：</p><p>系统将于本周六（2026年8月23日）凌晨2:00-6:00进行升级维护，届时系统将暂停服务。请提前做好相关安排。</p><p>给您带来的不便，敬请谅解。</p>',
    type: 'all',
    level: 'important',
    targetRoleIds: null,
    isTop: true,
    topTime: '2026-08-19 09:00:00',
    publishTime: '2026-08-19 08:00:00',
    recallTime: null,
    status: 'published',
    attachments: [
      {
        name: '升级说明.pdf',
        url: '/uploads/announcements/upgrade-guide.pdf',
        size: 204800
      }
    ],
    creatorId: 'user-admin',
    creatorName: '系统管理员',
    createdAt: '2026-08-18 10:00:00',
    updatedAt: '2026-08-19 09:00:00'
  },
  {
    id: 'ann-002',
    title: '新功能上线公告',
    content: '<p>好消息！</p><p>商品管理模块新增以下功能：</p><ul><li>商品批量导入</li><li>库存预警提醒</li><li>销售数据分析</li></ul><p>欢迎体验！</p>',
    type: 'targeted',
    level: 'normal',
    targetRoleIds: ['role-admin', 'role-operator'],
    isTop: false,
    topTime: null,
    publishTime: '2026-08-18 14:00:00',
    recallTime: null,
    status: 'published',
    attachments: [],
    creatorId: 'user-operator',
    creatorName: '运营人员',
    createdAt: '2026-08-18 12:00:00',
    updatedAt: null
  },
  {
    id: 'ann-003',
    title: '紧急维护通知',
    content: '<p>系统正在进行紧急维护，预计维护时间30分钟。</p><p>给您带来的不便，敬请谅解。</p>',
    type: 'all',
    level: 'urgent',
    targetRoleIds: null,
    isTop: true,
    topTime: '2026-08-17 16:00:00',
    publishTime: '2026-08-17 15:00:00',
    recallTime: '2026-08-17 18:00:00',
    status: 'recalled',
    attachments: [],
    creatorId: 'user-admin',
    creatorName: '系统管理员',
    createdAt: '2026-08-17 14:00:00',
    updatedAt: '2026-08-17 18:00:00'
  },
  {
    id: 'ann-004',
    title: '春节放假通知',
    content: '<p>各位同事：</p><p>2026年春节放假安排如下：</p><p>放假时间：2026年2月7日（农历除夕）至2月13日（农历初六），共7天。</p><p>2月14日（农历初七）正常上班。</p><p>祝大家新春快乐！</p>',
    type: 'all',
    level: 'normal',
    targetRoleIds: null,
    isTop: false,
    topTime: null,
    publishTime: null,
    recallTime: null,
    status: 'draft',
    attachments: [],
    creatorId: 'user-admin',
    creatorName: '系统管理员',
    createdAt: '2026-08-19 09:30:00',
    updatedAt: null
  },
  {
    id: 'ann-005',
    title: '会员积分系统升级',
    content: '<p>会员积分系统将于下周进行升级，届时将推出以下新功能：</p><ul><li>积分商城</li><li>积分兑换优惠券</li><li>积分过期提醒</li></ul>',
    type: 'targeted',
    level: 'important',
    targetRoleIds: ['role-operator'],
    isTop: false,
    topTime: null,
    publishTime: '2026-08-16 10:00:00',
    recallTime: null,
    status: 'published',
    attachments: [
      {
        name: '积分规则.docx',
        url: '/uploads/announcements/points-rule.docx',
        size: 102400
      }
    ],
    creatorId: 'user-operator',
    creatorName: '运营人员',
    createdAt: '2026-08-15 16:00:00',
    updatedAt: null
  }
]
```

- [ ] **Step 2: 提交 Mock 数据文件**

```bash
git add mock-server/data/announcement.js
git commit -m "feat(mock): 添加公告 Mock 数据"
```

---

## Task 2: 创建阅读记录 Mock 数据

**Files:**
- Create: `mock-server/data/announcement-read.js`

- [ ] **Step 1: 创建阅读记录 Mock 数据文件**

```javascript
// mock-server/data/announcement-read.js
module.exports = [
  {
    id: 'read-001',
    announcementId: 'ann-001',
    userId: 'user-operator',
    readAt: '2026-08-19 10:30:00'
  },
  {
    id: 'read-002',
    announcementId: 'ann-001',
    userId: 'user-001',
    readAt: '2026-08-19 11:00:00'
  },
  {
    id: 'read-003',
    announcementId: 'ann-002',
    userId: 'user-operator',
    readAt: '2026-08-18 15:00:00'
  },
  {
    id: 'read-004',
    announcementId: 'ann-005',
    userId: 'user-operator',
    readAt: '2026-08-16 12:00:00'
  }
]
```

- [ ] **Step 2: 提交阅读记录 Mock 数据**

```bash
git add mock-server/data/announcement-read.js
git commit -m "feat(mock): 添加公告阅读记录 Mock 数据"
```

---

## Task 3: 创建管理端 Mock API 路由

**Files:**
- Create: `mock-server/routes/admin/basic/announcement.js`

- [ ] **Step 1: 创建 Mock API 路由文件**

```javascript
// mock-server/routes/admin/basic/announcement.js
const express = require('express')
const router = express.Router()
const announcements = require('../../../data/announcement')
const readRecords = require('../../../data/announcement-read')

/**
 * 获取公告列表
 */
router.get('/', (req, res) => {
  const {
    pageIndex = 1,
    pageSize = 10,
    title,
    type,
    level,
    status,
    isTop
  } = req.query

  let filtered = [...announcements]

  // 筛选条件
  if (title) {
    filtered = filtered.filter(item => item.title.includes(title))
  }
  if (type) {
    filtered = filtered.filter(item => item.type === type)
  }
  if (level) {
    filtered = filtered.filter(item => item.level === level)
  }
  if (status) {
    filtered = filtered.filter(item => item.status === status)
  }
  if (isTop !== undefined) {
    const topValue = isTop === 'true' || isTop === true
    filtered = filtered.filter(item => item.isTop === topValue)
  }

  // 排序：置顶优先，然后按发布时间倒序
  filtered.sort((a, b) => {
    if (a.isTop !== b.isTop) {
      return b.isTop ? 1 : -1
    }
    if (a.topTime && b.topTime) {
      return new Date(b.topTime) - new Date(a.topTime)
    }
    if (a.publishTime && b.publishTime) {
      return new Date(b.publishTime) - new Date(a.publishTime)
    }
    return new Date(b.createdAt) - new Date(a.createdAt)
  })

  // 分页
  const start = (pageIndex - 1) * pageSize
  const end = start + parseInt(pageSize)
  const list = filtered.slice(start, end)

  res.json({
    code: 200,
    message: 'success',
    data: {
      list,
      total: filtered.length
    },
    timestamp: Date.now()
  })
})

/**
 * 获取公告详情
 */
router.get('/:id', (req, res) => {
  const { id } = req.params
  const announcement = announcements.find(item => item.id === id)

  if (!announcement) {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  // 添加阅读统计
  const reads = readRecords.filter(item => item.announcementId === id)
  const readCount = reads.length
  const unreadCount = 10 // 假设总用户数

  res.json({
    code: 200,
    message: 'success',
    data: {
      ...announcement,
      readStats: {
        readCount,
        unreadCount,
        totalCount: readCount + unreadCount
      }
    },
    timestamp: Date.now()
  })
})

/**
 * 创建公告
 */
router.post('/', (req, res) => {
  const {
    title,
    content,
    type,
    level,
    targetRoleIds,
    attachments
  } = req.body

  // 验证必填字段
  if (!title || !content || !type || !level) {
    return res.json({
      code: 400,
      message: '参数不完整',
      data: null,
      timestamp: Date.now()
    })
  }

  // 验证定向公告必须选择角色
  if (type === 'targeted' && (!targetRoleIds || targetRoleIds.length === 0)) {
    return res.json({
      code: 400,
      message: '定向公告必须选择目标角色',
      data: null,
      timestamp: Date.now()
    })
  }

  const newId = `ann-${Date.now()}`
  const newAnnouncement = {
    id: newId,
    title,
    content,
    type,
    level,
    targetRoleIds: targetRoleIds || null,
    isTop: false,
    topTime: null,
    publishTime: null,
    recallTime: null,
    status: 'draft',
    attachments: attachments || [],
    creatorId: 'user-admin',
    creatorName: '系统管理员',
    createdAt: new Date().toISOString().replace('T', ' ').substring(0, 19),
    updatedAt: null
  }

  announcements.push(newAnnouncement)

  res.json({
    code: 200,
    message: 'success',
    data: { id: newId },
    timestamp: Date.now()
  })
})

/**
 * 更新公告
 */
router.put('/:id', (req, res) => {
  const { id } = req.params
  const {
    title,
    content,
    type,
    level,
    targetRoleIds,
    attachments
  } = req.body

  const index = announcements.findIndex(item => item.id === id)

  if (index === -1) {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  const announcement = announcements[index]

  // 验证状态：已发布的公告不能编辑
  if (announcement.status === 'published') {
    return res.json({
      code: 400,
      message: '已发布的公告不能编辑',
      data: null,
      timestamp: Date.now()
    })
  }

  // 更新公告
  announcements[index] = {
    ...announcement,
    title,
    content,
    type,
    level,
    targetRoleIds: targetRoleIds || null,
    attachments: attachments || [],
    updatedAt: new Date().toISOString().replace('T', ' ').substring(0, 19)
  }

  res.json({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  })
})

/**
 * 删除公告
 */
router.delete('/:id', (req, res) => {
  const { id } = req.params
  const index = announcements.findIndex(item => item.id === id)

  if (index === -1) {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  const announcement = announcements[index]

  // 验证状态：已发布的公告不能删除
  if (announcement.status === 'published') {
    return res.json({
      code: 400,
      message: '已发布的公告不能删除',
      data: null,
      timestamp: Date.now()
    })
  }

  // 删除公告
  announcements.splice(index, 1)

  // 删除相关阅读记录
  const readIndexs = []
  readRecords.forEach((item, idx) => {
    if (item.announcementId === id) {
      readIndexs.push(idx)
    }
  })
  readIndexs.reverse().forEach(idx => readRecords.splice(idx, 1))

  res.json({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  })
})

/**
 * 发布公告
 */
router.put('/:id/publish', (req, res) => {
  const { id } = req.params
  const index = announcements.findIndex(item => item.id === id)

  if (index === -1) {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  const announcement = announcements[index]

  // 验证状态：只能发布草稿或已撤回的公告
  if (announcement.status !== 'draft' && announcement.status !== 'recalled') {
    return res.json({
      code: 400,
      message: '只能发布草稿或已撤回的公告',
      data: null,
      timestamp: Date.now()
    })
  }

  // 发布公告
  const now = new Date().toISOString().replace('T', ' ').substring(0, 19)
  announcements[index] = {
    ...announcement,
    status: 'published',
    publishTime: now,
    recallTime: null,
    updatedAt: now
  }

  res.json({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  })
})

/**
 * 撤回公告
 */
router.put('/:id/recall', (req, res) => {
  const { id } = req.params
  const index = announcements.findIndex(item => item.id === id)

  if (index === -1) {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  const announcement = announcements[index]

  // 验证状态：只能撤回已发布的公告
  if (announcement.status !== 'published') {
    return res.json({
      code: 400,
      message: '只能撤回已发布的公告',
      data: null,
      timestamp: Date.now()
    })
  }

  // 撤回公告
  const now = new Date().toISOString().replace('T', ' ').substring(0, 19)
  announcements[index] = {
    ...announcement,
    status: 'recalled',
    recallTime: now,
    updatedAt: now
  }

  res.json({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  })
})

/**
 * 置顶/取消置顶
 */
router.put('/:id/top', (req, res) => {
  const { id } = req.params
  const { isTop } = req.body

  const index = announcements.findIndex(item => item.id === id)

  if (index === -1) {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  const announcement = announcements[index]

  // 验证状态：只能对已发布的公告进行置顶操作
  if (announcement.status !== 'published') {
    return res.json({
      code: 400,
      message: '只能对已发布的公告进行置顶操作',
      data: null,
      timestamp: Date.now()
    })
  }

  // 置顶/取消置顶
  const now = new Date().toISOString().replace('T', ' ').substring(0, 19)
  announcements[index] = {
    ...announcement,
    isTop,
    topTime: isTop ? now : null,
    updatedAt: now
  }

  res.json({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  })
})

module.exports = router
```

- [ ] **Step 2: 提交 Mock API 路由**

```bash
git add mock-server/routes/admin/basic/announcement.js
git commit -m "feat(mock): 添加公告管理端 Mock API 路由"
```

---

## Task 4: 注册 Mock 路由

**Files:**
- Modify: `mock-server/app.js`

- [ ] **Step 1: 在 app.js 中注册公告路由**

找到路由注册部分（通常在其他路由注册附近），添加：

```javascript
// 在其他路由注册附近添加
const announcementRouter = require('./routes/admin/basic/announcement')
app.use('/api/admin/basic/announcement', announcementRouter)
```

- [ ] **Step 2: 启动 Mock 服务器验证**

Run: `cd mock-server && pnpm dev`
Expected: 服务正常启动，无报错

- [ ] **Step 3: 测试 API 端点**

使用浏览器或 curl 测试：
```bash
curl http://localhost:7700/api/admin/basic/announcement?pageIndex=1&pageSize=10
```

Expected: 返回公告列表数据

- [ ] **Step 4: 提交路由注册修改**

```bash
git add mock-server/app.js
git commit -m "feat(mock): 注册公告管理 Mock 路由"
```

---

## 验收检查

- [ ] 运行 Mock 服务器并测试所有 API

测试清单：
1. ✅ GET /api/admin/basic/announcement - 列表查询
2. ✅ GET /api/admin/basic/announcement/:id - 详情查询
3. ✅ POST /api/admin/basic/announcement - 创建公告
4. ✅ PUT /api/admin/basic/announcement/:id - 更新公告
5. ✅ DELETE /api/admin/basic/announcement/:id - 删除公告
6. ✅ PUT /api/admin/basic/announcement/:id/publish - 发布公告
7. ✅ PUT /api/admin/basic/announcement/:id/recall - 撤回公告
8. ✅ PUT /api/admin/basic/announcement/:id/top - 置顶操作

- [ ] 提交批次 2 所有文件

```bash
git add .
git commit -m "feat(mock): 完成公告管理批次2 - 管理端 Mock API"
```

---

## 后续批次

批次 2 完成后，将继续：
- 批次 3：官网前端 + Mock
- 批次 4：小程序前端 + Mock