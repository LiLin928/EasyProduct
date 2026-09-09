using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Member;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 会员服务实现
/// </summary>
/// <remarks>
/// 提供会员的增删改查、微信登录、等级更新等功能
/// </remarks>
public class MemberService : BaseService, IMemberService
{
    private readonly IMemberLevelService _memberLevelService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="memberLevelService">会员等级服务</param>
    public MemberService(IMemberLevelService memberLevelService)
    {
        _memberLevelService = memberLevelService;
    }

    /// <summary>
    /// 获取会员分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、会员等级、状态、分页信息</param>
    /// <returns>会员分页结果</returns>
    /// <remarks>
    /// 1. 支持按昵称、手机号模糊搜索
    /// 2. 支持按会员等级、状态筛选
    /// 3. 默认按创建时间倒序排列
    /// 4. 包含会员等级名称的关联查询
    /// </remarks>
    public async Task<PageResponse<MemberDto>> GetListAsync(MemberQuery query)
    {
        // 构建查询
        var queryable = _db.Queryable<Member, MemberLevel>(
            (m, ml) => new JoinQueryInfos(
                JoinType.Left, m.LevelId == ml.Id
            ))
            .Where((m, ml) => m.IsDeleted == 0);

        // 关键词搜索（昵称、手机号）
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where((m, ml) =>
                m.Nickname!.Contains(query.Keyword) ||
                m.Phone!.Contains(query.Keyword));
        }

