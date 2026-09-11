# 会员端商品、微信支付与单元测试实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现会员端商品浏览功能、微信支付集成以及核心业务单元测试，为小程序商城提供完整的购物支付能力。

**Architecture:** 采用分层架构设计，包括 DTO 层（数据传输对象）、Service 层（业务逻辑）、Controller 层（API 接口）。会员端商品控制器复用现有服务，微信支付服务采用配置化管理支持开发/生产环境切换，单元测试使用 In-Memory SQLite 确保隔离性。

**Tech Stack:** .NET 8.0, SqlSugar ORM, xUnit, Moq, Bogus, Quartz, Polly, 微信支付 V3 API

---

## 📋 文件结构映射

### 子系统 1：会员端商品控制器

**创建文件：**
- `EasyProduct.Models/Dto/Product/App/AppProductQueryDto.cs` - 商品查询参数
- `EasyProduct.Models/Dto/Product/App/AppProductListDto.cs` - 商品列表 DTO
- `EasyProduct.Models/Dto/Product/App/AppProductDetailDto.cs` - 商品详情 DTO
- `EasyProduct.Models/Dto/Product/App/AppSkuDto.cs` - SKU DTO
- `EasyProduct.Models/Dto/Product/App/SpecItemDto.cs` - 规格项 DTO
- `EasyProduct.Models/Dto/Product/App/SpecTemplateDto.cs` - 规格模板 DTO
- `EasyProduct.Models/Dto/Product/App/SalesStatisticsDto.cs` - 销量统计 DTO
- `EasyProduct.Models/Dto/Product/App/CategoryTreeDto.cs` - 分类树 DTO
- `EasyProduct.Business/Product/IProductQueryService.cs` - 商品查询服务接口
- `EasyProduct.Business/Product/ProductQueryService.cs` - 商品查询服务实现
- `EasyProduct.WebApi/Controllers/App/ProductController.cs` - 会员端商品控制器

**修改文件：**
- `EasyProduct.WebApi/Program.cs` - 添加限流配置

### 子系统 2：微信支付集成

**创建文件：**
- `EasyProduct.Models/Options/WxPayOptions.cs` - 微信支付配置类
- `EasyProduct.Business/Mall/IWxPayService.cs` - 微信支付服务接口
- `EasyProduct.Business/Mall/WxPayService.cs` - 微信支付服务实现
- `EasyProduct.Business/Mall/Jobs/PaymentTimeoutJob.cs` - 支付超时检查任务
- `EasyProduct.Models/Dto/Mall/Payment/PaymentCallbackLogDto.cs` - 支付回调日志 DTO
- `EasyProduct.Models/Entitys/Mall/PaymentCallbackLog.cs` - 支付回调日志实体

**修改文件：**
- `EasyProduct.WebApi/appsettings.json` - 添加微信支付配置
- `EasyProduct.WebApi/Program.cs` - 注册微信支付服务和定时任务

### 子系统 3：单元测试

**创建文件：**
- `EasyProduct.Tests/EasyProduct.Tests.csproj` - 测试项目
- `EasyProduct.Tests/TestBase/TestBase.cs` - 测试基类
- `EasyProduct.Tests/TestData/TestDataFactory.cs` - 测试数据工厂
- `EasyProduct.Tests/TestData/TestDataGenerator.cs` - 数据生成器（Bogus）
- `EasyProduct.Tests/Business/Mall/PaymentServiceTests.cs` - 支付服务测试
- `EasyProduct.Tests/Business/Mall/OrderServiceTests.cs` - 订单服务测试
- `EasyProduct.Tests/Business/Mall/CartServiceTests.cs` - 购物车服务测试
- `EasyProduct.Tests/Business/Mall/CouponServiceTests.cs` - 优惠券服务测试
- `EasyProduct.Tests/Business/Mall/PointServiceTests.cs` - 积分服务测试
- `EasyProduct.Tests/Infrastructure/DatabaseFixture.cs` - 数据库测试固件

---

## 🎯 子系统 1：会员端商品控制器

### Task 1: 创建商品查询参数 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Product/App/AppProductQueryDto.cs`

- [ ] **Step 1: 创建商品查询参数 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端商品查询参数
/// </summary>
public class AppProductQueryDto
{
    /// <summary>
    /// 分类 ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 搜索关键词
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 最低价格
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// 最高价格
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 品牌
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// 排序字段（price/sales/createTime）
    /// </summary>
    public string SortBy { get; set; } = "createTime";

    /// <summary>
    /// 排序方式（asc/desc）
    /// </summary>
    public string SortOrder { get; set; } = "desc";

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;
}
```

- [ ] **Step 2: 验证 DTO 编译通过**

Run: `cd EasyProduct.Models && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Models/Dto/Product/App/AppProductQueryDto.cs
git commit -m "feat(models): 添加会员端商品查询参数 DTO"
```

---

### Task 2: 创建商品列表 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Product/App/AppProductListDto.cs`

- [ ] **Step 1: 创建商品列表 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端商品列表 DTO
/// </summary>
public class AppProductListDto
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 主图
    /// </summary>
    public string? MainImage { get; set; }

    /// <summary>
    /// 最低价格
    /// </summary>
    public decimal MinPrice { get; set; }

    /// <summary>
    /// 最高价格
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 会员价
    /// </summary>
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 销量
    /// </summary>
    public int SalesCount { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string? CategoryName { get; set; }
}
```

- [ ] **Step 2: 验证 DTO 编译通过**

Run: `cd EasyProduct.Models && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Models/Dto/Product/App/AppProductListDto.cs
git commit -m "feat(models): 添加会员端商品列表 DTO"
```

---

### Task 3: 创建规格相关 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Product/App/SpecItemDto.cs`
- Create: `EasyProduct.Models/Dto/Product/App/SpecTemplateDto.cs`
- Create: `EasyProduct.Models/Dto/Product/App/SalesStatisticsDto.cs`

- [ ] **Step 1: 创建规格项 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 规格项 DTO
/// </summary>
public class SpecItemDto
{
    /// <summary>
    /// 规格名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 规格值
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
```

- [ ] **Step 2: 创建规格定义 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 规格定义 DTO
/// </summary>
public class SpecDefinitionDto
{
    /// <summary>
    /// 规格名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 可选值
    /// </summary>
    public List<string> Values { get; set; } = new();
}
```

- [ ] **Step 3: 创建规格模板 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 规格模板 DTO
/// </summary>
public class SpecTemplateDto
{
    /// <summary>
    /// 规格列表
    /// </summary>
    public List<SpecDefinitionDto> Specs { get; set; } = new();
}
```

- [ ] **Step 4: 创建销量统计 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 销量统计 DTO
/// </summary>
public class SalesStatisticsDto
{
    /// <summary>
    /// 总销量
    /// </summary>
    public int TotalSales { get; set; }

    /// <summary>
    /// 月销量
    /// </summary>
    public int MonthSales { get; set; }
}
```

- [ ] **Step 5: 验证编译通过**

Run: `cd EasyProduct.Models && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 6: 提交代码**

```bash
git add EasyProduct.Models/Dto/Product/App/SpecItemDto.cs
git add EasyProduct.Models/Dto/Product/App/SpecTemplateDto.cs
git add EasyProduct.Models/Dto/Product/App/SalesStatisticsDto.cs
git commit -m "feat(models): 添加规格相关 DTO 和销量统计 DTO"
```

---

### Task 4: 创建 SKU 和商品详情 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Product/App/AppSkuDto.cs`
- Create: `EasyProduct.Models/Dto/Product/App/AppProductDetailDto.cs`

- [ ] **Step 1: 创建 SKU DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端 SKU DTO
/// </summary>
public class AppSkuDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// SKU 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// SKU 编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 条码
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// 零售价
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 会员价
    /// </summary>
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 规格列表
    /// </summary>
    public List<SpecItemDto>? Specs { get; set; }
}
```

- [ ] **Step 2: 创建商品详情 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 会员端商品详情 DTO
/// </summary>
public class AppProductDetailDto
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    public string? MainImage { get; set; }

    /// <summary>
    /// 图片列表
    /// </summary>
    public List<string>? Images { get; set; }

    /// <summary>
    /// 分类 ID
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 品牌
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// 计量单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// SKU 列表
    /// </summary>
    public List<AppSkuDto> Skus { get; set; } = new();

    /// <summary>
    /// 规格模板
    /// </summary>
    public SpecTemplateDto? SpecTemplate { get; set; }

    /// <summary>
    /// 销量统计
    /// </summary>
    public SalesStatisticsDto? SalesStats { get; set; }
}
```

