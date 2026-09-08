# P2 商品管理模块实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现商品中心模块，包括商品分类、商品主档（SPU）、SKU 管理、渠道发布和商品图集管理，支持官网/小程序/B2B 三端共用。

**Architecture:** 模块化单体架构，Product 模块包含分类管理、SPU/SKU 管理、渠道发布、图集管理四个子模块。使用 SqlSugar ORM，遵循项目统一的数据访问规范，支持软删除和审计日志。

**Tech Stack:** .NET 8.0 + SqlSugar 5.1.4 + Mapster 10.x + MySqlConnector 2.5.x + Serilog 8.x

---

## 模块划分

本计划分为 5 个批次，每个批次独立可演示：

| 批次 | 功能模块 | 核心功能 | 预计工时 |
|------|---------|---------|---------|
| **批次 1** | 商品分类 | 分类树形管理 | 1-2 天 |
| **批次 2** | 商品主档 | SPU 管理 | 2-3 天 |
| **批次 3** | SKU 管理 | 规格与库存 | 1-2 天 |
| **批次 4** | 商品图集 | 图片管理 | 1 天 |
| **批次 5** | 渠道发布 | 多端发布 | 1-2 天 |

---

## 批次 1：商品分类管理

### Task 1: 更新数据库初始化脚本

**Files:**
- Modify: `sql/init-database.sql`

- [ ] **Step 1: 添加商品分类表定义**

在 `sql/init-database.sql` 文件的 `-- ========================================` 分隔线后添加：

```sql
-- ========================================
-- Product 模块表
-- ========================================

-- ----------------------------
-- 商品分类表 (product_category)
-- ----------------------------
DROP TABLE IF EXISTS `product_category`;
CREATE TABLE `product_category` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `parent_id` CHAR(36) DEFAULT '0' COMMENT '父分类ID（根节点为0）',
  `category_name` VARCHAR(100) NOT NULL COMMENT '分类名称',
  `category_code` VARCHAR(50) DEFAULT NULL COMMENT '分类编码',
  `icon` VARCHAR(255) DEFAULT NULL COMMENT '分类图标',
  `image` VARCHAR(500) DEFAULT NULL COMMENT '分类图片',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `level` INT DEFAULT 1 COMMENT '层级',
  `full_path` VARCHAR(500) DEFAULT NULL COMMENT '完整路径',
  `show_in_nav` INT DEFAULT 1 COMMENT '是否显示在导航：0=否，1=是',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品分类表';
```

- [ ] **Step 2: 验证 SQL 脚本**

运行以下命令验证 SQL 语法：

```bash
mysql -u root -p -e "source D:\4-MyProject\EasyProduct\sql\init-database.sql" 2>&1 | head -20
```

Expected: 无语法错误提示

- [ ] **Step 3: 提交**

```bash
git add sql/init-database.sql
git commit -m "feat(db): 添加商品分类表定义"
```

---

### Task 2: 创建商品分类实体

**Files:**
- Create: `EasyProduct.Models\Entitys\Product\product_category.cs`

- [ ] **Step 1: 创建商品分类实体类**

创建文件 `EasyProduct.Models\Entitys\Product\product_category.cs`：

```csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品分类实体
/// </summary>
/// <remarks>
/// 对应数据库表 product_category
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于存储商品分类信息，支持树形结构
/// </remarks>
[SugarTable("product_category", "商品分类表")]
public class product_category : BaseEntity
{
    /// <summary>
    /// 父分类ID
    /// </summary>
    /// <remarks>
    /// 上级分类ID，根分类为空字符串或"0"
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "父分类ID")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "分类名称")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分类编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "分类编码")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "分类图标")]
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "分类图片")]
    public string? Image { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 层级
    /// </summary>
    [SugarColumn(ColumnDescription = "层级")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 完整路径
    /// </summary>
    /// <remarks>
    /// 如：电子产品/手机/智能手机，用于快速定位和显示
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "完整路径")]
    public string? FullPath { get; set; }

    /// <summary>
    /// 是否显示在导航
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否显示在导航")]
    public int ShowInNav { get; set; } = 1;
}
```

- [ ] **Step 2: 验证编译**

```bash
cd EasyProduct.WebApi && dotnet build 2>&1 | grep -E "(成功|失败|error)"
```

Expected: 已成功生成

- [ ] **Step 3: 提交**

```bash
git add EasyProduct.Models/Entitys/Product/product_category.cs
git commit -m "feat(api): 创建商品分类实体类"
```

---

### Task 3: 创建商品分类 DTO

