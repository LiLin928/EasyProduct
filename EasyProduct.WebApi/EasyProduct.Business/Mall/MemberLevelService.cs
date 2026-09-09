using Mapster;
using SqlSugar;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.MemberLevel;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 会员等级服务实现
/// </summary>
/// <remarks>
/// 提供会员等级的增删改查、等级计算等功能
/// 继承 BaseService，使用属性注入的数据库上下文
/// </remarks>
public class MemberLevelService : BaseService, IMemberLevelService
{
    private readonly ILogger<MemberLevelService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public MemberLevelService(ILogger<MemberLevelService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取会员等级分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、状态、分页信息</param>
    /// <returns>会员等级分页结果</returns>
    public async Task<PageResponse<MemberLevelDto>> GetListAsync(MemberLevelQuery query)
    {
        var queryable = _db.Queryable<MemberLevel>()
            .Where(x => x.IsDeleted == 0);

        // 关键词搜索（等级名称或等级编码）
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where(x =>
                x.LevelName.Contains(query.Keyword) || x.LevelCode.Contains(query.Keyword));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        // 按等级数值升序排列
        queryable = queryable.OrderBy(x => x.Level);

        // 分页查询
        RefAsync<int> total = 0;
        var list = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<MemberLevelDto>.Create(
            list.Adapt<List<MemberLevelDto>>(),
            total.Value,
            query.PageIndex,
            query.PageSize);
    }

    /// <summary>
    /// 获取所有启用的会员等级
    /// </summary>
    /// <returns>启用的会员等级列表</returns>
    public async Task<List<MemberLevelDto>> GetAllAsync()
    {
        var list = await _db.Queryable<MemberLevel>()
            .Where(x => x.Status == MemberStatus.Enabled && x.IsDeleted == 0)
            .OrderBy(x => x.Level)
            .ToListAsync();

        return list.Adapt<List<MemberLevelDto>>();
    }

    /// <summary>
    /// 获取会员等级详情
    /// </summary>
    /// <param name="id">会员等级ID</param>
    /// <returns>会员等级详情</returns>
    public async Task<MemberLevelDto> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .Where(x => x.Id == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("会员等级不存在", 404);
        }

        return entity.Adapt<MemberLevelDto>();
    }

    /// <summary>
    /// 创建会员等级
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的会员等级ID</returns>
    public async Task<Guid> CreateAsync(MemberLevelCreateDto dto)
    {
        // 检查等级编码唯一性
        var codeExists = await _db.Queryable<MemberLevel>()
            .Where(x => x.LevelCode == dto.LevelCode && x.IsDeleted == 0)
            .AnyAsync();

        if (codeExists)
        {
            throw new BusinessException($"等级编码 {dto.LevelCode} 已存在", 400);
        }

        // 检查等级数值唯一性
        var levelExists = await _db.Queryable<MemberLevel>()
            .Where(x => x.Level == dto.Level && x.IsDeleted == 0)
            .AnyAsync();

        if (levelExists)
        {
            throw new BusinessException($"等级数值 {dto.Level} 已存在", 400);
        }

        var entity = dto.Adapt<MemberLevel>();
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.IsDeleted = 0;

        // 如果 DTO 中的 Status 为 0（禁用），则使用枚举值；否则默认启用
        if (dto.Status == 0)
        {
            entity.Status = MemberStatus.Disabled;
        }
        else
        {
            entity.Status = MemberStatus.Enabled;
        }

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建会员等级成功: {LevelCode}, ID: {Id}", entity.LevelCode, entity.Id);

        return entity.Id;
    }

    /// <summary>
    /// 更新会员等级
    /// </summary>
    /// <param name="id">会员等级ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, MemberLevelUpdateDto dto)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .Where(x => x.Id == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("会员等级不存在", 404);
        }

        // 检查等级编码唯一性（排除自身）
        if (!string.IsNullOrEmpty(dto.LevelCode) && dto.LevelCode != entity.LevelCode)
        {
            var codeExists = await _db.Queryable<MemberLevel>()
                .Where(x => x.LevelCode == dto.LevelCode && x.Id != id && x.IsDeleted == 0)
                .AnyAsync();

            if (codeExists)
            {
                throw new BusinessException($"等级编码 {dto.LevelCode} 已存在", 400);
            }
        }

        // 检查等级数值唯一性（排除自身）
        if (dto.Level.HasValue && dto.Level.Value != entity.Level)
        {
            var levelExists = await _db.Queryable<MemberLevel>()
                .Where(x => x.Level == dto.Level.Value && x.Id != id && x.IsDeleted == 0)
                .AnyAsync();

            if (levelExists)
            {
                throw new BusinessException($"等级数值 {dto.Level} 已存在", 400);
            }
        }

        // 更新字段（只更新非空字段）
        if (!string.IsNullOrEmpty(dto.LevelName)) entity.LevelName = dto.LevelName;
        if (!string.IsNullOrEmpty(dto.LevelCode)) entity.LevelCode = dto.LevelCode;
        if (dto.Level.HasValue) entity.Level = dto.Level.Value;
        if (dto.MinPoints.HasValue) entity.MinPoints = dto.MinPoints.Value;
        if (dto.MaxPoints.HasValue) entity.MaxPoints = dto.MaxPoints.Value;
        if (dto.DiscountRate.HasValue) entity.DiscountRate = dto.DiscountRate.Value;
        if (dto.Icon != null) entity.Icon = dto.Icon;
        if (dto.Status.HasValue) entity.Status = (MemberStatus)dto.Status.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新会员等级成功: {LevelCode}, ID: {Id}", entity.LevelCode, entity.Id);

        return true;
    }

    /// <summary>
    /// 删除会员等级
    /// </summary>
    /// <param name="id">会员等级ID</param>
    /// <returns>删除是否成功</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .Where(x => x.Id == id && x.IsDeleted == 0)
            .FirstAsync();

        if (entity == null)
        {
            throw new BusinessException("会员等级不存在", 404);
        }

        // 检查是否有会员使用此等级
        var memberCount = await _db.Queryable<Member>()
            .Where(x => x.LevelId == id && x.IsDeleted == 0)
            .CountAsync();

        if (memberCount > 0)
        {
            throw new BusinessException($"该等级下有 {memberCount} 个会员，无法删除", 400);
        }

        // 软删除
        entity.IsDeleted = 1;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除会员等级成功: {LevelCode}, ID: {Id}", entity.LevelCode, entity.Id);

        return true;
    }

    /// <summary>
    /// 根据积分计算会员等级
    /// </summary>
    /// <param name="points">会员积分</param>
    /// <returns>会员等级实体，未找到返回 null</returns>
    public async Task<MemberLevel?> CalculateLevelByPointsAsync(int points)
    {
        // 查询满足条件的等级：
        // 1. 状态为启用
        // 2. 积分在范围内：MinPoints ≤ points ≤ MaxPoints 或 MaxPoints=0（表示无上限）
        // 3. 按等级降序，取第一个（等级数值最大的）
        var level = await _db.Queryable<MemberLevel>()
            .Where(x =>
                x.Status == MemberStatus.Enabled &&
                x.IsDeleted == 0 &&
                x.MinPoints <= points &&
                (x.MaxPoints == 0 || x.MaxPoints >= points))
            .OrderBy(x => x.Level, OrderByType.Desc)
            .FirstAsync();

        return level;
    }
}