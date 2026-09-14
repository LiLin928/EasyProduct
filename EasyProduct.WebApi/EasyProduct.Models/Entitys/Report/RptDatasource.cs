using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Constants;
using EasyProduct.Models.Entitys.Base;
using SqlSugar;

namespace EasyProduct.Models.Entitys.Report;

/// <summary>
/// 报表数据源
/// </summary>
[SugarTable("rpt_datasource", "报表数据源表")]
public class RptDatasource : BaseEntity
{
    /// <summary>
    /// 数据源名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "数据源名称不能为空")]
    [MaxLength(100, ErrorMessage = "数据源名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型：mysql/postgresql/sqlserver/oracle
    /// </summary>
    [SugarColumn(Length = 20)]
    [Required(ErrorMessage = "数据源类型不能为空")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 主机地址
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "主机地址不能为空")]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// 端口号
    /// </summary>
    [Required(ErrorMessage = "端口号不能为空")]
    [Range(1, 65535, ErrorMessage = "端口号必须在1-65535之间")]
    public int Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "数据库名称不能为空")]
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（加密存储）
    /// </summary>
    [SugarColumn(Length = 500)]
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 连接状态：1-已连接，2-连接错误
    /// </summary>
    [SugarColumn(ColumnDescription = "连接状态")]
    public int Status { get; set; } = ReportConstants.DatasourceStatus.Error;

    /// <summary>
    /// 最后测试时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后测试时间", IsNullable = true)]
    public DateTime? LastTestTime { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}