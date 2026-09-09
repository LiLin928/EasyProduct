using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Product;

/// <summary>
/// 商品渠道发布实体
/// </summary>
/// <remarks>
/// 对应数据库表 product_channel
/// 继承 BaseEntity 基类，自动包含 Id、IsDeleted、CreatedAt、UpdatedAt、CreatedBy 字段
/// 用于管理商品在不同渠道（官网、小程序、B2B）的发布状态和配置
/// </remarks>
[SugarTable("product_channel", "商品渠道发布表")]
public class product_channel : BaseEntity
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 关联商品主档表（product_spu）的主键ID
    /// </remarks>
    [SugarColumn(ColumnDataType = "char(36)", ColumnDescription = "商品ID")]
    public string SpuId { get; set; } = string.Empty;

    /// <summary>
    /// 渠道编码
    /// </summary>
    /// <remarks>
    /// 渠道的唯一编码标识
    /// site=官网，miniapp=小程序，b2b=B2B
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "渠道编码")]
    public string ChannelCode { get; set; } = string.Empty;

    /// <summary>
    /// 上架状态
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的上架状态
    /// 0=下架，1=上架
    /// </remarks>
    [SugarColumn(ColumnDescription = "上架状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 渠道排序
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的显示顺序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "渠道排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 渠道价格（可选）
    /// </summary>
    /// <remarks>
    /// 该渠道的特殊价格，为空则使用SKU价格
    /// 允许不同渠道设置不同的销售价格
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = true, ColumnDescription = "渠道价格")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否显示价格
    /// </summary>
    /// <remarks>
    /// 控制该渠道是否显示商品价格
    /// true=显示，false=隐藏
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否显示价格")]
    public bool ShowPrice { get; set; } = true;

    /// <summary>
    /// 是否显示库存
    /// </summary>
    /// <remarks>
    /// 控制该渠道是否显示商品库存
    /// true=显示，false=隐藏
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否显示库存")]
    public bool ShowStock { get; set; } = true;

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的首次上架时间
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "发布时间")]
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 下架时间
    /// </summary>
    /// <remarks>
    /// 商品在该渠道的最后下架时间
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "下架时间")]
    public DateTime? UnpublishTime { get; set; }
}