- [ ] **Step 3: 验证编译通过**

Run: `cd EasyProduct.Models && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 4: 提交代码**

```bash
git add EasyProduct.Models/Dto/Product/App/AppSkuDto.cs
git add EasyProduct.Models/Dto/Product/App/AppProductDetailDto.cs
git commit -m "feat(models): 添加会员端 SKU 和商品详情 DTO"
```

---

### Task 5: 创建分类树 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Product/App/CategoryTreeDto.cs`

- [ ] **Step 1: 创建分类树 DTO**

```csharp
namespace EasyProduct.Models.Dto.Product.App;

/// <summary>
/// 分类树 DTO
/// </summary>
public class CategoryTreeDto
{
    /// <summary>
    /// 分类 ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 图片
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// 子分类
    /// </summary>
    public List<CategoryTreeDto>? Children { get; set; }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Models && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Models/Dto/Product/App/CategoryTreeDto.cs
git commit -m "feat(models): 添加会员端分类树 DTO"
```

---

### Task 6: 创建商品查询服务接口

**Files:**
- Create: `EasyProduct.Business/Product/IProductQueryService.cs`

- [ ] **Step 1: 创建商品查询服务接口**

```csharp
using EasyProduct.Models.Dto.Product.App;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品查询服务接口（会员端）
/// </summary>
/// <remarks>
/// 封装会员端商品查询逻辑，复用现有的 ISpuService、ICategoryService、ISkuService
/// </remarks>
public interface IProductQueryService
{
    /// <summary>
    /// 查询商品列表（会员端）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>商品列表分页结果</returns>
    Task<PageResponse<AppProductListDto>> GetProductListAsync(AppProductQueryDto query);

    /// <summary>
    /// 获取商品详情（会员端）
    /// </summary>
    /// <param name="productId">商品 ID</param>
    /// <returns>商品详情</returns>
    Task<AppProductDetailDto> GetProductDetailAsync(string productId);

    /// <summary>
    /// 获取商品分类树
    /// </summary>
    /// <returns>分类树</returns>
    Task<List<CategoryTreeDto>> GetCategoryTreeAsync();

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">搜索关键词</param>
    /// <returns>商品列表</returns>
    Task<List<AppProductListDto>> SearchProductsAsync(string keyword);

    /// <summary>
    /// 获取热销商品
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>热销商品列表</returns>
    Task<List<AppProductListDto>> GetHotProductsAsync(int limit);

    /// <summary>
    /// 获取新品推荐
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>新品推荐列表</returns>
    Task<List<AppProductListDto>> GetNewProductsAsync(int limit);
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Business && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Business/Product/IProductQueryService.cs
git commit -m "feat(business): 添加会员端商品查询服务接口"
```

---

### Task 7: 实现商品查询服务（基础查询方法）

**Files:**
- Create: `EasyProduct.Business/Product/ProductQueryService.cs`

**前置依赖:** 需要先检查项目中是否已有 `ISpuService`、`ICategoryService`、`ISkuService` 等现有服务。

- [ ] **Step 1: 检查现有服务**

Run: `cd EasyProduct.Business && find . -name "*SpuService*" -o -name "*CategoryService*" -o -name "*SkuService*"`

- [ ] **Step 2: 创建商品查询服务实现（基础结构）**

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.App;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品查询服务实现（会员端）
/// </summary>
/// <remarks>
/// 封装会员端商品查询逻辑，复用现有的 ISpuService、ICategoryService、ISkuService
/// </remarks>
public class ProductQueryService : BaseService, IProductQueryService
{
    private readonly ILogger<ProductQueryService> _logger;

