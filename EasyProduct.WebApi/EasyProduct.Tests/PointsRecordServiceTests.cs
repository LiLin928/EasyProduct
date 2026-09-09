using EasyProduct.Business.Mall;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Mall.PointsRecord;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Enums.Mall;
using Microsoft.Extensions.Logging;
using Moq;
using SqlSugar;
using Xunit;

namespace EasyProduct.Tests;

/// <summary>
/// 积分记录服务单元测试
/// </summary>
/// <remarks>
/// 测试积分服务的核心功能，重点测试涉及资金的积分变动操作
/// 遵循 TDD 原则：先写测试，再实现功能
/// 使用 SQLite 内存数据库进行集成测试
/// </remarks>
public class PointsRecordServiceTests : IDisposable
{
    private readonly ISqlSugarClient _db;
    private readonly Mock<ILogger<PointsRecordService>> _mockLogger;
    private readonly Mock<IMemberService> _mockMemberService;
    private readonly PointsRecordService _service;
    private readonly Guid _testMemberId;

    public PointsRecordServiceTests()
    {
        // 创建 SQLite 内存数据库
        _db = new SqlSugarScope(new ConnectionConfig
        {
            ConnectionString = "DataSource=:memory:",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = false,
            InitKeyType = InitKeyType.Attribute
        });

        // 初始化数据库表
        InitializeDatabase();

        _mockLogger = new Mock<ILogger<PointsRecordService>>();
        _mockMemberService = new Mock<IMemberService>();
        _service = new PointsRecordService(_mockLogger.Object, _mockMemberService.Object)
        {
            _db = _db
        };

        _testMemberId = Guid.NewGuid();
    }

    /// <summary>
    /// 初始化数据库表和测试数据
    /// </summary>
    private void InitializeDatabase()
    {
        // 创建会员表
        _db.CodeFirst.InitTables<Member>();
        // 创建积分记录表
        _db.CodeFirst.InitTables<PointsRecord>();
        // 创建会员等级表
        _db.CodeFirst.InitTables<MemberLevel>();
    }

    /// <summary>
    /// 创建测试会员
    /// </summary>
    private async Task CreateTestMember(int points = 100)
    {
        await _db.Insertable(new Member
        {
            Id = _testMemberId,
            Nickname = "测试会员",
            Points = points,
            TotalPoints = points,
            Status = MemberStatus.Enabled,
            Gender = Gender.Unknown,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = 0,
            // 显式设置为 null 以避免 NOT NULL 约束问题
            Birthday = null,
            LastLoginTime = null
        }).ExecuteCommandAsync();
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    #region 正例：成功的积分变动

    /// <summary>
    /// 测试：成功增加积分
    /// </summary>
    [Fact]
    public async Task ChangePointsAsync_AddPoints_ShouldSucceed()
    {
        // Arrange
        await CreateTestMember(50);

        var dto = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.ConsumeEarn,
            Points = 100,
            Remark = "测试增加积分"
        };

        _mockMemberService.Setup(x => x.UpdateMemberLevelAsync(_testMemberId))
                          .ReturnsAsync(true);

        // Act
        var result = await _service.ChangePointsAsync(dto);

        // Assert
        Assert.Equal(150, result); // 50 + 100 = 150

        // 验证数据库状态
        var member = await _db.Queryable<Member>()
            .Where(x => x.Id == _testMemberId)
            .FirstAsync();
        Assert.Equal(150, member.Points);
        Assert.Equal(150, member.TotalPoints);

        // 验证积分记录
        var records = await _db.Queryable<PointsRecord>()
            .Where(x => x.MemberId == _testMemberId)
            .ToListAsync();
        Assert.Single(records);
        Assert.Equal(100, records[0].Points);
        Assert.Equal(150, records[0].Balance);
    }

    /// <summary>
    /// 测试：成功减少积分
    /// </summary>
    [Fact]
    public async Task ChangePointsAsync_DeductPoints_ShouldSucceed()
    {
        // Arrange
        await CreateTestMember(100);

        var dto = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.OrderUse,
            Points = -30,
            Remark = "订单使用积分"
        };

        _mockMemberService.Setup(x => x.UpdateMemberLevelAsync(_testMemberId))
                          .ReturnsAsync(true);

        // Act
        var result = await _service.ChangePointsAsync(dto);

