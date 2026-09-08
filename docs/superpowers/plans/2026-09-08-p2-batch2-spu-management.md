# P2 批次 2：商品主档（SPU）管理实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现商品主档（SPU）管理功能，支持商品的创建、编辑、查询、删除等操作，支持富文本详情。

**Architecture:** 基于批次 1 已完成的商品分类模块，实现 SPU 管理。使用 SqlSugar ORM，遵循项目统一的数据访问规范。

**Tech Stack:** .NET 8.0 + SqlSugar 5.1.4 + Mapster 10.x

---

## 模块概述

**SPU（Standard Product Unit）- 标准产品单位**
- 商品主档，包含商品的基本信息
- 关联商品分类（product_category）
- 支持多种商品类型（实物商品、虚拟商品、票品等）
- 包含富文本详情
- 关联 SKU（批次 3）
- 关联商品图集（批次 4）
- 支持渠道发布（批次 5）

---

## 任务划分

本批次分为 5 个任务：

| 任务 | 内容 | 预计工时 |
|------|------|---------|
| **Task 1** | 更新数据库添加 SPU 表 | 0.5 天 |
| **Task 2** | 创建 SPU 实体和枚举 | 0.5 天 |
| **Task 3** | 创建 SPU DTO | 1 天 |
| **Task 4** | 创建 SPU 服务 | 1.5 天 |
| **Task 5** | 创建 SPU 控制器 | 1 天 |

---

## Task 1: 更新数据库添加 SPU 表

**Files:**
- Modify: `sql/init-database.sql`

**Step 1: 添加 SPU 表定义**

在 `sql/init-database.sql` 文件的 Product 模块部分添加：

```sql
-- ----------------------------
-- 商品主档表 (product_spu)
-- ----------------------------
DROP TABLE IF EXISTS `product_spu`;
CREATE TABLE `product_spu` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `spu_name` VARCHAR(200) NOT NULL COMMENT '商品名称',
  `spu_code` VARCHAR(50) DEFAULT NULL COMMENT '商品编码',
  `category_id` CHAR(36) DEFAULT NULL COMMENT '分类ID',
  `main_image` VARCHAR(500) DEFAULT NULL COMMENT '主图URL',
  `images` TEXT COMMENT '商品图集（JSON数组）',
  `description` TEXT COMMENT '商品描述（富文本）',
  `unit` VARCHAR(20) DEFAULT '件' COMMENT '计量单位',
  `spu_type` INT DEFAULT 1 COMMENT '商品类型：1=实物商品，2=虚拟商品，3=票品',
  `brand` VARCHAR(100) DEFAULT NULL COMMENT '品牌',
  `spec_template` TEXT COMMENT '规格模板（JSON）',
  `status` INT DEFAULT 1 COMMENT '状态：0=下架，1=上架',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_spu_code` (`spu_code`),
  KEY `idx_category_id` (`category_id`),
  KEY `idx_spu_type` (`spu_type`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品主档表';
```

**Step 2: 提交**

```bash
git add sql/init-database.sql
git commit -m "feat(db): 添加商品主档表定义

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建 SPU 实体和枚举

**Files:**
- Create: `EasyProduct.Models\Enums\Product\SpuType.cs`
- Create: `EasyProduct.Models\Entitys\Product\product_spu.cs`

**Step 1: 创建商品类型枚举**

创建文件 `EasyProduct.Models\Enums\Product\SpuType.cs`：

```csharp
namespace EasyProduct.Models.Enums.Product;

/// <summary>
/// 商品类型枚举
/// </summary>
public enum SpuType
{
    /// <summary>
    /// 实物商品
    /// </summary>
    Physical = 1,

    /// <summary>
    /// 虚拟商品
    /// </summary>
    Virtual = 2,

    /// <summary>
    /// 票品
    /// </summary>
    Ticket = 3
}
```

**Step 2: 创建 SPU 实体类**

创建文件 `EasyProduct.Models\Entitys\Product\product_spu.cs`：

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Product;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品主档实体
/// </summary>
[SugarTable("product_spu", "商品主档表")]
public class product_spu : BaseEntity
{
    /// <summary>
    /// 商品名称
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "商品名称")]
    public string SpuName { get; set; } = string.Empty;

    /// <summary>
    /// 商品编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "商品编码")]
    public string? SpuCode { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "分类ID")]
    public string? CategoryId { get; set; }

    /// <summary>
    /// 主图URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "主图URL")]
    public string? MainImage { get; set; }

    /// <summary>
    /// 商品图集（JSON数组）
    /// </summary>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "商品图集")]
    public string? Images { get; set; }

    /// <summary>
    /// 商品描述（富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "商品描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 计量单位
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "计量单位")]
    public string? Unit { get; set; } = "件";

    /// <summary>
    /// 商品类型
    /// </summary>
    [SugarColumn(ColumnDescription = "商品类型")]
    public SpuType SpuType { get; set; } = SpuType.Physical;

    /// <summary>
    /// 品牌
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "品牌")]
    public string? Brand { get; set; }

    /// <summary>
    /// 规格模板（JSON）
    /// </summary>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "规格模板")]
    public string? SpecTemplate { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;
}
```

**Step 3: 验证编译并提交**

```bash
cd EasyProduct.WebApi && dotnet build
git add .
git commit -m "feat(api): 创建商品主档实体和枚举

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 创建 SPU DTO

