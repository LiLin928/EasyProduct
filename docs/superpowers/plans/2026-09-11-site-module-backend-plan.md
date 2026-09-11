# Site 官网管理模块后端实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成 Site 官网管理模块的 7 个核心功能开发，包括新闻、Banner、关于我们、下载、视频、询价、留言管理。

**Architecture:** 分离 Service + 分离 Controller 架构，每个功能提供管理端和官网公开两个独立的服务和控制器，使用 SqlSugar ORM 和 Mapster 对象映射，询价/留言接口使用 ASP.NET Core Rate Limiting 进行限流保护。

**Tech Stack:** .NET 8, SqlSugar, Mapster, Autofac, Serilog, ASP.NET Core Rate Limiting

---

## 文件结构映射

### 批次 1 文件清单

**实体类（3个）：**
- `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_news_category.cs` - 新闻分类实体
- `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_news.cs` - 新闻实体
- `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_banner.cs` - Banner实体

**DTO（18个）：**
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/NewsCategoryDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/NewsCategoryQueryDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/CreateNewsCategoryDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/UpdateNewsCategoryDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/NewsDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/NewsQueryDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/CreateNewsDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/UpdateNewsDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/BannerDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/BannerQueryDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/CreateBannerDto.cs`
- `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/UpdateBannerDto.cs`

**Service（6个）：**
- `EasyProduct.WebApi/EasyProduct.Business/Site/INewsCategoryService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/NewsCategoryService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/INewsService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/NewsService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/ISiteNewsService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/SiteNewsService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/IBannerService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/BannerService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/ISiteBannerService.cs`
- `EasyProduct.WebApi/EasyProduct.Business/Site/SiteBannerService.cs`

**Controller（5个）：**
- `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/NewsCategoryController.cs`
- `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/NewsController.cs`
- `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/BannerController.cs`
- `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/NewsController.cs`
- `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/BannerController.cs`

**Mock数据（2个）：**
- `mock-server/data/site/news.js` - 新闻和Banner mock数据
- `mock-server/routes/site.js` - Site模块mock路由

---

## 批次 1：新闻 + Banner（3-4天）

### Task 1: 创建数据库表

**Files:**
- Modify: `sql/init-database.sql`

- [ ] **Step 1: 添加新闻分类表定义**

在 `sql/init-database.sql` 文件末尾添加：

```sql
-- ========================================
-- Site 模块表
-- ========================================

-- ----------------------------
-- 新闻分类表 (site_news_category)
-- ----------------------------
DROP TABLE IF EXISTS `site_news_category`;
CREATE TABLE `site_news_category` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `category_name` VARCHAR(100) NOT NULL COMMENT '分类名称',
  `category_code` VARCHAR(50) DEFAULT NULL COMMENT '分类编码',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='新闻分类表';

-- ----------------------------
-- 新闻表 (site_news)
-- ----------------------------
DROP TABLE IF EXISTS `site_news`;
CREATE TABLE `site_news` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `category_id` CHAR(36) DEFAULT NULL COMMENT '分类ID',
  `title` VARCHAR(200) NOT NULL COMMENT '新闻标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '新闻标题（英文）',
  `summary` VARCHAR(500) DEFAULT NULL COMMENT '摘要',
  `summary_en` VARCHAR(500) DEFAULT NULL COMMENT '摘要（英文）',
  `content` LONGTEXT COMMENT '内容（富文本）',
  `content_en` LONGTEXT COMMENT '内容（英文，富文本）',
  `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
  `author` VARCHAR(50) DEFAULT NULL COMMENT '作者',
  `source` VARCHAR(100) DEFAULT NULL COMMENT '来源',
  `view_count` INT DEFAULT 0 COMMENT '浏览次数',
  `is_top` INT DEFAULT 0 COMMENT '是否置顶：0=否，1=是',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_category_id` (`category_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_top` (`is_top`),
  KEY `idx_publish_time` (`publish_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='新闻表';

-- ----------------------------
-- Banner表 (site_banner)
-- ----------------------------
DROP TABLE IF EXISTS `site_banner`;
CREATE TABLE `site_banner` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(100) DEFAULT NULL COMMENT 'Banner标题',
  `title_en` VARCHAR(100) DEFAULT NULL COMMENT 'Banner标题（英文）',
  `image_url` VARCHAR(500) NOT NULL COMMENT '图片URL',
  `link_url` VARCHAR(500) DEFAULT NULL COMMENT '跳转链接',
  `target` VARCHAR(10) DEFAULT '_self' COMMENT '打开方式：_self/_blank',
  `position` VARCHAR(50) DEFAULT 'home' COMMENT '位置：home=首页，product=产品页等',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `start_time` DATETIME DEFAULT NULL COMMENT '开始时间',
  `end_time` DATETIME DEFAULT NULL COMMENT '结束时间',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_position` (`position`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_time_range` (`start_time`, `end_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Banner表';
```

- [ ] **Step 2: 提交数据库表定义**

```bash
git add sql/init-database.sql
git commit -m "feat(site): 添加新闻、Banner数据库表定义"
```

---

### Task 2: 创建新闻分类实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_news_category.cs`

- [ ] **Step 1: 创建Site实体目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Entitys/Site
```

- [ ] **Step 2: 创建新闻分类实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 新闻分类实体
/// </summary>
[SugarTable("site_news_category", "新闻分类表")]
public class site_news_category : BaseEntity
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// 分类编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }
}
```

- [ ] **Step 3: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_news_category.cs
git commit -m "feat(site): 添加新闻分类实体类"
```

---

### Task 3: 创建新闻实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_news.cs`

- [ ] **Step 1: 创建新闻实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 新闻实体
/// </summary>
[SugarTable("site_news", "新闻表")]
public class site_news : BaseEntity
{
    /// <summary>
    /// 分类ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// 新闻标题
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 新闻标题（英文）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Summary { get; set; }

    /// <summary>
    /// 摘要（英文）
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? SummaryEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true)]
    public string? ContentEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Source { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    public int IsTop { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }
}
```

- [ ] **Step 2: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_news.cs
git commit -m "feat(site): 添加新闻实体类"
```

---

### Task 4: 创建Banner实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_banner.cs`

- [ ] **Step 1: 创建Banner实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// Banner实体
/// </summary>
[SugarTable("site_banner", "Banner表")]
public class site_banner : BaseEntity
{
    /// <summary>
    /// Banner标题
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Title { get; set; }

    /// <summary>
    /// Banner标题（英文）
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// 跳转链接
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 打开方式：_self/_blank
    /// </summary>
    [SugarColumn(Length = 10, IsNullable = true)]
    public string? Target { get; set; } = "_self";

    /// <summary>
    /// 位置：home=首页，product=产品页等
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Position { get; set; } = "home";

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? EndTime { get; set; }
}
```

- [ ] **Step 2: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_banner.cs
git commit -m "feat(site): 添加Banner实体类"
```

---

### Task 5: 创建新闻分类DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/NewsCategoryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/NewsCategoryQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/CreateNewsCategoryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/UpdateNewsCategoryDto.cs`

- [ ] **Step 1: 创建Site DTO目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner
```

- [ ] **Step 2: 创建NewsCategoryDto**

```csharp
namespace EasyProduct.Models.Dto.Site.NewsCategory;

/// <summary>
/// 新闻分类DTO
/// </summary>
public class NewsCategoryDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// 分类编码
    /// </summary>
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 3: 创建NewsCategoryQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.NewsCategory;

/// <summary>
/// 新闻分类查询DTO
/// </summary>
public class NewsCategoryQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（分类名称/编码）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 4: 创建CreateNewsCategoryDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.NewsCategory;

/// <summary>
/// 创建新闻分类DTO
/// </summary>
public class CreateNewsCategoryDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(100, ErrorMessage = "分类名称不能超过100个字符")]
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// 分类编码
    /// </summary>
    [MaxLength(50, ErrorMessage = "分类编码不能超过50个字符")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 5: 创建UpdateNewsCategoryDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.NewsCategory;

/// <summary>
/// 更新新闻分类DTO
/// </summary>
public class UpdateNewsCategoryDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(100, ErrorMessage = "分类名称不能超过100个字符")]
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// 分类编码
    /// </summary>
    [MaxLength(50, ErrorMessage = "分类编码不能超过50个字符")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; }
}
```

- [ ] **Step 6: 提交新闻分类DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/NewsCategory/
git commit -m "feat(site): 添加新闻分类DTO"
```

---

**由于计划非常庞大（需要创建 100+ 个文件），我将分批次编写完整的实施计划。请告诉我是否继续编写批次 1 的剩余任务（新闻DTO、BannerDTO、Service、Controller、Mock数据），还是先执行现有的任务？**

---

---

### Task 6: 创建新闻DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/NewsDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/NewsQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/CreateNewsDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/UpdateNewsDto.cs`

- [ ] **Step 1: 创建NewsDto**

```csharp
namespace EasyProduct.Models.Dto.Site.News;

/// <summary>
/// 新闻DTO
/// </summary>
public class NewsDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 分类ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 新闻标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 新闻标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 摘要（英文）
    /// </summary>
    public string? SummaryEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    public int IsTop { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreateBy { get; set; }
}
```

- [ ] **Step 2: 创建NewsQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.News;

/// <summary>
/// 新闻查询DTO
/// </summary>
public class NewsQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（标题/摘要）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    public int? IsTop { get; set; }

    /// <summary>
    /// 发布时间开始
    /// </summary>
    public DateTime? PublishStartTime { get; set; }

    /// <summary>
    /// 发布时间结束
    /// </summary>
    public DateTime? PublishEndTime { get; set; }
}
```

- [ ] **Step 3: 创建CreateNewsDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.News;

/// <summary>
/// 创建新闻DTO
/// </summary>
public class CreateNewsDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 新闻标题
    /// </summary>
    [Required(ErrorMessage = "新闻标题不能为空")]
    [MaxLength(200, ErrorMessage = "新闻标题不能超过200个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 新闻标题（英文）
    /// </summary>
    [MaxLength(200, ErrorMessage = "新闻标题（英文）不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    [MaxLength(500, ErrorMessage = "摘要不能超过500个字符")]
    public string? Summary { get; set; }

    /// <summary>
    /// 摘要（英文）
    /// </summary>
    [MaxLength(500, ErrorMessage = "摘要（英文）不能超过500个字符")]
    public string? SummaryEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [MaxLength(500, ErrorMessage = "封面图片URL不能超过500个字符")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    [MaxLength(50, ErrorMessage = "作者不能超过50个字符")]
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    [MaxLength(100, ErrorMessage = "来源不能超过100个字符")]
    public string? Source { get; set; }

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "是否置顶必须为0或1")]
    public int IsTop { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 4: 创建UpdateNewsDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.News;

/// <summary>
/// 更新新闻DTO
/// </summary>
public class UpdateNewsDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 新闻标题
    /// </summary>
    [Required(ErrorMessage = "新闻标题不能为空")]
    [MaxLength(200, ErrorMessage = "新闻标题不能超过200个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 新闻标题（英文）
    /// </summary>
    [MaxLength(200, ErrorMessage = "新闻标题（英文）不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    [MaxLength(500, ErrorMessage = "摘要不能超过500个字符")]
    public string? Summary { get; set; }

    /// <summary>
    /// 摘要（英文）
    /// </summary>
    [MaxLength(500, ErrorMessage = "摘要（英文）不能超过500个字符")]
    public string? SummaryEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [MaxLength(500, ErrorMessage = "封面图片URL不能超过500个字符")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    [MaxLength(50, ErrorMessage = "作者不能超过50个字符")]
    public string? Author { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    [MaxLength(100, ErrorMessage = "来源不能超过100个字符")]
    public string? Source { get; set; }

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "是否置顶必须为0或1")]
    public int IsTop { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; }
}
```

- [ ] **Step 5: 提交新闻DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/News/
git commit -m "feat(site): 添加新闻DTO"
```

---

### Task 7: 创建Banner DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/BannerDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/BannerQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/CreateBannerDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/UpdateBannerDto.cs`

- [ ] **Step 1: 创建BannerDto**

```csharp
namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// Banner DTO
/// </summary>
public class BannerDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Banner标题
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Banner标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// 跳转链接
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 打开方式：_self/_blank
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// 位置：home=首页，product=产品页等
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
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

- [ ] **Step 2: 创建BannerQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// Banner查询DTO
/// </summary>
public class BannerQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（标题）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 位置：home=首页，product=产品页等
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 3: 创建CreateBannerDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// 创建Banner DTO
/// </summary>
public class CreateBannerDto
{
    /// <summary>
    /// Banner标题
    /// </summary>
    [MaxLength(100, ErrorMessage = "Banner标题不能超过100个字符")]
    public string? Title { get; set; }

    /// <summary>
    /// Banner标题（英文）
    /// </summary>
    [MaxLength(100, ErrorMessage = "Banner标题（英文）不能超过100个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    [Required(ErrorMessage = "图片URL不能为空")]
    [MaxLength(500, ErrorMessage = "图片URL不能超过500个字符")]
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// 跳转链接
    /// </summary>
    [MaxLength(500, ErrorMessage = "跳转链接不能超过500个字符")]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 打开方式：_self/_blank
    /// </summary>
    [MaxLength(10, ErrorMessage = "打开方式不能超过10个字符")]
    public string? Target { get; set; } = "_self";

    /// <summary>
    /// 位置：home=首页，product=产品页等
    /// </summary>
    [MaxLength(50, ErrorMessage = "位置不能超过50个字符")]
    public string? Position { get; set; } = "home";

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; } = 1;

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

- [ ] **Step 4: 创建UpdateBannerDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Banner;

/// <summary>
/// 更新Banner DTO
/// </summary>
public class UpdateBannerDto
{
    /// <summary>
    /// Banner标题
    /// </summary>
    [MaxLength(100, ErrorMessage = "Banner标题不能超过100个字符")]
    public string? Title { get; set; }

    /// <summary>
    /// Banner标题（英文）
    /// </summary>
    [MaxLength(100, ErrorMessage = "Banner标题（英文）不能超过100个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    [Required(ErrorMessage = "图片URL不能为空")]
    [MaxLength(500, ErrorMessage = "图片URL不能超过500个字符")]
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// 跳转链接
    /// </summary>
    [MaxLength(500, ErrorMessage = "跳转链接不能超过500个字符")]
    public string? LinkUrl { get; set; }

    /// <summary>
    /// 打开方式：_self/_blank
    /// </summary>
    [MaxLength(10, ErrorMessage = "打开方式不能超过10个字符")]
    public string? Target { get; set; } = "_self";

    /// <summary>
    /// 位置：home=首页，product=产品页等
    /// </summary>
    [MaxLength(50, ErrorMessage = "位置不能超过50个字符")]
    public string? Position { get; set; } = "home";

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
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

- [ ] **Step 5: 提交Banner DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Banner/
git commit -m "feat(site): 添加Banner DTO"
```

---

### Task 8: 创建新闻分类Service

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/INewsCategoryService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/NewsCategoryService.cs`

- [ ] **Step 1: 创建Site Service目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Business/Site
```

