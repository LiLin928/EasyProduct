namespace EasyProduct.Models.Dto.Report.Datasource;

/// <summary>
/// 数据源详情
/// </summary>
public class RptDatasourceDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 数据源名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 主机地址
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// 端口号
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（脱敏：******）
    /// </summary>
    public string Password { get; set; } = "******";

    /// <summary>
    /// 连接状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 最后测试时间
    /// </summary>
    public DateTime? LastTestTime { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}