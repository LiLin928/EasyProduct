using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Basic;

/// <summary>
/// 角色查询 DTO
/// </summary>
public class RoleQueryDto
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 角色名称关键词
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    public string? RoleCode { get; set; }

    /// <summary>
    /// 状态筛选：0=禁用，1=启用
    /// </summary>
    public Status? Status { get; set; }
}