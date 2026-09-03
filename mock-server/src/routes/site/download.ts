// src/routes/site/download.ts
import { Router } from "express";
import { ok, paginate } from "../../helpers/envelope.js";
import { DOWNLOADS, DOWNLOAD_CATEGORIES } from "../../data/site-full.js";

export const siteDownloadRouter = Router();

// 获取下载分类列表
siteDownloadRouter.get("/download-category/list", (_req, res) => {
  res.json(ok(DOWNLOAD_CATEGORIES));
});

// 获取下载列表（支持分类筛选）
siteDownloadRouter.get("/download/list", (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1);
  const pageSize = Number(req.query.pageSize ?? 10);
  const categoryId = req.query.categoryId as string | undefined;

  let list = DOWNLOADS.filter((d) => d.status === "published");

  // 按分类筛选
  if (categoryId) {
    list = list.filter((d) => d.categoryId === categoryId);
  }

  res.json(ok(paginate(list, pageIndex, pageSize)));
});

