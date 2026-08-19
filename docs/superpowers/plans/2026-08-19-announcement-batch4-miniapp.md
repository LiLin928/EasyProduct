# 公告管理 - 批次 4：小程序前端 + Mock API 实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现公告管理的小程序前端展示功能和 Mock API，支持阅读状态追踪

**Architecture:** 微信原生小程序 + TypeScript，需要 Member JWT 认证

**Tech Stack:** 微信小程序原生开发, TypeScript, 微信 API

---

## 文件结构

**创建文件：**
- `EasyProduct.MiniApp/src/types/announcement.ts` - 公告类型定义
- `EasyProduct.MiniApp/src/api/announcement.ts` - 公告 API（小程序）
- `EasyProduct.MiniApp/src/pages/announcement/index.ts` - 公告列表页逻辑
- `EasyProduct.MiniApp/src/pages/announcement/index.wxml` - 公告列表页模板
- `EasyProduct.MiniApp/src/pages/announcement/index.wxss` - 公告列表页样式
- `EasyProduct.MiniApp/src/pages/announcement/index.json` - 公告列表页配置
- `EasyProduct.MiniApp/src/pages/announcement/detail.ts` - 公告详情页逻辑
- `EasyProduct.MiniApp/src/pages/announcement/detail.wxml` - 公告详情页模板
- `EasyProduct.MiniApp/src/pages/announcement/detail.wxss` - 公告详情页样式
- `EasyProduct.MiniApp/src/pages/announcement/detail.json` - 公告详情页配置
- `mock-server/routes/app/announcement.js` - 小程序 Mock API

**修改文件：**
- `EasyProduct.MiniApp/src/app.json` - 添加公告页面路由
- `EasyProduct.MiniApp/src/i18n/zh-CN.json` - 添加公告国际化
- `EasyProduct.MiniApp/src/i18n/en-US.json` - 添加公告国际化

---

## Task 1: 创建小程序公告类型定义

**Files:**
- Create: `EasyProduct.MiniApp/src/types/announcement.ts`

- [ ] **Step 1: 创建公告类型定义文件**

```typescript
// src/types/announcement.ts

/**
 * 公告级别枚举
 */
export type AnnouncementLevel = 'normal' | 'important' | 'urgent'

/**
 * 附件信息
 */
export interface Attachment {
  name: string
  url: string
  size: number
}

/**
 * 用户公告实体（小程序展示，含阅读状态）
 */
export interface UserAnnouncement {
  id: string
  title: string
  content: string
  level: AnnouncementLevel
  isTop: boolean
  isRead: boolean
  publishTime: string
  attachments?: Attachment[]
}

/**
 * 公告查询参数
 */
export interface AnnouncementQuery {
  pageIndex: number
  pageSize: number
}
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.MiniApp && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 提交类型定义**

```bash
git add EasyProduct.MiniApp/src/types/announcement.ts
git commit -m "feat(miniapp): 添加小程序公告类型定义"
```

---

## Task 2: 创建小程序公告 API

**Files:**
- Create: `EasyProduct.MiniApp/src/api/announcement.ts`

- [ ] **Step 1: 创建公告 API 文件**

```typescript
// src/api/announcement.ts
import { get, post } from '@/utils/request'
import type { UserAnnouncement, AnnouncementQuery } from '@/types/announcement'

/**
 * 获取用户公告列表（含阅读状态）
 */
export const getUserAnnouncementList = (params: AnnouncementQuery) =>
  get<{ list: UserAnnouncement[]; total: number }>('/api/app/announcement', params)

/**
 * 获取用户公告详情
 */
export const getUserAnnouncementById = (id: string) =>
  get<UserAnnouncement>(`/api/app/announcement/${id}`)

/**
 * 标记公告已读
 */
export const markAnnouncementAsRead = (id: string) =>
  post<null>(`/api/app/announcement/${id}/read`)

/**
 * 获取未读公告数量
 */
export const getUnreadCount = () =>
  get<{ count: number }>('/api/app/announcement/unread-count')
```

- [ ] **Step 2: 运行类型检查验证**

Run: `cd EasyProduct.MiniApp && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 提交 API 层**