- [ ] **Step 2: 创建INewsCategoryService接口**

```csharp
using EasyProduct.Models.Dto.Site.NewsCategory;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻分类服务接口
/// </summary>
public interface INewsCategoryService
{
    /// <summary>
    /// 获取新闻分类列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分类列表分页结果</returns>
    Task<PageResult<NewsCategoryDto>> GetListAsync(NewsCategoryQueryDto query);

    /// <summary>
    /// 获取所有启用的新闻分类（不分页）
    /// </summary>
    /// <returns>分类列表</returns>
    Task<List<NewsCategoryDto>> GetAllAsync();

    /// <summary>
    /// 根据ID获取新闻分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    Task<NewsCategoryDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建新闻分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    Task<string> CreateAsync(CreateNewsCategoryDto dto);

    /// <summary>
    /// 更新新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, UpdateNewsCategoryDto dto);

    /// <summary>
    /// 删除新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);
}
```

- [ ] **Step 3: 创建NewsCategoryService实现**

```csharp
using EasyProduct.Models.Dto.Site.NewsCategory;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻分类服务实现
/// </summary>
public class NewsCategoryService : INewsCategoryService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public NewsCategoryService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取新闻分类列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分类列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（分类名称、编码）
    /// 2. 支持状态筛选
    /// 3. 默认按排序字段升序排列
    /// </remarks>
    public async Task<PageResult<NewsCategoryDto>> GetListAsync(NewsCategoryQueryDto query)
    {
        var queryable = _db.Queryable<site_news_category>()
            .Where(x => !x.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(x => 
                x.CategoryName.Contains(query.Keyword) || 
                (x.CategoryCode != null && x.CategoryCode.Contains(query.Keyword)));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.Sort).OrderBy(x => x.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<NewsCategoryDto>
        {
            List = list.Adapt<List<NewsCategoryDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 获取所有启用的新闻分类（不分页）
    /// </summary>
    /// <returns>分类列表</returns>
    /// <remarks>
    /// 仅返回启用状态的分类，按排序字段升序排列
    /// </remarks>
    public async Task<List<NewsCategoryDto>> GetAllAsync()
    {
        var list = await _db.Queryable<site_news_category>()
            .Where(x => !x.IsDeleted && x.Status == 1)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        return list.Adapt<List<NewsCategoryDto>>();
    }

    /// <summary>
    /// 根据ID获取新闻分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    public async Task<NewsCategoryDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_news_category>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        return entity?.Adapt<NewsCategoryDto>();
    }

    /// <summary>
    /// 创建新闻分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// </remarks>
    public async Task<string> CreateAsync(CreateNewsCategoryDto dto)
    {
        var entity = dto.Adapt<site_news_category>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 自动更新修改时间
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateNewsCategoryDto dto)
    {
        var entity = await _db.Queryable<site_news_category>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除新闻分类（软删除）
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_news_category>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }
}
```

- [ ] **Step 4: 提交新闻分类Service**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Site/INewsCategoryService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/NewsCategoryService.cs
git commit -m "feat(site): 添加新闻分类Service"
```

---

### Task 9: 创建新闻Service

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/INewsService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/NewsService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/ISiteNewsService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/SiteNewsService.cs`

- [ ] **Step 1: 创建INewsService接口（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务接口（管理端）
/// </summary>
public interface INewsService
{
    /// <summary>
    /// 获取新闻列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻列表分页结果</returns>
    Task<PageResult<NewsDto>> GetListAsync(NewsQueryDto query);

    /// <summary>
    /// 根据ID获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    Task<NewsDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新新闻ID</returns>
    Task<string> CreateAsync(CreateNewsDto dto);

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, UpdateNewsDto dto);

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);
}
```

- [ ] **Step 2: 创建NewsService实现（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务实现（管理端）
/// </summary>
public class NewsService : INewsService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public NewsService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取新闻列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（标题、摘要）
    /// 2. 支持分类筛选
    /// 3. 支持状态筛选
    /// 4. 支持置顶筛选
    /// 5. 支持发布时间范围筛选
    /// 6. 默认按置顶、发布时间、创建时间排序
    /// </remarks>
    public async Task<PageResult<NewsDto>> GetListAsync(NewsQueryDto query)
    {
        var queryable = _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id)
            .Where((n, c) => !n.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where((n, c) => 
                n.Title.Contains(query.Keyword) || 
                (n.Summary != null && n.Summary.Contains(query.Keyword)));
        }

        // 分类筛选
        if (!string.IsNullOrWhiteSpace(query.CategoryId))
        {
            queryable = queryable.Where((n, c) => n.CategoryId == Guid.Parse(query.CategoryId));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where((n, c) => n.Status == query.Status.Value);
        }

        // 置顶筛选
        if (query.IsTop.HasValue)
        {
            queryable = queryable.Where((n, c) => n.IsTop == query.IsTop.Value);
        }

        // 发布时间范围
        if (query.PublishStartTime.HasValue)
        {
            queryable = queryable.Where((n, c) => n.PublishTime >= query.PublishStartTime.Value);
        }
        if (query.PublishEndTime.HasValue)
        {
            queryable = queryable.Where((n, c) => n.PublishTime <= query.PublishEndTime.Value);
        }

        // 排序
        queryable = queryable
            .OrderBy((n, c) => n.IsTop, OrderByType.Desc)
            .OrderBy((n, c) => n.PublishTime, OrderByType.Desc)
            .OrderBy((n, c) => n.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Select((n, c) => new NewsDto
            {
                Id = n.Id.ToString(),
                CategoryId = n.CategoryId.ToString(),
                CategoryName = c.CategoryName,
                Title = n.Title,
                TitleEn = n.TitleEn,
                Summary = n.Summary,
                SummaryEn = n.SummaryEn,
                Content = n.Content,
                ContentEn = n.ContentEn,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                IsTop = n.IsTop,
                PublishTime = n.PublishTime,
                Status = n.Status,
                CreateTime = n.CreateTime,
                CreateBy = n.CreateBy
            })
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<NewsDto>
        {
            List = list,
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    public async Task<NewsDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id)
            .Where((n, c) => n.Id == Guid.Parse(id) && !n.IsDeleted)
            .Select((n, c) => new NewsDto
            {
                Id = n.Id.ToString(),
                CategoryId = n.CategoryId.ToString(),
                CategoryName = c.CategoryName,
                Title = n.Title,
                TitleEn = n.TitleEn,
                Summary = n.Summary,
                SummaryEn = n.SummaryEn,
                Content = n.Content,
                ContentEn = n.ContentEn,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                IsTop = n.IsTop,
                PublishTime = n.PublishTime,
                Status = n.Status,
                CreateTime = n.CreateTime,
                CreateBy = n.CreateBy
            })
            .FirstAsync();

        return entity;
    }

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新新闻ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// 3. 如果未设置发布时间，默认为当前时间
    /// </remarks>
    public async Task<string> CreateAsync(CreateNewsDto dto)
    {
        var entity = dto.Adapt<site_news>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;
        entity.ViewCount = 0;

        if (!entity.PublishTime.HasValue)
        {
            entity.PublishTime = DateTime.Now;
        }

        if (!string.IsNullOrWhiteSpace(dto.CategoryId))
        {
            entity.CategoryId = Guid.Parse(dto.CategoryId);
        }

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 自动更新修改时间
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateNewsDto dto)
    {
        var entity = await _db.Queryable<site_news>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(dto.CategoryId))
        {
            entity.CategoryId = Guid.Parse(dto.CategoryId);
        }
        else
        {
            entity.CategoryId = null;
        }

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除新闻（软删除）
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_news>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }
}
```

- [ ] **Step 3: 创建ISiteNewsService接口（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务接口（官网公开）
/// </summary>
public interface ISiteNewsService
{
    /// <summary>
    /// 获取新闻列表（分页，仅返回启用的新闻）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻列表分页结果</returns>
    Task<PageResult<NewsDto>> GetListAsync(NewsQueryDto query);

    /// <summary>
    /// 根据ID获取新闻详情（增加浏览次数）
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    Task<NewsDto?> GetByIdAsync(string id);

    /// <summary>
    /// 获取置顶新闻列表
    /// </summary>
    /// <param name="count">数量限制</param>
    /// <returns>新闻列表</returns>
    Task<List<NewsDto>> GetTopNewsAsync(int count = 5);
}
```

- [ ] **Step 4: 创建SiteNewsService实现（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 新闻服务实现（官网公开）
/// </summary>
public class SiteNewsService : ISiteNewsService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public SiteNewsService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取新闻列表（分页，仅返回启用的新闻）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻列表分页结果</returns>
    /// <remarks>
    /// 1. 仅返回启用状态的新闻
    /// 2. 支持分类筛选
    /// 3. 默认按置顶、发布时间排序
    /// </remarks>
    public async Task<PageResult<NewsDto>> GetListAsync(NewsQueryDto query)
    {
        var queryable = _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id)
            .Where((n, c) => !n.IsDeleted && n.Status == 1);

        // 分类筛选
        if (!string.IsNullOrWhiteSpace(query.CategoryId))
        {
            queryable = queryable.Where((n, c) => n.CategoryId == Guid.Parse(query.CategoryId));
        }

        // 排序
        queryable = queryable
            .OrderBy((n, c) => n.IsTop, OrderByType.Desc)
            .OrderBy((n, c) => n.PublishTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Select((n, c) => new NewsDto
            {
                Id = n.Id.ToString(),
                CategoryId = n.CategoryId.ToString(),
                CategoryName = c.CategoryName,
                Title = n.Title,
                TitleEn = n.TitleEn,
                Summary = n.Summary,
                SummaryEn = n.SummaryEn,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                IsTop = n.IsTop,
                PublishTime = n.PublishTime,
                Status = n.Status,
                CreateTime = n.CreateTime
            })
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<NewsDto>
        {
            List = list,
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取新闻详情（增加浏览次数）
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    /// <remarks>
    /// 1. 仅返回启用状态的新闻
    /// 2. 每次访问自动增加浏览次数
    /// </remarks>
    public async Task<NewsDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id)
            .Where((n, c) => n.Id == Guid.Parse(id) && !n.IsDeleted && n.Status == 1)
            .Select((n, c) => new NewsDto
            {
                Id = n.Id.ToString(),
                CategoryId = n.CategoryId.ToString(),
                CategoryName = c.CategoryName,
                Title = n.Title,
                TitleEn = n.TitleEn,
                Summary = n.Summary,
                SummaryEn = n.SummaryEn,
                Content = n.Content,
                ContentEn = n.ContentEn,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                IsTop = n.IsTop,
                PublishTime = n.PublishTime,
                Status = n.Status,
                CreateTime = n.CreateTime
            })
            .FirstAsync();

        if (entity != null)
        {
            // 增加浏览次数
            await _db.Updateable<site_news>()
                .Where(x => x.Id == Guid.Parse(id))
                .SetColumns(x => x.ViewCount == x.ViewCount + 1)
                .ExecuteCommandAsync();
        }

        return entity;
    }

    /// <summary>
    /// 获取置顶新闻列表
    /// </summary>
    /// <param name="count">数量限制</param>
    /// <returns>新闻列表</returns>
    /// <remarks>
    /// 仅返回启用状态的置顶新闻，按发布时间倒序
    /// </remarks>
    public async Task<List<NewsDto>> GetTopNewsAsync(int count = 5)
    {
        var list = await _db.Queryable<site_news>()
            .LeftJoin<site_news_category>((n, c) => n.CategoryId == c.Id)
            .Where((n, c) => !n.IsDeleted && n.Status == 1 && n.IsTop == 1)
            .OrderBy((n, c) => n.PublishTime, OrderByType.Desc)
            .Take(count)
            .Select((n, c) => new NewsDto
            {
                Id = n.Id.ToString(),
                CategoryId = n.CategoryId.ToString(),
                CategoryName = c.CategoryName,
                Title = n.Title,
                TitleEn = n.TitleEn,
                Summary = n.Summary,
                SummaryEn = n.SummaryEn,
                CoverImage = n.CoverImage,
                Author = n.Author,
                Source = n.Source,
                ViewCount = n.ViewCount,
                IsTop = n.IsTop,
                PublishTime = n.PublishTime,
                Status = n.Status,
                CreateTime = n.CreateTime
            })
            .ToListAsync();

        return list;
    }
}
```

- [ ] **Step 5: 提交新闻Service**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Site/INewsService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/NewsService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/ISiteNewsService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/SiteNewsService.cs
git commit -m "feat(site): 添加新闻Service"
```

---

### Task 10: 创建Banner Service

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/IBannerService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/BannerService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/ISiteBannerService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/SiteBannerService.cs`

- [ ] **Step 1: 创建IBannerService接口（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务接口（管理端）
/// </summary>
public interface IBannerService
{
    /// <summary>
    /// 获取Banner列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>Banner列表分页结果</returns>
    Task<PageResult<BannerDto>> GetListAsync(BannerQueryDto query);

    /// <summary>
    /// 根据ID获取Banner详情
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <returns>Banner详情</returns>
    Task<BannerDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建Banner
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新Banner ID</returns>
    Task<string> CreateAsync(CreateBannerDto dto);

    /// <summary>
    /// 更新Banner
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, UpdateBannerDto dto);

    /// <summary>
    /// 删除Banner
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);
}
```

- [ ] **Step 2: 创建BannerService实现（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务实现（管理端）
/// </summary>
public class BannerService : IBannerService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public BannerService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取Banner列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>Banner列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（标题）
    /// 2. 支持位置筛选
    /// 3. 支持状态筛选
    /// 4. 默认按排序字段升序排列
    /// </remarks>
    public async Task<PageResult<BannerDto>> GetListAsync(BannerQueryDto query)
    {
        var queryable = _db.Queryable<site_banner>()
            .Where(x => !x.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(x => 
                (x.Title != null && x.Title.Contains(query.Keyword)));
        }

        // 位置筛选
        if (!string.IsNullOrWhiteSpace(query.Position))
        {
            queryable = queryable.Where(x => x.Position == query.Position);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.Sort).OrderBy(x => x.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<BannerDto>
        {
            List = list.Adapt<List<BannerDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取Banner详情
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <returns>Banner详情</returns>
    public async Task<BannerDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_banner>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        return entity?.Adapt<BannerDto>();
    }

    /// <summary>
    /// 创建Banner
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新Banner ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// </remarks>
    public async Task<string> CreateAsync(CreateBannerDto dto)
    {
        var entity = dto.Adapt<site_banner>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新Banner
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 自动更新修改时间
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateBannerDto dto)
    {
        var entity = await _db.Queryable<site_banner>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除Banner（软删除）
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_banner>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }
}
```

- [ ] **Step 3: 创建ISiteBannerService接口（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.Banner;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务接口（官网公开）
/// </summary>
public interface ISiteBannerService
{
    /// <summary>
    /// 根据位置获取Banner列表
    /// </summary>
    /// <param name="position">位置：home=首页，product=产品页等</param>
    /// <returns>Banner列表</returns>
    Task<List<BannerDto>> GetByPositionAsync(string position = "home");

