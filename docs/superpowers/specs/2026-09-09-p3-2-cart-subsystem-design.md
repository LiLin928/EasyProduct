# P3.2 购物车子系统设计方案

> **日期：** 2026-09-09
> **状态：** 已评审通过
> **阶段：** P3 商城线
> **前置依赖：** P3.1 会员系统（已完成）、P2 商品管理（批次 1-3 已完成）

---

## 1. 概述

### 1.1 业务背景

购物车是小程序商城的核心功能，连接商品展示与订单支付，为会员提供商品暂存、数量调整、批量结算等功能。

### 1.2 设计目标

- ✅ 支持小程序会员完整购物车体验（添加、修改、删除、查询）
- ✅ 支持 Admin 后台查看和管理会员购物车
- ✅ 自动处理失效商品（下架、库存不足）
- ✅ 提供批量操作功能（批量添加、批量删除、批量选中）
- ✅ 实时计算购物车统计信息（总数量、选中数量、总金额）

### 1.3 核心决策

| 决策项 | 选择 | 理由 |
|--------|------|------|
| **使用场景** | 小程序端 + Admin 管理 | 符合业务需求，支持客服协助 |
| **核心功能** | 全部功能（基础 + 扩展） | 提供完整的购物车体验 |
| **存储方式** | 仅数据库持久化 | 实现简单，符合设计约定 |
| **实现方案** | 标准分层开发 | 架构一致，效率高，易维护 |

---

## 2. 架构设计

### 2.1 整体架构

```
┌────────────────────────────────────────────────────────────────┐
│                         前端层                                   │
│  MiniApp (小程序)          │         Admin (PC后台)              │
│  - 添加到购物车             │         - 查看会员购物车            │
│  - 修改数量                 │         - 帮助会员修改购物车        │
│  - 删除商品                 │         - 统计分析                 │
│  - 查询列表                 │                                    │
│  - 批量操作                 │                                    │
└────────────────────────────────────────────────────────────────┘
                              ↓
┌────────────────────────────────────────────────────────────────┐
│                       Controller 层                              │
│  App/CartController        │         Admin/CartController        │
│  /api/app/mall/cart/*      │         /api/admin/mall/cart/*      │
└────────────────────────────────────────────────────────────────┘
                              ↓
┌────────────────────────────────────────────────────────────────┐
│                        Service 层                                │
│              ICartService → CartService                         │
│  - 业务规则验证（SKU存在性、状态、库存）     │
│  - 价格计算（促销价、会员价）      │
│  - 失效商品处理                   │
└────────────────────────────────────────────────────────────────┘
                              ↓
┌────────────────────────────────────────────────────────────────┐
│                      数据访问层（ORM）                      │
│                      mall_cart 表                                │
└────────────────────────────────────────────────────────────────┘
                              ↓
┌────────────────────────────────────────────────────────────────┐
│                       关联模块服务                                │
│  ISkuService (SKU查询)     │         IMemberService (会员查询)   │
│  IProductService (商品信息) │                                    │
└────────────────────────────────────────────────────────────────┘
```

### 2.2 模块边界

购物车模块将：
- ✅ **允许调用**：Member、Product、SKU 服务（通过接口注入）
- ❌ **不允许直接访问**：Order、Payment 模块的数据库表
- ✅ **被调用方**：Order 模块在下单时会读取购物车数据

---

## 3. 数据模型设计

### 3.1 核心实体

#### mall_cart（购物车表）

```sql
CREATE TABLE mall_cart (
    id VARCHAR(36) PRIMARY KEY COMMENT '主键GUID',
    member_id VARCHAR(36) NOT NULL COMMENT '会员ID',
    sku_id VARCHAR(36) NOT NULL COMMENT 'SKU ID',
    quantity INT NOT NULL DEFAULT 1 COMMENT '数量',
    selected INT DEFAULT 1 COMMENT '是否选中：0=否，1=是',
    create_time DATETIME NOT NULL COMMENT '创建时间',
    update_time DATETIME NOT NULL COMMENT '更新时间',
    create_by VARCHAR(50) COMMENT '创建人',
    is_deleted INT DEFAULT 0 COMMENT '软删除标记：0=正常，1=已删除',
    
    INDEX idx_member_id (member_id) COMMENT '会员索引',
    INDEX idx_sku_id (sku_id) COMMENT 'SKU索引',
    UNIQUE KEY uk_member_sku (member_id, sku_id, is_deleted) COMMENT '会员+SKU唯一约束'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='购物车表';
```

