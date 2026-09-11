using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Site;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 留言实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_contact
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理官网提交的留言信息，支持中英文双语
/// </remarks>
[SugarTable("site_contact", "留言表")]
public class site_contact : BaseEntity
{
    /// <summary>
    /// 姓名
    /// </summary>
    /// <remarks>
    /// 提交留言的姓名，必填字段
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "姓名")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 姓名（英文）
    /// </summary>
    /// <remarks>
    /// 提交留言的英文姓名
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "姓名（英文）")]
    public string? NameEn { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <remarks>
    /// 提交留言的联系电话
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "联系电话")]
    public string? Phone { get; set; }

    /// <summary>
    /// 电子邮箱
    /// </summary>
    /// <remarks>
    /// 提交留言的电子邮箱
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "电子邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    /// <remarks>
    /// 提交留言的公司名称
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "公司名称")]
    public string? Company { get; set; }

    /// <summary>
    /// 公司名称（英文）
    /// </summary>
    /// <remarks>
    /// 提交留言的公司英文名称
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "公司名称（英文）")]
    public string? CompanyEn { get; set; }

    /// <summary>
    /// 主题
    /// </summary>
    /// <remarks>
    /// 留言的主题
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "主题")]
    public string? Subject { get; set; }

    /// <summary>
    /// 主题（英文）
    /// </summary>
    /// <remarks>
    /// 留言的英文主题
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "主题（英文）")]
    public string? SubjectEn { get; set; }

    /// <summary>
    /// 留言内容
    /// </summary>
    /// <remarks>
    /// 提交留言的详细内容，必填字段
    /// 支持大量文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "TEXT", ColumnDescription = "留言内容")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 留言内容（英文）
    /// </summary>
    /// <remarks>
    /// 提交留言的英文详细内容
    /// 支持大量文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "留言内容（英文）")]
    public string? MessageEn { get; set; }

    /// <summary>
    /// 留言状态
    /// </summary>
    /// <remarks>
    /// 使用 ContactStatus 枚举：
    /// - Unread = 0（未读）
    /// - Read = 1（已读）
    /// - Replied = 2（已回复）
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态")]
    public ContactStatus Status { get; set; } = ContactStatus.Unread;

    /// <summary>
    /// 回复内容
    /// </summary>
    /// <remarks>
    /// 管理员回复的内容
    /// 支持大量文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "回复内容")]
    public string? Reply { get; set; }

    /// <summary>
    /// 回复内容（英文）
    /// </summary>
    /// <remarks>
    /// 管理员回复的英文内容
    /// 支持大量文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "回复内容（英文）")]
    public string? ReplyEn { get; set; }

    /// <summary>
    /// 回复时间
    /// </summary>
    /// <remarks>
    /// 管理员回复留言的时间
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "回复时间")]
    public DateTime? RepliedAt { get; set; }

    /// <summary>
    /// 回复人ID
    /// </summary>
    /// <remarks>
    /// 回复留言的管理员用户ID（GUID 字符串）
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "回复人ID")]
    public string? RepliedBy { get; set; }
}