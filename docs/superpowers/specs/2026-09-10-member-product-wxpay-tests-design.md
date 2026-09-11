# 会员端商品控制器、微信支付集成、单元测试设计文档

**日期**：2026-09-10
**状态**：设计完成，待实施
**作者**：Claude

---

## 📋 概述

本文档定义了三个核心任务的设计方案：

1. **会员端商品控制器**：为小程序提供完整的商品浏览功能 2026-09-10-member-product-controller.md
2. **微信支付集成**：实现生产就绪的微信支付流程 2026-09-10-wxpay-integration.md
3. **单元测试**：保证核心业务逻辑的质量和可靠性 2026-09-10-unit-tests.md

---

## 🎯 任务 1：会员端商品控制器

### 1.1 业务需求

小程序需要完整的商品浏览功能，包括：

- ✅ 商品列表（支持筛选、排序、分页）
- ✅ 商品详情（完整信息展示）
- ✅ 商品分类（树形结构）
- ✅ 搜索商品（关键词搜索）
- ✅ 热销商品（销量排行）
- ✅ 新品推荐（创建时间排序）
- ❌ 商品评价（后续迭代）

### 1.2 架构设计

```
小程序前端
    ↓ HTTP 请求
会员端控制器层
    ├── ProductController (新增)
    │   ├── GetList() - 商品列表
    │   ├── GetDetail() - 商品详情
    │   ├── GetCategories() - 商品分类
    │   ├── Search() - 搜索商品
    │   ├── GetHot() - 热销商品
    │   └── GetNew() - 新品推荐
    ↓ 调用
业务服务层
    ├── ISpuService (已有)
    ├── ICategoryService (已有)
    ├── ISkuService (已有)
    └── IProductQueryService (新增，封装查询逻辑)
    ↓ 访问
数据层
    └── SqlSugar ORM
```

### 1.3 数据模型设计

#### 1.3.1 DTO 定义

**文件路径**：`EasyProduct.Models/Dto/Product/App/`

```csharp
// 商品查询参数
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

// 商品列表 DTO
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

// 商品详情 DTO
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

// SKU DTO
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

// 规格项 DTO
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

// 规格模板 DTO
public class SpecTemplateDto
{
    /// <summary>
    /// 规格列表
    /// </summary>
    public List<SpecDefinitionDto> Specs { get; set; } = new();
}

// 规格定义 DTO
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

// 销量统计 DTO
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

// 分类树 DTO
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

### 1.4 控制器设计

**文件路径**：`EasyProduct.Web/Controllers/App/Product/ProductController.cs`

```csharp
using EasyProduct.Web.Controllers.App.Base;
using EasyProduct.Business.Product;
using EasyProduct.Models.Dto.Product.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyProduct.Web.Controllers.App.Product;

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

### 1.5 服务接口设计

**文件路径**：`EasyProduct.Business/Product/IProductQueryService.cs`

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

### 1.6 限流配置

**文件路径**：`EasyProduct.Web/Program.cs`

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

// 使用限流中间件
app.UseRateLimiter();
```

### 1.7 实现要点

1. **复用现有服务**：`IProductQueryService` 封装 `ISpuService`、`ICategoryService`、`ISkuService` 的调用，避免重复代码
2. **数据聚合**：商品详情需要关联查询 SKU、分类、统计信息，在一次查询中完成
3. **性能优化**：
   - 热销和新品商品可缓存 5 分钟
   - 分类树可缓存 10 分钟
   - 使用 `AsNoTracking` 提升查询性能
4. **异常处理**：
   - 商品不存在返回 404
   - 其他异常由全局中间件处理
   - 记录详细日志便于排查
5. **公开访问**：所有商品 API 无需登录，使用 IP 限流防止滥用

---

## 🎯 任务 2：微信支付集成

### 2.1 业务需求

实现生产就绪的微信支付功能，包括：

- ✅ JSAPI 支付（小程序支付）
- ✅ 支付回调处理（签名验证、状态更新、幂等性）
- ✅ 支付状态查询
- ✅ 订单超时自动关闭
- ✅ 完整的错误处理和日志记录
- ✅ 配置管理（环境变量 + 配置文件）

### 2.2 配置设计

#### 2.2.1 配置类

**文件路径**：`EasyProduct.Models/Options/WxPayOptions.cs`

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

#### 2.2.2 配置文件示例

**文件路径**：`EasyProduct.Web/appsettings.json`

```json
{
  "WxPay": {
    "Enabled": true,
    "AppId": "wx1234567890abcdef",
    "MchId": "${WX_PAY_MCH_ID}",
    "ApiKey": "${WX_PAY_API_KEY}",
    "CertSerialNo": "${WX_PAY_CERT_SERIAL}",
    "CertPath": "certs/apiclient_key.pem",
    "NotifyUrl": "https://api.example.com/api/app/payment/callback",
    "ExpireMinutes": 30
  }
}
```

#### 2.2.3 环境变量配置

**Windows (PowerShell)**：
```powershell
$env:WX_PAY_MCH_ID = "1234567890"
$env:WX_PAY_API_KEY = "your_api_key_here"
$env:WX_PAY_CERT_SERIAL = "your_cert_serial_here"
```

**Linux/Mac**：
```bash
export WX_PAY_MCH_ID="1234567890"
export WX_PAY_API_KEY="your_api_key_here"
export WX_PAY_CERT_SERIAL="your_cert_serial_here"
```

### 2.3 服务接口设计

**文件路径**：`EasyProduct.Business/Mall/IWxPayService.cs`

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

### 2.4 服务实现设计

**文件路径**：`EasyProduct.Business/Mall/WxPayService.cs`

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

### 2.5 容错机制设计

#### 2.5.1 HTTP 重试策略

**文件路径**：`EasyProduct.Web/Program.cs`

```csharp
// 使用 Polly 添加重试策略
using Polly;
using Polly.Extensions.Http;

