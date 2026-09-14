using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 客户服务接口
/// </summary>
/// <remarks>
/// 提供客户的增删改查、联系人管理、地址管理等功能
/// </remarks>
public interface ICustomerService
{
    #region 客户管理

    /// <summary>
    /// 获取客户分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、客户类型、客户来源、状态、分页信息</param>
    /// <returns>客户分页结果</returns>
    Task<PageResponse<CustomerDto>> GetListAsync(CustomerQueryDto query);

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详情</returns>
    Task<CustomerDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的客户ID</returns>
    /// <remarks>
    /// 创建客户时会自动生成客户编码（C + 年月 + 序号）
    /// </remarks>
    Task<Guid> CreateAsync(CreateCustomerDto dto);

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateAsync(Guid id, UpdateCustomerDto dto);

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 启用/禁用客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    Task<bool> UpdateStatusAsync(Guid id, int status);

    #endregion

    #region 联系人管理

    /// <summary>
    /// 获取客户联系人列表
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <returns>联系人列表</returns>
    Task<List<CustomerContactDto>> GetContactsAsync(Guid customerId);

    /// <summary>
    /// 创建客户联系人
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的联系人ID</returns>
    /// <remarks>
    /// 如果设置为主要联系人，会自动取消其他联系人的主要标记
    /// </remarks>
    Task<Guid> CreateContactAsync(CreateCustomerContactDto dto);

    /// <summary>
    /// 更新客户联系人
    /// </summary>
    /// <param name="id">联系人ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateContactAsync(Guid id, UpdateCustomerContactDto dto);

    /// <summary>
    /// 删除客户联系人
    /// </summary>
    /// <param name="id">联系人ID</param>
    /// <returns>删除是否成功</returns>
    Task<bool> DeleteContactAsync(Guid id);

    #endregion

    #region 地址管理

    /// <summary>
    /// 获取客户地址列表
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <returns>地址列表</returns>
    Task<List<CustomerAddressDto>> GetAddressesAsync(Guid customerId);

    /// <summary>
    /// 创建客户地址
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的地址ID</returns>
    /// <remarks>
    /// 如果设置为默认地址，会自动取消其他地址的默认标记
    /// </remarks>
    Task<Guid> CreateAddressAsync(CreateCustomerAddressDto dto);

    /// <summary>
    /// 更新客户地址
    /// </summary>
    /// <param name="id">地址ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    Task<bool> UpdateAddressAsync(Guid id, UpdateCustomerAddressDto dto);

    /// <summary>
    /// 删除客户地址
    /// </summary>
    /// <param name="id">地址ID</param>
    /// <returns>删除是否成功</returns>
    Task<bool> DeleteAddressAsync(Guid id);

    #endregion

    #region 特殊功能

    /// <summary>
    /// 询价转客户
    /// </summary>
    /// <param name="inquiryId">询价单ID</param>
    /// <returns>新创建的客户ID</returns>
    /// <remarks>
    /// 从询价单创建客户档案：
    /// - 客户类型：B2B
    /// - 客户来源：询价
    /// - 自动填充联系人、联系方式等信息
    /// </remarks>
    Task<Guid> CreateFromInquiryAsync(Guid inquiryId);

    /// <summary>
    /// 获取启用的客户列表（下拉选择用）
    /// </summary>
    /// <param name="keyword">关键词（可选，用于搜索客户名称或编码）</param>
    /// <returns>客户列表</returns>
    Task<List<CustomerDto>> GetActiveListAsync(string? keyword = null);

    #endregion
}