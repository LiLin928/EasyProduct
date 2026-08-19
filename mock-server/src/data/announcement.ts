// src/data/announcement.ts
// Mock 数据：系统公告管理
import { guid } from '../helpers/id.js'

/**
 * 公告数据结构
 */
export interface Announcement {
  id: string
  title: string
  content: string
  type: 'system' | 'feature' | 'maintenance' | 'event' // 系统通知 | 功能更新 | 维护通知 | 活动
  level: 'normal' | 'important' | 'urgent' // 普通 | 重要 | 紧急
  status: 'draft' | 'published' | 'withdrawn' // 草稿 | 已发布 | 已撤回
  isTop: boolean // 是否置顶
  targetRoleIds: string[] | null // 定向角色ID，null 表示全员
  publishTime: string | null // 发布时间，草稿为 null
  createTime: string // 创建时间
  updateTime: string // 更新时间
  creatorId: string // 创建人ID
  creatorName: string // 创建人姓名
  viewCount: number // 浏览次数
  attachments: Array<{
    id: string
    name: string
    url: string
    size: number
    type: string
  }>
}

/**
 * 公告 Mock 数据
 */
export const ANNOUNCEMENTS: Announcement[] = [
  {
    id: 'ann-001',
    title: '系统升级通知',
    content: '尊敬的用户，为了提供更好的服务体验，系统将于本周六凌晨2:00-6:00进行升级维护，届时系统将暂停服务。请提前做好相关工作安排，感谢您的理解与支持。',
    type: 'system',
    level: 'important',
    status: 'published',
    isTop: true,
    targetRoleIds: null, // 全员公告
    publishTime: '2026-08-15 09:00:00',
    createTime: '2026-08-14 16:30:00',
    updateTime: '2026-08-15 09:00:00',
    creatorId: 'user-001',
    creatorName: '系统管理员',
    viewCount: 1256,
    attachments: [
      {
        id: 'attach-001',
        name: '升级计划说明.pdf',
        url: '/uploads/announcements/upgrade-plan-20260815.pdf',
        size: 204800,
        type: 'application/pdf'
      }
    ]
  },
  {
    id: 'ann-002',
    title: '新功能上线公告：订单批量导出功能',
    content: '各位同事，订单管理模块新增批量导出功能，支持按时间范围、订单状态等条件导出Excel报表。该功能已上线，欢迎体验使用。',
    type: 'feature',
    level: 'normal',
    status: 'published',
    isTop: false,
    targetRoleIds: ['role-sales', 'role-ops'], // 定向销售和运营角色
    publishTime: '2026-08-16 10:30:00',
    createTime: '2026-08-15 14:20:00',
    updateTime: '2026-08-16 10:30:00',
    creatorId: 'user-002',
    creatorName: '产品经理',
    viewCount: 89,
    attachments: []
  },
  {
    id: 'ann-003',
    title: '紧急维护通知：数据库异常修复',
    content: '由于数据库出现异常，系统于今日下午14:30-15:00进行了紧急维护。目前系统已恢复正常，如有数据异常请及时反馈。',
    type: 'maintenance',
    level: 'urgent',
    status: 'withdrawn', // 已撤回
    isTop: true,
    targetRoleIds: null, // 全员公告
    publishTime: '2026-08-18 14:00:00',
    createTime: '2026-08-18 13:50:00',
    updateTime: '2026-08-18 16:00:00',
    creatorId: 'user-003',
    creatorName: '运维专员',
    viewCount: 45,
    attachments: []
  },
  {
    id: 'ann-004',
    title: '2027年春节放假通知',
    content: '各位同事，根据国家法定节假日安排，2027年春节放假时间为1月28日至2月3日，共7天。请各部门提前做好工作安排，值班人员名单请于放假前提交至行政部。',
    type: 'event',
    level: 'normal',
    status: 'draft', // 草稿状态
    isTop: false,
    targetRoleIds: null, // 全员公告
    publishTime: null, // 草稿无发布时间
    createTime: '2026-08-19 09:00:00',
    updateTime: '2026-08-19 11:30:00',
    creatorId: 'user-004',
    creatorName: '行政专员',
    viewCount: 0,
    attachments: [
      {
        id: 'attach-002',
        name: '春节值班安排表.xlsx',
        url: '/uploads/announcements/spring-festival-schedule.xlsx',
        size: 51200,
        type: 'application/vnd.ms-excel'
      }
    ]
  },
  {
    id: 'ann-005',
    title: '会员积分系统升级公告',
    content: '尊敬的会员，为了提升您的使用体验，积分系统将于下周进行重大升级。升级后积分规则将有所调整，具体请查看附件《积分规则调整说明》。如有疑问请联系客服。',
    type: 'system',
    level: 'important',
    status: 'published',
    isTop: false,
    targetRoleIds: ['role-member'], // 定向会员角色
    publishTime: '2026-08-19 08:00:00',
    createTime: '2026-08-17 15:00:00',
    updateTime: '2026-08-19 08:00:00',
    creatorId: 'user-001',
    creatorName: '系统管理员',
    viewCount: 523,
    attachments: [
      {
        id: 'attach-003',
        name: '积分规则调整说明.pdf',
        url: '/uploads/announcements/points-rule-update.pdf',
        size: 153600,
        type: 'application/pdf'
      },
      {
        id: 'attach-004',
        name: '积分兑换流程.png',
        url: '/uploads/announcements/points-exchange-process.png',
        size: 307200,
        type: 'image/png'
      }
    ]
  }
]