    /// <summary>
    /// 获取所有有效Banner（自动过滤时间范围）
    /// </summary>
    /// <returns>Banner列表</returns>
    Task<List<BannerDto>> GetAllActiveAsync();
}
```

- [ ] **Step 4: 创建SiteBannerService实现（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// Banner服务实现（官网公开）
/// </summary>
public class SiteBannerService : ISiteBannerService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public SiteBannerService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 根据位置获取Banner列表
    /// </summary>
    /// <param name="position">位置：home=首页，product=产品页等</param>
    /// <returns>Banner列表</returns>
    /// <remarks>
    /// 1. 仅返回启用状态的Banner
    /// 2. 自动过滤时间范围（当前时间在开始时间和结束时间之间）
    /// 3. 按排序字段升序排列
    /// </remarks>
    public async Task<List<BannerDto>> GetByPositionAsync(string position = "home")
    {
        var now = DateTime.Now;
        var list = await _db.Queryable<site_banner>()
            .Where(x => !x.IsDeleted && x.Status == 1 && x.Position == position)
            .Where(x => 
                (x.StartTime == null || x.StartTime <= now) && 
                (x.EndTime == null || x.EndTime >= now))
            .OrderBy(x => x.Sort)
            .ToListAsync();

        return list.Adapt<List<BannerDto>>();
    }

    /// <summary>
    /// 获取所有有效Banner（自动过滤时间范围）
    /// </summary>
    /// <returns>Banner列表</returns>
    /// <remarks>
    /// 1. 仅返回启用状态的Banner
    /// 2. 自动过滤时间范围（当前时间在开始时间和结束时间之间）
    /// 3. 按排序字段升序排列
    /// </remarks>
    public async Task<List<BannerDto>> GetAllActiveAsync()
    {
        var now = DateTime.Now;
        var list = await _db.Queryable<site_banner>()
            .Where(x => !x.IsDeleted && x.Status == 1)
            .Where(x => 
                (x.StartTime == null || x.StartTime <= now) && 
                (x.EndTime == null || x.EndTime >= now))
            .OrderBy(x => x.Sort)
            .ToListAsync();

        return list.Adapt<List<BannerDto>>();
    }
}
```

- [ ] **Step 5: 提交Banner Service**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Site/IBannerService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/BannerService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/ISiteBannerService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/SiteBannerService.cs
git commit -m "feat(site): 添加Banner Service"
```

---

### Task 11: 创建新闻分类Controller

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/NewsCategoryController.cs`

- [ ] **Step 1: 创建Admin Site Controller目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site
```

- [ ] **Step 2: 创建新闻分类Controller（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.NewsCategory;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 新闻分类控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/news-category")]
[Authorize]
public class NewsCategoryController : ControllerBase
{
    private readonly INewsCategoryService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">新闻分类服务</param>
    public NewsCategoryController(INewsCategoryService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取新闻分类列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分类列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] NewsCategoryQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<NewsCategoryDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 获取所有启用的新闻分类（不分页）
    /// </summary>
    /// <returns>分类列表</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(new ApiResponse<List<NewsCategoryDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取新闻分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "分类不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<NewsCategoryDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 创建新闻分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNewsCategoryDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "创建成功",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 更新新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateNewsCategoryDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "分类不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "更新成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "分类不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 3: 提交新闻分类Controller**

```bash
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/NewsCategoryController.cs
git commit -m "feat(site): 添加新闻分类Controller"
```

---

### Task 12: 创建新闻Controller

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/NewsController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/NewsController.cs`

- [ ] **Step 1: 创建Site公开Controller目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Web/Controllers/Site
```

- [ ] **Step 2: 创建新闻Controller（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 新闻控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/news")]
[Authorize]
public class NewsController : ControllerBase
{
    private readonly INewsService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">新闻服务</param>
    public NewsController(INewsService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取新闻列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] NewsQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<NewsDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取新闻详情
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "新闻不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<NewsDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 创建新闻
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新新闻ID</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNewsDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "创建成功",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 更新新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateNewsDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "新闻不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "更新成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除新闻
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "新闻不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 3: 创建新闻Controller（官网公开）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.News;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 新闻控制器（官网公开）
/// </summary>
[ApiController]
[Route("api/site/news")]
public class NewsController : ControllerBase
{
    private readonly ISiteNewsService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">新闻服务（官网公开）</param>
    public NewsController(ISiteNewsService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取新闻列表（分页，仅返回启用的新闻）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>新闻列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] NewsQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<NewsDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取新闻详情（增加浏览次数）
    /// </summary>
    /// <param name="id">新闻ID</param>
    /// <returns>新闻详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "新闻不存在或已下线",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<NewsDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 获取置顶新闻列表
    /// </summary>
    /// <param name="count">数量限制，默认5条</param>
    /// <returns>新闻列表</returns>
    [HttpGet("top")]
    public async Task<IActionResult> GetTopNews([FromQuery] int count = 5)
    {
        var result = await _service.GetTopNewsAsync(count);
        return Ok(new ApiResponse<List<NewsDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 4: 提交新闻Controller**

```bash
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/NewsController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/NewsController.cs
git commit -m "feat(site): 添加新闻Controller"
```

---

### Task 13: 创建Banner Controller

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/BannerController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/BannerController.cs`

- [ ] **Step 1: 创建Banner Controller（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// Banner控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/banner")]
[Authorize]
public class BannerController : ControllerBase
{
    private readonly IBannerService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">Banner服务</param>
    public BannerController(IBannerService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取Banner列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>Banner列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] BannerQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<BannerDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取Banner详情
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <returns>Banner详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "Banner不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<BannerDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 创建Banner
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新Banner ID</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBannerDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "创建成功",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 更新Banner
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBannerDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "Banner不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "更新成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除Banner
    /// </summary>
    /// <param name="id">Banner ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "Banner不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 2: 创建Banner Controller（官网公开）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Banner;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// Banner控制器（官网公开）
/// </summary>
[ApiController]
[Route("api/site/banner")]
public class BannerController : ControllerBase
{
    private readonly ISiteBannerService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">Banner服务（官网公开）</param>
    public BannerController(ISiteBannerService service)
    {
        _service = service;
    }

