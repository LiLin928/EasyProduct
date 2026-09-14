namespace EasyProduct.Models.Enums.Crm;

/// <summary>
/// 客户类型枚举
/// </summary>
/// <remarks>
/// 用于区分客户类型：
/// - B2B: 企业客户（询价转化、手动建档）
/// - Retail: 零售客户（小程序会员关联）
/// </remarks>
public enum CustomerType
{
    /// <summary>
    /// B2B 客户（企业客户）
    /// </summary>
    B2B = 1,

    /// <summary>
    /// 零售客户（个人客户）
    /// </summary>
    Retail = 2
}

/// <summary>
/// 客户来源枚举
/// </summary>
/// <remarks>
/// 用于记录客户的来源渠道：
/// - Inquiry: 官网询价转客户
/// - Register: 小程序会员首单自动建档
/// - Manual: 后台手动创建
/// </remarks>
public enum CustomerSource
{
    /// <summary>
    /// 询价转化
    /// </summary>
    Inquiry = 1,

    /// <summary>
    /// 注册自动建档
    /// </summary>
    Register = 2,

    /// <summary>
    /// 手动建档
    /// </summary>
    Manual = 3
}

/// <summary>
/// 资质类型枚举
/// </summary>
/// <remarks>
/// 用于供应商资质管理：
/// - BusinessLicense: 营业执照
/// - ProductionLicense: 生产许可证
/// - QualityCertification: 质量认证
/// </remarks>
public enum QualificationType
{
    /// <summary>
    /// 营业执照
    /// </summary>
    BusinessLicense = 1,

    /// <summary>
    /// 生产许可证
    /// </summary>
    ProductionLicense = 2,

    /// <summary>
    /// 质量认证
    /// </summary>
    QualityCertification = 3
}