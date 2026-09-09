using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Mall;

/// <summary>
/// 订单项实体类
/// </summary>
/// <remarks>
/// 订单明细表，记录订单中每个商品的详细信息。
/// 采用快照模式，记录下单时的商品信息（名称、规格、价格等），
/// 确保订单数据不受商品信息变更的影响。
/// 支持部分退款，记录退款状态和退款数量。
/// </remarks>
[SugarTable("mall_order_item", "订单项表")]
public class OrderItem : BaseEntity
{
    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 关联订单主表 mall_order 的 Id 字段。
    /// 订单项归属于该订单。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// SKU ID
    /// </summary>
    /// <remarks>
    /// 关联商品SKU表 product_sku 的 Id 字段。
    /// 用于售后、库存扣减等业务操作。
    /// </remarks>
    [SugarColumn(Length = 36, IsNullable = false)]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    /// <remarks>
    /// 下单时的商品名称快照。
    /// 记录商品名称，不受后续商品信息变更影响。
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// SKU名称
    /// </summary>
    /// <remarks>
    /// 下单时的SKU名称快照。
    /// 记录SKU名称，不受后续SKU信息变更影响。
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string SkuName { get; set; } = string.Empty;

    /// <summary>
    /// SKU规格（JSON）
    /// </summary>
    /// <remarks>
    /// 下单时的SKU规格信息快照。
    /// JSON 格式存储，如：[{"key":"颜色","value":"红色"},{"key":"尺码","value":"L"}]。
    /// 用于前端展示和售后处理。
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? SkuSpec { get; set; }

    /// <summary>
    /// 商品图片
    /// </summary>
    /// <remarks>
    /// 下单时的商品主图快照。
    /// 记录商品图片URL，不受后续商品信息变更影响。
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string ProductImage { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    /// <remarks>
    /// 下单时的商品单价快照。
    /// 记录下单时的商品单价，不受后续价格调整影响。
    /// 保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    /// <remarks>
    /// 购买数量。
    /// 数量 * 单价 = 小计金额。
    /// </remarks>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 小计
    /// </summary>
    /// <remarks>
    /// 商品小计金额 = 单价 * 数量。
    /// 保留 2 位小数。
    /// </remarks>
    [SugarColumn(Length = 18, DecimalDigits = 2, IsNullable = false)]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 是否已退款（0否1是）
    /// </summary>
    /// <remarks>
    /// 退款状态标记：
    /// - 0: 未退款
    /// - 1: 已退款（部分或全部）
    /// 用于判断商品是否已申请退款。
    /// </remarks>
    [SugarColumn(IsNullable = false)]
    public int IsRefunded { get; set; } = 0;

    /// <summary>
    /// 已退款数量
    /// </summary>
    /// <remarks>
    /// 已退款商品数量。
    /// - 0: 未退款
    /// - 等于 Quantity: 全部退款
    /// - 小于 Quantity: 部分退款
    /// 支持部分退款场景。
    /// </remarks>
    [SugarColumn(IsNullable = false)]
    public int RefundQuantity { get; set; } = 0;
}