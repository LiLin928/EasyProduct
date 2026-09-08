using EasyProduct.Business.Product;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Product.Sku;
using EasyProduct.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Product;

/// <summary>
/// 商品SKU管理控制器
/// </summary>
/// <remarks>
/// 提供商品SKU（库存量单位）的管理接口，包括分页查询、详情查询、按SPU查询、创建、更新、删除、状态更新、库存更新等功能。
/// 所有接口需要管理员权限（AdminJwt 认证）。
/// </remarks>
[ApiController]
[Route("api/admin/product/sku")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class SkuController : BaseController
{
    private readonly ISkuService _skuService;
    private readonly ILogger<SkuController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="skuService">商品SKU服务接口</param>
    /// <param name="logger">日志记录器</param>
    public SkuController(ISkuService skuService, ILogger<SkuController> logger)
    {
        _skuService = skuService;
        _logger = logger;
    }

    /// <summary>
    /// 获取商品SKU分页列表
    /// </summary>
    /// <param name="skuName">SKU名称关键词（可选），支持模糊搜索</param>
    /// <param name="skuCode">SKU编码（可选），支持模糊搜索</param>
    /// <param name="spuId">SPU ID（可选），精确匹配</param>
    /// <param name="status">状态筛选（可选），0=禁用，1=启用</param>
    /// <param name="pageIndex">页码，从 1 开始，默认 1</param>
    /// <param name="pageSize">每页数量，默认 10</param>
    /// <returns>商品SKU分页列表</returns>
    /// <remarks>
    /// 1. 支持按SKU名称模糊搜索、SKU编码模糊搜索、SPU ID精确匹配、状态筛选
    /// 2. 支持分页查询，返回分页信息和SKU列表
    /// 3. 默认按创建时间倒序排列
    /// </remarks>
    [HttpGet("list")]
    public async Task<ApiResponse<PageResponse<SkuDto>>> GetList(
        [FromQuery] string? skuName = null,
        [FromQuery] string? skuCode = null,
        [FromQuery] string? spuId = null,
        [FromQuery] int? status = null,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        // 验证状态值是否有效
        if (status.HasValue && !Enum.IsDefined(typeof(Status), status.Value))
        {
            return Error<PageResponse<SkuDto>>("无效的状态值", 400);
        }

        var query = new SkuQueryDto
        {
            SkuName = skuName,
            SkuCode = skuCode,
            SpuId = spuId,
            Status = status.HasValue ? (Status)status.Value : null,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _skuService.GetSkuPageListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取商品SKU详情
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>商品SKU详情</returns>
    /// <remarks>
    /// 根据SKU ID获取SKU的详细信息，包括SKU名称、编码、条码、规格组合、价格、库存、状态等所有字段
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SkuDto>> GetById(string id)
    {
        var result = await _skuService.GetSkuByIdAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 按SPU查询商品SKU列表
    /// </summary>
    /// <param name="spuId">商品SPU ID</param>
    /// <returns>商品SKU列表</returns>
    /// <remarks>
    /// 1. 查询指定SPU下的所有SKU
    /// 2. 返回该SPU下所有状态的SKU
    /// 3. 用于商品编辑页面展示SKU列表
    /// </remarks>
    [HttpGet("spu/{spuId}")]
    public async Task<ApiResponse<List<SkuDto>>> GetBySpu(string spuId)
    {
        var result = await _skuService.GetSkuListBySpuAsync(spuId);
        return Success(result);
    }

    /// <summary>
    /// 创建商品SKU
    /// </summary>
    /// <param name="dto">创建商品SKU参数</param>
    /// <returns>创建结果</returns>
    /// <remarks>
    /// 1. SKU名称必填，最大长度 200 个字符
    /// 2. SPU ID 必填，关联的商品主档必须存在
    /// 3. 零售价必填，必须大于或等于 0
    /// 4. SKU编码唯一，不能与已有SKU编码重复
    /// 5. 创建成功返回 true
    /// </remarks>
    [HttpPost]
    public async Task<ApiResponse<object>> Create([FromBody] CreateSkuDto dto)
    {
        var result = await _skuService.CreateSkuAsync(dto);
        return result ? Success("创建成功") : Error<object>("创建失败");
    }

    /// <summary>
    /// 更新商品SKU
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="dto">更新商品SKU参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. SKU名称必填，最大长度 200 个字符
    /// 2. SPU ID 必填，关联的商品主档必须存在
    /// 3. 零售价必填，必须大于或等于 0
    /// 4. SKU编码不能与其他SKU重复
    /// 5. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}")]
    public async Task<ApiResponse<object>> Update(string id, [FromBody] UpdateSkuDto dto)
    {
        // 确保 DTO 中的 ID 与路径参数一致
        dto.Id = id;
        var result = await _skuService.UpdateSkuAsync(dto);
        return result ? Success("更新成功") : Error<object>("更新失败");
    }

    /// <summary>
    /// 删除商品SKU
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>删除结果</returns>
    /// <remarks>
    /// 1. 软删除，不会物理删除数据
    /// 2. 删除成功返回 true
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ApiResponse<object>> Delete(string id)
    {
        var result = await _skuService.DeleteSkuAsync(id);
        return result ? Success("删除成功") : Error<object>("删除失败");
    }

    /// <summary>
    /// 更新商品SKU状态
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="dto">状态参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. 切换SKU的启用/禁用状态
    /// 2. 禁用SKU时，不会影响其他SKU或SPU状态
    /// 3. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<object>> UpdateStatus(string id, [FromBody] UpdateSkuStatusDto dto)
    {
        var result = await _skuService.UpdateSkuStatusAsync(id, dto.Status);
        return result ? Success("状态更新成功") : Error<object>("状态更新失败");
    }

    /// <summary>
    /// 更新商品SKU库存
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="dto">库存参数</param>
    /// <returns>更新结果</returns>
    /// <remarks>
    /// 1. 直接设置SKU的库存数量（非增量更新）
    /// 2. 库存数量必须大于或等于 0
    /// 3. 更新成功返回 true
    /// </remarks>
    [HttpPut("{id}/stock")]
    public async Task<ApiResponse<object>> UpdateStock(string id, [FromBody] UpdateSkuStockDto dto)
    {
        var result = await _skuService.UpdateSkuStockAsync(id, dto.Stock);
        return result ? Success("库存更新成功") : Error<object>("库存更新失败");
    }
}

/// <summary>
/// 更新商品SKU状态请求 DTO
/// </summary>
public class UpdateSkuStatusDto
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

/// <summary>
/// 更新商品SKU库存请求 DTO
/// </summary>
public class UpdateSkuStockDto
{
    /// <summary>
    /// 库存数量
    /// </summary>
    /// <remarks>
    /// 必须大于或等于 0
    /// </remarks>
    public int Stock { get; set; }
}