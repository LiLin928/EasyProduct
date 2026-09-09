using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Product.Channel;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Enums.Product;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Product;

/// <summary>
/// 渠道发布服务实现
/// </summary>
/// <remarks>
/// 提供商品渠道发布的创建、管理、上架状态控制等功能
/// 支持三端渠道：官网（site）、小程序（miniapp）、B2B（b2b）
/// </remarks>
public class ChannelService : BaseService, IChannelService
{
    private readonly ILogger<ChannelService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public ChannelService(ILogger<ChannelService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取渠道发布列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>渠道发布列表</returns>
    public async Task<List<ChannelDto>> GetChannelListAsync(ChannelQueryDto query)
    {
        var queryable = _db.Queryable<product_channel>()
            .Where(x => x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.SpuId))
        {
            queryable = queryable.Where(x => x.SpuId == query.SpuId);
        }

        if (!string.IsNullOrEmpty(query.ChannelCode))
        {
            queryable = queryable.Where(x => x.ChannelCode == query.ChannelCode);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        var channels = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        var channelDtos = channels.Adapt<List<ChannelDto>>();

        // 关联商品名称
        foreach (var dto in channelDtos)
        {
            var spu = await _db.Queryable<product_spu>()
                .Where(x => x.Id.ToString() == dto.SpuId)
                .FirstAsync();
            dto.SpuName = spu?.SpuName;

            // 设置渠道名称
            dto.ChannelName = GetChannelName(dto.ChannelCode);
        }

        return channelDtos;
    }

    /// <summary>
    /// 获取渠道发布详情
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>渠道发布详情</returns>
    public async Task<ChannelDto> GetChannelByIdAsync(string id)
    {
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        var dto = channel.Adapt<ChannelDto>();

        // 关联商品名称
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId)
            .FirstAsync();
        dto.SpuName = spu?.SpuName;

        // 设置渠道名称
        dto.ChannelName = GetChannelName(dto.ChannelCode);

        return dto;
    }

    /// <summary>
    /// 获取商品的渠道发布情况
    /// </summary>
    /// <param name="spuId">商品ID</param>
    /// <returns>渠道发布列表</returns>
    public async Task<List<ChannelDto>> GetSpuChannelsAsync(string spuId)
    {
        var channels = await _db.Queryable<product_channel>()
            .Where(x => x.SpuId == spuId && x.IsDeleted == 0)
            .OrderBy(x => x.Sort)
            .ToListAsync();

        var channelDtos = channels.Adapt<List<ChannelDto>>();

        // 设置渠道名称
        foreach (var dto in channelDtos)
        {
            dto.ChannelName = GetChannelName(dto.ChannelCode);
        }

        return channelDtos;
    }

    /// <summary>
    /// 创建渠道发布
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新渠道发布ID</returns>
    public async Task<string> CreateChannelAsync(CreateChannelDto dto)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        // 验证渠道编码是否有效
        if (!ChannelCode.All.Contains(dto.ChannelCode))
        {
            throw new BusinessException($"无效的渠道编码：{dto.ChannelCode}");
        }

        // 检查是否已发布到该渠道
        var exists = await _db.Queryable<product_channel>()
            .Where(x => x.SpuId == dto.SpuId && x.ChannelCode == dto.ChannelCode && x.IsDeleted == 0)
            .AnyAsync();

        if (exists)
        {
            throw new BusinessException($"该商品已发布到渠道：{dto.ChannelCode}");
        }

        // 创建渠道发布实体
        var channel = dto.Adapt<product_channel>();
        channel.Id = Guid.NewGuid();
        channel.CreatedAt = DateTime.UtcNow;

        // 如果上架，设置发布时间
        if (channel.Status == 1 && !channel.PublishTime.HasValue)
        {
            channel.PublishTime = DateTime.UtcNow;
        }

        // 插入数据库
        await _db.Insertable(channel).ExecuteCommandAsync();

        _logger.LogInformation("创建渠道发布成功：商品ID: {SpuId}, 渠道: {ChannelCode}", dto.SpuId, dto.ChannelCode);

