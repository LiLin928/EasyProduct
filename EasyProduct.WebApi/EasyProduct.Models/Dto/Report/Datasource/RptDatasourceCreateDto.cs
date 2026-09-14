using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Report.Datasource;

/// <summary>
/// 创建数据源参数
/// </summary>
public class RptDatasourceCreateDto
{
    /// <summary>
    /// 数据源名称
    /// </summary>
    [Required(ErrorMessage = "数据源名称不能为空")]
    [MaxLength(100, ErrorMessage = "数据源名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型
    /// </summary>
    [Required(ErrorMessage = "数据源类型不能为空")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 主机地址
    /// </summary>
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
    [Required(ErrorMessage = "数据库名称不能为空")]
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 备注说明
    /// </summary>
    [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
    public string? Remark { get; set; }
}