using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 商品分类管理控制器
/// </summary>
/// <remarks>
/// 提供商品分类的管理接口，包括分类树形结构查询、分类列表、创建、更新、删除、状态切换等功能。
/// 所有接口需要管理员权限（AdminJwt 认证）。
/// </remarks>
[ApiController]
[Route("api/admin/product/category")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="categoryService">商品分类服务接口</param>
    /// <param name="logger">日志记录器</param>
    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// 获取分类树形结构
    /// </summary>
    /// <param name="parentId">父分类ID（可选），指定时只返回该分类下的子分类</param>
    /// <param name="onlyEnabled">是否仅查询启用的分类（可选），默认 false 返回所有状态</param>
    /// <returns>分类树形结构列表</returns>
    /// <remarks>
    /// 1. 返回分类的树形结构，包含父子层级关系
    /// 2. parentId 为空时返回所有分类，指定时返回该分类的子分类树
    /// 3. onlyEnabled=true 时只返回 Status=Enabled 的分类
    /// </remarks>
    [HttpGet("tree")]
    public async Task<ApiResponse<List<CategoryTreeDto>>> GetTree(
        [FromQuery] string? parentId = null,
        [FromQuery] bool? onlyEnabled = null)
    {
        var query = new CategoryTreeQueryDto
        {
            ParentId = parentId,
            OnlyEnabled = onlyEnabled
        };

        var result = await _categoryService.GetCategoryTreeAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取分类列表（扁平）
    /// </summary>
    /// <param name="categoryName">分类名称关键词（可选），支持模糊搜索</param>
    /// <param name="categoryCode">分类编码（可选），精确匹配</param>
    /// <param name="status">状态筛选（可选），0=禁用，1=启用</param>
    /// <param name="pageIndex">页码，从 1 开始，默认 1</param>
    /// <param name="pageSize">每页数量，默认 10</param>
    /// <returns>分类分页列表</returns>
    /// <remarks>
    /// 1. 返回扁平化的分类列表，不包含层级结构
    /// 2. 支持按分类名称模糊搜索、分类编码精确匹配、状态筛选
    /// 3. 默认按创建时间倒序排列
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<CategoryDto>>> GetList(
        [FromQuery] string? categoryName = null,
        [FromQuery] string? categoryCode = null,
        [FromQuery] int? status = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new CategoryQueryDto
        {
            CategoryName = categoryName,
            CategoryCode = categoryCode,
            Status = status.HasValue ? (Models.Enums.Status)status.Value : null,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _categoryService.GetCategoryListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    /// <remarks>
    /// 根据分类ID获取分类的详细信息，包括分类名称、编码、父分类、状态、排序等
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CategoryDto>> GetById(string id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 创建分类
    /// </summary>
    /// <param name="dto">创建分类参数</param>
    /// <returns>创建结果</returns>
    /// <remarks>
    /// 1. 分类编码必须唯一，不能与已有分类编码重复
    /// 2. 父分类ID必须存在且状态为启用
    /// 3. 创建成功返回 true
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<object>> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await _categoryService.CreateCategoryAsync(dto);
        return result ? Success("创建成功") : Error<object>("创建失败");
    }

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新分类参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. 分类编码不能与其他分类重复
    /// 2. 父分类不能设置为自己或自己的子分类
    /// 3. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<object>> Update(string id, [FromBody] UpdateCategoryDto dto)
    {
        // 确保 DTO 中的 ID 与路径参数一致
        dto.Id = id;
        var result = await _categoryService.UpdateCategoryAsync(dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>删除结果</returns>
    /// <remarks>
    /// 1. 软删除，不会物理删除数据
    /// 2. 存在子分类时不允许删除
    /// 3. 存在关联商品时不允许删除
    /// 4. 删除成功返回 true
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(string id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 更新分类状态
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">状态参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. 切换分类的启用/禁用状态
    /// 2. 禁用分类时，其子分类不会自动禁用
    /// 3. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<object>> UpdateStatus(string id, [FromBody] UpdateStatusDto dto)
    {
        var result = await _categoryService.UpdateCategoryStatusAsync(id, dto.Status);
        return result ? Success("状态更新成功") : Error<object>("状态更新失败");
    }
}

/// <summary>
/// 更新分类状态请求 DTO
/// </summary>
public class UpdateStatusDto
{
    /// <summary>
    /// 状态值
    /// </summary>
    /// <remarks>
    /// 0 = 禁用
    /// 1 = 启用
    /// </remarks>
    public int Status { get; set; }
}