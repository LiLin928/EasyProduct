using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 币种服务接口
/// </summary>
/// <remarks>
/// 提供币种的增删改查功能
/// </remarks>
public interface ICurrencyService
{
    /// <summary>
    /// 获取币种分页列表
    /// </summary>
    /// <param name="query">查询参数，包含币种代码、币种名称、状态、分页信息</param>
    /// <returns>币种分页结果</returns>
    Task<PageResponse<CurrencyDto>> GetListAsync(CurrencyQueryDto query);

    /// <summary>
    /// 获取币种详情
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <returns>币种详情</returns>
    Task<CurrencyDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建币种
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的币种ID</returns>
    /// <remarks>
    /// 如果设置为默认币种，会自动取消其他币种的默认标记
    /// </remarks>
    Task<Guid> CreateAsync(CreateCurrencyDto dto);

    /// <summary>
    /// 更新币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 如果设置为默认币种，会自动取消其他币种的默认标记
    /// </remarks>
    Task<bool> UpdateAsync(Guid id, UpdateCurrencyDto dto);

    /// <summary>
    /// 删除币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 启用/禁用币种
    /// </summary>
    /// <param name="id">币种ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    Task<bool> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// 获取启用的币种列表（下拉选择用）
    /// </summary>
    /// <returns>币种列表</returns>
    Task<List<CurrencyDto>> GetActiveListAsync();
}

/// <summary>
/// 税率服务接口
/// </summary>
/// <remarks>
/// 提供税率的增删改查功能
/// </remarks>
public interface ITaxRateService
{
    /// <summary>
    /// 获取税率分页列表
    /// </summary>
    /// <param name="query">查询参数，包含税率名称、状态、分页信息</param>
    /// <returns>税率分页结果</returns>
    Task<PageResponse<TaxRateDto>> GetListAsync(TaxRateQueryDto query);

    /// <summary>
    /// 获取税率详情
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <returns>税率详情</returns>
    Task<TaxRateDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建税率
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的税率ID</returns>
    Task<Guid> CreateAsync(CreateTaxRateDto dto);

    /// <summary>
    /// 更新税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateAsync(Guid id, UpdateTaxRateDto dto);

    /// <summary>
    /// 删除税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 启用/禁用税率
    /// </summary>
    /// <param name="id">税率ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    Task<bool> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// 获取启用的税率列表（下拉选择用）
    /// </summary>
    /// <returns>税率列表</returns>
    Task<List<TaxRateDto>> GetActiveListAsync();
}