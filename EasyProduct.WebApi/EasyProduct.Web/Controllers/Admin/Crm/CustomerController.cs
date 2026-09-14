using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 客户管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供客户的创建、查询、更新、删除等管理功能。
/// 提供联系人管理、地址管理等功能。
/// 提供询价转客户功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/customer")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class CustomerController : BaseController
{
    private readonly ICustomerService _customerService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="customerService">客户服务</param>
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    #region 客户管理

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建客户参数</param>
    /// <returns>客户ID</returns>
    /// <remarks>
    /// 创建新的客户档案。
    /// 客户编码自动生成（C + 年月 + 序号）。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateCustomerDto dto)
    {
        var result = await _customerService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "客户创建成功");
    }

    /// <summary>
    /// 分页查询客户列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>客户分页列表</returns>
    /// <remarks>
    /// 支持按客户名称、编码、类型、来源、状态、业务员筛选。
    /// 支持关键词模糊搜索。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<CustomerDto>>> GetList([FromQuery] CustomerQueryDto query)
    {
        var result = await _customerService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CustomerDto>> GetDetail(Guid id)
    {
        var result = await _customerService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="dto">更新客户参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        var result = await _customerService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 软删除客户档案。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _customerService.DeleteAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 启用/禁用客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>是否成功</returns>
    [HttpPatch("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(Guid id, [FromQuery] int status)
    {
        var result = await _customerService.UpdateStatusAsync(id, status);
        return Success(result);
    }

    #endregion

    #region 联系人管理

    /// <summary>
    /// 获取客户联系人列表
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <returns>联系人列表</returns>
    [HttpGet("{customerId}/contact")]
    public async Task<ApiResponse<List<CustomerContactDto>>> GetContacts(Guid customerId)
    {
        var result = await _customerService.GetContactsAsync(customerId);
        return Success(result);
    }

    /// <summary>
    /// 创建客户联系人
    /// </summary>
    /// <param name="dto">创建联系人参数</param>
    /// <returns>联系人ID</returns>
    /// <remarks>
    /// 如果设置为主要联系人，会自动取消其他联系人的主要标记。
    /// </remarks>
    [HttpPost("{customerId}/contact")]
    public async Task<ApiResponse<string>> CreateContact([FromBody] CreateCustomerContactDto dto)
    {
        var result = await _customerService.CreateContactAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "联系人创建成功");
    }

    /// <summary>
    /// 更新客户联系人
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <param name="contactId">联系人ID</param>
    /// <param name="dto">更新联系人参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{customerId}/contact/{contactId}")]
    public async Task<ApiResponse<bool>> UpdateContact(Guid customerId, Guid contactId, [FromBody] UpdateCustomerContactDto dto)
    {
        var result = await _customerService.UpdateContactAsync(contactId, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除客户联系人
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <param name="contactId">联系人ID</param>
    /// <returns>是否成功</returns>
    [HttpDelete("{customerId}/contact/{contactId}")]
    public async Task<ApiResponse<bool>> DeleteContact(Guid customerId, Guid contactId)
    {
        var result = await _customerService.DeleteContactAsync(contactId);
        return Success(result);
    }

    #endregion

    #region 地址管理

    /// <summary>
    /// 获取客户地址列表
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <returns>地址列表</returns>
    [HttpGet("{customerId}/address")]
    public async Task<ApiResponse<List<CustomerAddressDto>>> GetAddresses(Guid customerId)
    {
        var result = await _customerService.GetAddressesAsync(customerId);
        return Success(result);
    }

    /// <summary>
    /// 创建客户地址
    /// </summary>
    /// <param name="dto">创建地址参数</param>
    /// <returns>地址ID</returns>
    /// <remarks>
    /// 如果设置为默认地址，会自动取消其他地址的默认标记。
    /// </remarks>
    [HttpPost("{customerId}/address")]
    public async Task<ApiResponse<string>> CreateAddress([FromBody] CreateCustomerAddressDto dto)
    {
        var result = await _customerService.CreateAddressAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "地址创建成功");
    }

    /// <summary>
    /// 更新客户地址
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <param name="addressId">地址ID</param>
    /// <param name="dto">更新地址参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{customerId}/address/{addressId}")]
    public async Task<ApiResponse<bool>> UpdateAddress(Guid customerId, Guid addressId, [FromBody] UpdateCustomerAddressDto dto)
    {
        var result = await _customerService.UpdateAddressAsync(addressId, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除客户地址
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <param name="addressId">地址ID</param>
    /// <returns>是否成功</returns>
    [HttpDelete("{customerId}/address/{addressId}")]
    public async Task<ApiResponse<bool>> DeleteAddress(Guid customerId, Guid addressId)
    {
        var result = await _customerService.DeleteAddressAsync(addressId);
        return Success(result);
    }

    #endregion

    #region 特殊功能

    /// <summary>
    /// 询价转客户
    /// </summary>
    /// <param name="inquiryId">询价单ID</param>
    /// <returns>客户ID</returns>
    /// <remarks>
    /// 从询价单创建客户档案：
    /// - 客户类型：B2B
    /// - 客户来源：询价
    /// - 自动填充联系人、联系方式等信息
    /// </remarks>
    [HttpPost("from-inquiry/{inquiryId}")]
    public async Task<ApiResponse<string>> CreateFromInquiry(Guid inquiryId)
    {
        var result = await _customerService.CreateFromInquiryAsync(inquiryId);
        return ApiResponse<string>.Success(result.ToString(), "询价转客户成功");
    }

    /// <summary>
    /// 获取启用的客户列表（下拉选择用）
    /// </summary>
    /// <param name="keyword">关键词（可选）</param>
    /// <returns>客户列表</returns>
    /// <remarks>
    /// 获取启用的客户列表，用于下拉选择。
    /// 最多返回 50 条记录。
    /// </remarks>
    [HttpGet("active")]
    public async Task<ApiResponse<List<CustomerDto>>> GetActiveList([FromQuery] string? keyword = null)
    {
        var result = await _customerService.GetActiveListAsync(keyword);
        return Success(result);
    }

    #endregion
}