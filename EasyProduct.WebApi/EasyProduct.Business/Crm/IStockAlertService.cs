using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 库存预警服务接口
/// </summary>
/// <remarks>
/// 提供库存预警的查询、解决等功能
/// </remarks>
public interface IStockAlertService
{
    /// <summary>
    /// 获取库存预警分页列表
    /// </summary>
    /// <param name="query">查询参数，包含仓库、SKU、类型、状态等筛选条件</param>
    /// <returns>预警分页结果</returns>
    Task<PageResponse<StockAlertDto>> GetListAsync(StockAlertQueryDto query);

    /// <summary>
    /// 解决库存预警
    /// </summary>
    /// <param name="id">预警ID</param>
    /// <param name="dto">解决预警参数</param>
    /// <returns>解决是否成功</returns>
    /// <remarks>
    /// 解决预警时会：
    /// 1. 更新预警状态为已解决
    /// 2. 记录解决时间
    /// 3. 添加备注说明
    /// </remarks>
    Task<bool> ResolveAsync(Guid id, ResolveAlertDto dto);

    /// <summary>
    /// 检查库存预警
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="skuCode">SKU编码</param>
    /// <remarks>
    /// 检查指定仓库中指定 SKU 的库存是否触发预警。
    /// 如果触发预警，自动创建预警记录。
    /// 此方法在库存变动时自动调用。
    /// </remarks>
    Task CheckAlertAsync(string warehouseId, string skuCode);
}