    public ProductQueryService(ILogger<ProductQueryService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 查询商品列表（会员端）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>商品列表分页结果</returns>
    public async Task<PageResponse<AppProductListDto>> GetProductListAsync(AppProductQueryDto query)
    {
        // TODO: 实现商品列表查询逻辑
        // 1. 构建查询条件（分类、价格、品牌、关键词）
        // 2. 应用排序
        // 3. 分页查询
        // 4. 关联查询分类名称
        // 5. 返回结果

        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取商品详情（会员端）
    /// </summary>
    /// <param name="productId">商品 ID</param>
    /// <returns>商品详情</returns>
    public async Task<AppProductDetailDto> GetProductDetailAsync(string productId)
    {
        // TODO: 实现商品详情查询逻辑
        // 1. 查询商品基本信息
        // 2. 关联查询 SKU 列表
        // 3. 查询分类信息
        // 4. 构建规格模板
        // 5. 统计销量信息
        // 6. 返回结果

        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取商品分类树
    /// </summary>
    /// <returns>分类树</returns>
    public async Task<List<CategoryTreeDto>> GetCategoryTreeAsync()
    {
        // TODO: 实现分类树查询逻辑
        // 1. 查询所有启用的分类
        // 2. 构建树形结构
        // 3. 返回结果

        throw new NotImplementedException();
    }

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">搜索关键词</param>
    /// <returns>商品列表</returns>
    public async Task<List<AppProductListDto>> SearchProductsAsync(string keyword)
    {
        // TODO: 实现商品搜索逻辑
        // 1. 关键词模糊匹配（商品名称、编码）
        // 2. 返回前 20 条结果

        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取热销商品
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>热销商品列表</returns>
    public async Task<List<AppProductListDto>> GetHotProductsAsync(int limit)
    {
        // TODO: 实现热销商品查询逻辑
        // 1. 按销量倒序排序
        // 2. 返回前 N 条

        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取新品推荐
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>新品推荐列表</returns>
    public async Task<List<AppProductListDto>> GetNewProductsAsync(int limit)
    {
        // TODO: 实现新品推荐查询逻辑
        // 1. 按创建时间倒序排序
        // 2. 返回前 N 条

        throw new NotImplementedException();
    }
}
```

- [ ] **Step 3: 验证编译通过**

Run: `cd EasyProduct.Business && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 4: 提交代码**

```bash
git add EasyProduct.Business/Product/ProductQueryService.cs
git commit -m "feat(business): 添加会员端商品查询服务实现（基础结构）"
```

---

### Task 8: 创建会员端商品控制器

**Files:**
- Create: `EasyProduct.WebApi/Controllers/App/ProductController.cs`

- [ ] **Step 1: 创建会员端商品控制器**

```csharp
using EasyProduct.Business.Product;
using EasyProduct.Models.Dto.Product.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyProduct.WebApi.Controllers.App;

/// <summary>
/// 会员端商品控制器
/// </summary>
[ApiController]
[Route("api/app/product")]
public class ProductController : AppControllerBase
{
    private readonly IProductQueryService _productQueryService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(
        IProductQueryService productQueryService,
        ILogger<ProductController> logger)
    {
        _productQueryService = productQueryService;
        _logger = logger;
    }

    /// <summary>
    /// 获取商品列表（支持筛选、排序、分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>商品列表分页结果</returns>
    [HttpGet]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<PageResponse<AppProductListDto>>> GetList([FromQuery] AppProductQueryDto query)
    {
        var result = await _productQueryService.GetProductListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取商品详情
    /// </summary>
    /// <param name="id">商品 ID</param>
    /// <returns>商品详情</returns>
    [HttpGet("{id}")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<AppProductDetailDto>> GetDetail(string id)
    {
        var result = await _productQueryService.GetProductDetailAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 获取商品分类（树形结构）
    /// </summary>
    /// <returns>分类树</returns>
    [HttpGet("categories")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<CategoryTreeDto>>> GetCategories()
    {
        var result = await _productQueryService.GetCategoryTreeAsync();
        return Success(result);
    }

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">搜索关键词</param>
    /// <returns>商品列表</returns>
    [HttpGet("search")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<AppProductListDto>>> Search([FromQuery] string keyword)
    {
        var result = await _productQueryService.SearchProductsAsync(keyword);
        return Success(result);
    }

    /// <summary>
    /// 获取热销商品
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>热销商品列表</returns>
    [HttpGet("hot")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<AppProductListDto>>> GetHot([FromQuery] int limit = 10)
    {
        var result = await _productQueryService.GetHotProductsAsync(limit);
        return Success(result);
    }

    /// <summary>
    /// 获取新品推荐
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>新品推荐列表</returns>
    [HttpGet("new")]
    [EnableRateLimiting("AppPolicy")]
    public async Task<ApiResponse<List<AppProductListDto>>> GetNew([FromQuery] int limit = 10)
    {
        var result = await _productQueryService.GetNewProductsAsync(limit);
        return Success(result);
    }
}
```

- [ ] **Step 2: 检查 AppControllerBase 是否存在**

Run: `find EasyProduct.WebApi/Controllers/App -name "*.cs" | head -5`

如果不存在，需要先创建基类：

```csharp
namespace EasyProduct.WebApi.Controllers.App;

/// <summary>
/// 会员端控制器基类
/// </summary>
public abstract class AppControllerBase : ControllerBase
{
    /// <summary>
    /// 返回成功响应
    /// </summary>
    protected ApiResponse<T> Success<T>(T data)
    {
        return new ApiResponse<T>
        {
            Code = 200,
            Message = "操作成功",
            Data = data,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }
}
```

- [ ] **Step 3: 验证编译通过**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 4: 提交代码**

```bash
git add EasyProduct.WebApi/Controllers/App/ProductController.cs
git commit -m "feat(api): 添加会员端商品控制器"
```

---

### Task 9: 配置限流策略

**Files:**
- Modify: `EasyProduct.WebApi/Program.cs`

- [ ] **Step 1: 在 Program.cs 中添加限流配置**

找到 `Program.cs` 文件，在服务注册部分添加：

```csharp
// 添加限流策略
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("AppPolicy", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,  // 每分钟最多 100 次请求
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            code = 429,
            message = "请求过于频繁，请稍后再试",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        }, cancellationToken);
    };
});
```

在中间件部分添加：

```csharp
// 使用限流中间件
app.UseRateLimiter();
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.WebApi/Program.cs
git commit -m "feat(api): 添加会员端 API 限流配置"
```

---

## 🎯 子系统 2：微信支付集成

### Task 10: 创建微信支付配置类

**Files:**
- Create: `EasyProduct.Models/Options/WxPayOptions.cs`

- [ ] **Step 1: 创建微信支付配置类**

```csharp
namespace EasyProduct.Models.Options;

/// <summary>
/// 微信支付配置
/// </summary>
public class WxPayOptions
{
    /// <summary>
    /// 是否启用微信支付
    /// </summary>
    /// <remarks>
    /// 开发环境可设置为 false，使用模拟支付
    /// </remarks>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 小程序 AppId
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 商户号（从环境变量读取）
    /// </summary>
    /// <remarks>
    /// 环境变量：WX_PAY_MCH_ID
    /// </remarks>
    public string MchId { get; set; } = string.Empty;

    /// <summary>
    /// 商户密钥（从环境变量读取）
    /// </summary>
    /// <remarks>
    /// 环境变量：WX_PAY_API_KEY
    /// </remarks>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// 证书序列号（从环境变量读取）
    /// </summary>
    /// <remarks>
    /// 环境变量：WX_PAY_CERT_SERIAL
    /// </remarks>
    public string CertSerialNo { get; set; } = string.Empty;

    /// <summary>
    /// 证书路径
    /// </summary>
    /// <remarks>
    /// 相对于项目根目录，默认：certs/apiclient_key.pem
    /// </remarks>
    public string CertPath { get; set; } = "certs/apiclient_key.pem";

    /// <summary>
    /// 回调通知 URL
    /// </summary>
    /// <remarks>
    /// 必须是外网可访问的 HTTPS URL
    /// </remarks>
    public string NotifyUrl { get; set; } = string.Empty;

    /// <summary>
    /// 支付超时时间（分钟）
    /// </summary>
    /// <remarks>
    /// 超过此时间未支付的订单将自动关闭
    /// </remarks>
    public int ExpireMinutes { get; set; } = 30;
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Models && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Models/Options/WxPayOptions.cs
git commit -m "feat(models): 添加微信支付配置类"
```

---

### Task 11: 更新 appsettings.json 配置

**Files:**
- Modify: `EasyProduct.WebApi/appsettings.json`

- [ ] **Step 1: 在 appsettings.json 中添加微信支付配置**

```json
{
  "WxPay": {
    "Enabled": false,
    "AppId": "wx1234567890abcdef",
    "MchId": "",
    "ApiKey": "",
    "CertSerialNo": "",
    "CertPath": "certs/apiclient_key.pem",
    "NotifyUrl": "https://api.example.com/api/app/payment/callback",
    "ExpireMinutes": 30
  }
}
```

- [ ] **Step 2: 验证 JSON 格式正确**

Run: `cat EasyProduct.WebApi/appsettings.json | jq .`
Expected: Valid JSON output

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.WebApi/appsettings.json
git commit -m "feat(api): 添加微信支付配置到 appsettings.json"
```

---

### Task 12: 创建微信支付服务接口

**Files:**
- Create: `EasyProduct.Business/Mall/IWxPayService.cs`

- [ ] **Step 1: 创建微信支付服务接口**

```csharp
namespace EasyProduct.Business.Mall;

/// <summary>
/// 微信支付服务接口
/// </summary>
/// <remarks>
/// 提供微信小程序 JSAPI 支付功能。
/// </remarks>
public interface IWxPayService
{
    /// <summary>
    /// 创建 JSAPI 支付订单
    /// </summary>
    /// <param name="orderId">订单 ID</param>
    /// <param name="memberId">会员 ID</param>
    /// <param name="openid">微信 openid</param>
    /// <returns>支付参数</returns>
    /// <remarks>
    /// 1. 查询订单信息，验证订单状态。
    /// 2. 创建支付单（状态：pending）。
    /// 3. 调用微信 JSAPI 下单接口。
    /// 4. 返回前端支付所需的参数（timeStamp、nonceStr、package、signType、paySign）。
    /// </remarks>
    Task<JsapiPayParams> CreateJsapiOrderAsync(string orderId, string memberId, string openid);

    /// <summary>
    /// 处理支付回调
    /// </summary>
    /// <param name="callbackData">回调数据（JSON 格式）</param>
    /// <returns>处理结果</returns>
    /// <remarks>
    /// 1. 验证签名，确保回调来自微信。
    /// 2. 解析回调数据，提取支付结果。
    /// 3. 幂等性检查，防止重复处理。
    /// 4. 更新支付单状态（success/failed）。
    /// 5. 更新订单状态（paid）。
    /// 6. 记录支付流水。
    /// 7. 返回成功响应给微信。
    /// </remarks>
    Task<bool> HandlePayCallbackAsync(string callbackData);

    /// <summary>
    /// 查询支付状态
    /// </summary>
    /// <param name="paymentId">支付单 ID</param>
    /// <returns>支付状态</returns>
    /// <remarks>
    /// 1. 查询本地支付单状态。
    /// 2. 如果支付单已成功，直接返回。
    /// 3. 如果支付单状态为 pending，调用微信查询接口同步状态。
    /// 4. 更新支付单和订单状态。
    /// </remarks>
    Task<PaymentStatus> QueryPayStatusAsync(string paymentId);

    /// <summary>
    /// 关闭订单（超时未支付）
    /// </summary>
    /// <param name="paymentId">支付单 ID</param>
    /// <returns>是否关闭成功</returns>
    /// <remarks>
    /// 1. 检查支付单状态（仅 pending 状态可关闭）。
    /// 2. 调用微信关单接口。
    /// 3. 更新支付单状态为 closed。
    /// 4. 更新订单状态为 cancelled。
    /// </remarks>
    Task<bool> CloseOrderAsync(string paymentId);
}

/// <summary>
/// JSAPI 支付参数
/// </summary>
public class JsapiPayParams
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public string TimeStamp { get; set; } = string.Empty;

    /// <summary>
    /// 随机字符串
    /// </summary>
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>
    /// 订单详情扩展字符串
    /// </summary>
    public string Package { get; set; } = string.Empty;

    /// <summary>
    /// 签名方式
    /// </summary>
    public string SignType { get; set; } = "RSA";

    /// <summary>
    /// 签名
    /// </summary>
    public string PaySign { get; set; } = string.Empty;
}

/// <summary>
/// 支付状态
/// </summary>
public class PaymentStatus
{
    /// <summary>
    /// 支付单 ID
    /// </summary>
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 订单 ID
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 状态（pending、success、failed、closed）
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PayTime { get; set; }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Business && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Business/Mall/IWxPayService.cs
git commit -m "feat(business): 添加微信支付服务接口"
```

---

### Task 13: 实现微信支付服务（基础结构）

**Files:**
- Create: `EasyProduct.Business/Mall/WxPayService.cs`

- [ ] **Step 1: 创建微信支付服务实现（基础结构）**

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Payment;
using EasyProduct.Models.Enums.Mall;
using EasyProduct.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 微信支付服务实现
/// </summary>
/// <remarks>
/// 提供微信小程序 JSAPI 支付功能。
/// 开发环境使用模拟支付，生产环境调用真实微信支付 API。
/// </remarks>
public class WxPayService : BaseService, IWxPayService
{
    private readonly ILogger<WxPayService> _logger;
    private readonly WxPayOptions _options;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly HttpClient _httpClient;

    public WxPayService(
        ILogger<WxPayService> logger,
        IOptions<WxPayOptions> options,
        IPaymentService paymentService,
        IOrderService orderService,
        HttpClient httpClient)
    {
        _logger = logger;
        _options = options.Value;
        _paymentService = paymentService;
        _orderService = orderService;
        _httpClient = httpClient;
    }

    /// <summary>
    /// 创建 JSAPI 支付订单
    /// </summary>
    public async Task<JsapiPayParams> CreateJsapiOrderAsync(string orderId, string memberId, string openid)
    {
        // 1. 检查配置
        if (!_options.Enabled || string.IsNullOrEmpty(_options.AppId))
        {
            _logger.LogWarning("微信支付未启用或配置不完整，使用模拟支付");
            return CreateMockPayParams();
        }

        // 2. 查询订单信息
        var order = await _orderService.GetOrderDetailAsync(orderId);
        if (order == null)
            throw BusinessException.NotFound("订单不存在");

        if (order.Status != OrderStatus.PendingPayment)
            throw BusinessException.BadRequest("订单状态不正确，无法支付");

        // 3. 创建支付单
        var payment = await _paymentService.CreatePaymentAsync(new CreatePaymentDto
        {
            OrderId = orderId,
            MemberId = memberId,
            Amount = order.PayAmount,
            PaymentMethod = "wx_jsapi"
        });

        // 4. 调用微信下单接口
        var wxRequest = new
        {
            appid = _options.AppId,
            mchid = _options.MchId,
            description = $"订单-{order.OrderNo}",
            out_trade_no = payment.PaymentNo,
            notify_url = _options.NotifyUrl,
            amount = new { total = (int)(order.PayAmount * 100) },  // 单位：分
            payer = new { openid = openid }
        };

        var response = await PostWxApiAsync("v3/pay/transactions/jsapi", wxRequest);

        // 5. 生成支付参数
        return GeneratePayParams(response.prepay_id);
    }

    /// <summary>
    /// 处理支付回调
    /// </summary>
    public async Task<bool> HandlePayCallbackAsync(string callbackData)
    {
        // 1. 验证签名
        if (!VerifySignature(callbackData))
        {
            _logger.LogWarning("微信支付回调签名验证失败");
            return false;
        }

        // 2. 解析回调数据
        var callback = ParseCallbackData(callbackData);

        // 3. 幂等性检查
        var payment = await _paymentService.GetByPaymentNoAsync(callback.out_trade_no);
        if (payment == null)
        {
            _logger.LogWarning("支付单不存在：{PaymentNo}", callback.out_trade_no);
            return false;
        }

        if (payment.Status == PaymentStatus.Success)
        {
            _logger.LogInformation("支付单已处理，跳过：{PaymentNo}", callback.out_trade_no);
            return true;
        }

        // 4. 更新支付单状态
        if (callback.trade_state == "SUCCESS")
        {
            await _paymentService.UpdatePaymentStatusAsync(
                payment.Id.ToString(),
                PaymentStatus.Success,
                callback.transaction_id);

            // 5. 更新订单状态
            await _orderService.UpdateOrderStatusAsync(
                payment.OrderId,
                OrderStatus.PendingDelivery);

            // 6. 记录支付流水
            await _paymentService.CreatePaymentLogAsync(
                payment.Id.ToString(),
                "支付成功",
                callbackData);

            _logger.LogInformation("支付回调处理成功，支付单：{PaymentNo}", callback.out_trade_no);
            return true;
        }
        else
        {
            await _paymentService.UpdatePaymentStatusAsync(
                payment.Id.ToString(),
                PaymentStatus.Failed,
                callback.transaction_id);

            _logger.LogWarning("支付失败，支付单：{PaymentNo}，状态：{Status}", callback.out_trade_no, callback.trade_state);
            return false;
        }
    }

    /// <summary>
    /// 查询支付状态
    /// </summary>
    public async Task<PaymentStatus> QueryPayStatusAsync(string paymentId)
    {
        var payment = await _paymentService.GetPaymentAsync(paymentId);
        if (payment == null)
            throw BusinessException.NotFound("支付单不存在");

        // 如果支付单已成功，直接返回
        if (payment.Status == PaymentStatus.Success)
            return MapToPaymentStatus(payment);

        // 调用微信查询接口
        var wxResponse = await GetWxApiAsync($"v3/pay/transactions/out-trade-no/{payment.PaymentNo}?mchid={_options.MchId}");

        // 更新支付单状态
        if (wxResponse.trade_state == "SUCCESS")
        {
            await _paymentService.UpdatePaymentStatusAsync(
                payment.Id.ToString(),
                PaymentStatus.Success,
                wxResponse.transaction_id);

            await _orderService.UpdateOrderStatusAsync(
                payment.OrderId,
                OrderStatus.PendingDelivery);
        }

        return MapToPaymentStatus(payment);
    }

    /// <summary>
    /// 关闭订单
    /// </summary>
    public async Task<bool> CloseOrderAsync(string paymentId)
    {
        var payment = await _paymentService.GetPaymentAsync(paymentId);
        if (payment == null || payment.Status == PaymentStatus.Success)
            return false;

        // 调用微信关单接口
        await CloseWxApiAsync(
            $"v3/pay/transactions/out-trade-no/{payment.PaymentNo}/close",
            new { mchid = _options.MchId });

        // 更新支付单状态
        await _paymentService.UpdatePaymentStatusAsync(
            payment.Id.ToString(),
            PaymentStatus.Closed);

        // 更新订单状态
        await _orderService.UpdateOrderStatusAsync(
            payment.OrderId,
            OrderStatus.Cancelled);

        _logger.LogInformation("支付单已关闭：{PaymentNo}", payment.PaymentNo);
        return true;
    }

    // ========== 私有方法 ==========

    /// <summary>
    /// 创建模拟支付参数
    /// </summary>
    private JsapiPayParams CreateMockPayParams()
    {
        return new JsapiPayParams
        {
            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            NonceStr = Guid.NewGuid().ToString("N"),
            Package = "prepay_id=wx_mock_prepay_id",
            SignType = "RSA",
            PaySign = "mock_sign"
        };
    }

    /// <summary>
    /// 生成支付参数
    /// </summary>
    private JsapiPayParams GeneratePayParams(string prepayId)
    {
        var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonceStr = Guid.NewGuid().ToString("N");
        var package = $"prepay_id={prepayId}";

        // 生成签名
        var signStr = $"{_options.AppId}\n{timeStamp}\n{nonceStr}\n{package}\n";
        var paySign = Sign(signStr);

        return new JsapiPayParams
        {
            TimeStamp = timeStamp,
            NonceStr = nonceStr,
            Package = package,
            SignType = "RSA",
            PaySign = paySign
        };
    }

    /// <summary>
    /// 验证签名
    /// </summary>
    private bool VerifySignature(string callbackData)
    {
        // TODO: 实现签名验证逻辑
        // 参考微信支付文档：https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_5_5.shtml
        return true;
    }

    /// <summary>
    /// 解析回调数据
    /// </summary>
    private dynamic ParseCallbackData(string callbackData)
    {
        // TODO: 解析微信支付回调数据
        return JsonSerializer.Deserialize<dynamic>(callbackData);
    }

    /// <summary>
    /// 签名
    /// </summary>
    private string Sign(string message)
    {
        // TODO: 使用商户私钥签名
        using var rsa = RSA.Create();
        // 加载证书...
        var signature = rsa.SignData(
            Encoding.UTF8.GetBytes(message),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signature);
    }

    /// <summary>
    /// 调用微信 POST API
    /// </summary>
    private async Task<dynamic> PostWxApiAsync(string url, object data)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json");

        // TODO: 添加签名头
        var response = await _httpClient.PostAsync($"https://api.mch.weixin.qq.com/{url}", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<dynamic>(json);
    }

    /// <summary>
    /// 调用微信 GET API
    /// </summary>
    private async Task<dynamic> GetWxApiAsync(string url)
    {
        // TODO: 添加签名头
        var response = await _httpClient.GetAsync($"https://api.mch.weixin.qq.com/{url}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<dynamic>(json);
    }

    /// <summary>
    /// 调用微信 CLOSE API
    /// </summary>
    private async Task CloseWxApiAsync(string url, object data)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json");

        // TODO: 添加签名头
        var response = await _httpClient.PostAsync($"https://api.mch.weixin.qq.com/{url}", content);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// 映射支付状态
    /// </summary>
    private PaymentStatus MapToPaymentStatus(PaymentDto payment)
    {
        return new PaymentStatus
        {
            PaymentId = payment.Id.ToString(),
            OrderId = payment.OrderId,
            Status = payment.Status.ToString().ToLower(),
            PayTime = payment.PaymentTime
        };
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Business && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Business/Mall/WxPayService.cs
git commit -m "feat(business): 添加微信支付服务实现"
```

---

### Task 14: 创建支付超时检查任务

**Files:**
- Create: `EasyProduct.Business/Mall/Jobs/PaymentTimeoutJob.cs`

- [ ] **Step 1: 创建支付超时检查任务**

```csharp
using Quartz;

namespace EasyProduct.Business.Mall.Jobs;

/// <summary>
/// 支付超时检查任务
/// </summary>
/// <remarks>
/// 每 5 分钟执行一次，检查超过 30 分钟未支付的订单并关闭
/// </remarks>
public class PaymentTimeoutJob : IJob
{
    private readonly IPaymentService _paymentService;
    private readonly IWxPayService _wxPayService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentTimeoutJob> _logger;

    public PaymentTimeoutJob(
        IPaymentService paymentService,
        IWxPayService wxPayService,
        IOrderService orderService,
        ILogger<PaymentTimeoutJob> logger)
    {
        _paymentService = paymentService;
        _wxPayService = wxPayService;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("开始检查超时支付单");

        try
        {
            // 1. 查询超过 30 分钟未支付的支付单
            var timeoutPayments = await _paymentService.GetTimeoutPaymentsAsync(30);

            if (timeoutPayments.Count == 0)
            {
                _logger.LogInformation("没有超时支付单");
                return;
            }

            // 2. 关闭超时订单
            var successCount = 0;
            var failCount = 0;

            foreach (var payment in timeoutPayments)
            {
                try
                {
                    await _wxPayService.CloseOrderAsync(payment.Id.ToString());
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "关闭支付单失败：{PaymentId}", payment.Id);
                    failCount++;
                }
            }

            _logger.LogInformation("检查超时支付单完成，成功关闭 {SuccessCount} 个，失败 {FailCount} 个", successCount, failCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查超时支付单异常");
        }
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Business && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Business/Mall/Jobs/PaymentTimeoutJob.cs
git commit -m "feat(business): 添加支付超时检查任务"
```

---

### Task 15: 注册微信支付服务和定时任务

**Files:**
- Modify: `EasyProduct.WebApi/Program.cs`

- [ ] **Step 1: 在 Program.cs 中注册微信支付服务**

```csharp
// 注册微信支付配置
builder.Services.Configure<WxPayOptions>(builder.Configuration.GetSection("WxPay"));

// 注册微信支付服务（带 HttpClient 和重试策略）
builder.Services.AddHttpClient<WxPayService>()
    .AddTransientHttpErrorPolicy(p => p
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))  // 指数退避：1s, 2s, 4s
    .AddTransientHttpErrorPolicy(p => p
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));  // 熔断器：连续失败 5 次后熔断 30 秒

// 注册定时任务
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.ScheduleJob<PaymentTimeoutJob>(trigger => trigger
        .WithIdentity("PaymentTimeoutTrigger")
        .WithCronSchedule("0 */5 * * * ?")  // 每 5 分钟执行一次
        .Build());
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.WebApi/Program.cs
git commit -m "feat(api): 注册微信支付服务和定时任务"
```

---

## 🎯 子系统 3：单元测试

### Task 16: 创建测试项目

**Files:**
- Create: `EasyProduct.Tests/EasyProduct.Tests.csproj`

- [ ] **Step 1: 创建测试项目文件**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="xunit" Version="2.7.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.7">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.1">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="Bogus" Version="35.6.1" />
    <PackageReference Include="SqlSugarCore" Version="5.1.4.154" />
    <PackageReference Include="System.Data.SQLite" Version="1.0.118" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\EasyProduct.Business\EasyProduct.Business.csproj" />
    <ProjectReference Include="..\EasyProduct.Models\EasyProduct.Models.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: 创建测试项目目录结构**

```bash
mkdir -p EasyProduct.Tests/TestBase
mkdir -p EasyProduct.Tests/TestData
mkdir -p EasyProduct.Tests/Business/Mall
mkdir -p EasyProduct.Tests/Infrastructure
```

- [ ] **Step 3: 验证测试项目创建成功**

Run: `cd EasyProduct.Tests && dotnet restore`
Expected: Restore succeeded

- [ ] **Step 4: 提交代码**

```bash
git add EasyProduct.Tests/EasyProduct.Tests.csproj
git commit -m "feat(tests): 创建单元测试项目"
```

---

### Task 17: 创建测试基类

**Files:**
- Create: `EasyProduct.Tests/TestBase/TestBase.cs`

- [ ] **Step 1: 创建测试基类**

```csharp
using EasyProduct.Business.Mall;
using EasyProduct.Business.Product;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Entitys.Product;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Data.SQLite;

namespace EasyProduct.Tests;

/// <summary>
/// 测试基类，提供统一的测试设置和清理
/// </summary>
public class TestBase : IDisposable
{
    protected readonly ISqlSugarClient _db;
    protected readonly IServiceProvider _serviceProvider;
    protected readonly TestDataFactory _dataFactory;

    public TestBase()
    {
        // 1. 配置 In-Memory SQLite
        var connection = new SQLiteConnection("DataSource=:memory:");
        connection.Open();

        _db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = "DataSource=:memory:",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = false,
            InitKeyType = InitKeyType.Attribute
        });

        // 2. 创建表结构
        _db.CodeFirst.InitTables(
            typeof(Order),
            typeof(OrderItem),
            typeof(Payment),
            typeof(Cart),
            typeof(Coupon),
            typeof(UserCoupon),
            typeof(PointRecord),
            typeof(PointRule),
            typeof(product_spu),
            typeof(product_sku),
            typeof(product_category)
        );

        // 3. 配置依赖注入
        var services = new ServiceCollection();

        // 注册 SqlSugar
        services.AddSingleton(_db);

        // 注册服务
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IPointService, PointService>();
        services.AddScoped<IProductQueryService, ProductQueryService>();

        // 注册测试数据工厂
        services.AddScoped<TestDataFactory>();

        _serviceProvider = services.BuildServiceProvider();
        _dataFactory = _serviceProvider.GetRequiredService<TestDataFactory>();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Tests && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Tests/TestBase/TestBase.cs
git commit -m "feat(tests): 创建测试基类"
```

---

### Task 18: 创建测试数据工厂

**Files:**
- Create: `EasyProduct.Tests/TestData/TestDataFactory.cs`

- [ ] **Step 1: 创建测试数据工厂**

```csharp
using Bogus;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using SqlSugar;

namespace EasyProduct.Tests;

/// <summary>
/// 测试数据工厂，简化测试数据创建
/// </summary>
public class TestDataFactory
{
    private readonly ISqlSugarClient _db;
    private readonly Faker _faker = new Faker("zh_CN");

    public TestDataFactory(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 创建订单
    /// </summary>
    public async Task<Order> CreateOrderAsync(Action<Order>? configure = null)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNo = GenerateOrderNo(),
            MemberId = Guid.NewGuid().ToString(),
            TotalAmount = _faker.Random.Decimal(100, 1000),
            PayAmount = _faker.Random.Decimal(100, 1000),
            FreightAmount = 0,
            DiscountAmount = 0,
            Status = OrderStatus.PendingPayment,
            ReceiverName = _faker.Name.FullName(),
            ReceiverPhone = _faker.Phone.PhoneNumber(),
            ReceiverAddress = _faker.Address.FullAddress(),
            CreatedAt = DateTime.UtcNow
        };

        configure?.Invoke(order);
        await _db.Insertable(order).ExecuteCommandAsync();
        return order;
    }

    /// <summary>
    /// 创建支付单
    /// </summary>
    public async Task<Payment> CreatePaymentAsync(Action<Payment>? configure = null)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            PaymentNo = GeneratePaymentNo(),
            OrderId = Guid.NewGuid().ToString(),
            MemberId = Guid.NewGuid().ToString(),
            Amount = _faker.Random.Decimal(100, 1000),
            Status = PaymentStatus.Pending,
            PaymentMethod = "wx_jsapi",
            CreatedAt = DateTime.UtcNow
        };

        configure?.Invoke(payment);
        await _db.Insertable(payment).ExecuteCommandAsync();
        return payment;
    }

    /// <summary>
    /// 创建购物车
    /// </summary>
    public async Task<Cart> CreateCartAsync(Action<Cart>? configure = null)
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            MemberId = Guid.NewGuid().ToString(),
            SkuId = Guid.NewGuid().ToString(),
            Quantity = _faker.Random.Int(1, 10),
            Selected = 1,
            CreatedAt = DateTime.UtcNow
        };

        configure?.Invoke(cart);
        await _db.Insertable(cart).ExecuteCommandAsync();
        return cart;
    }

    /// <summary>
    /// 创建优惠券
    /// </summary>
    public async Task<Coupon> CreateCouponAsync(Action<Coupon>? configure = null)
    {
        var coupon = new Coupon
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = CouponType.Discount,
            DiscountAmount = _faker.Random.Decimal(10, 100),
            MinOrderAmount = _faker.Random.Decimal(100, 500),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddDays(30),
            TotalCount = 100,
            UsedCount = 0,
            Status = CouponStatus.Enabled,
            CreatedAt = DateTime.UtcNow
        };

        configure?.Invoke(coupon);
        await _db.Insertable(coupon).ExecuteCommandAsync();
        return coupon;
    }

    /// <summary>
    /// 创建用户优惠券
    /// </summary>
    public async Task<UserCoupon> CreateUserCouponAsync(Action<UserCoupon>? configure = null)
    {
        var coupon = await CreateCouponAsync();

        var userCoupon = new UserCoupon
        {
            Id = Guid.NewGuid(),
            CouponId = coupon.Id.ToString(),
            MemberId = Guid.NewGuid().ToString(),
            Status = UserCouponStatus.Unused,
            StartTime = coupon.StartTime,
            EndTime = coupon.EndTime,
            CreatedAt = DateTime.UtcNow
        };

        configure?.Invoke(userCoupon);
        await _db.Insertable(userCoupon).ExecuteCommandAsync();
        return userCoupon;
    }

    /// <summary>
    /// 创建会员
    /// </summary>
    public async Task<Member> CreateMemberAsync(Action<Member>? configure = null)
    {
        var member = new Member
        {
            Id = Guid.NewGuid(),
            OpenId = Guid.NewGuid().ToString("N"),
            Nickname = _faker.Name.FullName(),
            Points = 0,
            TotalPoints = 0,
            Balance = 0,
            FrozenPoints = 0,
            Status = MemberStatus.Enabled,
            CreatedAt = DateTime.UtcNow
        };

        configure?.Invoke(member);
        await _db.Insertable(member).ExecuteCommandAsync();
        return member;
    }

    /// <summary>
    /// 创建积分规则
    /// </summary>
    public async Task<PointRule> CreatePointRuleAsync(Action<PointRule>? configure = null)
    {
        var rule = new PointRule
        {
            Id = Guid.NewGuid(),
            Name = _faker.Random.Word(),
            Type = PointRuleType.Order,
            Points = _faker.Random.Int(10, 100),
            IsMultiple = true,
            MultipleBase = 10,
            MaxPoints = 1000,
            Status = Status.Enabled,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddYears(1),
            Sort = 0,
            CreatedAt = DateTime.UtcNow
        };

        configure?.Invoke(rule);
        await _db.Insertable(rule).ExecuteCommandAsync();
        return rule;
    }

    // 私有方法
    private string GenerateOrderNo() => $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{_faker.Random.Int(1000, 9999)}";
    private string GeneratePaymentNo() => $"PAY{DateTime.UtcNow:yyyyMMddHHmmss}{_faker.Random.Int(1000, 9999)}";
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyProduct.Tests && dotnet build`
Expected: Build succeeded with 0 errors

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Tests/TestData/TestDataFactory.cs
git commit -m "feat(tests): 创建测试数据工厂"
```

---

### Task 19: 创建支付服务测试

**Files:**
- Create: `EasyProduct.Tests/Business/Mall/PaymentServiceTests.cs`

- [ ] **Step 1: 创建支付服务测试类**

```csharp
using EasyProduct.Models.Dto.Mall.Payment;
using EasyProduct.Models.Enums.Mall;
using Xunit;

namespace EasyProduct.Tests.Business.Mall;

/// <summary>
/// 支付服务测试
/// </summary>
public class PaymentServiceTests : TestBase
{
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
    }

    [Fact]
    public async Task CreatePayment_ValidOrder_ShouldCreatePayment()
    {
        // Arrange
        var order = await _dataFactory.CreateOrderAsync();
        var dto = new CreatePaymentDto
        {
            OrderId = order.Id.ToString(),
            MemberId = order.MemberId,
            Amount = order.PayAmount,
            PaymentMethod = "wx_jsapi"
        };

        // Act
        var result = await _paymentService.CreatePaymentAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id.ToString(), result.OrderId);
        Assert.Equal(order.PayAmount, result.Amount);
        Assert.Equal(PaymentStatus.Pending, result.Status);
    }

    [Fact]
    public async Task GetPayment_ExistingPayment_ShouldReturnPayment()
    {
        // Arrange
        var payment = await _dataFactory.CreatePaymentAsync();

        // Act
        var result = await _paymentService.GetPaymentAsync(payment.Id.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payment.Id, result.Id);
        Assert.Equal(payment.PaymentNo, result.PaymentNo);
    }

    [Fact]
    public async Task UpdatePaymentStatus_PendingToSuccess_ShouldUpdateStatus()
    {
        // Arrange
        var payment = await _dataFactory.CreatePaymentAsync();
        var transactionId = "WX123456789";

        // Act
        await _paymentService.UpdatePaymentStatusAsync(
            payment.Id.ToString(),
            PaymentStatus.Success,
            transactionId);

        // Assert
        var result = await _paymentService.GetPaymentAsync(payment.Id.ToString());
        Assert.Equal(PaymentStatus.Success, result.Status);
        Assert.Equal(transactionId, result.TransactionId);
    }

    [Fact]
    public async Task GetTimeoutPayments_TimeoutPayments_ShouldReturnList()
    {
        // Arrange
        var timeoutPayment = await _dataFactory.CreatePaymentAsync(p =>
        {
            p.CreatedAt = DateTime.UtcNow.AddMinutes(-40);
        });

        var recentPayment = await _dataFactory.CreatePaymentAsync(p =>
        {
            p.CreatedAt = DateTime.UtcNow.AddMinutes(-10);
        });

        // Act
        var result = await _paymentService.GetTimeoutPaymentsAsync(30);

        // Assert
        Assert.Single(result);
        Assert.Equal(timeoutPayment.Id, result[0].Id);
    }
}
```

- [ ] **Step 2: 运行测试验证通过**

Run: `cd EasyProduct.Tests && dotnet test --filter "PaymentServiceTests" -v n`
Expected: All tests passed

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Tests/Business/Mall/PaymentServiceTests.cs
git commit -m "feat(tests): 添加支付服务单元测试"
```

---

### Task 20: 创建订单服务测试

**Files:**
- Create: `EasyProduct.Tests/Business/Mall/OrderServiceTests.cs`

- [ ] **Step 1: 创建订单服务测试类**

```csharp
using EasyProduct.Models.Dto.Mall.Order;
using EasyProduct.Models.Enums.Mall;
using Xunit;

namespace EasyProduct.Tests.Business.Mall;

/// <summary>
/// 订单服务测试
/// </summary>
public class OrderServiceTests : TestBase
{
    private readonly IOrderService _orderService;

    public OrderServiceTests()
    {
        _orderService = _serviceProvider.GetRequiredService<IOrderService>();
    }

    [Fact]
    public async Task CreateOrder_ValidData_ShouldCreateOrder()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync();
        var dto = new CreateOrderDto
        {
            MemberId = member.Id.ToString(),
            Items = new List<OrderItemDto>
            {
                new() { SkuId = Guid.NewGuid().ToString(), Quantity = 2, Price = 99.99m }
            },
            ReceiverName = "测试用户",
            ReceiverPhone = "13800138000",
            ReceiverAddress = "测试地址"
        };

        // Act
        var result = await _orderService.CreateOrderAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(member.Id.ToString(), result.MemberId);
        Assert.Equal(OrderStatus.PendingPayment, result.Status);
    }

    [Fact]
    public async Task UpdateOrderStatus_PaymentPendingToPaid_ShouldUpdateStatus()
    {
        // Arrange
        var order = await _dataFactory.CreateOrderAsync();

        // Act
        await _orderService.UpdateOrderStatusAsync(
            order.Id.ToString(),
            OrderStatus.PendingDelivery);

        // Assert
        var result = await _orderService.GetOrderDetailAsync(order.Id.ToString());
        Assert.Equal(OrderStatus.PendingDelivery, result.Status);
    }

    [Fact]
    public async Task CancelOrder_PendingOrder_ShouldCancel()
    {
        // Arrange
        var order = await _dataFactory.CreateOrderAsync();

        // Act
        await _orderService.UpdateOrderStatusAsync(
            order.Id.ToString(),
            OrderStatus.Cancelled);

        // Assert
        var result = await _orderService.GetOrderDetailAsync(order.Id.ToString());
        Assert.Equal(OrderStatus.Cancelled, result.Status);
    }
}
```

- [ ] **Step 2: 运行测试验证通过**

Run: `cd EasyProduct.Tests && dotnet test --filter "OrderServiceTests" -v n`
Expected: All tests passed

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Tests/Business/Mall/OrderServiceTests.cs
git commit -m "feat(tests): 添加订单服务单元测试"
```

---

### Task 21: 创建购物车服务测试

**Files:**
- Create: `EasyProduct.Tests/Business/Mall/CartServiceTests.cs`

- [ ] **Step 1: 创建购物车服务测试类**

```csharp
using EasyProduct.Models.Dto.Mall.Cart;
using Xunit;

namespace EasyProduct.Tests.Business.Mall;

/// <summary>
/// 购物车服务测试
/// </summary>
public class CartServiceTests : TestBase
{
    private readonly ICartService _cartService;

    public CartServiceTests()
    {
        _cartService = _serviceProvider.GetRequiredService<ICartService>();
    }

    [Fact]
    public async Task AddToCart_NewItem_ShouldAddToCart()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync();
        var dto = new CartAddDto
        {
            MemberId = member.Id.ToString(),
            SkuId = Guid.NewGuid().ToString(),
            Quantity = 2
        };

        // Act
        var result = await _cartService.AddToCartAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Quantity, result.Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_ExistingItem_ShouldUpdateQuantity()
    {
        // Arrange
        var cart = await _dataFactory.CreateCartAsync();
        var dto = new CartUpdateQuantityDto
        {
            CartId = cart.Id.ToString(),
            Quantity = 5
        };

        // Act
        var result = await _cartService.UpdateQuantityAsync(dto);

        // Assert
        Assert.Equal(dto.Quantity, result.Quantity);
    }

    [Fact]
    public async Task RemoveFromCart_ExistingItem_ShouldRemove()
    {
        // Arrange
        var cart = await _dataFactory.CreateCartAsync();

        // Act
        await _cartService.RemoveFromCartAsync(cart.Id.ToString());

        // Assert
        var result = await _cartService.GetCartListAsync(cart.MemberId);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ClearCart_MultipleItems_ShouldClearAll()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync();
        await _dataFactory.CreateCartAsync(c => c.MemberId = member.Id.ToString());
        await _dataFactory.CreateCartAsync(c => c.MemberId = member.Id.ToString());

        // Act
        await _cartService.ClearCartAsync(member.Id.ToString());

        // Assert
        var result = await _cartService.GetCartListAsync(member.Id.ToString());
        Assert.Empty(result);
    }
}
```

- [ ] **Step 2: 运行测试验证通过**

Run: `cd EasyProduct.Tests && dotnet test --filter "CartServiceTests" -v n`
Expected: All tests passed

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Tests/Business/Mall/CartServiceTests.cs
git commit -m "feat(tests): 添加购物车服务单元测试"
```

---

### Task 22: 创建优惠券服务测试

**Files:**
- Create: `EasyProduct.Tests/Business/Mall/CouponServiceTests.cs`

- [ ] **Step 1: 创建优惠券服务测试类**

```csharp
using EasyProduct.Models.Dto.Mall.Coupon;
using EasyProduct.Models.Enums.Mall;
using Xunit;

namespace EasyProduct.Tests.Business.Mall;

/// <summary>
/// 优惠券服务测试
/// </summary>
public class CouponServiceTests : TestBase
{
    private readonly ICouponService _couponService;

    public CouponServiceTests()
    {
        _couponService = _serviceProvider.GetRequiredService<ICouponService>();
    }

    [Fact]
    public async Task ReceiveCoupon_ValidCoupon_ShouldReceive()
    {
        // Arrange
        var coupon = await _dataFactory.CreateCouponAsync();
        var member = await _dataFactory.CreateMemberAsync();
        var dto = new ReceiveCouponDto
        {
            CouponId = coupon.Id.ToString(),
            MemberId = member.Id.ToString()
        };

        // Act
        var result = await _couponService.ReceiveCouponAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(coupon.Id.ToString(), result.CouponId);
        Assert.Equal(member.Id.ToString(), result.MemberId);
        Assert.Equal(UserCouponStatus.Unused, result.Status);
    }

    [Fact]
    public async Task UseCoupon_UnusedCoupon_ShouldUse()
    {
        // Arrange
        var userCoupon = await _dataFactory.CreateUserCouponAsync();
        var order = await _dataFactory.CreateOrderAsync();

        // Act
        await _couponService.UseCouponAsync(userCoupon.Id.ToString(), order.Id.ToString());

        // Assert
        var result = await _couponService.GetUserCouponAsync(userCoupon.Id.ToString());
        Assert.Equal(UserCouponStatus.Used, result.Status);
    }

    [Fact]
    public async Task GetAvailableCoupons_MemberWithCoupons_ShouldReturnList()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync();
        await _dataFactory.CreateUserCouponAsync(u => u.MemberId = member.Id.ToString());
        await _dataFactory.CreateUserCouponAsync(u =>
        {
            u.MemberId = member.Id.ToString();
            u.Status = UserCouponStatus.Used;
        });

        // Act
        var result = await _couponService.GetAvailableCouponsAsync(member.Id.ToString());

        // Assert
        Assert.Single(result);
        Assert.Equal(UserCouponStatus.Unused, result[0].Status);
    }
}
```

- [ ] **Step 2: 运行测试验证通过**

Run: `cd EasyProduct.Tests && dotnet test --filter "CouponServiceTests" -v n`
Expected: All tests passed

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Tests/Business/Mall/CouponServiceTests.cs
git commit -m "feat(tests): 添加优惠券服务单元测试"
```

---

### Task 23: 创建积分服务测试

**Files:**
- Create: `EasyProduct.Tests/Business/Mall/PointServiceTests.cs`

- [ ] **Step 1: 创建积分服务测试类**

```csharp
using EasyProduct.Models.Dto.Mall.Point;
using EasyProduct.Models.Enums.Mall;
using Xunit;

namespace EasyProduct.Tests.Business.Mall;

/// <summary>
/// 积分服务测试
/// </summary>
public class PointServiceTests : TestBase
{
    private readonly IPointService _pointService;

    public PointServiceTests()
    {
        _pointService = _serviceProvider.GetRequiredService<IPointService>();
    }

    [Fact]
    public async Task AddPoints_ValidMember_ShouldAddPoints()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync();
        var dto = new AddPointDto
        {
            MemberId = member.Id.ToString(),
            Points = 100,
            Source = PointSource.Order,
            Remark = "测试积分"
        };

        // Act
        await _pointService.AddPointsAsync(dto);

        // Assert
        var result = await _pointService.GetPointBalanceAsync(member.Id.ToString());
        Assert.Equal(100, result.AvailablePoints);
    }

    [Fact]
    public async Task ConsumePoints_EnoughPoints_ShouldConsume()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync(m => m.Points = 200);
        var dto = new ConsumePointDto
        {
            MemberId = member.Id.ToString(),
            Points = 100,
            Source = PointSource.Exchange,
            Remark = "兑换商品"
        };

        // Act
        await _pointService.ConsumePointsAsync(dto);

        // Assert
        var result = await _pointService.GetPointBalanceAsync(member.Id.ToString());
        Assert.Equal(100, result.AvailablePoints);
    }

    [Fact]
    public async Task FreezePoints_EnoughPoints_ShouldFreeze()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync(m => m.Points = 200);
        var dto = new FreezePointDto
        {
            MemberId = member.Id.ToString(),
            Points = 100,
            Source = PointSource.Order,
            Remark = "订单积分冻结"
        };

        // Act
        await _pointService.FreezePointsAsync(dto);

        // Assert
        var result = await _pointService.GetPointBalanceAsync(member.Id.ToString());
        Assert.Equal(100, result.AvailablePoints);
        Assert.Equal(100, result.FrozenPoints);
    }

