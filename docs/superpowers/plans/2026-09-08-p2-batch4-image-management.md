# P2 批次 4：商品图集管理实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现商品图集管理功能，支持商品图片的上传、管理、排序和删除。

**Architecture:** 基于批次 1-3 已完成的分类、SPU、SKU，实现商品图集管理。支持多图片上传、图片排序、主图设置。

**Tech Stack:** .NET 8.0 + SqlSugar 5.1.4 + Mapster 10.x + IFileStorageHelper

---

## 模块概述

**商品图集（product_image）**
- 商品图片管理
- 关联 SPU（product_spu）
- 支持多图片上传
- 支持图片排序
- 支持设置主图
- 支持图片删除

---

## 任务划分

| 任务 | 内容 | 预计工时 |
|------|------|---------|
| **Task 1** | 更新数据库添加图片表 | 0.5 天 |
| **Task 2** | 创建图片实体 | 0.5 天 |
| **Task 3** | 创建图片 DTO | 0.5 天 |
| **Task 4** | 创建图片服务 | 1 天 |
| **Task 5** | 创建图片控制器 | 1 天 |

---

## Task 1: 更新数据库添加商品图片表

**Files:**
- Modify: `sql/init-database.sql`

- [ ] **Step 1: 添加商品图片表定义**

在 `sql/init-database.sql` 文件的 Product 模块部分添加：

```sql
-- ----------------------------
-- 商品图集表 (product_image)
-- ----------------------------
DROP TABLE IF EXISTS `product_image`;
CREATE TABLE `product_image` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `spu_id` CHAR(36) NOT NULL COMMENT '商品ID',
  `image_url` VARCHAR(500) NOT NULL COMMENT '图片URL',
  `thumbnail_url` VARCHAR(500) DEFAULT NULL COMMENT '缩略图URL',
  `image_name` VARCHAR(200) DEFAULT NULL COMMENT '图片名称',
  `image_size` INT DEFAULT NULL COMMENT '图片大小（字节）',
  `image_type` VARCHAR(50) DEFAULT NULL COMMENT '图片类型（jpg/png/webp等）',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `is_main` TINYINT(1) DEFAULT 0 COMMENT '是否主图：0=否，1=是',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_spu_id` (`spu_id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_is_main` (`is_main`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品图集表';
```

- [ ] **Step 2: 提交**

```bash
git add sql/init-database.sql
git commit -m "feat(db): 添加商品图集表定义

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建商品图片实体

**Files:**
- Create: `EasyProduct.Models\Entitys\Product\product_image.cs`

- [ ] **Step 1: 创建图片实体类**

创建文件 `EasyProduct.Models\Entitys\Product\product_image.cs`：

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品图集实体
/// </summary>
[SugarTable("product_image", "商品图集表")]
public class product_image : BaseEntity
{
    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "商品ID")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "图片URL")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "缩略图URL")]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "图片名称")]
    public string? ImageName { get; set; }

    /// <summary>
    /// 图片大小（字节）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "图片大小")]
    public int? ImageSize { get; set; }

    /// <summary>
    /// 图片类型（jpg/png/webp等）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "图片类型")]
    public string? ImageType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否主图
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否主图")]
    public bool IsMain { get; set; } = false;
}
```

- [ ] **Step 2: 验证编译并提交**

```bash
cd EasyProduct.WebApi && dotnet build
git add .
git commit -m "feat(api): 创建商品图集实体类

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 创建商品图片 DTO

**Files:**
- Create: `EasyProduct.Models\Dto\Product\Image\ImageQueryDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Image\CreateImageDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Image\UpdateImageDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Image\ImageDto.cs`

- [ ] **Step 1: 创建查询参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Image\ImageQueryDto.cs`：

```csharp
namespace EasyProduct.Models.Dto.Product.Image;

