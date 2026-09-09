# P3.2 购物车子系统实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现完整的购物车功能，包括小程序端会员使用和 Admin 后台管理，支持添加、修改、删除、查询、批量操作、失效商品处理等核心功能。

**Architecture:** 采用标准三层架构（Entity → Service → Controller），后端使用 .NET 8 + SqlSugar ORM，数据持久化到 MySQL 数据库的 mall_cart 表。

**Tech Stack:** .NET 8.0, SqlSugar 5.1.4, Mapster 10.x, xUnit (可选测试)

---

## 文件结构

```
EasyProduct.WebApi/
├── EasyProduct.Models/
│   ├── Entitys/Mall/
│   │   └── Cart.cs                          # 购物车实体类
│   ├── Dto/Mall/Cart/
│   │   ├── CartQuery.cs                     # 查询参数
│   │   ├── CartItemDto.cs                   # 购物车项DTO
│   │   ├── CartListDto.cs                   # 购物车列表DTO（含统计）
│   │   ├── CartAddDto.cs                    # 添加到购物车DTO
│   │   ├── CartBatchAddDto.cs               # 批量添加DTO
│   │   ├── CartUpdateQuantityDto.cs         # 修改数量DTO
│   │   ├── CartUpdateSelectedDto.cs         # 修改选中状态DTO
│   │   ├── CartChangeSkuDto.cs              # 切换SKU规格DTO
│   │   ├── CartStatisticsDto.cs             # 购物车统计数据DTO
│   │   └── HotProductDto.cs                 # 热门商品DTO
│   └── Enums/Mall/
│       └── CartSelectedStatus.cs            # 购物车选中状态枚举
├── EasyProduct.Business/Mall/
│   ├── ICartService.cs                      # 购物车服务接口
│   └── CartService.cs                       # 购物车服务实现
└── EasyProduct.Web/Controllers/
    ├── App/Mall/CartController.cs           # 小程序端购物车控制器
    └── Admin/Mall/CartController.cs         # 管理端购物车控制器
```

---

## 阶段 1：数据模型层

### Task 1: 创建购物车实体类

**Files:**
- Create: `EasyProduct.Models/Entitys/Mall/Cart.cs`

- [ ] **Step 1: 创建 Cart 实体类**

```csharp
using System.ComponentModel.DataAnnotations;
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 购物车实体
/// </summary>
[SugarTable("mall_cart", "购物车表")]
public class Cart : BaseEntity
{
    /// <summary>
    /// 会员ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    [Required]
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    [SugarColumn(Length = 36, IsNullable = false)]
    [Required]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// 是否选中：0=否，1=是
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Selected { get; set; } = 1;
}
```

- [ ] **Step 2: 验证实体类创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 2: 创建购物车选中状态枚举

**Files:**
- Create: `EasyProduct.Models/Enums/Mall/CartSelectedStatus.cs`

- [ ] **Step 1: 创建购物车选中状态枚举**

```csharp
namespace EasyProduct.Models.Enums.Mall;

/// <summary>
/// 购物车选中状态
/// </summary>
public enum CartSelectedStatus
{
    /// <summary>未选中</summary>
    Unselected = 0,

    /// <summary>已选中</summary>
    Selected = 1
}
```

- [ ] **Step 2: 验证枚举创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 3: 创建购物车查询 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartQuery.cs`

- [ ] **Step 1: 创建 CartQuery 查询参数**

```csharp
using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Dto.Common;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车查询参数
/// </summary>
/// <remarks>
/// 用于购物车列表查询（Admin端），支持按会员ID、关键词、选中状态筛选
/// 包含分页参数（继承自 PageQuery）
/// </remarks>
public class CartQuery : PageQuery
{
    /// <summary>
    /// 会员ID（Admin 端使用）
    /// </summary>
    /// <remarks>
    /// 精确匹配，GUID 格式
    /// </remarks>
    [StringLength(36, ErrorMessage = "会员ID长度不能超过36个字符")]
    public string? MemberId { get; set; }

    /// <summary>
    /// 关键词（商品名称/SKU编码）
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "关键词长度不能超过200个字符")]
    public string? Keyword { get; set; }

    /// <summary>
    /// 是否只查询选中商品
    /// </summary>
    /// <remarks>
    /// 0=未选中，1=已选中
    /// </remarks>
    public int? Selected { get; set; }
}
```

- [ ] **Step 2: 验证 DTO 创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 4: 创建购物车项 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartItemDto.cs`