    [Fact]
    public async Task UnfreezePoints_FrozenPoints_ShouldUnfreeze()
    {
        // Arrange
        var member = await _dataFactory.CreateMemberAsync(m =>
        {
            m.Points = 100;
            m.FrozenPoints = 100;
        });
        var dto = new UnfreezePointDto
        {
            MemberId = member.Id.ToString(),
            Points = 50,
            Source = PointSource.Order,
            Remark = "订单积分解冻"
        };

        // Act
        await _pointService.UnfreezePointsAsync(dto);

        // Assert
        var result = await _pointService.GetPointBalanceAsync(member.Id.ToString());
        Assert.Equal(150, result.AvailablePoints);
        Assert.Equal(50, result.FrozenPoints);
    }
}
```

- [ ] **Step 2: 运行测试验证通过**

Run: `cd EasyProduct.Tests && dotnet test --filter "PointServiceTests" -v n`
Expected: All tests passed

- [ ] **Step 3: 提交代码**

```bash
git add EasyProduct.Tests/Business/Mall/PointServiceTests.cs
git commit -m "feat(tests): 添加积分服务单元测试"
```

---

### Task 24: 运行所有测试并生成覆盖率报告

**Files:**
- Test: `EasyProduct.Tests/`

- [ ] **Step 1: 运行所有测试**

Run: `cd EasyProduct.Tests && dotnet test --verbosity normal`
Expected: All tests passed

- [ ] **Step 2: 生成测试覆盖率报告**

Run: `cd EasyProduct.Tests && dotnet test --collect:"XPlat Code Coverage"`
Expected: Coverage report generated

- [ ] **Step 3: 提交代码**

```bash
git add .
git commit -m "feat(tests): 完成单元测试，生成覆盖率报告"
```

---

## 📊 实施总结

### 依赖关系图

```
Task 1-5 (DTO 创建)
    ↓
