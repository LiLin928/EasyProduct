using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Spu;
using EasyProduct.Models.Enums;
using EasyProduct.Models.Enums.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 商品主档（SPU）管理控制器
/// </summary>
/// <remarks>
/// 提供商品主档（SPU）的管理接口，包括分页查询、详情查询、按分类查询、创建、更新、删除、状态切换等功能。
/// 所有接口需要管理员权限（AdminJwt 认证）。
/// </remarks>
[ApiController]
[Route("api/admin/product/spu")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class SpuController : BaseController
{
    private readonly ISpuService _spuService;
    private readonly ILogger<SpuController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="spuService">商品主档服务接口</param>
    /// <param name="logger">日志记录器</param>
    public SpuController(ISpuService spuService, ILogger<SpuController> logger)
    {
        _spuService = spuService;
        _logger = logger;
    }

    /// <summary>
    /// 获取商品主档分页列表
    /// </summary>
    /// <param name="spuName">商品名称关键词（可选），支持模糊搜索</param>
    /// <param name="spuCode">商品编码（可选），支持模糊搜索</param>
    /// <param name="categoryId">分类ID（可选），精确匹配</param>
    /// <param name="spuType">商品类型（可选），1=实物商品，2=虚拟商品，3=票品</param>
    /// <param name="status">状态筛选（可选），0=禁用，1=启用</param>
    /// <param name="pageIndex">页码，从 1 开始，默认 1</param>
    /// <param name="pageSize">每页数量，默认 10</param>
    /// <returns>商品主档分页列表</returns>
    /// <remarks>
    /// 1. 支持按商品名称模糊搜索、商品编码模糊搜索、分类精确匹配、类型筛选、状态筛选
    /// 2. 支持分页查询，返回分页信息和商品列表
    /// 3. 默认按创建时间倒序排列
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<SpuDto>>> GetList(
        [FromQuery] string? spuName = null,
        [FromQuery] string? spuCode = null,
        [FromQuery] string? categoryId = null,
        [FromQuery] int? spuType = null,
        [FromQuery] int? status = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new SpuQueryDto
        {
            SpuName = spuName,
            SpuCode = spuCode,
            CategoryId = categoryId,
            SpuType = spuType.HasValue ? (SpuType)spuType.Value : null,
            Status = status.HasValue ? (Status)status.Value : null,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _spuService.GetSpuPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取商品主档详情
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品主档详情</returns>
    /// <remarks>
    /// 根据商品ID获取商品的详细信息，包括商品名称、编码、分类、类型、状态、规格模板等所有字段
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SpuDto>> GetById(string id)
    {
        var result = await _spuService.GetSpuByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 按分类查询商品主档列表
    /// </summary>
    /// <param name="categoryId">分类ID</param>
    /// <returns>商品主档列表</returns>
    /// <remarks>
    /// 1. 查询指定分类下的所有商品
    /// 2. 不包含子分类的商品，如需包含子分类请递归查询
    /// 3. 返回该分类下所有状态的商品
    /// </remarks>
    [HttpGet("category/{categoryId}")]
    public async Task<ApiResponse<List<SpuDto>>> GetByCategory(string categoryId)
    {
        var result = await _spuService.GetSpuListByCategoryAsync(categoryId);
        return Success(result);
    }

    /// <summary>
    /// 创建商品主档
    /// </summary>
    /// <param name="dto">创建商品主档参数</param>
    /// <returns>创建结果</returns>
    /// <remarks>
    /// 1. 商品名称必填，最大长度 200 个字符
    /// 2. 商品类型必填：1=实物商品，2=虚拟商品，3=票品
    /// 3. 商品编码唯一，不能与已有商品编码重复
    /// 4. 创建成功返回 true
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<object>> Create([FromBody] CreateSpuDto dto)
    {
        var result = await _spuService.CreateSpuAsync(dto);
        return result ? Success("创建成功") : Error<object>("创建失败");
    }

    /// <summary>
    /// 更新商品主档
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <param name="dto">更新商品主档参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. 商品名称必填，最大长度 200 个字符
    /// 2. 商品类型必填：1=实物商品，2=虚拟商品，3=票品
    /// 3. 商品编码不能与其他商品重复
    /// 4. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<object>> Update(string id, [FromBody] UpdateSpuDto dto)
    {
        // 确保 DTO 中的 ID 与路径参数一致
        dto.Id = id;
        var result = await _spuService.UpdateSpuAsync(dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除商品主档
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>删除结果</returns>
    /// <remarks>
    /// 1. 软删除，不会物理删除数据
    /// 2. 存在关联 SKU 时不允许删除
    /// 3. 删除成功返回 true
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(string id)
    {
        var result = await _spuService.DeleteSpuAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 更新商品主档状态
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <param name="dto">状态参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. 切换商品的启用/禁用状态
    /// 2. 禁用商品时，其关联 SKU 不会自动禁用
    /// 3. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<object>> UpdateStatus(string id, [FromBody] UpdateSpuStatusDto dto)
    {
        var result = await _spuService.UpdateSpuStatusAsync(id, dto.Status);
        return result ? Success("状态更新成功") : Error<object>("状态更新失败");
    }
}

/// <summary>
/// 更新商品主档状态请求 DTO
/// </summary>
public class UpdateSpuStatusDto
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