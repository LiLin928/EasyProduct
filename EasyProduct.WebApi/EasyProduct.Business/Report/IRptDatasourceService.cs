using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Report.Datasource;

namespace EasyProduct.Business.Report;

/// <summary>
/// 数据源服务接口
/// </summary>
public interface IRptDatasourceService
{
    /// <summary>
    /// 获取数据源分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、名称、类型、状态等筛选条件</param>
    /// <returns>数据源分页列表结果</returns>
    /// <remarks>
    /// 支持按名称模糊搜索、数据源类型筛选、连接状态筛选
    /// 默认按创建时间倒序排列
    /// </remarks>
    Task<PageResponse<RptDatasourceDto>> GetListAsync(RptDatasourceQuery query);

    /// <summary>
    /// 根据ID获取数据源详情
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>数据源详情信息，如果不存在则返回 null</returns>
    Task<RptDatasourceDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建数据源
    /// </summary>
    /// <param name="dto">创建数据源参数</param>
    /// <returns>创建成功返回新记录的ID</returns>
    /// <remarks>
    /// 1. 验证数据源名称唯一性
    /// 2. 验证连接参数的完整性
    /// 3. 密码进行加密存储
    /// </remarks>
    Task<Guid> CreateAsync(RptDatasourceCreateDto dto);

    /// <summary>
    /// 更新数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <param name="dto">更新数据源参数</param>
    /// <returns>更新成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 1. 验证数据源是否存在
    /// 2. 如果修改密码，需要加密存储
    /// 3. 如果修改连接参数，需要重置连接状态
    /// </remarks>
    Task<bool> UpdateAsync(Guid id, RptDatasourceUpdateDto dto);

    /// <summary>
    /// 删除数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>删除成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 删除前需要检查该数据源是否被报表引用，如果被引用则不允许删除
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 测试数据源连接
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>连接测试结果，包含是否成功、错误信息等</returns>
    /// <remarks>
    /// 1. 根据数据源类型使用对应的连接器进行连接测试
    /// 2. 更新数据源的连接状态和最后测试时间
    /// 3. 记录测试结果和错误信息
    /// </remarks>
    Task<RptConnectionTestResultDto> TestConnectionAsync(Guid id);

    /// <summary>
    /// 获取所有数据源列表（不分页）
    /// </summary>
    /// <returns>所有数据源列表</returns>
    /// <remarks>
    /// 用于下拉选择框，只返回 ID、Name、Type 等基本字段
    /// </remarks>
    Task<List<RptDatasourceDto>> GetAllAsync();
}