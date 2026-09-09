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
            .OrderByDescending(c => c.CreatedAt)
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
                    Id = cart.Id.ToString(),
                    SkuId = cart.SkuId,
                    Quantity = cart.Quantity,
                    Selected = cart.Selected,
                    IsInvalid = true,
                    InvalidReason = "商品已删除",
                    CreateTime = cart.CreatedAt
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
                Id = cart.Id.ToString(),
                SkuId = cart.SkuId,
                SpuId = spu.Id.ToString(),
                ProductName = spu.SpuName,
                SkuCode = sku.SkuCode ?? "",
                SpecText = BuildSpecText(sku.SpecJson),
                MainImage = spu.MainImage ?? "",
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
                CreateTime = cart.CreatedAt
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

    #region 添加

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
            throw new BusinessException($"商品规格不存在: {dto.SkuId}", 404);
        }

        if (sku.Status != Status.Enabled)
        {
            throw new BusinessException("商品已下架", 400);
        }

        // 2. 检查库存
        if (dto.Quantity > sku.Stock)
        {
            throw new BusinessException($"库存不足，当前库存 {sku.Stock} 件，您需要 {dto.Quantity} 件", 400);
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
                throw new BusinessException($"库存不足，当前库存 {sku.Stock} 件，购物车已有 {existingCart.Quantity} 件", 400);
            }

            existingCart.Quantity = newQuantity;
            existingCart.UpdatedAt = DateTime.Now;
            existingCart.UpdatedBy = memberId;
            await _db.Updateable(existingCart).ExecuteCommandAsync();

            _logger.LogInformation("会员 {MemberId} 更新购物车商品 {SkuId}，数量：{Quantity}",
                memberId, dto.SkuId, newQuantity);

            return existingCart.Id.ToString();
        }

        // 4. 新增购物车记录
        var cart = new Cart
        {
            MemberId = memberId,
            SkuId = dto.SkuId,
            Quantity = dto.Quantity,
            Selected = 1,
            CreatedBy = memberId
        };

        await _db.Insertable(cart).ExecuteCommandAsync();

        _logger.LogInformation("会员 {MemberId} 添加商品到购物车 {SkuId}，数量：{Quantity}",
            memberId, dto.SkuId, dto.Quantity);

        return cart.Id.ToString();
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

    #endregion

    #region 修改

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
            .Where(c => c.Id.ToString() == cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw new BusinessException($"购物车商品不存在: {cartId}", 404);
        }

        // 2. 验证数量是否合法
        if (quantity <= 0)
        {
            throw new BusinessException("数量必须大于0", 400);
        }

        // 3. 检查库存
        var sku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == cart.SkuId && s.IsDeleted == 0)
            .FirstAsync();

        if (sku != null && quantity > sku.Stock)
        {
            throw new BusinessException($"库存不足，当前库存 {sku.Stock} 件", 400);
        }

        // 4. 更新数量
        cart.Quantity = quantity;
        cart.UpdatedAt = DateTime.Now;
        cart.UpdatedBy = operatorId ?? cart.MemberId;

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
            .Where(c => cartIds.Contains(c.Id.ToString()) && c.IsDeleted == 0)
            .SetColumns(c => new Cart { Selected = selected, UpdatedAt = DateTime.Now })
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
            .SetColumns(c => new Cart { Selected = selected, UpdatedAt = DateTime.Now })
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
            .Where(c => c.Id.ToString() == cartId && c.MemberId == memberId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw new BusinessException($"购物车商品不存在: {cartId}", 404);
        }

        // 2. 验证新SKU是否存在
        var newSku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == newSkuId && s.IsDeleted == 0)
            .FirstAsync();

        if (newSku == null)
        {
            throw new BusinessException($"商品规格不存在: {newSkuId}", 404);
        }

        // 3. 检查新SKU是否已在购物车中
        var existingCart = await _db.Queryable<Cart>()
            .Where(c => c.MemberId == memberId && c.SkuId == newSkuId && c.Id.ToString() != cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (existingCart != null)
        {
            throw new BusinessException("该规格商品已在购物车中", 400);
        }

        // 4. 更新SKU
        cart.SkuId = newSkuId;
        cart.UpdatedAt = DateTime.Now;
        await _db.Updateable(cart).ExecuteCommandAsync();

        _logger.LogInformation("会员 {MemberId} 切换购物车 {CartId} 的SKU为 {NewSkuId}",
            memberId, cartId, newSkuId);

        return true;
    }

    #endregion

    #region 删除

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
            .Where(c => c.Id.ToString() == cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw new BusinessException($"购物车商品不存在: {cartId}", 404);
        }

        // 2. 软删除
        cart.IsDeleted = 1;
        cart.UpdatedAt = DateTime.Now;
        cart.UpdatedBy = operatorId ?? cart.MemberId;

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
            .Where(c => cartIds.Contains(c.Id.ToString()) && c.IsDeleted == 0)
            .SetColumns(c => new Cart
            {
                IsDeleted = 1,
                UpdatedAt = DateTime.Now,
                UpdatedBy = operatorId ?? c.MemberId
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
                UpdatedAt = DateTime.Now,
                UpdatedBy = memberId
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

    #endregion

    #region 其他查询

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
            .Where(c => c.Id.ToString() == cartId && c.IsDeleted == 0)
            .FirstAsync();

        if (cart == null)
        {
            throw new BusinessException($"购物车商品不存在: {cartId}", 404);
        }

        // 2. 获取会员购物车列表（复用 GetCartListAsync 的逻辑）
        var cartList = await GetCartListAsync(cart.MemberId);

        // 3. 查找指定购物车项
        var cartItem = cartList.Items.FirstOrDefault(i => i.Id == cartId);

        if (cartItem == null)
        {
            throw new BusinessException($"购物车商品不存在: {cartId}", 404);
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
        queryable = queryable.OrderByDescending(c => c.CreatedAt);

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
        var totalItems = await _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0)
            .CountAsync();

        // 2. 查询有购物车的会员数量
        var memberCount = await _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0)
            .GroupBy(c => c.MemberId)
            .CountAsync();

        // 3. 查询总金额
        var totalAmount = await _db.Queryable<Cart, product_sku>((c, s) => new JoinQueryInfos(
                JoinType.Left, c.SkuId == s.Id.ToString() && s.IsDeleted == 0
            ))
            .Where((c, s) => c.IsDeleted == 0)
            .SumAsync((c, s) => (s.MemberPrice ?? s.Price) * c.Quantity);

        // 4. 计算平均值
        var avgItemsPerMember = memberCount > 0 ? (decimal)totalItems / memberCount : 0;
        var avgAmountPerMember = memberCount > 0 ? totalAmount / memberCount : 0;

        // 5. 查询失效商品数
        var today = DateTime.Today;
        var invalidItems = await _db.Queryable<Cart, product_sku>((c, s) => new JoinQueryInfos(
                JoinType.Left, c.SkuId == s.Id.ToString()
            ))
            .Where((c, s) => c.IsDeleted == 0 && (s.Id == Guid.Empty || s.Status != Status.Enabled || c.Quantity > s.Stock))
            .CountAsync();

        // 6. 查询今日新增购物车商品数
        var todayNewItems = await _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0 && c.CreatedAt >= today)
            .CountAsync();

        // 7. 查询本周新增购物车商品数
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var weekNewItems = await _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0 && c.CreatedAt >= weekStart)
            .CountAsync();

        // 8. 查询本月新增购物车商品数
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthNewItems = await _db.Queryable<Cart>()
            .Where(c => c.IsDeleted == 0 && c.CreatedAt >= monthStart)
            .CountAsync();

        // 9. 返回统计结果
        return new CartStatisticsDto
        {
            TotalItems = totalItems,
            TotalAmount = totalAmount,
            MemberCount = memberCount,
            AvgItemsPerMember = Math.Round(avgItemsPerMember, 2),
            AvgAmountPerMember = Math.Round(avgAmountPerMember, 2),
            InvalidItems = invalidItems,
            TodayNewItems = todayNewItems,
            WeekNewItems = weekNewItems,
            MonthNewItems = monthNewItems
        };
    }

    #endregion
}