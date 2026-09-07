# Site 模块后端开发说明文档

> **模块名称**：Site - 官网门户
> **适用项目**：EasyProduct.WebApi
> **创建日期**：2026-09-07
> **最后更新**：2026-09-07

---

## 目录

1. [模块概述](#1-模块概述)
2. [技术栈和依赖](#2-技术栈和依赖)
3. [实体类设计](#3-实体类设计)
4. [DTO 设计](#4-dto-设计)
5. [Service 层实现指南](#5-service-层实现指南)
6. [Controller 层实现指南](#6-controller-层实现指南)
7. [数据库表设计](#7-数据库表设计)
8. [业务逻辑要点](#8-业务逻辑要点)
9. [单元测试要求](#9-单元测试要求)
10. [开发检查清单](#10-开发检查清单)

---

## 1. 模块概述

### 1.1 业务定位

Site 模块是 EasyProduct 的官网门户模块，负责管理企业官网的所有内容，包括新闻资讯、产品展示、视频中心、下载中心、Banner 轮播、联系方式等。

### 1.2 访问特点

- **公开访问**：所有官网前端接口无需认证，支持匿名访问
- **SEO 优化**：所有内容接口需要考虑搜索引擎优化
- **CDN 缓存**：高频访问内容需要支持 CDN 缓存
- **限流保护**：提交类接口需要 IP 限流防护

### 1.3 功能范围

| 功能模块 | 说明 | 主要实体 |
|---------|------|---------|
| 新闻资讯 | 新闻发布与管理 | SiteNews, SiteCategory |
| 产品展示 | 官网产品展示 | SiteProduct, SiteCategory |
| Banner 管理 | 首页轮播图 | SiteBanner |
| 视频中心 | 视频展示 | SiteVideo |
| 下载中心 | 资源下载 | SiteDownload |
| 关于我们 | 企业介绍 | SiteAbout |
| 联系方式 | 联系信息 | SiteContact |
| 在线咨询 | 访客留言 | SiteInquiry |

### 1.4 接口分区

| 分区 | 路由前缀 | 认证方式 | 消费者 |
|------|---------|---------|--------|
| 官网前端 | `/api/site/**` | 匿名访问（限流） | EasyProduct.Site |
| 管理后台 | `/api/admin/site-**` | Admin JWT | EasyProduct.Admin |

---

## 2. 技术栈和依赖

### 2.1 核心技术栈

| 类别 | 选型 | 版本 | 用途 |
|------|------|------|------|
| 运行时 | .NET | 8.0 (LTS) | Web API 宿主 |
| ORM | SqlSugarCore | 5.1.4.x | 数据访问 |
| MySQL 驱动 | MySqlConnector | 2.5.x | 数据库连接 |
| DI 容器 | Autofac | 8.x / 9.x | 依赖注入 |
| 日志 | Serilog | 8.x | 日志记录 |
| 对象映射 | Mapster | 10.x | DTO 映射 |
| 认证 | JwtBearer | 8.x | JWT 认证 |
| API 文档 | Swashbuckle.AspNetCore | 6.9.x | Swagger |

### 2.2 Site 模块特有依赖

无需额外依赖，使用项目统一技术栈。

### 2.3 项目引用关系

```
EasyProduct.WebApi/
├─ EasyProduct.Web/                    # ASP.NET Core 宿主
│  └─ Controllers/Site/                # Site Controller
├─ EasyProduct.Business/               # 业务层
│  └─ Site/                            # Site Service
├─ EasyProduct.Models/                 # 实体/DTO
│  ├─ Entitys/Site/                    # Site 实体
│  └─ Dto/Site/                        # Site DTO
└─ EasyProduct.Common/                 # 通用组件
```

---

## 3. 实体类设计

### 3.1 实体基类

所有 Site 模块实体继承自 `BaseEntity`：

```csharp
/// <summary>
/// 实体基类（所有业务表继承）
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID（GUID）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CreateBy { get; set; }

    /// <summary>
    /// 软删除标记（0-未删除，1-已删除）
    /// </summary>
    public int IsDeleted { get; set; }
}
```

### 3.2 SiteNews - 新闻资讯实体

```csharp
/// <summary>
/// 新闻资讯实体
/// </summary>
[SugarTable("site_news", "新闻资讯表")]
public class SiteNews : BaseEntity
{
    /// <summary>
    /// 分类ID
    /// </summary>
    [SugarColumn(Length = 36)]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 标题（中文）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    [SugarColumn(Length = 500)]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 正文内容（HTML）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsTop { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// 状态（0-草稿，1-已发布，2-已归档）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// SEO关键词
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? SeoKeywords { get; set; }

    /// <summary>
    /// SEO描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? SeoDescription { get; set; }
}
```

### 3.3 SiteCategory - 分类实体

```csharp
/// <summary>
/// 分类实体（用于新闻、产品、视频、下载等）
/// </summary>
[SugarTable("site_category", "分类表")]
public class SiteCategory : BaseEntity
{
    /// <summary>
    /// 分类名称（中文）
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称（英文）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? NameEn { get; set; }

    /// <summary>
    /// 父分类ID（支持多级分类）
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = true)]
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类类型（news-新闻，product-产品，video-视频，download-下载）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Type { get; set; } = "news";

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }
}
```

### 3.4 SiteBanner - Banner 实体

```csharp
/// <summary>
/// Banner轮播图实体
/// </summary>
[SugarTable("site_banner", "Banner轮播图表")]
public class SiteBanner : BaseEntity
{
    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 链接URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 打开方式（_blank-新窗口，_self-当前窗口）
    /// </summary>
    [SugarColumn(Length = 10)]
    public string Target { get; set; } = "_blank";

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 开始时间（定时发布）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间（定时下线）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? EndTime { get; set; }
}
```

### 3.5 SiteVideo - 视频实体

```csharp
/// <summary>
/// 视频实体
/// </summary>
[SugarTable("site_video", "视频表")]
public class SiteVideo : BaseEntity
{
    /// <summary>
    /// 标题（中文）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 时长（秒）
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }
}
```

### 3.6 SiteDownload - 下载资源实体

```csharp
/// <summary>
/// 下载资源实体
/// </summary>
[SugarTable("site_download", "下载资源表")]
public class SiteDownload : BaseEntity
{
    /// <summary>
    /// 标题（中文）
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型（扩展名）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }
}
```

### 3.7 SiteContact - 联系方式实体

```csharp
/// <summary>
/// 联系方式实体（单例，只维护一条记录）
/// </summary>
[SugarTable("site_contact", "联系方式表")]
public class SiteContact : BaseEntity
{
    /// <summary>
    /// 公司名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 传真
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Fax { get; set; }

    /// <summary>
    /// 微信号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Wechat { get; set; }

    /// <summary>
    /// 微博号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Weibo { get; set; }

    /// <summary>
    /// 地图链接URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? MapUrl { get; set; }
}
```

### 3.8 SiteInquiry - 在线咨询实体

```csharp
/// <summary>
/// 在线咨询实体
/// </summary>
[SugarTable("site_inquiry", "在线咨询表")]
public class SiteInquiry : BaseEntity
{
    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Company { get; set; }

    /// <summary>
    /// 咨询内容
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 处理状态（0-待处理，1-已处理）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 处理人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ProcessedBy { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ProcessedTime { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ProcessedRemark { get; set; }

    /// <summary>
    /// 访客IP
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 来源页面
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? SourceUrl { get; set; }
}
```

### 3.9 SiteAbout - 关于我们实体

```csharp
/// <summary>
/// 关于我们实体（单例，只维护一条记录）
/// </summary>
[SugarTable("site_about", "关于我们表")]
public class SiteAbout : BaseEntity
{
    /// <summary>
    /// 公司简介（中文）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 公司简介（英文）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 发展历程（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Milestones { get; set; }

    /// <summary>
    /// 企业荣誉（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Honors { get; set; }

    /// <summary>
    /// 企业文化（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Culture { get; set; }

    /// <summary>
    /// 图片列表（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Images { get; set; }
}
```

---

## 4. DTO 设计

### 4.1 新闻相关 DTO

#### SiteNewsQuery - 新闻查询入参

```csharp
/// <summary>
/// 新闻查询入参
/// </summary>
public class SiteNewsQuery : PageQuery
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 关键词（标题、摘要）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态（0-草稿，1-已发布，2-已归档）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool? IsTop { get; set; }
}
```

#### SiteNewsCreateDto - 新闻创建入参

```csharp
/// <summary>
/// 新闻创建入参
/// </summary>
public class SiteNewsCreateDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    [Required(ErrorMessage = "分类不能为空")]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 标题（中文）
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题最多200字符")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    [Required(ErrorMessage = "摘要不能为空")]
    [MaxLength(500, ErrorMessage = "摘要最多500字符")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 正文内容（HTML）
    /// </summary>
    [Required(ErrorMessage = "内容不能为空")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [MaxLength(500)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsTop { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 状态（0-草稿，1-已发布，2-已归档）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// SEO关键词
    /// </summary>
    [MaxLength(200)]
    public string? SeoKeywords { get; set; }

    /// <summary>
    /// SEO描述
    /// </summary>
    [MaxLength(500)]
    public string? SeoDescription { get; set; }
}
```

#### SiteNewsUpdateDto - 新闻更新入参

```csharp
/// <summary>
/// 新闻更新入参
/// </summary>
public class SiteNewsUpdateDto
{
    /// <summary>
    /// 新闻ID
    /// </summary>
    [Required(ErrorMessage = "新闻ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 分类ID
    /// </summary>
    [Required(ErrorMessage = "分类不能为空")]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 标题（中文）
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 正文内容（HTML）
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [MaxLength(500)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsTop { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 状态（0-草稿，1-已发布，2-已归档）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// SEO关键词
    /// </summary>
    [MaxLength(200)]
    public string? SeoKeywords { get; set; }

    /// <summary>
    /// SEO描述
    /// </summary>
    [MaxLength(500)]
    public string? SeoDescription { get; set; }
}
```

#### SiteNewsDto - 新闻出参

```csharp
/// <summary>
/// 新闻出参
/// </summary>
public class SiteNewsDto
{
    /// <summary>
    /// 新闻ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 分类ID
    /// </summary>
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 标题（中文）
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 正文内容（HTML）
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 封面图片URL
    /// </summary>
    public string? CoverImage { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsTop { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// 状态（0-草稿，1-已发布，2-已归档）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// SEO关键词
    /// </summary>
    public string? SeoKeywords { get; set; }

    /// <summary>
    /// SEO描述
    /// </summary>
    public string? SeoDescription { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}
```

### 4.2 分类相关 DTO

#### SiteCategoryCreateDto - 分类创建入参

```csharp
/// <summary>
/// 分类创建入参
/// </summary>
public class SiteCategoryCreateDto
{
    /// <summary>
    /// 分类名称（中文）
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称（英文）
    /// </summary>
    [MaxLength(50)]
    public string? NameEn { get; set; }

    /// <summary>
    /// 父分类ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类类型
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = "news";

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }
}
```

#### SiteCategoryDto - 分类出参

```csharp
/// <summary>
/// 分类出参
/// </summary>
public class SiteCategoryDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称（中文）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称（英文）
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// 父分类ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类类型
    /// </summary>
    public string Type { get; set; } = "news";

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 子分类列表
    /// </summary>
    public List<SiteCategoryDto>? Children { get; set; }
}
```

### 4.3 Banner 相关 DTO

#### SiteBannerCreateDto - Banner 创建入参

```csharp
/// <summary>
/// Banner创建入参
/// </summary>
public class SiteBannerCreateDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    [Required(ErrorMessage = "图片不能为空")]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 链接URL
    /// </summary>
    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 打开方式
    /// </summary>
    [MaxLength(10)]
    public string Target { get; set; } = "_blank";

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}
```

#### SiteBannerDto - Banner 出参

```csharp
/// <summary>
/// Banner出参
/// </summary>
public class SiteBannerDto
{
    /// <summary>
    /// BannerID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 链接URL
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 打开方式
    /// </summary>
    public string Target { get; set; } = "_blank";

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

### 4.4 视频相关 DTO

#### SiteVideoCreateDto - 视频创建入参

```csharp
/// <summary>
/// 视频创建入参
/// </summary>
public class SiteVideoCreateDto
{
    /// <summary>
    /// 标题（中文）
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [MaxLength(500)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    [Required(ErrorMessage = "视频URL不能为空")]
    [MaxLength(500)]
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 时长（秒）
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }
}
```

#### SiteVideoDto - 视频出参

```csharp
/// <summary>
/// 视频出参
/// </summary>
public class SiteVideoDto
{
    /// <summary>
    /// 视频ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 标题（中文）
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// 时长（秒）
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

### 4.5 下载相关 DTO

#### SiteDownloadCreateDto - 下载创建入参

```csharp
/// <summary>
/// 下载资源创建入参
/// </summary>
public class SiteDownloadCreateDto
{
    /// <summary>
    /// 标题（中文）
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    [Required(ErrorMessage = "文件不能为空")]
    [MaxLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型（扩展名）
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }
}
```

#### SiteDownloadDto - 下载出参

```csharp
/// <summary>
/// 下载资源出参
/// </summary>
public class SiteDownloadDto
{
    /// <summary>
    /// 下载ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 标题（中文）
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态（0-禁用，1-启用）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

### 4.6 在线咨询相关 DTO

#### SiteInquiryCreateDto - 咨询创建入参

```csharp
/// <summary>
/// 在线咨询创建入参（官网前端提交）
/// </summary>
public class SiteInquiryCreateDto
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "姓名不能为空")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    [Required(ErrorMessage = "手机号不能为空")]
    [MaxLength(20)]
    [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号格式不正确")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(50)]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    [MaxLength(100)]
    public string? Company { get; set; }

    /// <summary>
    /// 咨询内容
    /// </summary>
    [Required(ErrorMessage = "咨询内容不能为空")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 来源页面
    /// </summary>
    [MaxLength(200)]
    public string? SourceUrl { get; set; }
}
```

#### SiteInquiryProcessDto - 咨询处理入参

```csharp
/// <summary>
/// 在线咨询处理入参（管理后台）
/// </summary>
public class SiteInquiryProcessDto
{
    /// <summary>
    /// 咨询ID
    /// </summary>
    [Required(ErrorMessage = "咨询ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 处理状态
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "processed";

    /// <summary>
    /// 处理备注
    /// </summary>
    [MaxLength(500)]
    public string? ProcessedRemark { get; set; }
}
```

#### SiteInquiryDto - 咨询出参

```csharp
/// <summary>
/// 在线咨询出参
/// </summary>
public class SiteInquiryDto
{
    /// <summary>
    /// 咨询ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// 咨询内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 处理状态（0-待处理，1-已处理）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 处理人
    /// </summary>
    public string? ProcessedBy { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? ProcessedTime { get; set; }

    /// <summary>
    /// 处理备注
    /// </summary>
    public string? ProcessedRemark { get; set; }

    /// <summary>
    /// 访客IP
    /// </summary>
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 来源页面
    /// </summary>
    public string? SourceUrl { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

### 4.7 联系方式相关 DTO

#### SiteContactUpdateDto - 联系方式更新入参

```csharp
/// <summary>
/// 联系方式更新入参
/// </summary>
public class SiteContactUpdateDto
{
    /// <summary>
    /// 公司名称
    /// </summary>
    [Required(ErrorMessage = "公司名称不能为空")]
    [MaxLength(100)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 电话
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [Required]
    [MaxLength(50)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 传真
    /// </summary>
    [MaxLength(50)]
    public string? Fax { get; set; }

    /// <summary>
    /// 微信号
    /// </summary>
    [MaxLength(50)]
    public string? Wechat { get; set; }

    /// <summary>
    /// 微博号
    /// </summary>
    [MaxLength(50)]
    public string? Weibo { get; set; }

    /// <summary>
    /// 地图链接URL
    /// </summary>
    [MaxLength(500)]
    public string? MapUrl { get; set; }
}
```

#### SiteContactDto - 联系方式出参

```csharp
/// <summary>
/// 联系方式出参
/// </summary>
public class SiteContactDto
{
    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 传真
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// 微信号
    /// </summary>
    public string? Wechat { get; set; }

    /// <summary>
    /// 微博号
    /// </summary>
    public string? Weibo { get; set; }

    /// <summary>
    /// 地图链接URL
    /// </summary>
    public string? MapUrl { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}
```

### 4.8 关于我们相关 DTO

#### SiteAboutUpdateDto - 关于我们更新入参

```csharp
/// <summary>
/// 关于我们更新入参
/// </summary>
public class SiteAboutUpdateDto
{
    /// <summary>
    /// 公司简介（中文）
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 公司简介（英文）
    /// </summary>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 发展历程（JSON格式）
    /// </summary>
    public string? Milestones { get; set; }

    /// <summary>
    /// 企业荣誉（JSON格式）
    /// </summary>
    public string? Honors { get; set; }

    /// <summary>
    /// 企业文化（JSON格式）
    /// </summary>
    public string? Culture { get; set; }

    /// <summary>
    /// 图片列表（JSON格式）
    /// </summary>
    public string? Images { get; set; }
}
```

#### SiteAboutDto - 关于我们出参

```csharp
/// <summary>
/// 关于我们出参
/// </summary>
public class SiteAboutDto
{
    /// <summary>
    /// 公司简介（中文）
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 公司简介（英文）
    /// </summary>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 发展历程（JSON格式）
    /// </summary>
    public string? Milestones { get; set; }

    /// <summary>
    /// 企业荣誉（JSON格式）
    /// </summary>
    public string? Honors { get; set; }

    /// <summary>
    /// 企业文化（JSON格式）
    /// </summary>
    public string? Culture { get; set; }

    /// <summary>
    /// 图片列表（JSON格式）
    /// </summary>
    public string? Images { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}
```

---

## 5. Service 层实现指南

### 5.1 Service 接口定义

#### ISiteNewsService - 新闻服务接口

```csharp
/// <summary>
/// 新闻服务接口
/// </summary>
public interface ISiteNewsService
{
    /// <summary>
    /// 获取新闻分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻分页列表</returns>
    Task<PageResult<SiteNewsDto>> GetNewsListAsync(SiteNewsQuery query);

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    Task<SiteNewsDto> GetNewsByIdAsync(string id);

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新闻ID</returns>
    Task<string> CreateNewsAsync(SiteNewsCreateDto dto);

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateNewsAsync(SiteNewsUpdateDto dto);

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteNewsAsync(string id);

    /// <summary>
    /// 增加浏览次数
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    Task<bool> IncrementViewCountAsync(string id);

    /// <summary>
    /// 获取首页新闻列表
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>新闻列表</returns>
    Task<List<SiteNewsDto>> GetHomeNewsListAsync(int limit = 10);
}
```

#### ISiteCategoryService - 分类服务接口

```csharp
/// <summary>
/// 分类服务接口
/// </summary>
public interface ISiteCategoryService
{
    /// <summary>
    /// 获取分类列表（树形结构）
    /// </summary>
    /// <param name="type">分类类型</param>
    /// <returns>分类树</returns>
    Task<List<SiteCategoryDto>> GetCategoryTreeAsync(string type);

    /// <summary>
    /// 获取分类列表（平铺）
    /// </summary>
    /// <param name="type">分类类型</param>
    /// <returns>分类列表</returns>
    Task<List<SiteCategoryDto>> GetCategoryListAsync(string type);

    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>分类ID</returns>
    Task<string> CreateCategoryAsync(SiteCategoryCreateDto dto);

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateCategoryAsync(string id, SiteCategoryCreateDto dto);

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteCategoryAsync(string id);
}
```

#### ISiteBannerService - Banner 服务接口

```csharp
/// <summary>
/// Banner服务接口
/// </summary>
public interface ISiteBannerService
{
    /// <summary>
    /// 获取Banner列表（管理端）
    /// </summary>
    /// <returns>Banner列表</returns>
    Task<List<SiteBannerDto>> GetBannerListAsync();

    /// <summary>
    /// 获取启用的Banner列表（官网前端）
    /// </summary>
    /// <returns>Banner列表</returns>
    Task<List<SiteBannerDto>> GetEnabledBannerListAsync();

    /// <summary>
    /// 创建Banner
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>Banner ID</returns>
    Task<string> CreateBannerAsync(SiteBannerCreateDto dto);

    /// <summary>
    /// 更新Banner
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateBannerAsync(string id, SiteBannerCreateDto dto);

    /// <summary>
    /// 删除Banner
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteBannerAsync(string id);
}
```

#### ISiteVideoService - 视频服务接口

```csharp
/// <summary>
/// 视频服务接口
/// </summary>
public interface ISiteVideoService
{
    /// <summary>
    /// 获取视频列表
    /// </summary>
    /// <returns>视频列表</returns>
    Task<List<SiteVideoDto>> GetVideoListAsync();

    /// <summary>
    /// 创建视频
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>视频ID</returns>
    Task<string> CreateVideoAsync(SiteVideoCreateDto dto);

    /// <summary>
    /// 更新视频
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateVideoAsync(string id, SiteVideoCreateDto dto);

    /// <summary>
    /// 删除视频
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteVideoAsync(string id);

    /// <summary>
    /// 增加浏览次数
    /// </summary>
    /// <param name="id">视频ID</param>
    /// <returns>是否成功</returns>
    Task<bool> IncrementViewCountAsync(string id);
}
```

#### ISiteDownloadService - 下载服务接口

```csharp
/// <summary>
/// 下载服务接口
/// </summary>
public interface ISiteDownloadService
{
    /// <summary>
    /// 获取下载列表
    /// </summary>
    /// <returns>下载列表</returns>
    Task<List<SiteDownloadDto>> GetDownloadListAsync();

    /// <summary>
    /// 创建下载资源
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>下载ID</returns>
    Task<string> CreateDownloadAsync(SiteDownloadCreateDto dto);

    /// <summary>
    /// 更新下载资源
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateDownloadAsync(string id, SiteDownloadCreateDto dto);

    /// <summary>
    /// 删除下载资源
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteDownloadAsync(string id);

    /// <summary>
    /// 增加下载次数
    /// </summary>
    /// <param name="id">下载ID</param>
    /// <returns>是否成功</returns>
    Task<bool> IncrementDownloadCountAsync(string id);
}
```

#### ISiteInquiryService - 在线咨询服务接口

```csharp
/// <summary>
/// 在线咨询服务接口
/// </summary>
public interface ISiteInquiryService
{
    /// <summary>
    /// 提交在线咨询
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <param name="ip">访客IP</param>
    /// <returns>咨询ID</returns>
    Task<string> SubmitInquiryAsync(SiteInquiryCreateDto dto, string ip);

    /// <summary>
    /// 获取咨询列表（管理端）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>咨询分页列表</returns>
    Task<PageResult<SiteInquiryDto>> GetInquiryListAsync(SiteInquiryQuery query);

    /// <summary>
    /// 处理咨询
    /// </summary>
    /// <param name="dto">处理参数</param>
    /// <param name="processedBy">处理人</param>
    /// <returns>是否成功</returns>
    Task<bool> ProcessInquiryAsync(SiteInquiryProcessDto dto, string processedBy);
}
```

#### ISiteContactService - 联系方式服务接口

```csharp
/// <summary>
/// 联系方式服务接口
/// </summary>
public interface ISiteContactService
{
    /// <summary>
    /// 获取联系方式
    /// </summary>
    /// <returns>联系方式</returns>
    Task<SiteContactDto> GetContactAsync();

    /// <summary>
    /// 更新联系方式
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateContactAsync(SiteContactUpdateDto dto);
}
```

#### ISiteAboutService - 关于我们服务接口

```csharp
/// <summary>
/// 关于我们服务接口
/// </summary>
public interface ISiteAboutService
{
    /// <summary>
    /// 获取关于我们
    /// </summary>
    /// <returns>关于我们</returns>
    Task<SiteAboutDto> GetAboutAsync();

    /// <summary>
    /// 更新关于我们
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAboutAsync(SiteAboutUpdateDto dto);
}
```

### 5.2 Service 实现示例

#### SiteNewsService - 新闻服务实现

```csharp
/// <summary>
/// 新闻服务实现
/// </summary>
public class SiteNewsService : BaseService, ISiteNewsService
{
    private readonly ILogger<SiteNewsService> _logger;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">数据库上下文</param>
    /// <param name="logger">日志服务</param>
    /// <param name="cacheService">缓存服务</param>
    public SiteNewsService(
        ISqlSugarClient db,
        ILogger<SiteNewsService> logger,
        ICacheService cacheService) : base(db)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 获取新闻分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻分页列表</returns>
    public async Task<PageResult<SiteNewsDto>> GetNewsListAsync(SiteNewsQuery query)
    {
        var queryable = _db.Queryable<SiteNews>()
            .Includes(x => x.Category)
            .Where(x => !x.IsDeleted);

        // 分类筛选
        if (!string.IsNullOrEmpty(query.CategoryId))
        {
            queryable = queryable.Where(x => x.CategoryId == query.CategoryId);
        }

        // 关键词搜索
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where(x => x.Title.Contains(query.Keyword) ||
                                             x.Summary.Contains(query.Keyword));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 置顶筛选
        if (query.IsTop.HasValue)
        {
            queryable = queryable.Where(x => x.IsTop == query.IsTop.Value);
        }

        // 排序：置顶优先，然后按发布时间倒序
        queryable = queryable.OrderByDescending(x => x.IsTop)
                              .ThenByDescending(x => x.PublishTime);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var dtoList = list.Adapt<List<SiteNewsDto>>();

        return new PageResult<SiteNewsDto>
        {
            List = dtoList,
            Total = total
        };
    }

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    public async Task<SiteNewsDto> GetNewsByIdAsync(string id)
    {
        var cacheKey = $"site:news:{id}";
        var cached = await _cacheService.GetAsync<SiteNewsDto>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var entity = await _db.Queryable<SiteNews>()
            .Includes(x => x.Category)
            .Where(x => x.Id == Guid.Parse(id) && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw BusinessException.NotFound("新闻不存在");
        }

        var dto = entity.Adapt<SiteNewsDto>();
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

        return dto;
    }

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新闻ID</returns>
    public async Task<string> CreateNewsAsync(SiteNewsCreateDto dto)
    {
        // 验证分类是否存在
        var category = await _db.Queryable<SiteCategory>()
            .Where(x => x.Id == Guid.Parse(dto.CategoryId) && x.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw BusinessException.BadRequest("分类不存在");
        }

        var entity = dto.Adapt<SiteNews>();
        entity.Id = Guid.NewGuid();
        entity.PublishTime = dto.PublishTime ?? DateTime.Now;
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建新闻成功，ID：{NewsId}，标题：{Title}", entity.Id, entity.Title);

        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateNewsAsync(SiteNewsUpdateDto dto)
    {
        var entity = await _db.Queryable<SiteNews>()
            .Where(x => x.Id == Guid.Parse(dto.Id) && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw BusinessException.NotFound("新闻不存在");
        }

        // 验证分类是否存在
        var category = await _db.Queryable<SiteCategory>()
            .Where(x => x.Id == Guid.Parse(dto.CategoryId) && x.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw BusinessException.BadRequest("分类不存在");
        }

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();

        // 清除缓存
        await _cacheService.RemoveAsync($"site:news:{entity.Id}");

        _logger.LogInformation("更新新闻成功，ID：{NewsId}，标题：{Title}", entity.Id, entity.Title);

        return true;
    }

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteNewsAsync(string id)
    {
        var entity = await _db.Queryable<SiteNews>()
            .Where(x => x.Id == Guid.Parse(id) && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw BusinessException.NotFound("新闻不存在");
        }

        entity.IsDeleted = 1;
        entity.UpdateTime = DateTime.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();

        // 清除缓存
        await _cacheService.RemoveAsync($"site:news:{id}");

        _logger.LogInformation("删除新闻成功，ID：{NewsId}，标题：{Title}", entity.Id, entity.Title);

        return true;
    }

    /// <summary>
    /// 增加浏览次数
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> IncrementViewCountAsync(string id)
    {
        await _db.Updateable<SiteNews>()
            .SetColumns(x => x.ViewCount == x.ViewCount + 1)
            .Where(x => x.Id == Guid.Parse(id))
            .ExecuteCommandAsync();

        return true;
    }

    /// <summary>
    /// 获取首页新闻列表
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>新闻列表</returns>
    public async Task<List<SiteNewsDto>> GetHomeNewsListAsync(int limit = 10)
    {
        var cacheKey = "site:news:home";
        var cached = await _cacheService.GetAsync<List<SiteNewsDto>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var list = await _db.Queryable<SiteNews>()
            .Where(x => x.IsDeleted == 0 && x.Status == NewsStatus.Published)
            .OrderByDescending(x => x.IsTop)
            .ThenByDescending(x => x.PublishTime)
            .Take(limit)
            .ToListAsync();

        var dtoList = list.Adapt<List<SiteNewsDto>>();
        await _cacheService.SetAsync(cacheKey, dtoList, TimeSpan.FromMinutes(30));

        return dtoList;
    }
}
```

---

## 6. Controller 层实现指南

### 6.1 官网前端 Controller

#### SiteHomeController - 首页接口

```csharp
/// <summary>
/// 官网首页接口
/// </summary>
[ApiController]
[Route("api/site/home")]
public class SiteHomeController : BaseController
{
    private readonly ISiteBannerService _bannerService;
    private readonly ISiteNewsService _newsService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="bannerService">Banner服务</param>
    /// <param name="newsService">新闻服务</param>
    public SiteHomeController(
        ISiteBannerService bannerService,
        ISiteNewsService newsService)
    {
        _bannerService = bannerService;
        _newsService = newsService;
    }

    /// <summary>
    /// 获取首页Banner列表
    /// </summary>
    /// <returns>Banner列表</returns>
    [HttpGet("banners")]
    [ProducesResponseType(typeof(ApiResponse<List<SiteBannerDto>>), 200)]
    public async Task<IActionResult> GetBanners()
    {
        var list = await _bannerService.GetEnabledBannerListAsync();
        return Success(list);
    }

    /// <summary>
    /// 获取首页新闻列表
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>新闻列表</returns>
    [HttpGet("news")]
    [ProducesResponseType(typeof(ApiResponse<List<SiteNewsDto>>), 200)]
    public async Task<IActionResult> GetNews(int limit = 10)
    {
        var list = await _newsService.GetHomeNewsListAsync(limit);
        return Success(list);
    }
}
```

#### SiteNewsController - 新闻接口

```csharp
/// <summary>
/// 官网新闻接口
/// </summary>
[ApiController]
[Route("api/site/news")]
public class SiteNewsController : BaseController
{
    private readonly ISiteNewsService _newsService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="newsService">新闻服务</param>
    public SiteNewsController(ISiteNewsService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>
    /// 获取新闻列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻分页列表</returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResult<SiteNewsDto>>), 200)]
    public async Task<IActionResult> GetList([FromQuery] SiteNewsQuery query)
    {
        // 官网只显示已发布的新闻
        query.Status = NewsStatus.Published;
        var result = await _newsService.GetNewsListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SiteNewsDto>), 200)]
    public async Task<IActionResult> GetById(string id)
    {
        var news = await _newsService.GetNewsByIdAsync(id);

        // 官网只能查看已发布的新闻
        if (news.Status != NewsStatus.Published)
        {
            throw BusinessException.NotFound("新闻不存在");
        }

        // 增加浏览次数
        await _newsService.IncrementViewCountAsync(id);

        return Success(news);
    }
}
```

#### SiteCategoryController - 分类接口

```csharp
/// <summary>
/// 官网分类接口
/// </summary>
[ApiController]
[Route("api/site/category")]
public class SiteCategoryController : BaseController
{
    private readonly ISiteCategoryService _categoryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="categoryService">分类服务</param>
    public SiteCategoryController(ISiteCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// 获取分类列表
    /// </summary>
    /// <param name="type">分类类型</param>
    /// <returns>分类列表</returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<List<SiteCategoryDto>>), 200)]
    public async Task<IActionResult> GetList([FromQuery] string type = "news")
    {
        var list = await _categoryService.GetCategoryTreeAsync(type);
        return Success(list);
    }
}
```

#### SiteInquiryController - 在线咨询接口

```csharp
/// <summary>
/// 官网在线咨询接口
/// </summary>
[ApiController]
[Route("api/site")]
public class SiteInquiryController : BaseController
{
    private readonly ISiteInquiryService _inquiryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="inquiryService">咨询服务</param>
    public SiteInquiryController(ISiteInquiryService inquiryService)
    {
        _inquiryService = inquiryService;
    }

    /// <summary>
    /// 提交在线咨询
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>咨询ID</returns>
    [HttpPost("inquiry")]
    [EnableRateLimiting("IpRateLimit")] // IP 限流
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Submit([FromBody] SiteInquiryCreateDto dto)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var id = await _inquiryService.SubmitInquiryAsync(dto, ip);
        return Success(id, "提交成功");
    }
}
```

### 6.2 管理后台 Controller

#### AdminSiteNewsController - 新闻管理接口

```csharp
/// <summary>
/// 新闻管理接口
/// </summary>
[ApiController]
[Route("api/admin/site-news")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class AdminSiteNewsController : BaseController
{
    private readonly ISiteNewsService _newsService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="newsService">新闻服务</param>
    public AdminSiteNewsController(ISiteNewsService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>
    /// 获取新闻管理列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻分页列表</returns>
    [HttpGet("list")]
    [Permission("site:news:view")]
    [ProducesResponseType(typeof(ApiResponse<PageResult<SiteNewsDto>>), 200)]
    public async Task<IActionResult> GetList([FromQuery] SiteNewsQuery query)
    {
        var result = await _newsService.GetNewsListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    [HttpGet("{id}")]
    [Permission("site:news:view")]
    [ProducesResponseType(typeof(ApiResponse<SiteNewsDto>), 200)]
    public async Task<IActionResult> GetById(string id)
    {
        var news = await _newsService.GetNewsByIdAsync(id);
        return Success(news);
    }

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新闻ID</returns>
    [HttpPost]
    [Permission("site:news:create")]
    [OperateLog(Module = "site", Target = "news", Action = "create")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Create([FromBody] SiteNewsCreateDto dto)
    {
        var id = await _newsService.CreateNewsAsync(dto);
        return Success(id, "创建成功");
    }

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    [Permission("site:news:edit")]
    [OperateLog(Module = "site", Target = "news", Action = "update")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Update(string id, [FromBody] SiteNewsUpdateDto dto)
    {
        dto.Id = id;
        var result = await _newsService.UpdateNewsAsync(dto);
        return Success(result, "更新成功");
    }

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    [HttpDelete("{id}")]
    [Permission("site:news:delete")]
    [OperateLog(Module = "site", Target = "news", Action = "delete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _newsService.DeleteNewsAsync(id);
        return Success(result, "删除成功");
    }
}
```

#### AdminSiteCategoryController - 分类管理接口

```csharp
/// <summary>
/// 分类管理接口
/// </summary>
[ApiController]
[Route("api/admin/site-category")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class AdminSiteCategoryController : BaseController
{
    private readonly ISiteCategoryService _categoryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="categoryService">分类服务</param>
    public AdminSiteCategoryController(ISiteCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// 获取分类树
    /// </summary>
    /// <param name="type">分类类型</param>
    /// <returns>分类树</returns>
    [HttpGet("tree")]
    [Permission("site:category:view")]
    [ProducesResponseType(typeof(ApiResponse<List<SiteCategoryDto>>), 200)]
    public async Task<IActionResult> GetTree([FromQuery] string type = "news")
    {
        var list = await _categoryService.GetCategoryTreeAsync(type);
        return Success(list);
    }

    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>分类ID</returns>
    [HttpPost]
    [Permission("site:category:create")]
    [OperateLog(Module = "site", Target = "category", Action = "create")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Create([FromBody] SiteCategoryCreateDto dto)
    {
        var id = await _categoryService.CreateCategoryAsync(dto);
        return Success(id, "创建成功");
    }

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    [Permission("site:category:edit")]
    [OperateLog(Module = "site", Target = "category", Action = "update")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Update(string id, [FromBody] SiteCategoryCreateDto dto)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, dto);
        return Success(result, "更新成功");
    }

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    [HttpDelete("{id}")]
    [Permission("site:category:delete")]
    [OperateLog(Module = "site", Target = "category", Action = "delete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return Success(result, "删除成功");
    }
}
```

---

## 7. 数据库表设计

### 7.1 表结构 SQL

```sql
-- ============================================
-- Site 模块数据库表设计
-- 数据库：MySQL 8.0
-- 字符集：utf8mb4
-- 表前缀：site_
-- ============================================

-- ============================================
-- 1. 分类表
-- ============================================
CREATE TABLE `site_category` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `name` VARCHAR(50) NOT NULL COMMENT '分类名称',
  `name_en` VARCHAR(50) DEFAULT NULL COMMENT '分类名称（英文）',
  `parent_id` VARCHAR(36) DEFAULT NULL COMMENT '父分类ID',
  `type` VARCHAR(20) NOT NULL DEFAULT 'news' COMMENT '分类类型（news-新闻，product-产品，video-视频，download-下载）',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序序号',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态（0-禁用，1-启用）',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`),
  KEY `idx_type_status` (`type`, `status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='分类表';

-- ============================================
-- 2. 新闻资讯表
-- ============================================
CREATE TABLE `site_news` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `category_id` VARCHAR(36) NOT NULL COMMENT '分类ID',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '标题（英文）',
  `summary` VARCHAR(500) NOT NULL COMMENT '摘要',
  `content` TEXT NOT NULL COMMENT '正文内容（HTML）',
  `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
  `is_top` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否置顶',
  `view_count` INT NOT NULL DEFAULT 0 COMMENT '浏览次数',
  `publish_time` DATETIME NOT NULL COMMENT '发布时间',
  `status` INT NOT NULL DEFAULT 0 COMMENT '状态（0-草稿，1-已发布，2-已归档）',
  `seo_keywords` VARCHAR(200) DEFAULT NULL COMMENT 'SEO关键词',
  `seo_description` VARCHAR(500) DEFAULT NULL COMMENT 'SEO描述',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`),
  KEY `idx_category_id` (`category_id`),
  KEY `idx_status_publish_time` (`status`, `publish_time`),
  KEY `idx_is_top` (`is_top`),
  FULLTEXT KEY `ft_title_summary` (`title`, `summary`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='新闻资讯表';

-- ============================================
-- 3. Banner轮播图表
-- ============================================
CREATE TABLE `site_banner` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(100) NOT NULL COMMENT '标题',
  `image_url` VARCHAR(500) NOT NULL COMMENT '图片URL',
  `link_url` VARCHAR(500) DEFAULT NULL COMMENT '链接URL',
  `target` VARCHAR(10) NOT NULL DEFAULT '_blank' COMMENT '打开方式（_blank-新窗口，_self-当前窗口）',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序序号',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态（0-禁用，1-启用）',
  `start_time` DATETIME DEFAULT NULL COMMENT '开始时间',
  `end_time` DATETIME DEFAULT NULL COMMENT '结束时间',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`),
  KEY `idx_status_sort` (`status`, `sort`),
  KEY `idx_time_range` (`start_time`, `end_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Banner轮播图表';

-- ============================================
-- 4. 视频表
-- ============================================
CREATE TABLE `site_video` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '标题（英文）',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '描述',
  `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
  `video_url` VARCHAR(500) NOT NULL COMMENT '视频URL',
  `duration` INT NOT NULL DEFAULT 0 COMMENT '时长（秒）',
  `view_count` INT NOT NULL DEFAULT 0 COMMENT '浏览次数',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序序号',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态（0-禁用，1-启用）',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`),
  KEY `idx_status_sort` (`status`, `sort`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='视频表';

-- ============================================
-- 5. 下载资源表
-- ============================================
CREATE TABLE `site_download` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '标题（英文）',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '描述',
  `file_url` VARCHAR(500) NOT NULL COMMENT '文件URL',
  `file_size` BIGINT NOT NULL DEFAULT 0 COMMENT '文件大小（字节）',
  `file_type` VARCHAR(20) NOT NULL COMMENT '文件类型（扩展名）',
  `download_count` INT NOT NULL DEFAULT 0 COMMENT '下载次数',
  `sort` INT NOT NULL DEFAULT 0 COMMENT '排序序号',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态（0-禁用，1-启用）',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`),
  KEY `idx_status_sort` (`status`, `sort`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='下载资源表';

-- ============================================
-- 6. 联系方式表
-- ============================================
CREATE TABLE `site_contact` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `company_name` VARCHAR(100) NOT NULL COMMENT '公司名称',
  `address` VARCHAR(200) NOT NULL COMMENT '地址',
  `phone` VARCHAR(50) NOT NULL COMMENT '电话',
  `email` VARCHAR(50) NOT NULL COMMENT '邮箱',
  `fax` VARCHAR(50) DEFAULT NULL COMMENT '传真',
  `wechat` VARCHAR(50) DEFAULT NULL COMMENT '微信号',
  `weibo` VARCHAR(50) DEFAULT NULL COMMENT '微博号',
  `map_url` VARCHAR(500) DEFAULT NULL COMMENT '地图链接URL',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='联系方式表';

-- ============================================
-- 7. 在线咨询表
-- ============================================
CREATE TABLE `site_inquiry` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `name` VARCHAR(50) NOT NULL COMMENT '姓名',
  `phone` VARCHAR(20) NOT NULL COMMENT '手机号',
  `email` VARCHAR(50) DEFAULT NULL COMMENT '邮箱',
  `company` VARCHAR(100) DEFAULT NULL COMMENT '公司名称',
  `content` TEXT NOT NULL COMMENT '咨询内容',
  `status` INT NOT NULL DEFAULT 0 COMMENT '处理状态（0-待处理，1-已处理）',
  `processed_by` VARCHAR(50) DEFAULT NULL COMMENT '处理人',
  `processed_time` DATETIME DEFAULT NULL COMMENT '处理时间',
  `processed_remark` VARCHAR(500) DEFAULT NULL COMMENT '处理备注',
  `ip` VARCHAR(50) NOT NULL COMMENT '访客IP',
  `source_url` VARCHAR(200) DEFAULT NULL COMMENT '来源页面',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_ip` (`ip`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='在线咨询表';

-- ============================================
-- 8. 关于我们表
-- ============================================
CREATE TABLE `site_about` (
  `id` VARCHAR(36) NOT NULL COMMENT '主键ID',
  `description` TEXT NOT NULL COMMENT '公司简介',
  `description_en` TEXT DEFAULT NULL COMMENT '公司简介（英文）',
  `milestones` TEXT DEFAULT NULL COMMENT '发展历程（JSON格式）',
  `honors` TEXT DEFAULT NULL COMMENT '企业荣誉（JSON格式）',
  `culture` TEXT DEFAULT NULL COMMENT '企业文化（JSON格式）',
  `images` TEXT DEFAULT NULL COMMENT '图片列表（JSON格式）',
  `create_time` DATETIME NOT NULL COMMENT '创建时间',
  `update_time` DATETIME NOT NULL COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '软删除标记（0-未删除，1-已删除）',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='关于我们表';

-- ============================================
-- 初始数据
-- ============================================

-- 插入默认联系方式
INSERT INTO `site_contact` (`id`, `company_name`, `address`, `phone`, `email`, `create_time`, `update_time`)
VALUES
(UUID(), '示例公司', '上海市浦东新区张江高科技园区', '400-888-8888', 'contact@example.com', NOW(), NOW());

-- 插入默认关于我们
INSERT INTO `site_about` (`id`, `description`, `create_time`, `update_time`)
VALUES
(UUID(), '这是一家专注于创新和卓越的企业。', NOW(), NOW());
```

### 7.2 索引说明

| 表名 | 索引名 | 索引字段 | 说明 |
|------|-------|---------|------|
| site_category | idx_parent_id | parent_id | 父分类查询 |
| site_category | idx_type_status | type, status | 分类类型和状态查询 |
| site_news | idx_category_id | category_id | 分类查询 |
| site_news | idx_status_publish_time | status, publish_time | 状态和时间范围查询 |
| site_news | idx_is_top | is_top | 置顶查询 |
| site_news | ft_title_summary | title, summary | 全文检索 |
| site_banner | idx_status_sort | status, sort | 状态和排序查询 |
| site_inquiry | idx_status | status | 状态查询 |
| site_inquiry | idx_create_time | create_time | 时间范围查询 |

---

## 8. 业务逻辑要点

### 8.1 SEO 优化策略

#### 标题和元信息
```csharp
/// <summary>
/// 新闻实体需要包含SEO字段
/// </summary>
public class SiteNews : BaseEntity
{
    /// <summary>
    /// SEO关键词（用于 meta keywords）
    /// </summary>
    public string? SeoKeywords { get; set; }

    /// <summary>
    /// SEO描述（用于 meta description）
    /// </summary>
    public string? SeoDescription { get; set; }
}
```

#### URL 规范
- 新闻详情页：`/news/{id}` 或 `/news/{slug}`（可考虑 URL 友好的 slug）
- 分类页：`/category/{type}/{id}`
- 使用语义化的 URL 路径

#### 结构化数据
```json
{
  "@context": "https://schema.org",
  "@type": "NewsArticle",
  "headline": "新闻标题",
  "datePublished": "2026-09-07T10:00:00",
  "author": {
    "@type": "Organization",
    "name": "公司名称"
  }
}
```

### 8.2 CDN 缓存策略

#### 缓存规则

| 内容类型 | 缓存时间 | 缓存键 | 失效策略 |
|---------|---------|--------|---------|
| Banner 列表 | 30 分钟 | `site:banner:list` | 手动清除 |
| 首页新闻 | 30 分钟 | `site:news:home` | 手动清除 |
| 新闻详情 | 10 分钟 | `site:news:{id}` | 更新/删除时清除 |
| 分类树 | 1 小时 | `site:category:{type}` | 更新/删除时清除 |
| 联系方式 | 24 小时 | `site:contact` | 更新时清除 |
| 关于我们 | 24 小时 | `site:about` | 更新时清除 |

#### 缓存实现示例

```csharp
/// <summary>
/// 获取新闻详情（带缓存）
/// </summary>
public async Task<SiteNewsDto> GetNewsByIdAsync(string id)
{
    var cacheKey = $"site:news:{id}";

    // 尝试从缓存获取
    var cached = await _cacheService.GetAsync<SiteNewsDto>(cacheKey);
    if (cached != null)
    {
        return cached;
    }

    // 从数据库查询
    var entity = await _db.Queryable<SiteNews>()
        .Where(x => x.Id == Guid.Parse(id) && x.IsDeleted == 0)
        .FirstAsync();

    if (entity == null)
    {
        throw BusinessException.NotFound("新闻不存在");
    }

    var dto = entity.Adapt<SiteNewsDto>();

    // 写入缓存
    await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

    return dto;
}
```

#### 缓存失效

```csharp
/// <summary>
/// 更新新闻（清除缓存）
/// </summary>
public async Task<bool> UpdateNewsAsync(SiteNewsUpdateDto dto)
{
    // ... 更新逻辑 ...

    // 清除相关缓存
    await _cacheService.RemoveAsync($"site:news:{dto.Id}");
    await _cacheService.RemoveAsync("site:news:home");

    return true;
}
```

### 8.3 限流策略

#### IP 限流配置

```csharp
// Program.cs 中的限流配置
services.AddRateLimiter(options =>
{
    options.AddPolicy("IpRateLimit", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetSlidingWindowLimiter(
            ip,
            factory => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 10, // 每个 IP 10 次
                Window = TimeSpan.FromMinutes(1), // 1 分钟窗口
                SegmentsPerWindow = 2
            }
        );
    });
});
```

#### 限流应用

```csharp
/// <summary>
/// 提交在线咨询（IP 限流）
/// </summary>
[HttpPost("inquiry")]
[EnableRateLimiting("IpRateLimit")] // 启用 IP 限流
public async Task<IActionResult> Submit([FromBody] SiteInquiryCreateDto dto)
{
    // ... 提交逻辑 ...
}
```

### 8.4 内容安全

#### XSS 防护
- 新闻内容使用富文本编辑器，前端需要做 XSS 过滤
- 后端存储前进行 HTML 净化
- 前端展示时使用 `v-html` 配合 DOMPurify

#### SQL 注入防护
- 使用 SqlSugar 参数化查询
- 禁止字符串拼接 SQL
- 用户输入使用 DataAnnotations 校验

#### 敏感信息保护
- 联系方式、关于我们等页面考虑防止爬虫
- 访客留言信息加密存储（手机号、邮箱）

### 8.5 多语言支持

#### 字段设计
- 关键字段提供中英文版本
- 使用 `title` / `title_en` 命名规范

#### 前端适配
- 根据语言环境选择显示字段
- 提供 `Accept-Language` 请求头

#### 后端响应
```csharp
/// <summary>
/// 根据语言返回标题
/// </summary>
public string GetLocalizedTitle(SiteNewsDto news, string language)
{
    return language == "en" && !string.IsNullOrEmpty(news.TitleEn)
        ? news.TitleEn
        : news.Title;
}
```

### 8.6 内容审核

#### 发布审核流程
1. 草稿状态（`draft`）
2. 提交审核
3. 审核通过 → 发布（`published`）
4. 审核拒绝 → 退回修改

#### 状态流转

```
草稿 → 已发布 → 已归档
 ↓       ↑
审核中   退回
```

---

## 9. 单元测试要求

### 9.1 测试范围

Site 模块单元测试不强制要求，但以下场景建议编写测试：

1. **分类树构建**：多级分类的树形结构生成
2. **内容查询**：分页、筛选、排序逻辑
3. **缓存逻辑**：缓存命中、失效、更新
4. **限流逻辑**：IP 限流是否生效

### 9.2 测试示例

#### 分类树构建测试

```csharp
/// <summary>
/// 分类服务测试
/// </summary>
public class SiteCategoryServiceTests
{
    private readonly ISqlSugarClient _db;
    private readonly ISiteCategoryService _service;

    public SiteCategoryServiceTests()
    {
        // 初始化测试数据库
        _db = CreateTestDatabase();
        _service = new SiteCategoryService(_db, Mock.Of<ILogger<SiteCategoryService>>());
    }

    /// <summary>
    /// 测试：获取分类树（多级分类）
    /// </summary>
    [Fact]
    public async Task GetCategoryTreeAsync_MultiLevel_ReturnsTree()
    {
        // Given: 准备测试数据
        var parentId = Guid.NewGuid().ToString();
        var childId = Guid.NewGuid().ToString();

        await _db.Insertable(new SiteCategory
        {
            Id = Guid.Parse(parentId),
            Name = "父分类",
            Type = "news",
            Status = "enabled"
        }).ExecuteCommandAsync();

        await _db.Insertable(new SiteCategory
        {
            Id = Guid.Parse(childId),
            Name = "子分类",
            ParentId = parentId,
            Type = "news",
            Status = "enabled"
        }).ExecuteCommandAsync();

        // When: 执行测试
        var result = await _service.GetCategoryTreeAsync("news");

        // Then: 验证结果
        Assert.Single(result);
        Assert.Single(result[0].Children);
        Assert.Equal("父分类", result[0].Name);
        Assert.Equal("子分类", result[0].Children[0].Name);
    }

    /// <summary>
    /// 测试：删除有子分类的分类失败
    /// </summary>
    [Fact]
    public async Task DeleteCategoryAsync_HasChildren_ThrowsException()
    {
        // Given: 准备有子分类的父分类
        var parentId = Guid.NewGuid().ToString();

        await _db.Insertable(new SiteCategory
        {
            Id = Guid.Parse(parentId),
            Name = "父分类",
            Type = "news"
        }).ExecuteCommandAsync();

        await _db.Insertable(new SiteCategory
        {
            Id = Guid.NewGuid(),
            Name = "子分类",
            ParentId = parentId,
            Type = "news"
        }).ExecuteCommandAsync();

        // When & Then: 验证删除失败
        await Assert.ThrowsAsync<BusinessException>(
            () => _service.DeleteCategoryAsync(parentId)
        );
    }
}
```

#### 缓存逻辑测试

```csharp
/// <summary>
/// 新闻服务缓存测试
/// </summary>
public class SiteNewsCacheTests
{
    private readonly ISqlSugarClient _db;
    private readonly ICacheService _cache;
    private readonly ISiteNewsService _service;

    public SiteNewsCacheTests()
    {
        _db = CreateTestDatabase();
        _cache = new MemoryCacheService(); // 使用内存缓存测试
        _service = new SiteNewsService(_db, Mock.Of<ILogger<SiteNewsService>>(), _cache);
    }

    /// <summary>
    /// 测试：首次查询从数据库加载
    /// </summary>
    [Fact]
    public async Task GetNewsByIdAsync_FirstTime_LoadsFromDatabase()
    {
        // Given: 准备测试数据
        var newsId = Guid.NewGuid();
        await _db.Insertable(new SiteNews
        {
            Id = newsId,
            CategoryId = Guid.NewGuid(),
            Title = "测试新闻",
            Summary = "摘要",
            Content = "内容",
            PublishTime = DateTime.Now,
            Status = "published"
        }).ExecuteCommandAsync();

        // When: 首次查询
        var result = await _service.GetNewsByIdAsync(newsId.ToString());

        // Then: 结果正确
        Assert.NotNull(result);
        Assert.Equal("测试新闻", result.Title);

        // And: 缓存已存在
        var cached = await _cache.GetAsync<SiteNewsDto>($"site:news:{newsId}");
        Assert.NotNull(cached);
    }

    /// <summary>
    /// 测试：第二次查询从缓存加载
    /// </summary>
    [Fact]
    public async Task GetNewsByIdAsync_SecondTime_LoadsFromCache()
    {
        // Given: 准备测试数据和缓存
        var newsId = Guid.NewGuid();
        var cacheKey = $"site:news:{newsId}";

        await _cache.SetAsync(cacheKey, new SiteNewsDto
        {
            Id = newsId.ToString(),
            Title = "缓存新闻"
        });

        // When: 查询
        var result = await _service.GetNewsByIdAsync(newsId.ToString());

        // Then: 从缓存加载
        Assert.Equal("缓存新闻", result.Title);
    }
}
```

### 9.3 测试覆盖率

建议测试覆盖率：
- 分类服务：≥ 80%
- 新闻服务：≥ 70%
- 其他服务：≥ 60%

---

## 10. 开发检查清单

### 10.1 代码质量检查

- [ ] 所有公开方法添加中文 XML 注释
- [ ] Controller 添加 `[ProducesResponseType]` 特性
- [ ] DTO 字段添加 `[Required]` / `[MaxLength]` 等校验特性
- [ ] Service 层使用构造器注入（禁止属性注入）
- [ ] 禁止在 Controller 中手写 try/catch
- [ ] 所有查询使用参数化（禁止字符串拼接）
- [ ] 状态字段使用小写字符串常量

### 10.2 功能完整性检查

- [ ] 实现所有 CRUD 接口
- [ ] 管理端接口添加 `[Authorize]` 和 `[Permission]` 特性
- [ ] 管理端写操作添加 `[OperateLog]` 特性
- [ ] 官网前端接口支持匿名访问
- [ ] 提交类接口添加 IP 限流
- [ ] 新闻详情页增加浏览次数
- [ ] 下载资源增加下载次数

### 10.3 SEO 优化检查

- [ ] 新闻实体包含 SEO 字段
- [ ] 新闻内容支持全文检索
- [ ] 关键页面设置 meta 信息
- [ ] URL 路径语义化

### 10.4 性能优化检查

- [ ] 高频访问内容添加缓存
- [ ] 缓存键命名规范
- [ ] 更新操作清除相关缓存
- [ ] 数据库索引创建正确
- [ ] 分页查询使用 `Skip/Take`

### 10.5 安全性检查

- [ ] 富文本内容 XSS 过滤
- [ ] 用户输入校验
- [ ] SQL 注入防护
- [ ] 敏感信息保护

### 10.6 测试检查

- [ ] 分类树构建测试通过
- [ ] 缓存逻辑测试通过
- [ ] 分页查询测试通过
- [ ] 状态流转测试通过

### 10.7 文档检查

- [ ] Swagger XML 注释完整
- [ ] 接口返回类型标注正确
- [ ] 数据库表注释完整
- [ ] API 文档归档（`docs/api/swagger-site.json`）

### 10.8 提交前检查

- [ ] `dotnet build` 0 错误 0 新警告
- [ ] `dotnet test` 通过
- [ ] Commit 遵循 `feat(api):` / `fix(api):` 格式
- [ ] 提交说明清晰准确

---

## 附录 A：权限标识

| 权限标识 | 说明 | 对应菜单 |
|---------|------|---------|
| `site:news:view` | 查看新闻 | 新闻管理 |
| `site:news:create` | 创建新闻 | 新闻管理 |
| `site:news:edit` | 编辑新闻 | 新闻管理 |
| `site:news:delete` | 删除新闻 | 新闻管理 |
| `site:category:view` | 查看分类 | 分类管理 |
| `site:category:create` | 创建分类 | 分类管理 |
| `site:category:edit` | 编辑分类 | 分类管理 |
| `site:category:delete` | 删除分类 | 分类管理 |
| `site:banner:view` | 查看 Banner | Banner 管理 |
| `site:banner:create` | 创建 Banner | Banner 管理 |
| `site:banner:edit` | 编辑 Banner | Banner 管理 |
| `site:banner:delete` | 删除 Banner | Banner 管理 |
| `site:video:view` | 查看视频 | 视频管理 |
| `site:video:create` | 创建视频 | 视频管理 |
| `site:video:edit` | 编辑视频 | 视频管理 |
| `site:video:delete` | 删除视频 | 视频管理 |
| `site:download:view` | 查看下载 | 下载管理 |
| `site:download:create` | 创建下载 | 下载管理 |
| `site:download:edit` | 编辑下载 | 下载管理 |
| `site:download:delete` | 删除下载 | 下载管理 |
| `site:inquiry:view` | 查看咨询 | 咨询管理 |
| `site:inquiry:process` | 处理咨询 | 咨询管理 |
| `site:contact:edit` | 编辑联系方式 | 联系方式 |
| `site:about:edit` | 编辑关于我们 | 关于我们 |

---

## 附录 B：状态常量

```csharp
/// <summary>
/// Site 模块状态常量
/// </summary>
public static class SiteStatus
{
    /// <summary>
    /// 禁用
    /// </summary>
    public const int Disabled = 0;

    /// <summary>
    /// 启用
    /// </summary>
    public const int Enabled = 1;
}

/// <summary>
/// 新闻状态常量
/// </summary>
public static class NewsStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    public const int Draft = 0;

    /// <summary>
    /// 已发布
    /// </summary>
    public const int Published = 1;

    /// <summary>
    /// 已归档
    /// </summary>
    public const int Archived = 2;
}

/// <summary>
/// 咨询状态常量
/// </summary>
public static class InquiryStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    public const int Pending = 0;

    /// <summary>
    /// 已处理
    /// </summary>
    public const int Processed = 1;
}

/// <summary>
/// 分类类型常量
/// </summary>
public static class CategoryType
{
    /// <summary>
    /// 新闻
    /// </summary>
    public const string News = "news";

    /// <summary>
    /// 产品
    /// </summary>
    public const string Product = "product";

    /// <summary>
    /// 视频
    /// </summary>
    public const string Video = "video";

    /// <summary>
    /// 下载
    /// </summary>
    public const string Download = "download";
}
```

---

## 附录 C：注意事项

### C.1 依赖注入规范

**重要：遵循项目规范，使用构造器注入，禁止属性注入。**

虽然用户在需求中提到"使用属性注入"，但根据项目规范（`docs/backend-guidelines.md` 第 4.2 节），EasyProduct 项目明确要求：

> **一律构造器注入**。EasyWechatWeb 的属性注入写法（`public IXxxService _xxx { get; set; } = null!;`）在迁入时全部改写。

因此，Site 模块开发必须使用构造器注入：

```csharp
// ✅ 正确：构造器注入
public class SiteNewsService : BaseService, ISiteNewsService
{
    private readonly ILogger<SiteNewsService> _logger;
    private readonly ICacheService _cacheService;

    public SiteNewsService(
        ISqlSugarClient db,
        ILogger<SiteNewsService> logger,
        ICacheService cacheService) : base(db)
    {
        _logger = logger;
        _cacheService = cacheService;
    }
}

// ❌ 错误：属性注入（禁止）
public class SiteNewsService : BaseService, ISiteNewsService
{
    public ILogger<SiteNewsService> _logger { get; set; } = null!;
    public ICacheService _cacheService { get; set; } = null!;
}
```

### C.2 方法注释规范

**所有公开方法必须添加中文 XML 注释**，包括：
- `<summary>`：功能简述
- `<param>`：参数说明
- `<returns>`：返回值说明
- `<remarks>`：详细说明（可选）
- `<example>`：使用示例（可选）

示例：

```csharp
/// <summary>
/// 获取新闻分页列表
/// </summary>
/// <param name="query">查询参数，包含分页、分类、关键词等</param>
/// <returns>新闻分页结果</returns>
/// <remarks>
/// 1. 支持按分类、状态、置顶筛选
/// 2. 支持关键词模糊搜索（标题、摘要）
/// 3. 默认按置顶优先、发布时间倒序排列
/// </remarks>
public async Task<PageResult<SiteNewsDto>> GetNewsListAsync(SiteNewsQuery query)
{
    // 实现代码...
}
```

---

**文档版本：** 1.0
**最后更新：** 2026-09-07
**维护者：** EasyProduct 开发团队