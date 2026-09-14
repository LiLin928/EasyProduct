using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Crm;

/// <summary>
/// 币种实体类
/// </summary>
/// <remarks>
/// 币种表，用于管理系统支持的币种。
/// 支持多币种采购。
/// 默认币种：人民币（CNY）。
/// </remarks>
[SugarTable("crm_currency", "币种表")]
public class Currency : BaseEntity
{
    /// <summary>
    /// 币种代码
    /// </summary>
    /// <remarks>
    /// 币种代码，如：CNY（人民币）、USD（美元）、EUR（欧元）
    /// </remarks>
    [SugarColumn(Length = 10, ColumnDescription = "币种代码")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 币种名称
    /// </summary>
    /// <remarks>
    /// 币种名称，如：人民币、美元、欧元
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "币种名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 符号
    /// </summary>
    /// <remarks>
    /// 币种符号，如：¥、$、€
    /// </remarks>
    [SugarColumn(Length = 10, ColumnDescription = "符号")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// 汇率
    /// </summary>
    /// <remarks>
    /// 对人民币的汇率
    /// 如：USD 的汇率为 7.2，表示 1 美元 = 7.2 人民币
    /// </remarks>
    [SugarColumn(ColumnDescription = "汇率", DecimalDigits = 6)]
    public decimal ExchangeRate { get; set; } = 1.0m;

    /// <summary>
    /// 是否默认币种
    /// </summary>
    /// <remarks>
    /// 0=否，1=是
    /// 系统必须有一个默认币种（人民币 CNY）
    /// 切换默认币种时，旧的默认币种自动取消
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否默认币种")]
    public int IsDefault { get; set; } = 0;
}

/// <summary>
/// 税率实体类
/// </summary>
/// <remarks>
/// 税率表，用于管理系统支持的税率。
/// 用于销售订单（销项税）、采购订单（进项税）、发票等。
/// 常见税率：13%（一般货物）、9%（农产品、图书等）、6%（服务类）、0%（免税）
/// </remarks>
[SugarTable("crm_tax_rate", "税率表")]
public class TaxRate : BaseEntity
{
    /// <summary>
    /// 税率名称
    /// </summary>
    /// <remarks>
    /// 税率名称，如：13% 税率、9% 税率、6% 税率、免税
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "税率名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 税率
    /// </summary>
    /// <remarks>
    /// 税率值，如：0.13 表示 13%
    /// </remarks>
    [SugarColumn(ColumnDescription = "税率", DecimalDigits = 4)]
    public decimal Rate { get; set; } = 0.0m;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 税率描述，如：适用于一般货物
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "描述", IsNullable = true)]
    public string? Description { get; set; }
}