namespace EasyProduct.Models.Dto.Basic.Dept;

/// <summary>
/// 部门DTO
/// </summary>
public class DeptDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 上级部门ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 部门编码
    /// </summary>
    public string? DeptCode { get; set; }

    /// <summary>
    /// 部门负责人ID
    /// </summary>
    public string? LeaderId { get; set; }

    /// <summary>
    /// 部门负责人姓名
    /// </summary>
    public string? LeaderName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 部门完整路径
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// 部门层级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreateBy { get; set; }

    /// <summary>
    /// 子部门列表
    /// </summary>
    public List<DeptDto>? Children { get; set; }
}

/// <summary>
/// 部门树形结构DTO
/// </summary>
public class DeptTreeDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 上级部门ID
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// 部门编码
    /// </summary>
    public string? DeptCode { get; set; }

    /// <summary>
    /// 部门负责人姓名
    /// </summary>
    public string? LeaderName { get; set; }

    /// <summary>
    /// 部门完整路径
    /// </summary>
    public string? FullPath { get; set; }

    /// <summary>
    /// 部门层级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 子部门列表
    /// </summary>
    public List<DeptTreeDto>? Children { get; set; }
}