```bash
git add EasyProduct.MiniApp/src/api/announcement.ts
git commit -m "feat(miniapp): 添加小程序公告 API"
```

---

## Task 3: 添加小程序国际化文本

**Files:**
- Modify: `EasyProduct.MiniApp/src/i18n/zh-CN.json`
- Modify: `EasyProduct.MiniApp/src/i18n/en-US.json`

- [ ] **Step 1: 在 zh-CN.json 中添加公告国际化**

添加：

```json
"announcement": {
  "title": "公告中心",
  "unread": "未读",
  "level": {
    "normal": "普通",
    "important": "重要",
    "urgent": "紧急"
  },
  "publishTime": "发布时间",
  "attachments": "附件",
  "download": "下载",
  "noData": "暂无公告"
}
```

- [ ] **Step 2: 在 en-US.json 中添加公告国际化**

添加：

```json
"announcement": {
  "title": "Announcement",
  "unread": "Unread",
  "level": {
    "normal": "Normal",
    "important": "Important",
    "urgent": "Urgent"
  },
  "publishTime": "Publish Time",
  "attachments": "Attachments",
  "download": "Download",
  "noData": "No announcements"
}
```

- [ ] **Step 3: 提交国际化文件**

```bash
git add EasyProduct.MiniApp/src/i18n/zh-CN.json EasyProduct.MiniApp/src/i18n/en-US.json
git commit -m "feat(miniapp): 添加小程序公告国际化文本"
```

---

## Task 4: 创建小程序公告列表页

**Files:**
- Create: `EasyProduct.MiniApp/src/pages/announcement/index.ts`
- Create: `EasyProduct.MiniApp/src/pages/announcement/index.wxml`
- Create: `EasyProduct.MiniApp/src/pages/announcement/index.wxss`
- Create: `EasyProduct.MiniApp/src/pages/announcement/index.json`

- [ ] **Step 1: 创建公告列表页逻辑文件**

```typescript
// src/pages/announcement/index.ts
import { getUserAnnouncementList, getUnreadCount } from '@/api/announcement'
import type { UserAnnouncement } from '@/types/announcement'

interface AnnouncementPageData {
  announcements: UserAnnouncement[]
  loading: boolean
  hasMore: boolean
  pageIndex: number
  pageSize: number
  unreadCount: number
}

Page<AnnouncementPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    announcements: [],
    loading: false,
    hasMore: true,
    pageIndex: 1,
    pageSize: 10,
    unreadCount: 0
  },

  onLoad() {
    this.loadData()
    this.loadUnreadCount()
  },

  async loadData() {
    if (this.data.loading || !this.data.hasMore) return

    this.setData({ loading: true })

    try {
      const { list, total } = await getUserAnnouncementList({
        pageIndex: this.data.pageIndex,
        pageSize: this.data.pageSize
      })

      const hasMore = this.data.pageIndex * this.data.pageSize < total

      this.setData({
        announcements: [...this.data.announcements, ...list],
        hasMore,
        pageIndex: this.data.pageIndex + 1
      })
    } catch (error) {
      console.error('加载公告失败:', error)
    } finally {
      this.setData({ loading: false })
    }
  },

  async loadUnreadCount() {
    try {
      const { count } = await getUnreadCount()
      this.setData({ unreadCount: count })
    } catch (error) {
      console.error('获取未读数量失败:', error)
    }
  },

  onReachBottom() {
    this.loadData()
  },

  onPullDownRefresh() {
    this.setData({
      announcements: [],
      pageIndex: 1,
      hasMore: true
    })
    this.loadData()
    this.loadUnreadCount()
    wx.stopPullDownRefresh()
  },

  handleDetail(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({
      url: `/pages/announcement/detail?id=${id}`
    })
  },

  getLevelText(level: string): string {
    const levelMap: Record<string, string> = {
      normal: '普通',
      important: '重要',
      urgent: '紧急'
    }
    return levelMap[level] || '普通'
  },

  getLevelClass(level: string): string {
    const classMap: Record<string, string> = {
      normal: 'level-normal',
      important: 'level-important',
      urgent: 'level-urgent'
    }
    return classMap[level] || 'level-normal'
  },

  formatDate(dateStr: string): string {
    const date = new Date(dateStr)
    return `${date.getMonth() + 1}-${date.getDate()}`
  }
})
```

