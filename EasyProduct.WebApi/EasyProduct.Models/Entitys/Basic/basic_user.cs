using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Constants;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 用户实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_user
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// </remarks>
[SugarTable("basic_user", "用户表")]
public class basic_user : BaseEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码（BCrypt加密）
    /// </summary>
    [SugarColumn(Length = 255, ColumnDescription = "密码")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "真实姓名")]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "手机号")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "头像URL")]
    public string? Avatar { get; set; }
}
