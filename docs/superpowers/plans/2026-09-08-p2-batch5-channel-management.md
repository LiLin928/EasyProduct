# P2 批次 5：渠道发布管理实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现渠道发布管理功能，支持商品在官网、小程序、B2B 三端的上架状态管理。

**Architecture:** 基于批次 1-4 已完成的分类、SPU、SKU、图片，实现渠道发布管理。支持多渠道发布、上架状态控制、排序管理。

**Tech Stack:** .NET 8.0 + SqlSugar 5.1.4 + Mapster 10.x

---

## 模块概述

**渠道发布（product_channel）**
- 商品多渠道发布管理
- 关联 SPU（product_spu）
- 支持三个渠道：官网（site）、小程序（miniapp）、B2B（b2b）
- 支持上架状态控制
- 支持渠道排序
- 支持渠道价格设置

---

## 任务划分

| 任务 | 内容 | 预计工时 |
|------|------|---------|
| **Task 1** | 更新数据库添加渠道表 | 0.5 天 |
| **Task 2** | 创建渠道实体和枚举 | 0.5 天 |
| **Task 3** | 创建渠道 DTO | 0.5 天 |
| **Task 4** | 创建渠道服务 | 1.5 天 |
| **Task 5** | 创建渠道控制器 | 1 天 |

---

## Task 1: 更新数据库添加渠道发布表

**Files:**
- Modify: `sql/init-database.sql`

- [ ] **Step 1: 添加渠道发布表定义**

在 `sql/init-database.sql` 文件的 Product 模块部分添加：

```sql
-- ----------------------------
-- 商品渠道发布表 (product_channel)
-- ----------------------------
DROP TABLE IF EXISTS `product_channel`;
CREATE TABLE `product_channel` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `spu_id` CHAR(36) NOT NULL COMMENT '商品ID',
  `channel_code` VARCHAR(20) NOT NULL COMMENT '渠道编码：site=官网，miniapp=小程序，b2b=B2B',
  `status` INT DEFAULT 1 COMMENT '上架状态：0=下架，1=上架',
  `sort` INT DEFAULT 0 COMMENT '渠道排序',
  `price` DECIMAL(18,2) DEFAULT NULL COMMENT '渠道价格（可选，为空则使用SKU价格）',
  `show_price` TINYINT(1) DEFAULT 1 COMMENT '是否显示价格：0=否，1=是',
  `show_stock` TINYINT(1) DEFAULT 1 COMMENT '是否显示库存：0=否，1=是',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `unpublish_time` DATETIME DEFAULT NULL COMMENT '下架时间',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_spu_channel` (`spu_id`, `channel_code`),
  KEY `idx_channel_code` (`channel_code`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品渠道发布表';
```

- [ ] **Step 2: 提交**

```bash
git add sql/init-database.sql
git commit -m "feat(db): 添加商品渠道发布表定义

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建渠道实体和枚举

**Files:**
- Create: `EasyProduct.Models\Enums\Product\ChannelType.cs`
- Create: `EasyProduct.Models\Entitys\Product\product_channel.cs`

- [ ] **Step 1: 创建渠道类型枚举**

创建文件 `EasyProduct.Models\Enums\Product\ChannelType.cs`：

```csharp
namespace EasyProduct.Models.Enums.Product;

/// <summary>
/// 渠道类型枚举
/// </summary>
public enum ChannelType
{
    /// <summary>
    /// 官网
    /// </summary>
    Site = 1,

    /// <summary>
    /// 小程序
    /// </summary>
    MiniApp = 2,

    /// <summary>
    /// B2B
    /// </summary>
    B2B = 3
}

/// <summary>
/// 渠道编码常量
/// </summary>
public static class ChannelCode
{
    /// <summary>
    /// 官网渠道编码
    /// </summary>
    public const string Site = "site";

    /// <summary>
    /// 小程序渠道编码
    /// </summary>
    public const string MiniApp = "miniapp";

    /// <summary>
    /// B2B渠道编码
    /// </summary>
    public const string B2B = "b2b";

    /// <summary>
    /// 所有渠道编码列表
    /// </summary>
    public static readonly string[] All = { Site, MiniApp, B2B };
}
```

- [ ] **Step 2: 创建渠道实体类**

创建文件 `EasyProduct.Models\Entitys\Product\product_channel.cs`：

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品渠道发布实体
/// </summary>
[SugarTable("product_channel", "商品渠道发布表")]
public class product_channel : BaseEntity
{
    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "商品ID")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 渠道编码
    /// </summary>
    /// <remarks>
    /// site=官网，miniapp=小程序，b2b=B2B
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "渠道编码")]
    public string ChannelCode { get; set; } = string.Empty;

    /// <summary>
    /// 上架状态
    /// </summary>
    /// <remarks>
    /// 0=下架，1=上架
    /// </remarks>
    [SugarColumn(ColumnDescription = "上架状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 渠道排序
    /// </summary>
    [SugarColumn(ColumnDescription = "渠道排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 渠道价格（可选）
    /// </summary>
    /// <remarks>
    /// 为空则使用SKU价格
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = true, ColumnDescription = "渠道价格")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    [SugarColumn(ColumnDescription = "是否显示价格")]
    public bool ShowPrice { get; set; } = true;

    /// <summary>
    /// 是否显示库存
    /// </summary>
    [SugarColumn(ColumnDescription = "是否显示库存")]
    public bool ShowStock { get; set; } = true;

    /// <summary>
    /// 发布时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "发布时间")]
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 下架时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "下架时间")]
    public DateTime? UnpublishTime { get; set; }
}
```

- [ ] **Step 3: 验证编译并提交**

```bash
cd EasyProduct.WebApi && dotnet build
git add .
git commit -m "feat(api): 创建渠道发布实体和枚举

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 创建渠道 DTO