- [ ] **Step 2: 创建公告列表页模板文件**

```xml
<!-- src/pages/announcement/index.wxml -->
<view class="announcement-page">
  <view class="announcement-header">
    <text class="announcement-title">公告中心</text>
    <view wx:if="{{unreadCount > 0}}" class="unread-badge">{{unreadCount}}</view>
  </view>

  <view class="announcement-list">
    <block wx:for="{{announcements}}" wx:key="id">
      <view class="announcement-item" data-id="{{item.id}}" bindtap="handleDetail">
        <view class="item-header">
          <view class="item-left">
            <view wx:if="{{!item.isRead}}" class="unread-dot"></view>
            <view class="item-title">
              <text wx:if="{{item.level !== 'normal'}}" class="level-tag {{item.level}}">
                {{item.level === 'important' ? '重要' : '紧急'}}
              </text>
              {{item.title}}
            </view>
          </view>
          <text class="item-time">{{item.publishTime}}</text>
        </view>
      </view>
    </block>

    <view wx:if="{{loading}}" class="loading">加载中...</view>
    <view wx:if="{{!hasMore && announcements.length > 0}}" class="no-more">没有更多了</view>
    <view wx:if="{{!loading && announcements.length === 0}}" class="empty">暂无公告</view>
  </view>
</view>
```

- [ ] **Step 3: 创建公告列表页样式文件**

```css
/* src/pages/announcement/index.wxss */
.announcement-page {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.announcement-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px;
  background-color: #fff;
  border-bottom: 1px solid #eee;
}

.announcement-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
}

.unread-badge {
  background-color: #ff4d4f;
  color: #fff;
  font-size: 12px;
  padding: 2px 8px;
  border-radius: 10px;
}

.announcement-list {
  padding: 8px;
}

.announcement-item {
  margin-bottom: 8px;
  padding: 12px;
  background-color: #fff;
  border-radius: 8px;
}

.item-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.item-left {
  display: flex;
  align-items: center;
  flex: 1;
}

.unread-dot {
  width: 8px;
  height: 8px;
  background-color: #ff4d4f;
  border-radius: 50%;
  margin-right: 8px;
}

.item-title {
  font-size: 15px;
  color: #333;
  flex: 1;
}

.level-tag {
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 4px;
  margin-right: 8px;
  color: #fff;
}

.level-tag.important {
  background-color: #faad14;
}

.level-tag.urgent {
  background-color: #ff4d4f;
}

.item-time {
  font-size: 12px;
  color: #999;
}

.loading,
.no-more,
.empty {
  text-align: center;
  padding: 20px;
  color: #999;
  font-size: 14px;
}
```

- [ ] **Step 4: 创建公告列表页配置文件**

```json
{
  "navigationBarTitleText": "公告中心",
  "enablePullDownRefresh": true,
  "onReachBottomDistance": 50
}
```

- [ ] **Step 5: 提交公告列表页**

```bash
git add EasyProduct.MiniApp/src/pages/announcement/
git commit -m "feat(miniapp): 创建小程序公告列表页"
```

---

## Task 5: 创建小程序公告详情页

**Files:**
- Create: `EasyProduct.MiniApp/src/pages/announcement/detail.ts`
- Create: `EasyProduct.MiniApp/src/pages/announcement/detail.wxml`
- Create: `EasyProduct.MiniApp/src/pages/announcement/detail.wxss`
- Create: `EasyProduct.MiniApp/src/pages/announcement/detail.json`

- [ ] **Step 1: 创建公告详情页逻辑文件**

