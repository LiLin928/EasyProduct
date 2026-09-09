using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Mall.Member;

/// <summary>
/// 微信登录DTO
/// </summary>
/// <remarks>
/// 用于小程序微信授权登录
/// code 为必填字段，通过 wx.login() 获取
/// EncryptedData 和 Iv 为可选字段，用于获取用户加密信息
/// </remarks>
public class WechatLoginDto
{
    /// <summary>
    /// 微信登录code
    /// </summary>
    /// <remarks>
    /// 必填，通过小程序 wx.login() 获取
    /// 最大长度 100 个字符
    /// </remarks>
    [Required(ErrorMessage = "code不能为空")]
    [MaxLength(100, ErrorMessage = "code长度不能超过100个字符")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// 用户信息加密数据
    /// </summary>
    /// <remarks>
    /// 可选，首次登录时用于解密用户信息
    /// 包含昵称、头像等信息
    /// </remarks>
    public string? EncryptedData { get; set; }

    /// <summary>
    /// 加密算法初始向量
    /// </summary>
    /// <remarks>
    /// 可选，配合 EncryptedData 使用
    /// </remarks>
    public string? Iv { get; set; }
}