**Files:**
- Create: `EasyProduct.Models\Dto\Product\Category\CategoryQueryDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Category\CreateCategoryDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Category\UpdateCategoryDto.cs`
- Create: `EasyProduct.Models\Dto\Product\Category\CategoryDto.cs`

- [ ] **Step 1: 创建查询参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Category\CategoryQueryDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 商品分类查询参数
/// </summary>
public class CategoryQueryDto
{
    /// <summary>
    /// 分类名称（模糊搜索）
    /// </summary>
    [MaxLength(100)]
    public string? CategoryName { get; set; }

    /// <summary>
    /// 分类编码（模糊搜索）
    /// </summary>
    [MaxLength(50)]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 商品分类树形结构查询参数
/// </summary>
public class CategoryTreeQueryDto
{
    /// <summary>
    /// 父分类ID（可选，不传则查询全部）
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 是否只查询启用的分类
    /// </summary>
    public bool? OnlyEnabled { get; set; }
}
```

- [ ] **Step 2: 创建创建参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Category\CreateCategoryDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 创建商品分类参数
/// </summary>
public class CreateCategoryDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(100, ErrorMessage = "分类名称不能超过100个字符")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 父分类ID（根分类传空字符串或"0"）
    /// </summary>
    [MaxLength(36)]
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类编码
    /// </summary>
    [MaxLength(50, ErrorMessage = "分类编码不能超过50个字符")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    [MaxLength(255)]
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    [MaxLength(500)]
    public string? Image { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 是否显示在导航：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "显示导航值只能是0或1")]
    public int ShowInNav { get; set; } = 1;
}
```

- [ ] **Step 3: 创建更新参数 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Category\UpdateCategoryDto.cs`：

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 更新商品分类参数
/// </summary>
public class UpdateCategoryDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    [Required(ErrorMessage = "分类ID不能为空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(100, ErrorMessage = "分类名称不能超过100个字符")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 父分类ID
    /// </summary>
    [MaxLength(36)]
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类编码
    /// </summary>
    [MaxLength(50, ErrorMessage = "分类编码不能超过50个字符")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    [MaxLength(255)]
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    [MaxLength(500)]
    public string? Image { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值只能是0或1")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 是否显示在导航：0=否，1=是
    /// </summary>
    [Range(0, 1, ErrorMessage = "显示导航值只能是0或1")]
    public int ShowInNav { get; set; }
}
```

- [ ] **Step 4: 创建数据传输对象 DTO**

创建文件 `EasyProduct.Models\Dto\Product\Category\CategoryDto.cs`：

```csharp
namespace EasyProduct.Models.Dto.Product.Category;

/// <summary>
/// 商品分类DTO
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 父分类ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分类编码
    /// </summary>
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 完整路径
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// 是否显示在导航
    /// </summary>
    public int ShowInNav { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

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

    /// <summary>
    /// 子分类列表
    /// </summary>
    public List<CategoryDto>? Children { get; set; }
}

/// <summary>
/// 商品分类树形结构DTO
/// </summary>
public class CategoryTreeDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 父分类ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 分类编码
    /// </summary>
    public string? CategoryCode { get; set; }

    /// <summary>
    /// 分类图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 分类图片
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// 完整路径
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否显示在导航
    /// </summary>
    public int ShowInNav { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 子分类列表
    /// </summary>
    public List<CategoryTreeDto>? Children { get; set; }
}
```

- [ ] **Step 5: 验证编译**

```bash
cd EasyProduct.WebApi && dotnet build 2>&1 | grep -E "(成功|失败|error)"
```

Expected: 已成功生成

- [ ] **Step 6: 提交**

```bash
git add EasyProduct.Models/Dto/Product/Category/
git commit -m "feat(api): 创建商品分类 DTO"
```

---

### Task 4: 创建商品分类服务

**Files:**
- Create: `EasyProduct.Business\Product\ICategoryService.cs`
- Create: `EasyProduct.Business\Product\CategoryService.cs`

- [ ] **Step 1: 创建服务接口**

创建文件 `EasyProduct.Business\Product\ICategoryService.cs`：

```csharp
using EasyProduct.Models.Dto.Product.Category;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品分类服务接口
/// </summary>
/// <remarks>
/// 提供商品分类的增删改查、树形结构查询等功能
/// </remarks>
public interface ICategoryService
{
    /// <summary>
    /// 获取分类树形结构
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分类树形列表</returns>
    Task<List<CategoryTreeDto>> GetCategoryTreeAsync(CategoryTreeQueryDto query);

    /// <summary>
    /// 获取分类列表（扁平）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分类列表</returns>
    Task<List<CategoryDto>> GetCategoryListAsync(CategoryQueryDto query);

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    Task<CategoryDto> GetCategoryByIdAsync(string id);

    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    Task<string> CreateCategoryAsync(CreateCategoryDto dto);

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto);

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteCategoryAsync(string id);

    /// <summary>
    /// 更新分类状态
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateCategoryStatusAsync(string id, int status);
}
```

- [ ] **Step 2: 创建服务实现（第一部分：构造函数和基础方法）**

创建文件 `EasyProduct.Business\Product\CategoryService.cs`：

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Category;
using EasyProduct.Models.Entitys.Product;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品分类服务实现
/// </summary>
/// <remarks>
/// 提供商品分类的增删改查、树形结构查询等功能
/// </remarks>
public class CategoryService : BaseService, ICategoryService
{
    private readonly ILogger<CategoryService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public CategoryService(ILogger<CategoryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取分类树形结构
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分类树形列表</returns>
    public async Task<List<CategoryTreeDto>> GetCategoryTreeAsync(CategoryTreeQueryDto query)
    {
        // 查询所有分类（根据条件过滤）
        var queryable = _db.Queryable<product_category>()
            .Where(x => x.IsDeleted == 0);

        if (query.OnlyEnabled == true)
        {
            queryable = queryable.Where(x => x.Status == Models.Enums.Status.Enabled);
        }

        if (!string.IsNullOrEmpty(query.ParentId))
        {
            queryable = queryable.Where(x => x.ParentId == query.ParentId);
        }

        var categories = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        // 转换为 DTO
        var categoryDtos = categories.Adapt<List<CategoryTreeDto>>();

        // 构建树形结构
        return BuildCategoryTree(categoryDtos, "0");
    }

    /// <summary>
    /// 获取分类列表（扁平）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分类列表</returns>
    public async Task<List<CategoryDto>> GetCategoryListAsync(CategoryQueryDto query)
    {
        var queryable = _db.Queryable<product_category>()
            .Where(x => x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.CategoryName))
        {
            queryable = queryable.Where(x => x.CategoryName.Contains(query.CategoryName));
        }

        if (!string.IsNullOrEmpty(query.CategoryCode))
        {
            queryable = queryable.Where(x => x.CategoryCode != null && x.CategoryCode.Contains(query.CategoryCode));
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        var categories = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        return categories.Adapt<List<CategoryDto>>();
    }

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    public async Task<CategoryDto> GetCategoryByIdAsync(string id)
    {
        var category = await _db.Queryable<product_category>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        return category.Adapt<CategoryDto>();
    }
```

- [ ] **Step 3: 创建服务实现（第二部分：创建和更新方法）**

在 `CategoryService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    public async Task<string> CreateCategoryAsync(CreateCategoryDto dto)
    {
        // 检查分类编码是否重复
        if (!string.IsNullOrEmpty(dto.CategoryCode))
        {
            var exists = await _db.Queryable<product_category>()
                .Where(x => x.CategoryCode == dto.CategoryCode && x.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException($"分类编码 {dto.CategoryCode} 已存在");
            }
        }

        // 创建分类实体
        var category = dto.Adapt<product_category>();
        category.Id = Guid.NewGuid();
        category.Status = Models.Enums.Status.Enabled;
        category.CreatedAt = DateTime.UtcNow;

        // 计算分类层级和完整路径
        await CalculateCategoryLevelAndPath(category);

        // 插入数据库
        await _db.Insertable(category).ExecuteCommandAsync();

        _logger.LogInformation("创建商品分类成功：{CategoryName}, ID: {Id}", category.CategoryName, category.Id);

        return category.Id.ToString();
    }

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto)
    {
        // 检查分类是否存在
        var category = await _db.Queryable<product_category>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        // 检查分类编码是否重复（排除自己）
        if (!string.IsNullOrEmpty(dto.CategoryCode))
        {
            var exists = await _db.Queryable<product_category>()
                .Where(x => x.CategoryCode == dto.CategoryCode && x.Id.ToString() != dto.Id && x.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException($"分类编码 {dto.CategoryCode} 已存在");
            }
        }

        // 检查是否将分类设置为自己的子分类
        if (dto.ParentId == dto.Id)
        {
            throw new BusinessException("不能将分类的上级设置为自己");
        }

        // 更新分类信息
        category.CategoryName = dto.CategoryName;
        category.ParentId = dto.ParentId;
        category.CategoryCode = dto.CategoryCode;
        category.Icon = dto.Icon;
        category.Image = dto.Image;
        category.Sort = dto.Sort;
        category.Status = (Models.Enums.Status)dto.Status;
        category.ShowInNav = dto.ShowInNav;
        category.UpdatedAt = DateTime.UtcNow;

        // 重新计算分类层级和完整路径
        await CalculateCategoryLevelAndPath(category);

        // 更新数据库
        await _db.Updateable(category).ExecuteCommandAsync();

        _logger.LogInformation("更新商品分类成功：{CategoryName}, ID: {Id}", category.CategoryName, category.Id);

        return true;
    }
```

- [ ] **Step 4: 创建服务实现（第三部分：删除和辅助方法）**

在 `CategoryService.cs` 文件中继续添加：

```csharp
    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteCategoryAsync(string id)
    {
        // 检查分类是否存在
        var category = await _db.Queryable<product_category>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        // 检查是否有子分类
        var hasChildren = await _db.Queryable<product_category>()
            .Where(x => x.ParentId == id && x.IsDeleted == 0)
            .AnyAsync();

        if (hasChildren)
        {
            throw new BusinessException("该分类下存在子分类，不能删除");
        }

        // TODO: 检查分类下是否有商品（等商品表创建后实现）

        // 软删除
        category.IsDeleted = 1;
        category.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(category).ExecuteCommandAsync();

        _logger.LogInformation("删除商品分类成功：{CategoryName}, ID: {Id}", category.CategoryName, category.Id);

        return true;
    }

