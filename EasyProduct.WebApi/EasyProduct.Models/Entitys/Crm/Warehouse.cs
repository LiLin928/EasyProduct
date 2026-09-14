using SqlSugar;
using System;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 仓库实体类
/// </summary>
/// <remarks>
/// 仓库表，用于管理企业的仓库信息。
///
/// 业务规则：
/// - 仓库编码唯一，不可重复
/// - 仓库状态通过 BaseEntity.Status 字段管理（Enabled=启用，Disabled=停用）
/// - 仓库可关联库存账面、出入库流水、盘点单等
///
/// 数据关联：
/// - 库存账面（crm_stock）：一个仓库可以有多个库存记录
/// - 出入库流水（crm_stock_record）：记录仓库的出入库历史
/// - 盘点单（crm_stock_check）：对仓库库存进行盘点
/// </remarks>
[SugarTable("crm_warehouse", "仓库表")]
public class Warehouse : BaseEntity
{
    /// <summary>
    /// 仓库编码（唯一）
    /// </summary>
    /// <remarks>
    /// 格式：WH001、WH002...
    /// 必须唯一，不可重复
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "仓库编码")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 仓库名称
    /// </summary>
    /// <remarks>
    /// 例如：深圳主仓、上海分仓、广州临时仓
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "仓库名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 仓库地址
    /// </summary>
    /// <remarks>
    /// 详细地址，包括省市区街道门牌号
    /// 例如：深圳市南山区科技园北区
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "仓库地址")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 负责人姓名
    /// </summary>
    /// <remarks>
    /// 仓库管理员的姓名
    /// 例如：李建国、王芳
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "负责人姓名")]
    public string Manager { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <remarks>
    /// 仓库管理员的联系电话
    /// 例如：13800138001
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    /// <remarks>
    /// 可选字段，用于记录仓库的其他信息
    /// 例如：主营仓，存放全部品类
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "备注", IsNullable = true)]
    public string? Remark { get; set; }
}