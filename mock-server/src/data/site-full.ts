// src/data/site-full.ts
// [backend: Site 模块 | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

export type SiteNewsStatus = 'draft' | 'published'
export type SiteVideoStatus = 'draft' | 'published'
export type SiteDownloadStatus = 'draft' | 'published'

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
  isTop: boolean
  viewCount: number
  status: SiteNewsStatus
  publishTime: string
  createdAt: string
  updatedAt: string
}

// 新闻列表（30 条）
export const NEWS_FULL: SiteNews[] = Mock.mock({
  'list|30': [{
    id: '@guid',
    categoryId: '@guid',
    title: '@ctitle(15,30)',
    titleEn: '@title(8,15)',
    summary: '@cparagraph(1,2)',
    summaryEn: '@sentence(10,20)',
    content: '@cparagraph(5,10)',
    contentEn: '@paragraph(10,20)',
    coverImage: '@image(640x360)',
    isTop: '@boolean',
    'viewCount|100-9999': 1,
    status: 'published',
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((n: Record<string, unknown>) => ({
  ...n,
  id: guid(),
  status: 'published' as SiteNewsStatus,
  publishTime: isoTime(),
  createdAt: isoTime(),
  updatedAt: isoTime(),
})) as SiteNews[]

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

// 视频列表（10 条）
export const VIDEOS: SiteVideo[] = Mock.mock({
  'list|10': [{
    id: '@guid',
    title: '@ctitle(10,20)',
    titleEn: '@title(5,10)',
    coverImage: '@image(1280x720)',
    videoUrl: 'https://example.com/video.mp4',
    duration: '@integer(60,600)',
    'viewCount|100-5000': 1,
    status: 'published',
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((v: Record<string, unknown>, i: number) => ({
  ...v,
  id: guid(),
  status: 'published' as SiteVideoStatus,
  sort: i + 1,
  publishTime: isoTime(),
  createdAt: isoTime(),
  updatedAt: isoTime(),
})) as SiteVideo[]

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

// 下载列表（15 条）
export const DOWNLOADS: SiteDownload[] = Mock.mock({
  'list|15': [{
    id: '@guid',
    title: '@ctitle(8,15)',
    titleEn: '@title(4,8)',
    fileUrl: 'https://example.com/file.pdf',
    'fileSize|1024-10485760': 1,
    'downloadCount|0-500': 1,
    status: 'published',
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((d: Record<string, unknown>, i: number) => ({
  ...d,
  id: guid(),
  status: 'published' as SiteDownloadStatus,
  sort: i + 1,
  publishTime: isoTime(),
  createdAt: isoTime(),
  updatedAt: isoTime(),
})) as SiteDownload[]

export interface SiteAbout {
  id: string
  title: string
  titleEn: string
  content: string
  contentEn: string
  updatedAt: string
}

// 关于单页
export const ABOUT: SiteAbout = {
  id: guid(),
  title: '关于我们',
  titleEn: 'About Us',
  content: 'EasyProduct 是一家专注于工业设备与电子产品研发的高科技企业，致力于为客户提供优质的解决方案和专业的服务。',
  contentEn: 'EasyProduct is a high-tech enterprise focused on industrial equipment and electronics R&D, dedicated to providing quality solutions and professional services.',
  updatedAt: isoTime(),
}

export interface ContactInfo {
  address: string
  addressEn: string
  phone: string
  email: string
  workingHours: string
  workingHoursEn: string
}

// 联系信息
export const CONTACT_INFO: ContactInfo = {
  address: '北京市朝阳区建国路88号',
  addressEn: '88 Jianguo Road, Chaoyang District, Beijing',
  phone: '010-12345678',
  email: 'contact@easyproduct.com',
  workingHours: '周一至周五 9:00-18:00',
  workingHoursEn: 'Mon-Fri 9:00-18:00',
}
