using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Sku;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Enums;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 商品SKU服务实现
/// </summary>
/// <remarks>
/// 提供商品SKU（库存量单位）的增删改查、分页查询、状态更新、库存更新等功能
/// 继承 BaseService，使用属性注入获取数据库上下文
/// </remarks>
public class SkuService : BaseService, ISkuService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<SkuService> _logger;

    /// <summary>
    /// 构造函数，通过依赖注入获取日志记录器
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public SkuService(ILogger<SkuService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取商品SKU分页列表
    /// </summary>
    /// <param name="query">查询参数，支持SKU名称、SKU编码、SPU ID、状态筛选</param>
    /// <returns>商品SKU分页列表</returns>
    /// <remarks>
    /// 1. 支持按SKU名称模糊搜索
    /// 2. 支持按SKU编码模糊搜索
    /// 3. 支持按SPU ID精确匹配
    /// 4. 支持按状态筛选
    /// 5. 支持分页查询
    /// 6. 按创建时间倒序排列
    /// 7. 关联SPU表获取SPU名称
    /// </remarks>
    public async Task<PageResponse<SkuDto>> GetSkuPageListAsync(SkuQueryDto query)
    {
        // 1. 构建分页查询，关联SPU表获取SPU名称
        var queryable = _db.Queryable<product_sku, product_spu>((sku, spu) => new JoinQueryInfos(
                JoinType.Left, sku.SpuId == spu.Id.ToString() && spu.IsDeleted == 0
            ))
            .Where((sku, spu) => sku.IsDeleted == 0)
            .WhereIF(!string.IsNullOrEmpty(query.SkuName), (sku, spu) => sku.SkuName.Contains(query.SkuName!))
            .WhereIF(!string.IsNullOrEmpty(query.SkuCode), (sku, spu) => sku.SkuCode!.Contains(query.SkuCode!))
            .WhereIF(!string.IsNullOrEmpty(query.SpuId), (sku, spu) => sku.SpuId == query.SpuId)
            .WhereIF(query.Status.HasValue, (sku, spu) => sku.Status == query.Status!.Value)
            .OrderByDescending((sku, spu) => sku.CreatedAt)
            .Select((sku, spu) => new SkuDto
            {
                Id = sku.Id,
                SpuId = sku.SpuId,
                SpuName = spu.SpuName,
                SkuName = sku.SkuName,
                SkuCode = sku.SkuCode,
                Barcode = sku.Barcode,
                SpecJson = sku.SpecJson,
                Price = sku.Price,
                MemberPrice = sku.MemberPrice,
                WholesalePrice = sku.WholesalePrice,
                CostPrice = sku.CostPrice,
                Stock = sku.Stock,
                Status = sku.Status,
                CreatedAt = sku.CreatedAt,
                UpdatedAt = sku.UpdatedAt,
                CreatedBy = sku.CreatedBy,
                UpdatedBy = sku.UpdatedBy
            });

        // 2. 执行分页查询
        RefAsync<int> total = 0;
        var skuList = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 3. 返回分页结果
        return PageResponse<SkuDto>.Create(skuList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取商品SKU详情
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>商品SKU详情</returns>
    /// <exception cref="BusinessException">SKU不存在时抛出</exception>
    /// <remarks>
    /// 根据SKU ID查询SKU详细信息，包含SPU名称
    /// </remarks>
    public async Task<SkuDto> GetSkuByIdAsync(string id)
    {
        // 1. 查询SKU，关联SPU表获取SPU名称
        var sku = await _db.Queryable<product_sku, product_spu>((sku, spu) => new JoinQueryInfos(
                JoinType.Left, sku.SpuId == spu.Id.ToString() && spu.IsDeleted == 0
            ))
            .Where((sku, spu) => sku.Id.ToString() == id && sku.IsDeleted == 0)
            .Select((sku, spu) => new SkuDto
            {
                Id = sku.Id,
                SpuId = sku.SpuId,
                SpuName = spu.SpuName,
                SkuName = sku.SkuName,
                SkuCode = sku.SkuCode,
                Barcode = sku.Barcode,
                SpecJson = sku.SpecJson,
                Price = sku.Price,
                MemberPrice = sku.MemberPrice,
                WholesalePrice = sku.WholesalePrice,
                CostPrice = sku.CostPrice,
                Stock = sku.Stock,
                Status = sku.Status,
                CreatedAt = sku.CreatedAt,
                UpdatedAt = sku.UpdatedAt,
                CreatedBy = sku.CreatedBy,
                UpdatedBy = sku.UpdatedBy
            })
            .FirstAsync();

        // 2. 检查SKU是否存在
        if (sku == null)
        {
            throw new BusinessException("SKU不存在", 404);
        }

        // 3. 返回SKU详情
        return sku;
    }

    /// <summary>
    /// 按SPU查询商品SKU列表
    /// </summary>
    /// <param name="spuId">商品SPU ID</param>
    /// <returns>商品SKU列表</returns>
    /// <remarks>
    /// 查询指定SPU下的所有SKU
    /// </remarks>
    public async Task<List<SkuDto>> GetSkuListBySpuAsync(string spuId)
    {
        // 1. 查询指定SPU下的所有SKU，关联SPU表获取SPU名称
        var skuList = await _db.Queryable<product_sku, product_spu>((sku, spu) => new JoinQueryInfos(
                JoinType.Left, sku.SpuId == spu.Id.ToString() && spu.IsDeleted == 0
            ))
            .Where((sku, spu) => sku.SpuId == spuId && sku.IsDeleted == 0)
            .OrderByDescending((sku, spu) => sku.CreatedAt)
            .Select((sku, spu) => new SkuDto
            {
                Id = sku.Id,
                SpuId = sku.SpuId,
                SpuName = spu.SpuName,
                SkuName = sku.SkuName,
                SkuCode = sku.SkuCode,
                Barcode = sku.Barcode,
                SpecJson = sku.SpecJson,
                Price = sku.Price,
                MemberPrice = sku.MemberPrice,
                WholesalePrice = sku.WholesalePrice,
                CostPrice = sku.CostPrice,
                Stock = sku.Stock,
                Status = sku.Status,
                CreatedAt = sku.CreatedAt,
                UpdatedAt = sku.UpdatedAt,
                CreatedBy = sku.CreatedBy,
                UpdatedBy = sku.UpdatedBy
            })
            .ToListAsync();

        // 2. 返回SKU列表
        return skuList;
    }

    /// <summary>
    /// 创建商品SKU
    /// </summary>
    /// <param name="dto">创建商品SKU参数</param>
    /// <returns>创建成功返回 true</returns>
    /// <exception cref="BusinessException">SKU编码已存在、SPU不存在时抛出</exception>
    /// <remarks>
    /// 1. 检查SPU是否存在
    /// 2. 检查SKU编码唯一性
    /// 3. 创建SKU记录
    /// 4. 默认状态为启用
    /// </remarks>
    public async Task<bool> CreateSkuAsync(CreateSkuDto dto)
    {
        // 1. 检查SPU是否存在
        var spuExists = await _db.Queryable<product_spu>()
            .Where(spu => spu.Id.ToString() == dto.SpuId && spu.IsDeleted == 0)
            .AnyAsync();

        if (!spuExists)
        {
            throw new BusinessException("SPU不存在", 404);
        }

        // 2. 检查SKU编码是否已存在
        if (!string.IsNullOrEmpty(dto.SkuCode))
        {
            var exists = await _db.Queryable<product_sku>()
                .Where(sku => sku.SkuCode == dto.SkuCode && sku.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException("SKU编码已存在", 400);
            }
        }

        // 3. 创建SKU实体
        var sku = dto.Adapt<product_sku>();
        sku.Id = Guid.NewGuid();
        sku.Status = Status.Enabled;
        sku.IsDeleted = 0;
        sku.CreatedAt = DateTime.UtcNow;

        // 4. 插入数据库
        var result = await _db.Insertable(sku).ExecuteCommandAsync();

        // 5. 记录日志
        if (result > 0)
        {
            _logger.LogInformation("成功创建SKU: {SkuId}, 名称: {SkuName}, 编码: {SkuCode}", sku.Id, sku.SkuName, sku.SkuCode);
        }

        return result > 0;
    }

    /// <summary>
    /// 更新商品SKU
    /// </summary>
    /// <param name="dto">更新商品SKU参数</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">SKU不存在、SKU编码已存在、SPU不存在时抛出</exception>
    /// <remarks>
    /// 1. 检查SKU是否存在
    /// 2. 检查SPU是否存在
    /// 3. 检查SKU编码唯一性（排除自己）
    /// 4. 更新SKU记录
    /// </remarks>
    public async Task<bool> UpdateSkuAsync(UpdateSkuDto dto)
    {
        // 1. 查询SKU
        var sku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == dto.Id && s.IsDeleted == 0)
            .FirstAsync();

        if (sku == null)
        {
            throw new BusinessException("SKU不存在", 404);
        }

        // 2. 检查SPU是否存在
        var spuExists = await _db.Queryable<product_spu>()
            .Where(spu => spu.Id.ToString() == dto.SpuId && spu.IsDeleted == 0)
            .AnyAsync();

        if (!spuExists)
        {
            throw new BusinessException("SPU不存在", 404);
        }

        // 3. 检查SKU编码是否已存在（排除自己）
        if (!string.IsNullOrEmpty(dto.SkuCode))
        {
            var exists = await _db.Queryable<product_sku>()
                .Where(s => s.SkuCode == dto.SkuCode && s.Id.ToString() != dto.Id && s.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException("SKU编码已存在", 400);
            }
        }

        // 4. 更新字段
        sku.SpuId = dto.SpuId;
        sku.SkuName = dto.SkuName;
        sku.SkuCode = dto.SkuCode;
        sku.Barcode = dto.Barcode;
        sku.SpecJson = dto.SpecJson;
        sku.Price = dto.Price;
        sku.MemberPrice = dto.MemberPrice;
        sku.WholesalePrice = dto.WholesalePrice;
        sku.CostPrice = dto.CostPrice;
        sku.Stock = dto.Stock;
        sku.Status = dto.Status;
        sku.UpdatedAt = DateTime.UtcNow;

        // 5. 更新数据库
        var result = await _db.Updateable(sku).ExecuteCommandAsync();

        // 6. 记录日志
        if (result > 0)
        {
            _logger.LogInformation("成功更新SKU: {SkuId}, 名称: {SkuName}, 编码: {SkuCode}", sku.Id, sku.SkuName, sku.SkuCode);
        }

        return result > 0;
    }

    /// <summary>
    /// 删除商品SKU（软删除）
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>删除成功返回 true</returns>
    /// <exception cref="BusinessException">SKU不存在时抛出</exception>
    /// <remarks>
    /// 1. 检查SKU是否存在
    /// 2. 软删除SKU记录
    /// </remarks>
    public async Task<bool> DeleteSkuAsync(string id)
    {
        // 1. 查询SKU
        var sku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == id && s.IsDeleted == 0)
            .FirstAsync();

        if (sku == null)
        {
            throw new BusinessException("SKU不存在", 404);
        }

        // 2. 软删除SKU
        sku.IsDeleted = 1;
        sku.UpdatedAt = DateTime.UtcNow;
        sku.Status = Status.Disabled;

        var result = await _db.Updateable(sku).ExecuteCommandAsync();

        // 3. 记录日志
        if (result > 0)
        {
            _logger.LogInformation("成功删除SKU: {SkuId}, 名称: {SkuName}, 编码: {SkuCode}", sku.Id, sku.SkuName, sku.SkuCode);
        }

        return result > 0;
    }

    /// <summary>
    /// 更新商品SKU状态
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="status">状态值：0=禁用，1=启用</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">SKU不存在时抛出</exception>
    /// <remarks>
    /// 更新SKU的启用/禁用状态
    /// </remarks>
    public async Task<bool> UpdateSkuStatusAsync(string id, int status)
    {
        // 1. 查询SKU
        var sku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == id && s.IsDeleted == 0)
            .FirstAsync();

        if (sku == null)
        {
            throw new BusinessException("SKU不存在", 404);
        }

        // 2. 验证状态值是否有效
        if (!Enum.IsDefined(typeof(Status), status))
        {
            throw new BusinessException("无效的状态值", 400);
        }

        // 3. 更新状态
        sku.Status = (Status)status;
        sku.UpdatedAt = DateTime.UtcNow;

        // 4. 更新数据库
        var result = await _db.Updateable(sku)
            .UpdateColumns(s => new { s.Status, s.UpdatedAt })
            .ExecuteCommandAsync();

        // 5. 记录日志
        if (result > 0)
        {
            _logger.LogInformation("成功更新SKU状态: {SkuId}, 名称: {SkuName}, 新状态: {Status}", sku.Id, sku.SkuName, sku.Status);
        }

        return result > 0;
    }

    /// <summary>
    /// 更新商品SKU库存
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="stock">库存数量</param>
    /// <returns>更新成功返回 true</returns>
    /// <exception cref="BusinessException">SKU不存在、库存不能为负时抛出</exception>
    /// <remarks>
    /// 1. 检查SKU是否存在
    /// 2. 验证库存数量不能为负数
    /// 3. 更新SKU库存
    /// </remarks>
    public async Task<bool> UpdateSkuStockAsync(string id, int stock)
    {
        // 1. 查询SKU
        var sku = await _db.Queryable<product_sku>()
            .Where(s => s.Id.ToString() == id && s.IsDeleted == 0)
            .FirstAsync();

        if (sku == null)
        {
            throw new BusinessException("SKU不存在", 404);
        }

        // 2. 验证库存不能为负数
        if (stock < 0)
        {
            throw new BusinessException("库存不能为负数", 400);
        }

        // 3. 更新库存
        sku.Stock = stock;
        sku.UpdatedAt = DateTime.UtcNow;

        // 4. 更新数据库
        var result = await _db.Updateable(sku)
            .UpdateColumns(s => new { s.Stock, s.UpdatedAt })
            .ExecuteCommandAsync();

        // 5. 记录日志
        if (result > 0)
        {
            _logger.LogInformation("成功更新SKU库存: {SkuId}, 名称: {SkuName}, 新库存: {Stock}", sku.Id, sku.SkuName, stock);
        }

        return result > 0;
    }
}