/// <summary>
/// 商品图片查询参数
/// </summary>
public class ImageQueryDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public string? SpuId { get; set; }

    /// <summary>
    /// 是否主图
    /// </summary>
    public bool? IsMain { get; set; }
}
```

- [ ] **Step 2: 创建创建参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Image\CreateImageDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Image;

/// <summary>
/// 创建商品图片参数
/// </summary>
public class CreateImageDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    [Required(ErrorMessage = "商品ID不能为空")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    [Required(ErrorMessage = "图片URL不能为空")]
    [MaxLength(500, ErrorMessage = "图片URL不能超过500个字符")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    [MaxLength(200)]
    public string? ImageName { get; set; }

    /// <summary>
    /// 图片大小（字节）
    /// </summary>
    public int? ImageSize { get; set; }

    /// <summary>
    /// 图片类型
    /// </summary>
    [MaxLength(50)]
    public string? ImageType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否主图
    /// </summary>
    public bool IsMain { get; set; } = false;
}
```

- [ ] **Step 3: 创建更新参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Image\UpdateImageDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Image;

/// <summary>
/// 更新商品图片参数
/// </summary>
public class UpdateImageDto
{
    /// <summary>
    /// 图片ID
    /// </summary>
    [Required(ErrorMessage = "图片ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    [Required(ErrorMessage = "图片URL不能为空")]
    [MaxLength(500, ErrorMessage = "图片URL不能超过500个字符")]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    [MaxLength(200)]
    public string? ImageName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 是否主图
    /// </summary>
    public bool IsMain { get; set; }
}
```

- [ ] **Step 4: 创建数据传输对象 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Image\ImageDto.cs`：

```csharp
namespace EasyProduct.Models.Dto.Product.Image;

/// <summary>
/// 商品图片DTO
/// </summary>
public class ImageDto
{
    /// <summary>
    /// 图片ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID
    /// </summary>
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 图片URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 图片名称
    /// </summary>
    public string? ImageName { get; set; }

    /// <summary>
    /// 图片大小（字节）
    /// </summary>
    public int? ImageSize { get; set; }

    /// <summary>
    /// 图片类型
    /// </summary>
    public string? ImageType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否主图
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }
}
```

- [ ] **Step 5: 验证编译**

```bash
cd EasyProduct.WebApi && dotnet build 2>&1 | grep -E "(成功|失败|error)"
```

Expected: 已成功生成

- [ ] **Step 6: 提交**

```bash
git add EasyProduct.Models/Dto/Product/Image/
git commit -m "feat(api): 创建商品图片 DTO

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 4: 创建商品图片服务

**Files:**
- Create: `EasyProduct.Business\Product\IImageService.cs`
- Create: `EasyProduct.Business\Product\ImageService.cs`

- [ ] **Step 1: 创建服务接口**

创建文件 `EasyProduct.Business\Product\IImageService.cs`：

```csharp
using EasyProduct.Models.Dto.Product.Image;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品图片服务接口
/// </summary>
/// <remarks>
/// 提供商品图片的上传、管理、排序、删除等功能
/// </remarks>
public interface IImageService
{
    /// <summary>
    /// 获取图片列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>图片列表</returns>
    Task<List<ImageDto>> GetImageListAsync(ImageQueryDto query);

    /// <summary>
    /// 获取图片详情
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>图片详情</returns>
    Task<ImageDto> GetImageByIdAsync(string id);

    /// <summary>
    /// 创建图片
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新图片ID</returns>
    Task<string> CreateImageAsync(CreateImageDto dto);

    /// <summary>
    /// 批量创建图片
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="images">图片列表</param>
    /// <returns>成功创建的数量</returns>
    Task<int> BatchCreateImagesAsync(string spuId, List<CreateImageDto> images);

    /// <summary>
    /// 更新图片
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateImageAsync(UpdateImageDto dto);

    /// <summary>
    /// 删除图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteImageAsync(string id);

    /// <summary>
    /// 批量删除图片
    /// </summary>
    /// <param name="ids">图片ID列表</param>
    /// <returns>成功删除的数量</returns>
    Task<int> BatchDeleteImagesAsync(List<string> ids);

    /// <summary>
    /// 设置主图
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    Task<bool> SetMainImageAsync(string id);

    /// <summary>
    /// 更新图片排序
    /// </summary>
    /// <param name="imageId">图片ID</param>
    /// <param name="sort">排序值</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateImageSortAsync(string imageId, int sort);
}
```

- [ ] **Step 2: 创建服务实现（第一部分：构造函数和查询方法）**

创建文件 `EasyProduct.Business\Product\ImageService.cs`：

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Image;
using EasyProduct.Models.Entitys.Product;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品图片服务实现
/// </summary>
/// <remarks>
/// 提供商品图片的上传、管理、排序、删除等功能
/// </remarks>
public class ImageService : BaseService, IImageService
{
    private readonly ILogger<ImageService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public ImageService(ILogger<ImageService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取图片列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>图片列表</returns>
    public async Task<List<ImageDto>> GetImageListAsync(ImageQueryDto query)
    {
        var queryable = _db.Queryable<product_image>()
            .Where(x => x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.SpuId))
        {
            queryable = queryable.Where(x => x.SpuId == query.SpuId);
        }

        if (query.IsMain.HasValue)
        {
            queryable = queryable.Where(x => x.IsMain == query.IsMain.Value);
        }

        var images = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        return images.Adapt<List<ImageDto>>();
    }

    /// <summary>
    /// 获取图片详情
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>图片详情</returns>
    public async Task<ImageDto> GetImageByIdAsync(string id)
    {
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        return image.Adapt<ImageDto>();
    }
```

- [ ] **Step 3: 创建服务实现（第二部分：创建方法）**

在 `ImageService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 创建图片
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新图片ID</returns>
    public async Task<string> CreateImageAsync(CreateImageDto dto)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 如果设置为主图，先清除其他主图
        if (dto.IsMain)
        {
            await ClearMainImage(dto.SpuId);
        }

        // 创建图片实体
        var image = dto.Adapt<product_image>();
        image.Id = Guid.NewGuid();
        image.CreatedAt = DateTime.UtcNow;

        // 插入数据库
        await _db.Insertable(image).ExecuteCommandAsync();

        _logger.LogInformation("创建商品图片成功：商品ID: {SpuId}, 图片: {ImageUrl}", dto.SpuId, dto.ImageUrl);

        return image.Id.ToString();
    }

    /// <summary>
    /// 批量创建图片
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="images">图片列表</param>
    /// <returns>成功创建的数量</returns>
    public async Task<int> BatchCreateImagesAsync(string spuId, List<CreateImageDto> images)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == spuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        var imageEntities = new List<product_image>();
        for (int i = 0; i < images.Count; i++)
        {
            var image = images[i].Adapt<product_image>();
            image.Id = Guid.NewGuid();
            image.SpuId = spuId;
            image.Sort = i;
            image.CreatedAt = DateTime.UtcNow;

            // 如果设置为主图，先清除其他主图
            if (image.IsMain)
            {
                await ClearMainImage(spuId);
            }

            imageEntities.Add(image);
        }

        // 批量插入
        var count = await _db.Insertable(imageEntities).ExecuteCommandAsync();

        _logger.LogInformation("批量创建商品图片成功：商品ID: {SpuId}, 数量: {Count}", spuId, count);

        return count;
    }
```

- [ ] **Step 4: 创建服务实现（第三部分：更新和删除方法）**

在 `ImageService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 更新图片
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateImageAsync(UpdateImageDto dto)
    {
        // 检查图片是否存在
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        // 如果设置为主图，先清除其他主图
        if (dto.IsMain)
        {
            await ClearMainImage(image.SpuId);
        }

        // 更新图片信息
        image.ImageUrl = dto.ImageUrl;
        image.ThumbnailUrl = dto.ThumbnailUrl;
        image.ImageName = dto.ImageName;
        image.Sort = dto.Sort;
        image.IsMain = dto.IsMain;
        image.UpdatedAt = DateTime.UtcNow;

        // 更新数据库
        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("更新商品图片成功：ID: {Id}", dto.Id);

        return true;
    }

    /// <summary>
    /// 删除图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteImageAsync(string id)
    {
        // 检查图片是否存在
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        // 软删除
        image.IsDeleted = 1;
        image.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("删除商品图片成功：ID: {Id}", id);

        return true;
    }

    /// <summary>
    /// 批量删除图片
    /// </summary>
    /// <param name="ids">图片ID列表</param>
    /// <returns>成功删除的数量</returns>
    public async Task<int> BatchDeleteImagesAsync(List<string> ids)
    {
        var images = await _db.Queryable<product_image>()
            .Where(x => ids.Contains(x.Id.ToString()) && x.IsDeleted == 0)
            .ToListAsync();

        if (images.Count == 0)
        {
            return 0;
        }

        // 批量软删除
        foreach (var image in images)
        {
            image.IsDeleted = 1;
            image.UpdatedAt = DateTime.UtcNow;
        }

        var count = await _db.Updateable(images).ExecuteCommandAsync();

        _logger.LogInformation("批量删除商品图片成功：数量: {Count}", count);

        return count;
    }
```

- [ ] **Step 5: 创建服务实现（第四部分：辅助方法）**

在 `ImageService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 设置主图
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> SetMainImageAsync(string id)
    {
        // 检查图片是否存在
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        // 清除该商品的其他主图
        await ClearMainImage(image.SpuId);

        // 设置为主图
        image.IsMain = true;
        image.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("设置主图成功：商品ID: {SpuId}, 图片ID: {Id}", image.SpuId, id);

        return true;
    }

    /// <summary>
    /// 更新图片排序
    /// </summary>
    /// <param name="imageId">图片ID</param>
    /// <param name="sort">排序值</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateImageSortAsync(string imageId, int sort)
    {
        var image = await _db.Queryable<product_image>()
            .Where(x => x.Id.ToString() == imageId && x.IsDeleted == 0)
            .FirstAsync();

        if (image == null)
        {
            throw new BusinessException("图片不存在", 404);
        }

        image.Sort = sort;
        image.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(image).ExecuteCommandAsync();

        _logger.LogInformation("更新图片排序成功：图片ID: {Id}, 排序: {Sort}", imageId, sort);

        return true;
    }

    /// <summary>
    /// 清除主图标记
    /// </summary>
    /// <param name="spuId">商品ID</param>
    private async Task ClearMainImage(string spuId)
    {
        await _db.Updateable<product_image>()
            .SetColumns(x => x.IsMain == false)
            .SetColumns(x => x.UpdatedAt == DateTime.UtcNow)
            .Where(x => x.SpuId == spuId && x.IsDeleted == 0)
            .ExecuteCommandAsync();
    }
}
```

- [ ] **Step 6: 验证编译**

```bash
cd EasyProduct.WebApi && dotnet build 2>&1 | grep -E "(成功|失败|error)"
```

Expected: 已成功生成

- [ ] **Step 7: 提交**

```bash
git add EasyProduct.Business/Product/IImageService.cs EasyProduct.Business/Product/ImageService.cs
git commit -m "feat(api): 实现商品图片服务

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 5: 创建商品图片控制器

**Files:**
- Create: `EasyProduct.Web\Controllers\Admin\Product\ImageController.cs`

- [ ] **Step 1: 创建图片控制器**

创建文件 `EasyProduct.Web\Controllers\Admin\Product\ImageController.cs`：

```csharp
using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 商品图片管理控制器
/// </summary>
/// <remarks>
/// 提供商品图片的上传、管理、排序、删除等功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/product/image")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class ImageController : BaseController
{
    private readonly IImageService _imageService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="imageService">图片服务</param>
    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// 获取图片列表
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="isMain">是否主图</param>
    /// <returns>图片列表</returns>
    /// <remarks>
    /// 获取指定商品的图片列表
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<ImageDto>>> GetImageList(
        [FromQuery] string? spuId,
        [FromQuery] bool? isMain)
    {
        var query = new ImageQueryDto
        {
            SpuId = spuId,
            IsMain = isMain
        };

        var result = await _imageService.GetImageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取图片详情
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>图片详情</returns>
    /// <remarks>
    /// 根据ID获取图片的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ImageDto>> GetImageById(string id)
    {
        var result = await _imageService.GetImageByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建图片
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新图片ID</returns>
    /// <remarks>
    /// 创建新的商品图片
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateImage([FromBody] CreateImageDto dto)
    {
        var result = await _imageService.CreateImageAsync(dto);
        return Success(result, "图片创建成功");
    }

    /// <summary>
    /// 批量创建图片
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="images">图片列表</param>
    /// <returns>成功创建的数量</returns>
    /// <remarks>
    /// 批量创建商品图片
    /// </remarks>
    [HttpPost("batch/{spuId}")]
    public async Task<ApiResponse<int>> BatchCreateImages(string spuId, [FromBody] List<CreateImageDto> images)
    {
        var result = await _imageService.BatchCreateImagesAsync(spuId, images);
        return Success(result, $"成功创建 {result} 张图片");
    }

    /// <summary>
    /// 更新图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新图片信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateImage(string id, [FromBody] UpdateImageDto dto)
    {
        dto.Id = id;
        var result = await _imageService.UpdateImageAsync(dto);
        return Success(result, "图片更新成功");
    }

    /// <summary>
    /// 删除图片
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除图片（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteImage(string id)
    {
        var result = await _imageService.DeleteImageAsync(id);
        return Success(result, "图片删除成功");
    }

    /// <summary>
    /// 批量删除图片
    /// </summary>
    /// <param name="ids">图片ID列表</param>
    /// <returns>成功删除的数量</returns>
    /// <remarks>
    /// 批量删除图片（软删除）
    /// </remarks>
    [HttpDelete("batch")]
    public async Task<ApiResponse<int>> BatchDeleteImages([FromBody] List<string> ids)
    {
        var result = await _imageService.BatchDeleteImagesAsync(ids);
        return Success(result, $"成功删除 {result} 张图片");
    }

    /// <summary>
    /// 设置主图
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 将指定图片设置为主图
    /// </remarks>
    [HttpPut("{id}/main")]
    public async Task<ApiResponse<bool>> SetMainImage(string id)
    {
        var result = await _imageService.SetMainImageAsync(id);
        return Success(result, "主图设置成功");
    }

    /// <summary>
    /// 更新图片排序
    /// </summary>
    /// <param name="id">图片ID</param>
    /// <param name="sort">排序值</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新图片的排序值
    /// </remarks>
    [HttpPut("{id}/sort")]
    public async Task<ApiResponse<bool>> UpdateImageSort(string id, [FromQuery] int sort)
    {
        var result = await _imageService.UpdateImageSortAsync(id, sort);
        return Success(result, "图片排序更新成功");
    }
}
```

- [ ] **Step 2: 验证编译**

```bash
cd EasyProduct.WebApi && dotnet build 2>&1 | grep -E "(成功|失败|error)"
```

Expected: 已成功生成

- [ ] **Step 3: 提交**

```bash
git add EasyProduct.Web/Controllers/Admin/Product/ImageController.cs
git commit -m "feat(api): 实现商品图片控制器

- 创建 ImageController
- 提供图片列表查询、详情查询
- 提供图片 CRUD 接口
- 支持批量创建、批量删除
- 支持设置主图
- 支持更新图片排序
- 添加完整的中文注释

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## 验收标准

- ✅ 商品图片表已创建
- ✅ 商品图片实体、DTO、服务、控制器已实现
- ✅ 支持图片列表查询、详情查询
- ✅ 支持图片创建、更新、删除
- ✅ 支持批量创建、批量删除
- ✅ 支持设置主图
- ✅ 支持更新图片排序
- ✅ 项目构建成功（0 编译错误）
- ✅ 所有方法添加完整中文注释

---

## API 列表

- `GET /api/admin/product/image/list` - 获取图片列表
- `GET /api/admin/product/image/{id}` - 获取图片详情
- `POST /api/admin/product/image` - 创建图片
- `POST /api/admin/product/image/batch/{spuId}` - 批量创建图片
- `PUT /api/admin/product/image/{id}` - 更新图片
- `DELETE /api/admin/product/image/{id}` - 删除图片
- `DELETE /api/admin/product/image/batch` - 批量删除图片
- `PUT /api/admin/product/image/{id}/main` - 设置主图
- `PUT /api/admin/product/image/{id}/sort` - 更新图片排序