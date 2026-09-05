// pages/announcement/detail.ts —— 公告详情页
import { t } from '../../utils/i18n'
import { getUserAnnouncementById, markAnnouncementAsRead } from '../../api/announcement'
import type { UserAnnouncement } from '../../types/announcement'

interface PageData {
  title: string
  announcement: UserAnnouncement | null
  loading: boolean
  publishedAtText: string
  attachmentText: string
  downloadText: string
}

Page<PageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    title: '',
    announcement: null,
    loading: true,
    publishedAtText: '',
    attachmentText: '',
    downloadText: '',
  },

  onLoad(options: { id?: string }) {
    this.setData({
      title: t('common.announcement.detail'),
      publishedAtText: t('common.announcement.publishedAt'),
      attachmentText: t('common.announcement.attachment'),
      downloadText: t('common.announcement.download'),
    })

    if (options.id) {
      this.loadAnnouncement(options.id)
    } else {
      wx.showToast({ title: '参数错误', icon: 'none' })
      setTimeout(() => wx.navigateBack(), 1500)
    }
  },

  formatFileSize(size: number): string {
    if (size > 1024 * 1024) {
      return (size / 1024 / 1024).toFixed(2) + 'MB'
    }
    return (size / 1024).toFixed(2) + 'KB'
  },

  async loadAnnouncement(id: string) {
    try {
      const announcement = await getUserAnnouncementById(id)
      
      // 格式化附件大小
      if (announcement.attachments && announcement.attachments.length > 0) {
        announcement.attachments = announcement.attachments.map(item => ({
          ...item,
          sizeFormatted: this.formatFileSize(item.size)
        }))
      }
      
      this.setData({ announcement, loading: false })

      // 自动标记已读
      if (!announcement.isRead) {
        await markAnnouncementAsRead(id)
      }
    } catch (error) {
      wx.showToast({ title: t('common.loadFailed'), icon: 'none' })
      this.setData({ loading: false })
    }
  },

  downloadAttachment(e: WechatMiniprogram.TouchEvent) {
    const { url, name } = e.currentTarget.dataset

    wx.showLoading({ title: t('common.loading') })
    wx.downloadFile({
      url,
      success: (res) => {
        wx.hideLoading()
        if (res.statusCode === 200) {
          wx.openDocument({
            filePath: res.tempFilePath,
            fail: () => {
              wx.showToast({ title: '打开失败', icon: 'none' })
            },
          })
        }
      },
      fail: () => {
        wx.hideLoading()
        wx.showToast({ title: '下载失败', icon: 'none' })
      },
    })
  },

  copyLink(e: WechatMiniprogram.TouchEvent) {
    const { url } = e.currentTarget.dataset
    wx.setClipboardData({
      data: url,
      success: () => {
        wx.showToast({ title: '已复制链接', icon: 'success' })
      },
    })
  },
})
