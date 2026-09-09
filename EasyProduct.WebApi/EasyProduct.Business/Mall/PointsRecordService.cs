using Mapster;
using SqlSugar;
using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.PointsRecord;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Microsoft.Extensions.Logging;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 积分记录服务实现
/// </summary>
/// <remarks>
/// 提供积分记录的查询、积分变动、余额查询等功能
/// 继承 BaseService，使用属性注入的数据库上下文
/// 积分变动操作使用事务确保数据一致性
/// </remarks>
public class PointsRecordService : BaseService, IPointsRecordService
{
    private readonly ILogger<PointsRecordService> _logger;
    private readonly IMemberService _memberService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    /// <param name="memberService">会员服务</param>
    public PointsRecordService(
        ILogger<PointsRecordService> logger,
        IMemberService memberService)
    {
        _logger = logger;
        _memberService = memberService;
    }

    /// <summary>
    /// 获取积分记录分页列表
    /// </summary>
    /// <param name="query">查询参数，包含会员ID、积分类型、时间范围、分页信息</param>
    /// <returns>积分记录分页结果</returns>
    public async Task<PageResponse<PointsRecordDto>> GetListAsync(PointsRecordQuery query)
    {
        // 构建查询：关联会员表获取昵称
        var queryable = _db.Queryable<PointsRecord, Member>(
            (pr, m) => new JoinQueryInfos(
                JoinType.Left, pr.MemberId == m.Id
            ))
            .Where((pr, m) => pr.IsDeleted == 0);

        // 会员ID筛选
        if (query.MemberId.HasValue)
        {
            queryable = queryable.Where((pr, m) => pr.MemberId == query.MemberId.Value);
        }

        // 积分类型筛选
        if (query.PointsType.HasValue)
        {
            queryable = queryable.Where((pr, m) => (int)pr.PointsType == query.PointsType.Value);
        }

        // 时间范围筛选
        if (query.StartTime.HasValue)
        {
            queryable = queryable.Where((pr, m) => pr.CreatedAt >= query.StartTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where((pr, m) => pr.CreatedAt <= query.EndTime.Value);
        }

        // 按创建时间倒序排列
        queryable = queryable.OrderBy((pr, m) => pr.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var list = await queryable
            .Select((pr, m) => new PointsRecordDto
            {
                Id = pr.Id.ToString(),
                MemberId = pr.MemberId.ToString(),
                MemberNickname = m.Nickname,
                PointsType = (int)pr.PointsType,
                PointsTypeName = GetPointsTypeName(pr.PointsType),
                Points = pr.Points,
                Balance = pr.Balance,
                OrderNo = pr.OrderNo,
                Remark = pr.Remark,
                CreateTime = pr.CreatedAt,
                UpdateTime = pr.UpdatedAt
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<PointsRecordDto>.Create(
            list,
            total.Value,
            query.PageIndex,
            query.PageSize);
    }

    /// <summary>
    /// 积分变动
    /// </summary>
    /// <param name="dto">积分变动参数，包含会员ID、积分类型、变动数量、备注</param>
    /// <returns>变动后的积分余额</returns>
    /// <remarks>
    /// 此操作涉及资金流水，使用事务确保数据一致性
    /// 正数为增加积分，负数为减少积分
    /// 减少积分时，会检查余额是否充足
    /// 变动后会自动更新会员的当前积分和累计积分
    /// 变动后会自动更新会员等级
    /// </remarks>
    public async Task<int> ChangePointsAsync(PointsChangeDto dto)
    {
        // 1. 参数验证
        if (dto.Points == 0)
        {
            throw new BusinessException("积分变动数量不能为0", 400);
        }

        // 2. 检查会员是否存在
        var member = await _db.Queryable<Member>()
            .Where(x => x.Id == dto.MemberId && x.IsDeleted == 0)
            .FirstAsync();

        if (member == null)
        {
            throw new BusinessException("会员不存在", 404);
        }

        // 3. 使用事务执行积分变动
        int newBalance = 0;

        await _db.Ado.UseTranAsync(async () =>
        {
            // 3.1 锁定会员记录（防止并发问题）
            var lockedMember = await _db.Queryable<Member>()
                .Where(x => x.Id == dto.MemberId && x.IsDeleted == 0)
                .With(SqlWith.RowLock)
                .FirstAsync();

            if (lockedMember == null)
            {
                throw new BusinessException("会员不存在", 404);
            }

            // 3.2 验证余额（减少积分时检查）
            if (dto.Points < 0)
            {
                if (lockedMember.Points + dto.Points < 0)
                {
                    throw new BusinessException($"积分余额不足，当前余额：{lockedMember.Points}，需要扣除：{Math.Abs(dto.Points)}", 400);
                }
            }

            // 3.3 更新会员积分
            var oldPoints = lockedMember.Points;
            newBalance = oldPoints + dto.Points;

            lockedMember.Points = newBalance;

            // 如果是增加积分，累计积分也要增加
            if (dto.Points > 0)
            {
                lockedMember.TotalPoints += dto.Points;
            }

            lockedMember.UpdatedAt = DateTime.UtcNow;

            await _db.Updateable(lockedMember)
                .UpdateColumns(x => new { x.Points, x.TotalPoints, x.UpdatedAt })
                .ExecuteCommandAsync();

            // 3.4 插入积分记录
            var record = new PointsRecord
            {
                Id = Guid.NewGuid(),
                MemberId = dto.MemberId,
                PointsType = (PointsType)dto.PointsType,
                Points = dto.Points,
                Balance = newBalance,
                Remark = dto.Remark,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = 0
            };

            await _db.Insertable(record).ExecuteCommandAsync();

            _logger.LogInformation(
                "积分变动成功：会员ID={MemberId}，变动前={OldPoints}，变动={Points}，变动后={NewBalance}，类型={PointsType}",
                dto.MemberId, oldPoints, dto.Points, newBalance, dto.PointsType);
        });

        // 4. 更新会员等级（在事务外执行，避免长时间锁定）
        try
        {
            await _memberService.UpdateMemberLevelAsync(dto.MemberId);
        }
        catch (Exception ex)
        {
            // 等级更新失败不影响积分变动，只记录日志
            _logger.LogWarning(ex, "更新会员等级失败：会员ID={MemberId}", dto.MemberId);
        }

        return newBalance;
    }

    /// <summary>
    /// 获取会员当前积分余额
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>当前积分余额</returns>
    public async Task<int> GetMemberPointsBalanceAsync(Guid memberId)
    {
        var member = await _db.Queryable<Member>()
            .Where(x => x.Id == memberId && x.IsDeleted == 0)
            .FirstAsync();

        if (member == null)
        {
            throw new BusinessException("会员不存在", 404);
        }

        return member.Points;
    }

    /// <summary>
    /// 获取积分类型名称
    /// </summary>
    /// <param name="pointsType">积分类型枚举</param>
    /// <returns>积分类型中文名称</returns>
    private static string GetPointsTypeName(PointsType pointsType)
    {
        return pointsType switch
        {
            PointsType.ConsumeEarn => "消费获得",
            PointsType.OrderUse => "订单使用",
            PointsType.AdminAdjust => "后台调整",
            PointsType.SignIn => "签到",
            PointsType.RegisterGift => "注册赠送",
            _ => "未知类型"
        };
    }
}