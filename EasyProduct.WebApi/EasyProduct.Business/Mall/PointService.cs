using Mapster;
using SqlSugar;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.Point;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 积分服务实现
/// </summary>
/// <remarks>
/// 提供积分规则管理、积分发放、积分消费、积分查询和统计等功能。
/// 继承 BaseService，使用属性注入的数据库上下文。
/// 积分涉及资金流水，所有写操作使用事务确保数据一致性。
/// </remarks>
public class PointService : BaseService, IPointService
{
    private readonly ILogger<PointService> _logger;
    private readonly IMemberService _memberService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    /// <param name="memberService">会员服务</param>
    public PointService(
        ILogger<PointService> logger,
        IMemberService memberService)
    {
        _logger = logger;
        _memberService = memberService;
    }

    #region 积分规则管理

    /// <summary>
    /// 创建积分规则
    /// </summary>
    /// <param name="dto">创建积分规则参数</param>
    /// <returns>规则ID</returns>
    public async Task<string> CreateRuleAsync(CreatePointRuleDto dto)
    {
        // 验证时间范围
        if (dto.StartTime >= dto.EndTime)
        {
            throw BusinessException.BadRequest("生效时间必须早于失效时间");
        }

        // 创建规则实体
        var rule = dto.Adapt<PointRule>();
        rule.Id = Guid.NewGuid();
        rule.CreateTime = DateTime.Now;
        rule.UpdateTime = DateTime.Now;

        // 插入数据库
        await _db.Insertable(rule).ExecuteCommandAsync();

        _logger.LogInformation("创建积分规则成功，规则ID：{RuleId}，规则名称：{RuleName}", rule.Id, rule.Name);

        return rule.Id.ToString();
    }

    /// <summary>
    /// 更新积分规则
    /// </summary>
    /// <param name="dto">更新积分规则参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateRuleAsync(UpdatePointRuleDto dto)
    {
        // 验证时间范围
        if (dto.StartTime >= dto.EndTime)
        {
            throw BusinessException.BadRequest("生效时间必须早于失效时间");
        }

        // 查询规则
        var rule = await _db.Queryable<PointRule>()
            .Where(r => r.Id == Guid.Parse(dto.Id) && !r.IsDeleted)
            .FirstAsync();

        if (rule == null)
        {
            throw BusinessException.NotFound("积分规则不存在");
        }

        // 更新字段
        dto.Adapt(rule);
        rule.UpdateTime = DateTime.Now;

        // 更新数据库
        var result = await _db.Updateable(rule).ExecuteCommandAsync() > 0;

        _logger.LogInformation("更新积分规则成功，规则ID：{RuleId}", rule.Id);

        return result;
    }

    /// <summary>
    /// 删除积分规则
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteRuleAsync(string ruleId)
    {
        // 查询规则
        var rule = await _db.Queryable<PointRule>()
            .Where(r => r.Id == Guid.Parse(ruleId) && !r.IsDeleted)
            .FirstAsync();

        if (rule == null)
        {
            throw BusinessException.NotFound("积分规则不存在");
        }

        // 软删除
        rule.IsDeleted = true;
        rule.UpdateTime = DateTime.Now;

        var result = await _db.Updateable(rule).ExecuteCommandAsync() > 0;

        _logger.LogInformation("删除积分规则成功，规则ID：{RuleId}", ruleId);

        return result;
    }

    /// <summary>
    /// 分页查询积分规则列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>积分规则分页列表</returns>
    public async Task<PageResponse<PointRuleDto>> GetRuleListAsync(PointRuleQueryDto query)
    {
        var queryable = _db.Queryable<PointRule>()
            .Where(r => !r.IsDeleted);

        // 名称模糊查询
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            queryable = queryable.Where(r => r.Name.Contains(query.Name));
        }

        // 类型筛选
        if (query.Type.HasValue)
        {
            queryable = queryable.Where(r => r.Type == query.Type.Value);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(r => r.Status == query.Status.Value);
        }

