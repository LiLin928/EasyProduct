using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 采购订单服务接口
/// </summary>
/// <remarks>
/// 提供采购订单的增删改查、状态流转、金额计算等功能
/// </remarks>
public interface IPurchaseOrderService
{
    /// <summary>
    /// 获取供应商下拉选项列表
    /// </summary>
    /// <returns>供应商下拉选项列表</returns>
    Task<List<SupplierOptionDto>> GetSupplierOptionsAsync();

    /// <summary>
    /// 获取采购订单分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页结果</returns>
    Task<PageResponse<PurchaseOrderDto>> GetListAsync(PurchaseOrderQueryDto query);

    /// <summary>
    /// 获取采购订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    Task<PurchaseOrderDetailDto> GetDetailAsync(Guid id);

    /// <summary>
    /// 创建采购订单
    /// </summary>
    /// <param name="dto">创建订单参数</param>
    /// <returns>新创建的订单ID</returns>
    Task<Guid> CreateAsync(CreatePurchaseOrderDto dto);

    /// <summary>
    /// 更新采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">更新订单参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateAsync(Guid id, UpdatePurchaseOrderDto dto);

    /// <summary>
    /// 更新采购订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">状态更新参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateStatusAsync(Guid id, UpdatePurchaseOrderStatusDto dto);

    /// <summary>
    /// 删除采购订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>删除是否成功</returns>
    Task<bool> DeleteAsync(Guid id);
}