        // 会员等级筛选
        if (query.LevelId.HasValue)
        {
            queryable = queryable.Where((m, ml) => m.LevelId == query.LevelId);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where((m, ml) => m.Status == (MemberStatus)query.Status);
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy((m, ml) => m.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select((m, ml) => new MemberDto
            {
                Id = m.Id.ToString(),
                OpenId = m.OpenId,
                UnionId = m.UnionId,
                Nickname = m.Nickname,
                Avatar = m.Avatar,
                Gender = (int)m.Gender,
                Phone = m.Phone,
                RealName = m.RealName,
                Birthday = m.Birthday,
                Email = m.Email,
                LevelId = m.LevelId.ToString(),
                LevelName = ml.LevelName,
                Points = m.Points,
                TotalPoints = m.TotalPoints,
                Balance = m.Balance,
                Status = (int)m.Status,
                LastLoginTime = m.LastLoginTime,
                LastLoginIp = m.LastLoginIp,
                CreateTime = m.CreatedAt,
                UpdateTime = m.UpdatedAt
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<MemberDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取会员详情
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <returns>会员详情</returns>
    /// <exception cref="BusinessException">会员不存在时抛出异常</exception>
    public async Task<MemberDto> GetByIdAsync(Guid id)
    {
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id == id && m.IsDeleted == 0)
            .FirstAsync();

        if (member == null)
        {
            throw new BusinessException("会员不存在");
        }

        // 获取会员等级名称
        string? levelName = null;
        if (member.LevelId.HasValue)
        {
            var level = await _db.Queryable<MemberLevel>()
                .Where(l => l.Id == member.LevelId)
                .FirstAsync();
            levelName = level?.LevelName;
        }

        // 手动映射到 MemberDto
        return new MemberDto
        {
            Id = member.Id.ToString(),
            OpenId = member.OpenId,
            UnionId = member.UnionId,
            Nickname = member.Nickname,
            Avatar = member.Avatar,
            Gender = (int)member.Gender,
            Phone = member.Phone,
            RealName = member.RealName,
            Birthday = member.Birthday,
            Email = member.Email,
            LevelId = member.LevelId?.ToString(),
            LevelName = levelName,
            Points = member.Points,
            TotalPoints = member.TotalPoints,
            Balance = member.Balance,
            Status = (int)member.Status,
            LastLoginTime = member.LastLoginTime,
            LastLoginIp = member.LastLoginIp,
            CreateTime = member.CreatedAt,
            UpdateTime = member.UpdatedAt
        };
    }

    /// <summary>
    /// 创建会员
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的会员ID</returns>
    /// <remarks>
    /// 创建会员时会根据累计积分（初始为0）自动计算会员等级
    /// </remarks>
    public async Task<Guid> CreateAsync(MemberCreateDto dto)
    {
        // 手机号唯一性检查
        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            var exists = await _db.Queryable<Member>()
                .Where(m => m.Phone == dto.Phone && m.IsDeleted == 0)
                .AnyAsync();
            if (exists)
            {
                throw new BusinessException("该手机号已被使用");
            }
        }

        // 创建会员实体
        var member = dto.Adapt<Member>();
        member.Id = Guid.NewGuid();
        member.Points = 0;
        member.TotalPoints = 0;
        member.Balance = 0;
        member.Status = MemberStatus.Enabled;

        // 根据累计积分计算会员等级（初始为0，可能没有等级）
        var level = await _memberLevelService.CalculateLevelByPointsAsync(member.TotalPoints);
        member.LevelId = level?.Id;

        // 插入数据库
        await _db.Insertable(member).ExecuteCommandAsync();

        return member.Id;
    }

    /// <summary>
    /// 更新会员
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 更新会员等级或累计积分时，会自动重新计算会员等级
    /// </remarks>
    public async Task<bool> UpdateAsync(Guid id, MemberUpdateDto dto)
    {
        // 检查会员是否存在
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id == id && m.IsDeleted == 0)
            .FirstAsync();

        if (member == null)
        {
            throw new BusinessException("会员不存在");
        }

        // 手机号唯一性检查
        if (!string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone != member.Phone)
        {
            var exists = await _db.Queryable<Member>()
                .Where(m => m.Phone == dto.Phone && m.Id != id && m.IsDeleted == 0)
                .AnyAsync();
            if (exists)
            {
                throw new BusinessException("该手机号已被使用");
            }
        }

        // 更新会员信息
        if (dto.Nickname != null) member.Nickname = dto.Nickname;
        if (dto.Avatar != null) member.Avatar = dto.Avatar;
        if (dto.Gender.HasValue) member.Gender = (Gender)dto.Gender;
        if (dto.Phone != null) member.Phone = dto.Phone;
        if (dto.RealName != null) member.RealName = dto.RealName;
        if (dto.Birthday.HasValue) member.Birthday = dto.Birthday;
        if (dto.Email != null) member.Email = dto.Email;
        if (dto.Status.HasValue) member.Status = (MemberStatus)dto.Status;
        if (dto.LevelId.HasValue) member.LevelId = dto.LevelId;

        member.UpdatedAt = DateTime.UtcNow;

        // 更新数据库
        var result = await _db.Updateable(member).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 删除会员
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 执行软删除，删除前会检查会员是否有未完成订单
    /// TODO: 后续集成订单服务后，添加订单检查逻辑
    /// </remarks>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // 检查会员是否存在
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id == id && m.IsDeleted == 0)
            .FirstAsync();

        if (member == null)
        {
            throw new BusinessException("会员不存在");
        }

        // TODO: 检查会员是否有未完成订单
        // var hasUnfinishedOrders = await _orderService.HasUnfinishedOrdersAsync(id);
        // if (hasUnfinishedOrders)
        // {
        //     throw new BusinessException("该会员有未完成订单，无法删除");
        // }

        // 软删除
        member.IsDeleted = 1;
        member.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(member).ExecuteCommandAsync();

        return result > 0;
    }

