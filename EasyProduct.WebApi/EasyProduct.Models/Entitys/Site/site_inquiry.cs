using SqlSugar;
using EasyProduct.Models.Entitys.Base;
using EasyProduct.Models.Enums.Site;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 询价单实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_inquiry
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理官网提交的询价单信息，支持中英文双语
/// </remarks>
[SugarTable("site_inquiry", "询价单表")]
public class site_inquiry : BaseEntity
{
    /// <summary>
    /// 询价单号
    /// </summary>
    /// <remarks>
    /// 格式：INQ + yyyyMMdd + 4位序号
    /// 例如：INQ202609110001
    /// 唯一索引，不允许重复
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "询价单号")]
    public string InquiryNo { get; set; } = string.Empty;

    /// <summary>
    /// 公司名称
    /// </summary>
    /// <remarks>
    /// 提交询价的公司名称，必填字段
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "公司名称")]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 公司名称（英文）
    /// </summary>
    /// <remarks>
    /// 提交询价的公司英文名称
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "公司名称（英文）")]
    public string? CompanyNameEn { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    /// <remarks>
    /// 提交询价的联系人姓名，必填字段
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "联系人姓名")]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名（英文）
    /// </summary>
    /// <remarks>
    /// 提交询价的联系人英文姓名
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "联系人姓名（英文）")]
    public string? ContactNameEn { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <remarks>
    /// 提交询价的联系电话，必填字段
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 电子邮箱
    /// </summary>
    /// <remarks>
    /// 提交询价的电子邮箱
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "电子邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    /// <remarks>
    /// 提交询价的公司所在国家
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "国家")]
    public string? Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    /// <remarks>
    /// 提交询价的公司所在省份
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "省份")]
    public string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    /// <remarks>
    /// 提交询价的公司所在城市
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "城市")]
    public string? City { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    /// <remarks>
    /// 提交询价的公司详细地址
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "详细地址")]
    public string? Address { get; set; }

    /// <summary>
    /// 详细地址（英文）
    /// </summary>
    /// <remarks>
    /// 提交询价的公司详细英文地址
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "详细地址（英文）")]
    public string? AddressEn { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    /// <remarks>
    /// 询价单的备注信息，支持大量文本
    /// </remarks>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "备注信息")]
    public string? Remark { get; set; }

    /// <summary>
    /// 询价单状态
    /// </summary>
    /// <remarks>
    /// 使用 InquiryStatus 枚举：
    /// - Pending = 0（待处理）
    /// - Followed = 1（已跟进）
    /// - Converted = 2（已转客户）
    /// - Closed = 3（已关闭）
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态")]
    public InquiryStatus Status { get; set; } = InquiryStatus.Pending;

    /// <summary>
    /// 客户ID
    /// </summary>
    /// <remarks>
    /// 转客户后关联 crm_customer 表的ID
    /// 未转客户时为 null
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "客户ID")]
    public string? CustomerId { get; set; }

    /// <summary>
    /// 跟进时间
    /// </summary>
    /// <remarks>
    /// 询价单被标记为"已跟进"的时间
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "跟进时间")]
    public DateTime? FollowedAt { get; set; }

    /// <summary>
    /// 跟进人ID
    /// </summary>
    /// <remarks>
    /// 标记询价单为"已跟进"的用户ID（GUID 字符串）
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "跟进人ID")]
    public string? FollowedBy { get; set; }

    /// <summary>
    /// 转客户时间
    /// </summary>
    /// <remarks>
    /// 询价单被转为客户的时间
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "转客户时间")]
    public DateTime? ConvertedAt { get; set; }

    /// <summary>
    /// 转客户操作人ID
    /// </summary>
    /// <remarks>
    /// 执行转客户操作的用户ID（GUID 字符串）
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "转客户操作人ID")]
    public string? ConvertedBy { get; set; }

    /// <summary>
    /// 关闭时间
    /// </summary>
    /// <remarks>
    /// 询价单被关闭的时间
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "关闭时间")]
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// 关闭操作人ID
    /// </summary>
    /// <remarks>
    /// 执行关闭操作的用户ID（GUID 字符串）
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", IsNullable = true, ColumnDescription = "关闭操作人ID")]
    public string? ClosedBy { get; set; }

    /// <summary>
    /// 关闭原因
    /// </summary>
    /// <remarks>
    /// 询价单被关闭的原因说明
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "关闭原因")]
    public string? CloseReason { get; set; }

    /// <summary>
    /// 询价明细列表
    /// </summary>
    /// <remarks>
    /// 导航属性，关联 site_inquiry_item 表
    /// 不存储在数据库中
    /// </remarks>
    [SugarColumn(IsIgnore = true)]
    public List<site_inquiry_item>? Items { get; set; }
}