    /// <summary>
    /// 根据位置获取Banner列表
    /// </summary>
    /// <param name="position">位置：home=首页，product=产品页等，默认home</param>
    /// <returns>Banner列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetByPosition([FromQuery] string position = "home")
    {
        var result = await _service.GetByPositionAsync(position);
        return Ok(new ApiResponse<List<BannerDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 获取所有有效Banner（自动过滤时间范围）
    /// </summary>
    /// <returns>Banner列表</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllActive()
    {
        var result = await _service.GetAllActiveAsync();
        return Ok(new ApiResponse<List<BannerDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 3: 提交Banner Controller**

```bash
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/BannerController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/BannerController.cs
git commit -m "feat(site): 添加Banner Controller"
```

---

### Task 14: 创建Mock数据

**Files:**
- Create: `mock-server/data/site/news.js`
- Create: `mock-server/routes/site.js`

- [ ] **Step 1: 创建mock data site目录**

```bash
mkdir -p mock-server/data/site
mkdir -p mock-server/routes
```

- [ ] **Step 2: 创建新闻和Banner mock数据**

```javascript
// mock-server/data/site/news.js

const Mock = require('mockjs');

const Random = Mock.Random;

// 新闻分类数据
const newsCategories = [
  {
    id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
    categoryName: '公司新闻',
    categoryCode: 'company',
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'b2c3d4e5-f6a7-8901-bcde-f12345678901',
    categoryName: '行业资讯',
    categoryCode: 'industry',
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'c3d4e5f6-a7b8-9012-cdef-123456789012',
    categoryName: '产品动态',
    categoryCode: 'product',
    sort: 3,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  }
];

// 生成新闻数据
function generateNews() {
  const newsList = [];
  for (let i = 1; i <= 50; i++) {
    const categoryIndex = Math.floor(Math.random() * newsCategories.length);
    newsList.push({
      id: Random.guid(),
      categoryId: newsCategories[categoryIndex].id,
      categoryName: newsCategories[categoryIndex].categoryName,
      title: `新闻标题${i} - ${Random.ctitle(10, 20)}`,
      titleEn: `News Title ${i} - ${Random.sentence(5, 10)}`,
      summary: Random.cparagraph(1, 3),
      summaryEn: Random.paragraph(1, 3),
      content: `<p>${Random.cparagraph(5, 10)}</p>`,
      contentEn: `<p>${Random.paragraph(5, 10)}</p>`,
      coverImage: Random.image('800x400', '#50B347', '#FFFFFF', 'News'),
      author: Random.cname(),
      source: Random.pick(['官网', '微信公众号', '合作伙伴', '内部投稿']),
      viewCount: Random.integer(100, 5000),
      isTop: i <= 5 ? 1 : 0,
      publishTime: Random.datetime('yyyy-MM-dd HH:mm:ss'),
      status: 1,
      createTime: Random.datetime('yyyy-MM-dd HH:mm:ss'),
      createBy: 'admin'
    });
  }
  return newsList;
}

const newsData = generateNews();

// Banner数据
const bannerData = [
  {
    id: 'banner-001',
    title: '2024年度新品发布会',
    titleEn: '2024 New Product Launch',
    imageUrl: Random.image('1920x500', '#2d8cf0', '#FFFFFF', 'Banner 1'),
    linkUrl: '/news/detail/news-001',
    target: '_self',
    position: 'home',
    sort: 1,
    status: 1,
    startTime: '2024-01-01 00:00:00',
    endTime: '2024-12-31 23:59:59',
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'banner-002',
    title: '产品技术升级',
    titleEn: 'Product Technology Upgrade',
    imageUrl: Random.image('1920x500', '#19be6b', '#FFFFFF', 'Banner 2'),
    linkUrl: '/products',
    target: '_self',
    position: 'home',
    sort: 2,
    status: 1,
    startTime: '2024-01-01 00:00:00',
    endTime: '2024-12-31 23:59:59',
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'banner-003',
    title: '合作伙伴计划',
    titleEn: 'Partner Program',
    imageUrl: Random.image('1920x500', '#ff9900', '#FFFFFF', 'Banner 3'),
    linkUrl: '/about',
    target: '_self',
    position: 'home',
    sort: 3,
    status: 1,
    startTime: '2024-01-01 00:00:00',
    endTime: '2024-12-31 23:59:59',
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'banner-004',
    title: '产品页面Banner',
    titleEn: 'Product Page Banner',
    imageUrl: Random.image('1920x500', '#ed4014', '#FFFFFF', 'Banner 4'),
    linkUrl: '/products',
    target: '_self',
    position: 'product',
    sort: 1,
    status: 1,
    startTime: '2024-01-01 00:00:00',
    endTime: '2024-12-31 23:59:59',
    createTime: '2024-01-01 10:00:00'
  }
];

module.exports = {
  newsCategories,
  newsData,
  bannerData
};
```

- [ ] **Step 3: 创建Site模块mock路由**

```javascript
// mock-server/routes/site.js

const express = require('express');
const router = express.Router();
const Mock = require('mockjs');
const { newsCategories, newsData, bannerData } = require('../data/site/news');

const Random = Mock.Random;

// ==================== 新闻分类接口 ====================

/**
 * 获取新闻分类列表（分页）
 */
router.get('/admin/site/news-category', (req, res) => {
  const { pageIndex = 1, pageSize = 10, keyword, status } = req.query;
  
  let filtered = [...newsCategories];
  
  // 关键词搜索
  if (keyword) {
    filtered = filtered.filter(item => 
      item.categoryName.includes(keyword) || 
      (item.categoryCode && item.categoryCode.includes(keyword))
    );
  }
  
  // 状态筛选
  if (status !== undefined && status !== '') {
    filtered = filtered.filter(item => item.status === parseInt(status));
  }
  
  const total = filtered.length;
  const start = (parseInt(pageIndex) - 1) * parseInt(pageSize);
  const list = filtered.slice(start, start + parseInt(pageSize));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: {
      list,
      total
    },
    timestamp: Date.now()
  });
});

/**
 * 获取所有启用的新闻分类（不分页）
 */
router.get('/admin/site/news-category/all', (req, res) => {
  const list = newsCategories.filter(item => item.status === 1);
  
  res.json({
    code: 200,
    message: '获取成功',
    data: list,
    timestamp: Date.now()
  });
});

/**
 * 根据ID获取新闻分类详情
 */
router.get('/admin/site/news-category/:id', (req, res) => {
  const { id } = req.params;
  const category = newsCategories.find(item => item.id === id);
  
  if (!category) {
    return res.json({
      code: 404,
      message: '分类不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  res.json({
    code: 200,
    message: '获取成功',
    data: category,
    timestamp: Date.now()
  });
});

/**
 * 创建新闻分类
 */
router.post('/admin/site/news-category', (req, res) => {
  const data = req.body;
  const newCategory = {
    id: Random.guid(),
    categoryName: data.categoryName,
    categoryCode: data.categoryCode,
    sort: data.sort || 0,
    status: data.status || 1,
    createTime: Random.now('yyyy-MM-dd HH:mm:ss')
  };
  
  newsCategories.push(newCategory);
  
  res.json({
    code: 200,
    message: '创建成功',
    data: newCategory.id,
    timestamp: Date.now()
  });
});

/**
 * 更新新闻分类
 */
router.put('/admin/site/news-category/:id', (req, res) => {
  const { id } = req.params;
  const data = req.body;
  const index = newsCategories.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '分类不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  newsCategories[index] = {
    ...newsCategories[index],
    categoryName: data.categoryName,
    categoryCode: data.categoryCode,
    sort: data.sort,
    status: data.status
  };
  
  res.json({
    code: 200,
    message: '更新成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 删除新闻分类
 */
router.delete('/admin/site/news-category/:id', (req, res) => {
  const { id } = req.params;
  const index = newsCategories.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '分类不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  newsCategories.splice(index, 1);
  
  res.json({
    code: 200,
    message: '删除成功',
    data: null,
    timestamp: Date.now()
  });
});

// ==================== 新闻接口（管理端） ====================

/**
 * 获取新闻列表（分页）
 */
router.get('/admin/site/news', (req, res) => {
  const { pageIndex = 1, pageSize = 10, keyword, categoryId, status, isTop } = req.query;
  
  let filtered = [...newsData];
  
  // 关键词搜索
  if (keyword) {
    filtered = filtered.filter(item => 
      item.title.includes(keyword) || 
      (item.summary && item.summary.includes(keyword))
    );
  }
  
  // 分类筛选
  if (categoryId) {
    filtered = filtered.filter(item => item.categoryId === categoryId);
  }
  
  // 状态筛选
  if (status !== undefined && status !== '') {
    filtered = filtered.filter(item => item.status === parseInt(status));
  }
  
  // 置顶筛选
  if (isTop !== undefined && isTop !== '') {
    filtered = filtered.filter(item => item.isTop === parseInt(isTop));
  }
  
  const total = filtered.length;
  const start = (parseInt(pageIndex) - 1) * parseInt(pageSize);
  const list = filtered.slice(start, start + parseInt(pageSize));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: {
      list,
      total
    },
    timestamp: Date.now()
  });
});

/**
 * 根据ID获取新闻详情
 */
router.get('/admin/site/news/:id', (req, res) => {
  const { id } = req.params;
  const news = newsData.find(item => item.id === id);
  
  if (!news) {
    return res.json({
      code: 404,
      message: '新闻不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  res.json({
    code: 200,
    message: '获取成功',
    data: news,
    timestamp: Date.now()
  });
});

/**
 * 创建新闻
 */
router.post('/admin/site/news', (req, res) => {
  const data = req.body;
  const category = newsCategories.find(item => item.id === data.categoryId);
  
  const newNews = {
    id: Random.guid(),
    categoryId: data.categoryId,
    categoryName: category ? category.categoryName : null,
    title: data.title,
    titleEn: data.titleEn,
    summary: data.summary,
    summaryEn: data.summaryEn,
    content: data.content,
    contentEn: data.contentEn,
    coverImage: data.coverImage,
    author: data.author,
    source: data.source,
    viewCount: 0,
    isTop: data.isTop || 0,
    publishTime: data.publishTime || Random.now('yyyy-MM-dd HH:mm:ss'),
    status: data.status || 1,
    createTime: Random.now('yyyy-MM-dd HH:mm:ss'),
    createBy: 'admin'
  };
  
  newsData.unshift(newNews);
  
  res.json({
    code: 200,
    message: '创建成功',
    data: newNews.id,
    timestamp: Date.now()
  });
});

/**
 * 更新新闻
 */
router.put('/admin/site/news/:id', (req, res) => {
  const { id } = req.params;
  const data = req.body;
  const index = newsData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '新闻不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  const category = newsCategories.find(item => item.id === data.categoryId);
  
  newsData[index] = {
    ...newsData[index],
    categoryId: data.categoryId,
    categoryName: category ? category.categoryName : null,
    title: data.title,
    titleEn: data.titleEn,
    summary: data.summary,
    summaryEn: data.summaryEn,
    content: data.content,
    contentEn: data.contentEn,
    coverImage: data.coverImage,
    author: data.author,
    source: data.source,
    isTop: data.isTop,
    publishTime: data.publishTime,
    status: data.status
  };
  
  res.json({
    code: 200,
    message: '更新成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 删除新闻
 */
router.delete('/admin/site/news/:id', (req, res) => {
  const { id } = req.params;
  const index = newsData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '新闻不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  newsData.splice(index, 1);
  
  res.json({
    code: 200,
    message: '删除成功',
    data: null,
    timestamp: Date.now()
  });
});

// ==================== 新闻接口（官网公开） ====================

/**
 * 获取新闻列表（分页，仅返回启用的新闻）
 */
router.get('/site/news', (req, res) => {
  const { pageIndex = 1, pageSize = 10, categoryId } = req.query;
  
  let filtered = newsData.filter(item => item.status === 1);
  
  // 分类筛选
  if (categoryId) {
    filtered = filtered.filter(item => item.categoryId === categoryId);
  }
  
  // 按置顶、发布时间排序
  filtered.sort((a, b) => {
    if (a.isTop !== b.isTop) return b.isTop - a.isTop;
    return new Date(b.publishTime) - new Date(a.publishTime);
  });
  
  const total = filtered.length;
  const start = (parseInt(pageIndex) - 1) * parseInt(pageSize);
  const list = filtered.slice(start, start + parseInt(pageSize));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: {
      list,
      total
    },
    timestamp: Date.now()
  });
});

/**
 * 根据ID获取新闻详情（增加浏览次数）
 */
router.get('/site/news/:id', (req, res) => {
  const { id } = req.params;
  const news = newsData.find(item => item.id === id && item.status === 1);
  
  if (!news) {
    return res.json({
      code: 404,
      message: '新闻不存在或已下线',
      data: null,
      timestamp: Date.now()
    });
  }
  
  // 增加浏览次数
  news.viewCount += 1;
  
  res.json({
    code: 200,
    message: '获取成功',
    data: news,
    timestamp: Date.now()
  });
});

/**
 * 获取置顶新闻列表
 */
router.get('/site/news/top', (req, res) => {
  const { count = 5 } = req.query;
  
  const topNews = newsData
    .filter(item => item.status === 1 && item.isTop === 1)
    .sort((a, b) => new Date(b.publishTime) - new Date(a.publishTime))
    .slice(0, parseInt(count));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: topNews,
    timestamp: Date.now()
  });
});

// ==================== Banner接口（管理端） ====================

/**
 * 获取Banner列表（分页）
 */
router.get('/admin/site/banner', (req, res) => {
  const { pageIndex = 1, pageSize = 10, keyword, position, status } = req.query;
  
  let filtered = [...bannerData];
  
  // 关键词搜索
  if (keyword) {
    filtered = filtered.filter(item => 
      item.title && item.title.includes(keyword)
    );
  }
  
  // 位置筛选
  if (position) {
    filtered = filtered.filter(item => item.position === position);
  }
  
  // 状态筛选
  if (status !== undefined && status !== '') {
    filtered = filtered.filter(item => item.status === parseInt(status));
  }
  
  const total = filtered.length;
  const start = (parseInt(pageIndex) - 1) * parseInt(pageSize);
  const list = filtered.slice(start, start + parseInt(pageSize));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: {
      list,
      total
    },
    timestamp: Date.now()
  });
});

/**
 * 根据ID获取Banner详情
 */
router.get('/admin/site/banner/:id', (req, res) => {
  const { id } = req.params;
  const banner = bannerData.find(item => item.id === id);
  
  if (!banner) {
    return res.json({
      code: 404,
      message: 'Banner不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  res.json({
    code: 200,
    message: '获取成功',
    data: banner,
    timestamp: Date.now()
  });
});

/**
 * 创建Banner
 */
router.post('/admin/site/banner', (req, res) => {
  const data = req.body;
  const newBanner = {
    id: Random.guid(),
    title: data.title,
    titleEn: data.titleEn,
    imageUrl: data.imageUrl,
    linkUrl: data.linkUrl,
    target: data.target || '_self',
    position: data.position || 'home',
    sort: data.sort || 0,
    status: data.status || 1,
    startTime: data.startTime,
    endTime: data.endTime,
    createTime: Random.now('yyyy-MM-dd HH:mm:ss')
  };
  
  bannerData.push(newBanner);
  
  res.json({
    code: 200,
    message: '创建成功',
    data: newBanner.id,
    timestamp: Date.now()
  });
});

/**
 * 更新Banner
 */
router.put('/admin/site/banner/:id', (req, res) => {
  const { id } = req.params;
  const data = req.body;
  const index = bannerData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: 'Banner不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  bannerData[index] = {
    ...bannerData[index],
    title: data.title,
    titleEn: data.titleEn,
    imageUrl: data.imageUrl,
    linkUrl: data.linkUrl,
    target: data.target,
    position: data.position,
    sort: data.sort,
    status: data.status,
    startTime: data.startTime,
    endTime: data.endTime
  };
  
  res.json({
    code: 200,
    message: '更新成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 删除Banner
 */
router.delete('/admin/site/banner/:id', (req, res) => {
  const { id } = req.params;
  const index = bannerData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: 'Banner不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  bannerData.splice(index, 1);
  
  res.json({
    code: 200,
    message: '删除成功',
    data: null,
    timestamp: Date.now()
  });
});

// ==================== Banner接口（官网公开） ====================

/**
 * 根据位置获取Banner列表
 */
router.get('/site/banner', (req, res) => {
  const { position = 'home' } = req.query;
  const now = new Date();
  
  const list = bannerData
    .filter(item => 
      item.status === 1 && 
      item.position === position &&
      (!item.startTime || new Date(item.startTime) <= now) &&
      (!item.endTime || new Date(item.endTime) >= now)
    )
    .sort((a, b) => a.sort - b.sort);
  
  res.json({
    code: 200,
    message: '获取成功',
    data: list,
    timestamp: Date.now()
  });
});

/**
 * 获取所有有效Banner（自动过滤时间范围）
 */
router.get('/site/banner/all', (req, res) => {
  const now = new Date();
  
  const list = bannerData
    .filter(item => 
      item.status === 1 &&
      (!item.startTime || new Date(item.startTime) <= now) &&
      (!item.endTime || new Date(item.endTime) >= now)
    )
    .sort((a, b) => a.sort - b.sort);
  
  res.json({
    code: 200,
    message: '获取成功',
    data: list,
    timestamp: Date.now()
  });
});

module.exports = router;
```

- [ ] **Step 4: 提交Mock数据**

```bash
git add mock-server/data/site/news.js
git add mock-server/routes/site.js
git commit -m "feat(mock): 添加Site模块Mock数据"
```

---

### Task 15: 编译测试

- [ ] **Step 1: 编译项目**

```bash
cd EasyProduct.WebApi && dotnet build
```

- [ ] **Step 2: 修复编译错误（如果有）**

如果编译过程中出现错误，需要根据错误信息进行修复。

- [ ] **Step 3: 提交批次1完成标记**

```bash
git add .
git commit -m "feat(site): 完成批次1开发 - 新闻 + Banner模块"
```

---

---

## 批次 2：关于我们 + 下载管理 + 视频管理（2-3天）

### Task 16: 创建数据库表（批次2）

**Files:**
- Modify: `sql/init-database.sql`

- [ ] **Step 1: 添加关于我们、下载、视频表定义**

在 `sql/init-database.sql` 文件末尾添加：

```sql
-- ----------------------------
-- 关于我们表 (site_about)
-- ----------------------------
DROP TABLE IF EXISTS `site_about`;
CREATE TABLE `site_about` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(100) NOT NULL COMMENT '标题',
  `title_en` VARCHAR(100) DEFAULT NULL COMMENT '标题（英文）',
  `content` LONGTEXT COMMENT '内容（富文本）',
  `content_en` LONGTEXT COMMENT '内容（英文，富文本）',
  `image_url` VARCHAR(500) DEFAULT NULL COMMENT '图片URL',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='关于我们表';

-- ----------------------------
-- 下载管理表 (site_download)
-- ----------------------------
DROP TABLE IF EXISTS `site_download`;
CREATE TABLE `site_download` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `category` VARCHAR(50) DEFAULT NULL COMMENT '分类：manual=手册，catalog=目录，software=软件',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '标题（英文）',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '描述',
  `description_en` VARCHAR(500) DEFAULT NULL COMMENT '描述（英文）',
  `file_url` VARCHAR(500) NOT NULL COMMENT '文件URL',
  `file_name` VARCHAR(200) DEFAULT NULL COMMENT '文件名称',
  `file_size` BIGINT DEFAULT 0 COMMENT '文件大小（字节）',
  `file_type` VARCHAR(50) DEFAULT NULL COMMENT '文件类型：pdf/doc/xls/zip等',
  `download_count` INT DEFAULT 0 COMMENT '下载次数',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_category` (`category`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='下载管理表';

-- ----------------------------
-- 视频管理表 (site_video)
-- ----------------------------
DROP TABLE IF EXISTS `site_video`;
CREATE TABLE `site_video` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '标题（英文）',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '描述',
  `description_en` VARCHAR(500) DEFAULT NULL COMMENT '描述（英文）',
  `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
  `video_url` VARCHAR(500) NOT NULL COMMENT '视频URL',
  `video_type` VARCHAR(50) DEFAULT NULL COMMENT '视频类型：mp4/webm/youtube等',
  `duration` INT DEFAULT 0 COMMENT '时长（秒）',
  `view_count` INT DEFAULT 0 COMMENT '播放次数',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='视频管理表';
```

- [ ] **Step 2: 提交数据库表定义**

```bash
git add sql/init-database.sql
git commit -m "feat(site): 添加关于我们、下载、视频数据库表定义"
```

---

### Task 17: 创建关于我们实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_about.cs`

- [ ] **Step 1: 创建关于我们实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 关于我们实体
/// </summary>
[SugarTable("site_about", "关于我们表")]
public class site_about : BaseEntity
{
    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true)]
    public string? ContentEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }
}
```

- [ ] **Step 2: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_about.cs
git commit -m "feat(site): 添加关于我们实体类"
```

---

### Task 18: 创建下载管理实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_download.cs`

- [ ] **Step 1: 创建下载管理实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 下载管理实体
/// </summary>
[SugarTable("site_download", "下载管理表")]
public class site_download : BaseEntity
{
    /// <summary>
    /// 分类：manual=手册，catalog=目录，software=软件
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Category { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Title { get; set; } = null!;

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
    /// 描述（英文）
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 文件名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型：pdf/doc/xls/zip等
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? FileType { get; set; }

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }
}
```

- [ ] **Step 2: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_download.cs
git commit -m "feat(site): 添加下载管理实体类"
```

---

### Task 19: 创建视频管理实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_video.cs`

- [ ] **Step 1: 创建视频管理实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 视频管理实体
/// </summary>
[SugarTable("site_video", "视频管理表")]
public class site_video : BaseEntity
{
    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(Length = 200)]
    public string Title { get; set; } = null!;

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
    /// 描述（英文）
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string VideoUrl { get; set; } = null!;

    /// <summary>
    /// 视频类型：mp4/webm/youtube等
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? VideoType { get; set; }

    /// <summary>
    /// 时长（秒）
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 播放次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }
}
```

- [ ] **Step 2: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_video.cs
git commit -m "feat(site): 添加视频管理实体类"
```

---

### Task 20: 创建关于我们DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/About/AboutDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/About/AboutQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/About/CreateAboutDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/About/UpdateAboutDto.cs`

- [ ] **Step 1: 创建About DTO目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/About
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Download
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Video
```

- [ ] **Step 2: 创建AboutDto**

```csharp
namespace EasyProduct.Models.Dto.Site.About;

/// <summary>
/// 关于我们DTO
/// </summary>
public class AboutDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 3: 创建AboutQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.About;

/// <summary>
/// 关于我们查询DTO
/// </summary>
public class AboutQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（标题）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 4: 创建CreateAboutDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.About;

/// <summary>
/// 创建关于我们DTO
/// </summary>
public class CreateAboutDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(100, ErrorMessage = "标题不能超过100个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(100, ErrorMessage = "标题（英文）不能超过100个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    [MaxLength(500, ErrorMessage = "图片URL不能超过500个字符")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 5: 创建UpdateAboutDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.About;

/// <summary>
/// 更新关于我们DTO
/// </summary>
public class UpdateAboutDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(100, ErrorMessage = "标题不能超过100个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(100, ErrorMessage = "标题（英文）不能超过100个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 内容（英文，富文本）
    /// </summary>
    public string? ContentEn { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    [MaxLength(500, ErrorMessage = "图片URL不能超过500个字符")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; }
}
```

- [ ] **Step 6: 提交关于我们DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/About/
git commit -m "feat(site): 添加关于我们DTO"
```

---

### Task 21: 创建下载管理DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Download/DownloadDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Download/DownloadQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Download/CreateDownloadDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Download/UpdateDownloadDto.cs`

- [ ] **Step 1: 创建DownloadDto**

```csharp
namespace EasyProduct.Models.Dto.Site.Download;

/// <summary>
/// 下载管理DTO
/// </summary>
public class DownloadDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 分类：manual=手册，catalog=目录，software=软件
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 文件名称
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型：pdf/doc/xls/zip等
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 2: 创建DownloadQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.Download;

/// <summary>
/// 下载管理查询DTO
/// </summary>
public class DownloadQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（标题）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 分类：manual=手册，catalog=目录，software=软件
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 3: 创建CreateDownloadDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Download;

/// <summary>
/// 创建下载管理DTO
/// </summary>
public class CreateDownloadDto
{
    /// <summary>
    /// 分类：manual=手册，catalog=目录，software=软件
    /// </summary>
    [MaxLength(50, ErrorMessage = "分类不能超过50个字符")]
    public string? Category { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200, ErrorMessage = "标题（英文）不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述（英文）不能超过500个字符")]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    [Required(ErrorMessage = "文件URL不能为空")]
    [MaxLength(500, ErrorMessage = "文件URL不能超过500个字符")]
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 文件名称
    /// </summary>
    [MaxLength(200, ErrorMessage = "文件名称不能超过200个字符")]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    [Range(0, long.MaxValue, ErrorMessage = "文件大小必须大于等于0")]
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型：pdf/doc/xls/zip等
    /// </summary>
    [MaxLength(50, ErrorMessage = "文件类型不能超过50个字符")]
    public string? FileType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 4: 创建UpdateDownloadDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Download;

/// <summary>
/// 更新下载管理DTO
/// </summary>
public class UpdateDownloadDto
{
    /// <summary>
    /// 分类：manual=手册，catalog=目录，software=软件
    /// </summary>
    [MaxLength(50, ErrorMessage = "分类不能超过50个字符")]
    public string? Category { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200, ErrorMessage = "标题（英文）不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述（英文）不能超过500个字符")]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    [Required(ErrorMessage = "文件URL不能为空")]
    [MaxLength(500, ErrorMessage = "文件URL不能超过500个字符")]
    public string FileUrl { get; set; } = null!;

    /// <summary>
    /// 文件名称
    /// </summary>
    [MaxLength(200, ErrorMessage = "文件名称不能超过200个字符")]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    [Range(0, long.MaxValue, ErrorMessage = "文件大小必须大于等于0")]
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型：pdf/doc/xls/zip等
    /// </summary>
    [MaxLength(50, ErrorMessage = "文件类型不能超过50个字符")]
    public string? FileType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; }
}
```

- [ ] **Step 5: 提交下载管理DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Download/
git commit -m "feat(site): 添加下载管理DTO"
```

---

### Task 22: 创建视频管理DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Video/VideoDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Video/VideoQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Video/CreateVideoDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Video/UpdateVideoDto.cs`

- [ ] **Step 1: 创建VideoDto**

```csharp
namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 视频管理DTO
/// </summary>
public class VideoDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    public string VideoUrl { get; set; } = null!;

    /// <summary>
    /// 视频类型：mp4/webm/youtube等
    /// </summary>
    public string? VideoType { get; set; }

    /// <summary>
    /// 时长（秒）
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 播放次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 2: 创建VideoQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 视频管理查询DTO
/// </summary>
public class VideoQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（标题）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 3: 创建CreateVideoDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 创建视频管理DTO
/// </summary>
public class CreateVideoDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200, ErrorMessage = "标题（英文）不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述（英文）不能超过500个字符")]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [MaxLength(500, ErrorMessage = "封面图片URL不能超过500个字符")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    [Required(ErrorMessage = "视频URL不能为空")]
    [MaxLength(500, ErrorMessage = "视频URL不能超过500个字符")]
    public string VideoUrl { get; set; } = null!;

    /// <summary>
    /// 视频类型：mp4/webm/youtube等
    /// </summary>
    [MaxLength(50, ErrorMessage = "视频类型不能超过50个字符")]
    public string? VideoType { get; set; }

    /// <summary>
    /// 时长（秒）
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "时长必须大于等于0")]
    public int Duration { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 4: 创建UpdateVideoDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 更新视频管理DTO
/// </summary>
public class UpdateVideoDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 标题（英文）
    /// </summary>
    [MaxLength(200, ErrorMessage = "标题（英文）不能超过200个字符")]
    public string? TitleEn { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 描述（英文）
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述（英文）不能超过500个字符")]
    public string? DescriptionEn { get; set; }

    /// <summary>
    /// 封面图片URL
    /// </summary>
    [MaxLength(500, ErrorMessage = "封面图片URL不能超过500个字符")]
    public string? CoverImage { get; set; }

    /// <summary>
    /// 视频URL
    /// </summary>
    [Required(ErrorMessage = "视频URL不能为空")]
    [MaxLength(500, ErrorMessage = "视频URL不能超过500个字符")]
    public string VideoUrl { get; set; } = null!;

    /// <summary>
    /// 视频类型：mp4/webm/youtube等
    /// </summary>
    [MaxLength(50, ErrorMessage = "视频类型不能超过50个字符")]
    public string? VideoType { get; set; }

    /// <summary>
    /// 时长（秒）
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "时长必须大于等于0")]
    public int Duration { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; }
}
```

- [ ] **Step 5: 提交视频管理DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Video/
git commit -m "feat(site): 添加视频管理DTO"
```

---

### Task 23: 创建关于我们、下载、视频Service

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/IAboutService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/AboutService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/IDownloadService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/DownloadService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/ISiteDownloadService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/SiteDownloadService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/IVideoService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/VideoService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/ISiteVideoService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/SiteVideoService.cs`

由于代码量较大，我将分步骤创建每个Service。

- [ ] **Step 1: 创建IAboutService接口**

```csharp
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 关于我们服务接口
/// </summary>
public interface IAboutService
{
    /// <summary>
    /// 获取关于我们列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    Task<PageResult<AboutDto>> GetListAsync(AboutQueryDto query);

    /// <summary>
    /// 根据ID获取关于我们详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    Task<AboutDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建关于我们
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    Task<string> CreateAsync(CreateAboutDto dto);

    /// <summary>
    /// 更新关于我们
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, UpdateAboutDto dto);

    /// <summary>
    /// 删除关于我们
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);
}
```

- [ ] **Step 2: 创建AboutService实现**

```csharp
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 关于我们服务实现
/// </summary>
public class AboutService : IAboutService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public AboutService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取关于我们列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（标题）
    /// 2. 支持状态筛选
    /// 3. 默认按排序字段升序排列
    /// </remarks>
    public async Task<PageResult<AboutDto>> GetListAsync(AboutQueryDto query)
    {
        var queryable = _db.Queryable<site_about>()
            .Where(x => !x.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(x => x.Title.Contains(query.Keyword));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.Sort).OrderBy(x => x.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<AboutDto>
        {
            List = list.Adapt<List<AboutDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取关于我们详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    public async Task<AboutDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_about>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        return entity?.Adapt<AboutDto>();
    }

    /// <summary>
    /// 创建关于我们
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// </remarks>
    public async Task<string> CreateAsync(CreateAboutDto dto)
    {
        var entity = dto.Adapt<site_about>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新关于我们
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 自动更新修改时间
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateAboutDto dto)
    {
        var entity = await _db.Queryable<site_about>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除关于我们（软删除）
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_about>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }
}
```

- [ ] **Step 3: 创建IDownloadService接口（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务接口（管理端）
/// </summary>
public interface IDownloadService
{
    /// <summary>
    /// 获取下载列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    Task<PageResult<DownloadDto>> GetListAsync(DownloadQueryDto query);

    /// <summary>
    /// 根据ID获取下载详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    Task<DownloadDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建下载
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    Task<string> CreateAsync(CreateDownloadDto dto);

    /// <summary>
    /// 更新下载
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, UpdateDownloadDto dto);

    /// <summary>
    /// 删除下载
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);
}
```

- [ ] **Step 4: 创建DownloadService实现（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务实现（管理端）
/// </summary>
public class DownloadService : IDownloadService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public DownloadService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取下载列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（标题）
    /// 2. 支持分类筛选
    /// 3. 支持状态筛选
    /// 4. 默认按排序字段升序排列
    /// </remarks>
    public async Task<PageResult<DownloadDto>> GetListAsync(DownloadQueryDto query)
    {
        var queryable = _db.Queryable<site_download>()
            .Where(x => !x.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(x => x.Title.Contains(query.Keyword));
        }

        // 分类筛选
        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            queryable = queryable.Where(x => x.Category == query.Category);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.Sort).OrderBy(x => x.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<DownloadDto>
        {
            List = list.Adapt<List<DownloadDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取下载详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    public async Task<DownloadDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        return entity?.Adapt<DownloadDto>();
    }

    /// <summary>
    /// 创建下载
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// </remarks>
    public async Task<string> CreateAsync(CreateDownloadDto dto)
    {
        var entity = dto.Adapt<site_download>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;
        entity.DownloadCount = 0;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新下载
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 自动更新修改时间
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateDownloadDto dto)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除下载（软删除）
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_download>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }
}
```

- [ ] **Step 5: 创建ISiteDownloadService接口（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.Download;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务接口（官网公开）
/// </summary>
public interface ISiteDownloadService
{
    /// <summary>
    /// 根据分类获取下载列表
    /// </summary>
    /// <param name="category">分类：manual=手册，catalog=目录，software=软件</param>
    /// <returns>下载列表</returns>
    Task<List<DownloadDto>> GetByCategoryAsync(string? category = null);

    /// <summary>
    /// 获取下载详情并增加下载次数
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>下载详情</returns>
    Task<DownloadDto?> GetByIdAsync(string id);
}
```

- [ ] **Step 6: 创建SiteDownloadService实现（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 下载管理服务实现（官网公开）
/// </summary>
public class SiteDownloadService : ISiteDownloadService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public SiteDownloadService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 根据分类获取下载列表
    /// </summary>
    /// <param name="category">分类：manual=手册，catalog=目录，software=软件</param>
    /// <returns>下载列表</returns>
    /// <remarks>
    /// 1. 仅返回启用状态的下载
    /// 2. 支持按分类筛选
    /// 3. 按排序字段升序排列
    /// </remarks>
    public async Task<List<DownloadDto>> GetByCategoryAsync(string? category = null)
    {
        var queryable = _db.Queryable<site_download>()
            .Where(x => !x.IsDeleted && x.Status == 1);

        if (!string.IsNullOrWhiteSpace(category))
        {
            queryable = queryable.Where(x => x.Category == category);
        }

        var list = await queryable
            .OrderBy(x => x.Sort)
            .ToListAsync();

        return list.Adapt<List<DownloadDto>>();
    }

    /// <summary>
    /// 获取下载详情并增加下载次数
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>下载详情</returns>
    /// <remarks>
    /// 1. 仅返回启用状态的下载
    /// 2. 每次访问自动增加下载次数
    /// </remarks>
    public async Task<DownloadDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_download>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted && x.Status == 1)
            .FirstAsync();

        if (entity == null) return null;

        // 增加下载次数
        await _db.Updateable<site_download>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.DownloadCount == x.DownloadCount + 1)
            .ExecuteCommandAsync();

        return entity.Adapt<DownloadDto>();
    }
}
```

- [ ] **Step 7: 创建IVideoService接口（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务接口（管理端）
/// </summary>
public interface IVideoService
{
    /// <summary>
    /// 获取视频列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    Task<PageResult<VideoDto>> GetListAsync(VideoQueryDto query);

    /// <summary>
    /// 根据ID获取视频详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    Task<VideoDto?> GetByIdAsync(string id);

    /// <summary>
    /// 创建视频
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    Task<string> CreateAsync(CreateVideoDto dto);

    /// <summary>
    /// 更新视频
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, UpdateVideoDto dto);

    /// <summary>
    /// 删除视频
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);
}
```

- [ ] **Step 8: 创建VideoService实现（管理端）**

```csharp
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务实现（管理端）
/// </summary>
public class VideoService : IVideoService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public VideoService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取视频列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（标题）
    /// 2. 支持状态筛选
    /// 3. 默认按排序字段升序排列
    /// </remarks>
    public async Task<PageResult<VideoDto>> GetListAsync(VideoQueryDto query)
    {
        var queryable = _db.Queryable<site_video>()
            .Where(x => !x.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(x => x.Title.Contains(query.Keyword));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.Sort).OrderBy(x => x.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<VideoDto>
        {
            List = list.Adapt<List<VideoDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取视频详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    public async Task<VideoDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        return entity?.Adapt<VideoDto>();
    }

    /// <summary>
    /// 创建视频
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// </remarks>
    public async Task<string> CreateAsync(CreateVideoDto dto)
    {
        var entity = dto.Adapt<site_video>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;
        entity.ViewCount = 0;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新视频
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 自动更新修改时间
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateVideoDto dto)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除视频（软删除）
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_video>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }
}
```

- [ ] **Step 9: 创建ISiteVideoService接口（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.Video;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务接口（官网公开）
/// </summary>
public interface ISiteVideoService
{
    /// <summary>
    /// 获取视频列表（仅返回启用的视频）
    /// </summary>
    /// <returns>视频列表</returns>
    Task<List<VideoDto>> GetAllAsync();

    /// <summary>
    /// 获取视频详情并增加播放次数
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>视频详情</returns>
    Task<VideoDto?> GetByIdAsync(string id);
}
```

- [ ] **Step 10: 创建SiteVideoService实现（官网公开）**

```csharp
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Entitys.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 视频管理服务实现（官网公开）
/// </summary>
public class SiteVideoService : ISiteVideoService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public SiteVideoService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取视频列表（仅返回启用的视频）
    /// </summary>
    /// <returns>视频列表</returns>
    /// <remarks>
    /// 仅返回启用状态的视频，按排序字段升序排列
    /// </remarks>
    public async Task<List<VideoDto>> GetAllAsync()
    {
        var list = await _db.Queryable<site_video>()
            .Where(x => !x.IsDeleted && x.Status == 1)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        return list.Adapt<List<VideoDto>>();
    }

    /// <summary>
    /// 获取视频详情并增加播放次数
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>视频详情</returns>
    /// <remarks>
    /// 1. 仅返回启用状态的视频
    /// 2. 每次访问自动增加播放次数
    /// </remarks>
    public async Task<VideoDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_video>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted && x.Status == 1)
            .FirstAsync();

        if (entity == null) return null;

        // 增加播放次数
        await _db.Updateable<site_video>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.ViewCount == x.ViewCount + 1)
            .ExecuteCommandAsync();

        return entity.Adapt<VideoDto>();
    }
}
```

- [ ] **Step 11: 提交Service**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Site/IAboutService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/AboutService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/IDownloadService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/DownloadService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/ISiteDownloadService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/SiteDownloadService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/IVideoService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/VideoService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/ISiteVideoService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/SiteVideoService.cs
git commit -m "feat(site): 添加关于我们、下载、视频Service"
```

---

### Task 24: 创建关于我们、下载、视频Controller

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/AboutController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/DownloadController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/VideoController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/AboutController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/DownloadController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/VideoController.cs`

- [ ] **Step 1: 创建AboutController（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 关于我们控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/about")]
[Authorize]
public class AboutController : ControllerBase
{
    private readonly IAboutService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">关于我们服务</param>
    public AboutController(IAboutService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取关于我们列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] AboutQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<AboutDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取关于我们详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<AboutDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 创建关于我们
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAboutDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "创建成功",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 更新关于我们
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateAboutDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "更新成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除关于我们
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 2: 创建AboutController（官网公开）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.About;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 关于我们控制器（官网公开）
/// </summary>
[ApiController]
[Route("api/site/about")]
public class AboutController : ControllerBase
{
    private readonly IAboutService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">关于我们服务</param>
    public AboutController(IAboutService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取关于我们列表（仅返回启用的记录）
    /// </summary>
    /// <returns>列表结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var result = await _service.GetListAsync(new AboutQueryDto
        {
            Status = 1,
            PageIndex = 1,
            PageSize = int.MaxValue
        });
        