builder.Services.AddHttpClient<WxPayService>()
    .AddTransientHttpErrorPolicy(p => p
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))  // 指数退避：1s, 2s, 4s
    .AddTransientHttpErrorPolicy(p => p
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));  // 熔断器：连续失败 5 次后熔断 30 秒
```

#### 2.5.2 支付超时检查定时任务

**文件路径**：`EasyProduct.Business/Mall/Jobs/PaymentTimeoutJob.cs`

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

**定时任务注册**：

```csharp
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

#### 2.5.3 幂等性处理

**数据库唯一索引**（支付回调日志表）：

```sql
CREATE TABLE mall_payment_callback_log (
    id VARCHAR(36) PRIMARY KEY,
    payment_no VARCHAR(50) NOT NULL,
    transaction_id VARCHAR(50),
    callback_data TEXT,
    callback_time DATETIME NOT NULL,
    UNIQUE KEY uk_payment_no (payment_no)  -- 唯一索引保证幂等性
);
```

**幂等性处理代码**：

```csharp
public async Task<bool> HandlePayCallbackAsync(string callbackData)
{
    var callback = ParseCallbackData(callbackData);

    // 使用数据库唯一索引保证幂等性
    try
    {
        await _paymentService.CreatePaymentCallbackLogAsync(new PaymentCallbackLogDto
        {
            PaymentNo = callback.out_trade_no,
            TransactionId = callback.transaction_id,
            CallbackData = callbackData,
            CallbackTime = DateTime.UtcNow
        });
    }
    catch (UniqueConstraintException)
    {
        // 回调已处理，直接返回成功
        _logger.LogWarning("支付回调重复，支付单：{PaymentNo}", callback.out_trade_no);
        return true;
    }

    // 处理支付逻辑...
}
```

### 2.6 错误处理

**自定义异常类**：

```csharp
/// <summary>
/// 微信支付异常
/// </summary>
public class WxPayException : Exception
{
    /// <summary>
    /// 错误码
    /// </summary>
    public string ErrorCode { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; }

    public WxPayException(string errorCode, string message)
        : base($"微信支付错误：{errorCode} - {message}")
    {
        ErrorCode = errorCode;
        ErrorMessage = message;
    }
}
```

**错误处理示例**：

```csharp
public async Task<JsapiPayParams> CreateJsapiOrderAsync(...)
{
    try
    {
        // 调用微信 API
        var response = await PostWxApiAsync(...);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new WxPayException("API_ERROR", error);
        }

        return GeneratePayParams(...);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "微信支付网络请求失败");
        throw new BusinessException("支付服务暂时不可用，请稍后重试");
    }
    catch (WxPayException ex)
    {
        _logger.LogError(ex, "微信支付接口错误");
        throw new BusinessException($"支付失败：{ex.ErrorMessage}");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "微信支付未知错误");
        throw new BusinessException("支付失败，请联系客服");
    }
}
```

### 2.7 实现要点

1. **证书管理**：
   - 证书文件放在 `certs/` 目录
   - 使用 `.gitignore` 排除证书文件
   - 生产环境从安全存储加载证书

2. **签名验证**：
   - 使用微信提供的签名验证算法
   - 确保回调来自微信服务器

3. **超时处理**：
   - 定时任务每 5 分钟检查一次
   - 超过 30 分钟未支付的订单自动关闭

4. **日志记录**：
   - 所有支付操作记录详细日志
   - 包含：订单号、支付单号、金额、状态、时间、错误信息
   - 便于排查问题和财务对账

