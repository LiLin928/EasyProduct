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