        return channel.Id.ToString();
    }

    /// <summary>
    /// 批量发布到渠道
    /// </summary>
    /// <param name="dto">批量发布参数</param>
    /// <returns>成功发布的数量</returns>
    public async Task<int> BatchPublishAsync(BatchPublishDto dto)
    {
        // 验证商品是否存在
        var spu = await _db.Queryable<product_spu>()
            .Where(x => x.Id.ToString() == dto.SpuId && x.IsDeleted == 0)
            .FirstAsync();

        if (spu == null)
        {
            throw new BusinessException("商品不存在", 404);
        }

        var channelEntities = new List<product_channel>();

        foreach (var channelCode in dto.ChannelCodes)
        {
            // 验证渠道编码是否有效
            if (!ChannelCode.All.Contains(channelCode))
            {
                _logger.LogWarning("跳过无效的渠道编码：{ChannelCode}", channelCode);
                continue;
            }

            // 检查是否已发布到该渠道
            var exists = await _db.Queryable<product_channel>()
                .Where(x => x.SpuId == dto.SpuId && x.ChannelCode == channelCode && x.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                _logger.LogWarning("商品已发布到渠道，跳过：{ChannelCode}", channelCode);
                continue;
            }

            // 创建渠道发布实体
            var channel = new product_channel
            {
                Id = Guid.NewGuid(),
                SpuId = dto.SpuId,
                ChannelCode = channelCode,
                Status = dto.Status,
                Sort = channelEntities.Count,
                CreatedAt = DateTime.UtcNow
            };

            // 如果上架，设置发布时间
            if (channel.Status == 1)
            {
                channel.PublishTime = DateTime.UtcNow;
            }

            channelEntities.Add(channel);
        }

        if (channelEntities.Count == 0)
        {
            return 0;
        }

        // 批量插入
        var count = await _db.Insertable(channelEntities).ExecuteCommandAsync();

        _logger.LogInformation("批量发布到渠道成功：商品ID: {SpuId}, 数量: {Count}", dto.SpuId, count);

        return count;
    }

    /// <summary>
    /// 更新渠道发布
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateChannelAsync(UpdateChannelDto dto)
    {
        // 检查渠道发布是否存在
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        // 更新渠道发布信息
        channel.Status = dto.Status;
        channel.Sort = dto.Sort;
        channel.Price = dto.Price;
        channel.ShowPrice = dto.ShowPrice;
        channel.ShowStock = dto.ShowStock;
        channel.PublishTime = dto.PublishTime;
        channel.UnpublishTime = dto.UnpublishTime;
        channel.UpdatedAt = DateTime.UtcNow;

        // 如果从未上架变为上架，设置发布时间
        if (channel.Status == 1 && !channel.PublishTime.HasValue)
        {
            channel.PublishTime = DateTime.UtcNow;
        }

        // 如果从上架变为下架，设置下架时间
        if (channel.Status == 0 && !channel.UnpublishTime.HasValue)
        {
            channel.UnpublishTime = DateTime.UtcNow;
        }

        // 更新数据库
        await _db.Updateable(channel).ExecuteCommandAsync();

        _logger.LogInformation("更新渠道发布成功：ID: {Id}", dto.Id);

        return true;
    }

    /// <summary>
    /// 删除渠道发布
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteChannelAsync(string id)
    {
        // 检查渠道发布是否存在
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        // 软删除
        channel.IsDeleted = 1;
        channel.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(channel).ExecuteCommandAsync();

        _logger.LogInformation("删除渠道发布成功：ID: {Id}", id);

        return true;
    }

    /// <summary>
    /// 更新上架状态
    /// </summary>
    /// <param name="id">渠道发布ID</param>
    /// <param name="status">上架状态</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateChannelStatusAsync(string id, int status)
    {
        var channel = await _db.Queryable<product_channel>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (channel == null)
        {
            throw new BusinessException("渠道发布不存在", 404);
        }

        channel.Status = status;
        channel.UpdatedAt = DateTime.UtcNow;

        // 如果上架，设置发布时间
        if (status == 1 && !channel.PublishTime.HasValue)
        {
            channel.PublishTime = DateTime.UtcNow;
        }

        // 如果下架，设置下架时间
        if (status == 0 && !channel.UnpublishTime.HasValue)
        {
            channel.UnpublishTime = DateTime.UtcNow;
        }

        await _db.Updateable(channel).ExecuteCommandAsync();

        _logger.LogInformation("更新渠道发布状态成功：ID: {Id}, 状态: {Status}", id, status);

        return true;
    }

    /// <summary>
    /// 批量更新上架状态
    /// </summary>
    /// <param name="ids">渠道发布ID列表</param>
    /// <param name="status">上架状态</param>
    /// <returns>成功更新的数量</returns>
    public async Task<int> BatchUpdateStatusAsync(List<string> ids, int status)
    {
        var channels = await _db.Queryable<product_channel>()
            .Where(x => ids.Contains(x.Id.ToString()) && x.IsDeleted == 0)
            .ToListAsync();

        if (channels.Count == 0)
        {
            return 0;
        }

        var now = DateTime.UtcNow;

        // 批量更新状态
        foreach (var channel in channels)
        {
            channel.Status = status;
            channel.UpdatedAt = now;

            // 如果上架，设置发布时间
            if (status == 1 && !channel.PublishTime.HasValue)
            {
                channel.PublishTime = now;
            }

            // 如果下架，设置下架时间
            if (status == 0 && !channel.UnpublishTime.HasValue)
            {
                channel.UnpublishTime = now;
            }
        }

        var count = await _db.Updateable(channels).ExecuteCommandAsync();

        _logger.LogInformation("批量更新渠道发布状态成功：数量: {Count}, 状态: {Status}", count, status);

        return count;
    }

    /// <summary>
    /// 获取渠道名称
    /// </summary>
    /// <param name="channelCode">渠道编码</param>
    /// <returns>渠道名称</returns>
    private string GetChannelName(string channelCode)
    {
        return channelCode switch
        {
            ChannelCode.Site => "官网",
            ChannelCode.MiniApp => "小程序",
            ChannelCode.B2B => "B2B",
            _ => "未知渠道"
        };
    }
}