#### C# 实体类

```csharp
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

### 3.2 DTO 设计

#### 查询 DTO

```csharp
/// <summary>
/// 购物车查询参数
/// </summary>
public class CartQuery : PageQuery
{
    /// <summary>
    /// 会员ID（Admin 端使用）
    /// </summary>
    public string? MemberId { get; set; }

    /// <summary>
    /// 关键词（商品名称/SKU编码）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 是否只查询选中商品
    /// </summary>
    public int? Selected { get; set; }
}

/// <summary>
/// 购物车项DTO（含商品详情）
/// </summary>
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

/// <summary>
/// 购物车列表DTO（含统计信息）
/// </summary>
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

#### 操作 DTO

```csharp
/// <summary>
/// 添加到购物车DTO
/// </summary>
public class CartAddDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    [Required(ErrorMessage = "SKU ID不能为空")]
    [StringLength(36)]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// 批量添加到购物车DTO
/// </summary>
public class CartBatchAddDto
{
    /// <summary>
    /// 购物车项列表
    /// </summary>
    [Required]
    public List<CartAddDto> Items { get; set; } = new();
}

/// <summary>
/// 修改购物车数量DTO
/// </summary>
public class CartUpdateQuantityDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    [Required]
    [StringLength(36)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 新数量
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }
}

/// <summary>
/// 修改选中状态DTO
/// </summary>
public class CartUpdateSelectedDto
{
    /// <summary>
    /// 购物车ID列表
    /// </summary>
    [Required]
    public List<string> Ids { get; set; } = new();

    /// <summary>
    /// 是否选中：0=否，1=是
    /// </summary>
    [Required]
    [Range(0, 1, ErrorMessage = "选中状态只能为0或1")]
    public int Selected { get; set; }
}

/// <summary>
/// 切换SKU规格DTO
/// </summary>
public class CartChangeSkuDto
{
    /// <summary>
    /// 购物车ID
    /// </summary>
    [Required]
    [StringLength(36)]
    public string CartId { get; set; } = string.Empty;

    /// <summary>
    /// 新SKU ID
    /// </summary>
    [Required]
    [StringLength(36)]
    public string NewSkuId { get; set; } = string.Empty;
}
```

---

## 4. API 设计

### 4.1 小程序端 API（/api/app/mall/cart/*）

| HTTP | 路由 | 功能 | 入参 | 出参 |
|------|------|------|------|------|
| **GET** | `/api/app/mall/cart/list` | 查询我的购物车 | - | `CartListDto` |
| **POST** | `/api/app/mall/cart` | 添加到购物车 | `CartAddDto` | 购物车ID |
| **POST** | `/api/app/mall/cart/batch` | 批量添加 | `CartBatchAddDto` | 成功数量 |
| **PUT** | `/api/app/mall/cart/{id}/quantity` | 修改数量 | `CartUpdateQuantityDto` | `true` |
| **PUT** | `/api/app/mall/cart/{id}/selected` | 切换选中状态 | `{ selected: 0|1 }` | `true` |
| **PUT** | `/api/app/mall/cart/selected` | 批量修改选中状态 | `CartUpdateSelectedDto` | `true` |
| **PUT** | `/api/app/mall/cart/select-all` | 全选/取消全选 | `{ selected: 0|1 }` | `true` |
| **PUT** | `/api/app/mall/cart/{id}/sku` | 切换SKU规格 | `CartChangeSkuDto` | `true` |
| **DELETE** | `/api/app/mall/cart/{id}` | 删除单个商品 | 路径参数 `id` | `true` |
| **DELETE** | `/api/app/mall/cart/batch` | 批量删除 | `{ ids: string[] }` | 删除数量 |
| **DELETE** | `/api/app/mall/cart/clear` | 清空购物车 | - | 删除数量 |
| **DELETE** | `/api/app/mall/cart/invalid` | 清除失效商品 | - | 删除数量 |