**Files:**
- Create: `EasyProduct.Models\Dto\Product\Channel\ChannelQueryDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Channel\CreateChannelDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Channel\UpdateChannelDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Channel\ChannelDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Channel\BatchPublishDto.cs`

- [ ] **Step 1: 创建查询参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Channel\ChannelQueryDto.cs`：

```csharp
namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 渠道发布查询参数
/// </summary>
public class ChannelQueryDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public string? SpuId { get; set; }

    /// <summary>
    /// 渠道编码
    /// </summary>
    public string? ChannelCode { get; set; }

    /// <summary>
    /// 上架状态
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 2: 创建创建参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Channel\CreateChannelDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 创建渠道发布参数
/// </summary>
public class CreateChannelDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    [Required(ErrorMessage = "商品ID不能为空")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 渠道编码
    /// </summary>
    [Required(ErrorMessage = "渠道编码不能为空")]
    [MaxLength(20, ErrorMessage = "渠道编码不能超过20个字符")]
    public string ChannelCode { get; set; } = string.Empty;

    /// <summary>
    /// 上架状态
    /// </summary>
    [Range(0, 1, ErrorMessage = "上架状态只能是0或1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 渠道排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 渠道价格（可选）
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "价格必须大于等于0")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    public bool ShowPrice { get; set; } = true;

    /// <summary>
    /// 是否显示库存
    /// </summary>
    public bool ShowStock { get; set; } = true;

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 下架时间
    /// </summary>
    public DateTime? UnpublishTime { get; set; }
}
```

- [ ] **Step 3: 创建更新参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Channel\UpdateChannelDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 更新渠道发布参数
/// </summary>
public class UpdateChannelDto
{
    /// <summary>
    /// 渠道发布ID
    /// </summary>
    [Required(ErrorMessage = "渠道发布ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 上架状态
    /// </summary>
    [Range(0, 1, ErrorMessage = "上架状态只能是0或1")]
    public int Status { get; set; }

    /// <summary>
    /// 渠道排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 渠道价格（可选）
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "价格必须大于等于0")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    public bool ShowPrice { get; set; }

    /// <summary>
    /// 是否显示库存
    /// </summary>
    public bool ShowStock { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 下架时间
    /// </summary>
    public DateTime? UnpublishTime { get; set; }
}
```

- [ ] **Step 4: 创建数据传输对象 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Channel\ChannelDto.cs`：

```csharp
namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 渠道发布DTO
/// </summary>
public class ChannelDto
{
    /// <summary>
    /// 渠道发布ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID
    /// </summary>
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string? SpuName { get; set; }

    /// <summary>
    /// 渠道编码
    /// </summary>
    public string ChannelCode { get; set; } = string.Empty;

    /// <summary>
    /// 渠道名称
    /// </summary>
    public string? ChannelName { get; set; }

    /// <summary>
    /// 上架状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 渠道排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 渠道价格
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    public bool ShowPrice { get; set; }

    /// <summary>
    /// 是否显示库存
    /// </summary>
    public bool ShowStock { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 下架时间
    /// </summary>
    public DateTime? UnpublishTime { get; set; }

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

- [ ] **Step 5: 创建批量发布参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Channel\BatchPublishDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Channel;

/// <summary>
/// 批量发布参数
/// </summary>
public class BatchPublishDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    [Required(ErrorMessage = "商品ID不能为空")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 渠道编码列表
    /// </summary>
    [Required(ErrorMessage = "渠道编码列表不能为空")]
    public List<string> ChannelCodes { get; set; } = new();

    /// <summary>
    /// 上架状态
    /// </summary>
    [Range(0, 1, ErrorMessage = "上架状态只能是0或1")]
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 6: 验证编译**

```bash
cd EasyProduct.WebApi && dotnet build 2>&1 | grep -E "(成功|失败|error)"
```

Expected: 已成功生成

- [ ] **Step 7: 提交**

```bash
git add EasyProduct.Models/Dto/Product/Channel/
git commit -m "feat(api): 创建渠道发布 DTO

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 4: 创建渠道发布服务

**Files:**
- Create: `EasyProduct.Business\Product\IChannelService.cs`
- Create: `EasyProduct.Business\Product\ChannelService.cs`

- [ ] **Step 1: 创建服务接口**

创建文件 `EasyProduct.Business\Product\IChannelService.cs`：

```csharp
using EasyProduct.Models.Dto.Product.Channel;

namespace EasyProduct.Business.Product;

/// <summary>
/// 渠道发布服务接口
/// </summary>
/// <remarks>
/// 提供商品渠道发布的创建、管理、上架状态控制等功能
/// </remarks>
public interface IChannelService
{
    /// <summary>
    /// 获取渠道发布列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>渠道发布列表</returns>
    Task<List<ChannelDto>> GetChannelListAsync(ChannelQueryDto query);

    /// <summary>
    /// 获取渠道发布详情
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>渠道发布详情</returns>
    Task<ChannelDto> GetChannelByIdAsync(string id);

    /// <summary>
    /// 获取商品的渠道发布情况
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <returns>渠道发布列表</returns>
    Task<List<ChannelDto>> GetSpuChannelsAsync(string spuId);

    /// <summary>
    /// 创建渠道发布
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新渠道发布ID</returns>
    Task<string> CreateChannelAsync(CreateChannelDto dto);

    /// <summary>
    /// 批量发布到渠道
    /// </summary>
    /// <param name="dto">批量发布参数</param>
    /// <returns>成功发布的数量</returns>
    Task<int> BatchPublishAsync(BatchPublishDto dto);

    /// <summary>
    /// 更新渠道发布
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateChannelAsync(UpdateChannelDto dto);

    /// <summary>
    /// 删除渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteChannelAsync(string id);

    /// <summary>
    /// 更新上架状态
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="status">上架状态</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateChannelStatusAsync(string id, int status);

    /// <summary>
    /// 批量更新上架状态
    /// </summary>
    /// <param name="ids">渠道发布ID列表</param>
    /// <param name="status">上架状态</param>
    /// <returns>成功更新的数量</returns>
    Task<int> BatchUpdateStatusAsync(List<string> ids, int status);
}
```

- [ ] **Step 2: 创建服务实现（第一部分：构造函数和查询方法）**

创建文件 `EasyProduct.Business\Product\ChannelService.cs`：

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Channel;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Enums.Product;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 渠道发布服务实现
/// </summary>
/// <remarks>
/// 提供商品渠道发布的创建、管理、上架状态控制等功能
/// </remarks>
public class ChannelService : BaseService, IChannelService
{
    private readonly ILogger<ChannelService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public ChannelService(ILogger<ChannelService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取渠道发布列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>渠道发布列表</returns>
    public async Task<List<ChannelDto>> GetChannelListAsync(ChannelQueryDto query)
    {
        var queryable = _db.Queryable<product_channel>()
            .Where(x => x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.SpuId))
        {
            queryable = queryable.Where(x => x.SpuId == query.SpuId);
        }

        if (!string.IsNullOrEmpty(query.ChannelCode))
        {
            queryable = queryable.Where(x => x.ChannelCode == query.ChannelCode);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        var channels = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        var channelDtos = channels.Adapt<List<ChannelDto>>();

        // 关联商品名称
        foreach (var dto in channelDtos)
        {
            var spu = await _db.Queryable<product_spu>()
                .Where(x => x.Id.ToString() == dto.SpuId)
                .FirstAsync();
            dto.SpuName = spu?.SpuName;

            // 设置渠道名称
            dto.ChannelName = GetChannelName(dto.ChannelCode);
        }

        return channelDtos;
    }

    /// <summary>
    /// 获取渠道发布详情
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>渠道发布详情</returns>
    public async Task<ChannelDto> GetChannelByIdAsync(string id)
    {
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        var dto = channel.Adapt<ChannelDto>();

        // 关联商品名称
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId)
            .FirstAsync();
        dto.SpuName = spu?.SpuName;

        // 设置渠道名称
        dto.ChannelName = GetChannelName(dto.ChannelCode);

        return dto;
    }

    /// <summary>
    /// 获取商品的渠道发布情况
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <returns>渠道发布列表</returns>
    public async Task<List<ChannelDto>> GetSpuChannelsAsync(string spuId)
    {
        var channels = await _db.Queryable<product_channel>()
            .Where(x => x.SpuId == spuId && x.IsDeleted == 0)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        var channelDtos = channels.Adapt<List<ChannelDto>>();

        // 设置渠道名称
        foreach (var dto in channelDtos)
        {
            dto.ChannelName = GetChannelName(dto.ChannelCode);
        }

        return channelDtos;
    }
```

- [ ] **Step 3: 创建服务实现（第二部分：创建方法）**

在 `ChannelService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 创建渠道发布
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新渠道发布ID</returns>
    public async Task<string> CreateChannelAsync(CreateChannelDto dto)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 验证渠道编码是否有效
        if (!ChannelCode.All.Contains(dto.ChannelCode))
        {
            throw new BusinessException($"无效的渠道编码：{dto.ChannelCode}");
        }

        // 检查是否已发布到该渠道
        var exists = await _db.Queryable<product_channel>()
            .Where(x => x.SpuId == dto.SpuId && x.ChannelCode == dto.ChannelCode && x.IsDeleted == 0)
            .AnyAsync();

        if (exists)
        {
            throw new BusinessException($"该商品已发布到渠道：{dto.ChannelCode}");
        }

        // 创建渠道发布实体
        var channel = dto.Adapt<product_channel>();
        channel.Id = Guid.NewGuid();
        channel.CreatedAt = DateTime.UtcNow;

        // 如果上架，设置发布时间
        if (channel.Status == 1 && !channel.PublishTime.HasValue)
        {
            channel.PublishTime = DateTime.UtcNow;
        }

        // 插入数据库
        await _db.Insertable(channel).ExecuteCommandAsync();

        _logger.LogInformation("创建渠道发布成功：商品ID: {SpuId}, 渠道: {ChannelCode}", dto.SpuId, dto.ChannelCode);

        return channel.Id.ToString();
    }

    /// <summary>
    /// 批量发布到渠道
    /// </summary>
    /// <param name="dto">批量发布参数</param>
    /// <returns>成功发布的数量</returns>
    public async Task<int> BatchPublishAsync(BatchPublishDto dto)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        var channelEntities = new List<product_channel>();

        foreach (var channelCode in dto.ChannelCodes)
        {
            // 验证渠道编码是否有效
            if (!ChannelCode.All.Contains(channelCode))
            {
                _logger.LogWarning("跳过无效的渠道编码：{ChannelCode}", channelCode);
                continue;
            }

            // 检查是否已发布到该渠道
            var exists = await _db.Queryable<product_channel>()
                .Where(x => x.SpuId == dto.SpuId && x.ChannelCode == channelCode && x.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                _logger.LogWarning("商品已发布到渠道，跳过：{ChannelCode}", channelCode);
                continue;
            }

            // 创建渠道发布实体
            var channel = new product_channel
            {
                Id = Guid.NewGuid(),
                SpuId = dto.SpuId,
                ChannelCode = channelCode,
                Status = dto.Status,
                Sort = channelEntities.Count,
                CreatedAt = DateTime.UtcNow
            };

            // 如果上架，设置发布时间
            if (channel.Status == 1)
            {
                channel.PublishTime = DateTime.UtcNow;
            }

            channelEntities.Add(channel);
        }

        if (channelEntities.Count == 0)
        {
            return 0;
        }

        // 批量插入
        var count = await _db.Insertable(channelEntities).ExecuteCommandAsync();

        _logger.LogInformation("批量发布到渠道成功：商品ID: {SpuId}, 数量: {Count}", dto.SpuId, count);

        return count;
    }
```

- [ ] **Step 4: 创建服务实现（第三部分：更新和删除方法）**

在 `ChannelService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 更新渠道发布
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateChannelAsync(UpdateChannelDto dto)
    {
        // 检查渠道发布是否存在
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        // 更新渠道发布信息
        channel.Status = dto.Status;
        channel.Sort = dto.Sort;
        channel.Price = dto.Price;
        channel.ShowPrice = dto.ShowPrice;
        channel.ShowStock = dto.ShowStock;
        channel.PublishTime = dto.PublishTime;
        channel.UnpublishTime = dto.UnpublishTime;
        channel.UpdatedAt = DateTime.UtcNow;

        // 如果从未上架变为上架，设置发布时间
        if (channel.Status == 1 && !channel.PublishTime.HasValue)
        {
            channel.PublishTime = DateTime.UtcNow;
        }

        // 如果从上架变为下架，设置下架时间
        if (channel.Status == 0 && !channel.UnpublishTime.HasValue)
        {
            channel.UnpublishTime = DateTime.UtcNow;
        }

        // 更新数据库
        await _db.Updateable(channel).ExecuteCommandAsync();

        _logger.LogInformation("更新渠道发布成功：ID: {Id}", dto.Id);

        return true;
    }

    /// <summary>
    /// 删除渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteChannelAsync(string id)
    {
        // 检查渠道发布是否存在
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        // 软删除
        channel.IsDeleted = 1;
        channel.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(channel).ExecuteCommandAsync();

        _logger.LogInformation("删除渠道发布成功：ID: {Id}", id);

        return true;
    }

    /// <summary>
    /// 更新上架状态
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="status">上架状态</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateChannelStatusAsync(string id, int status)
    {
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        channel.Status = status;
        channel.UpdatedAt = DateTime.UtcNow;

        // 如果上架，设置发布时间
        if (status == 1 && !channel.PublishTime.HasValue)
        {
            channel.PublishTime = DateTime.UtcNow;
        }

        // 如果下架，设置下架时间
        if (status == 0 && !channel.UnpublishTime.HasValue)
        {
            channel.UnpublishTime = DateTime.UtcNow;
        }

        await _db.Updateable(channel).ExecuteCommandAsync();

        _logger.LogInformation("更新渠道发布状态成功：ID: {Id}, 状态: {Status}", id, status);

        return true;
    }

    /// <summary>
    /// 批量更新上架状态
    /// </summary>
    /// <param name="ids">渠道发布ID列表</param>
    /// <param name="status">上架状态</param>
    /// <returns>成功更新的数量</returns>
    public async Task<int> BatchUpdateStatusAsync(List<string> ids, int status)
    {
        var channels = await _db.Queryable<product_channel>()
            .Where(x => ids.Contains(x.Id.ToString()) && x.IsDeleted == 0)
            .ToListAsync();

        if (channels.Count == 0)
        {
            return 0;
        }

        var now = DateTime.UtcNow;

        // 批量更新状态
        foreach (var channel in channels)
        {
            channel.Status = status;
            channel.UpdatedAt = now;

            // 如果上架，设置发布时间
            if (status == 1 && !channel.PublishTime.HasValue)
            {
                channel.PublishTime = now;
            }

            // 如果下架，设置下架时间
            if (status == 0 && !channel.UnpublishTime.HasValue)
            {
                channel.UnpublishTime = now;
            }
        }

        var count = await _db.Updateable(channels).ExecuteCommandAsync();

        _logger.LogInformation("批量更新渠道发布状态成功：数量: {Count}, 状态: {Status}", count, status);

        return count;
    }
```

- [ ] **Step 5: 创建服务实现（第四部分：辅助方法）**

在 `ChannelService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 获取渠道名称
    /// </summary>
    /// <param name="channelCode">渠道编码</param>
    /// <returns>渠道名称</returns>
    private string GetChannelName(string channelCode)
    {
        return channelCode switch
        {
            ChannelCode.Site => "官网",
            ChannelCode.MiniApp => "小程序",
            ChannelCode.B2B => "B2B",
            _ => "未知渠道"
        };
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
git add EasyProduct.Business/Product/IChannelService.cs EasyProduct.Business/Product/ChannelService.cs
git commit -m "feat(api): 实现渠道发布服务

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 5: 创建渠道发布控制器

**Files:**
- Create: `EasyProduct.Web\Controllers\Admin\Product\ChannelController.cs`

- [ ] **Step 1: 创建渠道发布控制器**

创建文件 `EasyProduct.Web\Controllers\Admin\Product\ChannelController.cs`：

```csharp
using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Channel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 渠道发布管理控制器
/// </summary>
/// <remarks>
/// 提供商品渠道发布的创建、管理、上架状态控制等功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/product/channel")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class ChannelController : BaseController
{
    private readonly IChannelService _channelService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="channelService">渠道发布服务</param>
    public ChannelController(IChannelService channelService)
    {
        _channelService = channelService;
    }

    /// <summary>
    /// 获取渠道发布列表
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <param name="channelCode">渠道编码</param>
    /// <param name="status">上架状态</param>
    /// <returns>渠道发布列表</returns>
    /// <remarks>
    /// 获取渠道发布列表，支持按商品、渠道、状态筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<ChannelDto>>> GetChannelList(
        [FromQuery] string? spuId,
        [FromQuery] string? channelCode,
        [FromQuery] int? status)
    {
        var query = new ChannelQueryDto
        {
            SpuId = spuId,
            ChannelCode = channelCode,
            Status = status
        };

        var result = await _channelService.GetChannelListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取渠道发布详情
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>渠道发布详情</returns>
    /// <remarks>
    /// 根据ID获取渠道发布的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<ChannelDto>> GetChannelById(string id)
    {
        var result = await _channelService.GetChannelByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 获取商品的渠道发布情况
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <returns>渠道发布列表</returns>
    /// <remarks>
    /// 获取指定商品在所有渠道的发布情况
    /// </remarks>
    [HttpGet("spu/{spuId}")]
    public async Task<ApiResponse<List<ChannelDto>>> GetSpuChannels(string spuId)
    {
        var result = await _channelService.GetSpuChannelsAsync(spuId);
        return Success(result);
    }

    /// <summary>
    /// 创建渠道发布
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新渠道发布ID</returns>
    /// <remarks>
    /// 创建新的渠道发布
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateChannel([FromBody] CreateChannelDto dto)
    {
        var result = await _channelService.CreateChannelAsync(dto);
        return Success(result, "渠道发布创建成功");
    }

    /// <summary>
    /// 批量发布到渠道
    /// </summary>
    /// <param name="dto">批量发布参数</param>
    /// <returns>成功发布的数量</returns>
    /// <remarks>
    /// 批量将商品发布到多个渠道
    /// </remarks>
    [HttpPost("batch")]
    public async Task<ApiResponse<int>> BatchPublish([FromBody] BatchPublishDto dto)
    {
        var result = await _channelService.BatchPublishAsync(dto);
        return Success(result, $"成功发布到 {result} 个渠道");
    }

    /// <summary>
    /// 更新渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新渠道发布信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateChannel(string id, [FromBody] UpdateChannelDto dto)
    {
        dto.Id = id;
        var result = await _channelService.UpdateChannelAsync(dto);
        return Success(result, "渠道发布更新成功");
    }

    /// <summary>
    /// 删除渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除渠道发布（软删除）
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteChannel(string id)
    {
        var result = await _channelService.DeleteChannelAsync(id);
        return Success(result, "渠道发布删除成功");
    }

    /// <summary>
    /// 更新上架状态
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="status">上架状态：0=下架，1=上架</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 启用或禁用渠道发布
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateChannelStatus(string id, [FromQuery] int status)
    {
        var result = await _channelService.UpdateChannelStatusAsync(id, status);
        return Success(result, "上架状态更新成功");
    }

    /// <summary>
    /// 批量更新上架状态
    /// </summary>
    /// <param name="ids">渠道发布ID列表</param>
    /// <param name="status">上架状态：0=下架，1=上架</param>
    /// <returns>成功更新的数量</returns>
    /// <remarks>
    /// 批量启用或禁用渠道发布
    /// </remarks>
    [HttpPut("batch/status")]
    public async Task<ApiResponse<int>> BatchUpdateStatus([FromBody] List<string> ids, [FromQuery] int status)
    {
        var result = await _channelService.BatchUpdateStatusAsync(ids, status);
        return Success(result, $"成功更新 {result} 个渠道发布状态");
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
git add EasyProduct.Web/Controllers/Admin/Product/ChannelController.cs
git commit -m "feat(api): 实现渠道发布控制器

- 创建 ChannelController
- 提供渠道发布列表查询、详情查询
- 提供渠道发布 CRUD 接口
- 支持批量发布、批量更新状态
- 支持上架状态管理
- 添加完整的中文注释

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## 验收标准

- ✅ 渠道发布表已创建
- ✅ 渠道发布实体、DTO、服务、控制器已实现
- ✅ 支持渠道发布列表查询、详情查询
- ✅ 支持渠道发布创建、更新、删除
- ✅ 支持批量发布、批量更新状态
- ✅ 支持上架状态管理
- ✅ 支持三端渠道：官网、小程序、B2B
- ✅ 项目构建成功（0 编译错误）
- ✅ 所有方法添加完整中文注释

---

## API 列表

- `GET /api/admin/product/channel/list` - 获取渠道发布列表
- `GET /api/admin/product/channel/{id}` - 获取渠道发布详情
- `GET /api/admin/product/channel/spu/{spuId}` - 获取商品的渠道发布情况
- `POST /api/admin/product/channel` - 创建渠道发布
- `POST /api/admin/product/channel/batch` - 批量发布到渠道
- `PUT /api/admin/product/channel/{id}` - 更新渠道发布
- `DELETE /api/admin/product/channel/{id}` - 删除渠道发布
- `PUT /api/admin/product/channel/{id}/status` - 更新上架状态
- `PUT /api/admin/product/channel/batch/status` - 批量更新上架状态