    /// <summary>
    /// 更新分类状态
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateCategoryStatusAsync(string id, int status)
    {
        var category = await _db.Queryable<product_category>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (category == null)
        {
            throw new BusinessException("分类不存在", 404);
        }

        category.Status = (Models.Enums.Status)status;
        category.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(category).ExecuteCommandAsync();

        _logger.LogInformation("更新商品分类状态成功：{CategoryName}, 状态: {Status}", category.CategoryName, status);

        return true;
    }

    /// <summary>
    /// 构建分类树形结构
    /// </summary>
    /// <param name="allCategories">所有分类列表</param>
    /// <param name="parentId">父分类ID</param>
    /// <returns>树形结构列表</returns>
    private List<CategoryTreeDto> BuildCategoryTree(List<CategoryTreeDto> allCategories, string parentId)
    {
        var children = allCategories
            .Where(x => x.ParentId == parentId || (string.IsNullOrEmpty(x.ParentId) && parentId == "0"))
            .OrderBy(x => x.Sort)
            .ToList();

        foreach (var child in children)
        {
            child.Children = BuildCategoryTree(allCategories, child.Id);
        }

        return children;
    }

    /// <summary>
    /// 计算分类层级和完整路径
    /// </summary>
    /// <param name="category">分类实体</param>
    private async Task CalculateCategoryLevelAndPath(product_category category)
    {
        if (string.IsNullOrEmpty(category.ParentId) || category.ParentId == "0")
        {
            // 根分类
            category.Level = 1;
            category.FullPath = category.CategoryName;
        }
        else
        {
            // 查询父分类
            var parent = await _db.Queryable<product_category>()
                .Where(x => x.Id.ToString() == category.ParentId && x.IsDeleted == 0)
                .FirstAsync();

            if (parent == null)
            {
                throw new BusinessException("父分类不存在");
            }

            category.Level = parent.Level + 1;
            category.FullPath = $"{parent.FullPath}/{category.CategoryName}";
        }
    }
}
```

- [ ] **Step 5: 验证编译**

```bash
cd EasyProduct.WebApi && dotnet build 2>&1 | grep -E "(成功|失败|error)"
```

Expected: 已成功生成

- [ ] **Step 6: 提交**

```bash
git add EasyProduct.Business/Product/
git commit -m "feat(api): 实现商品分类服务"
```

---

### Task 5: 创建商品分类控制器

**Files:**
- Create: `EasyProduct.Web\Controllers\Admin\Product\CategoryController.cs`

- [ ] **Step 1: 创建分类控制器**

创建文件 `EasyProduct.Web\Controllers\Admin\Product\CategoryController.cs`：

```csharp
using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 商品分类管理控制器
/// </summary>
/// <remarks>
/// 提供商品分类的增删改查、树形结构查询等功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/product/category")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="categoryService">分类服务</param>
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// 获取分类树形结构
    /// </summary>
    /// <param name="parentId">父分类ID（可选，不传则查询全部）</param>
    /// <param name="onlyEnabled">是否只查询启用的分类</param>
    /// <returns>分类树形列表</returns>
    /// <remarks>
    /// 获取分类的树形结构，用于构建分类树
    /// 支持按父分类ID筛选，支持只查询启用的分类
    /// </remarks>
    [HttpGet("tree")]
    public async Task<ApiResponse<List<CategoryTreeDto>>> GetCategoryTree(
        [FromQuery] string? parentId,
        [FromQuery] bool? onlyEnabled)
    {
        var query = new CategoryTreeQueryDto
        {
            ParentId = parentId,
            OnlyEnabled = onlyEnabled
        };

        var result = await _categoryService.GetCategoryTreeAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取分类列表（扁平）
    /// </summary>
    /// <param name="categoryName">分类名称（模糊搜索）</param>
    /// <param name="categoryCode">分类编码（模糊搜索）</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>分类列表</returns>
    /// <remarks>
    /// 获取分类的扁平列表，支持按名称、编码、状态筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<CategoryDto>>> GetCategoryList(
        [FromQuery] string? categoryName,
        [FromQuery] string? categoryCode,
        [FromQuery] int? status)
    {
        var query = new CategoryQueryDto
        {
            CategoryName = categoryName,
            CategoryCode = categoryCode,
            Status = status
        };

        var result = await _categoryService.GetCategoryListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    /// <remarks>
    /// 根据ID获取分类的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CategoryDto>> GetCategoryById(string id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    /// <remarks>
    /// 创建新的分类
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var result = await _categoryService.CreateCategoryAsync(dto);
        return Success(result, "分类创建成功");
    }

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新分类信息
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> UpdateCategory(string id, [FromBody] UpdateCategoryDto dto)
    {
        dto.Id = id;
        var result = await _categoryService.UpdateCategoryAsync(dto);
        return Success(result, "分类更新成功");
    }

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除分类（软删除）
    /// 注意：如果分类下存在子分类或商品，则不能删除
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteCategory(string id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return Success(result, "分类删除成功");
    }