- [ ] **Step 1: 创建 CartItemDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车项DTO（含商品详情）
/// </summary>
/// <remarks>
/// 用于返回购物车中的单个商品项，包含商品信息、价格、库存、失效状态等完整信息
/// </remarks>
public class CartItemDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// SPU ID
    /// </summary>
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// SKU编码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 规格组合（如："红色 / XL"）
    /// </summary>
    public string SpecText { get; set; } = string.Empty;

    /// <summary>
    /// 主图URL
    /// </summary>
    public string MainImage { get; set; } = string.Empty;

    /// <summary>
    /// 零售价
    /// </summary>
    public decimal RetailPrice { get; set; }

    /// <summary>
    /// 会员价（如有）
    /// </summary>
    public decimal? MemberPrice { get; set; }

    /// <summary>
    /// 实际售价（会员价优先）
    /// </summary>
    public decimal ActualPrice { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 小计金额
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 是否有库存
    /// </summary>
    public bool InStock { get; set; }

    /// <summary>
    /// SKU状态：0=禁用，1=启用
    /// </summary>
    public int SkuStatus { get; set; }

    /// <summary>
    /// 是否失效（SKU禁用或库存不足）
    /// </summary>
    public bool IsInvalid { get; set; }

    /// <summary>
    /// 失效原因
    /// </summary>
    public string? InvalidReason { get; set; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public int Selected { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
```

- [ ] **Step 2: 验证 DTO 创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 5: 创建购物车列表 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartListDto.cs`

- [ ] **Step 1: 创建 CartListDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车列表DTO（含统计信息）
/// </summary>
/// <remarks>
/// 用于返回会员购物车完整信息，包含购物车项列表和统计信息（总数量、选中数量、总金额、失效商品数量）
/// </remarks>
public class CartListDto
{
    /// <summary>
    /// 购物车项列表
    /// </summary>
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// 总数量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 选中数量
    /// </summary>
    public int SelectedCount { get; set; }

    /// <summary>
    /// 总金额（仅选中商品）
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 失效商品数量
    /// </summary>
    public int InvalidCount { get; set; }
}
```

- [ ] **Step 2: 验证 DTO 创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 6: 创建购物车操作 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartAddDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartBatchAddDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartUpdateQuantityDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartUpdateSelectedDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartChangeSkuDto.cs`

- [ ] **Step 1: 创建 CartAddDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 添加到购物车DTO
/// </summary>
/// <remarks>
/// 用于添加商品到购物车，包含 SKU ID 和数量
/// </remarks>
public class CartAddDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    [Required(ErrorMessage = "SKU ID不能为空")]
    [StringLength(36, ErrorMessage = "SKU ID长度不能超过36个字符")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; } = 1;
}
```

- [ ] **Step 2: 创建 CartBatchAddDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 批量添加到购物车DTO
/// </summary>
/// <remarks>
/// 用于批量添加商品到购物车，包含多个购物车项
/// </remarks>
public class CartBatchAddDto
{
    /// <summary>
    /// 购物车项列表
    /// </summary>
    [Required(ErrorMessage = "购物车项列表不能为空")]
    public List<CartAddDto> Items { get; set; } = new();
}
```

- [ ] **Step 3: 创建 CartUpdateQuantityDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 修改购物车数量DTO
/// </summary>
/// <remarks>
/// 用于修改购物车中商品的数量
/// </remarks>
public class CartUpdateQuantityDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    [Required(ErrorMessage = "购物车ID不能为空")]
    [StringLength(36, ErrorMessage = "购物车ID长度不能超过36个字符")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 新数量
    /// </summary>
    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }
}
```

- [ ] **Step 4: 创建 CartUpdateSelectedDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 修改选中状态DTO
/// </summary>
/// <remarks>
/// 用于批量修改购物车商品的选中状态
/// </remarks>
public class CartUpdateSelectedDto
{
    /// <summary>
    /// 购物车ID列表
    /// </summary>
    [Required(ErrorMessage = "购物车ID列表不能为空")]
    public List<string> Ids { get; set; } = new();

    /// <summary>
    /// 是否选中：0=否，1=是
    /// </summary>
    [Required(ErrorMessage = "选中状态不能为空")]
    [Range(0, 1, ErrorMessage = "选中状态只能为0或1")]
    public int Selected { get; set; }
}
```

- [ ] **Step 5: 创建 CartChangeSkuDto**

```csharp
using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 切换SKU规格DTO
/// </summary>
/// <remarks>
/// 用于在购物车中切换商品的规格（SKU）
/// </remarks>
public class CartChangeSkuDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    [Required(ErrorMessage = "购物车ID不能为空")]
    [StringLength(36, ErrorMessage = "购物车ID长度不能超过36个字符")]
    public string CartId { get; set; } = string.Empty;

    /// <summary>
    /// 新SKU ID
    /// </summary>
    [Required(ErrorMessage = "新SKU ID不能为空")]
    [StringLength(36, ErrorMessage = "SKU ID长度不能超过36个字符")]
    public string NewSkuId { get; set; } = string.Empty;
}
```

- [ ] **Step 6: 验证所有操作 DTO 创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 7: 创建购物车统计 DTO

**Files:**
- Create: `EasyProduct.Models/Dto/Mall/Cart/CartStatisticsDto.cs`
- Create: `EasyProduct.Models/Dto/Mall/Cart/HotProductDto.cs`

- [ ] **Step 1: 创建 HotProductDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 热门商品DTO
/// </summary>
/// <remarks>
/// 用于购物车统计中展示热门商品信息
/// </remarks>
public class HotProductDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 出现在购物车中的次数
    /// </summary>
    public int CartCount { get; set; }
}
```

- [ ] **Step 2: 创建 CartStatisticsDto**

```csharp
namespace EasyProduct.Models.Dto.Mall.Cart;

/// <summary>
/// 购物车统计数据DTO
/// </summary>
/// <remarks>
/// 用于返回购物车统计分析数据，包括总购物车项数量、会员数量、平均商品数、热门商品TOP10
/// </remarks>
public class CartStatisticsDto
{
    /// <summary>
    /// 总购物车项数量
    /// </summary>
    public int TotalCartItems { get; set; }

    /// <summary>
    /// 有购物车的会员数量
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// 平均每会员购物车商品数
    /// </summary>
    public decimal AvgItemsPerMember { get; set; }

    /// <summary>
    /// 热门商品TOP10（按购物车中出现次数）
    /// </summary>
    public List<HotProductDto> HotProducts { get; set; } = new();
}
```

- [ ] **Step 3: 验证统计 DTO 创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 8: 提交数据模型层代码

**Files:**
- 已创建的所有 Entity 和 DTO 文件

- [ ] **Step 1: 添加所有新建文件到 Git**

Run: `cd EasyProduct.WebApi && git add .`
Expected: All new files staged

- [ ] **Step 2: 提交数据模型层代码**

Run:
```bash
cd EasyProduct.WebApi
git commit -m "feat(api): 添加购物车数据模型层

- 创建购物车实体类（Cart）
- 创建购物车选中状态枚举（CartSelectedStatus）
- 创建购物车查询和操作 DTO
- 创建购物车统计 DTO

Co-Authored-By: lilin <565387073@qq.com>"
```
Expected: Commit created successfully

---

## 阶段 2：业务逻辑层

### Task 9: 创建购物车服务接口

**Files:**
- Create: `EasyProduct.Business/Mall/ICartService.cs`

- [ ] **Step 1: 创建 ICartService 接口**

```csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Cart;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 购物车服务接口
/// </summary>
/// <remarks>
/// 提供购物车的增删改查、批量操作、失效商品处理、统计分析等功能
/// </remarks>
public interface ICartService
{
    #region 查询

    /// <summary>
    /// 获取会员购物车列表（含统计信息）
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>购物车列表，包含商品详情和统计信息</returns>
    Task<CartListDto> GetCartListAsync(string memberId);

    /// <summary>
    /// 获取购物车详情
    /// </summary>
    /// <param name="cartId">购物车ID</param>
    /// <returns>购物车项详情</returns>
    Task<CartItemDto> GetCartDetailAsync(string cartId);

    /// <summary>
    /// 分页查询购物车（Admin端）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>购物车分页列表</returns>
    Task<PageResponse<CartListDto>> GetCartPageListAsync(CartQuery query);

    /// <summary>
    /// 获取购物车统计数据
    /// </summary>
    /// <returns>购物车统计数据</returns>
    Task<CartStatisticsDto> GetStatisticsAsync();

    #endregion

    #region 添加

    /// <summary>
    /// 添加商品到购物车
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">添加参数</param>
    /// <returns>购物车ID</returns>
    Task<string> AddToCartAsync(string memberId, CartAddDto dto);

    /// <summary>
    /// 批量添加到购物车
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">批量添加参数</param>
    /// <returns>成功添加的数量</returns>
    Task<int> BatchAddToCartAsync(string memberId, CartBatchAddDto dto);

    #endregion

    #region 修改

    /// <summary>
    /// 修改购物车商品数量
    /// </summary>
    /// <param name="cartId">购物车ID</param>
    /// <param name="quantity">新数量</param>
    /// <param name="operatorId">操作人ID（可选，Admin操作时传入）</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateQuantityAsync(string cartId, int quantity, string? operatorId = null);

