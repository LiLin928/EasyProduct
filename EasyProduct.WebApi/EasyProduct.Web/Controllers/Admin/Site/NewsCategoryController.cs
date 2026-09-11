using EasyProduct.Business.Site;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Site.NewsCategory;
using EasyProduct.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Site;

/// <summary>
/// 新闻分类管理控制器
/// </summary>
/// <remarks>
/// 提供新闻分类的增删改查功能
/// 管理端接口，需要 Admin JWT 认证
/// 权限标识前缀：site:news-category:
/// </remarks>
[ApiController]
[Route("api/admin/site/news-category")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class NewsCategoryController : BaseController
{
    private readonly INewsCategoryService _newsCategoryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="newsCategoryService">新闻分类服务</param>
    public NewsCategoryController(INewsCategoryService newsCategoryService)
    {
        _newsCategoryService = newsCategoryService;
    }

    /// <summary>
    /// 获取新闻分类列表（分页）
    /// </summary>
    /// <param name="categoryName">分类名称（模糊搜索）</param>
    /// <param name="categoryCode">分类编码（模糊搜索）</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页条数，默认10</param>
    /// <returns>新闻分类列表</returns>
    /// <remarks>
    /// 获取新闻分类的分页列表，支持按名称、编码、状态筛选
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<List<NewsCategoryDto>>> GetList(
        [FromQuery] string? categoryName,
        [FromQuery] string? categoryCode,
        [FromQuery] Status? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new NewsCategoryQueryDto
        {
            CategoryName = categoryName,
            CategoryCode = categoryCode,
            Status = status,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _newsCategoryService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取所有启用的新闻分类（用于下拉选择）
    /// </summary>
    /// <returns>新闻分类列表</returns>
    /// <remarks>
    /// 获取所有启用状态的新闻分类，用于下拉选择框等场景
    /// </remarks>
    [HttpGet("all")]
    public async Task<ApiResponse<List<NewsCategoryDto>>> GetAll()
    {
        var result = await _newsCategoryService.GetEnabledListAsync();
        return Success(result);
    }

    /// <summary>
    /// 获取新闻分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>新闻分类详情</returns>
    /// <remarks>
    /// 根据ID获取新闻分类的详细信息
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<NewsCategoryDto>> GetById(string id)
    {
        var result = await _newsCategoryService.GetByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建新闻分类
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新分类ID</returns>
    /// <remarks>
    /// 创建新的新闻分类
    /// 注意：分类名称和分类编码不能重复
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<string>> Create([FromBody] CreateNewsCategoryDto dto)
    {
        var result = await _newsCategoryService.CreateAsync(dto);
        return Success(result, "新闻分类创建成功");
    }

    /// <summary>
    /// 更新新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新新闻分类信息
    /// 注意：分类名称和分类编码不能与其他分类重复
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(string id, [FromBody] UpdateNewsCategoryDto dto)
    {
        dto.Id = id;
        var result = await _newsCategoryService.UpdateAsync(dto);
        return Success(result, "新闻分类更新成功");
    }

    /// <summary>
    /// 删除新闻分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除新闻分类（软删除）
    /// 注意：如果分类下存在新闻，则不能删除
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var result = await _newsCategoryService.DeleteAsync(id);
        return Success(result, "新闻分类删除成功");
    }
}