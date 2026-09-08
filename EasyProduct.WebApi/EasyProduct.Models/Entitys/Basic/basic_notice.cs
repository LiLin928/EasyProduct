using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 公告实体
/// </summary>
/// <remarks>
/// 对应数据库表 basic_notice
/// 用于存储系统公告和通知信息
/// </remarks>
[SugarTable("basic_notice", "公告表")]
public class basic_notice : BaseEntity
{
    /// <summary>
    /// 公告标题
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "公告标题")]
    public string NoticeTitle { get; set; } = string.Empty;

    /// <summary>
    /// 公告内容（富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "TEXT", ColumnDescription = "公告内容")]
    public string NoticeContent { get; set; } = string.Empty;

    /// <summary>
    /// 公告类型：1=通知，2=公告
    /// </summary>
    [SugarColumn(ColumnDescription = "公告类型")]
    public int NoticeType { get; set; } = 1;

    /// <summary>
    /// 是否置顶：0=否，1=是
    /// </summary>
    [SugarColumn(ColumnDescription = "是否置顶")]
    public int TopFlag { get; set; } = 0;

    /// <summary>
    /// 发布时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "发布时间")]
    public DateTime? PublishTime { get; set; }
}