### 4.2 Admin 管理端 API（/api/admin/mall/cart/*）

| HTTP | 路由 | 功能 | 入参 | 出参 |
|------|------|------|------|------|
| **GET** | `/api/admin/mall/cart/list` | 查询会员购物车列表 | `CartQuery` | `PageResult<CartListDto>` |
| **GET** | `/api/admin/mall/cart/member/{memberId}` | 查询指定会员的购物车 | 路径参数 `memberId` | `CartListDto` |
| **GET** | `/api/admin/mall/cart/{id}` | 查询购物车详情 | 路径参数 `id` | `CartItemDto` |
| **PUT** | `/api/admin/mall/cart/{id}/quantity` | 修改数量（管理员操作） | `CartUpdateQuantityDto` | `true` |
| **DELETE** | `/api/admin/mall/cart/{id}` | 删除购物车项 | 路径参数 `id` | `true` |
| **GET** | `/api/admin/mall/cart/statistics` | 购物车统计分析 | - | `CartStatisticsDto` |

### 4.3 统计分析 DTO

```csharp
/// <summary>
/// 购物车统计数据DTO
/// </summary>
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

/// <summary>
/// 热门商品DTO
/// </summary>
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

---

## 5. 业务逻辑设计

### 5.1 Service 接口定义

```csharp
/// <summary>
/// 购物车服务接口
/// </summary>
public interface ICartService
{
    #region 查询
    /// <summary>
    /// 获取会员购物车列表（含统计信息）
    /// </summary>
    Task<CartListDto> GetCartListAsync(string memberId);

    /// <summary>
    /// 获取购物车详情
    /// </summary>
    Task<CartItemDto> GetCartDetailAsync(string cartId);

    /// <summary>
    /// 分页查询购物车（Admin端）
    /// </summary>
    Task<PageResult<CartListDto>> GetCartPageListAsync(CartQuery query);

    /// <summary>
    /// 获取购物车统计数据
    /// </summary>
    Task<CartStatisticsDto> GetStatisticsAsync();
    #endregion

    #region 添加
    /// <summary>
    /// 添加商品到购物车
    /// </summary>
    Task<string> AddToCartAsync(string memberId, CartAddDto dto);

    /// <summary>
    /// 批量添加到购物车
    /// </summary>
    Task<int> BatchAddToCartAsync(string memberId, CartBatchAddDto dto);
    #endregion

    #region 修改
    /// <summary>
    /// 修改购物车商品数量
    /// </summary>
    Task<bool> UpdateQuantityAsync(string cartId, int quantity, string? operatorId = null);

    /// <summary>
    /// 修改选中状态
    /// </summary>
    Task<bool> UpdateSelectedAsync(List<string> cartIds, int selected);

    /// <summary>
    /// 全选/取消全选
    /// </summary>
    Task<int> UpdateSelectAllAsync(string memberId, int selected);

    /// <summary>
    /// 切换SKU规格
    /// </summary>
    Task<bool> ChangeSkuAsync(string memberId, string cartId, string newSkuId);
    #endregion

    #region 删除
    /// <summary>
    /// 删除购物车项
    /// </summary>
    Task<bool> DeleteAsync(string cartId, string? operatorId = null);

    /// <summary>
    /// 批量删除
    /// </summary>
    Task<int> BatchDeleteAsync(List<string> cartIds, string? operatorId = null);

    /// <summary>
    /// 清空购物车
    /// </summary>
    Task<int> ClearCartAsync(string memberId);

