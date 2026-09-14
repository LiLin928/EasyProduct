using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 供应商实体类
/// </summary>
/// <remarks>
/// 供应商主表，用于管理供应商档案信息。
/// 供应商编码：S + 年月 + 序号（如 S2026090001）
/// 供应商状态：启用/禁用
/// 关联关系：
/// - 采购订单：crm_purchase_order 表
/// </remarks>
[SugarTable("crm_supplier", "供应商表")]
public class Supplier : BaseEntity
{
    /// <summary>
    /// 供应商编码（唯一）
    /// </summary>
    /// <remarks>
    /// 格式：S + 年月 + 序号（如 S2026090001）
    /// 自动生成，唯一标识
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "供应商编码")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 供应商名称
    /// </summary>
    /// <remarks>
    /// 供应商企业名称
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "供应商名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名
    /// </summary>
    /// <remarks>
    /// 主要联系人姓名
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "联系人姓名", IsNullable = true)]
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <remarks>
    /// 主要联系电话
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话", IsNullable = true)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    /// <remarks>
    /// 主要联系邮箱
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "联系邮箱", IsNullable = true)]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    /// <remarks>
    /// 供应商地址
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "地址", IsNullable = true)]
    public string? Address { get; set; }

    /// <summary>
    /// 开户行
    /// </summary>
    /// <remarks>
    /// 银行开户行名称
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "开户行", IsNullable = true)]
    public string? BankName { get; set; }

    /// <summary>
    /// 银行账号
    /// </summary>
    /// <remarks>
    /// 银行账号
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "银行账号", IsNullable = true)]
    public string? BankAccount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 备注信息
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "备注", IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>
/// 供应商资质实体类
/// </summary>
/// <remarks>
/// 供应商资质表，用于管理供应商的资质证书。
/// 资质类型：营业执照、生产许可证、质量认证等。
/// 支持资质到期提醒。
/// </remarks>
[SugarTable("crm_supplier_qualification", "供应商资质表")]
public class SupplierQualification : BaseEntity
{
    /// <summary>
    /// 供应商ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_supplier 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "供应商ID")]
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 资质类型
    /// </summary>
    /// <remarks>
    /// BusinessLicense = 营业执照，ProductionLicense = 生产许可证，QualityCertification = 质量认证
    /// </remarks>
    [SugarColumn(ColumnDescription = "资质类型")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 资质名称
    /// </summary>
    /// <remarks>
    /// 资质证书名称
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "资质名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 证书编号
    /// </summary>
    /// <remarks>
    /// 资质证书编号
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "证书编号", IsNullable = true)]
    public string? CertificateNo { get; set; }

    /// <summary>
    /// 发证日期
    /// </summary>
    /// <remarks>
    /// 资质证书发证日期
    /// </remarks>
    [SugarColumn(ColumnDescription = "发证日期", IsNullable = true)]
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// 有效期
    /// </summary>
    /// <remarks>
    /// 资质证书有效期
    /// 过期自动标记状态为"过期"
    /// </remarks>
    [SugarColumn(ColumnDescription = "有效期", IsNullable = true)]
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 证件图片URL
    /// </summary>
    /// <remarks>
    /// 资质证书图片地址
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "证件图片URL", IsNullable = true)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 资质状态
    /// </summary>
    /// <remarks>
    /// 0=过期，1=有效
    /// 根据有效期自动判断
    /// </remarks>
    [SugarColumn(ColumnDescription = "资质状态")]
    public int QualificationStatus { get; set; } = 1;
}