Task 6-7 (服务接口和实现)
    ↓
Task 8-9 (控制器和限流配置)
    ↓
Task 10-15 (微信支付集成)
    ↓
Task 16-24 (单元测试)
```

### 预估工作量

- **子系统 1（会员端商品控制器）**：2-3 小时
- **子系统 2（微信支付集成）**：3-4 小时
- **子系统 3（单元测试）**：2-3 小时
- **总计**：7-10 小时

### 关键检查点

1. ✅ DTO 编译通过（Task 1-5 完成后）
2. ✅ 服务层编译通过（Task 7 完成后）
3. ✅ 控制器编译通过（Task 8 完成后）
4. ✅ 微信支付服务编译通过（Task 13 完成后）
5. ✅ 测试项目编译通过（Task 17 完成后）
6. ✅ 所有测试通过（Task 24 完成后）

### 风险提示

1. **实体类不存在**：需要检查 `Order`、`Payment`、`Cart` 等实体类是否已定义
2. **现有服务接口不存在**：需要检查 `ISpuService`、`ICategoryService` 等是否已存在
3. **PaymentStatus 枚举不存在**：需要先定义相关枚举
4. **数据库表结构不一致**：需要确保测试实体与数据库表结构一致

### 后续优化建议

1. **商品查询服务实现**：Task 7 中的 TODO 部分需要根据现有商品表结构完善
2. **微信支付签名验证**：Task 13 中的签名验证逻辑需要实现
3. **缓存优化**：热销商品、新品推荐可添加缓存
4. **性能测试**：添加并发测试和性能测试
5. **集成测试**：添加 API 集成测试

---

**计划创建完成。下一步将进行实施。**