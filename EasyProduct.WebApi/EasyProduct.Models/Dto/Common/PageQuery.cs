namespace EasyProduct.Models.Dto.Common;

/// <summary>
/// 分页查询基类
/// </summary>
/// <remarks>
/// 所有分页查询 DTO 的基类，提供统一的分页参数
/// </remarks>
public class PageQuery
{
    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    /// <remarks>
    /// 默认值为 1，表示第一页
    /// </remarks>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <remarks>
    /// 默认值为 10，表示每页显示 10 条记录
    /// </remarks>
    public int PageSize { get; set; } = 10;
}