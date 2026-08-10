// src/data/site-full.ts
// [backend: Site 模块 | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'

// 新闻列表（30 条）
export const NEWS_FULL = Mock.mock({
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
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((n: Record<string, unknown>) => ({ ...n, id: guid(), publishTime: isoTime() }))

// 视频列表（10 条）
export const VIDEOS = Mock.mock({
  'list|10': [{
    id: '@guid',
    title: '@ctitle(10,20)',
    titleEn: '@title(5,10)',
    coverImage: '@image(1280x720)',
    videoUrl: 'https://example.com/video.mp4',
    duration: '@integer(60,600)',
    'viewCount|100-5000': 1,
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((v: Record<string, unknown>) => ({ ...v, id: guid(), publishTime: isoTime() }))

// 下载列表（15 条）
export const DOWNLOADS = Mock.mock({
  'list|15': [{
    id: '@guid',
    title: '@ctitle(8,15)',
    titleEn: '@title(4,8)',
    fileUrl: 'https://example.com/file.pdf',
    'fileSize|1024-10485760': 1,
    'downloadCount|0-500': 1,
    publishTime: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((d: Record<string, unknown>) => ({ ...d, id: guid(), publishTime: isoTime() }))

// 关于单页
export const ABOUT = {
  id: guid(),
  title: '关于我们',
  titleEn: 'About Us',
  content: 'EasyProduct 是一家专注于工业设备与电子产品研发的高科技企业，致力于为客户提供优质的解决方案和专业的服务。',
  contentEn: 'EasyProduct is a high-tech enterprise focused on industrial equipment and electronics R&D, dedicated to providing quality solutions and professional services.',
  updatedAt: isoTime(),
}

// 联系信息
export const CONTACT_INFO = {
  address: '北京市朝阳区建国路88号',
  addressEn: '88 Jianguo Road, Chaoyang District, Beijing',
  phone: '010-12345678',
  email: 'contact@easyproduct.com',
  workingHours: '周一至周五 9:00-18:00',
  workingHoursEn: 'Mon-Fri 9:00-18:00',
}