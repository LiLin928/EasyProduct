using EasyProduct.Business.Crm;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Crm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Crm;

/// <summary>
/// 供应商管理控制器（管理端）
/// </summary>
/// <remarks>
/// 提供供应商的创建、查询、更新、删除等管理功能。
/// 提供供应商资质管理功能。
/// 需要管理员权限。
/// </remarks>
[ApiController]
[Route("api/admin/crm/supplier")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class SupplierController : BaseController
{
    private readonly ISupplierService _supplierService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="supplierService">供应商服务</param>
    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    #region 供应商管理

    /// <summary>
    /// 创建供应商
    /// </summary>
    /// <param name="dto">创建供应商参数</param>
    /// <returns>供应商ID</returns>
    /// <remarks>
    /// 创建新的供应商档案。
    /// 供应商编码自动生成（S + 年月 + 序号）。
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateSupplierDto dto)
    {
        var result = await _supplierService.CreateAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "供应商创建成功");
    }

    /// <summary>
    /// 分页查询供应商列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>供应商分页列表</returns>
    /// <remarks>
    /// 支持按供应商名称、编码、状态筛选。
    /// 支持关键词模糊搜索。
    /// 支持分页查询。
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<SupplierDto>>> GetList([FromQuery] SupplierQueryDto query)
    {
        var result = await _supplierService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取供应商详情
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>供应商详情</returns>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SupplierDto>> GetDetail(Guid id)
    {
        var result = await _supplierService.GetByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 更新供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <param name="dto">更新供应商参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateSupplierDto dto)
    {
        var result = await _supplierService.UpdateAsync(id, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 软删除供应商档案。
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var result = await _supplierService.DeleteAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 启用/禁用供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>是否成功</returns>
    [HttpPatch("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(Guid id, [FromQuery] int status)
    {
        var result = await _supplierService.UpdateStatusAsync(id, status);
        return Success(result);
    }

    #endregion

    #region 资质管理

    /// <summary>
    /// 获取供应商资质列表
    /// </summary>
    /// <param name="supplierId">供应商ID</param>
    /// <returns>资质列表</returns>
    [HttpGet("{supplierId}/qualification")]
    public async Task<ApiResponse<List<SupplierQualificationDto>>> GetQualifications(Guid supplierId)
    {
        var result = await _supplierService.GetQualificationsAsync(supplierId);
        return Success(result);
    }

    /// <summary>
    /// 创建供应商资质
    /// </summary>
    /// <param name="dto">创建资质参数</param>
    /// <returns>资质ID</returns>
    /// <remarks>
    /// 资质状态根据有效期自动判断。
    /// </remarks>
    [HttpPost("{supplierId}/qualification")]
    public async Task<ApiResponse<string>> CreateQualification([FromBody] CreateSupplierQualificationDto dto)
    {
        var result = await _supplierService.CreateQualificationAsync(dto);
        return ApiResponse<string>.Success(result.ToString(), "资质创建成功");
    }

    /// <summary>
    /// 更新供应商资质
    /// </summary>
    /// <param name="supplierId">供应商ID</param>
    /// <param name="qualificationId">资质ID</param>
    /// <param name="dto">更新资质参数</param>
    /// <returns>是否成功</returns>
    [HttpPut("{supplierId}/qualification/{qualificationId}")]
    public async Task<ApiResponse<bool>> UpdateQualification(Guid supplierId, Guid qualificationId, [FromBody] UpdateSupplierQualificationDto dto)
    {
        var result = await _supplierService.UpdateQualificationAsync(qualificationId, dto);
        return Success(result);
    }

    /// <summary>
    /// 删除供应商资质
    /// </summary>
    /// <param name="supplierId">供应商ID</param>
    /// <param name="qualificationId">资质ID</param>
    /// <returns>是否成功</returns>
    [HttpDelete("{supplierId}/qualification/{qualificationId}")]
    public async Task<ApiResponse<bool>> DeleteQualification(Guid supplierId, Guid qualificationId)
    {
        var result = await _supplierService.DeleteQualificationAsync(qualificationId);
        return Success(result);
    }

    #endregion

    #region 辅助功能

    /// <summary>
    /// 获取启用的供应商列表（下拉选择用）
    /// </summary>
    /// <param name="keyword">关键词（可选）</param>
    /// <returns>供应商列表</returns>
    /// <remarks>
    /// 获取启用的供应商列表，用于下拉选择。
    /// 最多返回 50 条记录。
    /// </remarks>
    [HttpGet("active")]
    public async Task<ApiResponse<List<SupplierDto>>> GetActiveList([FromQuery] string? keyword = null)
    {
        var result = await _supplierService.GetActiveListAsync(keyword);
        return Success(result);
    }

    #endregion
}