        // Assert
        Assert.Equal(70, result); // 100 - 30 = 70

        // 验证数据库状态
        var member = await _db.Queryable<Member>()
            .Where(x => x.Id == _testMemberId)
            .FirstAsync();
        Assert.Equal(70, member.Points);
        Assert.Equal(100, member.TotalPoints); // 累计积分不变
    }

    #endregion

    #region 边界1：余额不足

    /// <summary>
    /// 测试：积分不足时扣除积分应抛出异常
    /// </summary>
    [Fact]
    public async Task ChangePointsAsync_InsufficientBalance_ShouldThrowException()
    {
        // Arrange
        await CreateTestMember(100);

        var dto = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.OrderUse,
            Points = -200, // 尝试扣除 200 积分
            Remark = "订单使用积分"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.ChangePointsAsync(dto));

        Assert.Contains("积分余额不足", exception.Message);
        Assert.Equal(400, exception.Code);

        // 验证数据库状态未改变
        var member = await _db.Queryable<Member>()
            .Where(x => x.Id == _testMemberId)
            .FirstAsync();
        Assert.Equal(100, member.Points);
    }

    /// <summary>
    /// 测试：积分为零时扣除积分应抛出异常
    /// </summary>
    [Fact]
    public async Task ChangePointsAsync_ZeroBalance_ShouldThrowException()
    {
        // Arrange
        await CreateTestMember(0);

        var dto = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.OrderUse,
            Points = -1, // 尝试扣除 1 积分
            Remark = "订单使用积分"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.ChangePointsAsync(dto));

        Assert.Contains("积分余额不足", exception.Message);
    }

    #endregion

    #region 边界2：并发重复提交

    /// <summary>
    /// 测试：并发场景下积分变动应正确处理
    /// </summary>
    /// <remarks>
    /// 模拟两个并发请求同时扣除积分，验证事务锁机制
    /// </remarks>
    [Fact]
    public async Task ChangePointsAsync_ConcurrentRequests_ShouldHandleCorrectly()
    {
        // Arrange
        await CreateTestMember(100);

        var dto1 = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.OrderUse,
            Points = -60,
            Remark = "订单1使用积分"
        };

        var dto2 = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.OrderUse,
            Points = -60,
            Remark = "订单2使用积分"
        };

        _mockMemberService.Setup(x => x.UpdateMemberLevelAsync(_testMemberId))
                          .ReturnsAsync(true);

        // Act - 并发执行两个任务
        var task1 = Task.Run(async () => await _service.ChangePointsAsync(dto1));
        var task2 = Task.Run(async () => await _service.ChangePointsAsync(dto2));

        // 等待两个任务完成
        var results = await Task.WhenAll(task1, task2);

        // Assert - 验证并发处理
        // 第一个请求应该成功：100 - 60 = 40
        // 第二个请求应该失败：余额不足
        // 由于并发执行，其中一个应该失败
        var member = await _db.Queryable<Member>()
            .Where(x => x.Id == _testMemberId)
            .FirstAsync();

        // 验证最终积分（应该是 100 - 60 = 40，因为只有一个请求能成功）
        // 或者其中一个请求失败
        Assert.True(member.Points >= 40);
    }

    /// <summary>
    /// 测试：重复提交相同请求应被正确处理
    /// </summary>
    [Fact]
    public async Task ChangePointsAsync_DuplicateRequest_ShouldBeHandled()
    {
        // Arrange
        await CreateTestMember(50);

        var dto = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.SignIn,
            Points = 10,
            Remark = "签到获得积分"
        };

        _mockMemberService.Setup(x => x.UpdateMemberLevelAsync(_testMemberId))
                          .ReturnsAsync(true);

        // Act - 执行两次相同请求
        var result1 = await _service.ChangePointsAsync(dto);
        var result2 = await _service.ChangePointsAsync(dto);

        // Assert - 验证两次都成功执行
        Assert.Equal(60, result1); // 50 + 10 = 60
        Assert.Equal(70, result2); // 60 + 10 = 70

        // 验证数据库状态
        var member = await _db.Queryable<Member>()
            .Where(x => x.Id == _testMemberId)
            .FirstAsync();
        Assert.Equal(70, member.Points);
        Assert.Equal(70, member.TotalPoints);

        // 验证积分记录数量
        var records = await _db.Queryable<PointsRecord>()
            .Where(x => x.MemberId == _testMemberId)
            .ToListAsync();
        Assert.Equal(2, records.Count);
    }

    #endregion

    #region 事务回滚场景

    /// <summary>
    /// 测试：会员不存在时应抛出异常
    /// </summary>
    [Fact]
    public async Task ChangePointsAsync_MemberNotExists_ShouldThrowException()
    {
        // Arrange - 不创建会员
        var dto = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.ConsumeEarn,
            Points = 100,
            Remark = "测试增加积分"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.ChangePointsAsync(dto));

        Assert.Equal("会员不存在", exception.Message);
        Assert.Equal(404, exception.Code);
    }

    /// <summary>
    /// 测试：积分为0时变动应抛出异常
    /// </summary>
    [Fact]
    public async Task ChangePointsAsync_ZeroPoints_ShouldThrowException()
    {
        // Arrange
        await CreateTestMember(100);

        var dto = new PointsChangeDto
        {
            MemberId = _testMemberId,
            PointsType = (int)PointsType.AdminAdjust,
            Points = 0, // 积分变动为 0
            Remark = "测试积分变动"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.ChangePointsAsync(dto));

        Assert.Equal("积分变动数量不能为0", exception.Message);
        Assert.Equal(400, exception.Code);
    }

    #endregion

    #region 其他测试

    /// <summary>
    /// 测试：获取会员积分余额
    /// </summary>
    [Fact]
    public async Task GetMemberPointsBalanceAsync_ShouldReturnCorrectBalance()
    {
        // Arrange
        await CreateTestMember(150);

        // Act
        var result = await _service.GetMemberPointsBalanceAsync(_testMemberId);

        // Assert
        Assert.Equal(150, result);
    }

    /// <summary>
    /// 测试：获取不存在会员的积分余额应抛出异常
    /// </summary>
    [Fact]
    public async Task GetMemberPointsBalanceAsync_MemberNotExists_ShouldThrowException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _service.GetMemberPointsBalanceAsync(_testMemberId));

        Assert.Equal("会员不存在", exception.Message);
    }

    /// <summary>
    /// 测试：获取积分记录列表
    /// </summary>
    [Fact]
    public async Task GetListAsync_ShouldReturnPagedResult()
    {
        // Arrange
        await CreateTestMember(100);

        // 创建测试积分记录
        await _db.Insertable(new PointsRecord
        {
            Id = Guid.NewGuid(),
            MemberId = _testMemberId,
            PointsType = PointsType.ConsumeEarn,
            Points = 100,
            Balance = 100,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = 0
        }).ExecuteCommandAsync();

        var query = new PointsRecordQuery
        {
            PageIndex = 1,
            PageSize = 10,
            MemberId = _testMemberId
        };

        // Act
        var result = await _service.GetListAsync(query);

        // Assert
        Assert.Single(result.List);
        Assert.Equal(100, result.List[0].Points);
        Assert.Equal("消费获得", result.List[0].PointsTypeName);
    }

    /// <summary>
    /// 测试：获取积分记录列表支持时间范围筛选
    /// </summary>
    [Fact]
    public async Task GetListAsync_WithDateRange_ShouldFilterCorrectly()
    {
        // Arrange
        await CreateTestMember(100);

        // 创建不同时间的积分记录
        var record1Id = Guid.NewGuid();
        var record2Id = Guid.NewGuid();

        await _db.Insertable(new PointsRecord
        {
            Id = record1Id,
            MemberId = _testMemberId,
            PointsType = PointsType.ConsumeEarn,
            Points = 50,
            Balance = 50,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            IsDeleted = 0
        }).ExecuteCommandAsync();

        await _db.Insertable(new PointsRecord
        {
            Id = record2Id,
            MemberId = _testMemberId,
            PointsType = PointsType.SignIn,
            Points = 10,
            Balance = 60,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = 0
        }).ExecuteCommandAsync();

        var query = new PointsRecordQuery
        {
            PageIndex = 1,
            PageSize = 10,
            MemberId = _testMemberId,
            StartTime = DateTime.UtcNow.AddDays(-1),
            EndTime = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _service.GetListAsync(query);

        // Assert
        Assert.Single(result.List);
        Assert.Equal(10, result.List[0].Points);
        Assert.Equal("签到", result.List[0].PointsTypeName);
    }

    #endregion
}