    /// <summary>
    /// 修改选中状态
    /// </summary>
    /// <param name="cartIds">购物车ID列表</param>
    /// <param name="selected">选中状态：0=未选中，1=已选中</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateSelectedAsync(List<string> cartIds, int selected);

    /// <summary>
    /// 全选/取消全选
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="selected">选中状态：0=未选中，1=已选中</param>
    /// <returns>影响的数量</returns>
    Task<int> UpdateSelectAllAsync(string memberId, int selected);

    /// <summary>
    /// 切换SKU规格
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="cartId">购物车ID</param>
    /// <param name="newSkuId">新SKU ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ChangeSkuAsync(string memberId, string cartId, string newSkuId);

    #endregion

    #region 删除

    /// <summary>
    /// 删除购物车项
    /// </summary>
    /// <param name="cartId">购物车ID</param>
    /// <param name="operatorId">操作人ID（可选，Admin操作时传入）</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(string cartId, string? operatorId = null);

    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="cartIds">购物车ID列表</param>
    /// <param name="operatorId">操作人ID（可选，Admin操作时传入）</param>
    /// <returns>删除数量</returns>
    Task<int> BatchDeleteAsync(List<string> cartIds, string? operatorId = null);

    /// <summary>
    /// 清空购物车
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>删除数量</returns>
    Task<int> ClearCartAsync(string memberId);

    /// <summary>
    /// 清除失效商品
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>删除数量</returns>
    Task<int> ClearInvalidItemsAsync(string memberId);

