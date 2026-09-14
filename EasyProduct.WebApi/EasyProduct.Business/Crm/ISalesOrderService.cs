using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 销售订单服务接口
/// </summary>
/// <remarks>
/// 提供销售订单的增删改查、状态流转、金额计算等功能
/// </remarks>
public interface ISalesOrderService
{
    #region 订单管理

    /// <summary>
    /// 获取客户下拉选项列表
    /// </summary>
    /// <returns>客户下拉选项列表</returns>
    /// <remarks>
    /// 获取状态为启用的客户列表，用于订单创建时的客户选择
    /// </remarks>
    Task<List<CustomerOptionDto>> GetCustomerOptionsAsync();

    /// <summary>
    /// 获取销售订单分页列表
    /// </summary>
    /// <param name="query">查询参数，包含订单编号、客户ID、状态等筛选条件</param>
    /// <returns>订单分页结果（不包含明细）</returns>
    Task<PageResponse<SalesOrderDto>> GetListAsync(SalesOrderQueryDto query);

    /// <summary>
    /// 获取销售订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情（包含明细）</returns>
    Task<SalesOrderDetailDto> GetDetailAsync(Guid id);

    /// <summary>
    /// 创建销售订单
    /// </summary>
    /// <param name="dto">创建订单参数</param>
    /// <returns>新创建的订单ID</returns>
    /// <remarks>
    /// 创建订单时会：
    /// 1. 自动生成订单编号（SO-{year}-{sequence:04d}）
    /// 2. 自动填充客户名称（冗余字段）
    /// 3. 自动计算明细金额和订单总金额
    /// 4. 默认状态为草稿（draft）
    /// </remarks>
    Task<Guid> CreateAsync(CreateSalesOrderDto dto);

    /// <summary>
    /// 更新销售订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">更新订单参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 仅草稿状态可修改。
    /// 更新时会重新计算明细金额和订单总金额。
    /// </remarks>
    Task<bool> UpdateAsync(Guid id, UpdateSalesOrderDto dto);

    /// <summary>
    /// 更新销售订单状态
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="dto">状态更新参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 状态流转规则：
    /// - draft → confirmed 或 cancelled
    /// - confirmed → shipped 或 cancelled
    /// - shipped → completed
    ///
    /// 发货（shipped）时会：
    /// 1. 检查库存是否充足
    /// 2. 自动创建出库记录
    /// 3. 扣减库存
    /// </remarks>
    Task<bool> UpdateStatusAsync(Guid id, UpdateSalesOrderStatusDto dto);

    /// <summary>
    /// 删除销售订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 仅草稿状态可删除。
    /// 使用软删除，将 IsDeleted 字段设置为 1。
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    #endregion

    #region 私有方法（供内部使用）

    /// <summary>
    /// 生成订单编号
    /// </summary>
    /// <returns>订单编号（格式：SO-{year}-{sequence:04d}）</returns>
    Task<string> GenerateOrderNoAsync();

    /// <summary>
    /// 计算订单明细金额
    /// </summary>
    /// <param name="item">订单明细</param>
    /// <remarks>
    /// 计算规则：
    /// - amount = price × quantity
    /// - taxAmount = amount × taxRate / 100
    /// - totalAmount = amount + taxAmount
    /// </remarks>
    void CalculateItemAmount(SalesOrderItemDto item);

    /// <summary>
    /// 计算订单总金额
    /// </summary>
    /// <param name="items">订单明细列表</param>
    /// <returns>订单总金额（subtotalAmount, taxAmount, totalAmount）</returns>
    /// <remarks>
    /// 计算规则：
    /// - subtotalAmount = sum(items.amount)
    /// - taxAmount = sum(items.taxAmount)
    /// - totalAmount = subtotalAmount + taxAmount
    /// </remarks>
    (decimal subtotalAmount, decimal taxAmount, decimal totalAmount) CalculateOrderAmount(List<SalesOrderItemDto> items);

    /// <summary>
    /// 验证状态流转是否合法
    /// </summary>
    /// <param name="currentStatus">当前状态</param>
    /// <param name="newStatus">新状态</param>
    /// <returns>是否合法</returns>
    bool ValidateStatusTransition(string currentStatus, string newStatus);

    /// <summary>
    /// 检查库存是否充足
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <returns>检查结果（是否充足，不足的商品列表）</returns>
    /// <remarks>
    /// 检查每个明细项的库存是否满足数量需求。
    /// 返回不足的商品列表，用于提示用户。
    /// </remarks>
    Task<(bool available, List<string> insufficientItems)> CheckStockAvailabilityAsync(Guid orderId);

    /// <summary>
    /// 处理销售出库
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <remarks>
    /// 发货时执行：
    /// 1. 创建出库记录（stock_record）
    /// 2. 扣减库存（stock）
    ///
    /// 注意：此方法需要库存模块支持，当前预留接口。
    /// </remarks>
    Task ProcessSalesOutboundAsync(Guid orderId);

    #endregion
}