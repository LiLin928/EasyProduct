using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 库存服务接口
/// </summary>
/// <remarks>
/// 提供库存查询、调整、检查等功能
/// </remarks>
public interface IStockService
{
    /// <summary>
    /// 获取库存分页列表
    /// </summary>
    /// <param name="query">查询参数，包含仓库、SKU等筛选条件</param>
    /// <returns>库存分页结果</returns>
    Task<PageResponse<StockDto>> GetListAsync(StockQueryDto query);

    /// <summary>
    /// 获取库存详情
    /// </summary>
    /// <param name="id">库存ID</param>
    /// <returns>库存详情</returns>
    Task<StockDto> GetDetailAsync(Guid id);

    /// <summary>
    /// 库存调整
    /// </summary>
    /// <param name="dto">库存调整参数</param>
    /// <returns>调整是否成功</returns>
    /// <remarks>
    /// 调整库存时会：
    /// 1. 更新库存账面（增加或减少数量）
    /// 2. 创建出入库流水记录
    /// 3. 检查库存预警（低于下限或高于上限）
    /// </remarks>
    Task<bool> AdjustAsync(StockAdjustDto dto);

    /// <summary>
    /// 检查库存是否充足
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="skuCode">SKU编码</param>
    /// <param name="quantity">所需数量</param>
    /// <returns>库存是否充足</returns>
    /// <remarks>
    /// 检查指定仓库中指定 SKU 的可用库存是否满足需求
    /// </remarks>
    Task<bool> CheckStockAsync(string warehouseId, string skuCode, int quantity);
}