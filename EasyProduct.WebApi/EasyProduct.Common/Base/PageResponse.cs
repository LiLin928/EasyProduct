namespace EasyProduct.Common.Base;

/// <summary>
/// 分页响应格式，用于封装分页查询的结果
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
public class PageResponse<T>
{
    /// <summary>
    /// 数据列表，包含当前页的所有数据项
    /// </summary>
    public List<T> List { get; set; } = new();

    /// <summary>
    /// 总记录数，表示查询结果的总数量
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码，从 1 开始计数
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页数量，表示每页显示的记录数
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数，根据 Total 和 PageSize 计算得出
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevPage { get; set; }

    /// <summary>
    /// 创建分页响应对象
    /// </summary>
    public static PageResponse<T> Create(List<T> items, int total, int pageIndex, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PageResponse<T>
        {
            List = items,
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasNextPage = pageIndex < totalPages,
            HasPrevPage = pageIndex > 1
        };
    }

    /// <summary>
    /// 创建空的分页响应对象
    /// </summary>
    public static PageResponse<T> Empty(int pageIndex = 1, int pageSize = 10)
    {
        return Create(new List<T>(), 0, pageIndex, pageSize);
    }
}