5. **开发环境**：
   - `Enabled = false` 时使用模拟支付
   - 返回模拟的支付参数
   - 不影响本地开发和测试

---

## 🎯 任务 3：单元测试

### 3.1 测试范围

核心业务逻辑 + 支付流程，包括：

- ✅ 支付服务测试（创建、更新、查询）
- ✅ 订单服务测试（创建、支付、发货、完成、取消）
- ✅ 购物车服务测试（添加、修改、删除、结算）
- ✅ 优惠券服务测试（领取、使用、过期）
- ✅ 积分服务测试（发放、消费、冻结、解冻）

### 3.2 测试框架

- **测试框架**：xUnit
- **模拟框架**：Moq
- **数据库**：In-Memory SQLite
- **数据生成**：Bogus

### 3.3 项目结构

```
EasyProduct.Tests/
├── EasyProduct.Tests.csproj
├── TestBase/
│   └── TestBase.cs                    # 测试基类
├── TestData/
│   ├── TestDataFactory.cs             # 测试数据工厂
│   └── TestDataGenerator.cs           # 数据生成器（使用 Bogus）
├── Business/
│   ├── Mall/
│   │   ├── PaymentServiceTests.cs     # 支付服务测试
│   │   ├── OrderServiceTests.cs       # 订单服务测试
│   │   ├── CartServiceTests.cs        # 购物车服务测试
│   │   ├── CouponServiceTests.cs      # 优惠券服务测试
│   │   └── PointServiceTests.cs       # 积分服务测试
│   └── Product/
│       └── ProductServiceTests.cs     # 商品服务测试
└── Infrastructure/
    └── DatabaseFixture.cs             # 数据库测试固件
```

### 3.4 测试基类

**文件路径**：`EasyProduct.Tests/TestBase/TestBase.cs`

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

### 3.5 测试数据工厂

**文件路径**：`EasyProduct.Tests/TestData/TestDataFactory.cs`

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

### 3.6 测试覆盖率目标

| 模块 | 目标覆盖率 | 说明 |
|------|-----------|------|
| 支付服务 | 90% | 核心资金流程 |
| 订单服务 | 85% | 核心业务流程 |
| 购物车服务 | 80% | 基础功能 |
| 优惠券服务 | 85% | 资金相关 |
| 积分服务 | 85% | 资金相关 |

### 3.7 实现要点

1. **测试隔离**：
   - 每个测试类使用独立的 In-Memory 数据库
   - 测试之间互不影响
   - 可以并行执行

2. **数据生成**：
   - 使用 Bogus 库生成随机数据
   - 更接近真实场景
   - 减少硬编码

3. **并发测试**：
   - 使用 `Parallel.For` 测试并发场景
   - 如库存扣减、积分扣减
   - 验证并发安全性

4. **边界测试**：
   - 测试边界条件（库存不足、积分不足、优惠券过期）
   - 测试异常分支
   - 确保错误处理正确

5. **命名规范**：
   - 测试方法命名：`方法名_场景_期望结果`
   - 如：`CreatePayment_ValidOrder_ShouldCreatePayment`
   - 清晰表达测试意图

---

## 📊 实施计划

### 优先级和依赖关系

```
任务 1：会员端商品控制器
    ↓ （无依赖）
    
任务 2：微信支付集成
    ↓ （依赖：任务 1 的会员端基础设施）
    
任务 3：单元测试
    ↓ （依赖：任务 1 和任务 2 的代码）
```

### 预估工作量

| 任务 | 预估时间 | 关键路径 |
|------|---------|---------|
| 任务 1：会员端商品控制器 | 4-6 小时 | DTO 设计、服务实现、控制器实现 |
| 任务 2：微信支付集成 | 6-8 小时 | 配置管理、服务实现、容错机制 |
| 任务 3：单元测试 | 4-6 小时 | 测试基础设施、测试用例编写 |
| **总计** | **14-20 小时** | - |

### 风险和缓解措施

| 风险 | 影响 | 缓解措施 |
|------|------|---------|
| 微信支付证书配置错误 | 支付失败 | 提供详细的配置文档和检查清单 |
| 测试数据库与生产环境差异 | 测试不可靠 | 使用相同的 ORM 和迁移脚本 |
| 限流配置不当 | 用户体验差 | 提供合理的默认值和监控告警 |
| 并发问题 | 数据不一致 | 添加单元测试验证并发场景 |

---

## 📝 相关文档

- [后端开发规范](/docs/backend-guidelines.md)
- [前端开发规范](/docs/frontend-guidelines.md)
- [整合设计方案](/docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md)
- [微信支付官方文档](https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_5_1.shtml)

---

## ✅ 下一步

本设计文档已完成，下一步将创建详细的实施计划（writing-plans）。

**请审阅本设计文档，如有需要调整的地方请告知。**