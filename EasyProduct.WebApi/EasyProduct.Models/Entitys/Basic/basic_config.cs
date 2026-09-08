using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 系统参数实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_config
/// 用于存储系统配置参数
/// </remarks>
[SugarTable("basic_config", "系统参数表")]
public class basic_config : BaseEntity
{
    /// <summary>
    /// 参数名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "参数名称")]
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 参数键名
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "参数键名")]
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 参数键值
    /// </summary>
    [SugarColumn(Length = 500, ColumnDescription = "参数键值")]
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// 系统内置：0=否，1=是
    /// </summary>
    [SugarColumn(ColumnDescription = "系统内置")]
    public int ConfigType { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }
}