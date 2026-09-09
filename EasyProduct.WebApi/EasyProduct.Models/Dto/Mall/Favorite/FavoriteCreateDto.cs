using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Favorite;

/// <summary>
/// 收藏创建DTO
/// </summary>
/// <remarks>
/// 用于创建收藏记录，仅需要提供商品SPU ID
/// 会员ID从登录信息中获取
/// </remarks>
public class FavoriteCreateDto
{
    /// <summary>
    /// 商品SPU ID
    /// </summary>
    /// <remarks>
    /// 必填，GUID 格式
    /// </remarks>
    [Required(ErrorMessage = "商品ID不能为空")]
    public Guid SpuId { get; set; }
}