// src/routes/site/download.ts
import { Router } from "express";
import { ok } from "../../helpers/envelope.js";
import { DOWNLOADS } from "../../data/site-full.js";

export const siteDownloadRouter = Router();

// 获取下载列表（官网公开）
siteDownloadRouter.get("/download/list/:category?", (req, res) => {
  const category = req.params.category as string | undefined;

  // 只返回启用状态的下载
  let list = DOWNLOADS.filter((d) => d.status === 1);

  // 按分类筛选
  if (category) {
    list = list.filter((d) => d.category === category);
  }

  // 排序：先按排序号，再按创建时间倒序
  list.sort((a, b) => a.sort - b.sort || new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

  res.json(ok(list));
});

// 获取下载详情并增加下载次数（官网公开）
siteDownloadRouter.get("/download/:id", (req, res) => {
  const download = DOWNLOADS.find((d) => d.id === req.params.id && d.status === 1);

  if (!download) {
    res.json(ok(null));
    return;
  }

  // 增加下载次数
  download.downloadCount++;

  res.json(ok(download));
});