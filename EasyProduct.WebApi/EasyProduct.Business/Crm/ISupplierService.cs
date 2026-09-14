using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 供应商服务接口
/// </summary>
/// <remarks>
/// 提供供应商的增删改查、资质管理等功能
/// </remarks>
public interface ISupplierService
{
    /// <summary>
    /// 获取供应商分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、状态、分页信息</param>
    /// <returns>供应商分页结果</returns>
    Task<PageResponse<SupplierDto>> GetListAsync(SupplierQueryDto query);

    /// <summary>
    /// 获取供应商详情
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>供应商详情</returns>
    Task<SupplierDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建供应商
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的供应商ID</returns>
    /// <remarks>
    /// 创建供应商时会自动生成供应商编码（S + 年月 + 序号）
    /// </remarks>
    Task<Guid> CreateAsync(CreateSupplierDto dto);

    /// <summary>
    /// 更新供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateAsync(Guid id, UpdateSupplierDto dto);

    /// <summary>
    /// 删除供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 启用/禁用供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    Task<bool> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// 获取供应商资质列表
    /// </summary>
    /// <param name="supplierId">供应商ID</param>
    /// <returns>资质列表</returns>
    Task<List<SupplierQualificationDto>> GetQualificationsAsync(Guid supplierId);

    /// <summary>
    /// 创建供应商资质
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的资质ID</returns>
    Task<Guid> CreateQualificationAsync(CreateSupplierQualificationDto dto);

    /// <summary>
    /// 更新供应商资质
    /// </summary>
    /// <param name="id">资质ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateQualificationAsync(Guid id, UpdateSupplierQualificationDto dto);

    /// <summary>
    /// 删除供应商资质
    /// </summary>
    /// <param name="id">资质ID</param>
    /// <returns>删除是否成功</returns>
    Task<bool> DeleteQualificationAsync(Guid id);

    /// <summary>
    /// 获取启用的供应商列表（下拉选择用）
    /// </summary>
    /// <param name="keyword">关键词（可选，用于搜索供应商名称或编码）</param>
    /// <returns>供应商列表</returns>
    Task<List<SupplierDto>> GetActiveListAsync(string? keyword = null);
}