```typescript
// src/pages/announcement/detail.ts
import { getUserAnnouncementById, markAnnouncementAsRead } from '@/api/announcement'
import type { UserAnnouncement } from '@/types/announcement'

interface DetailPageData {
  announcement: UserAnnouncement | null
  loading: boolean
}

Page<DetailPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    announcement: null,
    loading: true
  },

  onLoad(options: { id: string }) {
    const { id } = options
    if (id) {
      this.loadDetail(id)
    }
  },

  async loadDetail(id: string) {
    this.setData({ loading: true })

    try {
      const announcement = await getUserAnnouncementById(id)
      this.setData({ announcement })

      // 自动标记已读
      if (!announcement.isRead) {
        await markAnnouncementAsRead(id)
      }
    } catch (error) {
      console.error('加载公告详情失败:', error)
      wx.showToast({
        title: '加载失败',
        icon: 'error'
      })
    } finally {
      this.setData({ loading: false })
    }
  },

  handleDownload(e: WechatMiniprogram.TouchEvent) {
    const { url } = e.currentTarget.dataset
    wx.downloadFile({
      url,
      success(res) {
        if (res.statusCode === 200) {
          wx.openDocument({
            filePath: res.tempFilePath,
            success() {
              console.log('打开文档成功')
            },
            fail(err) {
              console.error('打开文档失败:', err)
              wx.showToast({
                title: '打开失败',
                icon: 'error'
              })
            }
          })
        }
      },
      fail(err) {
        console.error('下载失败:', err)
        wx.showToast({
          title: '下载失败',
          icon: 'error'
        })
      }
    })
  },

  getLevelText(level: string): string {
    const levelMap: Record<string, string> = {
      normal: '普通',
      important: '重要',
      urgent: '紧急'
    }
    return levelMap[level] || '普通'
  },

  formatDate(dateStr: string): string {
    const date = new Date(dateStr)
    return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`
  },

  formatFileSize(size: number): string {
    if (size < 1024) {
      return `${size}B`
    } else if (size < 1024 * 1024) {
      return `${(size / 1024).toFixed(1)}KB`
    } else {
      return `${(size / (1024 * 1024)).toFixed(1)}MB`
    }
  }
})
```

- [ ] **Step 2: 创建公告详情页模板文件**

```xml
<!-- src/pages/announcement/detail.wxml -->
<view class="detail-page">
  <view wx:if="{{loading}}" class="loading">加载中...</view>

  <view wx:if="{{announcement && !loading}}" class="detail-content">
    <view class="detail-header">
      <view class="detail-title">
        <text wx:if="{{announcement.level !== 'normal'}}" class="level-tag {{announcement.level}}">
          {{announcement.level === 'important' ? '重要' : '紧急'}}
        </text>
        {{announcement.title}}
      </view>
      <view class="detail-meta">
        <text class="publish-time">发布时间：{{announcement.publishTime}}</text>
      </view>
    </view>

    <view class="detail-body">
      <rich-text nodes="{{announcement.content}}"></rich-text>
    </view>

    <view wx:if="{{announcement.attachments && announcement.attachments.length > 0}}" class="detail-attachments">
      <view class="attachments-title">附件</view>
      <view class="attachments-list">
        <view wx:for="{{announcement.attachments}}" wx:key="url" class="attachment-item">
          <view class="attachment-info">
            <text class="attachment-name">{{item.name}}</text>
            <text class="attachment-size">{{item.size}}</text>
          </view>
          <button
            class="download-btn"
            data-url="{{item.url}}"
            bindtap="handleDownload"
          >
            下载
          </button>
        </view>
      </view>
    </view>
  </view>

  <view wx:if="{{!loading && !announcement}}" class="empty">公告不存在</view>
</view>
```

- [ ] **Step 3: 创建公告详情页样式文件**

```css
/* src/pages/announcement/detail.wxss */
.detail-page {
  min-height: 100vh;
  background-color: #fff;
}

.loading,
.empty {
  text-align: center;
  padding: 100px 0;
  color: #999;
  font-size: 14px;
}

.detail-content {
  padding: 16px;
}

.detail-header {
  padding-bottom: 12px;
  border-bottom: 1px solid #eee;
  margin-bottom: 16px;
}

.detail-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  line-height: 1.5;
  margin-bottom: 8px;
}

.level-tag {
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 4px;
  margin-right: 8px;
  color: #fff;
}

.level-tag.important {
  background-color: #faad14;
}

.level-tag.urgent {
  background-color: #ff4d4f;
}

.detail-meta {
  font-size: 12px;
  color: #999;
}

.publish-time {
  margin-right: 16px;
}

.detail-body {
  font-size: 15px;
  color: #333;
  line-height: 1.8;
}

.detail-attachments {
  margin-top: 24px;
  padding-top: 16px;
  border-top: 1px solid #eee;
}

