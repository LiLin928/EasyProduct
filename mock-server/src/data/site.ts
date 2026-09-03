// src/data/site.ts
// [backend: Site 模块 | status: pending]
import Mock from "mockjs";
import { guid, isoTime } from "../helpers/id.js";

export type BannerStatus = "enabled" | "disabled";

export interface SiteBanner {
  id: string;
  title: string;
  titleEn: string;
  imageUrl: string;
  link: string;
  sort: number;
  status: BannerStatus;
  createdAt: string;
  updatedAt: string;
}

// Banner 数据 - 使用 picsum.photos 生成有效占位图
export const BANNERS: SiteBanner[] = Array.from({ length: 3 }, (_, i) => ({
  id: guid(),
  title: Mock.mock("@ctitle(8,16)"),
  titleEn: Mock.mock("@title(3,5)"),
  imageUrl: `https://picsum.photos/1920/600?random=${i}`,
  link: "",
  sort: i + 1,
  status: "enabled" as BannerStatus,
  createdAt: isoTime(),
  updatedAt: isoTime(),
}));
