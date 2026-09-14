using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 出入库流水服务接口
/// </summary>
/// <remarks>
/// 提供出入库流水的查询、创建等功能
/// </remarks>
public interface IStockRecordService
{
    /// <summary>
    /// 获取出入库流水分页列表
    /// </summary>
    /// <param name="query">查询参数，包含仓库、SKU、类型等筛选条件</param>
    /// <returns>流水分页结果</returns>
    Task<PageResponse<StockRecordDto>> GetListAsync(StockRecordQueryDto query);

    /// <summary>
    /// 创建出入库流水记录
    /// </summary>
    /// <param name="dto">创建流水参数</param>
    /// <returns>新创建的流水ID</returns>
    /// <remarks>
    /// 此方法仅供内部使用，不对外暴露 API。
    /// 在库存变动时自动调用（采购入库、销售出库等）。
    /// </remarks>
    Task<Guid> CreateAsync(CreateStockRecordDto dto);
}