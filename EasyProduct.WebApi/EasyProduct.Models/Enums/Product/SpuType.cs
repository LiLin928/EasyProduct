namespace EasyProduct.Models.Enums.Product;

/// <summary>
/// 商品类型枚举
/// </summary>
/// <remarks>
/// 用于区分商品的不同类型：
/// - 实物商品：需要物流配送的实体商品
/// - 虚拟商品：不需要物流，如充值卡、会员服务等
/// - 票品：演出票、景点门票等
/// </remarks>
public enum SpuType
{
    /// <summary>
    /// 实物商品
    /// </summary>
    /// <remarks>
    /// 需要物流配送的实体商品，如服装、电子产品等
    /// </remarks>
    Physical = 1,

    /// <summary>
    /// 虚拟商品
    /// </summary>
    /// <remarks>
    /// 不需要物流配送的商品，如充值卡、会员服务、优惠券等
    /// </remarks>
    Virtual = 2,

    /// <summary>
    /// 票品
    /// </summary>
    /// <remarks>
    /// 演出票、景点门票、电影票等
    /// </remarks>
    Ticket = 3
}