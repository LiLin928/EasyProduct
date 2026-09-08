using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 字典类型实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_dict_type
/// 用于存储字典的分类信息，如：订单状态、用户状态等
/// </remarks>
[SugarTable("basic_dict_type", "字典类型表")]
public class basic_dict_type : BaseEntity
{
    /// <summary>
    /// 字典名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "字典名称")]
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型（唯一标识）
    /// </summary>
    /// <remarks>
    /// 用于标识字典类型，如：order_status、user_status
    /// 必须唯一，作为字典数据的分类标识
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "字典类型")]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}

/// <summary>
/// 字典数据实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_dict_data
/// 用于存储字典的具体数据项
/// </remarks>
[SugarTable("basic_dict_data", "字典数据表")]
public class basic_dict_data : BaseEntity
{
    /// <summary>
    /// 字典类型
    /// </summary>
    /// <remarks>
    /// 关联 basic_dict_type 表的 DictType 字段
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "字典类型")]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 字典标签
    /// </summary>
    /// <remarks>
    /// 显示给用户看的标签，如：待支付、已支付
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "字典标签")]
    public string DictLabel { get; set; } = string.Empty;

    /// <summary>
    /// 字典值
    /// </summary>
    /// <remarks>
    /// 实际存储的值，如：pending、paid
    /// 与前端常量保持一致
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "字典值")]
    public string DictValue { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}