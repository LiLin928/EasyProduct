// src/data/site-full.ts
// [backend: Site 模块 | status: pending]
import Mock from "mockjs";
import { guid, isoTime } from "../helpers/id.js";

export type Status = 0 | 1; // 0=禁用，1=启用

export interface SiteNews {
  id: string;
  categoryId: string;
  title: string;
  titleEn: string;
  summary: string;
  summaryEn: string;
  content: string;
  contentEn: string;
  coverImage: string;
  author: string;
  source: string;
  viewCount: number;
  isTop: 0 | 1; // 0=不置顶，1=置顶
  publishTime: string;
  status: Status;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
}

// 新闻列表（30 条）- 使用有效图片 URL
export const NEWS_FULL: SiteNews[] = Array.from({ length: 30 }, (_, i) => {
  const id = guid();
  return {
    id,
    categoryId: guid(),
    title: Mock.mock("@ctitle(15,30)"),
    titleEn: Mock.mock("@title(8,15)"),
    summary: Mock.mock("@cparagraph(1,2)"),
    summaryEn: Mock.mock("@sentence(10,20)"),
    content: Mock.mock("@cparagraph(5,10)"),
    contentEn: Mock.mock("@paragraph(10,20)"),
    coverImage: `https://picsum.photos/640/360?random=${i}`,
    author: Mock.mock("@cname") as string,
    source: i % 2 === 0 ? '原创' : '转载',
    viewCount: Mock.mock("@integer(100, 9999)") as number,
    isTop: i % 4 === 0 ? 1 : 0, // 25% 置顶
    publishTime: isoTime(-i), // 递减时间
    status: 1 as Status,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  };
});

export interface VideoCategory {
  id: string;
  name: string;
  nameEn: string;
  sort: number;
}

export const VIDEO_CATEGORIES: VideoCategory[] = [
  { id: "cat-1", name: "产品教程", nameEn: "Product Tutorials", sort: 1 },
  { id: "cat-2", name: "企业宣传", nameEn: "Corporate", sort: 2 },
  { id: "cat-3", name: "客户案例", nameEn: "Case Studies", sort: 3 },
  { id: "cat-4", name: "技术培训", nameEn: "Technical Training", sort: 4 },
];

export interface SiteVideo {
  id: string;
  categoryId: string;
  title: string;
  titleEn: string;
  description: string;
  descriptionEn: string;
  coverImage: string;
  videoUrl: string;
  videoType: string;
  duration: number;
  playCount: number;
  category: string;
  sort: number;
  status: Status;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
}

// 视频列表（10 条）
export const VIDEOS: SiteVideo[] = Array.from({ length: 10 }, (_, i) => ({
  id: guid(),
  title: Mock.mock("@ctitle(10,20)"),
  titleEn: Mock.mock("@title(5,10)"),
  description: Mock.mock("@cparagraph(1,2)"),
  descriptionEn: Mock.mock("@sentence(10,20)"),
  coverImage: `https://picsum.photos/1280/720?random=${i}`,
  videoUrl: "https://example.com/video.mp4",
  videoType: "mp4",
  duration: Mock.mock("@integer(60, 600)") as number,
  playCount: Mock.mock("@integer(100, 5000)") as number,
  category: VIDEO_CATEGORIES[i % 4].id,
  sort: i + 1,
  status: 1 as Status,
  createdAt: isoTime(),
  updatedAt: isoTime(),
  createdBy: 'user-001',
}));

export interface DownloadCategory {
  id: string;
  name: string;
  nameEn: string;
  sort: number;
}

export const DOWNLOAD_CATEGORIES: DownloadCategory[] = [
  { id: "dcat-1", name: "产品手册", nameEn: "Product Manuals", sort: 1 },
  { id: "dcat-2", name: "技术文档", nameEn: "Technical Documents", sort: 2 },
  { id: "dcat-3", name: "软件下载", nameEn: "Software Downloads", sort: 3 },
  { id: "dcat-4", name: "驱动程序", nameEn: "Drivers", sort: 4 },
];

export interface SiteDownload {
  id: string;
  title: string;
  titleEn: string;
  description: string;
  descriptionEn: string;
  fileUrl: string;
  fileName: string;
  fileSize: number;
  fileType: string;
  downloadCount: number;
  category: string;
  sort: number;
  status: Status;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
}

// 下载列表（15 条）
export const DOWNLOADS: SiteDownload[] = Array.from({ length: 15 }, (_, i) => {
  const categoryIndex = i % 4;
  return {
    id: guid(),
    title: Mock.mock("@ctitle(8,15)"),
    titleEn: Mock.mock("@title(4,8)"),
    description: Mock.mock("@cparagraph(1,2)"),
    descriptionEn: Mock.mock("@sentence(10,20)"),
    fileUrl: "https://example.com/file.pdf",
    fileName: `file-${i + 1}.pdf`,
    fileSize: Mock.mock("@integer(1024, 10485760)") as number,
    fileType: "pdf",
    downloadCount: Mock.mock("@integer(0, 500)") as number,
    category: DOWNLOAD_CATEGORIES[categoryIndex].id,
    sort: i + 1,
    status: 1 as Status,
    createdAt: isoTime(),
    updatedAt: isoTime(),
    createdBy: 'user-001',
  };
});

export interface SiteAbout {
  id: string;
  title: string;
  titleEn: string;
  subtitle: string;
  subtitleEn: string;
  content: string;
  contentEn: string;
  coverImage: string;
  keywords: string;
  description: string;
  status: Status;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
}

// 关于单页
export const ABOUT: SiteAbout = {
  id: guid(),
  title: "关于我们",
  titleEn: "About Us",
  subtitle: "专注于工业设备与电子产品研发的高科技企业",
  subtitleEn: "A high-tech enterprise focused on industrial equipment and electronics R&D",
  content:
    "EasyProduct 是一家专注于工业设备与电子产品研发的高科技企业，致力于为客户提供优质的解决方案和专业的服务。我们拥有专业的研发团队和完善的质量管理体系，产品远销全球多个国家和地区。",
  contentEn:
    "EasyProduct is a high-tech enterprise focused on industrial equipment and electronics R&D, dedicated to providing quality solutions and professional services. We have a professional R&D team and a perfect quality management system, and our products are exported to many countries and regions around the world.",
  coverImage: "https://picsum.photos/1920/600?random=about",
  keywords: "工业设备,电子产品,研发,高科技",
  description: "EasyProduct - 专注于工业设备与电子产品研发的高科技企业",
  status: 1 as Status,
  createdAt: isoTime(),
  updatedAt: isoTime(),
  createdBy: 'user-001',
};

export interface ContactInfo {
  address: string;
  addressEn: string;
  phone: string;
  email: string;
  workingHours: string;
  workingHoursEn: string;
}

// 联系信息
export const CONTACT_INFO: ContactInfo = {
  address: "北京市朝阳区建国路88号",
  addressEn: "88 Jianguo Road, Chaoyang District, Beijing",
  phone: "010-12345678",
  email: "contact@easyproduct.com",
  workingHours: "周一至周五 9:00-18:00",
  workingHoursEn: "Mon-Fri 9:00-18:00",
};