    #endregion
}
```

- [ ] **Step 2: 验证接口创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 10: 实现购物车服务 - 核心查询方法

**Files:**
- Create: `EasyProduct.Business/Mall/CartService.cs`

- [ ] **Step 1: 创建 CartService 类并实现 GetCartListAsync 方法（第1部分）**

```csharp
using System.Text.Json;
using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Cart;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Enums;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 购物车服务实现
/// </summary>
/// <remarks>
/// 提供购物车的增删改查、批量操作、失效商品处理、统计分析等功能
/// 继承 BaseService，使用属性注入获取数据库上下文
/// </remarks>
public class CartService : BaseService, ICartService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<CartService> _logger;

    /// <summary>
    /// SKU服务
    /// </summary>
    private readonly ISkuService _skuService;

    /// <summary>
    /// 构造函数，通过依赖注入获取日志记录器和SKU服务
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="skuService">SKU服务</param>
    public CartService(ILogger<CartService> logger, ISkuService skuService)
    {
        _logger = logger;
        _skuService = skuService;
    }

    #region 查询

    /// <summary>
    /// 获取会员购物车列表（含统计信息）
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>购物车列表，包含商品详情和统计信息</returns>
    /// <remarks>
    /// 1. 查询会员的所有购物车项
    /// 2. 关联查询SKU和SPU信息
    /// 3. 自动标记失效商品（SKU禁用、库存不足）
    /// 4. 计算统计信息（总数量、选中数量、总金额、失效数量）
    /// </remarks>
    public async Task<CartListDto> GetCartListAsync(string memberId)
    {
        // 1. 查询购物车基础数据
        var carts = await _db.Queryable<Cart>()
            .Where(c => c.MemberId == memberId && c.IsDeleted == 0)
            .OrderByDescending(c => c.CreateTime)
            .ToListAsync();

        if (!carts.Any())
        {
            return new CartListDto();
        }

        // 2. 查询SKU详情
        var skuIds = carts.Select(c => c.SkuId).Distinct().ToList();
        var skus = await _db.Queryable<product_sku>()
            .Where(s => skuIds.Contains(s.Id.ToString()) && s.IsDeleted == 0)
            .ToListAsync();

        // 3. 查询SPU详情
        var spuIds = skus.Select(s => s.SpuId).Distinct().ToList();
        var spus = await _db.Queryable<product_spu>()
            .Where(s => spuIds.Contains(s.Id.ToString()) && s.IsDeleted == 0)
            .ToListAsync();

        // 4. 组装购物车项DTO
        var items = new List<CartItemDto>();
        foreach (var cart in carts)
        {
            var sku = skus.FirstOrDefault(s => s.Id.ToString() == cart.SkuId);
            var spu = sku != null ? spus.FirstOrDefault(s => s.Id.ToString() == sku.SpuId) : null;

            if (sku == null || spu == null)
            {
                // SKU或SPU被删除，标记为失效
                items.Add(new CartItemDto
                {
                    Id = cart.Id,
                    SkuId = cart.SkuId,
                    Quantity = cart.Quantity,
                    Selected = cart.Selected,
                    IsInvalid = true,
                    InvalidReason = "商品已删除",
                    CreateTime = cart.CreateTime
                });
                continue;
            }

            // 判断是否失效
            var isInvalid = sku.Status != Status.Enabled || cart.Quantity > sku.Stock;
            var invalidReason = "";
            if (sku.Status != Status.Enabled)
            {
                invalidReason = "商品已下架";
            }
            else if (cart.Quantity > sku.Stock)
            {
                invalidReason = $"库存不足，当前库存 {sku.Stock} 件";
            }

            items.Add(new CartItemDto
            {
                Id = cart.Id,
                SkuId = cart.SkuId,
                SpuId = spu.Id,
                ProductName = spu.SpuName,
                SkuCode = sku.SkuCode,
                SpecText = BuildSpecText(sku.SpecJson),
                MainImage = spu.MainImage,
                RetailPrice = sku.Price,
                MemberPrice = sku.MemberPrice,
                ActualPrice = sku.MemberPrice ?? sku.Price,
                Quantity = cart.Quantity,
                Subtotal = (sku.MemberPrice ?? sku.Price) * cart.Quantity,
                Stock = sku.Stock,
                InStock = sku.Stock >= cart.Quantity,
                SkuStatus = (int)sku.Status,
                IsInvalid = isInvalid,
                InvalidReason = isInvalid ? invalidReason : null,
                Selected = cart.Selected,
                CreateTime = cart.CreateTime
            });
        }

        // 5. 计算统计信息
        var result = new CartListDto
        {
            Items = items,
            TotalCount = items.Count,
            SelectedCount = items.Count(i => i.Selected == 1),
            InvalidCount = items.Count(i => i.IsInvalid),
            TotalAmount = items.Where(i => i.Selected == 1 && !i.IsInvalid).Sum(i => i.Subtotal)
        };

        return result;
    }

    /// <summary>
    /// 构建规格文本
    /// </summary>
    /// <param name="specJson">规格JSON字符串</param>
    /// <returns>规格文本（如："红色 / XL"）</returns>
    private string BuildSpecText(string? specJson)
    {
        if (string.IsNullOrEmpty(specJson))
        {
            return "";
        }

        try
        {
            var specs = JsonSerializer.Deserialize<List<SpecItem>>(specJson);
            return specs != null ? string.Join(" / ", specs.Select(s => s.Value)) : "";
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// 规格项
    /// </summary>
    private class SpecItem
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    #endregion
}
```

- [ ] **Step 2: 验证 Service 创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 11: 实现购物车服务 - 查询和添加方法

**Files:**
- Modify: `EasyProduct.Business/Mall/CartService.cs`

- [ ] **Step 1: 在 CartService 中添加 GetCartDetailAsync 方法**

在 `#endregion` 之前添加：

```csharp
    /// <summary>
    /// 获取购物车详情
    /// </summary>
    /// <param name="cartId">购物车ID</param>
    /// <returns>购物车项详情</returns>
    /// <exception cref="BusinessException">购物车项不存在时抛出</exception>
    public async Task<CartItemDto> GetCartDetailAsync(string cartId)
    {
        // 1. 查询购物车项
        var cart = await _db.Queryable<Cart>()
            .Where(c => c.Id == cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw BusinessException.NotFound($"购物车商品不存在: {cartId}");
        }

        // 2. 获取会员购物车列表（复用 GetCartListAsync 的逻辑）
        var cartList = await GetCartListAsync(cart.MemberId);

        // 3. 查找指定购物车项
        var cartItem = cartList.Items.FirstOrDefault(i => i.Id == cartId);

        if (cartItem == null)
        {
            throw BusinessException.NotFound($"购物车商品不存在: {cartId}");
        }

        return cartItem;
    }

    /// <summary>
    /// 分页查询购物车（Admin端）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>购物车分页列表</returns>
    /// <remarks>
    /// 1. 支持按会员ID精确匹配
    /// 2. 支持按商品名称/SKU编码模糊搜索
    /// 3. 支持按选中状态筛选
    /// 4. 支持分页查询
    /// </remarks>
    public async Task<PageResponse<CartListDto>> GetCartPageListAsync(CartQuery query)
    {
        // 1. 构建基础查询
        var queryable = _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0);

        // 2. 添加筛选条件
        if (!string.IsNullOrEmpty(query.MemberId))
        {
            queryable = queryable.Where(c => c.MemberId == query.MemberId);
        }

        if (!string.IsNullOrEmpty(query.Keyword))
        {
            // 关联查询SKU和SPU，按商品名称或SKU编码搜索
            var keyword = query.Keyword;
            queryable = queryable.LeftJoin<product_sku>((c, s) => c.SkuId == s.Id.ToString() && s.IsDeleted == 0)
                .LeftJoin<product_spu>((c, s, p) => s.SpuId == p.Id.ToString() && p.IsDeleted == 0)
                .Where((c, s, p) => p.SpuName.Contains(keyword) || s.SkuCode.Contains(keyword));
        }

        if (query.Selected.HasValue)
        {
            queryable = queryable.Where(c => c.Selected == query.Selected.Value);
        }

        // 3. 按创建时间倒序
        queryable = queryable.OrderByDescending(c => c.CreateTime);

        // 4. 执行分页查询
        RefAsync<int> total = 0;
        var carts = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 5. 按会员分组，每个会员的购物车作为一个 CartListDto
        var memberIds = carts.Select(c => c.MemberId).Distinct().ToList();
        var cartListDtos = new List<CartListDto>();

        foreach (var memberId in memberIds)
        {
            var cartList = await GetCartListAsync(memberId);
            cartListDtos.Add(cartList);
        }

        // 6. 返回分页结果
        return PageResponse<CartListDto>.Create(cartListDtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取购物车统计数据
    /// </summary>
    /// <returns>购物车统计数据</returns>
    public async Task<CartStatisticsDto> GetStatisticsAsync()
    {
        // 1. 查询总购物车项数量
        var totalCartItems = await _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0)
            .CountAsync();

        // 2. 查询有购物车的会员数量
        var memberCount = await _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0)
            .GroupBy(c => c.MemberId)
            .CountAsync();

        // 3. 计算平均每会员购物车商品数
        var avgItemsPerMember = memberCount > 0 ? (decimal)totalCartItems / memberCount : 0;

        // 4. 查询热门商品TOP10
        var hotProducts = await _db.Queryable<Cart, product_sku, product_spu>((c, s, p) => new JoinQueryInfos(
                JoinType.Left, c.SkuId == s.Id.ToString() && s.IsDeleted == 0,
                JoinType.Left, s.SpuId == p.Id.ToString() && p.IsDeleted == 0
            ))
            .Where((c, s, p) => c.IsDeleted == 0)
            .GroupBy((c, s, p) => new { p.Id, p.SpuName })
            .Select((c, s, p) => new HotProductDto
            {
                ProductId = p.Id.ToString(),
                ProductName = p.SpuName,
                CartCount = SqlFunc.AggregateCount()
            })
            .OrderByDescending((c, s, p) => SqlFunc.AggregateCount())
            .Take(10)
            .ToListAsync();

        // 5. 返回统计结果
        return new CartStatisticsDto
        {
            TotalCartItems = totalCartItems,
            MemberCount = memberCount,
            AvgItemsPerMember = Math.Round(avgItemsPerMember, 2),
            HotProducts = hotProducts
        };
    }
```

- [ ] **Step 2: 添加 AddToCartAsync 方法**

在 `#region 添加` 中添加：

```csharp
    /// <summary>
    /// 添加商品到购物车
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">添加参数</param>
    /// <returns>购物车ID</returns>
    /// <exception cref="BusinessException">SKU不存在、SKU禁用、库存不足时抛出</exception>
    /// <remarks>
    /// 1. 验证SKU是否存在且启用
    /// 2. 检查库存是否充足
    /// 3. 检查是否已在购物车中
    /// 4. 存在则累加数量，不存在则新增记录
    /// </remarks>
    public async Task<string> AddToCartAsync(string memberId, CartAddDto dto)
    {
        // 1. 验证SKU是否存在且启用
        var sku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == dto.SkuId && s.IsDeleted == 0)
            .FirstAsync();

        if (sku == null)
        {
            throw BusinessException.NotFound($"商品规格不存在: {dto.SkuId}");
        }

        if (sku.Status != Status.Enabled)
        {
            throw BusinessException.BadRequest("商品已下架");
        }

        // 2. 检查库存
        if (dto.Quantity > sku.Stock)
        {
            throw BusinessException.BadRequest($"库存不足，当前库存 {sku.Stock} 件，您需要 {dto.Quantity} 件");
        }

        // 3. 检查是否已在购物车中
        var existingCart = await _db.Queryable<Cart>()
            .Where(c => c.MemberId == memberId && c.SkuId == dto.SkuId && c.IsDeleted == 0)
            .FirstAsync();

        if (existingCart != null)
        {
            // 累加数量
            var newQuantity = existingCart.Quantity + dto.Quantity;
            if (newQuantity > sku.Stock)
            {
                throw BusinessException.BadRequest($"库存不足，当前库存 {sku.Stock} 件，购物车已有 {existingCart.Quantity} 件");
            }

            existingCart.Quantity = newQuantity;
            existingCart.UpdateTime = DateTime.Now;
            await _db.Updateable(existingCart).ExecuteCommandAsync();

            _logger.LogInformation("会员 {MemberId} 更新购物车商品 {SkuId}，数量：{Quantity}",
                memberId, dto.SkuId, newQuantity);

            return existingCart.Id;
        }

        // 4. 新增购物车记录
        var cart = new Cart
        {
            MemberId = memberId,
            SkuId = dto.SkuId,
            Quantity = dto.Quantity,
            Selected = 1,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now,
            CreateBy = memberId,
            IsDeleted = 0
        };

        await _db.Insertable(cart).ExecuteCommandAsync();

        _logger.LogInformation("会员 {MemberId} 添加商品到购物车 {SkuId}，数量：{Quantity}",
            memberId, dto.SkuId, dto.Quantity);

        return cart.Id;
    }

    /// <summary>
    /// 批量添加到购物车
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="dto">批量添加参数</param>
    /// <returns>成功添加的数量</returns>
    /// <remarks>
    /// 批量添加商品到购物车，如果某个商品添加失败，不影响其他商品的添加
    /// </remarks>
    public async Task<int> BatchAddToCartAsync(string memberId, CartBatchAddDto dto)
    {
        var successCount = 0;

        foreach (var item in dto.Items)
        {
            try
            {
                await AddToCartAsync(memberId, item);
                successCount++;
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning("批量添加购物车失败，会员ID: {MemberId}, SKU ID: {SkuId}, 原因: {Message}",
                    memberId, item.SkuId, ex.Message);
            }
        }

        return successCount;
    }
```

- [ ] **Step 3: 验证代码编译通过**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 12: 实现购物车服务 - 修改和删除方法

**Files:**
- Modify: `EasyProduct.Business/Mall/CartService.cs`

- [ ] **Step 1: 添加修改方法**

在 `#region 修改` 中添加：

```csharp
    /// <summary>
    /// 修改购物车商品数量
    /// </summary>
    /// <param name="cartId">购物车ID</param>
    /// <param name="quantity">新数量</param>
    /// <param name="operatorId">操作人ID（可选，Admin操作时传入）</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">购物车项不存在、数量不合法、库存不足时抛出</exception>
    public async Task<bool> UpdateQuantityAsync(string cartId, int quantity, string? operatorId = null)
    {
        // 1. 验证购物车项是否存在
        var cart = await _db.Queryable<Cart>()
            .Where(c => c.Id == cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw BusinessException.NotFound($"购物车商品不存在: {cartId}");
        }

        // 2. 验证数量是否合法
        if (quantity <= 0)
        {
            throw BusinessException.BadRequest("数量必须大于0");
        }

        // 3. 检查库存
        var sku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == cart.SkuId && s.IsDeleted == 0)
            .FirstAsync();

        if (sku != null && quantity > sku.Stock)
        {
            throw BusinessException.BadRequest($"库存不足，当前库存 {sku.Stock} 件");
        }

        // 4. 更新数量
        cart.Quantity = quantity;
        cart.UpdateTime = DateTime.Now;
        cart.UpdateBy = operatorId ?? cart.MemberId;

        await _db.Updateable(cart).ExecuteCommandAsync();

        _logger.LogInformation("购物车 {CartId} 数量更新为 {Quantity}，操作人：{Operator}",
            cartId, quantity, operatorId ?? cart.MemberId);

        return true;
    }

    /// <summary>
    /// 修改选中状态
    /// </summary>
    /// <param name="cartIds">购物车ID列表</param>
    /// <param name="selected">选中状态：0=未选中，1=已选中</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateSelectedAsync(List<string> cartIds, int selected)
    {
        if (cartIds == null || !cartIds.Any())
        {
            return false;
        }

        var result = await _db.Updateable<Cart>()
            .Where(c => cartIds.Contains(c.Id) && c.IsDeleted == 0)
            .SetColumns(c => new Cart { Selected = selected, UpdateTime = DateTime.Now })
            .ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 全选/取消全选
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="selected">选中状态：0=未选中，1=已选中</param>
    /// <returns>影响的数量</returns>
    public async Task<int> UpdateSelectAllAsync(string memberId, int selected)
    {
        var result = await _db.Updateable<Cart>()
            .Where(c => c.MemberId == memberId && c.IsDeleted == 0)
            .SetColumns(c => new Cart { Selected = selected, UpdateTime = DateTime.Now })
            .ExecuteCommandAsync();

        _logger.LogInformation("会员 {MemberId} {Action}所有购物车商品",
            memberId, selected == 1 ? "选中" : "取消选中");

        return result;
    }

    /// <summary>
    /// 切换SKU规格
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="cartId">购物车ID</param>
    /// <param name="newSkuId">新SKU ID</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">购物车项不存在、新SKU不存在、新SKU已在购物车中时抛出</exception>
    public async Task<bool> ChangeSkuAsync(string memberId, string cartId, string newSkuId)
    {
        // 1. 验证购物车项是否存在
        var cart = await _db.Queryable<Cart>()
            .Where(c => c.Id == cartId && c.MemberId == memberId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw BusinessException.NotFound($"购物车商品不存在: {cartId}");
        }

        // 2. 验证新SKU是否存在
        var newSku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == newSkuId && s.IsDeleted == 0)
            .FirstAsync();

        if (newSku == null)
        {
            throw BusinessException.NotFound($"商品规格不存在: {newSkuId}");
        }

        // 3. 检查新SKU是否已在购物车中
        var existingCart = await _db.Queryable<Cart>()
            .Where(c => c.MemberId == memberId && c.SkuId == newSkuId && c.Id != cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (existingCart != null)
        {
            throw BusinessException.BadRequest("该规格商品已在购物车中");
        }

        // 4. 更新SKU
        cart.SkuId = newSkuId;
        cart.UpdateTime = DateTime.Now;
        await _db.Updateable(cart).ExecuteCommandAsync();

        _logger.LogInformation("会员 {MemberId} 切换购物车 {CartId} 的SKU为 {NewSkuId}",
            memberId, cartId, newSkuId);

        return true;
    }
```

- [ ] **Step 2: 添加删除方法**

在 `#region 删除` 中添加：

```csharp
    /// <summary>
    /// 删除购物车项
    /// </summary>
    /// <param name="cartId">购物车ID</param>
    /// <param name="operatorId">操作人ID（可选，Admin操作时传入）</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">购物车项不存在时抛出</exception>
    public async Task<bool> DeleteAsync(string cartId, string? operatorId = null)
    {
        // 1. 验证购物车项是否存在
        var cart = await _db.Queryable<Cart>()
            .Where(c => c.Id == cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw BusinessException.NotFound($"购物车商品不存在: {cartId}");
        }

        // 2. 软删除
        cart.IsDeleted = 1;
        cart.UpdateTime = DateTime.Now;
        cart.UpdateBy = operatorId ?? cart.MemberId;

        await _db.Updateable(cart).ExecuteCommandAsync();

        _logger.LogInformation("购物车 {CartId} 已删除，操作人：{Operator}",
            cartId, operatorId ?? cart.MemberId);

        return true;
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="cartIds">购物车ID列表</param>
    /// <param name="operatorId">操作人ID（可选，Admin操作时传入）</param>
    /// <returns>删除数量</returns>
    public async Task<int> BatchDeleteAsync(List<string> cartIds, string? operatorId = null)
    {
        if (cartIds == null || !cartIds.Any())
        {
            return 0;
        }

        var result = await _db.Updateable<Cart>()
            .Where(c => cartIds.Contains(c.Id) && c.IsDeleted == 0)
            .SetColumns(c => new Cart
            {
                IsDeleted = 1,
                UpdateTime = DateTime.Now,
                UpdateBy = operatorId ?? c.MemberId
            })
            .ExecuteCommandAsync();

        _logger.LogInformation("批量删除 {Count} 个购物车项，操作人：{Operator}",
            result, operatorId ?? "会员");

        return result;
    }

    /// <summary>
    /// 清空购物车
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>删除数量</returns>
    public async Task<int> ClearCartAsync(string memberId)
    {
        var result = await _db.Updateable<Cart>()
            .Where(c => c.MemberId == memberId && c.IsDeleted == 0)
            .SetColumns(c => new Cart
            {
                IsDeleted = 1,
                UpdateTime = DateTime.Now,
                UpdateBy = memberId
            })
            .ExecuteCommandAsync();

        _logger.LogInformation("会员 {MemberId} 清空购物车，删除 {Count} 件商品",
            memberId, result);

        return result;
    }

    /// <summary>
    /// 清除失效商品
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>删除数量</returns>
    public async Task<int> ClearInvalidItemsAsync(string memberId)
    {
        // 1. 获取会员购物车列表
        var cartList = await GetCartListAsync(memberId);

        // 2. 获取失效商品的ID列表
        var invalidCartIds = cartList.Items
            .Where(i => i.IsInvalid)
            .Select(i => i.Id)
            .ToList();

        if (!invalidCartIds.Any())
        {
            return 0;
        }

        // 3. 批量删除失效商品
        var result = await BatchDeleteAsync(invalidCartIds, memberId);

        _logger.LogInformation("会员 {MemberId} 清除 {Count} 件失效商品",
            memberId, result);

        return result;
    }
```

- [ ] **Step 3: 验证代码编译通过**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 13: 提交业务逻辑层代码

**Files:**
- `EasyProduct.Business/Mall/ICartService.cs`
- `EasyProduct.Business/Mall/CartService.cs`

- [ ] **Step 1: 添加文件到 Git**

Run: `cd EasyProduct.WebApi && git add .`
Expected: All files staged

- [ ] **Step 2: 提交业务逻辑层代码**

Run:
```bash
cd EasyProduct.WebApi
git commit -m "feat(api): 实现购物车服务层

- 创建购物车服务接口（ICartService）
- 实现购物车核心业务逻辑
- 支持添加、修改、删除、查询
- 支持批量操作和失效商品处理
- 实现购物车统计分析功能

Co-Authored-By: lilin <565387073@qq.com>"
```
Expected: Commit created successfully

---

## 阶段 3：控制器层 - 小程序端

### Task 14: 创建小程序端购物车控制器

**Files:**
- Create: `EasyProduct.Web/Controllers/App/Mall/CartController.cs`

- [ ] **Step 1: 创建 AppCartController（第1部分：基础框架和查询方法）**

```csharp
using EasyProduct.Business.Mall;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.App.Mall;

/// <summary>
/// 小程序端购物车控制器
/// </summary>
/// <remarks>
/// 提供小程序会员的购物车管理接口，包括查询、添加、修改、删除、批量操作等功能
/// 所有接口需要会员权限（MemberJwt 认证）
/// </remarks>
[ApiController]
[Route("api/app/mall/cart")]
[Authorize(AuthenticationSchemes = "MemberJwt")]
public class AppCartController : BaseController
{
    private readonly ICartService _cartService;
    private readonly ILogger<AppCartController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="cartService">购物车服务接口</param>
    /// <param name="logger">日志记录器</param>
    public AppCartController(ICartService cartService, ILogger<AppCartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// 查询我的购物车
    /// </summary>
    /// <returns>购物车列表，包含商品详情和统计信息</returns>
    /// <remarks>
    /// 获取当前登录会员的购物车完整信息，包括：
    /// - 购物车项列表（含商品详情、价格、库存、失效状态）
    /// - 统计信息（总数量、选中数量、总金额、失效商品数量）
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<CartListDto>> GetMyCart()
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _cartService.GetCartListAsync(memberId);
        return Success(result);
    }

    /// <summary>
    /// 添加到购物车
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>购物车ID</returns>
    /// <remarks>
    /// 添加商品到购物车，如果商品已在购物车中，则累加数量
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> AddToCart([FromBody] CartAddDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var cartId = await _cartService.AddToCartAsync(memberId, dto);
        return Success(cartId, "添加成功");
    }

    /// <summary>
    /// 批量添加到购物车
    /// </summary>
    /// <param name="dto">批量添加参数</param>
    /// <returns>成功添加的数量</returns>
    /// <remarks>
    /// 批量添加商品到购物车，如果某个商品添加失败，不影响其他商品的添加
    /// </remarks>
    [HttpPost("batch")]
    public async Task<ApiResponse<int>> BatchAddToCart([FromBody] CartBatchAddDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var count = await _cartService.BatchAddToCartAsync(memberId, dto);
        return Success(count, $"成功添加 {count} 件商品");
    }
}
```

- [ ] **Step 2: 添加修改方法**

在 `AppCartController` 类中添加：

```csharp
    /// <summary>
    /// 修改购物车数量
    /// </summary>
    /// <param name="id">购物车ID</param>
    /// <param name="dto">修改数量参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 修改购物车中指定商品的数量，需要验证库存是否充足
    /// </remarks>
    [HttpPut("{id}/quantity")]
    public async Task<ApiResponse<bool>> UpdateQuantity(string id, [FromBody] CartUpdateQuantityDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _cartService.UpdateQuantityAsync(id, dto.Quantity, memberId);
        return Success(result, "修改成功");
    }

    /// <summary>
    /// 切换选中状态
    /// </summary>
    /// <param name="id">购物车ID</param>
    /// <param name="selected">选中状态：0=未选中，1=已选中</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 切换购物车中单个商品的选中状态
    /// </remarks>
    [HttpPut("{id}/selected")]
    public async Task<ApiResponse<bool>> ToggleSelected(string id, [FromBody] dynamic body)
    {
        int selected = body.selected;
        var result = await _cartService.UpdateSelectedAsync(new List<string> { id }, selected);
        return Success(result, selected == 1 ? "已选中" : "已取消选中");
    }

    /// <summary>
    /// 批量修改选中状态
    /// </summary>
    /// <param name="dto">批量修改选中状态参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 批量修改购物车中多个商品的选中状态
    /// </remarks>
    [HttpPut("selected")]
    public async Task<ApiResponse<bool>> BatchUpdateSelected([FromBody] CartUpdateSelectedDto dto)
    {
        var result = await _cartService.UpdateSelectedAsync(dto.Ids, dto.Selected);
        return Success(result);
    }

    /// <summary>
    /// 全选/取消全选
    /// </summary>
    /// <param name="body">包含 selected 字段的动态对象</param>
    /// <returns>影响的数量</returns>
    /// <remarks>
    /// 设置当前会员购物车中所有商品的选中状态
    /// </remarks>
    [HttpPut("select-all")]
    public async Task<ApiResponse<int>> SelectAll([FromBody] dynamic body)
    {
        int selected = body.selected;
        var memberId = GetCurrentUserId().ToString();
        var count = await _cartService.UpdateSelectAllAsync(memberId, selected);
        return Success(count, selected == 1 ? $"已选中 {count} 件商品" : $"已取消选中 {count} 件商品");
    }

    /// <summary>
    /// 切换SKU规格
    /// </summary>
    /// <param name="dto">切换SKU规格参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 在购物车中切换商品的规格（SKU），例如从红色M码切换到蓝色L码
    /// </remarks>
    [HttpPut("sku")]
    public async Task<ApiResponse<bool>> ChangeSku([FromBody] CartChangeSkuDto dto)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _cartService.ChangeSkuAsync(memberId, dto.CartId, dto.NewSkuId);
        return Success(result, "规格切换成功");
    }
```

- [ ] **Step 3: 添加删除方法**

在 `AppCartController` 类中添加：

```csharp
    /// <summary>
    /// 删除单个商品
    /// </summary>
    /// <param name="id">购物车ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 从购物车中删除指定商品
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteCart(string id)
    {
        var memberId = GetCurrentUserId().ToString();
        var result = await _cartService.DeleteAsync(id, memberId);
        return Success(result, "删除成功");
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="body">包含 ids 数组的动态对象</param>
    /// <returns>删除数量</returns>
    /// <remarks>
    /// 批量删除购物车中的商品
    /// </remarks>
    [HttpDelete("batch")]
    public async Task<ApiResponse<int>> BatchDelete([FromBody] dynamic body)
    {
        List<string> ids = ((Newtonsoft.Json.Linq.JArray)body.ids).ToObject<List<string>>();
        var memberId = GetCurrentUserId().ToString();
        var count = await _cartService.BatchDeleteAsync(ids, memberId);
        return Success(count, $"成功删除 {count} 件商品");
    }

    /// <summary>
    /// 清空购物车
    /// </summary>
    /// <returns>删除数量</returns>
    /// <remarks>
    /// 清空当前会员的所有购物车商品
    /// </remarks>
    [HttpDelete("clear")]
    public async Task<ApiResponse<int>> ClearCart()
    {
        var memberId = GetCurrentUserId().ToString();
        var count = await _cartService.ClearCartAsync(memberId);
        return Success(count, $"已清空购物车，删除 {count} 件商品");
    }

    /// <summary>
    /// 清除失效商品
    /// </summary>
    /// <returns>删除数量</returns>
    /// <remarks>
    /// 清除购物车中已失效的商品（商品下架、库存不足）
    /// </remarks>
    [HttpDelete("invalid")]
    public async Task<ApiResponse<int>> ClearInvalidItems()
    {
        var memberId = GetCurrentUserId().ToString();
        var count = await _cartService.ClearInvalidItemsAsync(memberId);
        return Success(count, $"已清除 {count} 件失效商品");
    }
```

- [ ] **Step 4: 验证控制器创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 15: 提交小程序端控制器代码

**Files:**
- `EasyProduct.Web/Controllers/App/Mall/CartController.cs`

- [ ] **Step 1: 添加文件到 Git**

Run: `cd EasyProduct.WebApi && git add .`
Expected: File staged

- [ ] **Step 2: 提交小程序端控制器代码**

Run:
```bash
cd EasyProduct.WebApi
git commit -m "feat(api): 实现小程序端购物车控制器

- 创建小程序端购物车控制器（AppCartController）
- 实现12个购物车管理接口
- 支持查询、添加、修改、删除、批量操作
- 支持失效商品清除功能

Co-Authored-By: lilin <565387073@qq.com>"
```
Expected: Commit created successfully

---

## 阶段 4：控制器层 - 管理端

### Task 16: 创建管理端购物车控制器

**Files:**
- Create: `EasyProduct.Web/Controllers/Admin/Mall/CartController.cs`

- [ ] **Step 1: 创建 AdminCartController（第1部分：基础框架和查询方法）**

```csharp
using EasyProduct.Business.Mall;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Mall;

/// <summary>
/// 管理端购物车控制器
/// </summary>
/// <remarks>
/// 提供管理端的购物车查询和管理接口，包括查看会员购物车、修改数量、删除购物车项、统计分析等功能
/// 所有接口需要管理员权限（AdminJwt 认证）
/// </remarks>
[ApiController]
[Route("api/admin/mall/cart")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class AdminCartController : BaseController
{
    private readonly ICartService _cartService;
    private readonly ILogger<AdminCartController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="cartService">购物车服务接口</param>
    /// <param name="logger">日志记录器</param>
    public AdminCartController(ICartService cartService, ILogger<AdminCartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// 查询会员购物车列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>购物车分页列表</returns>
    /// <remarks>
    /// 支持按会员ID精确匹配、商品名称/SKU编码模糊搜索、选中状态筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<CartListDto>>> GetCartList([FromQuery] CartQuery query)
    {
        var result = await _cartService.GetCartPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 查询指定会员的购物车
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>购物车列表，包含商品详情和统计信息</returns>
    /// <remarks>
    /// 查看指定会员的购物车完整信息，用于客服协助会员处理问题
    /// </remarks>
    [HttpGet("member/{memberId}")]
    public async Task<ApiResponse<CartListDto>> GetMemberCart(string memberId)
    {
        var result = await _cartService.GetCartListAsync(memberId);
        return Success(result);
    }

    /// <summary>
    /// 查询购物车详情
    /// </summary>
    /// <param name="id">购物车ID</param>
    /// <returns>购物车项详情</returns>
    /// <remarks>
    /// 查看购物车中单个商品的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CartItemDto>> GetCartDetail(string id)
    {
        var result = await _cartService.GetCartDetailAsync(id);
        return Success(result);
    }
}
```

- [ ] **Step 2: 添加修改和删除方法**

在 `AdminCartController` 类中添加：

```csharp
    /// <summary>
    /// 修改购物车数量（管理员操作）
    /// </summary>
    /// <param name="id">购物车ID</param>
    /// <param name="dto">修改数量参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 管理员帮助会员修改购物车中商品的数量
    /// </remarks>
    [HttpPut("{id}/quantity")]
    public async Task<ApiResponse<bool>> UpdateQuantity(string id, [FromBody] CartUpdateQuantityDto dto)
    {
        var adminId = GetCurrentUserId().ToString();
        var result = await _cartService.UpdateQuantityAsync(id, dto.Quantity, adminId);
        return Success(result, "修改成功");
    }

    /// <summary>
    /// 删除购物车项（管理员操作）
    /// </summary>
    /// <param name="id">购物车ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 管理员帮助会员删除购物车中的商品
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> DeleteCart(string id)
    {
        var adminId = GetCurrentUserId().ToString();
        var result = await _cartService.DeleteAsync(id, adminId);
        return Success(result, "删除成功");
    }

    /// <summary>
    /// 购物车统计分析
    /// </summary>
    /// <returns>购物车统计数据</returns>
    /// <remarks>
    /// 获取购物车统计数据，包括：
    /// - 总购物车项数量
    /// - 有购物车的会员数量
    /// - 平均每会员购物车商品数
    /// - 热门商品TOP10
    /// </remarks>
    [HttpGet("statistics")]
    public async Task<ApiResponse<CartStatisticsDto>> GetStatistics()
    {
        var result = await _cartService.GetStatisticsAsync();
        return Success(result);
    }
```

- [ ] **Step 3: 验证控制器创建成功**

Run: `cd EasyProduct.WebApi && dotnet build`
Expected: Build succeeded with 0 errors

---

### Task 17: 提交管理端控制器代码

**Files:**
- `EasyProduct.Web/Controllers/Admin/Mall/CartController.cs`

- [ ] **Step 1: 添加文件到 Git**

Run: `cd EasyProduct.WebApi && git add .`
Expected: File staged

- [ ] **Step 2: 提交管理端控制器代码**

Run:
```bash
cd EasyProduct.WebApi
git commit -m "feat(api): 实现管理端购物车控制器

- 创建管理端购物车控制器（AdminCartController）
- 实现6个购物车管理接口
- 支持查看会员购物车、修改数量、删除
- 支持购物车统计分析功能

Co-Authored-By: lilin <565387073@qq.com>"
```
Expected: Commit created successfully

---

## 阶段 5：测试与验证

### Task 18: 运行项目并测试接口

**Files:**
- 无需修改文件

- [ ] **Step 1: 启动后端项目**

Run: `cd EasyProduct.WebApi/EasyProduct.Web && dotnet run`
Expected: 项目启动成功，监听端口 5000

- [ ] **Step 2: 访问 Swagger 文档**

Open: `http://localhost:5000/swagger`
Expected: Swagger UI 显示所有购物车接口

- [ ] **Step 3: 测试小程序端接口**

在 Swagger UI 中测试以下接口：
1. `POST /api/app/mall/cart` - 添加到购物车
2. `GET /api/app/mall/cart/list` - 查询我的购物车
3. `PUT /api/app/mall/cart/{id}/quantity` - 修改数量
4. `DELETE /api/app/mall/cart/{id}` - 删除商品

Expected: 所有接口正常返回

- [ ] **Step 4: 测试管理端接口**

在 Swagger UI 中测试以下接口：
1. `GET /api/admin/mall/cart/list` - 查询会员购物车列表
2. `GET /api/admin/mall/cart/statistics` - 购物车统计分析

Expected: 所有接口正常返回

---

### Task 19: 创建数据库表

**Files:**
- 无需修改文件

- [ ] **Step 1: 检查是否有数据库迁移工具**

根据项目配置，使用 CodeFirst 或手动执行 SQL 脚本创建表

- [ ] **Step 2: 执行数据库表创建 SQL**

```sql
USE easyproduct;

-- 创建购物车表
CREATE TABLE IF NOT EXISTS mall_cart (
    id VARCHAR(36) PRIMARY KEY COMMENT '主键GUID',
    member_id VARCHAR(36) NOT NULL COMMENT '会员ID',
    sku_id VARCHAR(36) NOT NULL COMMENT 'SKU ID',
    quantity INT NOT NULL DEFAULT 1 COMMENT '数量',
    selected INT DEFAULT 1 COMMENT '是否选中：0=否，1=是',
    create_time DATETIME NOT NULL COMMENT '创建时间',
    update_time DATETIME NOT NULL COMMENT '更新时间',
    create_by VARCHAR(50) COMMENT '创建人',
    update_by VARCHAR(50) COMMENT '更新人',
    is_deleted INT DEFAULT 0 COMMENT '软删除标记：0=正常，1=已删除',
    
    INDEX idx_member_id (member_id) COMMENT '会员索引',
    INDEX idx_sku_id (sku_id) COMMENT 'SKU索引',
    UNIQUE KEY uk_member_sku (member_id, sku_id, is_deleted) COMMENT '会员+SKU唯一约束'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='购物车表';
```

Expected: 表创建成功

---

### Task 20: 最终提交与总结

**Files:**
- 所有购物车相关文件

- [ ] **Step 1: 检查是否有未提交的文件**

Run: `cd EasyProduct.WebApi && git status`
Expected: No uncommitted changes

- [ ] **Step 2: 推送所有提交到远程仓库**

Run: `cd EasyProduct.WebApi && git push`
Expected: All commits pushed successfully

- [ ] **Step 3: 更新记忆文件**

创建记忆文件记录购物车模块完成状态：

```bash
# 在 memory 目录下创建文件
```

---

## 自我审查检查清单

完成所有任务后，请验证以下内容：

### 1. 规格覆盖检查

- [x] 所有设计文档中的功能都已实现
- [x] 所有 API 接口都已创建
- [x] 所有 DTO 都已定义
- [x] 所有业务规则都已实现

### 2. 代码质量检查

- [x] 所有方法都有完整的中文注释
- [x] 所有异常都已正确处理
- [x] 所有数据库操作都使用 SqlSugar ORM
- [x] 所有 Controller 都继承 BaseController
- [x] 所有 Service 都继承 BaseService

### 3. 测试检查

- [x] 所有接口都可以通过 Swagger 测试
- [x] 数据库表已创建
- [x] 项目可以成功启动

### 4. 文档检查

- [x] 所有 API 接口都有 Swagger 注释
- [x] 所有 DTO 都有完整的字段注释
- [x] Git 提交信息符合规范

---

## 完成标志

当所有任务都完成并验证通过后，购物车子系统开发完成。

**预计总工时：** 12 小时（1.5-2 天）

**关键里程碑：**
1. ✅ 数据模型层完成（Task 1-8）
2. ✅ 业务逻辑层完成（Task 9-13）
3. ✅ 控制器层完成（Task 14-17）
4. ✅ 测试验证完成（Task 18-20）