    /// <summary>
    /// 清除失效商品
    /// </summary>
    Task<int> ClearInvalidItemsAsync(string memberId);
    #endregion
}
```

### 5.2 核心业务规则

#### 规则 1：添加购物车

```
┌─────────────────────────────────────────────┐
│           添加商品到购物车                    │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 1. 验证 SKU 是否存在且启用                    │
│    - ISkuService.GetDetailAsync(skuId)      │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 2. 检查库存是否充足                           │
│    - quantity ≤ stock                       │
│    - 否则提示"库存不足"                       │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 3. 检查是否已在购物车中                        │
│    - 根据 memberId + skuId 查询              │
│    - 存在 → 累加数量                          │
│    - 不存在 → 新增记录                         │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 4. 保存购物车记录                             │
│    - 更新或插入                               │
│    - 记录操作日志                             │
└─────────────────────────────────────────────┘
```

#### 规则 2：修改数量

```
┌─────────────────────────────────────────────┐
│           修改购物车数量                      │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 1. 验证购物车项是否存在                        │
│    - 不存在 → "购物车商品不存在"               │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 2. 验证数量是否合法                           │
│    - quantity > 0                            │
│    - 否则 → "数量必须大于0"                    │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 3. 检查库存是否充足                           │
│    - quantity ≤ stock                       │
│    - 否则 → "库存不足，当前库存 X 件"          │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│ 4. 更新数量                                   │
│    - 记录操作日志                             │
└─────────────────────────────────────────────┘
```

#### 规则 3：失效商品处理

```
商品失效条件：
1. SKU 状态 = Disabled (0)
2. SKU 库存 < 购物车数量
3. SKU 或 SPU 被软删除

失效商品处理：
- 查询购物车列表时自动标记失效状态
- 前端显示灰色样式 + "失效" 标签
- 提供"清除失效商品"按钮
- 下单时自动过滤失效商品
```

---

## 6. 错误处理

### 6.1 业务异常定义

```csharp
/// <summary>
/// 购物车业务异常
/// </summary>
public static class CartExceptions
{
    /// <summary>
    /// 购物车商品不存在
    /// </summary>
    public static BusinessException CartItemNotFound(string cartId) 
        => BusinessException.NotFound($"购物车商品不存在: {cartId}");

    /// <summary>
    /// 商品已下架
    /// </summary>
    public static BusinessException ProductOffShelf 
        => BusinessException.BadRequest("商品已下架");

    /// <summary>
    /// 库存不足
    /// </summary>
    public static BusinessException InsufficientStock(int stock, int requested)
        => BusinessException.BadRequest($"库存不足，当前库存 {stock} 件，您需要 {requested} 件");

    /// <summary>
    /// 数量不合法
    /// </summary>
    public static BusinessException InvalidQuantity 
        => BusinessException.BadRequest("数量必须大于0");

    /// <summary>
    /// SKU不存在
    /// </summary>
    public static BusinessException SkuNotFound(string skuId)
        => BusinessException.NotFound($"商品规格不存在: {skuId}");

    /// <summary>
    /// 购物车为空
    /// </summary>
    public static BusinessException CartIsEmpty 
        => BusinessException.BadRequest("购物车为空");

    /// <summary>
    /// 无选中商品
    /// </summary>
    public static BusinessException NoSelectedItems 
        => BusinessException.BadRequest("请选择要结算的商品");
}
```

---

## 7. 测试策略

### 7.1 单元测试

购物车不涉及"涉钱逻辑"（无资金流水），不强制单测，但建议编写测试以保证质量。

**测试用例清单：**

- ✅ 添加购物车：SKU 不存在、SKU 禁用、库存不足、新增、累加数量
- ✅ 修改数量：购物车项不存在、数量不合法、库存不足、成功更新
- ✅ 失效商品：SKU 禁用、库存不足、SKU 删除
- ✅ 统计信息：总数量、选中数量、失效数量、总金额计算
- ✅ 批量操作：批量删除、清空购物车

### 7.2 集成测试场景

| 场景 | 测试步骤 | 预期结果 |
|------|---------|---------|
| **添加商品** | 1. 添加商品到购物车<br/>2. 查询购物车列表 | 购物车显示新增商品 |
| **修改数量** | 1. 修改购物车数量<br/>2. 查询购物车列表 | 数量更新正确，小计金额正确 |
| **库存不足** | 1. 修改数量超过库存<br/>2. 查看响应 | 返回错误提示，原数量不变 |
| **商品下架** | 1. 禁用SKU<br/>2. 查询购物车 | 商品标记为失效 |
| **批量操作** | 1. 批量选中<br/>2. 批量删除<br/>3. 清空购物车 | 操作成功，统计信息正确 |
| **跨设备同步** | 1. 小程序添加商品<br/>2. Admin 查看购物车 | Admin 可以看到相同数据 |

---

## 8. 性能优化

### 8.1 索引设计

```sql
-- 主键索引
PRIMARY KEY (id)

