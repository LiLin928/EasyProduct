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
}