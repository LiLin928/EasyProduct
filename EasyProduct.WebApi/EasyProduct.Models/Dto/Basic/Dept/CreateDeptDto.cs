using System.ComponentModel.DataAnnotations;

namespace EasyProduct.Models.Dto.Basic.Dept;

/// <summary>
/// 创建部门参数
/// </summary>
public class CreateDeptDto
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [Required(ErrorMessage = "部门名称不能为空")]
    [MaxLength(100, ErrorMessage = "部门名称不能超过100个字符")]
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 上级部门ID（根部门传空字符串或"0"）
    /// </summary>
    [MaxLength(36)]
    public string? ParentId { get; set; }

    /// <summary>
    /// 部门编码
    /// </summary>
    [MaxLength(50, ErrorMessage = "部门编码不能超过50个字符")]
    public string? DeptCode { get; set; }

    /// <summary>
    /// 部门负责人ID
    /// </summary>
    [MaxLength(36)]
    public string? LeaderId { get; set; }

    /// <summary>
    /// 部门负责人姓名
    /// </summary>
    [MaxLength(50)]
    public string? LeaderName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [MaxLength(20)]
    [RegularExpression(@"^[\d\-+\s]*$", ErrorMessage = "联系电话格式不正确")]
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    [MaxLength(100)]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "排序值必须大于等于0")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}