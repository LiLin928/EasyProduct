using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 仓库服务接口
/// </summary>
/// <remarks>
/// 提供仓库的增删改查、下拉选项等功能
/// </remarks>
public interface IWarehouseService
{
    /// <summary>
    /// 获取仓库下拉选项列表
    /// </summary>
    /// <returns>仓库下拉选项列表</returns>
    /// <remarks>
    /// 获取状态为启用的仓库列表，用于其他模块选择仓库时的下拉选项
    /// </remarks>
    Task<List<WarehouseOptionDto>> GetWarehouseOptionsAsync();

    /// <summary>
    /// 获取仓库分页列表
    /// </summary>
    /// <param name="query">查询参数，包含编码、名称、状态等筛选条件</param>
    /// <returns>仓库分页结果</returns>
    Task<PageResponse<WarehouseDto>> GetListAsync(WarehouseQueryDto query);

    /// <summary>
    /// 获取仓库详情
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <returns>仓库详情</returns>
    Task<WarehouseDto> GetDetailAsync(Guid id);

    /// <summary>
    /// 创建仓库
    /// </summary>
    /// <param name="dto">创建仓库参数</param>
    /// <returns>新创建的仓库ID</returns>
    /// <remarks>
    /// 创建仓库时会：
    /// 1. 检查编码是否已存在
    /// 2. 默认状态为启用（active）
    /// </remarks>
    Task<Guid> CreateAsync(CreateWarehouseDto dto);

    /// <summary>
    /// 更新仓库
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <param name="dto">更新仓库参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateAsync(Guid id, UpdateWarehouseDto dto);

    /// <summary>
    /// 删除仓库
    /// </summary>
    /// <param name="id">仓库ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1。
    /// 删除前会检查是否有关联数据（库存、出入库流水、盘点单）。
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);
}