.attachments-title {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 12px;
}

.attachments-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.attachment-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  background-color: #f5f5f5;
  border-radius: 8px;
}

.attachment-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.attachment-name {
  font-size: 14px;
  color: #333;
}

.attachment-size {
  font-size: 12px;
  color: #999;
}

.download-btn {
  padding: 4px 12px;
  font-size: 12px;
  background-color: #1890ff;
  color: #fff;
  border-radius: 4px;
}
```

- [ ] **Step 4: 创建公告详情页配置文件**

```json
{
  "navigationBarTitleText": "公告详情"
}
```

- [ ] **Step 5: 提交公告详情页**

```bash
git add EasyProduct.MiniApp/src/pages/announcement/detail.*
git commit -m "feat(miniapp): 创建小程序公告详情页"
```

---

## Task 6: 创建小程序 Mock API

**Files:**
- Create: `mock-server/routes/app/announcement.js`

- [ ] **Step 1: 创建小程序 Mock API 路由文件**

```javascript
// mock-server/routes/app/announcement.js
const express = require('express')
const router = express.Router()
const announcements = require('../../data/announcement')
const readRecords = require('../../data/announcement-read')

// 模拟当前用户 ID（实际应从 JWT 解析）
const CURRENT_USER_ID = 'user-operator'

/**
 * 获取用户公告列表（含阅读状态）
 */
router.get('/', (req, res) => {
  const { pageIndex = 1, pageSize = 10 } = req.query

  // 仅返回已发布、未撤回的公告
  let published = announcements.filter(
    item => item.status === 'published'
  )

  // 根据用户角色过滤定向公告
  // 假设当前用户拥有 role-admin 和 role-operator 角色
  const userRoles = ['role-admin', 'role-operator']
  published = published.filter(item => {
    if (item.type === 'all') return true
    if (!item.targetRoleIds) return false
    return item.targetRoleIds.some(roleId => userRoles.includes(roleId))
  })

  // 添加阅读状态
  const listWithReadStatus = published.map(item => {
    const readRecord = readRecords.find(
      r => r.announcementId === item.id && r.userId === CURRENT_USER_ID
    )
    return {
      id: item.id,
      title: item.title,
      content: item.content,
      level: item.level,
      isTop: item.isTop,
      isRead: !!readRecord,
      publishTime: item.publishTime,
      attachments: item.attachments
    }
  })

  // 排序：置顶优先，然后按发布时间倒序
  listWithReadStatus.sort((a, b) => {
    if (a.isTop !== b.isTop) {
      return b.isTop ? 1 : -1
    }
    return new Date(b.publishTime) - new Date(a.publishTime)
  })

  // 分页
  const start = (pageIndex - 1) * pageSize
  const end = start + parseInt(pageSize)
  const list = listWithReadStatus.slice(start, end)

  res.json({
    code: 200,
    message: 'success',
    data: {
      list,
      total: listWithReadStatus.length
    },
    timestamp: Date.now()
  })
})

/**
 * 获取用户公告详情
 */