**Files:**
- Create: `EasyProduct.Models\Dto\Product\Spu\SpuQueryDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Spu\CreateSpuDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Spu\UpdateSpuDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Spu\SpuDto.cs`

参照批次 1 的 DTO 风格，创建以下文件：

**SpuQueryDto.cs** - 查询参数：
- SpuName: string? (商品名称)
- SpuCode: string? (商品编码)
- CategoryId: string? (分类ID)
- SpuType: int? (商品类型)
- Status: int? (状态)

**CreateSpuDto.cs** - 创建参数：
- SpuName: string (必填)
- SpuCode: string? (商品编码)
- CategoryId: string? (分类ID)
- MainImage: string? (主图)
- Images: string? (图集JSON)
- Description: string? (富文本描述)
- Unit: string? (单位)
- SpuType: int (商品类型)
- Brand: string? (品牌)
- SpecTemplate: string? (规格模板)

**UpdateSpuDto.cs** - 更新参数：
- Id: string (必填)
- 同 CreateSpuDto 的所有字段
- Status: int (状态)

**SpuDto.cs** - 数据传输对象：
- 包含所有实体字段
- CategoryName: string? (分类名称，关联查询)

---

## Task 4: 创建 SPU 服务

**Files:**
- Create: `EasyProduct.Business\Product\ISpuService.cs`
- Create: `EasyProduct.Business\Product\SpuService.cs`

**服务方法：**
- GetSpuPageListAsync(SpuQueryDto query) - 分页查询
- GetSpuByIdAsync(string id) - 获取详情
- GetSpuListByCategoryAsync(string categoryId) - 按分类查询
- CreateSpuAsync(CreateSpuDto dto) - 创建
- UpdateSpuAsync(UpdateSpuDto dto) - 更新
- DeleteSpuAsync(string id) - 删除
- UpdateSpuStatusAsync(string id, int status) - 更新状态

**业务逻辑：**
- 创建时检查编码唯一性
- 更新时检查编码唯一性（排除自己）
- 删除时检查是否有关联的 SKU（等批次 3 实现后添加）
- 关联查询分类名称

---

## Task 5: 创建 SPU 控制器

**Files:**
- Create: `EasyProduct.Web\Controllers\Admin\Product\SpuController.cs`

**API 接口：**
- `GET /api/admin/product/spu/list` - 分页查询
- `GET /api/admin/product/spu/{id}` - 获取详情
- `GET /api/admin/product/spu/category/{categoryId}` - 按分类查询
- `POST /api/admin/product/spu` - 创建
- `PUT /api/admin/product/spu/{id}` - 更新
- `DELETE /api/admin/product/spu/{id}` - 删除
- `PUT /api/admin/product/spu/{id}/status` - 更新状态

---

## 验收标准

- ✅ SPU 表已创建
- ✅ SPU 实体、DTO、服务、控制器已实现
- ✅ 支持分页查询、详情查询、创建、更新、删除
- ✅ 项目构建成功（0 编译错误）
- ✅ 所有方法添加完整中文注释

---

## 注意事项

1. 遵循批次 1 已建立的代码风格
2. 所有方法必须有完整中文注释
3. 使用 Mapster 进行对象映射
4. 使用 BusinessException 抛出业务异常
5. 软删除标记：IsDeleted == 0