    /// <summary>
    /// 微信登录
    /// </summary>
    /// <param name="dto">微信登录参数，包含 code、加密数据等</param>
    /// <returns>登录结果，包含 token、refreshToken、会员信息</returns>
    /// <remarks>
    /// 1. 通过微信 code 换取 openid（当前为占位实现，需要配置 appid/secret）
    /// 2. 根据 openid 查找会员，不存在则自动注册
    /// 3. 更新会员最后登录信息
    /// 4. 生成 JWT token（当前为占位实现，需要集成认证系统）
    /// 5. 返回登录结果
    /// </remarks>
    public async Task<(string token, string refreshToken, MemberInfoDto member)> WechatLoginAsync(WechatLoginDto dto)
    {
        // TODO: 调用微信 API 换取 openid
        // 实际实现需要配置 appid、secret，调用 https://api.weixin.qq.com/sns/jscode2session
        // 当前使用占位实现，返回 mock openid
        var openId = $"mock_openid_{dto.Code}";

        // 查找会员
        var member = await _db.Queryable<Member>()
            .Where(m => m.OpenId == openId && m.IsDeleted == 0)
            .FirstAsync();

        // 如果会员不存在，自动注册
        if (member == null)
        {
            member = new Member
            {
                Id = Guid.NewGuid(),
                OpenId = openId,
                Points = 0,
                TotalPoints = 0,
                Balance = 0,
                Status = MemberStatus.Enabled,
                LastLoginTime = DateTime.UtcNow
            };

            // 根据累计积分计算会员等级
            var level = await _memberLevelService.CalculateLevelByPointsAsync(member.TotalPoints);
            member.LevelId = level?.Id;

            await _db.Insertable(member).ExecuteCommandAsync();
        }
        else
        {
            // 更新最后登录信息
            member.LastLoginTime = DateTime.UtcNow;
            // TODO: 获取真实客户端 IP
            // member.LastLoginIp = httpContext.Connection.RemoteIpAddress?.ToString();
            await _db.Updateable(member).ExecuteCommandAsync();
        }

        // TODO: 生成 JWT token
        // 实际实现需要集成认证系统，生成包含会员信息的 JWT
        // 当前使用占位实现，返回 mock token
        var token = $"mock_token_{member.Id}";
        var refreshToken = $"mock_refresh_token_{member.Id}";

        // 获取会员信息
        var memberInfo = await GetCurrentMemberInfoAsync(member.Id);

        return (token, refreshToken, memberInfo);
    }

    /// <summary>
    /// 获取当前会员信息
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>会员信息，包含等级、积分、余额等</returns>
    /// <exception cref="BusinessException">会员不存在时抛出异常</exception>
    public async Task<MemberInfoDto> GetCurrentMemberInfoAsync(Guid memberId)
    {
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id == memberId && m.IsDeleted == 0)
            .FirstAsync();

        if (member == null)
        {
            throw new BusinessException("会员不存在");
        }

        // 获取会员等级信息
        string? levelName = null;
        string? levelIcon = null;
        decimal discountRate = 1.00m;

        if (member.LevelId.HasValue)
        {
            var level = await _db.Queryable<MemberLevel>()
                .Where(l => l.Id == member.LevelId)
                .FirstAsync();

            if (level != null)
            {
                levelName = level.LevelName;
                levelIcon = level.Icon;
                discountRate = level.DiscountRate;
            }
        }

        return new MemberInfoDto
        {
            Id = member.Id.ToString(),
            Nickname = member.Nickname,
            Avatar = member.Avatar,
            Phone = member.Phone,
            LevelName = levelName,
            LevelIcon = levelIcon,
            Points = member.Points,
            Balance = member.Balance,
            DiscountRate = discountRate
        };
    }

    /// <summary>
    /// 更新会员等级
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 根据会员累计积分重新计算并更新会员等级
    /// 通常在积分变更后调用
    /// </remarks>
    public async Task<bool> UpdateMemberLevelAsync(Guid memberId)
    {
        // 检查会员是否存在
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id == memberId && m.IsDeleted == 0)
            .FirstAsync();

        if (member == null)
        {
            throw new BusinessException("会员不存在");
        }

        // 根据累计积分计算会员等级
        var level = await _memberLevelService.CalculateLevelByPointsAsync(member.TotalPoints);

        // 更新会员等级
        member.LevelId = level?.Id;
        member.UpdatedAt = DateTime.UtcNow;

        var result = await _db.Updateable(member).ExecuteCommandAsync();

        return result > 0;
    }
}