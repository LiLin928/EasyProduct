// pages/announcement/index.ts —— 公告列表页
import { t } from '../../utils/i18n'
import { getUserAnnouncementList, getUnreadCount } from '../../api/announcement'
import type { UserAnnouncement } from '../../types/announcement'

interface PageData {
  title: string
  announcements: UserAnnouncement[]
  pageIndex: number
  pageSize: number
  hasMore: boolean
  loading: boolean
  unreadCount: number
  noDataText: string
  noMoreText: string
}

Page<PageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    title: '',
    announcements: [],
    pageIndex: 1,
    pageSize: 10,
    hasMore: true,
    loading: false,
    unreadCount: 0,
    noDataText: '',
    noMoreText: '',
  },

  onLoad() {
    this.setData({
      title: t('common.announcement.title'),
      noDataText: t('common.announcement.noData'),
      noMoreText: t('common.announcement.noMore'),
    })
    this.loadAnnouncements()
    this.loadUnreadCount()
  },

  onShow() {
    // 每次显示页面时刷新未读数量
    this.loadUnreadCount()
  },

  onPullDownRefresh() {
    this.setData({ pageIndex: 1, hasMore: true, announcements: [] })
    this.loadAnnouncements(true)
  },

  onReachBottom() {
    if (this.data.hasMore && !this.data.loading) {
      this.loadAnnouncements()
    }
  },

  async loadAnnouncements(isRefresh = false) {
    if (this.data.loading) return
    this.setData({ loading: true })

    try {
      const { list, total } = await getUserAnnouncementList({
        pageIndex: this.data.pageIndex,
        pageSize: this.data.pageSize,
      })

      const newList = this.data.pageIndex === 1 ? list : [...this.data.announcements, ...list]
      const hasMore = newList.length < total

      this.setData({
        announcements: newList,
        hasMore,
        pageIndex: this.data.pageIndex + 1,
      })

      if (isRefresh) {
        wx.stopPullDownRefresh()
      }
    } catch (error) {
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
      if (isRefresh) {
        wx.stopPullDownRefresh()
      }
    } finally {
      this.setData({ loading: false })
    }
  },

  async loadUnreadCount() {
    try {
      const { count } = await getUnreadCount()
      this.setData({ unreadCount: count })
    } catch (error) {
      // 静默失败
    }
  },

  goToDetail(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset
    wx.navigateTo({ url: `/pages/announcement/detail?id=${id}` })
  },
})