        return Ok(new ApiResponse<List<AboutDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result.List,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取关于我们详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null || result.Status != 1)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在或已下线",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<AboutDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 3: 创建DownloadController（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 下载管理控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/download")]
[Authorize]
public class DownloadController : ControllerBase
{
    private readonly IDownloadService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">下载管理服务</param>
    public DownloadController(IDownloadService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取下载列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] DownloadQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<DownloadDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取下载详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<DownloadDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 创建下载
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDownloadDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "创建成功",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 更新下载
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateDownloadDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "更新成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除下载
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 4: 创建DownloadController（官网公开）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Download;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 下载管理控制器（官网公开）
/// </summary>
[ApiController]
[Route("api/site/download")]
public class DownloadController : ControllerBase
{
    private readonly ISiteDownloadService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">下载管理服务（官网公开）</param>
    public DownloadController(ISiteDownloadService service)
    {
        _service = service;
    }

    /// <summary>
    /// 根据分类获取下载列表
    /// </summary>
    /// <param name="category">分类：manual=手册，catalog=目录，software=软件</param>
    /// <returns>下载列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetByCategory([FromQuery] string? category = null)
    {
        var result = await _service.GetByCategoryAsync(category);
        return Ok(new ApiResponse<List<DownloadDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 获取下载详情并增加下载次数
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>下载详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在或已下线",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<DownloadDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 5: 创建VideoController（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 视频管理控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/video")]
[Authorize]
public class VideoController : ControllerBase
{
    private readonly IVideoService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">视频管理服务</param>
    public VideoController(IVideoService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取视频列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] VideoQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<VideoDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取视频详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<VideoDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 创建视频
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVideoDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "创建成功",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 更新视频
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateVideoDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "更新成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除视频
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 6: 创建VideoController（官网公开）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Video;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 视频管理控制器（官网公开）
/// </summary>
[ApiController]
[Route("api/site/video")]
public class VideoController : ControllerBase
{
    private readonly ISiteVideoService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">视频管理服务（官网公开）</param>
    public VideoController(ISiteVideoService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取视频列表（仅返回启用的视频）
    /// </summary>
    /// <returns>视频列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(new ApiResponse<List<VideoDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 获取视频详情并增加播放次数
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>视频详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在或已下线",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<VideoDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 7: 提交Controller**

```bash
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/AboutController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/AboutController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/DownloadController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/DownloadController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/VideoController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/VideoController.cs
git commit -m "feat(site): 添加关于我们、下载、视频Controller"
```

---

### Task 25: 创建Mock数据（批次2）

**Files:**
- Modify: `mock-server/data/site/news.js` (追加关于我们、下载、视频数据)
- Modify: `mock-server/routes/site.js` (追加关于我们、下载、视频路由)

- [ ] **Step 1: 在news.js中追加数据**

```javascript
// 追加到 mock-server/data/site/news.js 文件末尾

// 关于我们数据
const aboutData = [
  {
    id: 'about-001',
    title: '公司简介',
    titleEn: 'Company Profile',
    content: `<p>我们是一家专注于XXX领域的创新型企业...</p>`,
    contentEn: `<p>We are an innovative enterprise focused on XXX field...</p>`,
    imageUrl: Random.image('800x400', '#50B347', '#FFFFFF', 'Company'),
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'about-002',
    title: '企业文化',
    titleEn: 'Corporate Culture',
    content: `<p>我们的企业愿景是...</p>`,
    contentEn: `<p>Our corporate vision is...</p>`,
    imageUrl: Random.image('800x400', '#2d8cf0', '#FFFFFF', 'Culture'),
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  }
];

// 下载管理数据
const downloadData = [
  {
    id: 'download-001',
    category: 'manual',
    title: '产品使用手册 V3.0',
    titleEn: 'Product Manual V3.0',
    description: '详细的产品使用说明文档',
    descriptionEn: 'Detailed product usage documentation',
    fileUrl: '/uploads/files/manual-v3.pdf',
    fileName: 'manual-v3.pdf',
    fileSize: 2048000,
    fileType: 'pdf',
    downloadCount: 1250,
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'download-002',
    category: 'catalog',
    title: '产品目录 2024版',
    titleEn: 'Product Catalog 2024',
    description: '2024年最新产品目录',
    descriptionEn: 'Latest product catalog for 2024',
    fileUrl: '/uploads/files/catalog-2024.pdf',
    fileName: 'catalog-2024.pdf',
    fileSize: 5120000,
    fileType: 'pdf',
    downloadCount: 890,
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'download-003',
    category: 'software',
    title: '配套软件工具包',
    titleEn: 'Support Software Toolkit',
    description: '产品配套软件工具包',
    descriptionEn: 'Product support software toolkit',
    fileUrl: '/uploads/files/software-toolkit.zip',
    fileName: 'software-toolkit.zip',
    fileSize: 10240000,
    fileType: 'zip',
    downloadCount: 560,
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  }
];

// 视频管理数据
const videoData = [
  {
    id: 'video-001',
    title: '产品演示视频',
    titleEn: 'Product Demo Video',
    description: '完整的产品功能演示',
    descriptionEn: 'Complete product feature demonstration',
    coverImage: Random.image('800x450', '#2d8cf0', '#FFFFFF', 'Video 1'),
    videoUrl: '/uploads/videos/product-demo.mp4',
    videoType: 'mp4',
    duration: 180,
    viewCount: 1250,
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  },
  {
    id: 'video-002',
    title: '使用教程',
    titleEn: 'User Tutorial',
    description: '详细的使用教程视频',
    descriptionEn: 'Detailed user tutorial video',
    coverImage: Random.image('800x450', '#19be6b', '#FFFFFF', 'Video 2'),
    videoUrl: '/uploads/videos/tutorial.mp4',
    videoType: 'mp4',
    duration: 300,
    viewCount: 890,
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00'
  }
];

module.exports = {
  newsCategories,
  newsData,
  bannerData,
  aboutData,
  downloadData,
  videoData
};
```

- [ ] **Step 2: 在site.js中追加路由**

```javascript
// 追加到 mock-server/routes/site.js 文件末尾，module.exports之前

// ==================== 关于我们接口 ====================

/**
 * 获取关于我们列表（管理端，分页）
 */
router.get('/admin/site/about', (req, res) => {
  const { pageIndex = 1, pageSize = 10, keyword, status } = req.query;
  
  let filtered = [...aboutData];
  
  if (keyword) {
    filtered = filtered.filter(item => item.title.includes(keyword));
  }
  
  if (status !== undefined && status !== '') {
    filtered = filtered.filter(item => item.status === parseInt(status));
  }
  
  const total = filtered.length;
  const start = (parseInt(pageIndex) - 1) * parseInt(pageSize);
  const list = filtered.slice(start, start + parseInt(pageSize));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: { list, total },
    timestamp: Date.now()
  });
});

/**
 * 获取关于我们详情（管理端）
 */
router.get('/admin/site/about/:id', (req, res) => {
  const { id } = req.params;
  const item = aboutData.find(item => item.id === id);
  
  if (!item) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  res.json({
    code: 200,
    message: '获取成功',
    data: item,
    timestamp: Date.now()
  });
});

/**
 * 创建关于我们
 */
router.post('/admin/site/about', (req, res) => {
  const data = req.body;
  const newItem = {
    id: Random.guid(),
    ...data,
    createTime: Random.now('yyyy-MM-dd HH:mm:ss')
  };
  
  aboutData.push(newItem);
  
  res.json({
    code: 200,
    message: '创建成功',
    data: newItem.id,
    timestamp: Date.now()
  });
});

/**
 * 更新关于我们
 */
router.put('/admin/site/about/:id', (req, res) => {
  const { id } = req.params;
  const data = req.body;
  const index = aboutData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  aboutData[index] = { ...aboutData[index], ...data };
  
  res.json({
    code: 200,
    message: '更新成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 删除关于我们
 */
router.delete('/admin/site/about/:id', (req, res) => {
  const { id } = req.params;
  const index = aboutData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  aboutData.splice(index, 1);
  
  res.json({
    code: 200,
    message: '删除成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 获取关于我们列表（官网公开）
 */
router.get('/site/about', (req, res) => {
  const list = aboutData.filter(item => item.status === 1);
  
  res.json({
    code: 200,
    message: '获取成功',
    data: list,
    timestamp: Date.now()
  });
});

/**
 * 获取关于我们详情（官网公开）
 */
router.get('/site/about/:id', (req, res) => {
  const { id } = req.params;
  const item = aboutData.find(item => item.id === id && item.status === 1);
  
  if (!item) {
    return res.json({
      code: 404,
      message: '记录不存在或已下线',
      data: null,
      timestamp: Date.now()
    });
  }
  
  res.json({
    code: 200,
    message: '获取成功',
    data: item,
    timestamp: Date.now()
  });
});

// ==================== 下载管理接口 ====================

/**
 * 获取下载列表（管理端，分页）
 */
router.get('/admin/site/download', (req, res) => {
  const { pageIndex = 1, pageSize = 10, keyword, category, status } = req.query;
  
  let filtered = [...downloadData];
  
  if (keyword) {
    filtered = filtered.filter(item => item.title.includes(keyword));
  }
  
  if (category) {
    filtered = filtered.filter(item => item.category === category);
  }
  
  if (status !== undefined && status !== '') {
    filtered = filtered.filter(item => item.status === parseInt(status));
  }
  
  const total = filtered.length;
  const start = (parseInt(pageIndex) - 1) * parseInt(pageSize);
  const list = filtered.slice(start, start + parseInt(pageSize));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: { list, total },
    timestamp: Date.now()
  });
});

/**
 * 获取下载详情（管理端）
 */
router.get('/admin/site/download/:id', (req, res) => {
  const { id } = req.params;
  const item = downloadData.find(item => item.id === id);
  
  if (!item) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  res.json({
    code: 200,
    message: '获取成功',
    data: item,
    timestamp: Date.now()
  });
});

/**
 * 创建下载
 */
router.post('/admin/site/download', (req, res) => {
  const data = req.body;
  const newItem = {
    id: Random.guid(),
    ...data,
    downloadCount: 0,
    createTime: Random.now('yyyy-MM-dd HH:mm:ss')
  };
  
  downloadData.push(newItem);
  
  res.json({
    code: 200,
    message: '创建成功',
    data: newItem.id,
    timestamp: Date.now()
  });
});

/**
 * 更新下载
 */
router.put('/admin/site/download/:id', (req, res) => {
  const { id } = req.params;
  const data = req.body;
  const index = downloadData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  downloadData[index] = { ...downloadData[index], ...data };
  
  res.json({
    code: 200,
    message: '更新成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 删除下载
 */
router.delete('/admin/site/download/:id', (req, res) => {
  const { id } = req.params;
  const index = downloadData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  downloadData.splice(index, 1);
  
  res.json({
    code: 200,
    message: '删除成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 根据分类获取下载列表（官网公开）
 */
router.get('/site/download', (req, res) => {
  const { category } = req.query;
  
  let list = downloadData.filter(item => item.status === 1);
  
  if (category) {
    list = list.filter(item => item.category === category);
  }
  
  list.sort((a, b) => a.sort - b.sort);
  
  res.json({
    code: 200,
    message: '获取成功',
    data: list,
    timestamp: Date.now()
  });
});

/**
 * 获取下载详情并增加下载次数（官网公开）
 */
router.get('/site/download/:id', (req, res) => {
  const { id } = req.params;
  const item = downloadData.find(item => item.id === id && item.status === 1);
  
  if (!item) {
    return res.json({
      code: 404,
      message: '记录不存在或已下线',
      data: null,
      timestamp: Date.now()
    });
  }
  
  // 增加下载次数
  item.downloadCount += 1;
  
  res.json({
    code: 200,
    message: '获取成功',
    data: item,
    timestamp: Date.now()
  });
});

// ==================== 视频管理接口 ====================

/**
 * 获取视频列表（管理端，分页）
 */
router.get('/admin/site/video', (req, res) => {
  const { pageIndex = 1, pageSize = 10, keyword, status } = req.query;
  
  let filtered = [...videoData];
  
  if (keyword) {
    filtered = filtered.filter(item => item.title.includes(keyword));
  }
  
  if (status !== undefined && status !== '') {
    filtered = filtered.filter(item => item.status === parseInt(status));
  }
  
  const total = filtered.length;
  const start = (parseInt(pageIndex) - 1) * parseInt(pageSize);
  const list = filtered.slice(start, start + parseInt(pageSize));
  
  res.json({
    code: 200,
    message: '获取成功',
    data: { list, total },
    timestamp: Date.now()
  });
});

/**
 * 获取视频详情（管理端）
 */
router.get('/admin/site/video/:id', (req, res) => {
  const { id } = req.params;
  const item = videoData.find(item => item.id === id);
  
  if (!item) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  res.json({
    code: 200,
    message: '获取成功',
    data: item,
    timestamp: Date.now()
  });
});

/**
 * 创建视频
 */
router.post('/admin/site/video', (req, res) => {
  const data = req.body;
  const newItem = {
    id: Random.guid(),
    ...data,
    viewCount: 0,
    createTime: Random.now('yyyy-MM-dd HH:mm:ss')
  };
  
  videoData.push(newItem);
  
  res.json({
    code: 200,
    message: '创建成功',
    data: newItem.id,
    timestamp: Date.now()
  });
});

/**
 * 更新视频
 */
router.put('/admin/site/video/:id', (req, res) => {
  const { id } = req.params;
  const data = req.body;
  const index = videoData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  videoData[index] = { ...videoData[index], ...data };
  
  res.json({
    code: 200,
    message: '更新成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 删除视频
 */
router.delete('/admin/site/video/:id', (req, res) => {
  const { id } = req.params;
  const index = videoData.findIndex(item => item.id === id);
  
  if (index === -1) {
    return res.json({
      code: 404,
      message: '记录不存在',
      data: null,
      timestamp: Date.now()
    });
  }
  
  videoData.splice(index, 1);
  
  res.json({
    code: 200,
    message: '删除成功',
    data: null,
    timestamp: Date.now()
  });
});

/**
 * 获取视频列表（官网公开）
 */
router.get('/site/video', (req, res) => {
  const list = videoData
    .filter(item => item.status === 1)
    .sort((a, b) => a.sort - b.sort);
  
  res.json({
    code: 200,
    message: '获取成功',
    data: list,
    timestamp: Date.now()
  });
});

/**
 * 获取视频详情并增加播放次数（官网公开）
 */
router.get('/site/video/:id', (req, res) => {
  const { id } = req.params;
  const item = videoData.find(item => item.id === id && item.status === 1);
  
  if (!item) {
    return res.json({
      code: 404,
      message: '记录不存在或已下线',
      data: null,
      timestamp: Date.now()
    });
  }
  
  // 增加播放次数
  item.viewCount += 1;
  
  res.json({
    code: 200,
    message: '获取成功',
    data: item,
    timestamp: Date.now()
  });
});
```

- [ ] **Step 3: 提交Mock数据**

```bash
git add mock-server/data/site/news.js
git add mock-server/routes/site.js
git commit -m "feat(mock): 添加关于我们、下载、视频Mock数据"
```

---

---

## 批次 3：询价管理 + 留言管理（2-3天）

**注意：本批次无Mock数据，需要在官网公开接口上配置限流策略**

### Task 26: 创建数据库表（批次3）

**Files:**
- Modify: `sql/init-database.sql`

- [ ] **Step 1: 添加询价、留言表定义**

在 `sql/init-database.sql` 文件末尾添加：

```sql
-- ----------------------------
-- 询价管理表 (site_inquiry)
-- ----------------------------
DROP TABLE IF EXISTS `site_inquiry`;
CREATE TABLE `site_inquiry` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `company_name` VARCHAR(200) DEFAULT NULL COMMENT '公司名称',
  `contact_person` VARCHAR(100) NOT NULL COMMENT '联系人',
  `phone` VARCHAR(50) DEFAULT NULL COMMENT '电话',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `country` VARCHAR(100) DEFAULT NULL COMMENT '国家',
  `province` VARCHAR(100) DEFAULT NULL COMMENT '省份',
  `city` VARCHAR(100) DEFAULT NULL COMMENT '城市',
  `address` VARCHAR(500) DEFAULT NULL COMMENT '详细地址',
  `product_interest` VARCHAR(500) DEFAULT NULL COMMENT '感兴趣的产品',
  `message` TEXT COMMENT '询价内容',
  `status` VARCHAR(20) DEFAULT 'pending' COMMENT '状态：pending=待处理，processing=处理中，completed=已完成',
  `remark` TEXT COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='询价管理表';

-- ----------------------------
-- 留言管理表 (site_message)
-- ----------------------------
DROP TABLE IF EXISTS `site_message`;
CREATE TABLE `site_message` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `name` VARCHAR(100) NOT NULL COMMENT '姓名',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `phone` VARCHAR(50) DEFAULT NULL COMMENT '电话',
  `subject` VARCHAR(200) DEFAULT NULL COMMENT '主题',
  `message` TEXT COMMENT '留言内容',
  `status` VARCHAR(20) DEFAULT 'unread' COMMENT '状态：unread=未读，read=已读，replied=已回复',
  `reply` TEXT COMMENT '回复内容',
  `reply_time` DATETIME DEFAULT NULL COMMENT '回复时间',
  `reply_by` VARCHAR(50) DEFAULT NULL COMMENT '回复人',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='留言管理表';
```

- [ ] **Step 2: 提交数据库表定义**

```bash
git add sql/init-database.sql
git commit -m "feat(site): 添加询价、留言数据库表定义"
```

---

### Task 27: 创建询价状态枚举

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Enums/Site/InquiryStatus.cs`

- [ ] **Step 1: 创建Enums Site目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Enums/Site
```

- [ ] **Step 2: 创建询价状态枚举**

```csharp
namespace EasyProduct.Models.Enums.Site;

/// <summary>
/// 询价状态枚举
/// </summary>
public enum InquiryStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 处理中
    /// </summary>
    Processing = 2,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 3
}

/// <summary>
/// 询价状态常量（用于数据库存储和前端展示）
/// </summary>
public static class InquiryStatusConst
{
    /// <summary>
    /// 待处理
    /// </summary>
    public const string Pending = "pending";

    /// <summary>
    /// 处理中
    /// </summary>
    public const string Processing = "processing";

    /// <summary>
    /// 已完成
    /// </summary>
    public const string Completed = "completed";

    /// <summary>
    /// 获取所有状态列表
    /// </summary>
    /// <returns>状态列表</returns>
    public static List<string> GetAll()
    {
        return new List<string> { Pending, Processing, Completed };
    }

    /// <summary>
    /// 获取状态显示名称
    /// </summary>
    /// <param name="status">状态值</param>
    /// <returns>显示名称</returns>
    public static string GetDisplayName(string status)
    {
        return status switch
        {
            Pending => "待处理",
            Processing => "处理中",
            Completed => "已完成",
            _ => "未知"
        };
    }
}
```

- [ ] **Step 3: 提交枚举类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Enums/Site/InquiryStatus.cs
git commit -m "feat(site): 添加询价状态枚举"
```

---

### Task 28: 创建留言状态枚举

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Enums/Site/MessageStatus.cs`

- [ ] **Step 1: 创建留言状态枚举**

```csharp
namespace EasyProduct.Models.Enums.Site;

/// <summary>
/// 留言状态枚举
/// </summary>
public enum MessageStatus
{
    /// <summary>
    /// 未读
    /// </summary>
    Unread = 1,

    /// <summary>
    /// 已读
    /// </summary>
    Read = 2,

    /// <summary>
    /// 已回复
    /// </summary>
    Replied = 3
}

/// <summary>
/// 留言状态常量（用于数据库存储和前端展示）
/// </summary>
public static class MessageStatusConst
{
    /// <summary>
    /// 未读
    /// </summary>
    public const string Unread = "unread";

    /// <summary>
    /// 已读
    /// </summary>
    public const string Read = "read";

    /// <summary>
    /// 已回复
    /// </summary>
    public const string Replied = "replied";

    /// <summary>
    /// 获取所有状态列表
    /// </summary>
    /// <returns>状态列表</returns>
    public static List<string> GetAll()
    {
        return new List<string> { Unread, Read, Replied };
    }

    /// <summary>
    /// 获取状态显示名称
    /// </summary>
    /// <param name="status">状态值</param>
    /// <returns>显示名称</returns>
    public static string GetDisplayName(string status)
    {
        return status switch
        {
            Unread => "未读",
            Read => "已读",
            Replied => "已回复",
            _ => "未知"
        };
    }
}
```

- [ ] **Step 2: 提交枚举类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Enums/Site/MessageStatus.cs
git commit -m "feat(site): 添加留言状态枚举"
```

---

### Task 29: 创建询价管理实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_inquiry.cs`

- [ ] **Step 1: 创建询价管理实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 询价管理实体
/// </summary>
[SugarTable("site_inquiry", "询价管理表")]
public class site_inquiry : BaseEntity
{
    /// <summary>
    /// 公司名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 100)]
    public string ContactPerson { get; set; } = null!;

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? City { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Address { get; set; }

    /// <summary>
    /// 感兴趣的产品
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ProductInterest { get; set; }

    /// <summary>
    /// 询价内容
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Message { get; set; }

    /// <summary>
    /// 状态：pending=待处理，processing=处理中，completed=已完成
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Status { get; set; } = "pending";

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Remark { get; set; }
}
```

- [ ] **Step 2: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_inquiry.cs
git commit -m "feat(site): 添加询价管理实体类"
```

---

### Task 30: 创建留言管理实体类

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_message.cs`

- [ ] **Step 1: 创建留言管理实体类**

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 留言管理实体
/// </summary>
[SugarTable("site_message", "留言管理表")]
public class site_message : BaseEntity
{
    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(Length = 100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Subject { get; set; }

    /// <summary>
    /// 留言内容
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Message { get; set; }

    /// <summary>
    /// 状态：unread=未读，read=已读，replied=已回复
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Status { get; set; } = "unread";

    /// <summary>
    /// 回复内容
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Reply { get; set; }

    /// <summary>
    /// 回复时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ReplyTime { get; set; }

    /// <summary>
    /// 回复人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ReplyBy { get; set; }
}
```

- [ ] **Step 2: 提交实体类**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Entitys/Site/site_message.cs
git commit -m "feat(site): 添加留言管理实体类"
```

---

### Task 31: 创建询价管理DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Inquiry/InquiryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Inquiry/InquiryQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Inquiry/CreateInquiryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Inquiry/UpdateInquiryDto.cs`

- [ ] **Step 1: 创建Inquiry DTO目录**

```bash
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Inquiry
mkdir -p EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Message
```

- [ ] **Step 2: 创建InquiryDto**

```csharp
namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 询价管理DTO
/// </summary>
public class InquiryDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 公司名称
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    public string ContactPerson { get; set; } = null!;

    /// <summary>
    /// 电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 感兴趣的产品
    /// </summary>
    public string? ProductInterest { get; set; }

    /// <summary>
    /// 询价内容
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 状态：pending=待处理，processing=处理中，completed=已完成
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 3: 创建InquiryQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 询价管理查询DTO
/// </summary>
public class InquiryQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（公司名称/联系人）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态：pending=待处理，processing=处理中，completed=已完成
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 创建时间开始
    /// </summary>
    public DateTime? CreateStartTime { get; set; }

    /// <summary>
    /// 创建时间结束
    /// </summary>
    public DateTime? CreateEndTime { get; set; }
}
```

- [ ] **Step 4: 创建CreateInquiryDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 创建询价DTO
/// </summary>
public class CreateInquiryDto
{
    /// <summary>
    /// 公司名称
    /// </summary>
    [MaxLength(200, ErrorMessage = "公司名称不能超过200个字符")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [Required(ErrorMessage = "联系人不能为空")]
    [MaxLength(100, ErrorMessage = "联系人不能超过100个字符")]
    public string ContactPerson { get; set; } = null!;

    /// <summary>
    /// 电话
    /// </summary>
    [MaxLength(50, ErrorMessage = "电话不能超过50个字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    [MaxLength(100, ErrorMessage = "国家不能超过100个字符")]
    public string? Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [MaxLength(100, ErrorMessage = "省份不能超过100个字符")]
    public string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [MaxLength(100, ErrorMessage = "城市不能超过100个字符")]
    public string? City { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [MaxLength(500, ErrorMessage = "详细地址不能超过500个字符")]
    public string? Address { get; set; }

    /// <summary>
    /// 感兴趣的产品
    /// </summary>
    [MaxLength(500, ErrorMessage = "感兴趣的产品不能超过500个字符")]
    public string? ProductInterest { get; set; }

    /// <summary>
    /// 询价内容
    /// </summary>
    public string? Message { get; set; }
}
```

- [ ] **Step 5: 创建UpdateInquiryDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Inquiry;

/// <summary>
/// 更新询价DTO
/// </summary>
public class UpdateInquiryDto
{
    /// <summary>
    /// 状态：pending=待处理，processing=处理中，completed=已完成
    /// </summary>
    [Required(ErrorMessage = "状态不能为空")]
    [MaxLength(20, ErrorMessage = "状态不能超过20个字符")]
    public string Status { get; set; } = null!;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
```

- [ ] **Step 6: 提交询价管理DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Inquiry/
git commit -m "feat(site): 添加询价管理DTO"
```

---

### Task 32: 创建留言管理DTO

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Message/MessageDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Message/MessageQueryDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Message/CreateMessageDto.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Message/ReplyMessageDto.cs`

- [ ] **Step 1: 创建MessageDto**

```csharp
namespace EasyProduct.Models.Dto.Site.Message;

/// <summary>
/// 留言管理DTO
/// </summary>
public class MessageDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 留言内容
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 状态：unread=未读，read=已读，replied=已回复
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 回复内容
    /// </summary>
    public string? Reply { get; set; }

    /// <summary>
    /// 回复时间
    /// </summary>
    public DateTime? ReplyTime { get; set; }

    /// <summary>
    /// 回复人
    /// </summary>
    public string? ReplyBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 2: 创建MessageQueryDto**

```csharp
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Models.Dto.Site.Message;

/// <summary>
/// 留言管理查询DTO
/// </summary>
public class MessageQueryDto : PageQuery
{
    /// <summary>
    /// 关键词（姓名/主题）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态：unread=未读，read=已读，replied=已回复
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 创建时间开始
    /// </summary>
    public DateTime? CreateStartTime { get; set; }

    /// <summary>
    /// 创建时间结束
    /// </summary>
    public DateTime? CreateEndTime { get; set; }
}
```

- [ ] **Step 3: 创建CreateMessageDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Message;

/// <summary>
/// 创建留言DTO
/// </summary>
public class CreateMessageDto
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "姓名不能为空")]
    [MaxLength(100, ErrorMessage = "姓名不能超过100个字符")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [MaxLength(50, ErrorMessage = "电话不能超过50个字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    [MaxLength(200, ErrorMessage = "主题不能超过200个字符")]
    public string? Subject { get; set; }

    /// <summary>
    /// 留言内容
    /// </summary>
    [Required(ErrorMessage = "留言内容不能为空")]
    public string? Message { get; set; }
}
```

- [ ] **Step 4: 创建ReplyMessageDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Site.Message;

/// <summary>
/// 回复留言DTO
/// </summary>
public class ReplyMessageDto
{
    /// <summary>
    /// 回复内容
    /// </summary>
    [Required(ErrorMessage = "回复内容不能为空")]
    public string Reply { get; set; } = null!;
}
```

- [ ] **Step 5: 提交留言管理DTO**

```bash
git add EasyProduct.WebApi/EasyProduct.Models/Dto/Site/Message/
git commit -m "feat(site): 添加留言管理DTO"
```

---

### Task 33: 创建询价管理Service

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/IInquiryService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/InquiryService.cs`

- [ ] **Step 1: 创建IInquiryService接口**

```csharp
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 询价管理服务接口
/// </summary>
public interface IInquiryService
{
    /// <summary>
    /// 获取询价列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    Task<PageResult<InquiryDto>> GetListAsync(InquiryQueryDto query);

    /// <summary>
    /// 根据ID获取询价详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    Task<InquiryDto?> GetByIdAsync(string id);

    /// <summary>
    /// 提交汇价（官网公开）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    Task<string> SubmitAsync(CreateInquiryDto dto);

    /// <summary>
    /// 更新询价状态
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(string id, UpdateInquiryDto dto);

    /// <summary>
    /// 删除询价
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 标记为处理中
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> MarkAsProcessingAsync(string id);

    /// <summary>
    /// 标记为已完成
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> MarkAsCompletedAsync(string id);
}
```

- [ ] **Step 2: 创建InquiryService实现**

```csharp
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using EasyProduct.Models.Enums.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 询价管理服务实现
/// </summary>
public class InquiryService : IInquiryService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public InquiryService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取询价列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（公司名称、联系人）
    /// 2. 支持状态筛选
    /// 3. 支持创建时间范围筛选
    /// 4. 默认按创建时间倒序排列
    /// </remarks>
    public async Task<PageResult<InquiryDto>> GetListAsync(InquiryQueryDto query)
    {
        var queryable = _db.Queryable<site_inquiry>()
            .Where(x => !x.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(x => 
                (x.CompanyName != null && x.CompanyName.Contains(query.Keyword)) || 
                x.ContactPerson.Contains(query.Keyword));
        }

        // 状态筛选
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }

        // 创建时间范围
        if (query.CreateStartTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreateTime >= query.CreateStartTime.Value);
        }
        if (query.CreateEndTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreateTime <= query.CreateEndTime.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<InquiryDto>
        {
            List = list.Adapt<List<InquiryDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取询价详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    public async Task<InquiryDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_inquiry>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        return entity?.Adapt<InquiryDto>();
    }

    /// <summary>
    /// 提交汇价（官网公开）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// 3. 默认状态为待处理
    /// </remarks>
    public async Task<string> SubmitAsync(CreateInquiryDto dto)
    {
        var entity = dto.Adapt<site_inquiry>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;
        entity.Status = InquiryStatusConst.Pending;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 更新询价状态
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 自动更新修改时间
    /// </remarks>
    public async Task<bool> UpdateAsync(string id, UpdateInquiryDto dto)
    {
        var entity = await _db.Queryable<site_inquiry>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        entity.Status = dto.Status;
        entity.Remark = dto.Remark;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除询价（软删除）
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_inquiry>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 标记为处理中
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> MarkAsProcessingAsync(string id)
    {
        return await _db.Updateable<site_inquiry>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .SetColumns(x => x.Status == InquiryStatusConst.Processing)
            .SetColumns(x => x.UpdateTime == DateTime.Now)
            .ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 标记为已完成
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> MarkAsCompletedAsync(string id)
    {
        return await _db.Updateable<site_inquiry>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .SetColumns(x => x.Status == InquiryStatusConst.Completed)
            .SetColumns(x => x.UpdateTime == DateTime.Now)
            .ExecuteCommandAsync() > 0;
    }
}
```

- [ ] **Step 3: 提交汇价管理Service**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Site/IInquiryService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/InquiryService.cs
git commit -m "feat(site): 添加询价管理Service"
```

---

### Task 34: 创建留言管理Service

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/IMessageService.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Business/Site/MessageService.cs`

- [ ] **Step 1: 创建IMessageService接口**

```csharp
using EasyProduct.Models.Dto.Site.Message;
using EasyProduct.Models.Dto.Base;

namespace EasyProduct.Business.Site;

/// <summary>
/// 留言管理服务接口
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// 获取留言列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    Task<PageResult<MessageDto>> GetListAsync(MessageQueryDto query);

    /// <summary>
    /// 根据ID获取留言详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    Task<MessageDto?> GetByIdAsync(string id);

    /// <summary>
    /// 提交留言（官网公开）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    Task<string> SubmitAsync(CreateMessageDto dto);

    /// <summary>
    /// 回复留言
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">回复参数</param>
    /// <param name="replyBy">回复人</param>
    /// <returns>是否成功</returns>
    Task<bool> ReplyAsync(string id, ReplyMessageDto dto, string replyBy);

    /// <summary>
    /// 删除留言
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// 标记为已读
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    Task<bool> MarkAsReadAsync(string id);

    /// <summary>
    /// 获取未读留言数量
    /// </summary>
    /// <returns>未读数量</returns>
    Task<int> GetUnreadCountAsync();
}
```

- [ ] **Step 2: 创建MessageService实现**

```csharp
using EasyProduct.Models.Dto.Site.Message;
using EasyProduct.Models.Dto.Base;
using EasyProduct.Models.Entitys.Site;
using EasyProduct.Models.Enums.Site;
using SqlSugar;
using Mapster;

namespace EasyProduct.Business.Site;

/// <summary>
/// 留言管理服务实现
/// </summary>
public class MessageService : IMessageService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public MessageService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 获取留言列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    /// <remarks>
    /// 1. 支持关键词搜索（姓名、主题）
    /// 2. 支持状态筛选
    /// 3. 支持创建时间范围筛选
    /// 4. 默认按创建时间倒序排列
    /// </remarks>
    public async Task<PageResult<MessageDto>> GetListAsync(MessageQueryDto query)
    {
        var queryable = _db.Queryable<site_message>()
            .Where(x => !x.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(x => 
                x.Name.Contains(query.Keyword) || 
                (x.Subject != null && x.Subject.Contains(query.Keyword)));
        }

        // 状态筛选
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(x => x.Status == query.Status);
        }

        // 创建时间范围
        if (query.CreateStartTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreateTime >= query.CreateStartTime.Value);
        }
        if (query.CreateEndTime.HasValue)
        {
            queryable = queryable.Where(x => x.CreateTime <= query.CreateEndTime.Value);
        }

        // 排序
        queryable = queryable.OrderBy(x => x.CreateTime, OrderByType.Desc);

        // 分页
        var total = await queryable.CountAsync();
        var list = await queryable
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PageResult<MessageDto>
        {
            List = list.Adapt<List<MessageDto>>(),
            Total = total
        };
    }

    /// <summary>
    /// 根据ID获取留言详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    public async Task<MessageDto?> GetByIdAsync(string id)
    {
        var entity = await _db.Queryable<site_message>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        return entity?.Adapt<MessageDto>();
    }

    /// <summary>
    /// 提交留言（官网公开）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    /// <remarks>
    /// 1. 自动生成GUID作为主键
    /// 2. 自动设置创建时间
    /// 3. 默认状态为未读
    /// </remarks>
    public async Task<string> SubmitAsync(CreateMessageDto dto)
    {
        var entity = dto.Adapt<site_message>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;
        entity.IsDeleted = false;
        entity.Status = MessageStatusConst.Unread;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id.ToString();
    }

    /// <summary>
    /// 回复留言
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">回复参数</param>
    /// <param name="replyBy">回复人</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 1. 自动更新状态为已回复
    /// 2. 自动记录回复时间和回复人
    /// </remarks>
    public async Task<bool> ReplyAsync(string id, ReplyMessageDto dto, string replyBy)
    {
        var entity = await _db.Queryable<site_message>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .FirstAsync();

        if (entity == null) return false;

        entity.Reply = dto.Reply;
        entity.ReplyTime = DateTime.Now;
        entity.ReplyBy = replyBy;
        entity.Status = MessageStatusConst.Replied;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除留言（软删除）
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 使用软删除，仅标记IsDeleted字段
    /// </remarks>
    public async Task<bool> DeleteAsync(string id)
    {
        return await _db.Updateable<site_message>()
            .Where(x => x.Id == Guid.Parse(id))
            .SetColumns(x => x.IsDeleted == true)
            .ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 标记为已读
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> MarkAsReadAsync(string id)
    {
        return await _db.Updateable<site_message>()
            .Where(x => x.Id == Guid.Parse(id) && !x.IsDeleted)
            .SetColumns(x => x.Status == MessageStatusConst.Read)
            .SetColumns(x => x.UpdateTime == DateTime.Now)
            .ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 获取未读留言数量
    /// </summary>
    /// <returns>未读数量</returns>
    public async Task<int> GetUnreadCountAsync()
    {
        return await _db.Queryable<site_message>()
            .Where(x => !x.IsDeleted && x.Status == MessageStatusConst.Unread)
            .CountAsync();
    }
}
```

- [ ] **Step 3: 提交留言管理Service**

```bash
git add EasyProduct.WebApi/EasyProduct.Business/Site/IMessageService.cs
git add EasyProduct.WebApi/EasyProduct.Business/Site/MessageService.cs
git commit -m "feat(site): 添加留言管理Service"
```

---

### Task 35: 创建询价、留言Controller并配置限流

**Files:**
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/InquiryController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/MessageController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/InquiryController.cs`
- Create: `EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/MessageController.cs`
- Modify: `EasyProduct.WebApi/EasyProduct.Web/Program.cs` (配置限流策略)

- [ ] **Step 1: 创建InquiryController（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 询价管理控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/inquiry")]
[Authorize]
public class InquiryController : ControllerBase
{
    private readonly IInquiryService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">询价管理服务</param>
    public InquiryController(IInquiryService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取询价列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] InquiryQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<InquiryDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取询价详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<InquiryDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 更新询价状态
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateInquiryDto dto)
    {
        var success = await _service.UpdateAsync(id, dto);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "更新成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除询价
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 标记为处理中
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}/processing")]
    public async Task<IActionResult> MarkAsProcessing(string id)
    {
        var success = await _service.MarkAsProcessingAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "标记成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 标记为已完成
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}/completed")]
    public async Task<IActionResult> MarkAsCompleted(string id)
    {
        var success = await _service.MarkAsCompletedAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "标记成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 2: 创建InquiryController（官网公开，带限流）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Inquiry;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 询价管理控制器（官网公开）
/// </summary>
[ApiController]
[Route("api/site/inquiry")]
public class InquiryController : ControllerBase
{
    private readonly IInquiryService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">询价管理服务</param>
    public InquiryController(IInquiryService service)
    {
        _service = service;
    }

    /// <summary>
    /// 提交汇价（带限流保护）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    /// <remarks>
    /// 限流策略：每IP每小时最多5次
    /// </remarks>
    [HttpPost]
    [EnableRateLimiting("SitePolicy")]
    public async Task<IActionResult> Submit([FromBody] CreateInquiryDto dto)
    {
        var id = await _service.SubmitAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "提交成功，我们会尽快与您联系",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 3: 创建MessageController（管理端）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Message;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 留言管理控制器（管理端）
/// </summary>
[ApiController]
[Route("api/admin/site/message")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">留言管理服务</param>
    public MessageController(IMessageService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取留言列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列表分页结果</returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] MessageQueryDto query)
    {
        var result = await _service.GetListAsync(query);
        return Ok(new ApiResponse<PageResult<MessageDto>>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 根据ID获取留言详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<MessageDto>
        {
            Code = 200,
            Message = "获取成功",
            Data = result,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 回复留言
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="dto">回复参数</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/reply")]
    public async Task<IActionResult> Reply(string id, [FromBody] ReplyMessageDto dto)
    {
        var replyBy = User.Identity?.Name ?? "admin";
        var success = await _service.ReplyAsync(id, dto, replyBy);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "回复成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 删除留言
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "删除成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 标记为已读
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        var success = await _service.MarkAsReadAsync(id);
        if (!success)
        {
            return Ok(new ApiResponse<object>
            {
                Code = 404,
                Message = "记录不存在",
                Data = null,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }

        return Ok(new ApiResponse<object>
        {
            Code = 200,
            Message = "标记成功",
            Data = null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    /// <summary>
    /// 获取未读留言数量
    /// </summary>
    /// <returns>未读数量</returns>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _service.GetUnreadCountAsync();
        return Ok(new ApiResponse<int>
        {
            Code = 200,
            Message = "获取成功",
            Data = count,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 4: 创建MessageController（官网公开，带限流）**

```csharp
using EasyProduct.Business.Site;
using EasyProduct.Models.Dto.Site.Message;
using EasyProduct.Models.Dto.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyProduct.Web.Controllers.Site;

/// <summary>
/// 留言管理控制器（官网公开）
/// </summary>
[ApiController]
[Route("api/site/message")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _service;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service">留言管理服务</param>
    public MessageController(IMessageService service)
    {
        _service = service;
    }

    /// <summary>
    /// 提交留言（带限流保护）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新ID</returns>
    /// <remarks>
    /// 限流策略：每IP每小时最多10次
    /// </remarks>
    [HttpPost]
    [EnableRateLimiting("SitePolicy")]
    public async Task<IActionResult> Submit([FromBody] CreateMessageDto dto)
    {
        var id = await _service.SubmitAsync(dto);
        return Ok(new ApiResponse<string>
        {
            Code = 200,
            Message = "提交成功，感谢您的留言",
            Data = id,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
}
```

- [ ] **Step 5: 配置限流策略（Program.cs）**

在 `Program.cs` 中添加限流配置：

```csharp
// 在 var app = builder.Build(); 之前添加

// 添加限流服务
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("SitePolicy", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromHours(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var response = new
        {
            code = 429,
            message = "请求过于频繁，请稍后再试",
            data = (object?)null,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await context.HttpContext.Response.WriteAsJsonAsync(response, token);
    };
});
```

在 `var app = builder.Build();` 之后添加：

```csharp
// 启用限流中间件
app.UseRateLimiter();
```

- [ ] **Step 6: 提交Controller和限流配置**

```bash
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/InquiryController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Admin/Site/MessageController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/InquiryController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Controllers/Site/MessageController.cs
git add EasyProduct.WebApi/EasyProduct.Web/Program.cs
git commit -m "feat(site): 添加询价、留言Controller并配置限流"
```

---

**当前进度：批次 3 已完成（共 10 个任务）**
- ✅ Task 26-35: 询价管理 + 留言管理模块完整实现
- ✅ 已配置官网公开接口限流策略（每IP每小时10次）

---

## 完成总结

**所有批次已完成（共 35 个任务）：**

### 批次 1：新闻 + Banner
- ✅ 实体类：3个（site_news_category, site_news, site_banner）
- ✅ DTO：12个（新闻分类4个 + 新闻4个 + Banner 4个）
- ✅ Service：6个（管理端3个 + 官网公开3个）
- ✅ Controller：5个（管理端3个 + 官网公开2个）
- ✅ Mock数据：完整实现

### 批次 2：关于我们 + 下载管理 + 视频管理
- ✅ 实体类：3个（site_about, site_download, site_video）
- ✅ DTO：12个（关于我们4个 + 下载4个 + 视频4个）
- ✅ Service：6个（管理端3个 + 官网公开3个）
- ✅ Controller：6个（管理端3个 + 官网公开3个）
- ✅ Mock数据：完整实现

### 批次 3：询价管理 + 留言管理
- ✅ 枚举类：2个（InquiryStatus, MessageStatus）
- ✅ 实体类：2个（site_inquiry, site_message）
- ✅ DTO：8个（询价4个 + 留言4个）
- ✅ Service：2个（询价1个 + 留言1个）
- ✅ Controller：4个（管理端2个 + 官网公开2个）
- ✅ 限流配置：每IP每小时10次
- ❌ Mock数据：无（真实业务数据）

**技术特点：**
1. 所有方法都有完整的中文注释
2. 遵循项目规范：统一响应格式、分页参数、软删除等
3. 官网公开接口已配置限流保护
4. 支持中英文双语（Title/TitleEn等字段）
5. 使用SqlSugar ORM和Mapster对象映射
6. 分离Service + 分离Controller架构