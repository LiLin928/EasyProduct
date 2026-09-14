using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 盘点服务接口
/// </summary>
/// <remarks>
/// 提供盘点单的增删改查、完成盘点等功能
/// </remarks>
public interface IStockCheckService
{
    /// <summary>
    /// 获取盘点单分页列表
    /// </summary>
    /// <param name="query">查询参数，包含编号、仓库、状态等筛选条件</param>
    /// <returns>盘点单分页结果（不包含明细）</returns>
    Task<PageResponse<StockCheckDto>> GetListAsync(StockCheckQueryDto query);

    /// <summary>
    /// 获取盘点单详情
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>盘点单详情（包含明细）</returns>
    Task<StockCheckDetailDto> GetDetailAsync(Guid id);

    /// <summary>
    /// 创建盘点单
    /// </summary>
    /// <param name="dto">创建盘点单参数</param>
    /// <returns>新创建的盘点单ID</returns>
    /// <remarks>
    /// 创建盘点单时会：
    /// 1. 自动生成盘点单编号（SC-{year}-{sequence:04d}）
    /// 2. 自动填充仓库名称（冗余字段）
    /// 3. 如果不提供明细，则从库存账面自动生成
    /// 4. 自动填充系统数量
    /// 5. 默认状态为草稿（draft）
    /// </remarks>
    Task<Guid> CreateAsync(CreateStockCheckDto dto);

    /// <summary>
    /// 更新盘点单
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <param name="dto">更新盘点单参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 仅草稿和盘点中状态可修改。
    /// 更新明细时会重新计算盘点差异。
    /// </remarks>
    Task<bool> UpdateAsync(Guid id, UpdateStockCheckDto dto);

    /// <summary>
    /// 完成盘点
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>完成是否成功</returns>
    /// <remarks>
    /// 完成盘点时会：
    /// 1. 验证所有明细的实盘数量已填写
    /// 2. 根据盘点差异调整库存
    /// 3. 创建出入库流水记录
    /// 4. 更新盘点状态为已完成
    /// </remarks>
    Task<bool> CompleteAsync(Guid id);

    /// <summary>
    /// 删除盘点单
    /// </summary>
    /// <param name="id">盘点单ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 仅草稿状态可删除。
    /// 使用软删除。
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);
}