router.get('/:id', (req, res) => {
  const { id } = req.params
  const announcement = announcements.find(item => item.id === id)

  // 仅返回已发布的公告
  if (!announcement || announcement.status !== 'published') {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  // 添加阅读状态
  const readRecord = readRecords.find(
    r => r.announcementId === id && r.userId === CURRENT_USER_ID
  )

  res.json({
    code: 200,
    message: 'success',
    data: {
      id: announcement.id,
      title: announcement.title,
      content: announcement.content,
      level: announcement.level,
      isTop: announcement.isTop,
      isRead: !!readRecord,
      publishTime: announcement.publishTime,
      attachments: announcement.attachments
    },
    timestamp: Date.now()
  })
})

/**
 * 标记公告已读
 */
router.post('/:id/read', (req, res) => {
  const { id } = req.params
  const announcement = announcements.find(item => item.id === id)

  if (!announcement || announcement.status !== 'published') {
    return res.json({
      code: 404,
      message: '公告不存在',
      data: null,
      timestamp: Date.now()
    })
  }

  // 检查是否已读
  const existingRead = readRecords.find(
    r => r.announcementId === id && r.userId === CURRENT_USER_ID
  )

  if (!existingRead) {
    // 添加阅读记录
    readRecords.push({
      id: `read-${Date.now()}`,
      announcementId: id,
      userId: CURRENT_USER_ID,
      readAt: new Date().toISOString().replace('T', ' ').substring(0, 19)
    })
  }

  res.json({
    code: 200,
    message: 'success',
    data: null,
    timestamp: Date.now()
  })
})

/**
 * 获取未读公告数量
 */
router.get('/unread-count', (req, res) => {
  // 仅统计已发布、未撤回的公告
  let published = announcements.filter(
    item => item.status === 'published'
  )

  // 根据用户角色过滤
  const userRoles = ['role-admin', 'role-operator']
  published = published.filter(item => {
    if (item.type === 'all') return true
    if (!item.targetRoleIds) return false
    return item.targetRoleIds.some(roleId => userRoles.includes(roleId))
  })

  // 统计未读数量
  const unreadCount = published.filter(item => {
    return !readRecords.find(
      r => r.announcementId === item.id && r.userId === CURRENT_USER_ID
    )
  }).length

  res.json({
    code: 200,
    message: 'success',
    data: {
      count: unreadCount
    },
    timestamp: Date.now()
  })
})

module.exports = router
```

- [ ] **Step 2: 提交小程序 Mock API**

```bash
git add mock-server/routes/app/announcement.js
git commit -m "feat(mock): 添加小程序公告 Mock API"
```

---

## Task 7: 注册小程序 Mock 路由

**Files:**
- Modify: `mock-server/app.js`

- [ ] **Step 1: 在 app.js 中注册小程序公告路由**

找到路由注册部分，添加：

```javascript
// 小程序公告路由
const appAnnouncementRouter = require('./routes/app/announcement')
app.use('/api/app/announcement', appAnnouncementRouter)
```

- [ ] **Step 2: 重启 Mock 服务器验证**

Run: `cd mock-server && pnpm dev`
Expected: 服务正常启动

- [ ] **Step 3: 测试小程序 API**

测试：
```bash
curl http://localhost:7700/api/app/announcement?pageIndex=1&pageSize=10
curl http://localhost:7700/api/app/announcement/unread-count
```

Expected: 返回带阅读状态的公告列表和未读数量

- [ ] **Step 4: 提交路由注册**

```bash
git add mock-server/app.js
git commit -m "feat(mock): 注册小程序公告 Mock 路由"
```

---

## Task 8: 添加小程序页面路由

**Files:**
- Modify: `EasyProduct.MiniApp/src/app.json`

- [ ] **Step 1: 在 app.json 中添加公告页面路由**

在 pages 数组中添加：

```json
"pages/announcement/index",
"pages/announcement/detail"
```

- [ ] **Step 2: 运行小程序类型检查**

Run: `cd EasyProduct.MiniApp && pnpm type-check`
Expected: PASS

- [ ] **Step 3: 提交路由配置**

```bash
git add EasyProduct.MiniApp/src/app.json
git commit -m "feat(miniapp): 添加小程序公告页面路由"
```

---

## 验收检查

- [ ] 运行小程序完整验证

```bash
cd EasyProduct.MiniApp
pnpm type-check
pnpm lint
pnpm build
```

Expected: 全部通过

- [ ] 在微信开发者工具中测试小程序页面

1. 编译小程序项目
2. 访问公告列表页
3. 验证未读数量显示
4. 点击公告查看详情
5. 验证自动标记已读
6. 验证附件下载

- [ ] 提交批次 4 所有文件

```bash
git add .
git commit -m "feat: 完成公告管理批次4 - 小程序前端 + Mock API"
```

---

## 总结

批次 4 完成后，公告管理功能的三端开发全部完成：

✅ **批次 1：** 管理端前端（类型定义 + API + 列表页 + 编辑页）
✅ **批次 2：** 管理端 Mock API（完整 CRUD + 状态流转）
✅ **批次 3：** 官网前端 + Mock API（公开访问）
✅ **批次 4：** 小程序前端 + Mock API（阅读状态追踪）

**后续工作：**
- 后端实现（.NET 8 + SqlSugar）
- 单元测试
- 集成测试
- 生产部署