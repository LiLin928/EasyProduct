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
}