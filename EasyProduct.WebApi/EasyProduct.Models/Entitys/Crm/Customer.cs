using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 客户实体类
/// </summary>
/// <remarks>
/// 客户主表，用于管理客户档案信息。
/// 客户类型：
/// - B2B 客户：企业客户，来源于询价转化或手动建档
/// - 零售客户：个人客户，来源于小程序会员首单自动建档
///
/// 客户来源：
/// - 询价转化：官网询价审核通过后一键转客户
/// - 注册建档：小程序会员首单自动创建客户档案
/// - 手动建档：后台管理员手动创建
///
/// 关联关系：
/// - 业务员：关联 basic_user 表
/// - 会员：mall_member 表的 customer_id 字段
/// - 询价单：site_inquiry 表
/// </remarks>
[SugarTable("crm_customer", "客户表")]
public class Customer : BaseEntity
{
    /// <summary>
    /// 客户编码（唯一）
    /// </summary>
    /// <remarks>
    /// 格式：C + 年月 + 序号（如 C2026090001）
    /// 自动生成，唯一标识
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "客户编码")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    /// <remarks>
    /// 企业名称或个人姓名
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "客户名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客户类型
    /// </summary>
    /// <remarks>
    /// B2B = 企业客户，Retail = 零售客户
    /// </remarks>
    [SugarColumn(ColumnDescription = "客户类型")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 客户来源
    /// </summary>
    /// <remarks>
    /// Inquiry = 询价转化，Register = 注册建档，Manual = 手动建档
    /// </remarks>
    [SugarColumn(ColumnDescription = "客户来源")]
    public int Source { get; set; } = 3;

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
    /// 客户地址
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "地址", IsNullable = true)]
    public string? Address { get; set; }

    /// <summary>
    /// 归属业务员ID
    /// </summary>
    /// <remarks>
    /// 关联 basic_user 表的 Id 字段
    /// 可为空，未分配业务员的客户此字段为空
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "归属业务员ID", IsNullable = true)]
    public string? BusinessUserId { get; set; }

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
/// 客户联系人实体类
/// </summary>
/// <remarks>
/// 客户联系人表，用于管理客户的多个联系人。
/// 一个客户可以有多个联系人。
/// 支持设置主要联系人。
/// </remarks>
[SugarTable("crm_customer_contact", "客户联系人表")]
public class CustomerContact : BaseEntity
{
    /// <summary>
    /// 客户ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_customer 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "客户ID")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 联系人姓名
    /// </summary>
    /// <remarks>
    /// 联系人姓名
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "联系人姓名")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <remarks>
    /// 联系电话
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话", IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <remarks>
    /// 联系邮箱
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "邮箱", IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    /// <remarks>
    /// 联系人职位
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "职位", IsNullable = true)]
    public string? Position { get; set; }

    /// <summary>
    /// 是否主要联系人
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// 一个客户只能有一个主要联系人
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否主要联系人")]
    public int IsPrimary { get; set; } = 0;
}

/// <summary>
/// 客户地址实体类
/// </summary>
/// <remarks>
/// 客户地址表，用于管理客户的多个收货地址。
/// 一个客户可以有多个地址。
/// 支持设置默认地址。
/// </remarks>
[SugarTable("crm_customer_address", "客户地址表")]
public class CustomerAddress : BaseEntity
{
    /// <summary>
    /// 客户ID
    /// </summary>
    /// <remarks>
    /// 关联 crm_customer 表的 Id 字段
    /// </remarks>
    [SugarColumn(ColumnDataType = "varchar(36)", ColumnDescription = "客户ID")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓名
    /// </summary>
    /// <remarks>
    /// 收货人姓名
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "收货人姓名")]
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <remarks>
    /// 收货人联系电话
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 省份
    /// </summary>
    /// <remarks>
    /// 省份名称
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "省份")]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    /// <remarks>
    /// 城市名称
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "城市")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    /// <remarks>
    /// 区县名称
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "区县", IsNullable = true)]
    public string? District { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    /// <remarks>
    /// 详细地址（街道、门牌号等）
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "详细地址")]
    public string DetailAddress { get; set; } = string.Empty;

    /// <summary>
    /// 是否默认地址
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// 一个客户只能有一个默认地址
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否默认地址")]
    public int IsDefault { get; set; } = 0;
}