        // 排序
        queryable = queryable.OrderBy(r => r.Sort).OrderBy(r => r.CreateTime, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 映射 DTO
        var result = list.Adapt<List<PointRuleDto>>();

        return PageResponse<PointRuleDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取积分规则详情
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <returns>积分规则详情</returns>
    public async Task<PointRuleDto> GetRuleDetailAsync(string ruleId)
    {
        var rule = await _db.Queryable<PointRule>()
            .Where(r => r.Id == Guid.Parse(ruleId) && !r.IsDeleted)
            .FirstAsync();

        if (rule == null)
        {
            throw BusinessException.NotFound("积分规则不存在");
        }

        return rule.Adapt<PointRuleDto>();
    }

    /// <summary>
    /// 获取启用的积分规则列表
    /// </summary>
    /// <param name="type">规则类型（可选）</param>
    /// <returns>启用的积分规则列表</returns>
    public async Task<List<PointRuleDto>> GetActiveRulesAsync(PointRuleType? type = null)
    {
        var now = DateTime.Now;

        var queryable = _db.Queryable<PointRule>()
            .Where(r => !r.IsDeleted && r.Status)
            .Where(r => r.StartTime <= now && r.EndTime >= now);

        if (type.HasValue)
        {
            queryable = queryable.Where(r => r.Type == type.Value);
        }

        var list = await queryable.OrderBy(r => r.Sort).ToListAsync();

        return list.Adapt<List<PointRuleDto>>();
    }

    #endregion

    #region 积分发放

    /// <summary>
    /// 下单赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <param name="orderAmount">订单金额</param>
    /// <returns>赠送积分数</returns>
    public async Task<int> GrantOrderPointsAsync(string memberId, string orderId, decimal orderAmount)
    {
        // 获取下单规则
        var rules = await GetActiveRulesAsync(PointRuleType.Order);
        if (rules.Count == 0)
        {
            _logger.LogWarning("未找到下单赠送积分规则");
            return 0;
        }

        // 使用第一个规则（后续可扩展为多条规则叠加）
        var rule = rules[0];
        int points;

        // 计算积分
        if (rule.IsMultiple)
        {
            // 按金额倍数计算
            points = (int)(orderAmount / rule.MultipleBase) * rule.Points;
        }
        else
        {
            // 固定积分
            points = rule.Points;
        }

        // 应用最大积分限制
        if (rule.MaxPoints > 0 && points > rule.MaxPoints)
        {
            points = rule.MaxPoints;
        }

        // 发放积分
        await GrantPointsAsync(memberId, PointSource.Order, points, $"下单赠送积分，订单号：{orderId}", orderId);

        _logger.LogInformation("下单赠送积分成功，会员ID：{MemberId}，订单号：{OrderId}，积分数：{Points}", memberId, orderId, points);

        return points;
    }

    /// <summary>
    /// 评价赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>赠送积分数</returns>
    public async Task<int> GrantReviewPointsAsync(string memberId, string orderId)
    {
        // 获取评价规则
        var rules = await GetActiveRulesAsync(PointRuleType.Review);
        if (rules.Count == 0)
        {
            _logger.LogWarning("未找到评价赠送积分规则");
            return 0;
        }

        var rule = rules[0];
        var points = rule.Points;

        // 发放积分
        await GrantPointsAsync(memberId, PointSource.Review, points, $"评价赠送积分，订单号：{orderId}", orderId);

        _logger.LogInformation("评价赠送积分成功，会员ID：{MemberId}，订单号：{OrderId}，积分数：{Points}", memberId, orderId, points);

        return points;
    }

    /// <summary>
    /// 签到赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>赠送积分数</returns>
    public async Task<int> GrantCheckInPointsAsync(string memberId)
    {
        // 获取签到规则
        var rules = await GetActiveRulesAsync(PointRuleType.CheckIn);
        if (rules.Count == 0)
        {
            _logger.LogWarning("未找到签到赠送积分规则");
            return 0;
        }

        var rule = rules[0];
        var points = rule.Points;

        // 发放积分
        await GrantPointsAsync(memberId, PointSource.CheckIn, points, "签到赠送积分");

        _logger.LogInformation("签到赠送积分成功，会员ID：{MemberId}，积分数：{Points}", memberId, points);

        return points;
    }

    /// <summary>
    /// 邀请赠送积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="inviteeId">被邀请人ID</param>
    /// <returns>赠送积分数</returns>
    public async Task<int> GrantInvitePointsAsync(string memberId, string inviteeId)
    {
        // 获取邀请规则
        var rules = await GetActiveRulesAsync(PointRuleType.Invite);
        if (rules.Count == 0)
        {
            _logger.LogWarning("未找到邀请赠送积分规则");
            return 0;
        }

        var rule = rules[0];
        var points = rule.Points;

        // 发放积分
        await GrantPointsAsync(memberId, PointSource.Invite, points, $"邀请赠送积分，被邀请人ID：{inviteeId}", inviteeId);

        _logger.LogInformation("邀请赠送积分成功，会员ID：{MemberId}，被邀请人ID：{InviteeId}，积分数：{Points}", memberId, inviteeId, points);

        return points;
    }

    /// <summary>
    /// 系统调整积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="points">积分数（正数增加，负数减少）</param>
    /// <param name="remark">备注</param>
    /// <returns>是否成功</returns>
    public async Task<bool> AdjustPointsAsync(string memberId, int points, string remark)
    {
        var source = points > 0 ? PointSource.SystemAdjust : PointSource.SystemAdjust;

        await GrantPointsAsync(memberId, source, points, remark);

        _logger.LogInformation("系统调整积分成功，会员ID：{MemberId}，积分数：{Points}，备注：{Remark}", memberId, points, remark);

        return true;
    }

    /// <summary>
    /// 发放积分（内部方法）
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="source">积分来源</param>
    /// <param name="points">积分数</param>
    /// <param name="remark">备注</param>
    /// <param name="relatedId">关联ID</param>
    private async Task GrantPointsAsync(string memberId, PointSource source, int points, string remark, string? relatedId = null)
    {
        // 使用事务
        await _db.Ado.UseTranAsync(async () =>
        {
            // 查询会员
            var member = await _db.Queryable<Member>()
                .Where(m => m.Id == Guid.Parse(memberId) && !m.IsDeleted)
                .FirstAsync();

            if (member == null)
            {
                throw BusinessException.NotFound("会员不存在");
            }

            // 如果是减少积分，检查余额
            if (points < 0 && member.Points + points < 0)
            {
                throw BusinessException.BadRequest("积分余额不足");
            }

            // 更新会员积分
            member.Points += points;
            if (points > 0)
            {
                member.TotalPoints += points;
            }
            member.UpdateTime = DateTime.Now;

            await _db.Updateable(member).ExecuteCommandAsync();

            // 创建积分流水
            var record = new PointRecord
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                Type = points > 0 ? PointType.Income : PointType.Expense,
                Source = source,
                Points = points,
                Balance = member.Points,
                Remark = remark,
                RelatedId = relatedId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            await _db.Insertable(record).ExecuteCommandAsync();
        });
    }

    #endregion

    #region 积分消费

    /// <summary>
    /// 兑换优惠券
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>兑换记录ID</returns>
    public async Task<string> ExchangeCouponAsync(string memberId, string couponId)
    {
        // TODO: 后续实现优惠券服务
        throw new NotImplementedException("优惠券兑换功能待实现");
    }

    /// <summary>
    /// 订单抵扣积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <param name="points">使用积分数</param>
    /// <returns>抵扣金额</returns>
    public async Task<decimal> DeductOrderPointsAsync(string memberId, string orderId, int points)
    {
        if (points <= 0)
        {
            throw BusinessException.BadRequest("使用积分数必须大于0");
        }

        // 计算抵扣金额（100 积分 = 1 元）
        var deductionAmount = points / 100m;

        // 使用事务
        await _db.Ado.UseTranAsync(async () =>
        {
            // 查询会员
            var member = await _db.Queryable<Member>()
                .Where(m => m.Id == Guid.Parse(memberId) && !m.IsDeleted)
                .FirstAsync();

            if (member == null)
            {
                throw BusinessException.NotFound("会员不存在");
            }

            // 检查积分余额
            if (member.Points < points)
            {
                throw BusinessException.BadRequest("积分余额不足");
            }

            // 冻结积分
            member.Points -= points;
            // TODO: 添加冻结积分字段
            member.UpdateTime = DateTime.Now;

            await _db.Updateable(member).ExecuteCommandAsync();

            // 创建积分流水（冻结）
            var record = new PointRecord
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                Type = PointType.Frozen,
                Source = PointSource.OrderDeduction,
                Points = -points,
                Balance = member.Points,
                Remark = $"订单抵扣积分冻结，订单号：{orderId}",
                RelatedId = orderId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            await _db.Insertable(record).ExecuteCommandAsync();
        });

        _logger.LogInformation("订单抵扣积分成功，会员ID：{MemberId}，订单号：{OrderId}，积分数：{Points}，抵扣金额：{DeductionAmount}", memberId, orderId, points, deductionAmount);

        return deductionAmount;
    }

    /// <summary>
    /// 取消订单解冻积分
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UnfreezeOrderPointsAsync(string memberId, string orderId)
    {
        // 查询冻结的积分流水
        var frozenRecord = await _db.Queryable<PointRecord>()
            .Where(r => r.MemberId == memberId && r.RelatedId == orderId && r.Type == PointType.Frozen && !r.IsDeleted)
            .FirstAsync();

        if (frozenRecord == null)
        {
            _logger.LogWarning("未找到冻结的积分记录，会员ID：{MemberId}，订单号：{OrderId}", memberId, orderId);
            return false;
        }

        var points = Math.Abs(frozenRecord.Points);

        // 使用事务
        await _db.Ado.UseTranAsync(async () =>
        {
            // 查询会员
            var member = await _db.Queryable<Member>()
                .Where(m => m.Id == Guid.Parse(memberId) && !m.IsDeleted)
                .FirstAsync();

            if (member == null)
            {
                throw BusinessException.NotFound("会员不存在");
            }

            // 解冻积分
            member.Points += points;
            // TODO: 减少冻结积分字段
            member.UpdateTime = DateTime.Now;

            await _db.Updateable(member).ExecuteCommandAsync();

            // 创建积分流水（解冻）
            var record = new PointRecord
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                Type = PointType.Unfrozen,
                Source = PointSource.OrderDeduction,
                Points = points,
                Balance = member.Points,
                Remark = $"取消订单解冻积分，订单号：{orderId}",
                RelatedId = orderId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            await _db.Insertable(record).ExecuteCommandAsync();
        });

        _logger.LogInformation("取消订单解冻积分成功，会员ID：{MemberId}，订单号：{OrderId}，积分数：{Points}", memberId, orderId, points);

        return true;
    }

    #endregion

    #region 积分查询

    /// <summary>
    /// 获取会员积分余额
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>积分余额信息</returns>
    public async Task<PointBalanceDto> GetMemberBalanceAsync(string memberId)
    {
        var member = await _db.Queryable<Member>()
            .Where(m => m.Id == Guid.Parse(memberId) && !m.IsDeleted)
            .FirstAsync();

        if (member == null)
        {
            throw BusinessException.NotFound("会员不存在");
        }

        return new PointBalanceDto
        {
            MemberId = memberId,
            TotalPoints = member.TotalPoints,
            AvailablePoints = member.Points,
            FrozenPoints = 0 // TODO: 添加冻结积分字段
        };
    }

    /// <summary>
    /// 分页查询积分流水
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>积分流水分页列表</returns>
    public async Task<PageResponse<PointRecordDto>> GetRecordListAsync(PointRecordQueryDto query)
    {
        var queryable = _db.Queryable<PointRecord>()
            .Where(r => !r.IsDeleted);

        // 会员ID筛选
        if (!string.IsNullOrWhiteSpace(query.MemberId))
        {
            queryable = queryable.Where(r => r.MemberId == query.MemberId);
        }

        // 类型筛选
        if (query.Type.HasValue)
        {
            queryable = queryable.Where(r => r.Type == query.Type.Value);
        }

        // 来源筛选
        if (query.Source.HasValue)
        {
            queryable = queryable.Where(r => r.Source == query.Source.Value);
        }

        // 时间范围筛选
        if (query.StartTime.HasValue)
        {
            queryable = queryable.Where(r => r.CreateTime >= query.StartTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(r => r.CreateTime <= query.EndTime.Value);
        }

        // 排序
        queryable = queryable.OrderBy(r => r.CreateTime, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 映射 DTO
        var result = list.Adapt<List<PointRecordDto>>();

        return PageResponse<PointRecordDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取积分兑换记录
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>兑换记录分页列表</returns>
    public async Task<PageResponse<PointExchangeDto>> GetExchangeListAsync(string memberId, int pageIndex = 1, int pageSize = 10)
    {
        var queryable = _db.Queryable<PointExchange>()
            .Where(e => e.MemberId == memberId && !e.IsDeleted)
            .OrderBy(e => e.CreateTime, OrderByType.Desc);

        RefAsync<int> total = 0;
        var list = await queryable.ToPageListAsync(pageIndex, pageSize, total);

        var result = list.Adapt<List<PointExchangeDto>>();

        return PageResponse<PointExchangeDto>.Create(result, total.Value, pageIndex, pageSize);
    }

    #endregion

    #region 积分统计

    /// <summary>
    /// 获取会员积分统计
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>统计信息</returns>
    public async Task<Dictionary<string, object>> GetMemberPointsStatisticsAsync(string memberId)
    {
        var balance = await GetMemberBalanceAsync(memberId);

        // 统计各类积分
        var incomeTotal = await _db.Queryable<PointRecord>()
            .Where(r => r.MemberId == memberId && r.Type == PointType.Income && !r.IsDeleted)
            .SumAsync(r => r.Points);

        var expenseTotal = await _db.Queryable<PointRecord>()
            .Where(r => r.MemberId == memberId && r.Type == PointType.Expense && !r.IsDeleted)
            .SumAsync(r => Math.Abs(r.Points));

        return new Dictionary<string, object>
        {
            { "totalPoints", balance.TotalPoints },
            { "availablePoints", balance.AvailablePoints },
            { "frozenPoints", balance.FrozenPoints },
            { "incomeTotal", incomeTotal },
            { "expenseTotal", expenseTotal }
        };
    }

    #endregion
}