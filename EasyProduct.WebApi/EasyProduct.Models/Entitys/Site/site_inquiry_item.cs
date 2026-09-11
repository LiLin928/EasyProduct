using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Site;

/// <summary>
/// 询价明细实体
/// </summary>
/// <remarks>
/// 对应数据库表 site_inquiry_item
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、Status、CreatedAt、UpdatedAt、CreatedBy、UpdatedBy 字段
/// 用于管理询价单中的产品明细信息，支持中英文双语
/// </remarks>
[SugarTable("site_inquiry_item", "询价明细表")]
public class site_inquiry_item : BaseEntity
{
    /// <summary>
    /// 询价单ID
    /// </summary>
    /// <remarks>
    /// 关联 site_inquiry 表的ID
    /// 外键约束，级联删除
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "询价单ID")]
    public string InquiryId { get; set; } = string.Empty;

    /// <summary>
    /// 询价单号
    /// </summary>
    /// <remarks>
    /// 冗余字段，方便查询
    /// 与 site_inquiry.InquiryNo 保持一致
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "询价单号")]
    public string InquiryNo { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    /// <remarks>
    /// 询价的产品名称，必填字段
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "产品名称")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称（英文）
    /// </summary>
    /// <remarks>
    /// 询价的产品英文名称
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "产品名称（英文）")]
    public string? ProductNameEn { get; set; }

    /// <summary>
    /// 产品编码
    /// </summary>
    /// <remarks>
    /// 产品的编码或SKU
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "产品编码")]
    public string? ProductCode { get; set; }

    /// <summary>
    /// 规格型号
    /// </summary>
    /// <remarks>
    /// 产品的规格型号说明
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "规格型号")]
    public string? Specification { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    /// <remarks>
    /// 询价的产品数量
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "数量")]
    public int? Quantity { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    /// <remarks>
    /// 产品数量单位，如：件、箱、公斤等
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "单位")]
    public string? Unit { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    /// <remarks>
    /// 询价明细的备注信息
    /// </remarks>
    [SugarColumn(ColumnDataType = "TEXT", IsNullable = true, ColumnDescription = "备注信息")]
    public string? Remark { get; set; }
}