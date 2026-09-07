// src/types/site.ts

import type { PageQuery } from './api'

// ---- News ----
export type SiteNewsStatus = 'draft' | 'published'

export interface SiteNews {
  id: string
  categoryId: string
  title: string
  titleEn: string
  summary: string
  summaryEn: string
  content: string
  contentEn: string
  coverImage: string
  isTop: 0 | 1  // 是否置顶：0=否，1=是
  viewCount: number
  status: SiteNewsStatus
  publishTime: string
  createdAt: string
  updatedAt: string
}

export interface SiteNewsQuery extends PageQuery {
  title?: string
  status?: SiteNewsStatus
  isTop?: boolean
}

export interface SiteNewsParams {
  title: string
  titleEn?: string
  summary?: string
  summaryEn?: string
  content?: string
  contentEn?: string
  coverImage?: string
  categoryId?: string
  isTop?: boolean
}

// ---- Category ----
export type CategoryStatus = 'enabled' | 'disabled'

export interface ProductCategory {
  id: string
  name: string
  nameEn: string
  parentId: string
  sort: number
  status: CategoryStatus
  createdAt: string
  updatedAt: string
}

export interface CategoryParams {
  name: string
  nameEn?: string
  parentId?: string
  sort?: number
  status?: CategoryStatus
}

// ---- Banner ----
export type BannerStatus = 'enabled' | 'disabled'

export interface SiteBanner {
  id: string
  title: string
  titleEn: string
  imageUrl: string
  link: string
  sort: number
  status: BannerStatus
  createdAt: string
  updatedAt: string
}

export interface BannerQuery extends PageQuery {
  title?: string
  status?: BannerStatus
}

export interface BannerParams {
  title: string
  titleEn?: string
  imageUrl?: string
  link?: string
  sort?: number
  status?: BannerStatus
}

// ---- Video ----
export type SiteVideoStatus = 'draft' | 'published'

export interface SiteVideo {
  id: string
  title: string
  titleEn: string
  coverImage: string
  videoUrl: string
  duration: number
  viewCount: number
  status: SiteVideoStatus
  sort: number
  publishTime: string
  createdAt: string
  updatedAt: string
}

export interface VideoQuery extends PageQuery {
  title?: string
  status?: SiteVideoStatus
}

export interface VideoParams {
  title: string
  titleEn?: string
  coverImage?: string
  videoUrl?: string
  duration?: number
  sort?: number
  status?: SiteVideoStatus
}

// ---- Download ----
export type SiteDownloadStatus = 'draft' | 'published'

export interface SiteDownload {
  id: string
  title: string
  titleEn: string
  fileUrl: string
  fileSize: number
  downloadCount: number
  status: SiteDownloadStatus
  sort: number
  publishTime: string
  createdAt: string
  updatedAt: string
}

export interface DownloadQuery extends PageQuery {
  title?: string
  status?: SiteDownloadStatus
}

export interface DownloadParams {
  title: string
  titleEn?: string
  fileUrl?: string
  fileSize?: number
  sort?: number
  status?: SiteDownloadStatus
}

// ---- About ----
export interface SiteAbout {
  id: string
  title: string
  titleEn: string
  content: string
  contentEn: string
  updatedAt: string
}

export interface AboutParams {
  title?: string
  titleEn?: string
  content?: string
  contentEn?: string
}

// ---- Inquiry ----
export type InquiryStatus = 'pending' | 'processing' | 'completed' | 'converted'

export interface InquiryItem {
  productId: string
  productName: string
  quantity: number
  unit: string
  remark?: string
}

export interface SiteInquiry {
  id: string
  companyName: string
  contactName: string
  phone: string
  email: string
  items: InquiryItem[]
  status: InquiryStatus
  remark?: string
  createdAt: string
  updatedAt: string
}

export interface InquiryQuery extends PageQuery {
  status?: InquiryStatus
  companyName?: string
}

// ---- Contact ----
export type ContactStatus = 'unread' | 'read' | 'archived'

export interface ContactMessage {
  id: string
  name: string
  company: string
  phone: string
  email: string
  subject: string
  message: string
  status: ContactStatus
  createdAt: string
  updatedAt: string
}

export interface ContactQuery extends PageQuery {
  status?: ContactStatus
  name?: string
}

// ---- Status option constants ----
export const NEWS_STATUS_OPTIONS = [
  { label: 'site.news.statusDraft', value: 'draft' as const },
  { label: 'site.news.statusPublished', value: 'published' as const },
]

export const VIDEO_STATUS_OPTIONS = [
  { label: 'site.video.statusDraft', value: 'draft' as const },
  { label: 'site.video.statusPublished', value: 'published' as const },
]

export const DOWNLOAD_STATUS_OPTIONS = [
  { label: 'site.download.statusDraft', value: 'draft' as const },
  { label: 'site.download.statusPublished', value: 'published' as const },
]

export const INQUIRY_STATUS_OPTIONS = [
  { label: 'site.inquiry.statusPending', value: 'pending' as const },
  { label: 'site.inquiry.statusProcessing', value: 'processing' as const },
  { label: 'site.inquiry.statusCompleted', value: 'completed' as const },
  { label: 'site.inquiry.statusConverted', value: 'converted' as const },
]

export const CONTACT_STATUS_OPTIONS = [
  { label: 'site.contact.statusUnread', value: 'unread' as const },
  { label: 'site.contact.statusRead', value: 'read' as const },
  { label: 'site.contact.statusArchived', value: 'archived' as const },
]
