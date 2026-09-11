using System.ComponentModel.DataAnnotations;
using EasyProduct.Models.Enums;

namespace EasyProduct.Models.Dto.Site.Video;

/// <summary>
/// 视频查询 DTO
/// </summary>
/// <remarks>
/// 用于视频列表查询，支持按标题、分类、状态筛选
/// 包含分页参数
/// </remarks>
public class VideoQueryDto
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
    /// 标题关键词
    /// </summary>
    /// <remarks>
    /// 支持模糊搜索，最大长度 200 个字符
    /// </remarks>
    [StringLength(200, ErrorMessage = "标题长度不能超过200个字符")]
    public string? Title { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    /// <remarks>
    /// 精确匹配分类
    /// </remarks>
    [StringLength(50, ErrorMessage = "分类长度不能超过50个字符")]
    public string? Category { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    /// <remarks>
    /// 使用 Status 枚举：Disabled（0）= 禁用，Enabled（1）= 启用
    /// </remarks>
    public Status? Status { get; set; }

    /// <summary>
    /// 视频类型
    /// </summary>
    /// <remarks>
    /// 精确匹配视频类型
    /// </remarks>
    [StringLength(20, ErrorMessage = "视频类型长度不能超过20个字符")]
    public string? VideoType { get; set; }
}