-- 会员索引（高频查询：按会员查购物车）
INDEX idx_member_id (member_id)

-- SKU索引（验证商品时使用）
INDEX idx_sku_id (sku_id)

-- 唯一约束（防止重复添加）
UNIQUE KEY uk_member_sku (member_id, sku_id, is_deleted)
```

### 8.2 查询优化

1. **购物车列表查询**：
   - 使用 JOIN 一次性查询 Cart + SKU + SPU
   - 避免循环查询 N+1 问题

2. **缓存策略**：
   - SKU 信息可缓存（变更频率低）
   - 购物车数据不缓存（变更频率高）

3. **批量操作**：
   - 使用 SqlSugar 的 `BatchUpdate` / `BatchDelete`
   - 避免循环单条操作

---

## 9. 开发计划

| 阶段 | 工作内容 | 预计工时 |
|------|---------|---------|
| **Day 1 上午** | 实体、DTO、数据库表、枚举定义 | 3 小时 |
| **Day 1 下午** | Service 接口 + 核心方法实现 | 3 小时 |
| **Day 2 上午** | Controller 层（小程序端 + Admin 端） | 3 小时 |
| **Day 2 下午** | 单元测试 + 集成测试 + 文档 | 3 小时 |
| **总计** | **1.5-2 天** | **12 小时** |

---

## 10. 文件结构

```
EasyProduct.WebApi/
├── EasyProduct.Models/
│   ├── Entitys/Mall/
│   │   └── Cart.cs
│   ├── Dto/Mall/Cart/
│   │   ├── CartQuery.cs
│   │   ├── CartItemDto.cs
│   │   ├── CartListDto.cs
│   │   ├── CartAddDto.cs
│   │   ├── CartBatchAddDto.cs
│   │   ├── CartUpdateQuantityDto.cs
│   │   ├── CartUpdateSelectedDto.cs
│   │   ├── CartChangeSkuDto.cs
│   │   └── CartStatisticsDto.cs
├── EasyProduct.Business/Mall/
│   ├── ICartService.cs
│   └── CartService.cs
└── EasyProduct.Web/Controllers/
    ├── App/Mall/CartController.cs
    └── Admin/Mall/CartController.cs
```

---

## 11. 风险与注意事项

### 11.1 技术风险

- **并发问题**：多个请求同时添加同一 SKU 到购物车，可能导致数量不一致
  - 解决方案：使用数据库唯一约束 + 事务保证

- **库存实时性**：购物车中显示的库存可能不是最新
  - 解决方案：下单时再次验证库存，购物车仅做参考

### 11.2 业务风险

- **购物车数据量**：活跃会员购物车可能积累大量商品
  - 解决方案：定时清理超过一定时间未更新的购物车项

- **商品价格变动**：加入购物车后价格可能变化
  - 解决方案：下单时使用最新价格，购物车显示当前价格

---

## 12. YAGNI 边界

**明确不做：**

- ❌ 购物车分享功能（暂不需要）
- ❌ 购物车优惠券自动计算（在订单模块处理）
- ❌ 购物车推荐算法（一期不做）
- ❌ 购物车商品对比功能（暂不需要）

---

## 附录：状态枚举

```csharp
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