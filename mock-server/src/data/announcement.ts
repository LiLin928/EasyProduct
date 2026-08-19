// src/data/announcement.ts
// Mock 数据：系统公告管理
import { guid } from '../helpers/id.js'

/**
 * 公告类型（前端类型定义对齐）
 */
export type AnnouncementType = 'all' | 'targeted'

/**
 * 公告级别
 */
export type AnnouncementLevel = 'normal' | 'important' | 'urgent'

/**
 * 公告状态
 */
export type AnnouncementStatus = 'draft' | 'published' | 'recalled'

/**
 * 附件结构（前端类型定义对齐）
 */
export interface Attachment {
  name: string
  url: string
  size: number
}

/**
 * 公告数据结构（前端类型定义对齐）
 */
export interface Announcement {
  id: string
  title: string
  content: string
  type: AnnouncementType
  level: AnnouncementLevel
  targetRoleIds?: string[]
  isTop: boolean
  topTime?: string
  publishTime?: string
  recallTime?: string
  status: AnnouncementStatus
  attachments?: Attachment[]
  creatorId: string
  creatorName: string
  createdAt: string
  updatedAt?: string
}

/**
 * 公告 Mock 数据
 */
export const ANNOUNCEMENTS: Announcement[] = [
  {
    id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
    title: '系统升级通知',
    content: '尊敬的用户，为了提供更好的服务体验，系统将于本周六凌晨2:00-6:00进行升级维护，届时系统将暂停服务。请提前做好相关工作安排，感谢您的理解与支持。',
    type: 'all',
    level: 'important',
    status: 'published',
    isTop: true,
    topTime: '2026-08-15T09:00:00Z',
    publishTime: '2026-08-15T09:00:00Z',
    creatorId: 'user-001',
    creatorName: '系统管理员',
    createdAt: '2026-08-14T16:30:00Z',
    updatedAt: '2026-08-15T09:00:00Z',
    attachments: [
      {
        name: '升级计划说明.pdf',
        url: '/uploads/announcements/upgrade-plan-20260815.pdf',
        size: 204800
      }
    ]
  },
  {
    id: 'b2c3d4e5-f6a7-8901-bcde-f23456789012',
    title: '新功能上线公告：订单批量导出功能',
    content: '各位同事，订单管理模块新增批量导出功能，支持按时间范围、订单状态等条件导出Excel报表。该功能已上线，欢迎体验使用。',
    type: 'targeted',
    level: 'normal',
    targetRoleIds: ['role-sales', 'role-ops'],
    status: 'published',
    isTop: false,
    publishTime: '2026-08-16T10:30:00Z',
    creatorId: 'user-002',
    creatorName: '产品经理',
    createdAt: '2026-08-15T14:20:00Z',
    updatedAt: '2026-08-16T10:30:00Z',
    attachments: []
  },
  {
    id: 'c3d4e5f6-a7b8-9012-cdef-345678901234',
    title: '紧急维护通知：数据库异常修复',
    content: '由于数据库出现异常，系统于今日下午14:30-15:00进行了紧急维护。目前系统已恢复正常，如有数据异常请及时反馈。',
    type: 'all',
    level: 'urgent',
    status: 'recalled',
    isTop: true,
    topTime: '2026-08-18T14:00:00Z',
    publishTime: '2026-08-18T14:00:00Z',
    recallTime: '2026-08-18T16:00:00Z',
    creatorId: 'user-003',
    creatorName: '运维专员',
    createdAt: '2026-08-18T13:50:00Z',
    updatedAt: '2026-08-18T16:00:00Z',
    attachments: []
  },
  {
    id: 'd4e5f6a7-b8c9-0123-def0-456789012345',
    title: '2027年春节放假通知',
    content: '各位同事，根据国家法定节假日安排，2027年春节放假时间为1月28日至2月3日，共7天。请各部门提前做好工作安排，值班人员名单请于放假前提交至行政部。',
    type: 'all',
    level: 'normal',
    status: 'draft',
    isTop: false,
    creatorId: 'user-004',
    creatorName: '行政专员',
    createdAt: '2026-08-19T09:00:00Z',
    updatedAt: '2026-08-19T11:30:00Z',
    attachments: [
      {
        name: '春节值班安排表.xlsx',
        url: '/uploads/announcements/spring-festival-schedule.xlsx',
        size: 51200
      }
    ]
  },
  {
    id: 'e5f6a7b8-c9d0-1234-ef01-567890123456',
    title: '会员积分系统升级公告',
    content: '尊敬的会员，为了提升您的使用体验，积分系统将于下周进行重大升级。升级后积分规则将有所调整，具体请查看附件《积分规则调整说明》。如有疑问请联系客服。',
    type: 'targeted',
    level: 'important',
    targetRoleIds: ['role-member'],
    status: 'published',
    isTop: false,
    publishTime: '2026-08-19T08:00:00Z',
    creatorId: 'user-001',
    creatorName: '系统管理员',
    createdAt: '2026-08-17T15:00:00Z',
    updatedAt: '2026-08-19T08:00:00Z',
    attachments: [
      {
        name: '积分规则调整说明.pdf',
        url: '/uploads/announcements/points-rule-update.pdf',
        size: 153600
      },
      {
        name: '积分兑换流程.png',
        url: '/uploads/announcements/points-exchange-process.png',
        size: 307200
      }
    ]
  }
]