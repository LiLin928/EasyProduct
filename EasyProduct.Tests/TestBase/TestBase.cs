using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Xunit;

namespace EasyProduct.Tests.TestBase;

/// <summary>
/// 测试基类，提供统一的测试设置和清理
/// </summary>
/// <remarks>
/// 提供以下功能：
/// 1. In-Memory SQLite 数据库配置
/// 2. 依赖注入容器配置
/// 3. 数据库表自动创建
/// 4. 资源自动释放
///
/// 使用示例：
/// <code>
/// public class OrderServiceTests : TestBase
/// {
///     private readonly OrderService _orderService;
///
///     public OrderServiceTests()
///     {
///         _orderService = new OrderService(_db);
///     }
///
///     [Fact]
///     public async Task CreateOrder_ShouldReturnOrder()
///     {
///         // 测试代码...
///     }
/// }
/// </code>
/// </remarks>
public class TestBase : IDisposable
{
    /// <summary>
    /// SqlSugar 数据库客户端
    /// </summary>
    protected readonly ISqlSugarClient _db;

    /// <summary>
    /// 依赖注入服务提供者
    /// </summary>
    protected readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 测试数据工厂
    /// </summary>
    protected readonly TestData.TestDataFactory _dataFactory;

    /// <summary>
    /// 构造函数，初始化测试环境
    /// </summary>
    public TestBase()
    {
        // 配置 In-Memory SQLite 数据库
        _db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = "DataSource=:memory:",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = false,
            InitKeyType = InitKeyType.Attribute
        });

        // 配置依赖注入
        var services = new ServiceCollection();
        services.AddSingleton(_db);

        // 注册其他服务（根据需要添加）
        // services.AddScoped<IOrderService, OrderService>();

        _serviceProvider = services.BuildServiceProvider();

        // 初始化测试数据工厂
        _dataFactory = new TestData.TestDataFactory(_db);

        // 创建数据库表（根据需要添加实体类型）
        InitializeDatabase();
    }

    /// <summary>
    /// 初始化数据库，创建必要的表结构
    /// </summary>
    /// <remarks>
    /// 根据测试需要，创建对应的数据库表。
    /// 可以在子类中重写此方法以创建特定的表。
    /// </remarks>
    protected virtual void InitializeDatabase()
    {
        // 创建基础表（根据实际需要添加实体类型）
        // _db.CodeFirst.InitTables<Order, Payment, OrderItem>();
    }

    /// <summary>
    /// 释放资源，清理测试环境
    /// </summary>
    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}