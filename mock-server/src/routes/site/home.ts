// src/routes/site/home.ts
import { Router } from "express";
import { ok, paginate } from "../../helpers/envelope.js";
import { BANNERS } from "../../data/site.js";
import { NEWS_FULL } from "../../data/site-full.js";

export const siteHomeRouter = Router();

siteHomeRouter.get("/banner/list", (_req, res) => res.json(ok(BANNERS)));

siteHomeRouter.get("/news/list", (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1);
  const pageSize = Number(req.query.pageSize ?? 10);
  const keyword = req.query.keyword as string | undefined;

  let filtered = NEWS_FULL;
  if (keyword) {
    const kw = keyword.toLowerCase();
    filtered = filtered.filter(
      (n) => n.title.includes(keyword) || n.titleEn.toLowerCase().includes(kw)
    );
  }

  // 只返回列表需要的字段
  const list = filtered.map((n) => ({
    id: n.id,
    categoryId: n.categoryId,
    title: n.title,
    titleEn: n.titleEn,
    summary: n.summary,
    coverImage: n.coverImage,
    isTop: n.isTop,
    viewCount: n.viewCount,
    publishTime: n.publishTime,
  }));

  res.json(ok(paginate(list, pageIndex, pageSize)));
});