    /// <summary>
    /// 更新分类状态
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 启用或禁用分类
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateCategoryStatus(string id, [FromQuery] int status)
    {
        var result = await _categoryService.UpdateCategoryStatusAsync(id, status);
        return Success(result, "分类状态更新成功");
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
git add EasyProduct.Web/Controllers/Admin/Product/
git commit -m "feat(api): 实现商品分类控制器

- 创建 CategoryController
- 提供分类树形结构查询
- 提供分类 CRUD 接口
- 支持分类状态管理
- 添加完整的中文注释"
```

---

## 批次 1 完成验收

**验收标准：**
- ✅ 商品分类表已创建
- ✅ 商品分类实体、DTO、服务、控制器已实现
- ✅ 支持树形结构查询
- ✅ 支持 CRUD 操作
- ✅ 项目构建成功（0 编译错误）
- ✅ 所有方法添加完整中文注释

**API 列表：**
- `GET /api/admin/product/category/tree` - 获取分类树
- `GET /api/admin/product/category/list` - 获取分类列表
- `GET /api/admin/product/category/{id}` - 获取分类详情
- `POST /api/admin/product/category` - 创建分类
- `PUT /api/admin/product/category/{id}` - 更新分类
- `DELETE /api/admin/product/category/{id}` - 删除分类
- `PUT /api/admin/product/category/{id}/status` - 更新分类状态

---

## 批次 2-5 说明

由于篇幅限制，批次 2-5 的详细计划将在后续补充。每个批次将包含：

- **批次 2：商品主档（SPU）管理**
  - SPU 表定义
  - SPU 实体和 DTO
  - SPU 服务和控制器
  - 富文本详情支持

- **批次 3：SKU 管理**
  - SKU 表定义
  - SKU 实体和 DTO
  - SKU 服务和控制器
  - 规格组合管理

- **批次 4：商品图集**
  - 商品图片表定义
  - 图片上传和管理
  - 图片排序和删除

- **批次 5：渠道发布**
  - 渠道发布表定义
  - 多端发布管理
  - 上架状态控制

---

## 实施建议

**推荐执行方式：**

1. **Subagent-Driven（推荐）**：为每个任务派发独立子代理，快速迭代
2. **Inline Execution**：在当前会话中批量执行，有检查点

**建议：**
- 按批次顺序实施
- 每个批次完成后进行验收
- 批次 1 完成后可进行前端对接测试
- 注意遵循项目现有的代码规范和注释规范