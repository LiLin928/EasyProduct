using SqlSugar;

namespace EasyProduct.Models.Entitys;

/// <summary>
/// 实体基类
/// </summary>
/// <remarks>
/// 所有实体类的基类，包含通用字段：主键、创建时间、更新时间、创建人、软删除标记
/// </remarks>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID（GUID）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

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
    /// <remarks>
    /// 管理端=用户名；系统任务=system
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CreateBy { get; set; }

    /// <summary>
    /// 软删除标记
    /// </summary>
    public bool IsDeleted { get; set; }
}